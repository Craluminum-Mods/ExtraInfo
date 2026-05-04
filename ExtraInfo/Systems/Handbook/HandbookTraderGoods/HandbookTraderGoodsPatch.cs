using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookTraderGoods;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookTraderGoodsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddTraderInfo(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddTraderInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookTraderGoods != true) return;

        CollectibleObject collObj = inSlot.Itemstack!.Collectible;
        if (collObj is ItemCreature itemCreature)
        {
            if (!TradeHandbookInfoSystem.unresolvedTradeProps.TryGetValue(itemCreature.Code, out TradeProperties? tradeProps) || tradeProps == null)
            {
                return;
            }

            ItemStack gear = new(capi.World.GetItem(new AssetLocation("gear-rusty")));
            List<TradeItem> buyingStacks = [.. tradeProps.Buying.List.Where(tradeItem => tradeItem.Resolve(capi.World, ""))];
            List<TradeItem> sellingStacks = [.. tradeProps.Selling.List.Where(tradeItem2 => tradeItem2.Resolve(capi.World, ""))];

            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("You can Sell"));
            List<RichTextComponentBase> richTextSell = [];
            foreach (TradeItem item in buyingStacks)
            {
                richTextSell.AddTraderInfo(capi, item, openDetailPageFor, gear);
            }
            list.AddRange(richTextSell);

            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("You can Buy"));
            List<RichTextComponentBase> richTextBuy = [];
            foreach (TradeItem item in sellingStacks)
            {
                richTextBuy.AddTraderInfo(capi, item, openDetailPageFor, gear);
            }
            list.AddRange(richTextBuy);
        }

        bool any = false;
        List<RichTextComponentBase> richTextSellBy = [];
        foreach ((AssetLocation traderCode, TradeProperties props) in TradeHandbookInfoSystem.unresolvedTradeProps)
        {
            if (props.Buying.List.Any(x => x.Code == collObj.Code) == true)
            {
                Item? traderItem = capi.World.GetItem(traderCode);
                if (traderItem == null) continue;

                ItemStack traderStack = new ItemStack(traderItem);
                richTextSellBy.AddStack(capi, openDetailPageFor, traderStack);
                any = true;
            }
        }
        if (any)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Purchased by"));
            list.AddRange(richTextSellBy);
        }

        any = false;
        List<RichTextComponentBase> richTextBuyBy = [];
        foreach ((AssetLocation traderCode, TradeProperties props) in TradeHandbookInfoSystem.unresolvedTradeProps)
        {
            if (props.Selling.List.Any(x => x.Code == collObj.Code) == true)
            {
                Item? traderItem = capi.World.GetItem(traderCode);
                if (traderItem == null) continue;

                ItemStack traderStack = new ItemStack(traderItem);
                richTextBuyBy.AddStack(capi, openDetailPageFor, traderStack);
                any = true;
            }
        }
        if (any)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Sold by"));
            list.AddRange(richTextBuyBy);
        }
    }

    private static void AddTraderInfo(this List<RichTextComponentBase> richText, ICoreClientAPI capi, TradeItem val, ActionConsumable<string> openDetailPageFor, ItemStack gear)
    {
        if (val.ResolvedItemstack == null) return;

        richText.AddStack(capi, openDetailPageFor, val.ResolvedItemstack, showStacksize: true);
        richText.Add(new RichTextComponent(capi, "\t" + TextExtensions.GetMinMax(val.Stock), CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
        richText.AddEqualSign(capi);
        richText.AddStack(capi, openDetailPageFor, gear);
        richText.Add(new RichTextComponent(capi, TextExtensions.GetMinMax(val.Price) + "\n", CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }
}