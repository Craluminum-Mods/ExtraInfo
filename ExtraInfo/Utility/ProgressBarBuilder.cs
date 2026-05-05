using System;
using System.Text;

namespace ExtraInfo;

public static class ProgressBarBuilder
{
    /// <summary>
    /// Generates a progress bar string using an integer percentage.
    /// </summary>
    /// <returns>A formatted string: [████░░░░] 50%</returns>
    public static string Get(int progress, int width = 20) => Get((double)progress, width);

    /// <summary>
    /// Generates a progress bar string using a float percentage.
    /// </summary>
    /// <returns>A formatted string: [████░░░░] 50%</returns>
    public static string Get(float progress, int width = 20) => Get((double)progress, width);

    /// <summary>
    /// Generates a progress bar string using a double percentage.
    /// </summary>
    /// <returns>A formatted string: [████░░░░] 50%</returns>
    public static string Get(double progress, int width = 20)
    {
        StringBuilder sb = new StringBuilder(width + 15);
        Build(sb, progress, width);
        return sb.ToString();
    }

    public static void Build(StringBuilder sb, double progress, int width = 20)
    {
        progress = Math.Clamp(progress, 0.0, 100.0);

        int completedWidth = (int)(progress / 100.0 * width);
        int remainingWidth = width - completedWidth;

        sb.Append('[');
        sb.Append('█', completedWidth);
        sb.Append('░', remainingWidth);
        sb.Append("] ");

        sb.Append(progress.ToString("F0"));
        sb.Append('%');
    }
}