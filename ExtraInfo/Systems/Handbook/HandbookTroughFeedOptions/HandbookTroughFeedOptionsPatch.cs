using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookTroughFeedOptions;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookTroughFeedOptionsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddTroughInfo(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddTroughInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookTroughFeedOptions != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockTroughBase blockTrough) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:trough-validfeed"));

        foreach (ContentConfig config in blockTrough.contentConfigs)
        {
            if (config.Content.Code.ToShortString().Contains("-*"))
            {
                List<ItemStack> stacks = GetWildcardTroughStacks(capi, config);
                list.AddStacks(capi, openDetailPageFor, [.. stacks]);
            }
            else
            {
                if (config.Content.ResolvedItemstack != null)
                {
                    list.AddStack(capi, openDetailPageFor, config.Content.ResolvedItemstack);
                }
            }
        }
    }

    private static List<ItemStack> GetWildcardTroughStacks(ICoreClientAPI capi, ContentConfig config)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "troughWildcardStacks-" + config.Code, delegate
        {
            List<ItemStack> stacks = [];
            foreach (CollectibleObject obj in capi.World.Collectibles.Where(x => x.WildCardMatch(config.Content.Code)))
            {
                ItemStack stack = new(obj);
                if (!stack.ResolveBlockOrItem(capi.World)) continue;
                stacks.Add(stack);
            }
            return stacks;
        });
    }
}
