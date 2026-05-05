using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.SkepProgress;

[HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
public static class SkepProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc, EnumHivePopSize ___hivePopSize, double ___harvestableAtTotalHours)
    {
        if (Config?.ShowSkepProgress != true) return;
        if (__instance.Harvestable) return;

        // progress bar is not shown, because poor population prevents any progress
        if (___hivePopSize <= EnumHivePopSize.Poor) return;

        ICoreAPI api = __instance.Api;
        double currentTotalHours = api.World.Calendar.TotalHours;
        double honeyHoursLeft = ___harvestableAtTotalHours - currentTotalHours;
        double honeyTotal = 132.0;

        if (honeyHoursLeft > 0)
        {
            var props = new TimeBasedProgressBarProperties()
            {
                HeaderKey = Lang.Get("extrainfo:WillFinishIn"),
                HoursTotal = honeyTotal,
                HoursLeft = Math.Clamp(honeyHoursLeft, 0, honeyTotal)
            };

            string honeyBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(api, props);

            dsc.AppendLine().Append(honeyBlock);
        }
    }
}