using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BlockTransitionInfo;

[HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
public static class BlockTransitionInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntity __instance, StringBuilder dsc)
    {
        if (Config?.ShowBlockTransitionInfo != true) return;

        if (__instance is not BlockEntityTransient blockEntity) return;

        TransientProperties props = blockEntity.GetField<TransientProperties>("props");
        if (props == null) return;

        blockEntity.CheckTransition(0);

        double hoursLeft = blockEntity.GetField<double>("transitionHoursLeft");

        if (hoursLeft > 0)
        {
            float totalHours = props.InGameHours;

            float completedPercent = (float)Math.Clamp((1.0 - (hoursLeft / totalHours)) * 100, 0, 100);

            double igSeconds = hoursLeft * 3600;
            float speedOfTime = blockEntity.Api.World.Calendar.SpeedOfTime;

            string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
                igSeconds,
                speedOfTime,
                completedPercent,
                Lang.Get("extrainfo:TimeUntilTransition"));

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}