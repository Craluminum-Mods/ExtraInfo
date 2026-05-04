using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookEatableByCreatures;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookEatableByCreaturesPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddEntitiesThatEatCollectible(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddEntitiesThatEatCollectible(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookEatableByCreatures != true) return;

        List<EntityProperties> entityTypes = [];
        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            CreatureDiet? creatureDiet = entityType.Attributes?["creatureDiet"].AsObject<CreatureDiet>();
            if (creatureDiet?.Matches(inSlot.Itemstack) == true)
            {
                entityTypes.Add(entityType);
            }
        }

        if (entityTypes.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("Eaten by"));

        List<List<ItemStack>> groupedStacks = entityTypes.GetGroupedCreatureStacks(capi);
        for (int i = 0; i < groupedStacks.Count; i++)
        {
            if (groupedStacks[i].Count == 1)
            {
                list.AddStack(capi, openDetailPageFor, groupedStacks[i].First());
            }
            else
            {
                list.AddStacks(capi, openDetailPageFor, [.. groupedStacks[i]]);
            }
        }
    }
}