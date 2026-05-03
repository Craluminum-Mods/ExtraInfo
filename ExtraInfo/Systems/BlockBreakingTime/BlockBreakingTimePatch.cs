namespace ExtraInfo.Systems.BreakingTimeInfo;

[HarmonyPatchCategory("Other")]
[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class BlockBreakingTimePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config?.ShowBlockBreakingTime != true) return;

        var damagedBlocks = (world as ClientMain)?.GetField<Dictionary<BlockPos, BlockDamage>>("damagedBlocks");
        if (damagedBlocks == null || !damagedBlocks.TryGetValue(pos, out var damage)) return;

        float total = world.BlockAccessor.GetBlock(pos).GetResistance(world.BlockAccessor, pos);
        if (total <= 0) return;

        float completedPercent = 100f - Math.Clamp((damage.RemainingResistance / total) * 100f, 0, 100);

        StringBuilder barBuilder = new StringBuilder();
        ProgressBar.Build(barBuilder, completedPercent, width: 15);

        string translatedLine = Lang.Get("extrainfo:UntilBroken", barBuilder.ToString());

        StringBuilder sb = new(__result);
        sb.AppendLine().Append(translatedLine);

        __result = sb.ToString().TrimEnd();
    }
}