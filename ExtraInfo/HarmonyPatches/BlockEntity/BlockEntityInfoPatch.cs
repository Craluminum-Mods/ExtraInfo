namespace ExtraInfo;

[HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
public static class BlockEntityInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntity __instance, StringBuilder dsc)
    {
        dsc.GetMechanicalBlockInfo(__instance);
    }
}