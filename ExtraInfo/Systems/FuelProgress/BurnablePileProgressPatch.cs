using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.FuelProgress;

[HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
public static class BurnablePileProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc, float ___burnHoursPerItem)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowFuelProgress) return;
        if (!__instance.IsBurning) return;
        if (___burnHoursPerItem <= 0 || __instance.Inventory == null || __instance.Inventory.Empty) return;

        double elapsedOnCurrentItem = __instance.GetHoursLeft(__instance.Api.World.Calendar.TotalHours);
        double hoursLeftInCurrentItem = Math.Max(0, ___burnHoursPerItem - elapsedOnCurrentItem);

        double hoursInItemsBelow = (__instance.Inventory[0].StackSize - 1) * ___burnHoursPerItem;
        double totalRemainingHours = hoursInItemsBelow + hoursLeftInCurrentItem;

        double currentPileCapacity = __instance.Inventory[0].StackSize * ___burnHoursPerItem;

        if (currentPileCapacity > 0)
        {
            var props = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get("extrainfo:Fuel"),
                HoursTotal = currentPileCapacity,
                HoursLeft = totalRemainingHours,

                ShowProgressBar = Config.ShowProgressBars,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowInGameTime = Config.ShowInGameTimeInfo
            };

            dsc.AppendLine().Append(TimeFormatter.BuildTimeBlockPlusProgressBar(__instance.Api, props, reversed: true));
        }
    }
}