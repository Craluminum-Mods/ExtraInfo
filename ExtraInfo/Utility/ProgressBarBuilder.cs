using System;
using System.Text;

namespace ExtraInfo;

public static class ProgressBarBuilder
{
    public static void Build(StringBuilder sb, double progress)
    {
        progress = Math.Clamp(progress, 0.0, 100.0);

        int completedWidth = (int)Math.Round(progress / 100.0 * Config.ProgressBarWidth);
        int remainingWidth = Config.ProgressBarWidth - completedWidth;

        sb.Append(Config.ProgressBarStartCap);

        if (completedWidth > 0)
        {
            sb.Append(Config.ProgressBarFillChar, completedWidth);
        }

        if (remainingWidth > 0)
        {
            sb.Append(Config.ProgressBarEmptyChar, remainingWidth);
        }

        sb.Append(Config.ProgressBarEndCap + " ");
        sb.Append(progress.ToString("F0"));
        sb.Append('%');
    }

    public static string Build(double progress)
    {
        StringBuilder sb = new();
        Build(sb, progress);
        return sb.ToString();
    }
}