using Vintagestory.GameContent.Mechanics;

namespace ExtraInfo;

public static class InfoExtensions
{
    // Debug logging helper - kept for future debugging needs.
    // Usage: DebugLog(world, "message");
    private static void DebugLog(IWorldAccessor world, string message)
    {
        world?.Logger?.Notification("[ExtraInfo] " + message);
    }

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

            if (neighborBE == null)
            {
                continue;
            }

            double burnStart = neighborBE.GetField<double>("burnStartTotalHours");
            double hours = 12.0 - (world.Calendar.TotalHours - burnStart);

            sb.AppendLine()
                .Append(ColorText(Text.Coke))
                .Append(": ")
                .Append(ColorText(Text.HoursAndMinutes(hours)));

            return sb.ToString().TrimEnd();
        }

        return __result;
    }

    // Beehive kiln takes approximately 10.9 hours total to fire items.
    // See: https://wiki.vintagestory.at/Beehive_kiln/draft
    private const double BeehiveKilnFiringHours = 10.9;

    public static string GetBeehiveKilnInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowBeehiveKilnProgress)
        {
            return __result;
        }

        Block block = world.BlockAccessor.GetBlock(pos);
        if (block?.Code?.Path == null || !block.Code.Path.Contains("doorkiln")) return __result;

        StringBuilder sb = new(__result);

        // Search nearby for the kiln entity (within 2 blocks in all directions)
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    var neighborPos = pos.AddCopy(dx, dy, dz);
                    var be = world.BlockAccessor.GetBlockEntity(neighborPos);
                    if (be?.GetType().Name != "BlockEntityBeeHiveKiln") continue;

                    // Calculate direction from door to kiln interior (only use X/Z, ignore Y)
                    int dirX = Math.Sign(neighborPos.X - pos.X);
                    int dirZ = Math.Sign(neighborPos.Z - pos.Z);

                    // If kiln entity is at same position as door, detect direction by looking for ground storage
                    if (dirX == 0 && dirZ == 0)
                    {
                        // Check each cardinal direction for ground storage
                        BlockPos[] cardinals = new[] { pos.NorthCopy(), pos.EastCopy(), pos.SouthCopy(), pos.WestCopy() };
                        foreach (var checkPos in cardinals)
                        {
                            if (world.IsGroundStorage(checkPos, out _))
                            {
                                dirX = Math.Sign(checkPos.X - pos.X);
                                dirZ = Math.Sign(checkPos.Z - pos.Z);
                                break;
                            }
                        }

                        // If no ground storage, try to find grating blocks (at Y-1)
                        if (dirX == 0 && dirZ == 0)
                        {
                            foreach (var checkPos in cardinals)
                            {
                                var grateCheckPos = checkPos.DownCopy(1);
                                var grateBlock = world.BlockAccessor.GetBlock(grateCheckPos);
                                if (grateBlock?.Code?.Path?.Contains("grating") == true)
                                {
                                    dirX = Math.Sign(checkPos.X - pos.X);
                                    dirZ = Math.Sign(checkPos.Z - pos.Z);
                                    break;
                                }
                            }
                        }

                        // Still couldn't determine direction
                        if (dirX == 0 && dirZ == 0) continue;
                    }

                    // Perpendicular direction for the 3-wide grid
                    int perpX = dirZ;
                    int perpZ = -dirX;

                    // Build 3x3 grid positions (depth 1-3, width -1 to +1)
                    List<BlockPos> gridPositions = new();
                    for (int depth = 1; depth <= 3; depth++)
                    {
                        for (int side = -1; side <= 1; side++)
                        {
                            int gx = dirX * depth + perpX * side;
                            int gz = dirZ * depth + perpZ * side;
                            gridPositions.Add(pos.AddCopy(gx, 0, gz));
                        }
                    }

                    // Analyze kiln contents - includes max heat received by any unfired item
                    var (unfired, fired, maxHoursHeatReceived) = AnalyzeKilnContents(world, gridPositions);

                    // Get fuel time remaining
                    double fuelTimeRemaining = GetMinFuelTimeRemaining(world, gridPositions);

                    // Get kiln status and calculate time remaining based on item heat, not kiln total
                    bool receivesHeat = be.GetField<bool>("receivesHeat");
                    double hoursRemaining = BeehiveKilnFiringHours - maxHoursHeatReceived;

                    // Display kiln timer
                    sb.AppendLine();
                    if (receivesHeat && unfired > 0)
                    {
                        // Kiln is actively firing with unfired items
                        if (hoursRemaining > 0)
                        {
                            sb.Append(ColorText(Text.Kiln));
                            sb.Append(": ");
                            sb.Append(ColorText(Text.HoursAndMinutes(hoursRemaining)));
                        }
                        else
                        {
                            // Timer exceeded but still receiving heat - items should be done soon
                            sb.Append(ColorText(Text.Kiln));
                            sb.Append(": ");
                            sb.Append(ColorText(Lang.Get("Firing")));
                        }
                    }
                    else if (receivesHeat && unfired == 0 && fired > 0)
                    {
                        sb.Append(ColorText(Text.KilnFiringComplete));
                    }
                    else if (receivesHeat && unfired == 0 && fired == 0)
                    {
                        sb.Append(ColorText(Lang.Get("Nothing in kiln to fire")));
                    }
                    else if (!receivesHeat && unfired > 0)
                    {
                        sb.Append(ColorText(Lang.Get("Items still unfired - add fuel")));
                    }
                    else if (!receivesHeat && unfired == 0 && fired > 0)
                    {
                        sb.Append(ColorText(Text.KilnFiringComplete));
                    }
                    else
                    {
                        sb.Append(ColorText(Text.Kiln));
                        sb.Append(": ");
                        sb.Append(ColorText(Text.NotBurning));
                    }

                    // Display item counts if any items present
                    if (unfired > 0 || fired > 0)
                    {
                        sb.AppendLine();
                        if (unfired > 0)
                        {
                            sb.Append(ColorText(Text.Unfired));
                            sb.Append(": ");
                            sb.Append(unfired);
                        }
                        if (unfired > 0 && fired > 0)
                        {
                            sb.Append(" | ");
                        }
                        if (fired > 0)
                        {
                            sb.Append(ColorText(Text.Fired));
                            sb.Append(": ");
                            sb.Append(fired);
                        }
                    }

                    // Display fuel status
                    sb.AppendLine();
                    sb.Append(ColorText(Text.Fuel));
                    sb.Append(": ");
                    if (fuelTimeRemaining >= 0)
                    {
                        sb.Append(ColorText(Text.HoursAndMinutes(fuelTimeRemaining)));
                    }
                    else
                    {
                        sb.Append(ColorText(Text.NotBurning));
                    }

                    return sb.ToString().TrimEnd();
                }
            }
        }

        return __result;
    }

    private static bool IsUnfiredItem(ItemStack stack)
    {
        if (stack == null) return false;
        string code = stack.Collectible.Code.Path.ToLowerInvariant();
        return code.Contains("-raw") || code.Contains("rawclay") ||
               code.Contains("clayform") || code.Contains("unfired");
    }

    private static (int unfired, int fired, double maxHoursHeatReceived) AnalyzeKilnContents(IWorldAccessor world, List<BlockPos> positions)
    {
        int unfired = 0;
        int fired = 0;
        double maxHoursHeatReceived = 0;

        foreach (var gridPos in positions)
        {
            if (world.IsGroundStorage(gridPos, out var gs))
            {
                var stack = gs.GetContainedStack();
                if (stack != null)
                {
                    int amount = gs.GetTotalAmount();
                    if (IsUnfiredItem(stack))
                    {
                        unfired += amount;

                        // Track max hoursHeatReceived for unfired items
                        double itemHeatReceived = stack.Attributes?.TryGetFloat("hoursHeatReceived") ?? 0;
                        if (itemHeatReceived > maxHoursHeatReceived)
                        {
                            maxHoursHeatReceived = itemHeatReceived;
                        }
                    }
                    else
                    {
                        fired += amount;
                    }
                }
            }
        }

        return (unfired, fired, maxHoursHeatReceived);
    }

    private static double GetMinFuelTimeRemaining(IWorldAccessor world, List<BlockPos> itemPositions)
    {
        double minTime = double.MaxValue;
        bool anyBurning = false;

        foreach (var gridPos in itemPositions)
        {
            // Fuel is 2 blocks below grating/item position
            var fuelPos = gridPos.DownCopy(2);

            if (world.BlockAccessor.GetBlockEntity(fuelPos) is BlockEntityCoalPile pile && pile.IsBurning)
            {
                anyBurning = true;

                float burnHoursPerLayer = pile.BurnHoursPerLayer;
                var inv = pile.GetField<InventoryGeneric>("inventory");
                int stackSize = inv?[0]?.StackSize ?? 0;
                double totalHours = stackSize / 2.0 * burnHoursPerLayer;

                double burnStart = pile.GetField<double>("burnStartTotalHours");
                double elapsed = world.Calendar.TotalHours - burnStart;
                double remaining = totalHours - elapsed;

                if (remaining < minTime)
                {
                    minTime = remaining;
                }
            }
        }

        return anyBurning ? Math.Max(0, minTime) : -1;
    }

    public static string GetSteelInfo(this string __result, IWorldAccessor world, BlockPos pos)
    {
        if (Core.Config == null || !Core.Config.ShowCementationFurnaceProgress)
        {
            return __result;
        }

        Block block = world.BlockAccessor.GetBlock(pos);
        string codePath = block.Code.Path;

        // Check if this is an iron door (regular or metal variant)
        bool isIronDoor = codePath.Contains("irondoor") ||
                          (codePath.Contains("door") && codePath.Contains("iron"));
        if (!isIronDoor) return __result;

        StringBuilder sb = new(__result);

        // From door, coffin is 2 blocks away in a cardinal direction at same Y level
        BlockPos[] neighborPositions = new BlockPos[]
        {
            pos.NorthCopy(2),
            pos.EastCopy(2),
            pos.SouthCopy(2),
            pos.WestCopy(2)
        };

        foreach (BlockPos coffinPos in neighborPositions)
        {
            var coffinBlock = world.BlockAccessor.GetBlock(coffinPos);

            // Look for stonecoffin or stonecoffinsection
            if (!coffinBlock.Code.Path.Contains("stonecoffin")) continue;

            // Check fuel status first (2 blocks below the coffin)
            BlockPos fuelPos = coffinPos.DownCopy(2);
            var fuelPile = world.BlockAccessor.GetBlockEntity(fuelPos) as BlockEntityCoalPile;
            bool isBurning = fuelPile?.IsBurning ?? false;

            // Get the coffin entity (stonecoffinsection has BlockEntityStoneCoffin)
            if (world.BlockAccessor.GetBlockEntity(coffinPos) is BlockEntityStoneCoffin be)
            {
                bool processComplete = be.GetField<bool>("processComplete");
                if (processComplete)
                {
                    sb.AppendLine(Lang.Get("Carburization process complete. Break to retrieve blister steel."));
                }
                else
                {
                    double progress = be.GetField<double>("progress");
                    if (progress > 0.0)
                    {
                        int percent = (int)(progress * 100.0);
                        sb.AppendLine(Text.CarburizationComplete(percent));
                    }
                    else if (isBurning)
                    {
                        sb.AppendLine(Lang.Get("extrainfo:Carburization.Starting"));
                    }
                }
            }

            // Display fuel status
            if (fuelPile != null)
            {
                if (isBurning)
                {
                    // Calculate actual burn time based on fuel type and stack size
                    float burnHoursPerLayer = fuelPile.BurnHoursPerLayer;
                    var inventory = fuelPile.GetField<InventoryGeneric>("inventory");
                    int stackSize = inventory?[0]?.StackSize ?? 0;
                    double totalBurnHours = stackSize / 2.0 * burnHoursPerLayer;

                    double burnStart = fuelPile.GetField<double>("burnStartTotalHours");
                    double hoursElapsed = world.Calendar.TotalHours - burnStart;
                    double hours = totalBurnHours - hoursElapsed;

                    sb.AppendLine(ColorText(Text.Fuel + ": " + Text.HoursAndMinutes(hours)));
                }
                else
                {
                    sb.AppendLine(ColorText(Text.Fuel + ": " + Lang.Get("Not burning")));
                }
            }

            break; // Found the coffin, no need to check other directions
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
