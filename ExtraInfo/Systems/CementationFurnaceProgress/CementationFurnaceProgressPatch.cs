namespace ExtraInfo.Systems.CementationFurnaceProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CementationFurnaceProgressPatch
{
    private const double TotalCarburizationHours = 40.0;

    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config?.ShowCementationFurnaceProgress != true) return;

        Block block = world.BlockAccessor.GetBlock(pos);
        if (block == null) return;

        string codePath = block.Code.Path;
        if (codePath == null || !codePath.Contains("door")) return;

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
                        double burnStart = fuelPile.GetField<double>("burnStartTotalHours");
                        double hoursElapsed = Math.Max(0, world.Calendar.TotalHours - burnStart);

                        double layerProgress = Math.Clamp(hoursElapsed / fuelPile.BurnHoursPerLayer, 0, 1);

                        double effectiveLayersLeft = fuelPile.Layers - layerProgress;

                        double maxLayers = 8.0;

                        float totalFuelPercent = (float)Math.Clamp((effectiveLayersLeft / maxLayers) * 100, 0, 100);

                        double totalRemainingHours = effectiveLayersLeft * fuelPile.BurnHoursPerLayer;

                        sb.AppendLine();
                        sb.Append(TimeFormatter.BuildVerticalTimeBlock(
                            igSeconds: totalRemainingHours * 3600,
                            speedOfTime: world.Calendar.SpeedOfTime,
                            completedPercent: totalFuelPercent,
                            headerKey: Lang.Get("Fuel")
                        ));
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