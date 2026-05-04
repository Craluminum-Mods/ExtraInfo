using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookWorkableTemp;

[HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
public static class HandbookWorkableTempPatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
    {
        if (Config?.ShowHandbookWorkableTemp != true) return;
        if (inSlot.Itemstack?.Collectible?.GetCollectibleInterface<IAnvilWorkable>() is not { }) return;

        float temperature = inSlot.Itemstack.Collectible.GetTemperature(world, inSlot.Itemstack);
        float meltingpoint = inSlot.Itemstack.Collectible.GetMeltingPoint(world, null, inSlot);

        float workableTemp = (inSlot.Itemstack.ItemAttributes?["workableTemperature"].Exists) switch
        {
            true => inSlot.Itemstack.ItemAttributes["workableTemperature"].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(Lang.Get("extrainfo:AlwaysWorkable")),
            _ => dsc.AppendLine(Lang.Get("extrainfo:WorkableTemperature", workableTemp))
        };

        if (inSlot is DummySlot || inSlot is ItemSlotCreative) return;

        if (temperature < workableTemp)
        {
            dsc.AppendLine(Lang.Get("Too cold to work"));
        }
    }
}
