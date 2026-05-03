namespace ExtraInfo;

public static class TextExtensions
{
    public static string ColorText(string text) => Lang.Get("extrainfo:Color.Cyan", text);

    public static string GetMinMax(NatFloat natFloat)
    {
        int min = GetMin(natFloat);
        int max = GetMax(natFloat);
        return min == max ? $"{min}" : string.Format("{0} - {1}", min, max);
    }

    public static int GetMin(NatFloat natFloat) => (int)Math.Max(0, Math.Floor(natFloat.avg - natFloat.var));
    public static int GetMax(NatFloat natFloat) => (int)Math.Max(1, Math.Ceiling(natFloat.avg + natFloat.var));

    public static string GetMinMaxPercent(PanningDrop drop, float extraMul)
    {
        float min = (drop.Chance.avg - drop.Chance.var) * extraMul * 100;
        float max = (drop.Chance.avg + drop.Chance.var) * extraMul * 100;
        min = (float)Math.Round(min, 5);
        max = (float)Math.Round(max, 5);
        return min == max ? $"{min} %" : string.Format("{0} - {1} %", min, max);
    }
}