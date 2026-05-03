namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class PlacedBlockInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        __result = __result.GetCokeInfo(world, pos);
        __result = __result.GetSteelInfo(world, pos);
        __result = __result.GetCharcoalPitInfo(world, pos);
        __result = __result.GetBeehiveKilnInfo(world, pos);
    }
}
