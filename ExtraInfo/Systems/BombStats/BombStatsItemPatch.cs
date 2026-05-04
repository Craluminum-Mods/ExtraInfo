namespace ExtraInfo.Systems.BombStats;

[HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
public static class BombStatsItemPatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
    {
        if (Core.Config?.ShowBombStats != true) return;
        if (inSlot.Itemstack?.Block is not BlockBomb) return;

        dsc.AppendLine(BombExtensions.BlastRadius(inSlot.Itemstack.ItemAttributes["blastRadius"].AsInt()))
            .AppendLine(BombExtensions.InjureRadius(inSlot.Itemstack.ItemAttributes["injureRadius"].AsInt()))
            .AppendLine(BombExtensions.BlastType(inSlot.Itemstack.ItemAttributes["blastType"].AsObject<EnumBlastType>()));
    }
}