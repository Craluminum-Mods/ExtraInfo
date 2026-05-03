namespace ExtraInfo;

public static class TimeFormatter
{
    public static string FormatFullTime(double totalSeconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(totalSeconds);

        if (t.TotalDays >= 1)
        {
            int days = (int)Math.Floor(t.TotalDays);
            int hours = t.Hours;

            return hours > 0
                ? Lang.Get("{0} days, {1} hours", days, hours)
                : Lang.Get("count-days", days);
        }

        if (t.TotalHours >= 1)
        {
            int hours = (int)Math.Floor(t.TotalHours);
            int minutes = t.Minutes;

            return minutes > 0
                ? Lang.Get("{0} hours, {1} minutes", hours, minutes)
                : Lang.Get("{0} hours", hours);
        }

        if (t.TotalMinutes >= 1)
        {
            int minutes = t.Minutes;
            int seconds = t.Seconds;

            return seconds > 0
                ? Lang.Get("{0} minutes, {1} seconds", minutes, seconds)
                : Lang.Get("{0} minutes", minutes);
        }

        return Lang.Get("{0} seconds", t.Seconds);
    }

    public static string BuildVerticalTimeBlock(double igSeconds, float speedOfTime, float completedPercent, string headerKey)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(headerKey);

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