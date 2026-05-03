namespace ExtraInfo;

public static class StyleExtensions
{
    public static int[] LiquidColorGradient;

    static StyleExtensions()
    {
        int[] array = new int[11]
        {
            ColorUtil.Hex2Int("#FFFFFF"),
            ColorUtil.Hex2Int("#E6F0FF"),
            ColorUtil.Hex2Int("#CCE0FF"),
            ColorUtil.Hex2Int("#B3D1FF"),
            ColorUtil.Hex2Int("#99C2FF"),
            ColorUtil.Hex2Int("#80B3FF"),
            ColorUtil.Hex2Int("#66A3FF"),
            ColorUtil.Hex2Int("#4D94FF"),
            ColorUtil.Hex2Int("#3385FF"),
            ColorUtil.Hex2Int("#1A75FF"),
            ColorUtil.Hex2Int("#0066FF")
        };

        LiquidColorGradient = new int[100];
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                LiquidColorGradient[10 * i + j] = ColorUtil.ColorOverlay(array[i], array[i + 1], (float)j / 10f);
            }
        }
    }
}