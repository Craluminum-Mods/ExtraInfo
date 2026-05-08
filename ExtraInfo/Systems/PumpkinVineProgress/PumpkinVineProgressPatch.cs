using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.PumpkinVineProgress;

[HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
public static class PumpkinVineProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntity __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowPumpkinVineProgress) return;

        if (__instance is not BlockEntityPumpkinVine vine || vine.Api == null) return;

        double currentHours = vine.Api.World.Calendar.TotalHours;

        // 1. Vine progress
        double vineRemaining = vine.totalHoursForNextStage - currentHours;

        if (vineRemaining > 0)
        {
            var vineProps = new TimeBasedProgressBarProperties()
            {
                HeaderKey = Lang.Get("extrainfo:NextVineStageIn"),
                HoursTotal = BlockEntityPumpkinVine.vineHoursToGrow,
                HoursLeft = vineRemaining,
                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(vine.Api, vineProps));
        }

        // 2. Pumpkin progress
        foreach (var facing in BlockFacing.HORIZONTALS)
        {
            if (vine.pumpkinTotalHoursForNextStage.TryGetValue(facing, out double targetHours) && targetHours > 0)
            {
                double fruitRemaining = targetHours - currentHours;

                if (fruitRemaining > 0)
                {
                    var fruitProps = new TimeBasedProgressBarProperties()
                    {
                        HeaderKey = Lang.Get("extrainfo:NextPumpkinStageIn"),
                        HoursTotal = BlockEntityPumpkinVine.pumpkinHoursToGrow,
                        HoursLeft = fruitRemaining,
                        ShowProgressBar = Config.ShowProgressBars,
                        ShowRealTime = Config.ShowRealTimeInfo,
                        ShowInGameTime = Config.ShowInGameTimeInfo
                    };

                    dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(vine.Api, fruitProps));
                }
            }
        }
    }
}