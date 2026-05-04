using Vintagestory.API.Config;

namespace ExtraInfo;

public static class Constants
{
    public static string Enabled => Lang.Get("worldconfig-snowAccum-Enabled");
    public static string Disabled => Lang.Get("worldconfig-snowAccum-Disabled");

    public static string ToggleName(string name) => Lang.Get("extrainfo:Toggle", name);
    public static string StringToggle(bool state, string name) => Lang.Get("extrainfo:Toggle." + state, name, state ? Enabled : Disabled);
}