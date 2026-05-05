using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BeehiveKilnProgress;

[HarmonyPatch(typeof(BlockEntityBeeHiveKiln), nameof(BlockEntityBeeHiveKiln.GetBlockInfo))]
public static class BeehiveKilnProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBeeHiveKiln __instance, IPlayer forPlayer, StringBuilder dsc,
        bool ___receivesHeat, BEBehaviorDoor ___beBehaviorDoor)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowBeehiveKilnProgress) return;

        double targetHours = BlockEntityBeeHiveKiln.ItemBurnTimeHours;

        if (__instance.TotalHoursHeatReceived <= 0 || __instance.TotalHoursHeatReceived >= targetHours) return;

        bool isProcessing = ___receivesHeat && __instance.StructureComplete && !___beBehaviorDoor.Opened;

        double hoursRemaining = Math.Max(0, targetHours - __instance.TotalHoursHeatReceived);

        string statusKey = isProcessing ? "extrainfo:WillFinishIn" : "Out of fuel.";

        var props = new TimeBasedProgressBarProperties()
        {
            HeaderKey = Lang.Get(statusKey),
            HoursTotal = targetHours,
            HoursLeft = hoursRemaining,

            ShowProgressBar = Config.ShowProgressBars,
            ShowRealTime = Config.ShowRealTimeInfo,
            ShowInGameTime = Config.ShowInGameTimeInfo
        };

        string verticalBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(__instance.Api, props);

        dsc.AppendLine().Append(verticalBlock);
    }
}