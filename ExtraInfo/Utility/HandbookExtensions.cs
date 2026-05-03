namespace ExtraInfo;

public static class HandbookExtensions
{
    public static void AddPitKilnInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookPitKiln != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockPitkiln blockPitKiln) return;

        List<JsonItemStackBuildStage> fuelStacks = blockPitKiln.GetFuelStacks(capi);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.Fuel);

        List<RichTextComponentBase> richText = [];

        foreach (JsonItemStackBuildStage fuel in fuelStacks)
        {
            if (fuel.ResolvedItemstack == null || fuel.BurnTimeHours == null) continue;
            richText.AddStack(capi, openDetailPageFor, fuel.ResolvedItemstack);
            richText.Add(new RichTextComponent(capi, Text.Hours((float)fuel.BurnTimeHours) + "\n", CairoFont.WhiteSmallText())
            {
                VerticalAlign = EnumVerticalAlign.Middle
            });
        }

        list.AddRange(richText);
    }

    public static void AddPanningDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookPanningDrops != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockPan blockPan) return;

        Dictionary<ItemStack[], PanningDrop[]> panningDrops = GetPanningDrops(capi, blockPan);

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.PanningDrops);

        List<RichTextComponentBase> richText = [];

        foreach (KeyValuePair<ItemStack[], PanningDrop[]> keyVal in panningDrops)
        {
            List<ItemStack[]> stackLists = keyVal.Key.GroupBy(stack => stack.Collectible.FirstCodePart()).Select(group => group.ToArray()).ToList();
            foreach (ItemStack[] stacks in stackLists)
            {
                richText.AddStacks(capi, openDetailPageFor, stacks);
            }

            richText.AddEqualSign(capi);
            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));

            int count = 3;
            foreach (PanningDrop drop in panningDrops[keyVal.Key])
            {
                if (!drop.Resolve(capi.World, "") || drop.ResolvedItemstack == null) continue;
                richText.AddStack(capi, openDetailPageFor, drop.ResolvedItemstack);

                float extraMul = drop.DropModbyStat is null ? 1f : capi.World.Player.Entity.Stats.GetBlended(drop.DropModbyStat);

                count--;
                richText.Add(new RichTextComponent(capi, GetMinMaxPercent(drop, extraMul) + (count == 0 ? "\n" : "\t\t\t\t\t"), CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });

                if (count == 0) count = 3;
            }

            richText.Add(new ClearFloatTextComponent(capi, unScaleMarginTop: 7));
        }

        list.AddRange(richText);
    }

    public static void AddTroughInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookTroughFeedOptions != true) return;
        if (inSlot.Itemstack!.Collectible is not BlockTroughBase blockTrough) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Constants.Text.ValidAnimalFeed);

        foreach (ContentConfig config in blockTrough.contentConfigs)
        {
            if (config.Content.Code.ToShortString().Contains("-*"))
            {
                List<ItemStack> stacks = GetWildcardTroughStacks(capi, config);
                list.AddStacks(capi, openDetailPageFor, stacks.ToArray());
            }
            else
            {
                if (config.Content.ResolvedItemstack != null)
                {
                    list.AddStack(capi, openDetailPageFor, config.Content.ResolvedItemstack);
                }
            }
        }
    }

    public static void AddEntitiesThatEatCollectible(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookEatableByCreatures != true) return;

        List<EntityProperties> entityTypes = [];
        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            CreatureDiet? creatureDiet = entityType.Attributes?["creatureDiet"].AsObject<CreatureDiet>();
            if (creatureDiet?.Matches(inSlot.Itemstack) == true)
            {
                entityTypes.Add(entityType);
            }
        }

        if (entityTypes.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.EatenBy);

        List<List<ItemStack>> groupedStacks = entityTypes.GetGroupedCreatureStacks(capi);
        for (int i = 0; i < groupedStacks.Count; i++)
        {
            if (groupedStacks[i].Count == 1)
            {
                list.AddStack(capi, openDetailPageFor, groupedStacks[i].First());
            }
            else
            {
                list.AddStacks(capi, openDetailPageFor, groupedStacks[i].ToArray());
            }
        }
    }

    public static void AddEntityDietInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookCreatureDiet != true) return;
        if (inSlot.Itemstack!.Collectible is not ItemCreature itemCreature) return;

        EntityProperties? entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        CreatureDiet? creatureDiet = entityType?.Attributes?["creatureDiet"].AsObject<CreatureDiet>();
        if (creatureDiet == null) return;

        List<ItemStack> stacks = [];
        for (int i = 0; i < capi.World.Collectibles.Count; i++)
        {
            if (creatureDiet.Matches(capi.World.Collectibles[i]))
            {
                List<ItemStack> _stacks = capi.World.Collectibles[i].GetHandBookStacks(capi);
                if (_stacks != null)
                {
                    stacks.AddRange(_stacks);
                }
            }
        }

        if (stacks.Count == 0) return;

        list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.Food);

        List<List<ItemStack>> groupedStacks = stacks.GroupStacksByFirstCodePart();
        for (int i = 0; i < groupedStacks.Count; i++)
        {
            if (groupedStacks[i].Count == 1)
            {
                list.AddStack(capi, openDetailPageFor, groupedStacks[i].First());
            }
            else
            {
                list.AddStacks(capi, openDetailPageFor, groupedStacks[i].ToArray());
            }
        }
    }

    public static void AddEntityDropsInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookEntityDrops != true) return;
        if (inSlot.Itemstack!.Collectible is not ItemCreature itemCreature) return;

        EntityProperties? entityType = capi.World.GetEntityType(new AssetLocation(itemCreature.Code.Domain, itemCreature.CodeEndWithoutParts(1)));
        if (entityType == null) return;

        List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
        if (harvestStacks != null && harvestStacks.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.ObtainedByKillingAndHarvesting);

            List<RichTextComponentBase> richTextHarvest = [];

            foreach (BlockDropItemStack stack in harvestStacks)
            {
                if (stack.ResolvedItemstack == null) continue;
                richTextHarvest.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextHarvest.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextHarvest);
        }

        if (entityType.Drops != null && entityType.Drops.Length != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.ObtainedByKilling);

            List<RichTextComponentBase> richTextOther = [];

            foreach (BlockDropItemStack stack in entityType.Drops)
            {
                if (stack.ResolvedItemstack == null) continue;
                richTextOther.AddStack(capi, openDetailPageFor, stack.ResolvedItemstack);
                richTextOther.Add(new RichTextComponent(capi, GetMinMax(stack.Quantity) + "\n", CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                });
            }

            list.AddRange(richTextOther);
        }
    }

    public static void AddEntityDropsInfoForDrop(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookEntityDrops != true) return;

        CollectibleObject collObj = inSlot.Itemstack!.Collectible;

        List<RichTextComponentBase> richTextHarvest = [];
        List<RichTextComponentBase> richTextDrop = [];

        foreach (EntityProperties entityType in capi.World.EntityTypes)
        {
            List<BlockDropItemStack> harvestStacks = GetHarvestableDrops(capi, entityType);
            if (harvestStacks?.Count != 0 && harvestStacks?.Find(stack => stack?.Code == collObj?.Code) != null)
            {
                ItemStack? stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextHarvest.AddStack(capi, openDetailPageFor, stack);
            }

            if (entityType.Drops.Length != 0 && entityType.Drops.ToList().Find(stack => stack.Code == collObj.Code) != null)
            {
                ItemStack? stack = entityType.GetCreatureStack(capi);
                if (stack == null) continue;

                richTextDrop.AddStack(capi, openDetailPageFor, stack);
            }
        }

        if (richTextHarvest.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.ObtainedByKillingAndHarvesting);
            list.AddRange(richTextHarvest);
        }

        if (richTextDrop.Count != 0)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.ObtainedByKilling);
            list.AddRange(richTextDrop);
        }
    }

    public static void AddTraderInfo(this List<RichTextComponentBase> list, ItemSlot inSlot, ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor)
    {
        if (Core.Config?.ShowHandbookTraderGoods != true) return;

        CollectibleObject collObj = inSlot.Itemstack!.Collectible;
        if (collObj is ItemCreature itemCreature)
        {
            if (!TraderInfoSystem.unresolvedTradeProps.TryGetValue(itemCreature.Code, out TradeProperties? tradeProps) || tradeProps == null)
            {
                return;
            }

            ItemStack gear = new(capi.World.GetItem(new AssetLocation("gear-rusty")));
            List<TradeItem> buyingStacks = tradeProps.Buying.List.Where(tradeItem => tradeItem.Resolve(capi.World, "")).ToList();
            List<TradeItem> sellingStacks = tradeProps.Selling.List.Where(tradeItem2 => tradeItem2.Resolve(capi.World, "")).ToList();

            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.YouCanSell);
            List<RichTextComponentBase> richTextSell = [];
            foreach (TradeItem item in buyingStacks)
            {
                richTextSell.AddTraderInfo(capi, item, openDetailPageFor, gear);
            }
            list.AddRange(richTextSell);

            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.YouCanBuy);
            List<RichTextComponentBase> richTextBuy = [];
            foreach (TradeItem item in sellingStacks)
            {
                richTextBuy.AddTraderInfo(capi, item, openDetailPageFor, gear);
            }
            list.AddRange(richTextBuy);
        }

        bool any = false;
        List<RichTextComponentBase> richTextSellBy = [];
        foreach ((AssetLocation traderCode, TradeProperties props) in TraderInfoSystem.unresolvedTradeProps)
        {
            if (props.Buying.List.Any(x => x.Code == collObj.Code) == true)
            {
                Item? traderItem = capi.World.GetItem(traderCode);
                if (traderItem == null) continue;

                ItemStack traderStack = new ItemStack(traderItem);
                richTextSellBy.AddStack(capi, openDetailPageFor, traderStack);
                any = true;
            }
        }
        if (any)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.PurchasedBy);
            list.AddRange(richTextSellBy);
        }

        any = false;
        List<RichTextComponentBase> richTextBuyBy = [];
        foreach ((AssetLocation traderCode, TradeProperties props) in TraderInfoSystem.unresolvedTradeProps)
        {
            if (props.Selling.List.Any(x => x.Code == collObj.Code) == true)
            {
                Item? traderItem = capi.World.GetItem(traderCode);
                if (traderItem == null) continue;

                ItemStack traderStack = new ItemStack(traderItem);
                richTextBuyBy.AddStack(capi, openDetailPageFor, traderStack);
                any = true;
            }
        }
        if (any)
        {
            list.AddMarginAndTitle(capi, marginTop: 7, titletext: Text.SoldBy);
            list.AddRange(richTextBuyBy);
        }
    }

    private static List<JsonItemStackBuildStage> GetFuelStacks(this BlockPitkiln blockPitKiln, ICoreClientAPI capi)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "pitKilnFuelStacks-" + blockPitKiln.Code, delegate
        {
            JsonItemStackBuildStage[]? fuelStacks = blockPitKiln?.Attributes?["buildMats"]?["fuel"]?.AsObject<JsonItemStackBuildStage[]>();
            if (fuelStacks == null) return [];

            List<JsonItemStackBuildStage> stacks = [.. fuelStacks.Where(stack => stack?.BurnTimeHours != null)];

            foreach (JsonItemStackBuildStage stack in stacks)
            {
                stack.Resolve(capi.World, "");
            }

            return stacks;
        });
    }

    private static Dictionary<ItemStack[], PanningDrop[]> GetPanningDrops(ICoreClientAPI capi, BlockPan blockPan)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "blockPanDrops-" + blockPan.Code, delegate
        {
            Dictionary<string, PanningDrop[]> dropsBySourceMat = blockPan.GetField<Dictionary<string, PanningDrop[]>>("dropsBySourceMat");

            Dictionary<ItemStack[], PanningDrop[]> panningDrops = [];

            foreach (string key in dropsBySourceMat.Keys)
            {
                List<ItemStack> blockStacks = [];
                foreach (Block block in capi.World.Blocks.Where(code => code != null && code.WildCardMatch(key)))
                {
                    string? rocktype = block.Variant["rock"];

                    foreach (PanningDrop drop in dropsBySourceMat[key])
                    {
                        if (drop.Code.Path.Contains("{rocktype}"))
                        {
                            drop.Code.Path = drop.Code.Path.Replace("{rocktype}", rocktype);
                        }
                    }

                    ItemStack stack = new(block);
                    if (!stack.ResolveBlockOrItem(capi.World)) continue;
                    if (stack.Collectible.Variant["layer"] != null) continue;
                    blockStacks.Add(stack);
                }
                panningDrops.Add(blockStacks.ToArray(), dropsBySourceMat[key]);
                blockStacks = [];
            }
            return panningDrops;
        });
    }

    private static List<ItemStack> GetWildcardTroughStacks(ICoreClientAPI capi, ContentConfig config)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "troughWildcardStacks-" + config.Code, delegate
        {
            List<ItemStack> stacks = [];
            foreach (CollectibleObject obj in capi.World.Collectibles.Where(x => x.WildCardMatch(config.Content.Code)))
            {
                ItemStack stack = new(obj);
                if (!stack.ResolveBlockOrItem(capi.World)) continue;
                stacks.Add(stack);
            }
            return stacks;
        });
    }

    private static List<BlockDropItemStack> GetHarvestableDrops(ICoreClientAPI capi, EntityProperties entityType)
    {
        return ObjectCacheUtil.GetOrCreate(capi, "harvestableDrops-" + entityType.Code, delegate
        {
            BlockDropItemStack[]? harvestableDrops = entityType.Attributes?["harvestableDrops"]?.AsObject<BlockDropItemStack[]>();
            if (harvestableDrops == null) return [];

            BlockDropItemStack[] array = harvestableDrops;
            foreach (BlockDropItemStack hstack in array)
            {
                hstack.Resolve(capi.World, "handbook info", new AssetLocation());
            }
            return array.ToList();
        });
    }
}
