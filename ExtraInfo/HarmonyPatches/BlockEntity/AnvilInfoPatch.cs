namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityAnvil), nameof(BlockEntityAnvil.GetBlockInfo))]
public static class AnvilInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityAnvil __instance, StringBuilder dsc)
    {
        dsc.GetWorkableTempInfoForAnvil(__instance);
    }
}