namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
public static class HeldItemInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
    {
        CollectibleObject obj = inSlot.Itemstack.Collectible;
        dsc.GetWorkableTempInfoForItem(inSlot, world);
    }
}
