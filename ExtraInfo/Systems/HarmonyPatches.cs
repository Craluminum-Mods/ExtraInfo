namespace ExtraInfo;

public class HarmonyPatches : ModSystem
{
    public Harmony HarmonyInstance => new(Mod.Info.ModID);

    public override void StartClientSide(ICoreClientAPI api)
    {
        HarmonyInstance.PatchCategory("Other");
        HarmonyInstance.PatchCategory("OpenHandbookForEntity");
        HarmonyInstance.PatchCategory("RemoveTradeHandbookInfo");
    }

    public override void Dispose()
    {
        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }
}