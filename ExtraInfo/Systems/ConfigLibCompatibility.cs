using ConfigLib;
using ExtraInfo.Configuration;
using ImGuiNET;
using System.Numerics;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ExtraInfo.Systems;

public class ConfigLibCompatibility
{
    public const string MOD_ID = "extrainfo";

    public ConfigLibCompatibility(ICoreAPI api)
    {
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig(MOD_ID, (id, buttons) =>
        {
            if (buttons.Save) ModConfig.WriteConfig(api, Core.Config);
            if (buttons.Restore) Core.Config = ModConfig.ReadConfig(api);
            if (buttons.Defaults) Core.Config = new();
            Edit(api, Core.Config, id);
        });
    }

    private void Edit(ICoreAPI api, Configuration.Config config, string id)
    {
        ImGui.NewLine();
        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.General"));
        config.ShowProgressBars = OnCheckBox(id, config.ShowProgressBars, nameof(config.ShowProgressBars));
        config.ShowRealTimeInfo = OnCheckBox(id, config.ShowRealTimeInfo, nameof(config.ShowRealTimeInfo));
        config.ShowInGameTimeInfo = OnCheckBox(id, config.ShowInGameTimeInfo, nameof(config.ShowInGameTimeInfo));
        config.ShowBlockBreakingTime = OnCheckBox(id, config.ShowBlockBreakingTime, nameof(config.ShowBlockBreakingTime));
        config.ShowBlockTransitionInfo = OnCheckBox(id, config.ShowBlockTransitionInfo, nameof(config.ShowBlockTransitionInfo));
        config.ShowFuelProgress = OnCheckBox(id, config.ShowFuelProgress, nameof(config.ShowFuelProgress));

        ImGui.Separator();
        if (ImGui.CollapsingHeader(Lang.Get($"{MOD_ID}:Config.Category.Clayforming") + $"##clayforming-{id}"))
        {
            config.ShowClayformingPlacementPreview = OnCheckBox(id, config.ShowClayformingPlacementPreview, nameof(config.ShowClayformingPlacementPreview));
            config.ClayformingPlacementPreviewColor = PickColor(id, config.ClayformingPlacementPreviewColor, nameof(config.ClayformingPlacementPreviewColor));
        }
        ImGui.Separator();

        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.Handbook"));
        config.OpenHandbookPageForEntity = OnCheckBox(id, config.OpenHandbookPageForEntity, nameof(config.OpenHandbookPageForEntity));
        config.ShowHandbookCreatureDiet = OnCheckBox(id, config.ShowHandbookCreatureDiet, nameof(config.ShowHandbookCreatureDiet));
        config.ShowHandbookEatableByCreatures = OnCheckBox(id, config.ShowHandbookEatableByCreatures, nameof(config.ShowHandbookEatableByCreatures));
        config.ShowHandbookEntityDrops = OnCheckBox(id, config.ShowHandbookEntityDrops, nameof(config.ShowHandbookEntityDrops));
        config.ShowHandbookPanningDrops = OnCheckBox(id, config.ShowHandbookPanningDrops, nameof(config.ShowHandbookPanningDrops));
        config.ShowHandbookPitKiln = OnCheckBox(id, config.ShowHandbookPitKiln, nameof(config.ShowHandbookPitKiln));
        config.ShowHandbookTraderGoods = OnCheckBox(id, config.ShowHandbookTraderGoods, nameof(config.ShowHandbookTraderGoods));
        config.ShowHandbookTroughFeedOptions = OnCheckBox(id, config.ShowHandbookTroughFeedOptions, nameof(config.ShowHandbookTroughFeedOptions));
        config.ShowHandbookWorkableTemp = OnCheckBox(id, config.ShowHandbookWorkableTemp, nameof(config.ShowHandbookWorkableTemp));
        ImGui.NewLine();
        
        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.Metalworking"));
        config.ShowAnvilWorkableTemp = OnCheckBox(id, config.ShowAnvilWorkableTemp, nameof(config.ShowAnvilWorkableTemp));
        config.ShowBloomeryProgress = OnCheckBox(id, config.ShowBloomeryProgress, nameof(config.ShowBloomeryProgress));
        config.ShowCementationFurnaceProgress = OnCheckBox(id, config.ShowCementationFurnaceProgress, nameof(config.ShowCementationFurnaceProgress));
        config.ShowCharcoalPitProgress = OnCheckBox(id, config.ShowCharcoalPitProgress, nameof(config.ShowCharcoalPitProgress));
        config.ShowCokeOvenProgress = OnCheckBox(id, config.ShowCokeOvenProgress, nameof(config.ShowCokeOvenProgress));
        config.ShowStackMetalUnits = OnCheckBox(id, config.ShowStackMetalUnits, nameof(config.ShowStackMetalUnits));
        ImGui.NewLine();

        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.Processing"));
        config.ShowBeehiveKilnProgress = OnCheckBox(id, config.ShowBeehiveKilnProgress, nameof(config.ShowBeehiveKilnProgress));
        config.ShowBoilerProgress = OnCheckBox(id, config.ShowBoilerProgress, nameof(config.ShowBoilerProgress));
        config.ShowFirepitProgress = OnCheckBox(id, config.ShowFirepitProgress, nameof(config.ShowFirepitProgress));
        config.ShowPitKilnProgress = OnCheckBox(id, config.ShowPitKilnProgress, nameof(config.ShowPitKilnProgress));
        config.ShowQuernGrindingProgress = OnCheckBox(id, config.ShowQuernGrindingProgress, nameof(config.ShowQuernGrindingProgress));
        ImGui.NewLine();

        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.Husbandry"));
        config.ShowAnimalPregnancyProgress = OnCheckBox(id, config.ShowAnimalPregnancyProgress, nameof(config.ShowAnimalPregnancyProgress));
        config.ShowBerryBushProgress = OnCheckBox(id, config.ShowBerryBushProgress, nameof(config.ShowBerryBushProgress));
        config.ShowFarmlandProgress = OnCheckBox(id, config.ShowFarmlandProgress, nameof(config.ShowFarmlandProgress));
        config.ShowSkepProgress = OnCheckBox(id, config.ShowSkepProgress, nameof(config.ShowSkepProgress));
        config.ShowTreeProgress = OnCheckBox(id, config.ShowTreeProgress, nameof(config.ShowTreeProgress));
        config.ShowTreeStats = OnCheckBox(id, config.ShowTreeStats, nameof(config.ShowTreeStats));
        ImGui.NewLine();

        ImGui.TextWrapped(Lang.Get($"{MOD_ID}:Config.Category.Misc"));
        config.ShowBombStats = OnCheckBox(id, config.ShowBombStats, nameof(config.ShowBombStats));
        config.ShowMechanicalBlockInfo = OnCheckBox(id, config.ShowMechanicalBlockInfo, nameof(config.ShowMechanicalBlockInfo));
        config.ShowPileTotalItems = OnCheckBox(id, config.ShowPileTotalItems, nameof(config.ShowPileTotalItems));
        config.ShowTranslocatorDestination = OnCheckBox(id, config.ShowTranslocatorDestination, nameof(config.ShowTranslocatorDestination));
    }

    private bool OnCheckBox(string id, bool value, string name)
    {
        string fullName = Lang.Get($"{MOD_ID}:Config.Setting." + name);
        bool newValue = value;
        ImGui.Checkbox(fullName + $"##{name}-{id}", ref newValue);
        return newValue;
    }

    private byte[] PickColor(string id, byte[] color, string name)
    {
        string fullName = Lang.Get($"{MOD_ID}:Config.Setting." + name);

        Vector4 newValue = new Vector4(
            color[0] / 255f, // R
            color[1] / 255f, // G
            color[2] / 255f, // B
            color[3] / 255f  // A
        );

        ImGui.SetNextItemWidth(150);
        ImGuiColorEditFlags flags = ImGuiColorEditFlags.PickerHueBar | ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaBar;
        if (ImGui.ColorEdit4(fullName + $"##{name}-{id}", ref newValue, flags))
        {
            color[0] = (byte)(newValue.X * 255);
            color[1] = (byte)(newValue.Y * 255);
            color[2] = (byte)(newValue.Z * 255);
            color[3] = (byte)(newValue.W * 255);
        }

        return color;
    }
}