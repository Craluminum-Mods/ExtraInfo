using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.CokeOvenProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CokeOvenProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Config?.ShowCokeOvenProgress != true) return;

        if (world.BlockAccessor.GetBlock(pos) is not BlockCokeOvenDoor) return;

        BlockPos[] positions = [pos.NorthCopy(), pos.EastCopy(), pos.SouthCopy(), pos.WestCopy()];

        foreach (BlockPos neighborPos in positions)
        {
            if (world.BlockAccessor.GetBlockEntity(neighborPos) is not BlockEntityCoalPile blockEntity) continue;

            if (!blockEntity.IsBurning) continue;

            double totalBurnHours = blockEntity.Layers * blockEntity.BurnHoursPerLayer;

            double burnStart = blockEntity.GetField<double>("burnStartTotalHours");
            double hoursElapsed = world.Calendar.TotalHours - burnStart;
            double hoursRemaining = Math.Max(0, totalBurnHours - hoursElapsed);

            float completedPercent = (float)Math.Clamp((hoursElapsed / totalBurnHours) * 100, 0, 100);

            StringBuilder sb = new(__result);
            sb.AppendLine();

            string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
                igSeconds: hoursRemaining * 3600,
                speedOfTime: world.Calendar.SpeedOfTime,
                completedPercent: completedPercent,
                headerKey: Lang.Get("extrainfo:WillFinishIn")
            );

            sb.Append(verticalBlock);

            __result = sb.ToString().TrimEnd();
            break;
        }
    }
}