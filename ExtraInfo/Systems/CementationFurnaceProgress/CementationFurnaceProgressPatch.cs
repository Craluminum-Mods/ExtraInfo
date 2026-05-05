using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.CementationFurnaceProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CementationFurnaceProgressPatch
{
    private const double TotalCarburizationHours = 159.2;

    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Config?.ShowCementationFurnaceProgress != true) return;

        Block block = world.BlockAccessor.GetBlock(pos);
        if (block?.Code?.Path == null || !block.Code.Path.Contains("door")) return;

        BlockPos[] neighborPositions = [pos.NorthCopy(3), pos.EastCopy(3), pos.SouthCopy(3), pos.WestCopy(3)];

        foreach (BlockPos coffinPos in neighborPositions)
        {
            if (world.BlockAccessor.GetBlockEntity(coffinPos) is not BlockEntityStoneCoffin be || !be.StructureComplete) continue;

            bool processComplete = be.GetField<bool>("processComplete");
            double progress = be.GetField<double>("progress");
            bool receivesHeat = be.GetField<bool>("receivesHeat");

            StringBuilder sb = new(__result);
            bool barAdded = false;

            TimeBasedProgressBarProperties barProps = new();

            if (processComplete)
            {
                barProps.HeaderKey = Lang.Get("Carburization process complete. Break to retrieve blister steel.");
                barProps.HoursTotal = TotalCarburizationHours;
                barProps.HoursLeft = 0;
            }
            else if (progress > 0.0 || receivesHeat)
            {
                barProps.HeaderKey = Lang.Get(receivesHeat ? "extrainfo:WillFinishIn" : "Out of fuel.");
                barProps.HoursTotal = TotalCarburizationHours;
                barProps.HoursLeft = Math.Max(0, (1.0 - progress) * 160.0);
            }

            if (barProps.HoursTotal > 0)
            {
                sb.AppendLine().AppendLine();
                sb.AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(world.Api, barProps));
                barAdded = true;
            }

            if (Config.ShowFuelProgress && be.FuelPositions?.Length > 0)
            {
                if (world.BlockAccessor.GetBlockEntity(be.FuelPositions.FirstOrDefault()) is BlockEntityCoalPile fuelPile)
                {
                    if (fuelPile.IsBurning)
                    {
                        InfoExtensions.AppendFuelProgress(sb, fuelPile, Lang.Get("extrainfo:Fuel"));
                    }
                    else if (!processComplete)
                    {
                        if (!barAdded) sb.AppendLine().AppendLine();
                        else sb.AppendLine();
                        
                        sb.Append($"{Lang.Get("extrainfo:Fuel")}: {Lang.Get("Not burning")}");
                    }
                }
            }

            __result = sb.ToString().TrimEnd();
            break;
        }
    }
}