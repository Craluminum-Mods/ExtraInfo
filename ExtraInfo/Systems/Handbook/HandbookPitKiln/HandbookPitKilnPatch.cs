using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookPitKiln;

[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class HandbookPitKilnPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = [.. __result];
        list.AddPitKilnInfo(inSlot, capi, openDetailPageFor);
        __result = [.. list];
    }

    private static void AddPitKilnInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Config?.ShowHandbookPitKiln != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockPitkiln blockPitKiln) return;

        List<JsonItemStackBuildStage> fuelStacks = blockPitKiln.GetPitKilnFuelStacks(capi);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Lang.Get("extrainfo:Fuel"));

        List<RichTextComponentBase> richText = [];

        foreach (JsonItemStackBuildStage fuel in fuelStacks)
        {
            if (fuel.ResolvedItemstack == null || fuel.BurnTimeHours == null) continue;
            richText.AddStack(capi, openDetailPageFor, fuel.ResolvedItemstack);
            richText.Add(new RichTextComponent(capi, Lang.Get("{0} hours", (float)fuel.BurnTimeHours) + "\n", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }


    private static List<JsonItemStackBuildStage> GetPitKilnFuelStacks(this BlockPitkiln blockPitKiln, ICoreClientAPI capi)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "pitKilnFuelStacks-" + blockPitKiln.Code, delegate
        {
            JsonItemStackBuildStage[]? fuelStacks = blockPitKiln?.Attributes?["buildMats"]?["fuel"]?.AsObject<JsonItemStackBuildStage[]>();
            if (fuelStacks == null) return [];

            List<JsonItemStackBuildStage> stacks = [.. fuelStacks.Where(stack => stack?.BurnTimeHours != null)];

            foreach (JsonItemStackBuildStage stack in stacks)
            {
                stack.Resolve(capi.World, "");
            }

            return stacks;
        });
    }
}