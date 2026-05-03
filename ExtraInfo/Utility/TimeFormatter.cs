namespace ExtraInfo;

public static class TimeFormatter
{
    public static string FormatFullTime(double totalSeconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(totalSeconds);

        if (t.TotalDays >= 1) return Lang.Get("{0} days", Math.Floor(t.TotalDays));

        if (t.TotalHours >= 1)
        {
            return t.Minutes > 0
                ? Lang.Get("{0} hours, {1} minutes", Math.Floor(t.TotalHours), t.Minutes)
                : Lang.Get("{0} hours", Math.Floor(t.TotalHours));
        }

        if (t.TotalMinutes >= 1)
        {
            return t.Seconds > 0
                ? Lang.Get("{0} minutes, {1} seconds", t.Minutes, t.Seconds)
                : Lang.Get("{0} minutes", t.Minutes);
        }

        return Lang.Get("{0} seconds", t.Seconds);
    }

    public static string BuildVerticalTimeBlock(double igSeconds, float speedOfTime, float completedPercent)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(Lang.Get("extrainfo:WillFinishIn"));

        StringBuilder barBuilder = new StringBuilder();
        ProgressBar.Build(barBuilder, completedPercent, width: 15);
        sb.AppendLine(barBuilder.ToString());

        string igTime = FormatFullTime(igSeconds);
        string irlTime = FormatFullTime(igSeconds / speedOfTime);

        sb.AppendLine(Lang.Get("extrainfo:InGameTime", igTime));
        sb.Append(Lang.Get("extrainfo:InRealTime", irlTime));

        return sb.ToString();
    }
}