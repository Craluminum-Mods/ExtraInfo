namespace ExtraInfo;

public class CollectibleBehaviorTreeGrowthDescription : CollectibleBehavior
{
    public CollectibleBehaviorTreeGrowthDescription(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        if (Core.Config == null || !Core.Config.ShowTreeStats)
        {
            return;
        }

        AppendInfo(inSlot, dsc, world);
    }

    private static void AppendInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
    {
        if (inSlot?.Itemstack?.Collectible == null) return;

        NatFloat sproutDays = null;
        NatFloat matureDays = null;

        if (inSlot.Itemstack.Collectible is ItemTreeSeed item)
        {
            Block block = world.GetBlock($"{item.Code.Domain}:sapling-{item.Variant["type"]}-free");
            if (block == null) return;
            if (block.Attributes == null) return;

            sproutDays = block.Attributes[Text.SproutDaysAttr].AsObject<NatFloat>();
            matureDays = block.Attributes[Text.MatureDaysAttr].AsObject<NatFloat>();
        }
        else if (inSlot.Itemstack.Collectible is BlockPlant block && !string.IsNullOrEmpty(block.EntityClass))
        {
            if (block.Attributes == null) return;

            sproutDays = block.Attributes[Text.SproutDaysAttr].AsObject<NatFloat>();
            matureDays = block.Attributes[Text.MatureDaysAttr].AsObject<NatFloat>();
        }

        if (sproutDays != null)
        {
            dsc.AppendLine(ColorText(Text.WillSproutIn(GetMin(sproutDays), GetMax(sproutDays))));
        }
        if (matureDays != null)
        {
            dsc.AppendLine(ColorText(Text.WillMatureIn(GetMin(matureDays), GetMax(matureDays))));
        }
    }
}