using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using HarmonyLib;

namespace ExtraInfo.Systems.QuernGrindingProgress;

[HarmonyPatch(typeof(BlockEntityOpenableContainer), nameof(BlockEntityOpenableContainer.GetBlockInfo))]
public static class QuernGrindingProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityOpenableContainer __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowQuernGrindingProgress != true) return;
        if (__instance is not BlockEntityQuern blockEntity) return;

        int inputCount = blockEntity.InputSlot?.Itemstack?.StackSize ?? 0;
        int outputCount = blockEntity.OutputSlot?.Itemstack?.StackSize ?? 0;

        if (inputCount == 0 && outputCount > 0 && !blockEntity.CanGrind())
        {
            dsc.AppendLine();
            dsc.AppendLine(Lang.Get("extrainfo:quern-progress-info-header-total"));
            ProgressBar.Build(dsc, 100f, width: 15);
            dsc.AppendLine();
            return;
        }

        if (!blockEntity.CanGrind()) return;

        float maxTime = blockEntity.maxGrindingTime();
        if (maxTime <= 0) return;

        double perItemPercent = Math.Clamp((blockEntity.inputGrindTime / maxTime) * 100.0, 0, 100);

        int totalItems = inputCount + outputCount;
        if (totalItems <= 0) return;

        double totalPercent = Math.Clamp(((outputCount + (perItemPercent / 100.0)) / totalItems) * 100.0, 0, 100);

        dsc.AppendLine();

        dsc.AppendLine(Lang.Get("extrainfo:quern-progress-info-header-total"));
        ProgressBar.Build(dsc, (float)totalPercent, width: 15);
        dsc.AppendLine();

        dsc.AppendLine(Lang.Get("extrainfo:quern-progress-info-header-single-item"));
        ProgressBar.Build(dsc, (float)perItemPercent, width: 15);
        dsc.AppendLine();
    }
}