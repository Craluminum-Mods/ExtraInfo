using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookPanningDrops;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static partial class HandbookPanningDropsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookPanningDrops != true) return;
        if (inSlot.Itemstack?.Collectible is not BlockPan blockPan) return;

        List<RichTextComponentBase> list = [.. __result];
        var panningGroups = GetPanningDrops(capi, blockPan);

        list.AddMarginAndTitle(capi, marginTop: 10, titletext: Lang.Get("extrainfo:PanningDrops"));

        foreach (var group in panningGroups)
        {
            list.AddStacks(capi, openDetailPageFor, group.Sources);

            list.AddEqualSign(capi);

            int count = 0;
            foreach (var dropObj in group.Drops)
            {
                if (dropObj is PanningDropExt drop)
                {
                    if (!drop.Resolve(capi.World, "panning drop handbook") || drop.ResolvedItemstack == null) continue;

                    list.AddStack(capi, openDetailPageFor, drop.ResolvedItemstack);

                    float extraMul = drop.DropModbyStat == null ? 1f : capi.World.Player.Entity.Stats.GetBlended(drop.DropModbyStat);

                    count++;
                    string chanceText = drop.GetMinMaxPercent(extraMul);
                    string spacing = (count % 3 == 0) ? "\n" : "    ";

                    list.Add(new RichTextComponent(capi, chanceText + spacing, CairoFont.WhiteSmallText())
                    {
                        VerticalAlign = EnumVerticalAlign.Middle
                    });
                }
                else if (dropObj is PanningDropExt[] drops)
                {
                    List<PanningDropExt> toDrops = [];
                    foreach (var dropToResolve in drops)
                    {
                        if (!dropToResolve.Resolve(capi.World, "panning drop handbook") || dropToResolve.ResolvedItemstack == null) continue;
                        toDrops.Add(dropToResolve);
                    }

                    list.AddStacks(capi, openDetailPageFor, [.. toDrops.Select(x => x.ResolvedItemStack)!]);

                    float extraMul = toDrops[0].DropModbyStat == null ? 1f : capi.World.Player.Entity.Stats.GetBlended(toDrops[0].DropModbyStat);

                    count++;
                    string chanceText = toDrops[0].GetMinMaxPercent(extraMul);
                    string spacing = (count % 3 == 0) ? "\n" : "    ";

                    list.Add(new RichTextComponent(capi, chanceText + spacing, CairoFont.WhiteSmallText())
                    {
                        VerticalAlign = EnumVerticalAlign.Middle
                    });
                }

            }
            list.Add(new ClearFloatTextComponent(capi, 10));
        }

        __result = [.. list];
    }

    private static List<PanningGroup> GetPanningDrops(ICoreClientAPI capi, BlockPan blockPan)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "blockPanDropsExtList-" + blockPan.Code, () =>
        {
            var dropsBySourceMat = blockPan.GetField<Dictionary<string, PanningDrop[]>>("dropsBySourceMat");
            var finalGroups = new List<PanningGroup>();

            if (dropsBySourceMat == null) return finalGroups;

            foreach (var entry in dropsBySourceMat)
            {
                List<Block> matchingBlocks = [.. capi.World.Blocks.Where(b => b?.Code != null && b.WildCardMatch(entry.Key) && !b.Variant.ContainsKey("layer"))];

                if (matchingBlocks.Count == 0) continue;

                object[] resolvedDrops = new object[entry.Value.Length];

                for (int i = 0; i < entry.Value.Length; i++)
                {
                    var original = entry.Value[i];

                    if (original.Code.Path.Contains("{rocktype}"))
                    {
                        List<PanningDropExt> variantDrops = [];
                        foreach (var block in matchingBlocks)
                        {
                            string rock = block.Variant.Get("rock", "granite");
                            var ext = PanningDropExt.FromOriginal(original);
                            ext.FillPlaceHolder("rocktype", rock);
                            variantDrops.Add(ext);
                        }
                        resolvedDrops[i] = variantDrops.ToArray();
                    }
                    else
                    {
                        var ext = PanningDropExt.FromOriginal(original);
                        resolvedDrops[i] = ext;
                    }
                }

                finalGroups.Add(new PanningGroup
                {
                    Sources = [.. matchingBlocks.Select(b => new ItemStack(b))],
                    Drops = resolvedDrops
                });
            }
            return finalGroups;
        });
    }

    private static string GetMinMaxPercent(this PanningDropExt drop, float extraMul)
    {
        NatFloat chance = drop.Chance;
        float min, max;

        if (chance.dist == EnumDistribution.INVEXP ||
            chance.dist == EnumDistribution.STRONGINVEXP ||
            chance.dist == EnumDistribution.STRONGERINVEXP)
        {
            min = chance.avg;
            max = chance.avg + chance.var;
        }
        else
        {
            min = chance.avg - chance.var;
            max = chance.avg + chance.var;
        }

        min = (float)Math.Max(0, Math.Round(min * extraMul * 100, 4));
        max = (float)Math.Max(0, Math.Round(max * extraMul * 100, 4));

        return min == max ? $"{min}%" : $"{min}-{max}%";
    }
}