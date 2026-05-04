using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo.Systems.Handbook.HandbookPanningDrops;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookPanningDropsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddPanningDropsInfo(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddPanningDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookPanningDrops != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockPan blockPan) return;

        Dictionary<ItemStack[], PanningDrop[]> panningDrops = GetPanningDrops(capi, blockPan);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:PanningDrops"));

        List<RichTextComponentBase> richText = [];

        foreach (KeyValuePair<ItemStack[], PanningDrop[]> keyVal in panningDrops)
        {
            List<ItemStack[]> stackLists = keyVal.Key.GroupBy(stack => stack.Collectible.FirstCodePart()).Select(group => group.ToArray()).ToList();
            foreach (ItemStack[] stacks in stackLists)
            {
                richText.AddStacks(capi, openDetailPageFor, stacks);
            }

            richText.AddEqualSign(capi);
            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));

            int count = 3;
            foreach (PanningDrop drop in panningDrops[keyVal.Key])
            {
                if (!drop.Resolve(capi.World, "") || drop.ResolvedItemstack == null) continue;
                richText.AddStack(capi, openDetailPageFor, drop.ResolvedItemstack);

                float extraMul = drop.DropModbyStat is null ? 1f : capi.World.Player.Entity.Stats.GetBlended(drop.DropModbyStat);

                count--;
                richText.Add(new RichTextComponent(capi, GetMinMaxPercent(drop, extraMul) + (count == 0 ? "\n" : "\t\t\t\t\t"), CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });

                if (count == 0) count = 3;
            }

            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));
        }

        list.AddRange(richText);
    }

    private static Dictionary<ItemStack[], PanningDrop[]> GetPanningDrops(ICoreClientAPI capi, BlockPan blockPan)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "blockPanDrops-" + blockPan.Code, delegate
        {
            Dictionary<string, PanningDrop[]> dropsBySourceMat = blockPan.GetField<Dictionary<string, PanningDrop[]>>("dropsBySourceMat");

            Dictionary<ItemStack[], PanningDrop[]> panningDrops = [];

            foreach (string key in dropsBySourceMat.Keys)
            {
                List<ItemStack> blockStacks = [];
                foreach (Block block in capi.World.Blocks.Where(code => code != null && code.WildCardMatch(key)))
                {
                    string? rocktype = block.Variant["rock"];

                    foreach (PanningDrop drop in dropsBySourceMat[key])
                    {
                        if (drop.Code.Path.Contains("{rocktype}"))
                        {
                            drop.Code.Path = drop.Code.Path.Replace("{rocktype}", rocktype);
                        }
                    }

                    ItemStack stack = new(block);
                    if (!stack.ResolveBlockOrItem(capi.World)) continue;
                    if (stack.Collectible.Variant["layer"] != null) continue;
                    blockStacks.Add(stack);
                }
                panningDrops.Add(blockStacks.ToArray(), dropsBySourceMat[key]);
                blockStacks = [];
            }
            return panningDrops;
        });
    }
}