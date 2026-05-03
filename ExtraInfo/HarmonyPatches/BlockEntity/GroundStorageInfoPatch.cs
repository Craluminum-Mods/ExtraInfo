namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
public static class GroundStorageInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc)
    {
        dsc.GetGroundStorageInfo(__instance);
    }
}