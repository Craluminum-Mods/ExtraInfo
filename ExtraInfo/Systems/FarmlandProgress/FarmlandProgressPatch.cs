using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.FarmlandProgress;

[HarmonyPatch(typeof(BlockEntityFarmland), nameof(BlockEntityFarmland.GetBlockInfo))]
public static class FarmlandProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityFarmland __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowFarmlandProgress) return;

        if (__instance?.Api == null) return;

        Block cropBlock = __instance.GetCrop();

        if (cropBlock?.CropProps == null) return;

        if (__instance.GetCropStage(cropBlock) >= cropBlock.CropProps.GrowthStages) return;

        double targetHours = __instance.TotalHoursForNextStage;
        double currentHours = __instance.Api.World.Calendar.TotalHours;
        double hoursRemaining = targetHours - currentHours;

        if (hoursRemaining > 0)
        {
            double lastUpdate = __instance.TotalHoursLastUpdate;
            double totalStageWindow = targetHours - lastUpdate;

            var props = new TimeBasedProgressBarProperties()
            {
                HeaderKey = Lang.Get("extrainfo:NextStageIn"),
                HoursTotal = totalStageWindow,
                HoursLeft = hoursRemaining,

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            string verticalBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(__instance.Api, props);

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}