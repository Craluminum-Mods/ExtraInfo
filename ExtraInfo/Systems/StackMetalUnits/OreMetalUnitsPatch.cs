namespace ExtraInfo.Systems.StackMetalUnits;

[HarmonyPatch(typeof(ItemOre), nameof(ItemOre.GetHeldItemInfo))]
public static class OreMetalUnitsPatch
{
    [HarmonyPostfix]
    public static void Postfix(ItemSlot inSlot, IWorldAccessor world, StringBuilder dsc)
    {
        if (Core.Config?.ShowStackMetalUnits != true) return;
        if (inSlot.Itemstack == null || inSlot.StackSize <= 1) return;

        var attr = inSlot.Itemstack.ItemAttributes;
        if (attr == null || !attr["metalUnits"].Exists) return;

        int unitsPerItem = attr["metalUnits"].AsInt();
        float totalUnits = unitsPerItem * inSlot.StackSize;

        string metalPart = inSlot.Itemstack.Collectible.LastCodePart(1);
        if (metalPart.Contains("_")) metalPart = metalPart.Split('_')[1];

        AssetLocation nuggetLoc = new AssetLocation("nugget-" + metalPart);
        Item? itemNugget = world.GetItem(nuggetLoc);

        string metalName;
        string smeltingType = "smelt";

        if (itemNugget != null)
        {
            ItemStack tempStack = new ItemStack(itemNugget);
            var nProps = itemNugget.GetCombustibleProperties(world, tempStack, null);

            if (nProps?.SmeltedStack?.ResolvedItemstack != null)
            {
                smeltingType = nProps.SmeltingType.ToString().ToLowerInvariant();

                string? metalVariant = nProps.SmeltedStack.ResolvedItemstack.Collectible?.Variant?["metal"];
                metalName = metalVariant != null
                    ? Lang.Get("material-" + metalVariant)
                    : nProps.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");
            }
            else
            {
                metalName = itemNugget.GetHeldItemName(tempStack);
            }
        }
        else
        {
            metalName = metalPart;
        }

        string langKey = "game:smeltdesc-" + smeltingType + "ore-plural";
        dsc.AppendLine(ColorText(Lang.Get(langKey, totalUnits.ToString("0.#"), metalName)));
    }
}