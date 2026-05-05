using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.CharcoalPitProgress;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class CharcoalPitProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowCharcoalPitProgress) return;

        if (world.BlockAccessor.GetBlockEntity(pos.DownCopy()) is not BlockEntityCharcoalPit blockEntity) return;

        string blockName = blockEntity.Block.GetPlacedBlockName(world, pos.DownCopy());

        StringBuilder dsc = new();
        dsc.Append(Lang.Get("extrainfo:CenteredBoldText", blockName));

        if (blockEntity.Lit)
        {
            EnumCharcoalPitState state = (EnumCharcoalPitState)blockEntity.GetField<int>("state");
            double finishedAfterTotalHours = blockEntity.GetField<double>("finishedAfterTotalHours");
            double startingAfterTotalHours = blockEntity.GetField<double>("startingAfterTotalHours");
            double currentHours = world.Calendar.TotalHours;

            var props = new TimeBasedProgressBarProperties()
            {
                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            switch (state)
            {
                case EnumCharcoalPitState.Sealed:
                case EnumCharcoalPitState.Unsealed:
                    {
                        double burnHours = 18.0;
                        double hoursRemaining = finishedAfterTotalHours - currentHours;

                        if (hoursRemaining > 0)
                        {
                            props.HeaderKey = Lang.Get("extrainfo:WillFinishIn");
                            props.HoursTotal = burnHours;
                            props.HoursLeft = hoursRemaining;
                        }
                        break;
                    }

                default:
                    {
                        double ignitionDuration = 0.5;
                        double hoursRemaining = startingAfterTotalHours - currentHours;

                        if (hoursRemaining > 0)
                        {
                            props.HeaderKey = Lang.Get("Warming up...");
                            props.HoursTotal = ignitionDuration;
                            props.HoursLeft = hoursRemaining;
                        }
                        break;
                    }
            }

            if (props.HoursTotal > 0)
            {
                dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(world.Api, props));
            }
        }

        if (dsc.Length > 0)
        {
            __result = __result.TrimEnd() + Environment.NewLine + dsc.ToString().TrimEnd();
        }
    }
}