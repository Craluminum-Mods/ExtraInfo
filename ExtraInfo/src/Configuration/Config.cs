namespace ExtraInfo.Configuration;

public class Config
{
    public bool HighlightReinforcedBlocks { get; set; } = true;
    public bool HighlightMycelium { get; set; } = true;

    public bool FixVTMLInBlockTooltip { get; set; } = true;

    public bool OpenHandbookPageForEntity { get; set; } = true;
    public bool ShowHandbookCreatureDiet { get; set; } = true;
    public bool ShowHandbookEatableByCreatures { get; set; } = true;
    public bool ShowHandbookEntityDrops { get; set; } = true;
    public bool ShowHandbookEntityStats { get; set; } = true;
    public bool ShowHandbookPanningDrops { get; set; } = true;
    public bool ShowHandbookPitKiln { get; set; } = true;
    public bool ShowHandbookTraderGoods { get; set; } = true;
    public bool ShowHandbookTroughFeedOptions { get; set; } = true;
    public bool ShowHandbookWorkableTemp { get; set; } = true;

    public bool ShowAnvilWorkableTemp { get; set; } = true;
    public bool ShowBloomeryProgress { get; set; } = true;
    public bool ShowCementationFurnaceProgress { get; set; } = true;
    public bool ShowCharcoalPitProgress { get; set; } = true;
    public bool ShowCokeOvenProgress { get; set; } = true;
    public bool ShowPitKilnProgress { get; set; } = true;
    public bool ShowQuernGrindingProgress { get; set; } = true;
    public bool ShowSkepProgress { get; set; } = true;

    public bool ShowBlockBreakingTime { get; set; } = true;
    public bool ShowBlockTransitionInfo { get; set; } = true;
    public bool ShowBombStats { get; set; } = true;
    public bool ShowFarmlandDropsSoil { get; set; } = true;
    public bool ShowFarmlandProgress { get; set; } = true;
    public bool ShowMechanicalBlockInfo { get; set; }
    public bool ShowPileTotalItems { get; set; } = true;
    public bool ShowStackMetalUnits { get; set; } = true;
    public bool ShowTemperatureInName { get; set; } = true;
    public bool ShowTranslocatorDestination { get; set; } = true;
    public bool ShowTreeStats { get; set; } = true;

    public Config() { }

    public Config(Config previousConfig)
    {
        HighlightReinforcedBlocks = previousConfig.HighlightReinforcedBlocks;
        HighlightMycelium = previousConfig.HighlightMycelium;

        FixVTMLInBlockTooltip = previousConfig.FixVTMLInBlockTooltip;

        OpenHandbookPageForEntity = previousConfig.OpenHandbookPageForEntity;
        ShowHandbookCreatureDiet = previousConfig.ShowHandbookCreatureDiet;
        ShowHandbookEatableByCreatures = previousConfig.ShowHandbookEatableByCreatures;
        ShowHandbookEntityDrops = previousConfig.ShowHandbookEntityDrops;
        ShowHandbookEntityStats = previousConfig.ShowHandbookEntityStats;
        ShowHandbookPanningDrops = previousConfig.ShowHandbookPanningDrops;
        ShowHandbookPitKiln = previousConfig.ShowHandbookPitKiln;
        ShowHandbookTraderGoods = previousConfig.ShowHandbookTraderGoods;
        ShowHandbookTroughFeedOptions = previousConfig.ShowHandbookTroughFeedOptions;
        ShowHandbookWorkableTemp = previousConfig.ShowHandbookWorkableTemp;

        ShowAnvilWorkableTemp = previousConfig.ShowAnvilWorkableTemp;
        ShowBloomeryProgress = previousConfig.ShowBloomeryProgress;
        ShowCementationFurnaceProgress = previousConfig.ShowCementationFurnaceProgress;
        ShowCharcoalPitProgress = previousConfig.ShowCharcoalPitProgress;
        ShowCokeOvenProgress = previousConfig.ShowCokeOvenProgress;
        ShowPitKilnProgress = previousConfig.ShowPitKilnProgress;
        ShowQuernGrindingProgress = previousConfig.ShowQuernGrindingProgress;
        ShowSkepProgress = previousConfig.ShowSkepProgress;

        ShowBlockBreakingTime = previousConfig.ShowBlockBreakingTime;
        ShowBlockTransitionInfo = previousConfig.ShowBlockTransitionInfo;
        ShowBombStats = previousConfig.ShowBombStats;
        ShowFarmlandDropsSoil = previousConfig.ShowFarmlandDropsSoil;
        ShowFarmlandProgress = previousConfig.ShowFarmlandProgress;
        ShowMechanicalBlockInfo = previousConfig.ShowMechanicalBlockInfo;
        ShowPileTotalItems = previousConfig.ShowPileTotalItems;
        ShowStackMetalUnits = previousConfig.ShowStackMetalUnits;
        ShowTemperatureInName = previousConfig.ShowTemperatureInName;
        ShowTranslocatorDestination = previousConfig.ShowTranslocatorDestination;
        ShowTreeStats = previousConfig.ShowTreeStats;
    }
}