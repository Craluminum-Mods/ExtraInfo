using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookCreatureDiet;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookCreatureDietPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddEntityDietInfo(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddEntityDietInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookCreatureDiet != true) return;
        if (inSlot.Itemstack!.Collectible is not ItemCreature itemCreature) return;

        EntityProperties? entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        CreatureDiet? creatureDiet = entityType?.Attributes?["creatureDiet"].AsObject<CreatureDiet>();
        if (creatureDiet == null) return;

        List<ItemStack> stacks = [];
        foreach (CollectibleObject obj in capi.World.Collectibles)
        {
            if (obj == null || obj.Code == null) continue;

            List<ItemStack> _stacks = obj.GetHandBookStacks(capi);
            if (_stacks == null) continue;

            foreach (ItemStack _stack in _stacks)
            {
                if (creatureDiet.Matches(_stack))
                {
                    stacks.Add(_stack);
                }
            }
        }

        if (stacks.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:Food"));

        List<List<ItemStack>> groupedStacks = stacks.GroupStacksByFirstCodePart();
        for (int i = 0; i < groupedStacks.Count; i++)
        {
            if (groupedStacks[i].Count == 1)
            {
                list.AddStack(capi, openDetailPageFor, groupedStacks[i].First());
            }
            else
            {
                list.AddStacks(capi, openDetailPageFor, groupedStacks[i].ToArray());
            }
        }
    }
}