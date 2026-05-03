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

        float workableTemp = (stack.Collectible.Attributes?[Text.WorkableTemperatureAttr].Exists) switch
        {
            true => stack.Collectible.Attributes[Text.WorkableTemperatureAttr].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Text.AlwaysWorkable)),
            _ => dsc.AppendLine(ColorText(Text.WorkableTemperature(workableTemp)))
        };
    }

    public static void GetWorkableTempInfoForItem(this StringBuilder dsc, ItemSlot inSlot, IWorldAccessor world)
    {
        if (Core.Config?.ShowHandbookWorkableTemp != true) return;
        if (inSlot.Itemstack?.Collectible is not IAnvilWorkable) return;

        float temperature = inSlot.Itemstack.Collectible.GetTemperature(world, inSlot.Itemstack);
        float meltingpoint = inSlot.Itemstack.Collectible.GetMeltingPoint(world, null, inSlot);

        float workableTemp = (inSlot.Itemstack.ItemAttributes?[Text.WorkableTemperatureAttr].Exists) switch
        {
            true => inSlot.Itemstack.ItemAttributes[Text.WorkableTemperatureAttr].AsFloat(meltingpoint / 2),
            _ => meltingpoint / 2,
        };

        _ = workableTemp switch
        {
            0 => dsc.AppendLine(ColorText(Text.AlwaysWorkable)),
            _ => dsc.AppendLine(ColorText(Text.WorkableTemperature(workableTemp)))
        };

        if (inSlot is DummySlot || inSlot is ItemSlotCreative) return;

        if (temperature < workableTemp)
        {
            dsc.AppendLine(ColorText(Text.TooColdToWork));
        }
    }

    public static void GetStackSizeUnitsForOre(this StringBuilder dsc, ItemSlot inSlot, IWorldAccessor world)
    {
        if (Core.Config?.ShowStackMetalUnits != true) return;
        if (inSlot.Itemstack?.Collectible is not ItemOre || inSlot.StackSize <= 1) return;

        if (inSlot.Itemstack.Collectible.CombustibleProps?.SmeltedStack?.ResolvedItemstack == null && inSlot.Itemstack.ItemAttributes?["metalUnits"].Exists == true)
        {
            float units2 = inSlot.Itemstack.ItemAttributes["metalUnits"].AsInt() * inSlot.StackSize;
            string orename = inSlot.Itemstack.Collectible.LastCodePart(1);
            if (orename.Contains('_'))
            {
                orename = orename.Split('_')[1];
            }
            AssetLocation loc = new("nugget-" + orename);
            Item? item = world.GetItem(loc);
            if (item?.CombustibleProps?.SmeltedStack?.ResolvedItemstack != null)
            {
                string metalname2 = item.CombustibleProps.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");
                dsc.AppendLine(ColorText(Lang.Get("{0} units of {1}", units2.ToString("0.#"), metalname2)));
            }
        }
    }

    public static void GetStackSizeUnitsForNugget(this StringBuilder dsc, ItemSlot inSlot)
    {
        if (Core.Config?.ShowStackMetalUnits != true) return;
        if (inSlot.Itemstack?.Collectible is not ItemNugget) return;

        CombustibleProperties combProps = inSlot.Itemstack.Collectible.CombustibleProps;

        if (inSlot.StackSize <= 1 || combProps.SmeltedStack == null || combProps.SmeltedStack.ResolvedItemstack == null) return;

        string smelttype = combProps.SmeltingType.ToString().ToLowerInvariant();
        int instacksize = combProps.SmeltedRatio;
        float units = combProps.SmeltedStack.ResolvedItemstack.StackSize * 100f / instacksize * inSlot.StackSize;
        string metalname = combProps.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");
        dsc.AppendLine(ColorText(Lang.Get("game:smeltdesc-" + smelttype + "ore-plural", units.ToString("0.#"), metalname)));
    }

    public static void GetGroundStorageInfo(this StringBuilder dsc, BlockEntityGroundStorage blockEntity)
    {
        if (Core.Config?.ShowPileTotalItems != true) return;
        if (blockEntity == null) return;
        if (blockEntity.StorageProps?.Layout != EnumGroundStorageLayout.Stacking) return;
        if (blockEntity.Inventory?.Count == 0) return;

        ICoreAPI api = blockEntity.Api;
        BlockPos centerPos = blockEntity.Pos;

        int totalAmount = blockEntity.GetTotalAmount();
        int totalAmountSame = totalAmount;

        for (int y = centerPos.Y - 1; ; y--)
        {
            BlockPos pos = new(centerPos.X, y, centerPos.Z, centerPos.dimension);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage? blockEntityGroundStorage))
            {
                if (blockEntityGroundStorage == null) break;

                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (blockEntity.HasSameContent(blockEntityGroundStorage))
                {
                    totalAmountSame += blockEntityGroundStorage.GetTotalAmount();
                }
            }
            else
            {
                break;
            }
        }

        for (int y = centerPos.Y + 1; ; y++)
        {
            BlockPos pos = new(centerPos.X, y, centerPos.Z, centerPos.dimension);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage? blockEntityGroundStorage))
            {
                if (blockEntityGroundStorage == null) break;

                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (blockEntity.HasSameContent(blockEntityGroundStorage))
                {
                    totalAmountSame += blockEntityGroundStorage.GetTotalAmount();
                }
            }
            else
            {
                break;
            }
        }

        dsc.AppendLine();
        dsc.Append(ColorText(Text.Everything));
        dsc.Append(": ");
        dsc.Append(totalAmount).AppendLine();

        dsc.Append(ColorText(Text.Current));
        dsc.Append(": ");
        dsc.Append(totalAmountSame).AppendLine();
    }

    public static void GetBombInfo(this StringBuilder sb, Block block, BlockEntityBomb blockEntity)
    {
        if (Core.Config?.ShowBombStats != true) return;

        if (blockEntity != null)
        {
            sb.AppendLine(ColorText(Text.BlastRadius(blockEntity.BlastRadius)));
            sb.AppendLine(ColorText(Text.InjureRadius(blockEntity.InjureRadius)));
            sb.AppendLine(ColorText(Text.BlastType(blockEntity.BlastType)));
            sb.AppendLine(ColorText(Text.FuseTimeSeconds(blockEntity.FuseTimeSeconds)));
        }
        else if (block != null)
        {
            if (block.Attributes == null) return;
            sb.AppendLine(ColorText(Text.BlastRadius(block.Attributes[Text.BlastRadiusAttr].AsInt())));
            sb.AppendLine(ColorText(Text.InjureRadius(block.Attributes[Text.InjureRadiusAttr].AsInt())));
            sb.AppendLine(ColorText(Text.BlastType(block.Attributes[Text.BlastTypeAttr].AsObject<EnumBlastType>())));
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
        dsc.AppendLine(ColorText(Text.TeleportsTo(targetpos)));
    }

    public static void GetMechanicalBlockInfo(this StringBuilder sb, BlockEntity blockEntity)
    {
        if (Core.Config?.ShowMechanicalBlockInfo != true) return;

        MechanicalNetwork? network = blockEntity?.GetBehavior<BEBehaviorMPBase>()?.Network;
        if (network == null) return;

        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.Speed", network.Speed)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.TotalAvailableTorque", network.TotalAvailableTorque)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.NetworkTorque", network.NetworkTorque)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.NetworkResistance", network.NetworkResistance)));
    }
}
