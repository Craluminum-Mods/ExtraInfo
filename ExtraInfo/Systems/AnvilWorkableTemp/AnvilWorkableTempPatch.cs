using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.AnvilWorkableTemp;

[HarmonyPatch(typeof(BlockEntityAnvil), nameof(BlockEntityAnvil.GetBlockInfo))]
public static class AnvilWorkableTempPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityAnvil __instance, StringBuilder dsc)
    {
        if (Config?.ShowAnvilWorkableTemp != true) return;
        if (__instance?.WorkItemStack == null || __instance.SelectedRecipe == null) return;

        ItemStack stack = __instance.WorkItemStack;

        float meltingpoint = stack.Collectible.GetMeltingPoint(__instance.Api.World, null, new DummySlot(stack));

        float workableTemp = (stack.Collectible.Attributes?["workableTemperature"].Exists) switch
        {
            true => stack.Collectible.Attributes["workableTemperature"].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(Lang.Get("extrainfo:AlwaysWorkable")),
            _ => dsc.AppendLine(Lang.Get("extrainfo:WorkableTemperature", workableTemp))
        };
    }
}