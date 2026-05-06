using HarmonyLib;
using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.ClayformingPlacementPreview;

[HarmonyPatch(typeof(ClayFormRenderer), nameof(ClayFormRenderer.OnRenderFrame))]
public class ClayformingPlacementPreviewPatch
{
    private static Dictionary<BlockPos, MeshRef?> previewMeshRefByPos = [];
    private static int lastVoxelX = -1, lastVoxelZ = -1, lastLayer = -1;
    private static Size2i? lastVoxelSize;
    private const float extraHeight = 0.0075f;

    public static void Postfix(ClayFormRenderer __instance, float deltaTime, EnumRenderStage stage, ICoreClientAPI ___api, BlockPos ___pos)
    {
        if (!Config.ShowClayformingPlacementPreview) return;
        if (stage != EnumRenderStage.AfterFinalComposition || ___api == null) return;

        if (___api.World.BlockAccessor.GetBlockEntity(___pos) is not BlockEntityClayForm beclayform)
        {
            ClearPreview(___pos);
            return;
        }

        IClientPlayer player = ___api.World.Player;
        BlockSelection? blockSel = player.CurrentBlockSelection;
        ItemSlot? slot = player.InventoryManager?.ActiveHotbarSlot;

        if (blockSel == null || !blockSel.Position.Equals(___pos) || slot?.Itemstack?.Collectible is not ItemClay itemClay)
        {
            ClearPreview(___pos);
            return;
        }

        Size2i? voxelSize = GetVoxelSize(itemClay, slot, player, blockSel);
        if (voxelSize == null) return;

        Vec3i hitVoxel = GetHitVoxel(blockSel);
        int currentRecipeLayer = GetCurrentLayer(beclayform);

        if (ShouldUpdatePreview(hitVoxel.X, hitVoxel.Z, currentRecipeLayer, voxelSize))
        {
            UpdatePreviewMesh(___pos, ___api, hitVoxel.X, currentRecipeLayer, hitVoxel.Z, voxelSize);
        }

        RenderPreview(__instance, ___api, ___pos);
    }

    private static void RenderPreview(ClayFormRenderer instance, ICoreClientAPI api, BlockPos pos)
    {
        if (!previewMeshRefByPos.TryGetValue(pos, out MeshRef? meshRef) || meshRef == null || meshRef.Disposed) return;

        var modelMat = instance.GetField<Matrixf>("ModelMat");
        var origin = instance.GetField<Vec3f>("origin");

        IRenderAPI rpi = api.Render;
        rpi.GlToggleBlend(true);

        IShaderProgram prog = rpi.GetEngineShader(EnumShaderProgram.Wireframe);
        prog.Use();
        prog.Uniform("origin", origin);
        prog.UniformMatrix("projectionMatrix", rpi.CurrentProjectionMatrix);
        prog.UniformMatrix("modelViewMatrix", modelMat.Values);

        rpi.RenderMesh(meshRef);

        prog.Stop();
    }

    private static void UpdatePreviewMesh(BlockPos pos, ICoreClientAPI api, int centerX, int recipeLayer, int centerZ, Size2i voxelSize)
    {
        MeshData newPreviewMesh = GenerateWireframeMesh(api, centerX, recipeLayer, centerZ, voxelSize);

        if (newPreviewMesh.VerticesCount > 0)
        {
            MeshRef newPreviewMeshRef = api.Render.UploadMesh(newPreviewMesh);
            if (previewMeshRefByPos.TryGetValue(pos, out MeshRef? oldPreviewMeshRef))
            {
                api.Event.EnqueueMainThreadTask(() => oldPreviewMeshRef?.Dispose(), "disposemesh");
            }
            previewMeshRefByPos[pos] = newPreviewMeshRef;
        }
    }

    private static bool ShouldUpdatePreview(int voxelX, int voxelZ, int recipeLayer, Size2i voxelSize)
    {
        if (voxelX == lastVoxelX && voxelZ == lastVoxelZ && recipeLayer == lastLayer && SameSize(voxelSize, lastVoxelSize))
        {
            return false;
        }

        lastVoxelX = voxelX;
        lastVoxelZ = voxelZ;
        lastLayer = recipeLayer;
        lastVoxelSize = voxelSize;
        return true;
    }

    private static int GetCurrentLayer(BlockEntityClayForm be)
    {
        int recipeLayer = be.CallMethod<int>("NextNotMatchingRecipeLayer", 0);
        return Math.Min(recipeLayer, 15);
    }

    private static Vec3i GetHitVoxel(BlockSelection blockSel)
    {
        return new Vec3i()
        {
            X = GameMath.Clamp((int)(blockSel.HitPosition.X * 16), 0, 15),
            Y = 0,
            Z = GameMath.Clamp((int)(blockSel.HitPosition.Z * 16), 0, 15)
        };
    }

    private static Size2i? GetVoxelSize(ItemClay item, ItemSlot slot, IPlayer player, BlockSelection blockSel)
    {
        return item.GetToolMode(slot, player, blockSel) switch
        {
            0 => new Size2i(1, 1),
            1 => new Size2i(2, 2),
            2 => new Size2i(3, 3),
            _ => null
        };
    }

    private static void ClearPreview(BlockPos pos)
    {
        if (previewMeshRefByPos.TryGetValue(pos, out MeshRef? mesh))
        {
            mesh?.Dispose();
            previewMeshRefByPos.Remove(pos);
        }
        lastVoxelX = -1;
        lastVoxelSize = null;
    }

    private static bool SameSize(Size2i? sizeA, Size2i? sizeB)
    {
        if (sizeA == null || sizeB == null) return false;
        return sizeA.Width == sizeB.Width && sizeA.Height == sizeB.Height;
    }

    private static MeshData GenerateWireframeMesh(ICoreClientAPI api, int centerX, int recipeLayer, int centerZ, Size2i voxelSize)
    {
        MeshData previewMesh = new MeshData(24, 36, withNormals: false, withUv: false, withRgba: true, withFlags: false);
        previewMesh.SetMode(EnumDrawMode.Lines);

        byte[] rgba = Config.ClayformingPlacementPreviewColor;
        int argbColor = ColorUtil.ToRgba(rgba[3], rgba[0], rgba[1], rgba[2]);

        MeshData voxelMesh = LineMeshUtil.GetCube(argbColor);

        NormalizeMeshToVoxels(voxelMesh);

        int lowX = -(voxelSize.Width / 2), highX = lowX + voxelSize.Width - 1;
        int lowZ = -(voxelSize.Height / 2), highZ = lowZ + voxelSize.Height - 1;

        for (int deltaX = lowX; deltaX <= highX; deltaX++)
        {
            for (int deltaZ = lowZ; deltaZ <= highZ; deltaZ++)
            {
                int x = centerX + deltaX, z = centerZ + deltaZ;
                if (x < 0 || x >= 16 || z < 0 || z >= 16) continue;

                AddVoxelToMesh(previewMesh, voxelMesh, x, recipeLayer, z);
            }
        }
        return previewMesh;
    }

    private static void AddVoxelToMesh(MeshData previewMesh, MeshData voxelMesh, int x, int recipeLayer, int z)
    {
        MeshData newVoxel = voxelMesh.Clone();
        for (int i = 0; i < newVoxel.xyz.Length; i += 3)
        {
            newVoxel.xyz[i] += x / 16f;
            newVoxel.xyz[i + 1] += (recipeLayer / 16f) + extraHeight;
            newVoxel.xyz[i + 2] += z / 16f;
        }
        previewMesh.AddMeshData(newVoxel);
    }

    private static void NormalizeMeshToVoxels(MeshData voxelMesh)
    {
        if (voxelMesh.xyz == null || voxelMesh.xyz.Length == 0) return;

        float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

        for (int i = 0; i < voxelMesh.xyz.Length; i += 3)
        {
            minX = Math.Min(minX, voxelMesh.xyz[i]);
            minY = Math.Min(minY, voxelMesh.xyz[i + 1]);
            minZ = Math.Min(minZ, voxelMesh.xyz[i + 2]);
            maxX = Math.Max(maxX, voxelMesh.xyz[i]);
            maxY = Math.Max(maxY, voxelMesh.xyz[i + 1]);
            maxZ = Math.Max(maxZ, voxelMesh.xyz[i + 2]);
        }

        float width = maxX - minX;
        float height = maxY - minY;
        float depth = maxZ - minZ;

        if (width == 0) width = 1;
        if (height == 0) height = 1;
        if (depth == 0) depth = 1;

        for (int i = 0; i < voxelMesh.xyz.Length; i += 3)
        {
            voxelMesh.xyz[i] = (voxelMesh.xyz[i] - minX) / width / 16f;
            voxelMesh.xyz[i + 1] = (voxelMesh.xyz[i + 1] - minY) / height / 16f;
            voxelMesh.xyz[i + 2] = (voxelMesh.xyz[i + 2] - minZ) / depth / 16f;
        }
    }
}