namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityBeehive), nameof(BlockEntityBeehive.GetBlockInfo))]
public static class SkepInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBeehive __instance, StringBuilder dsc)
    {
        dsc.GetSkepInfo(__instance);
    }
}