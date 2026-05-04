using Vintagestory.GameContent.Mechanics;

namespace ExtraInfo;

public static class InfoExtensions
{
    public static void GetWorkableTempInfoForAnvil(this StringBuilder dsc, BlockEntityAnvil blockEntity)
    {
        if (Core.Config?.ShowAnvilWorkableTemp != true) return;
        if (blockEntity == null) return;
        if (blockEntity.WorkItemStack == null || blockEntity.SelectedRecipe == null) return;

        ItemStack stack = blockEntity.WorkItemStack;

        float meltingpoint = stack.Collectible.GetMeltingPoint(blockEntity.Api.World, null, new DummySlot(stack));

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

    public static void GetWorkableTempInfoForItem(this StringBuilder dsc, ItemSlot inSlot, IWorldAccessor world)
    {
        if (Core.Config?.ShowHandbookWorkableTemp != true) return;
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

    public static void GetTranslocatorInfo(this StringBuilder dsc, BlockEntityStaticTranslocator blockEntity)
    {
        if (Core.Config?.ShowTranslocatorDestination != true) return;
        if (blockEntity?.tpLocation == null) return;

        BlockPos pos = blockEntity.Api.World.DefaultSpawnPosition.AsBlockPos;
        BlockPos targetpos = blockEntity.tpLocation.Copy().Sub(pos.X, 0, pos.Z);
        if (blockEntity.tpLocationIsOffset)
        {
            targetpos.Add(blockEntity.Pos.X, pos.Y, pos.Z);
        }
        dsc.AppendLine(Lang.Get("Teleports to {0}", targetpos));
    }

    public static void GetMechanicalBlockInfo(this StringBuilder sb, BlockEntity blockEntity)
    {
        if (Core.Config?.ShowMechanicalBlockInfo != true) return;

        MechanicalNetwork? network = blockEntity?.GetBehavior<BEBehaviorMPBase>()?.Network;
        if (network == null) return;

        sb.AppendLine(Lang.Get("extrainfo:Mechanics.Speed", network.Speed));
        sb.AppendLine(Lang.Get("extrainfo:Mechanics.TotalAvailableTorque", network.TotalAvailableTorque));
        sb.AppendLine(Lang.Get("extrainfo:Mechanics.NetworkTorque", network.NetworkTorque));
        sb.AppendLine(Lang.Get("extrainfo:Mechanics.NetworkResistance", network.NetworkResistance));
    }
}
