using ConfigLib;
using ExtraInfo.Configuration;
using ImGuiNET;
using System;
using System.Collections.Generic;
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

    private static void Edit(ICoreAPI api, Configuration.Config config, string id)
    {
        if (BeginSection(id, "General"))
        {
            DrawCheckBox(id, config, nameof(config.ShowRealTimeInfo));
            DrawCheckBox(id, config, nameof(config.ShowInGameTimeInfo));
            DrawCheckBox(id, config, nameof(config.ShowBlockBreakingTime));
            DrawCheckBox(id, config, nameof(config.ShowBlockTransitionInfo));
            DrawCheckBox(id, config, nameof(config.ShowFuelProgress));
            EndSection();
        }
        if (BeginSection(id, "ProgressBar"))
        {
            DrawCheckBox(id, config, nameof(config.ShowProgressBars));
            DrawPickSymbol(id, config, nameof(config.ProgressBarFillChar));
            DrawPickSymbol(id, config, nameof(config.ProgressBarEmptyChar));
            DrawPickSymbol(id, config, nameof(config.ProgressBarStartCap));
            DrawPickSymbol(id, config, nameof(config.ProgressBarEndCap));
            DrawInputInt(id, config, nameof(config.ProgressBarWidth), 50);

            ImGui.Spacing();
            ImGui.SeparatorText(Lang.Get($"{MOD_ID}:Config.Setting.Preview"));

            ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetColorU32(ImGuiCol.FrameBg));

            if (ImGui.BeginChild("preview_frame", new Vector2(0, 70), true, ImGuiWindowFlags.AlwaysAutoResize))
            {
                var barProps = new TimeBasedProgressBarProperties();
                barProps.IsRawSeconds = true;
                barProps.HoursLeft = 367;
                barProps.HoursTotal = 1100;
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                ImGui.TextUnformatted(TimeFormatter.BuildTimeBlockPlusProgressBar(api, barProps));
                ImGui.EndChild();
            }
            ImGui.PopStyleColor();

            EndSection();
        }
        if (BeginSection(id, "Clayforming"))
        {
            DrawCheckBox(id, config, nameof(config.ShowClayformingPlacementPreview));
            DrawPickColor(id, config, nameof(config.ClayformingPlacementPreviewColor));
            EndSection();
        }
        if (BeginSection(id, "Handbook"))
        {
            DrawCheckBox(id, config, nameof(config.OpenHandbookPageForEntity));
            DrawCheckBox(id, config, nameof(config.ShowHandbookCreatureDiet));
            DrawCheckBox(id, config, nameof(config.ShowHandbookEatableByCreatures));
            DrawCheckBox(id, config, nameof(config.ShowHandbookEntityDrops));
            DrawCheckBox(id, config, nameof(config.ShowHandbookPanningDrops));
            DrawCheckBox(id, config, nameof(config.ShowHandbookPitKiln));
            DrawCheckBox(id, config, nameof(config.ShowHandbookTraderGoods));
            DrawCheckBox(id, config, nameof(config.ShowHandbookTroughFeedOptions));
            DrawCheckBox(id, config, nameof(config.ShowHandbookWorkableTemp));
            EndSection();
        }
        if (BeginSection(id, "Metalworking"))
        {
            DrawCheckBox(id, config, nameof(config.ShowAnvilWorkableTemp));
            DrawCheckBox(id, config, nameof(config.ShowBloomeryProgress));
            DrawCheckBox(id, config, nameof(config.ShowCementationFurnaceProgress));
            DrawCheckBox(id, config, nameof(config.ShowCharcoalPitProgress));
            DrawCheckBox(id, config, nameof(config.ShowCokeOvenProgress));
            DrawCheckBox(id, config, nameof(config.ShowStackMetalUnits));
            EndSection();
        }
        if (BeginSection(id, "Processing"))
        {
            DrawCheckBox(id, config, nameof(config.ShowBeehiveKilnProgress));
            DrawCheckBox(id, config, nameof(config.ShowBoilerProgress));
            DrawCheckBox(id, config, nameof(config.ShowFirepitProgress));
            DrawCheckBox(id, config, nameof(config.ShowPitKilnProgress));
            DrawCheckBox(id, config, nameof(config.ShowQuernGrindingProgress));
            EndSection();
        }
        if (BeginSection(id, "Husbandry"))
        {
            DrawCheckBox(id, config, nameof(config.ShowAnimalPregnancyProgress));
            DrawCheckBox(id, config, nameof(config.ShowBerryBushProgress));
            DrawCheckBox(id, config, nameof(config.ShowFarmlandProgress));
            DrawCheckBox(id, config, nameof(config.ShowSkepProgress));
            DrawCheckBox(id, config, nameof(config.ShowTreeProgress));
            DrawCheckBox(id, config, nameof(config.ShowTreeStats));
            EndSection();
        }
        if (BeginSection(id, "Misc"))
        {
            DrawCheckBox(id, config, nameof(config.ShowBombStats));
            DrawCheckBox(id, config, nameof(config.ShowMechanicalBlockInfo));
            DrawCheckBox(id, config, nameof(config.ShowPileTotalItems));
            DrawCheckBox(id, config, nameof(config.ShowTranslocatorDestination));
            EndSection();
        }
    }

    private static bool BeginSection(string id, string key)
    {
        return ImGui.CollapsingHeader(Lang.Get($"{MOD_ID}:Config.Category.{key}") + $"##cat-{key}-{id}");
    }

    private static void EndSection()
    {
        ImGui.Spacing();
    }

    private static void DrawCheckBox(string id, Configuration.Config config, string propertyName)
    {
        var propertyInfo = config.GetType().GetProperty(propertyName);
        bool configValue = (bool)propertyInfo.GetValue(config);

        if (ImGui.Checkbox($"##{propertyName}-{id}", ref configValue))
        {
            propertyInfo.SetValue(config, configValue);
        }

        ImGui.SameLine();
        ImGui.Text(Lang.Get($"{MOD_ID}:Config.Setting.{propertyName}"));
    }

    private static void DrawInputInt(string id, Configuration.Config config, string propertyName, int max = 100)
    {
        var propertyInfo = config.GetType().GetProperty(propertyName);
        int configValue = (int)propertyInfo.GetValue(config);

        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt($"##{propertyName}-{id}", ref configValue, 1, 10))
        {
            propertyInfo.SetValue(config, Math.Clamp(configValue, 0, max));
        }

        ImGui.SameLine();
        ImGui.Text(Lang.Get($"{MOD_ID}:Config.Setting.{propertyName}"));
    }

    private static void DrawPickColor(string id, Configuration.Config config, string propertyName)
    {
        var propertyInfo = config.GetType().GetProperty(propertyName);
        byte[] configValue = (byte[])propertyInfo.GetValue(config);

        ImGui.SetNextItemWidth(150);
        propertyInfo.SetValue(config, PickColorInternal(id, configValue, propertyName));

        ImGui.SameLine();
        ImGui.Text(Lang.Get($"{MOD_ID}:Config.Setting.{propertyName}"));
    }

    private static void DrawPickSymbol(string id, Configuration.Config config, string propertyName)
    {
        var propertyInfo = config.GetType().GetProperty(propertyName);
        char configValue = (char)propertyInfo.GetValue(config);

        propertyInfo.SetValue(config, PickSymbolInternal(id, configValue, propertyName));

        ImGui.SameLine();
        ImGui.Text(Lang.Get($"{MOD_ID}:Config.Setting.{propertyName}"));
    }

    private static byte[] PickColorInternal(string id, byte[] color, string name)
    {
        Vector4 newValue = new Vector4(color[0] / 255f, color[1] / 255f, color[2] / 255f, color[3] / 255f);
        ImGuiColorEditFlags flags = ImGuiColorEditFlags.PickerHueBar | ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaBar;

        if (ImGui.ColorEdit4($"##color-{name}-{id}", ref newValue, flags))
        {
            color[0] = (byte)(newValue.X * 255);
            color[1] = (byte)(newValue.Y * 255);
            color[2] = (byte)(newValue.Z * 255);
            color[3] = (byte)(newValue.W * 255);
        }
        return color;
    }

    private static char PickSymbolInternal(string id, char symbol, string name)
    {
        char newValue = symbol;
        List<char> ignoredSymbols = new List<char> { '<', '>', ' ' };
        string input = symbol.ToString();

        ImGui.SetNextItemWidth(40);
        if (ImGui.InputText($"##text_{name}-{id}", ref input, 1))
        {
            if (input.Length > 0 && !ignoredSymbols.Contains(input[0]))
            {
                newValue = input[0];
            }
        }

        ImGui.SameLine();

        if (ImGui.Button($"{Lang.Get("extrainfo:pick-symbol")}##btn_{name}-{id}"))
        {
            ImGui.OpenPopup($"grid_popup_{name}-{id}");
        }

        if (ImGui.BeginPopup($"grid_popup_{name}-{id}"))
        {
            List<char> symbols = new List<char> { '#', '_', '[', ']', '(', ')', '|', '=', '-', '.' };
            for (int i = 32; i <= 126; i++)
            {
                char c = (char)i;
                if (!ignoredSymbols.Contains(c) && !symbols.Contains(c)) symbols.Add(c);
            }

            if (ImGui.BeginTable($"table_{name}-{id}", 15, ImGuiTableFlags.SizingFixedFit))
            {
                foreach (char s in symbols)
                {
                    ImGui.TableNextColumn();
                    bool isSelected = s == symbol;
                    if (isSelected) ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonActive]);

                    string sLabel = s == '#' ? $"{s} " : $"{s}";
                    if (ImGui.Button($"{sLabel}##{id}_{s}", new Vector2(30, 30)))
                    {
                        newValue = s;
                        ImGui.CloseCurrentPopup();
                    }

                    if (isSelected) ImGui.PopStyleColor();
                }
                ImGui.EndTable();
            }
            ImGui.EndPopup();
        }

        return newValue;
    }
}