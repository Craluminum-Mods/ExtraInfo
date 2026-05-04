using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;

namespace ExtraInfo.Systems.BreakingTime;

[HarmonyPatch(typeof(Block), nameof(Block.GetPlacedBlockInfo))]
public static class BlockBreakingTimePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Config?.ShowBlockBreakingTime != true) return;

        var damagedBlocks = (world as ClientMain)?.GetField<Dictionary<BlockPos, BlockDamage>>("damagedBlocks");
        if (damagedBlocks == null || !damagedBlocks.TryGetValue(pos, out var damage)) return;

        float total = world.BlockAccessor.GetBlock(pos).GetResistance(world.BlockAccessor, pos);
        if (total <= 0) return;

        float completedPercent = 100f - Math.Clamp((damage.RemainingResistance / total) * 100f, 0, 100);

        StringBuilder sb = new(__result);
        sb.AppendLine();
        sb.AppendLine(Lang.Get("extrainfo:block-info-header-until-broken"));
        ProgressBar.Build(sb, completedPercent, width: 15);
        __result = sb.ToString().TrimEnd();
    }
}