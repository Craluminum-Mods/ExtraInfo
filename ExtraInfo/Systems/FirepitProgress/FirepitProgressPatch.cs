using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.FirepitProgress;

[HarmonyPatch(typeof(BlockEntityOpenableContainer), nameof(BlockEntityOpenableContainer.GetBlockInfo))]
public static class FirepitProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityOpenableContainer __instance, StringBuilder dsc)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowFirepitProgress) return;
        if (__instance is not BlockEntityFirepit blockEntity) return;

        int inputCount = blockEntity.inputSlot?.Itemstack?.StackSize ?? 0;
        int outputCount = blockEntity.outputSlot?.Itemstack?.StackSize ?? 0;
        int totalItems = inputCount + outputCount;

        // 1. Total crafting progress
        if (totalItems > 0)
        {
            float maxTime = blockEntity.maxCookingTime();
            float totalMaxTime = totalItems * maxTime;
            float totalRemaining = (inputCount * maxTime) - blockEntity.inputStackCookingTime;

            var stackProps = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get("extrainfo:firepit-progress-smeltable-total") + " " + (blockEntity.inputSlot.Empty ? Lang.Get("{0}x {1}", blockEntity.outputSlot.StackSize, blockEntity.outputSlot.Itemstack.GetName()) : Lang.Get("{0}x {1}", totalItems, blockEntity.inputStack.GetName())),
                HoursTotal = totalMaxTime,
                HoursLeft = Math.Max(0, totalRemaining),
                ShowInGameTime = false,
                IsRawSeconds = true,

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo
            };

            dsc.AppendLine().AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(blockEntity.Api, stackProps, reversed: false));
        }

        // 2. Current crafting progress
        if (blockEntity.inputSlot != null && !blockEntity.inputSlot.Empty)
        {
            float maxTime = blockEntity.maxCookingTime();
            float remaining = maxTime - blockEntity.inputStackCookingTime;

            if (maxTime > 0 && remaining > 0)
            {
                var itemProps = new TimeBasedProgressBarProperties
                {
                    HeaderKey = Lang.Get("extrainfo:firepit-progress-smeltable-current") + " " + blockEntity.inputStack.GetName(),
                    HoursTotal = maxTime,
                    HoursLeft = remaining,
                    ShowInGameTime = false,
                    IsRawSeconds = true,

                    ShowProgressBar = Config.ShowProgressBars,
                    ShowRealTime = Config.ShowRealTimeInfo
                };

                dsc.AppendLine().AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(blockEntity.Api, itemProps, reversed: false));
            }
        }

        // 3. Fuel progress
        if (Config.ShowFuelProgress && blockEntity.IsBurning)
        {
            // Total fuel
            if (!blockEntity.fuelSlot.Empty && blockEntity.fuelSlot.StackSize > 1)
            {
                float burnPerItem = blockEntity.maxFuelBurnTime;
                float totalStackBurn = burnPerItem * blockEntity.fuelSlot.StackSize;
                float remainingInFullStack = (burnPerItem * (blockEntity.fuelSlot.StackSize - 1)) + blockEntity.fuelBurnTime;

                var totalFuelProps = new TimeBasedProgressBarProperties
                {
                    HeaderKey = Lang.Get("extrainfo:firepit-progress-fuel-total"),
                    HoursTotal = totalStackBurn,
                    HoursLeft = remainingInFullStack,
                    ShowInGameTime = false,
                    ShowProgressBar = false,
                    IsRawSeconds = true,

                    ShowRealTime = Config.ShowRealTimeInfo
                };
                dsc.AppendLine().AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(blockEntity.Api, totalFuelProps, reversed: true));
            }

            // Current fuel
            var fuelProps = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get("extrainfo:firepit-progress-fuel-current"),
                HoursTotal = blockEntity.maxFuelBurnTime,
                HoursLeft = blockEntity.fuelBurnTime,
                ShowInGameTime = false,
                IsRawSeconds = true,

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo
            };
            dsc.AppendLine().AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(blockEntity.Api, fuelProps, reversed: true));
        }
    }
}