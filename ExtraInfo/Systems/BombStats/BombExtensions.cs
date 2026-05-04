using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ExtraInfo.Systems.BombStats;

public static class BombExtensions
{
    public static string FuseTimeSeconds(float seconds)
    {
        return Lang.Get("extrainfo:bomb-FuseTime", Seconds(seconds));
    }

    public static string BlastRadius(float value)
    {
        return Lang.Get("extrainfo:bomb-BlastRadius", value);
    }

    public static string InjureRadius(float value)
    {
        return Lang.Get("extrainfo:bomb-InjureRadius", value);
    }

    public static string BlastType(EnumBlastType value)
    {
        return Lang.Get("extrainfo:bomb-BlastType", value switch
        {
            EnumBlastType.OreBlast => Lang.Get("blockmaterial-Ore"),
            EnumBlastType.RockBlast => Lang.Get("blockmaterial-Stone"),
            EnumBlastType.EntityBlast => Lang.Get("tabname-creatures"),
            _ => Lang.Get("foodcategory-unknown")
        });
    }

    public static string Seconds(double seconds)
    {
        return Lang.Get("{0} seconds", seconds);
    }

    public static string Seconds(float seconds)
    {
        return Lang.Get("{0} seconds", seconds);
    }
}