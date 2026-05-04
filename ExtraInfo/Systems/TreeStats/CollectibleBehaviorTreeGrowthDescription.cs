using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using static ExtraInfo.TextExtensions;

namespace ExtraInfo.Systems.TreeStats;

public class CollectibleBehaviorTreeGrowthDescription : CollectibleBehavior
{
    public CollectibleBehaviorTreeGrowthDescription(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        if (Config?.ShowTreeStats != true) return;
        AppendInfo(inSlot, dsc, world);
    }

    private static void AppendInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
    {
        if (inSlot?.Itemstack?.Collectible == null) return;

        NatFloat? sproutDays = null;
        NatFloat? matureDays = null;

        if (inSlot.Itemstack.Collectible is ItemTreeSeed item)
        {
            Block? block = world.GetBlock($"{item.Code.Domain}:sapling-{item.Variant["type"]}-free");
            if (block == null) return;
            if (block.Attributes == null) return;

            sproutDays = block.Attributes["sproutDays"].AsObject<NatFloat>();
            matureDays = block.Attributes["matureDays"].AsObject<NatFloat>();
        }
        else if (inSlot.Itemstack.Collectible is BlockPlant block && !string.IsNullOrEmpty(block.EntityClass))
        {
            if (block.Attributes == null) return;

            sproutDays = block.Attributes["sproutDays"].AsObject<NatFloat>();
            matureDays = block.Attributes["matureDays"].AsObject<NatFloat>();
        }

        if (sproutDays != null)
        {
            dsc.AppendLine(Lang.Get("extrainfo:sprout-in-days", GetMin(sproutDays), GetMax(sproutDays)));
        }
        if (matureDays != null)
        {
            dsc.AppendLine(Lang.Get("extrainfo:mature-in-days", GetMin(matureDays), GetMax(matureDays)));
        }
    }
}