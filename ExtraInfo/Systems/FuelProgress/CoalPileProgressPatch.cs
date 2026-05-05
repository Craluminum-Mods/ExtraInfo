using HarmonyLib;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.FuelProgress;

[HarmonyPatch(typeof(BlockEntityItemPile), nameof(BlockEntityItemPile.GetBlockInfo))]
public static class CoalPileProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityItemPile __instance, StringBuilder dsc)
    {
        if (Config?.ShowFuelProgress != true) return;

        if (__instance is not BlockEntityCoalPile coalPile) return;

        if (!coalPile.IsBurning) return;

        dsc.AppendLine(); 
        InfoExtensions.AppendFuelProgress(dsc, coalPile, Lang.Get("extrainfo:Fuel"));
    }
}