using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace ExtraInfo;

public static class ItemstackExtensions
{
    public static ItemStack? GetCreatureStack(this EntityProperties? entityType, ICoreClientAPI capi)
    {
        if (entityType == null) return null;
        AssetLocation location = entityType.Code.Clone().WithPathPrefix("creature-");
        Item? item = capi.World.GetItem(location);
        if (item == null) return null;
        return new ItemStack(item);
    }

    public static List<List<ItemStack>> GroupStacksByFirstCodePart(this List<ItemStack> stacks)
    {
        Dictionary<string, List<ItemStack>> groups = [];

        foreach (ItemStack stack in stacks)
        {
            string firstCodePart = stack.Collectible.FirstCodePart();
            if (!groups.ContainsKey(firstCodePart))
            {
                groups[firstCodePart] = [];
            }

            groups[firstCodePart].Add(stack);
        }

        return [.. groups.Values];
    }

    public static List<List<ItemStack>> GetGroupedCreatureStacks(this List<EntityProperties> entityTypes, ICoreClientAPI capi)
    {
        Dictionary<string, List<ItemStack>> groups = [];
        foreach (EntityProperties entityType in entityTypes)
        {
            ItemStack? stack = entityType.GetCreatureStack(capi);
            if (stack == null) continue;

            string? groupcode = entityType.Attributes?["handbook"]?["groupcode"].AsString();
            if (string.IsNullOrEmpty(groupcode))
            {
                groupcode = entityType.Code;
            }

            if (!groups.ContainsKey(groupcode))
            {
                groups[groupcode] = [];
            }

            groups[groupcode].Add(stack);

        }
        return [.. groups.Values];
    }
}