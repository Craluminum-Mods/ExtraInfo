using HarmonyLib;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookTraderGoods;

[HarmonyPatch(typeof(TradeHandbookInfo), "AddTraderHandbookInfo")]
public static class RemoveVanillaTradeHandbookInfoPatch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        if (Config?.ShowHandbookTraderGoods != true)
        {
            return true;
        }
        return false;
    }
}