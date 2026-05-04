using HarmonyLib;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.TranslocatorDestination;

[HarmonyPatch(typeof(BlockEntityStaticTranslocator), nameof(BlockEntityStaticTranslocator.GetBlockInfo))]
public static class TranslocatorDestinationPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityStaticTranslocator __instance, StringBuilder dsc)
    {
        if (Config?.ShowTranslocatorDestination != true) return;
        if (__instance?.tpLocation == null) return;

        BlockPos pos = __instance.Api.World.DefaultSpawnPosition.AsBlockPos;
        BlockPos targetpos = __instance.tpLocation.Copy().Sub(pos.X, 0, pos.Z);
        if (__instance.tpLocationIsOffset)
        {
            targetpos.Add(__instance.Pos.X, pos.Y, pos.Z);
        }
        dsc.AppendLine(Lang.Get("Teleports to {0}", targetpos));
    }
}