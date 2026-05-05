using System;
using System.Text;
using Vintagestory.GameContent;

namespace ExtraInfo;

public static class InfoExtensions
{
    public static void AppendFuelProgress(this StringBuilder sb, BlockEntityCoalPile fuelPile, string headerKey)
    {
        if (!fuelPile.IsBurning) return;

        double totalHours = fuelPile.Api.World.Calendar.TotalHours;
        double burnStart = fuelPile.GetField<double>("burnStartTotalHours");

        double hoursElapsedOnCurrentLayer = Math.Max(0, totalHours - burnStart);
        double hoursLeftInCurrentLayer = Math.Max(0, fuelPile.BurnHoursPerLayer - hoursElapsedOnCurrentLayer);
        double hoursInLayersBelow = (fuelPile.Layers - 1) * fuelPile.BurnHoursPerLayer;
        double totalRemainingHours = hoursInLayersBelow + hoursLeftInCurrentLayer;

        double currentPileCapacity = fuelPile.Layers * fuelPile.BurnHoursPerLayer;

        var props = new TimeBasedProgressBarProperties
        {
            HeaderKey = headerKey,
            HoursTotal = currentPileCapacity,
            HoursLeft = totalRemainingHours,

            ShowProgressBar = Config.ShowProgressBars,
            ShowRealTime = Config.ShowRealTimeInfo,
            ShowInGameTime = Config.ShowInGameTimeInfo
        };

        sb.AppendLine();
        sb.Append(TimeFormatter.BuildTimeBlockPlusProgressBar(fuelPile.Api, props, reversed: true));
    }

    public static bool ApplyTimersOrProgressBars()
    {
        return Config != null && (Config.ShowProgressBars || Config.ShowRealTimeInfo || Config.ShowInGameTimeInfo);
    }
}