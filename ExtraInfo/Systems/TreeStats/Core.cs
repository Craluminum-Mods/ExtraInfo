using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.TreeStats;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TreeGrowthDescription", typeof(CollectibleBehaviorTreeGrowthDescription));
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client) return;

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null || obj.Code == null) continue;
            if (obj is not ItemTreeSeed and not BlockPlant) continue;

            obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTreeGrowthDescription(obj));
        }
    }
}