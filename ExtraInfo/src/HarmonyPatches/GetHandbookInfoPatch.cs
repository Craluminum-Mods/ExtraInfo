namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo), nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo))]
public static class GetHandbookInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref RichTextComponentBase[] __result, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = __result.ToList();

        list.AddEntityDropsInfo(inSlot, capi, openDetailPageFor);
        list.AddEntityDropsInfoForDrop(inSlot, capi, openDetailPageFor);
        list.AddPanningDropsInfo(inSlot, capi, openDetailPageFor);
        list.AddTroughInfo(inSlot, capi, openDetailPageFor);
        list.AddPitKilnInfo(inSlot, capi, openDetailPageFor);
        list.AddTraderInfo(inSlot, capi, openDetailPageFor);
        list.AddEntitiesThatEatCollectible(inSlot, capi, openDetailPageFor);
        list.AddEntityDietInfo(inSlot, capi, openDetailPageFor);

        __result = list.ToArray();
    }
}