namespace ExtraInfo.Configuration;

public class Config
{
    #region General
    public bool ShowRealTimeInfo { get; set; } = true;
    public bool ShowInGameTimeInfo { get; set; } = true;
    public bool ShowBlockBreakingTime { get; set; } = true;
    public bool ShowBlockTransitionInfo { get; set; } = true;
    public bool ShowFuelProgress { get; set; } = true;
    #endregion
    #region ProgressBar
    public bool ShowProgressBars { get; set; } = true;
    public char ProgressBarFillChar { get; set; } = '#';
    public char ProgressBarEmptyChar { get; set; } = '_';
    public char ProgressBarStartCap { get; set; } = '[';
    public char ProgressBarEndCap { get; set; } = ']';
    public int ProgressBarWidth { get; set; } = 20;
    #endregion
    #region Clayforming
    public bool ShowClayformingPlacementPreview { get; set; } = true;
    public byte[] ClayformingPlacementPreviewColor { get; set; } = [0, 255, 255, 255]; // RGBA
    #endregion
    #region Handbook
    public bool OpenHandbookPageForEntity { get; set; } = true;
    public bool ShowHandbookCreatureDiet { get; set; } = true;
    public bool ShowHandbookEatableByCreatures { get; set; } = true;
    public bool ShowHandbookEntityDrops { get; set; } = true;
    public bool ShowHandbookPanningDrops { get; set; } = true;
    public bool ShowHandbookPitKiln { get; set; } = true;
    public bool ShowHandbookTraderGoods { get; set; } = true;
    public bool ShowHandbookTroughFeedOptions { get; set; } = true;
    public bool ShowHandbookWorkableTemp { get; set; } = true;
    #endregion
    #region Metalworking
    public bool ShowAnvilWorkableTemp { get; set; } = true;
    public bool ShowBloomeryProgress { get; set; } = true;
    public bool ShowCementationFurnaceProgress { get; set; } = true;
    public bool ShowCharcoalPitProgress { get; set; } = true;
    public bool ShowCokeOvenProgress { get; set; } = true;
    public bool ShowStackMetalUnits { get; set; } = true;
    #endregion
    #region Processing
    public bool ShowBeehiveKilnProgress { get; set; } = true;
    public bool ShowBoilerProgress { get; set; } = true;
    public bool ShowFirepitProgress { get; set; } = true;
    public bool ShowPitKilnProgress { get; set; } = true;
    public bool ShowQuernGrindingProgress { get; set; } = true;
    #endregion
    #region Husbandry
    public bool ShowAnimalPregnancyProgress { get; set; } = true;
    public bool ShowBerryBushProgress { get; set; } = true;
    public bool ShowFarmlandProgress { get; set; } = true;
    public bool ShowPumpkinVineProgress { get; set; } = true;
    public bool ShowSkepProgress { get; set; } = true;
    public bool ShowTreeProgress { get; set; } = true;
    public bool ShowTreeStats { get; set; } = true;
    #endregion
    #region Misc
    public bool ShowBombStats { get; set; } = true;
    public bool ShowMechanicalBlockInfo { get; set; } = false;
    public bool ShowPileTotalItems { get; set; } = true;
    public bool ShowTranslocatorDestination { get; set; } = true;
    #endregion
    public Config() { }

    public Config(Config? previousConfig)
    {
        if (previousConfig == null) return;

        foreach (var property in typeof(Config).GetProperties())
        {
            if (property.CanWrite && property.CanRead)
            {
                property.SetValue(this, property.GetValue(previousConfig));
            }
        }
    }
}