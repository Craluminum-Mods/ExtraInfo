using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.CementationFurnaceProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CementationFurnaceProgressPatch
{
    private const double TotalCarburizationHours = 160.0;

    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Config?.ShowCementationFurnaceProgress != true) return;

        Block block = world.BlockAccessor.GetBlock(pos);
        if (block?.Code?.Path == null || !block.Code.Path.Contains("door")) return;

        BlockPos[] neighborPositions = [pos.NorthCopy(3), pos.EastCopy(3), pos.SouthCopy(3), pos.WestCopy(3)];

        foreach (BlockPos coffinPos in neighborPositions)
        {
            if (world.BlockAccessor.GetBlockEntity(coffinPos) is not BlockEntityStoneCoffin be) continue;

            if (!be.StructureComplete) continue;

            bool processComplete = be.GetField<bool>("processComplete");
            double progress = be.GetField<double>("progress");
            bool receivesHeat = be.GetField<bool>("receivesHeat");

            StringBuilder sb = new(__result);
            sb.AppendLine().AppendLine();

            if (processComplete)
            {
                sb.AppendLine(TimeFormatter.BuildVerticalTimeBlock(
                    igSeconds: 0,
                    speedOfTime: world.Calendar.SpeedOfTime,
                    completedPercent: 100f,
                    headerKey: Lang.Get("Carburization process complete. Break to retrieve blister steel.")
                ));
            }
            else if (progress > 0.0 || receivesHeat)
            {
                float completedPercent = (float)Math.Clamp(progress * 100.0, 0, 100);

                double hoursRemaining = Math.Max(0, (1.0 - progress) * TotalCarburizationHours);

                string statusKey = receivesHeat ? "extrainfo:WillFinishIn" : "Out of fuel.";
                double displaySeconds = receivesHeat ? hoursRemaining * 3600 : 0;

                sb.AppendLine(TimeFormatter.BuildVerticalTimeBlock(
                    igSeconds: displaySeconds,
                    speedOfTime: world.Calendar.SpeedOfTime,
                    completedPercent: completedPercent,
                    headerKey: Lang.Get(statusKey)
                ));
            }

            if (be.FuelPositions != null && be.FuelPositions.Length > 0)
            {
                if (world.BlockAccessor.GetBlockEntity(be.FuelPositions[0]) is BlockEntityCoalPile fuelPile)
                {
                    if (fuelPile.IsBurning)
                    {
                        InfoExtensions.AppendFuelProgress(sb, fuelPile, Lang.Get("Fuel"));
                    }
                    else if (!processComplete)
                    {
                        sb.AppendLine().Append($"{Lang.Get("Fuel")}: {Lang.Get("Not burning")}");
                    }
                }
            }

            __result = sb.ToString().TrimEnd();
            break;
        }
    }
}