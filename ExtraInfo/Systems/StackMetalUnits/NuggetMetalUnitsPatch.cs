using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.StackMetalUnits;

[HarmonyPatch(typeof(ItemNugget), nameof(ItemNugget.GetHeldItemInfo))]
public static class NuggetMetalUnitsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
    {
        if (Config?.ShowStackMetalUnits != true) return;
        if (inSlot.Itemstack == null || inSlot.StackSize <= 1) return;

        CombustibleProperties props = inSlot.Itemstack.Collectible.GetCombustibleProperties(world, inSlot.Itemstack, null);
        if (props?.SmeltedStack?.ResolvedItemstack == null) return;

        float unitsPerNugget = props.SmeltedStack.ResolvedItemstack.StackSize * 100f / props.SmeltedRatio;
        float totalUnits = unitsPerNugget * inSlot.StackSize;

        string smeltingType = props.SmeltingType.ToString().ToLowerInvariant();
        
        string? metalCode = props.SmeltedStack.ResolvedItemstack.Collectible.Variant?["metal"];
        string metalName = metalCode != null 
            ? Lang.Get("material-" + metalCode) 
            : props.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");

        dsc.AppendLine(Lang.Get("game:smeltdesc-" + smeltingType + "ore-plural", totalUnits.ToString("0.#"), metalName));
    }
}