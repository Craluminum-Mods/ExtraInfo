namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityStaticTranslocator), nameof(BlockEntityStaticTranslocator.GetBlockInfo))]
public static class TranslocatorInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityStaticTranslocator __instance, StringBuilder dsc)
    {
        dsc.GetTranslocatorInfo(__instance);
    }
}