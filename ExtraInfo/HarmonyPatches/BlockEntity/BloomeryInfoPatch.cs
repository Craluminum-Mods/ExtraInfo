namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityBloomery), nameof(BlockEntityBloomery.GetBlockInfo))]
public static class BloomeryInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBloomery __instance, StringBuilder dsc)
    {
        dsc.GetBloomeryInfo(__instance);
    }
}