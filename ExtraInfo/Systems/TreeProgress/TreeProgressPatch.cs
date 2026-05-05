using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.TreeProgress;

[HarmonyPatch(typeof(BlockEntitySapling), nameof(BlockEntitySapling.GetBlockInfo))]
public static class TreeProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntitySapling __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowTreeProgress) return;

        double targetHours = __instance.GetField<double>("totalHoursTillGrowth");
        double currentHours = __instance.Api.World.Calendar.TotalHours;
        double hoursRemaining = targetHours - currentHours;

        if (hoursRemaining > 0)
        {
            NatFloat growthRnd = __instance.GetProperty<NatFloat>("nextStageDaysRnd");
            float growthRateMod = __instance.GetProperty<float>("GrowthRateMod");

            double totalStageHours = growthRnd.avg * 24.0 * growthRateMod;

            var props = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get(__instance.stage == EnumTreeGrowthStage.Seed ? "extrainfo:WillSproutIn" : "extrainfo:WillMatureIn"),
                HoursTotal = totalStageHours,
                HoursLeft = Math.Clamp(hoursRemaining, 0, totalStageHours),

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(__instance.Api, props));
        }
    }
}