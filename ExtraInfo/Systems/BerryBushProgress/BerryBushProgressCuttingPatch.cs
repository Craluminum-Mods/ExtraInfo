using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BerryBushProgress;

[HarmonyPatch(typeof(BEBehaviorFruitingBushCutting), nameof(BEBehaviorFruitingBushCutting.GetBlockInfo))]
public static class BerryBushProgressCuttingPatch
{
    [HarmonyPostfix]
    public static void Postfix(BEBehaviorFruitingBushCutting __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowBerryBushProgress) return;

        ICoreAPI api = __instance.Api;
        double currentDays = api.World.Calendar.TotalDays;
        double daysRemaining = __instance.matureTotalDays - currentDays;

        if (daysRemaining > 0)
        {
            double maxMonths = __instance.Block.Attributes["matureTotalMonthsMax"].AsDouble(4);
            
            double totalHours = maxMonths * api.World.Calendar.DaysPerMonth * api.World.Calendar.HoursPerDay;
            double hoursLeft = daysRemaining * api.World.Calendar.HoursPerDay;

            var props = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get("extrainfo:WillMatureIn"),
                HoursTotal = totalHours,
                HoursLeft = Math.Clamp(hoursLeft, 0, totalHours),

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(api, props));
        }
    }
}
