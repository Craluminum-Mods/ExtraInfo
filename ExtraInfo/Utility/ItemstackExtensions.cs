namespace ExtraInfo;

public static class ItemstackExtensions
{
    public static ItemStack GetCreatureStack(this EntityProperties entityType, ICoreClientAPI capi)
    {
        AssetLocation location = entityType.Code.Clone().WithPathPrefix("creature-");
        Item item = capi.World.GetItem(location);
        return item == null ? null : new ItemStack(item);
    }

    public static List<List<ItemStack>> GroupStacksByFirstCodePart(this List<ItemStack> stacks)
    {
        Dictionary<string, List<ItemStack>> groups = new();

        foreach (ItemStack stack in stacks)
        {
            string firstCodePart = stack.Collectible.FirstCodePart();
            if (!groups.ContainsKey(firstCodePart))
            {
                groups[firstCodePart] = new List<ItemStack>();
            }

            groups[firstCodePart].Add(stack);
        }

        return groups.Values.ToList();
    }

    public static List<List<ItemStack>> GetGroupedCreatureStacks(this List<EntityProperties> entityTypes, ICoreClientAPI capi)
    {
        Dictionary<string, List<ItemStack>> groups = new();
        foreach (EntityProperties entityType in entityTypes)
        {
            ItemStack stack = entityType.GetCreatureStack(capi);
            if (stack == null)
            {
                continue;
            }

            string groupcode = entityType.Attributes?["handbook"]?["groupcode"].AsString();
            if (string.IsNullOrEmpty(groupcode))
            {
                groupcode = entityType.Code;
            }

            if (!groups.ContainsKey(groupcode))
            {
                groups[groupcode] = new List<ItemStack>();
            }

            groups[groupcode].Add(stack);

        }
        return groups.Values.ToList();
    }
}