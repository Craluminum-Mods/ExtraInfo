using Vintagestory.GameContent.Mechanics;

namespace ExtraInfo;

public static class InfoExtensions
{
    public static void GetWorkableTempInfoForAnvil(this StringBuilder dsc, BlockEntityAnvil blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowAnvilWorkableTemp)
        {
            return;
        }

        if (blockEntity == null) return;

        if (blockEntity.WorkItemStack == null || blockEntity.SelectedRecipe == null)
        {
            return;
        }

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
        if (Core.Config == null || !Core.Config.ShowHandbookWorkableTemp)
        {
            return;
        }

        if (inSlot.Itemstack.Collectible is not IAnvilWorkable)
        {
            return;
        }

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
        if (Core.Config == null || !Core.Config.ShowStackMetalUnits)
        {
            return;
        }

        if (inSlot.Itemstack.Collectible is not ItemOre || inSlot.StackSize <= 1)
        {
            return;
        }

        if (inSlot.Itemstack.Collectible.CombustibleProps?.SmeltedStack?.ResolvedItemstack == null && inSlot.Itemstack.ItemAttributes?["metalUnits"].Exists == true)
        {
            float units2 = inSlot.Itemstack.ItemAttributes["metalUnits"].AsInt() * inSlot.StackSize;
            string orename = inSlot.Itemstack.Collectible.LastCodePart(1);
            if (orename.Contains('_'))
            {
                orename = orename.Split('_')[1];
            }
            AssetLocation loc = new("nugget-" + orename);
            Item item = world.GetItem(loc);
            if (item.CombustibleProps?.SmeltedStack?.ResolvedItemstack != null)
            {
                string metalname2 = item.CombustibleProps.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");
                dsc.AppendLine(ColorText(Lang.Get("{0} units of {1}", units2.ToString("0.#"), metalname2)));
            }
        }
    }

    public static void GetStackSizeUnitsForNugget(this StringBuilder dsc, ItemSlot inSlot)
    {
        if (Core.Config == null || !Core.Config.ShowStackMetalUnits)
        {
            return;
        }

        if (inSlot.Itemstack.Collectible is not ItemNugget)
        {
            return;
        }

        CombustibleProperties combProps = inSlot.Itemstack.Collectible.CombustibleProps;

        if (inSlot.StackSize <= 1 || combProps?.SmeltedStack == null)
        {
            return;
        }

        string smelttype = combProps.SmeltingType.ToString().ToLowerInvariant();
        int instacksize = combProps.SmeltedRatio;
        float units = combProps.SmeltedStack.ResolvedItemstack.StackSize * 100f / instacksize * inSlot.StackSize;
        string metalname = combProps.SmeltedStack.ResolvedItemstack.GetName().Replace(" ingot", "");
        dsc.AppendLine(ColorText(Lang.Get("game:smeltdesc-" + smelttype + "ore-plural", units.ToString("0.#"), metalname)));
    }

    public static string GetBlockBreakingTimeInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowBlockBreakingTime)
        {
            return __result;
        }

        StringBuilder sb = new(__result);

        Dictionary<BlockPos, BlockDamage> damagedBlocks = (world as ClientMain).GetField<Dictionary<BlockPos, BlockDamage>>("damagedBlocks");
        if (damagedBlocks?.Count == 0) return __result;

        BlockDamage currentBlockDamage = damagedBlocks.FirstOrDefault(x => x.Key == pos).Value;
        if (currentBlockDamage == null) return __result;

        Block block = world.BlockAccessor.GetBlock(pos);
        float totalValue = block.GetResistance(world.BlockAccessor, pos);
        float remainingValue = currentBlockDamage.RemainingResistance;

        float remainingPercentage = remainingValue / totalValue * 100;

        sb.AppendLine().Append(ColorText(Text.RemainingResistance(remainingPercentage.ToString("F0"))));

        return sb.ToString().TrimEnd();
    }

    public static void GetGroundStorageInfo(this StringBuilder dsc, BlockEntityGroundStorage blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowPileTotalItems)
        {
            return;
        }

        if (blockEntity == null || blockEntity?.StorageProps?.Layout != EnumGroundStorageLayout.Stacking || blockEntity?.Inventory?.Count == 0)
        {
            return;
        }

        ICoreAPI api = blockEntity.Api;
        BlockPos centerPos = blockEntity.Pos;

        int totalAmount = blockEntity.GetTotalAmount();
        int totalAmountSame = totalAmount;

        for (int y = centerPos.Y - 1; ; y--)
        {
            BlockPos pos = new(centerPos.X, y, centerPos.Z, centerPos.dimension);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage blockEntityGroundStorage))
            {
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
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage blockEntityGroundStorage))
            {
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

    public static string GetCrockSealedInName(this string __result, ItemSlot inSlot)
    {
        if (Core.Config == null || !Core.Config.ShowSealedCrockName)
        {
            return __result;
        }

        StringBuilder oldSb = new(__result);

        if (inSlot?.Itemstack?.Attributes?.GetBool(Text.SealedAttr) == true)
        {
            StringBuilder newSb = new();

            newSb.Append(Text.SealedText);
            newSb.Append(' ');

            oldSb.Insert(0, newSb);
        }

        return oldSb.ToString();
    }

    public static void GetFarmlandDropSoilChanceInfo(this StringBuilder dsc, BlockEntityFarmland blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowFarmlandDropsSoil)
        {
            return;
        }

        if (blockEntity == null) return;

        bool isModEnabled = blockEntity.Api.ModLoader.IsModEnabled(Modid.FarmlandDropsSoil);
        if (!isModEnabled)
        {
            return;
        }

        Mod mod = blockEntity.Api.ModLoader.GetMod(Modid.FarmlandDropsSoil);

        float nutrients = blockEntity.Nutrients.Zip(blockEntity.OriginalFertility, (current, original) => current / original).Min();

        dsc.AppendLine(ColorText(string.Format(Text.FormatPercent, mod.Info.Name, (int)(nutrients * 100))));
    }

    public static void GetFarmlandInfo(this StringBuilder dsc, BlockEntityFarmland blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowFarmlandProgress)
        {
            return;
        }

        if (blockEntity == null) return;

        double hours = blockEntity.TotalHoursForNextStage - blockEntity.Api.World.Calendar.TotalHours;

        Block block = GetCrop();

        if (block != null && (GetCropStage(block) < block.CropProps.GrowthStages))
        {
            dsc.AppendLine(ColorText(Text.HoursAndMinutes(hours)));
        }

        Block GetCrop()
        {
            Block block = blockEntity.Api.World.BlockAccessor.GetBlock(blockEntity.UpPos);
            if (block == null || block.CropProps == null)
            {
                return null;
            }
            return block;
        }

        static int GetCropStage(Block block)
        {
            _ = int.TryParse(block.LastCodePart(), out int stage);
            return stage;
        }
    }

    public static void GetBloomeryInfo(this StringBuilder dsc, BlockEntityBloomery blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowBloomeryProgress)
        {
            return;
        }

        if (blockEntity == null) return;
        ICoreAPI api = blockEntity.Api;

        if (blockEntity.GetField<bool>("burning"))
        {
            double hours = (blockEntity.GetField<double>("burningUntilTotalDays") - api.World.Calendar.TotalDays) * api.World.Calendar.HoursPerDay;
            dsc.AppendLine(ColorText(Text.HoursAndMinutes(hours)));
        }
    }

    public static void GetQuernInfo(this StringBuilder dsc, BlockEntityOpenableContainer blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowQuernGrindingProgress)
        {
            return;
        }

        if (blockEntity == null) return;
        if (blockEntity is not BlockEntityQuern quern) return;

        if (quern.CanGrind() && quern.GrindSpeed > 0)
        {
            double percent = quern.inputGrindTime / quern.maxGrindingTime();
            int mss = quern.InputSlot?.Itemstack?.StackSize + (quern.OutputSlot?.Itemstack?.StackSize ?? 0) ?? 1;
            double stackSize = (double)(quern.InputSlot?.Itemstack?.StackSize ?? 0) / mss;
            stackSize = 1.0 - stackSize;
            stackSize += percent / mss;
            stackSize *= 100;
            percent *= 100;

            dsc.Append(ColorText(Text.Everything));
            dsc.Append(' ');
            dsc.AppendFormat("{0:#}%", Math.Round(stackSize, 2)).AppendLine();

            dsc.Append(ColorText(Text.One));
            dsc.Append(' ');
            dsc.AppendFormat("{0:#}%", Math.Round(percent, 2)).AppendLine();
        }
    }

    public static string GetCokeInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowCokeOvenProgress)
        {
            return __result;
        }

        if (world.BlockAccessor.GetBlock(pos) is not BlockCokeOvenDoor) return __result;

        StringBuilder sb = new(__result);

        BlockPos[] positions = new[] { pos.NorthCopy(), pos.EastCopy(), pos.SouthCopy(), pos.WestCopy() };
        foreach (BlockPos neighborPos in positions)
        {
            BlockEntityCoalPile neighborBE = world.BlockAccessor.GetBlockEntity(neighborPos) as BlockEntityCoalPile;

            if (neighborBE?.IsBurning == false)
            {
                return __result;
            }

            double? hours = neighborBE?.GetHoursLeft(neighborBE.GetField<double>("burnStartTotalHours"));

            if (!hours.HasValue)
            {
                continue;
            }

            sb.AppendLine()
                .Append(ColorText(Text.Coke))
                .Append(": ")
                .Append(ColorText(Text.Hours(hours.Value)));

            return sb.ToString().TrimEnd();
        }

        return __result;
    }

    public static string GetSteelInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowCementationFurnaceProgress)
        {
            return __result;
        }

        if (world.BlockAccessor.GetBlock(pos) is not BlockDoor blockDoor) return __result;
        if (blockDoor.FirstCodePart() != "irondoor") return __result;

        StringBuilder sb = new(__result);

        BlockPos _pos = blockDoor.IsUpperHalf() switch
        {
            true => pos.DownCopy(),
            false => pos,
        };

        BlockPos[] neighborPositions = new BlockPos[]
        {
            _pos.NorthCopy(3),
            _pos.EastCopy(3),
            _pos.SouthCopy(3),
            _pos.WestCopy(3)
        };

        foreach (BlockPos blockPos in neighborPositions)
        {
            if (world.BlockAccessor.GetBlockEntity(blockPos) is not BlockEntityStoneCoffin be) continue;

            bool processComplete = be.GetField<bool>("processComplete");
            if (processComplete)
            {
                sb.AppendLine(Lang.Get("Carburization process complete. Break to retrieve blister steel."));
                continue;
            }

            double progress = be.GetField<double>("progress");
            if (progress <= 0.0) continue;

            int percent = (int)(progress * 100.0);
            sb.AppendLine(Text.CarburizationComplete(percent));
        }

        return sb.ToString().TrimEnd();
    }

    public static string GetCharcoalPitInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowCharcoalPitProgress)
        {
            return __result;
        }

        if (world.BlockAccessor.GetBlockEntity(pos.DownCopy()) is not BlockEntityCharcoalPit blockEntity) return __result;

        StringBuilder sb = new(__result);

        switch (blockEntity.GetField<int>("state"))
        {
            case > 0:
                {
                    double hours = blockEntity.GetField<double>("finishedAfterTotalHours") - world.Calendar.TotalHours;
                    sb.Append(ColorText(Text.CharcoalPit));
                    sb.Append(": ");
                    sb.AppendLine(ColorText(Text.HoursAndMinutes(hours)));
                    return sb.ToString().TrimEnd();
                }

            default:
                {
                    double hours = blockEntity.GetField<double>("startingAfterTotalHours") - world.Calendar.TotalHours;
                    sb.Append(ColorText(Text.CharcoalPit));
                    sb.Append(": ");
                    sb.Append(ColorText(Text.WarmingUp));
                    sb.Append(' ');
                    sb.AppendLine(ColorText(Text.MinutesAndSeconds(hours)));
                    return sb.ToString().TrimEnd();
                }
        }
    }

    public static void GetPitKilnInfo(this StringBuilder dsc, BlockEntityPitKiln blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowPitKilnProgress)
        {
            return;
        }

        if (blockEntity == null) return;
        ICoreAPI api = blockEntity.Api;

        if (blockEntity.Lit)
        {
            double hours = blockEntity.BurningUntilTotalHours - api.World.Calendar.TotalHours;
            dsc.AppendLine(ColorText(Text.HoursAndMinutes(hours)));
        }
    }

    public static void GetBombInfo(this StringBuilder sb, Block block, BlockEntityBomb blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowBombStats)
        {
            return;
        }

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

    public static void GetTransientInfo(this StringBuilder dsc, BlockEntityTransient blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowBlockTransitionInfo)
        {
            return;
        }

        if (blockEntity == null) return;
        TransientProperties props = blockEntity.GetField<TransientProperties>("props");
        if (props == null) return;

        blockEntity.CheckTransition(0);

        double hoursLeft = blockEntity.GetField<double>("transitionHoursLeft");
        dsc.AppendLine(ColorText(Text.Hours(hoursLeft)));
    }

    public static void GetSkepInfo(this StringBuilder dsc, BlockEntityBeehive blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowSkepProgress)
        {
            return;
        }

        if (blockEntity == null) return;
        if (blockEntity.Block is not BlockSkep) return;
        double hours = blockEntity.GetField<double>("harvestableAtTotalHours") - blockEntity.Api.World.Calendar.TotalHours;
        dsc.AppendLine(ColorText(Text.HoursAndMinutes(hours)));
    }

    public static void GetTranslocatorInfo(this StringBuilder dsc, BlockEntityStaticTranslocator blockEntity)
    {
        if (Core.Config == null || !Core.Config.ShowTranslocatorDestination)
        {
            return;
        }

        if (blockEntity == null) return;
        if (blockEntity.tpLocation == null) return;

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
        if (Core.Config == null || !Core.Config.ShowMechanicalBlockInfo)
        {
            return;
        }

        MechanicalNetwork network = blockEntity?.GetBehavior<BEBehaviorMPBase>()?.Network;
        if (network == null) return;

        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.Speed", network.Speed)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.TotalAvailableTorque", network.TotalAvailableTorque)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.NetworkTorque", network.NetworkTorque)));
        sb.AppendLine(ColorText(Lang.Get("extrainfo:Mechanics.NetworkResistance", network.NetworkResistance)));
    }
}