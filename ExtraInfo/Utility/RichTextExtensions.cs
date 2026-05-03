using Cairo;

namespace ExtraInfo;

public static class RichTextExtensions
{
    public static void AddTraderInfo(this List<RichTextComponentBase> richText, ICoreClientAPI capi, TradeItem val, ActionConsumable<string> openDetailPageFor, ItemStack gear)
    {
        richText.AddStack(capi, openDetailPageFor, val.ResolvedItemstack, showStacksize: true);
        richText.Add(new RichTextComponent(capi, "\t" + GetMinMax(val.Stock), CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
        richText.AddEqualSign(capi);
        richText.AddStack(capi, openDetailPageFor, gear);
        richText.Add(new RichTextComponent(capi, GetMinMax(val.Price) + "\n", CairoFont.WhiteSmallText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }

    public static void AddMarginAndTitle(this List<RichTextComponentBase> list, ICoreClientAPI capi, float marginTop, string titletext)
    {
        list.Add(new ClearFloatTextComponent(capi, marginTop));
        list.Add(new RichTextComponent(capi, titletext + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold)));
    }

    public static void AddEqualSign(this List<RichTextComponentBase> richText, ICoreClientAPI capi)
    {
        richText.Add(new RichTextComponent(capi, " = ", CairoFont.WhiteMediumText())
        {
            VerticalAlign = EnumVerticalAlign.Middle
        });
    }

    public static void AddStack(this List<RichTextComponentBase> richText, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack stack, bool showStacksize = false)
    {
        richText.Add(new ItemstackTextComponent(capi, stack, 40, 0, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)))
        {
            ShowStacksize = showStacksize
        });
    }

    public static void AddStacks(this List<RichTextComponentBase> richText, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack[] stacks, bool showStacksize = false)
    {
        richText.Add(new SlideshowItemstackTextComponent(capi, stacks, 40, EnumFloat.Inline, (cs) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)))
        {
            ShowStackSize = showStacksize
        });
    }
}