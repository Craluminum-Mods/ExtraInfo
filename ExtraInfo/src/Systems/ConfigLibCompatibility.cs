using ConfigLib;
using ExtraInfo.Configuration;
using ImGuiNET;

namespace ExtraInfo;

public class ConfigLibCompatibility
{
    public const string Modid = "extrainfo";

    private const string prefixSetting = $"{Modid}:Config.Setting.";
    private const string categoryCrafting = $"{Modid}:Config.Category.Crafting";
    private const string categoryHandbook = $"{Modid}:Config.Category.Handbook";
    private const string categoryMisc = $"{Modid}:Config.Category.Miscellaneous";

    public ConfigLibCompatibility(ICoreAPI api)
    {
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig(Modid, (id, buttons) =>
        {
            if (buttons.Save) ModConfig.WriteConfig(api, Core.Config);
            if (buttons.Restore) Core.Config = ModConfig.ReadConfig(api);
            if (buttons.Defaults) Core.Config = new();
            Edit(api, Core.Config, id);
        });
    }

    private void Edit(ICoreAPI api, Configuration.Config config, string id)
    {
        config.HighlightReinforcedBlocks = OnCheckBox(id, config.HighlightReinforcedBlocks, nameof(config.HighlightReinforcedBlocks));
        config.HighlightMycelium = OnCheckBox(id, config.HighlightMycelium, nameof(config.HighlightMycelium));
        ImGui.NewLine();
        ImGui.TextWrapped(Lang.Get(categoryHandbook));
        config.OpenHandbookPageForEntity = OnCheckBox(id, config.OpenHandbookPageForEntity, nameof(config.OpenHandbookPageForEntity));
        config.ShowHandbookCreatureDiet = OnCheckBox(id, config.ShowHandbookCreatureDiet, nameof(config.ShowHandbookCreatureDiet));
        config.ShowHandbookEatableByCreatures = OnCheckBox(id, config.ShowHandbookEatableByCreatures, nameof(config.ShowHandbookEatableByCreatures));
        config.ShowHandbookEntityDrops = OnCheckBox(id, config.ShowHandbookEntityDrops, nameof(config.ShowHandbookEntityDrops));
        config.ShowHandbookEntityStats = OnCheckBox(id, config.ShowHandbookEntityStats, nameof(config.ShowHandbookEntityStats));
        config.ShowHandbookPanningDrops = OnCheckBox(id, config.ShowHandbookPanningDrops, nameof(config.ShowHandbookPanningDrops));
        config.ShowHandbookPitKiln = OnCheckBox(id, config.ShowHandbookPitKiln, nameof(config.ShowHandbookPitKiln));
        config.ShowHandbookTraderGoods = OnCheckBox(id, config.ShowHandbookTraderGoods, nameof(config.ShowHandbookTraderGoods));
        config.ShowHandbookTroughFeedOptions = OnCheckBox(id, config.ShowHandbookTroughFeedOptions, nameof(config.ShowHandbookTroughFeedOptions));
        config.ShowHandbookWorkableTemp = OnCheckBox(id, config.ShowHandbookWorkableTemp, nameof(config.ShowHandbookWorkableTemp));
        ImGui.NewLine();
        ImGui.TextWrapped(Lang.Get(categoryCrafting));
        config.ShowAnvilWorkableTemp = OnCheckBox(id, config.ShowAnvilWorkableTemp, nameof(config.ShowAnvilWorkableTemp));
        config.ShowBloomeryProgress = OnCheckBox(id, config.ShowBloomeryProgress, nameof(config.ShowBloomeryProgress));
        config.ShowCementationFurnaceProgress = OnCheckBox(id, config.ShowCementationFurnaceProgress, nameof(config.ShowCementationFurnaceProgress));
        config.ShowCharcoalPitProgress = OnCheckBox(id, config.ShowCharcoalPitProgress, nameof(config.ShowCharcoalPitProgress));
        config.ShowCokeOvenProgress = OnCheckBox(id, config.ShowCokeOvenProgress, nameof(config.ShowCokeOvenProgress));
        config.ShowPitKilnProgress = OnCheckBox(id, config.ShowPitKilnProgress, nameof(config.ShowPitKilnProgress));
        config.ShowQuernGrindingProgress = OnCheckBox(id, config.ShowQuernGrindingProgress, nameof(config.ShowQuernGrindingProgress));
        config.ShowSkepProgress = OnCheckBox(id, config.ShowSkepProgress, nameof(config.ShowSkepProgress));
        ImGui.NewLine();
        ImGui.TextWrapped(Lang.Get(categoryMisc));
        config.ShowBlockBreakingTime = OnCheckBox(id, config.ShowBlockBreakingTime, nameof(config.ShowBlockBreakingTime));
        config.ShowBlockTransitionInfo = OnCheckBox(id, config.ShowBlockTransitionInfo, nameof(config.ShowBlockTransitionInfo));
        config.ShowBombStats = OnCheckBox(id, config.ShowBombStats, nameof(config.ShowBombStats));
        config.ShowFarmlandDropsSoil = OnCheckBox(id, config.ShowFarmlandDropsSoil, nameof(config.ShowFarmlandDropsSoil));
        config.ShowFarmlandProgress = OnCheckBox(id, config.ShowFarmlandProgress, nameof(config.ShowFarmlandProgress));
        config.ShowMechanicalBlockInfo = OnCheckBox(id, config.ShowMechanicalBlockInfo, nameof(config.ShowMechanicalBlockInfo));
        config.ShowPileTotalItems = OnCheckBox(id, config.ShowPileTotalItems, nameof(config.ShowPileTotalItems));
        config.ShowStackMetalUnits = OnCheckBox(id, config.ShowStackMetalUnits, nameof(config.ShowStackMetalUnits));
        config.ShowTranslocatorDestination = OnCheckBox(id, config.ShowTranslocatorDestination, nameof(config.ShowTranslocatorDestination));
        config.ShowTreeStats = OnCheckBox(id, config.ShowTreeStats, nameof(config.ShowTreeStats));
    }

    private bool OnCheckBox(string id, bool value, string name)
    {
        bool newValue = value;
        ImGui.Checkbox(Lang.Get(prefixSetting + name) + $"##{name}-{id}", ref newValue);
        return newValue;
    }
}