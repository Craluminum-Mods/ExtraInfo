using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ExtraInfo;

public static class TimeFormatter
{
    public static string FormatFullTime(double totalSeconds)
    {
        if (double.IsNaN(totalSeconds) || double.IsInfinity(totalSeconds)) return "";

        totalSeconds = Math.Clamp(totalSeconds, 0, TimeSpan.MaxValue.TotalSeconds - 1);

        TimeSpan t = TimeSpan.FromSeconds(totalSeconds);

        if (t.TotalDays >= 1)
        {
            int days = (int)Math.Floor(t.TotalDays);
            int hours = t.Hours;
            int minutes = t.Minutes;

            if (hours > 0 && minutes > 0)
                return Lang.Get("{0} days, {1} hours, {2} minutes", days, hours, minutes);

            if (hours > 0)
                return Lang.Get("{0} days, {1} hours", days, hours);

            return Lang.Get("count-days", days);
        }

        if (t.TotalHours >= 1)
        {
            int hours = (int)Math.Floor(t.TotalHours);
            int minutes = t.Minutes;
            int seconds = t.Seconds;

            if (minutes > 0 && seconds > 0)
                return Lang.Get("{0} hours, {1} minutes, {2} seconds", hours, minutes, seconds);

            if (minutes > 0)
                return Lang.Get("{0} hours, {1} minutes", hours, minutes);

            return Lang.Get("{0} hours", hours);
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

    public static string BuildTimeBlockPlusProgressBar(ICoreAPI api, TimeBasedProgressBarProperties props, bool reversed = false)
    {
        if (!props.ShowProgressBar && !props.ShowInGameTime && !props.ShowRealTime) return "";

        float completedPercent = 0;
        if (props.HoursTotal > 0)
        {
            double ratio = props.HoursLeft / props.HoursTotal;
            completedPercent = (float)Math.Clamp((reversed ? ratio : 1.0 - ratio) * 100, 0, 100);
        }

        double igSeconds;
        double irlSeconds;

        if (props.IsRawSeconds)
        {
            igSeconds = props.HoursLeft;
            irlSeconds = props.HoursLeft;
        }
        else
        {
            float speedOfTime = api.World.Calendar.SpeedOfTime;
            float calendarMul = api.World.Calendar.CalendarSpeedMul;
            igSeconds = props.HoursLeft * 3600;
            irlSeconds = (igSeconds / speedOfTime) / calendarMul;
        }

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(props.HeaderKey)) sb.AppendLine(props.HeaderKey);

        if (props.ShowProgressBar)
        {
            ProgressBarBuilder.Build(sb, completedPercent);
            sb.AppendLine();
        }

        if (igSeconds > 0)
        {
            string igTimeStr = FormatFullTime(igSeconds);
            string irlTimeStr = FormatFullTime(irlSeconds);

            if (props.ShowInGameTime)
            {
                sb.AppendLine(Lang.Get("extrainfo:InGameTime", igTimeStr));
            }
            if (props.ShowRealTime)
            {
                sb.AppendLine(Lang.Get("extrainfo:InRealTime", irlTimeStr));
            }
        }

        return sb.ToString().TrimEnd();
    }
}