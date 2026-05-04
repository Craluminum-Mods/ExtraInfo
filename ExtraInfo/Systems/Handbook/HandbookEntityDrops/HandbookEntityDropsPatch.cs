using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo.Systems.Handbook.HandbookEntityDrops;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookEntityDropsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];

        list.AddEntityDropsInfo(inSlot, capi, openDetailPageFor);
        list.AddEntityDropsInfoForDrop(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddEntityDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookEntityDrops != true) return;
        if (inSlot.Itemstack!.Collectible is not ItemCreature itemCreature) return;

        EntityProperties? entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        if (entityType == null) return;

        List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
        if (harvestStacks != null && harvestStacks.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Obtained by killing & harvesting"));

            List<RichTextComponentBase> richTextHarvest = [];

            foreach (BlockDropItemStack stack in harvestStacks)
            {
                if (stack.ResolvedItemstack == null) continue;
                richTextHarvest.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextHarvest.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextHarvest);
        }

        if (entityType.Drops != null && entityType.Drops.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Obtained by killing"));

            List<RichTextComponentBase> richTextOther = [];

            foreach (BlockDropItemStack stack in entityType.Drops)
            {
                if (stack.ResolvedItemstack == null) continue;
                richTextOther.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextOther.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextOther);
        }
    }

    private static void AddEntityDropsInfoForDrop(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookEntityDrops != true) return;

        CollectibleObject collObj = inSlot.Itemstack!.Collectible;

        List<RichTextComponentBase> richTextHarvest = [];
        List<RichTextComponentBase> richTextDrop = [];

        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
            if (harvestStacks?.Count != 0 && harvestStacks?.Find(stack => stack?.Code == collObj?.Code) != null)
            {
                ItemStack? stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextHarvest.AddStack(capi, openDetailPageFor, stack);
            }

            if (entityType.Drops.Length != 0 && entityType.Drops.ToList().Find(stack => stack.Code == collObj.Code) != null)
            {
                ItemStack? stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextDrop.AddStack(capi, openDetailPageFor, stack);
            }
        }

        if (richTextHarvest.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Obtained by killing & harvesting"));
            list.AddRange(richTextHarvest);
        }

        if (richTextDrop.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Obtained by killing"));
            list.AddRange(richTextDrop);
        }
    }

    private static List<BlockDropItemStack> GetHarvestableDrops(ICoreClientAPI capi, EntityProperties entityType)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "harvestableDrops-" + entityType.Code, delegate
        {
            BlockDropItemStack[]? harvestableDrops = entityType.Attributes?["harvestableDrops"]?.AsObject<BlockDropItemStack[]>();
            if (harvestableDrops == null) return [];

            BlockDropItemStack[] array = harvestableDrops;
            foreach (BlockDropItemStack hstack in array)
            {
                hstack.Resolve(capi.World, "handbook info", new AssetLocation());
            }
            return array.ToList();
        });
    }
}
