using System;
using System.Text;
using Vintagestory.API.Config;
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

        double maxLayers = fuelPile.MaxStackSize / 2.0;
        double maxPossibleHours = maxLayers * fuelPile.BurnHoursPerLayer;
        float totalFuelPercent = (float)Math.Clamp((totalRemainingHours / maxPossibleHours) * 100, 0, 100);

        sb.AppendLine();
        sb.Append(TimeFormatter.BuildVerticalTimeBlock(
            igSeconds: totalRemainingHours * 3600,
            speedOfTime: fuelPile.Api.World.Calendar.SpeedOfTime,
            completedPercent: totalFuelPercent,
            headerKey: Lang.Get("Fuel")
        ));
    }
}