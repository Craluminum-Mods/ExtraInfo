using System;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BeehiveKilnProgress;

[HarmonyPatch(typeof(BlockEntityBeeHiveKiln), nameof(BlockEntityBeeHiveKiln.GetBlockInfo))]
public static class PlacedBlockInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBeeHiveKiln __instance, IPlayer forPlayer, StringBuilder dsc, bool ___receivesHeat)
    {
        if (Core.Config?.ShowBeehiveKilnProgress != true) return;

        double targetHours = BlockEntityBeeHiveKiln.ItemBurnTimeHours;

        if (__instance.TotalHoursHeatReceived <= 0 || __instance.TotalHoursHeatReceived >= targetHours) return;

        double hoursRemaining = Math.Max(0, targetHours - __instance.TotalHoursHeatReceived);
        float completedPercent = (float)Math.Clamp((float)(__instance.TotalHoursHeatReceived / targetHours) * 100, 0, 100);

        string statusKey = ___receivesHeat ? "extrainfo:WillFinishIn" : "extrainfo:BeehiveKilnPaused";

        double displaySeconds = ___receivesHeat ? hoursRemaining * 3600 : 0;

        string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
            igSeconds: displaySeconds,
            speedOfTime: __instance.Api.World.Calendar.SpeedOfTime,
            completedPercent: completedPercent,
            headerKey: Lang.Get(statusKey)
        );

        dsc.AppendLine();
        dsc.Append(verticalBlock);
    }
}