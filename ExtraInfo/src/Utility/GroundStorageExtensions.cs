namespace ExtraInfo;

public static class GroundStorageExtensions
{
    public static bool IsGroundStorage(this IWorldAccessor world, BlockPos pos, out BlockEntityGroundStorage beGroundStorage)
    {
        return (beGroundStorage = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityGroundStorage) != null;
    }

    public static int GetTotalAmount(this BlockEntityGroundStorage beGroundStorage)
    {
        return beGroundStorage?.Inventory?[0]?.StackSize ?? 0;
    }

    public static ItemStack GetContainedStack(this BlockEntityGroundStorage beGroundStorage)
    {
        return beGroundStorage?.Inventory?[0]?.Itemstack;
    }

    public static bool HasSameContent<T>(this T beGroundStorage, T beGroundStorageOther) where T : BlockEntityGroundStorage
    {
        ItemStack thisStack = GetContainedStack(beGroundStorage);
        ItemStack otherStack = GetContainedStack(beGroundStorageOther);

        if (thisStack == null || otherStack == null) return false;

        CollectibleObject obj = thisStack.Collectible;
        return obj.Equals(thisStack, otherStack, GlobalConstants.IgnoredStackAttributes);
    }
}