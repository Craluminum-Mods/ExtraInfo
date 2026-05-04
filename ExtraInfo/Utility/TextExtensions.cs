using System;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public static class TextExtensions
{
    public static string GetMinMax(NatFloat natFloat)
    {
        int min = GetMin(natFloat);
        int max = GetMax(natFloat);
        return min == max ? $"{min}" : string.Format("{0} - {1}", min, max);
    }

    public static int GetMin(NatFloat natFloat) => (int)Math.Max(0, Math.Floor(natFloat.avg - natFloat.var));
    public static int GetMax(NatFloat natFloat) => (int)Math.Max(1, Math.Ceiling(natFloat.avg + natFloat.var));
}