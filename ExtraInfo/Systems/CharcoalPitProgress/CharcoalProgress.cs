namespace ExtraInfo.Systems.CharcoalPitProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CharcoalPitProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config?.ShowCharcoalPitProgress != true) return;

        if (world.BlockAccessor.GetBlockEntity(pos.DownCopy()) is not BlockEntityCharcoalPit blockEntity)
            return;

        string blockName = blockEntity.Block.GetPlacedBlockName(world, pos.DownCopy());

        StringBuilder dsc = new();
        dsc.Append(Lang.Get("extrainfo:CenteredBoldText", blockName));

        if (blockEntity.Lit)
        {
            EnumCharcoalPitState state = (EnumCharcoalPitState)blockEntity.GetField<int>("state");

            double finishedAfterTotalHours = blockEntity.GetField<double>("finishedAfterTotalHours");
            double startingAfterTotalHours = blockEntity.GetField<double>("startingAfterTotalHours");
            float burnHours = 18f;

            double currentHours = world.Calendar.TotalHours;
            float speedOfTime = world.Calendar.SpeedOfTime;

            switch (state)
            {
                case EnumCharcoalPitState.Sealed:
                case EnumCharcoalPitState.Unsealed:
                    {
                        double hoursRemaining = finishedAfterTotalHours - currentHours;
                        if (hoursRemaining > 0)
                        {
                            double startHours = finishedAfterTotalHours - burnHours;
                            float completedPercent = (float)Math.Clamp(((currentHours - startHours) / burnHours) * 100, 0, 100);

                            dsc.AppendLine()
                                .Append(TimeFormatter.BuildVerticalTimeBlock(
                                    igSeconds: hoursRemaining * 3600,
                                    speedOfTime: speedOfTime,
                                    completedPercent: completedPercent,
                                    headerKey: Lang.Get("extrainfo:WillFinishIn")));
                        }

                        break;
                    }

                default:
                    {
                        double hoursRemaining = startingAfterTotalHours - currentHours;
                        if (hoursRemaining > 0)
                        {
                            float ignitionDuration = 0.5f;
                            double startHours = startingAfterTotalHours - ignitionDuration;
                            float completedPercent = (float)Math.Clamp(((currentHours - startHours) / ignitionDuration) * 100, 0, 100);

                            dsc.AppendLine()
                                .Append(TimeFormatter.BuildVerticalTimeBlock(
                                    igSeconds: hoursRemaining * 3600,
                                    speedOfTime: speedOfTime,
                                    completedPercent: completedPercent,
                                    headerKey: Lang.Get("Warming up...")));
                        }

                        break;
                    }
            }
        }

        if (dsc.Length > 0)
        {
            __result = __result.TrimEnd() + Environment.NewLine + dsc.ToString().TrimEnd();
        }
    }
}