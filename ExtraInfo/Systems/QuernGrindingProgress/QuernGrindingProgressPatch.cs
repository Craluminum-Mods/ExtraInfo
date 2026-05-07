using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.QuernGrindingProgress;

[HarmonyPatch(typeof(BlockEntityOpenableContainer), nameof(BlockEntityOpenableContainer.GetBlockInfo))]
public static class QuernGrindingProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityOpenableContainer __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowProgressBars) return;
        if (!Config.ShowQuernGrindingProgress) return;
        if (__instance is not BlockEntityQuern blockEntity) return;

        int inputCount = blockEntity.InputSlot?.Itemstack?.StackSize ?? 0;
        int outputCount = blockEntity.OutputSlot?.Itemstack?.StackSize ?? 0;

        if (inputCount == 0 && outputCount > 0 && !blockEntity.CanGrind())
        {
            dsc.AppendLine();
            dsc.AppendLine(Lang.Get("extrainfo:quern-progress-info-header-total"));
            ProgressBarBuilder.Build(dsc, 100f);
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
        ProgressBarBuilder.Build(dsc, (float)totalPercent);
        dsc.AppendLine();

        dsc.AppendLine(Lang.Get("extrainfo:quern-progress-info-header-single-item"));
        ProgressBarBuilder.Build(dsc, (float)perItemPercent);
        dsc.AppendLine();
    }
}