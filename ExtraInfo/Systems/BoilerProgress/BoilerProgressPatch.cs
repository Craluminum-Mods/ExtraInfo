using HarmonyLib;
using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BoilerProgress;

[HarmonyPatch(typeof(BlockBoiler), nameof(BlockBoiler.GetPlacedBlockInfo))]
public static class BoilerProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockBoiler __instance, ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (!ApplyTimersOrProgressBars()) return;
        if (!Config.ShowBoilerProgress) return;
        if (!Config.ShowFuelProgress) return;
        if (world.BlockAccessor.GetBlockEntity(pos) is not BlockEntityBoiler blockEntity) return;

        StringBuilder dsc = new StringBuilder(__result);

        /* NOTE:
         * Total crafting timer info would look way to unpredictable,
         * while crafting timer info for current cycle is just 4 seconds.
         * We don't need timers for very short processes
         */
        
        // Fuel progress
        if (blockEntity.IsBurning || blockEntity.IsSmoldering)
        {
            double lastTick = blockEntity.GetField<double>("lastTickTotalHours");
            float fuelHours = blockEntity.fuelHours;
            double passedSinceTick = world.Calendar.TotalHours - lastTick;
            float smoothedFuelLeft = fuelHours - (float)passedSinceTick;

            var fuelProps = new TimeBasedProgressBarProperties
            {
                HeaderKey = Lang.Get("extrainfo:firepit-progress-fuel-current"),
                HoursTotal = Math.Max(2f, smoothedFuelLeft + (float)passedSinceTick),
                HoursLeft = Math.Max(0, smoothedFuelLeft),

                ShowInGameTime = Config.ShowInGameTimeInfo,
                ShowRealTime = Config.ShowRealTimeInfo,
                ShowProgressBar = Config.ShowProgressBars,
                IsRawSeconds = false
            };

            dsc.AppendLine().AppendLine().AppendLine(TimeFormatter.BuildTimeBlockPlusProgressBar(blockEntity.Api, fuelProps, reversed: true));
        }

        __result = dsc.ToString();
    }
}