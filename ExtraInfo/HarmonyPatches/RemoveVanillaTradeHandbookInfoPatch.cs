namespace ExtraInfo;

[HarmonyPatchCategory("RemoveTradeHandbookInfo")]
[HarmonyPatch(typeof(TradeHandbookInfo), "AddTraderHandbookInfo")]
public static class RemoveVanillaTradeHandbookInfoPatch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        if (Core.Config == null || !Core.Config.ShowHandbookTraderGoods)
        {
            return true;
        }

        return false;
    }
}