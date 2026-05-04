using Cairo;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ExtraInfo;

public static class RichTextExtensions
{
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