using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.AnimalLactationProgress;

[HarmonyPatch(typeof(EntityBehaviorMilkable), nameof(EntityBehaviorMilkable.GetInfoText))]
public static class AnimalLactationProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(EntityBehaviorMilkable __instance, StringBuilder infotext, float ___lactatingDaysAfterBirth)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowAnimalLactationProgress) return;

        Entity entity = __instance.entity;
        ICoreAPI api = entity.Api;
        IGameCalendar calendar = api.World.Calendar;
        float hoursPerDay = calendar.HoursPerDay;

        // 1. Lactation progress
        EntityBehaviorMultiply? bhmul = entity.GetBehavior<EntityBehaviorMultiply>();
        if (bhmul == null) return;

        double lactationDaysLeft = ___lactatingDaysAfterBirth -
                                   Math.Max(0.0, calendar.TotalDays - bhmul.TotalDaysLastBirth);

        if (lactationDaysLeft <= 0) return;

        // 2. Refill progress
        double lastMilked = entity.WatchedAttributes.GetFloat("lastMilkedTotalHours");
        double hoursSinceMilking = calendar.TotalHours - lastMilked;
        double refillHoursLeft = hoursPerDay - hoursSinceMilking;

        if (refillHoursLeft > 0)
        {
            var refillProps = new TimeBasedProgressBarProperties()
            {
                HeaderKey = Lang.Get("extrainfo:animal-milk-refill-in"),
                HoursTotal = hoursPerDay,
                HoursLeft = refillHoursLeft,
                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            infotext.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(api, refillProps)).AppendLine().AppendLine();
        }

        double lactationHoursLeft = lactationDaysLeft * hoursPerDay;
        double totalLactationHours = ___lactatingDaysAfterBirth * hoursPerDay;

        var lactationProps = new TimeBasedProgressBarProperties()
        {
            HeaderKey = Lang.Get("extrainfo:animal-lactation-ends-in"),
            HoursTotal = totalLactationHours,
            HoursLeft = lactationHoursLeft,
            ShowProgressBar = Config.ShowProgressBars,
            ShowRealTime = Config.ShowRealTimeInfo,
            ShowInGameTime = Config.ShowInGameTimeInfo
        };

        infotext.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(api, lactationProps)).AppendLine().AppendLine();
    }
}