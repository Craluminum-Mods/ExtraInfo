using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.AnimalPregnancyProgress;

[HarmonyPatch(typeof(EntityBehaviorMultiply), nameof(EntityBehaviorMultiply.GetInfoText))]
public static class AnimalPregnancyProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(EntityBehaviorMultiply __instance, StringBuilder infotext)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowAnimalPregnancyProgress) return;

        ICoreAPI api = __instance.entity.World.Api;
        float hoursPerDay = api.World.Calendar.HoursPerDay;

        if (__instance.IsPregnant)
        {
            double daysNow = api.World.Calendar.TotalDays;
            double daysLeft = __instance.PregnancyDays - (daysNow - __instance.TotalDaysPregnancyStart);

            double hoursLeft = daysLeft * hoursPerDay;
            double totalDurationHours = __instance.PregnancyDays * hoursPerDay;

            if (hoursLeft <= 0)
            {
                infotext.AppendLine(Lang.Get("extrainfo:animal-giving-birth-soon"));
            }
            else
            {
                infotext.AppendLine().Append(BuildBar(api, "extrainfo:animal-will-give-birth-in", totalDurationHours, hoursLeft));
                infotext.AppendLine();
                infotext.AppendLine();
            }
        }
        else
        {
            double daysNow = api.World.Calendar.TotalDays;
            double cooldownLeft = __instance.TotalDaysCooldownUntil - daysNow;

            if (cooldownLeft > 0)
            {
                double totalCooldownHours = __instance.MultiplyCooldownDaysMax * hoursPerDay;
                double hoursLeft = cooldownLeft * hoursPerDay;

                infotext.AppendLine().Append(BuildBar(api, "extrainfo:animal-ready-to-mate-in", totalCooldownHours, hoursLeft));
                infotext.AppendLine();
                infotext.AppendLine();
            }
        }
    }

    private static string BuildBar(ICoreAPI api, string headerKey, double totalHours, double hoursLeft)
    {
        TimeBasedProgressBarProperties barProps = new()
        {
            HeaderKey = Lang.Get(headerKey),
            HoursTotal = totalHours,
            HoursLeft = hoursLeft,
            ShowProgressBar = Config.ShowProgressBars,
            ShowRealTime = Config.ShowRealTimeInfo,
            ShowInGameTime = Config.ShowInGameTimeInfo
        };
        return TimeFormatter.BuildTimeBlockPlusProgressBar(api, barProps);
    }
}