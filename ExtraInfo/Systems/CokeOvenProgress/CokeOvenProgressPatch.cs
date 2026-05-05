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
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowCokeOvenProgress) return;
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

            StringBuilder sb = new(__result);
            sb.AppendLine().AppendLine();

            TimeBasedProgressBarProperties barProps = new()
            {
                HeaderKey = Lang.Get("extrainfo:WillFinishIn"),
                HoursTotal = totalBurnHours,
                HoursLeft = hoursRemaining,

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            string verticalBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(world.Api, barProps);
            sb.Append(verticalBlock);

            __result = sb.ToString().TrimEnd();
            break;
        }
    }
}