using System;
using System.Collections.Generic;
using System.Text;

namespace ExtraInfo.Systems.SkepProgress;

[HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
public static class SkepProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowSkepProgress != true) return;

        if (__instance.Harvestable) return;

        EnumHivePopSize popSize = __instance.GetField<EnumHivePopSize>("hivePopSize");
        int flowers = __instance.GetField<int>("quantityNearbyFlowers");
        int hives = __instance.GetField<int>("quantityNearbyHives");

        double targetHours = __instance.GetField<double>("harvestableAtTotalHours");
        double currentHours = __instance.Api.World.Calendar.TotalHours;
        double hoursLeft = targetHours - currentHours;

        float completedPercent = 0f;

        if (popSize == EnumHivePopSize.Poor)
        {
            int effectiveFlowers = flowers - (3 * hives);
            completedPercent = Math.Clamp((effectiveFlowers / 3f) * 25f, 5f, 25f);
        }
        else
        {
            double totalCycle = 72.0;
            float timeProgress = (float)Math.Clamp((1.0 - (hoursLeft / totalCycle)) * 75f, 0, 75f);

            completedPercent = 25f + timeProgress;
        }

        if (completedPercent > 0)
        {
            float speedOfTime = __instance.Api.World.Calendar.SpeedOfTime;

            double displaySeconds = (popSize > EnumHivePopSize.Poor && hoursLeft > 0 && hoursLeft < 500)
                ? hoursLeft * 3600
                : 0;

            string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
                igSeconds: displaySeconds,
                speedOfTime: speedOfTime,
                completedPercent: completedPercent,
                headerKey: Lang.Get("extrainfo:WillFinishIn")
            );

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}