namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityPitKiln), nameof(BlockEntityPitKiln.GetBlockInfo))]
public static class PitKilnInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityPitKiln __instance, StringBuilder dsc)
    {
        dsc.GetPitKilnInfo(__instance);
    }
}