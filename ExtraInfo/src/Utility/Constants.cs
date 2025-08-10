namespace ExtraInfo;

public static class Constants
{
    public static string Enabled => Lang.Get("worldconfig-snowAccum-Enabled");
    public static string Disabled => Lang.Get("worldconfig-snowAccum-Disabled");

    public static string ToggleName(string name) => Lang.Get("extrainfo:Toggle", name);
    public static string StringToggle(bool state, string name) => Lang.Get("extrainfo:Toggle." + state, name, state ? Enabled : Disabled);

    public static class ColorsRGBA
    {
        public static int Cyan => ColorUtil.ColorFromRgba(new Vec4f(0f, 1f, 1f, 0.5f)); // #77f7f7
        public static int Red => ColorUtil.ColorFromRgba(new Vec4f(1f, 0.4f, 0.4f, 0.5f)); // #ff6666
        public static int Yellow => ColorUtil.ColorFromRgba(new Vec4f(1f, 1f, 0.4f, 0.5f)); // #ffff66
    }

    public static class Colors
    {
        public const string Yellow = "#EEEE90";
        public const string Green = "#90EE90";
    }

    public static class Modid
    {
        public const string FarmlandDropsSoil = "farmlanddropssoil";
    }

    public static class Text
    {
        public const string MatureDaysAttr = "matureDays";
        public const string SproutDaysAttr = "sproutDays";
        public const string WorkableTemperatureAttr = "workableTemperature";
        public const string BlastRadiusAttr = "blastRadius";
        public const string InjureRadiusAttr = "injureRadius";
        public const string BlastTypeAttr = "blastType";

        public const string FormatSndBrackets = "{0} ({1})";
        public const string FormatPercent = "{0}: {1}%";

        public static readonly string AlwaysWorkable = Lang.Get("extrainfo:AlwaysWorkable");
        public static readonly string ValidAnimalFeed = Lang.Get("extrainfo:trough-validfeed");
        public static readonly string CharcoalPit = Lang.Get("block-charcoalpit");
        public static readonly string Coke = Lang.Get("item-coke");
        public static readonly string Current = Lang.Get("extrainfo:Current");
        public static readonly string EatenByWildAnimals = Lang.Get("Eaten by wild animals.");
        public static readonly string EatenByAnimals = Lang.Get("Eaten by animals");
        public static readonly string EatenBy = Lang.Get("Eaten by");
        public static readonly string Everything = Lang.Get("tabname-general");
        public static readonly string Food = Lang.Get("extrainfo:Food");
        public static readonly string Fuel = Lang.Get("extrainfo:Fuel");
        public static readonly string Harvestable = Lang.Get("Harvestable");
        public static readonly string ObtainedByKilling = Lang.Get("Obtained by killing");
        public static readonly string ObtainedByKillingAndHarvesting = Lang.Get("Obtained by killing & harvesting");
        public static readonly string One = Lang.Get("extrainfo:One");
        public static readonly string Other = Lang.Get("blockmaterial-Other");
        public static readonly string PanningDrops = Lang.Get("extrainfo:PanningDrops");
        public static readonly string PurchasedBy = Lang.Get("Purchased by");
        public static readonly string SoldBy = Lang.Get("Sold by");
        public static readonly string TooColdToWork = Lang.Get("Too cold to work");
        public static readonly string WarmingUp = Lang.Get("Warming up...");
        public static readonly string YouCanBuy = Lang.Get("You can Buy");
        public static readonly string YouCanSell = Lang.Get("You can Sell");

        public static string Damage(float damage) => Lang.Get("extrainfo:Damage", damage);
        public static string DamageTier(int damageTier) => Lang.Get("Damage tier: {0}", damageTier);
        public static string Health(float health) => Lang.Get("Health: {0}{1} hp", health, "");

        public static string FuseTimeSeconds(float seconds) => Lang.Get("extrainfo:bomb-FuseTime", Seconds(seconds));
        public static string BlastRadius(float value) => Lang.Get("extrainfo:bomb-BlastRadius", value);
        public static string InjureRadius(float value) => Lang.Get("extrainfo:bomb-InjureRadius", value);

        public static string BlastType(EnumBlastType value) => Lang.Get("extrainfo:bomb-BlastType", value switch
        {
            EnumBlastType.OreBlast => Lang.Get("blockmaterial-Ore"),
            EnumBlastType.RockBlast => Lang.Get("blockmaterial-Stone"),
            EnumBlastType.EntityBlast => Lang.Get("tabname-creatures"),
            _ => Lang.Get("foodcategory-unknown")
        });

        public static string Hours(double hours) => Lang.Get("{0} hours", Math.Floor(hours));
        public static string Hours(float hours) => Lang.Get("{0} hours", Math.Floor(hours));

        public static string HoursAndMinutes(double hours)
        {
            double minutes = Math.Floor((hours - Math.Truncate(hours)) * 60.0);
            return Lang.Get("{0} hours, {1} minutes", Math.Floor(hours), minutes);
        }

        public static string HoursAndMinutes(float hours)
        {
            double minutes = Math.Floor((hours - Math.Truncate(hours)) * 60.0);
            return Lang.Get("{0} hours, {1} minutes", Math.Floor(hours), minutes);
        }

        public static string MinutesAndSeconds(double hours)
        {
            double minutes = Math.Floor((hours - Math.Truncate(hours)) * 60.0);
            double seconds = Math.Floor((minutes - Math.Truncate(minutes)) * 60.0);
            return Lang.Get("{0} minutes, {1} seconds", minutes, seconds);
        }

        public static string MinutesAndSeconds(float hours)
        {
            double minutes = Math.Floor((hours - Math.Truncate(hours)) * 60.0);
            double seconds = Math.Floor((minutes - Math.Truncate(minutes)) * 60.0);
            return Lang.Get("{0} minutes, {1} seconds", minutes, seconds);
        }

        public static string Seconds(double seconds) => Lang.Get("{0} seconds", seconds);
        public static string Seconds(float seconds) => Lang.Get("{0} seconds", seconds);

        public static string TeleportsTo(BlockPos pos) => Lang.Get("Teleports to {0}", pos);

        public static string CarburizationComplete(int percent) => Lang.Get("Carburization: {0}% complete", $"{percent}");
        public static string RemainingResistance(string percent) => Lang.Get("extrainfo:RemainingResistance", $"{percent}%");

        public static string WorkableTemperature(float temperature) => Lang.Get("extrainfo:WorkableTemperature", (int)temperature);

        public static string WillMatureIn(int min, int max) => Lang.Get("extrainfo:mature-in-days", min, max);
        public static string WillSproutIn(int min, int max) => Lang.Get("extrainfo:sprout-in-days", min, max);
    }
}