namespace ExtraInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(BlockEntityFarmland), nameof(BlockEntityFarmland.GetBlockInfo))]
public static class FarmlandInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityFarmland __instance, StringBuilder dsc)
    {
        dsc.GetFarmlandDropSoilChanceInfo(__instance);
        dsc.GetFarmlandInfo(__instance);
    }
}