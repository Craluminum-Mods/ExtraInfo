using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent.Mechanics;

namespace ExtraInfo.Systems.MechanicalBlockInfo;

[HarmonyPatch(typeof(BlockEntity), nameof(BlockEntity.GetBlockInfo))]
public static class MechanicalBlockInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntity __instance, StringBuilder dsc)
    {
        if (Config?.ShowMechanicalBlockInfo != true) return;

        MechanicalNetwork? network = __instance?.GetBehavior<BEBehaviorMPBase>()?.Network;
        if (network == null) return;

        dsc.AppendLine(Lang.Get("extrainfo:Mechanics.Speed", network.Speed));
        dsc.AppendLine(Lang.Get("extrainfo:Mechanics.TotalAvailableTorque", network.TotalAvailableTorque));
        dsc.AppendLine(Lang.Get("extrainfo:Mechanics.NetworkTorque", network.NetworkTorque));
        dsc.AppendLine(Lang.Get("extrainfo:Mechanics.NetworkResistance", network.NetworkResistance));
    }
}
