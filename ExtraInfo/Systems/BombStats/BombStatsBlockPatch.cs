using System;
using System.Collections.Generic;
using System.Text;

namespace ExtraInfo.Systems.BombStats;

[HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
public static class BombStatsBlockPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntity __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowBombStats != true) return;
        if (__instance is not BlockEntityBomb blockEntity) return;

        dsc.AppendLine(BombExtensions.BlastRadius(blockEntity.BlastRadius))
            .AppendLine(BombExtensions.InjureRadius(blockEntity.InjureRadius))
            .AppendLine(BombExtensions.BlastType(blockEntity.BlastType))
            .AppendLine(BombExtensions.FuseTimeSeconds(blockEntity.FuseTimeSeconds));
    }
}