using System;
using System.Collections.Generic;
using System.Text;

namespace ExtraInfo.Systems.FarmlandProgress;

[HarmonyPatch(typeof(BlockEntityFarmland), nameof(BlockEntityFarmland.GetBlockInfo))]
public static class FarmlandProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityFarmland __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowFarmlandProgress != true) return;
        if (__instance?.Api == null) return;

        Block cropBlock = __instance.GetCrop();
        if (cropBlock?.CropProps == null) return;

        if (__instance.GetCropStage(cropBlock) >= cropBlock.CropProps.GrowthStages) return;

        ICoreAPI api = __instance.Api;

        double targetHours = __instance.TotalHoursForNextStage;
        double currentHours = api.World.Calendar.TotalHours;
        double hoursRemaining = targetHours - currentHours;

        if (hoursRemaining > 0)
        {
            double lastUpdate = __instance.TotalHoursLastUpdate;

            double totalWindow = targetHours - lastUpdate;
            float completedPercent = 0;

            if (totalWindow > 0)
            {
                double elapsed = currentHours - lastUpdate;
                completedPercent = (float)Math.Clamp((elapsed / totalWindow) * 100, 0, 100);
            }

            double igSeconds = hoursRemaining * 3600;
            float speedOfTime = api.World.Calendar.SpeedOfTime;

            string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
                igSeconds: igSeconds,
                speedOfTime: speedOfTime,
                completedPercent: completedPercent,
                headerKey: Lang.Get("extrainfo:NextStageIn"));

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}