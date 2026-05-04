global using static ExtraInfo.Systems.Core;
using ExtraInfo.Configuration;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems;

public class Core : ModSystem
{
    public Harmony HarmonyInstance => new(Mod.Info.ModID);
#nullable disable
    public static Config Config { get; set; }
#nullable enable
    public override void StartPre(ICoreAPI api)
    {
        Config = ModConfig.ReadConfig(api);

        if (api.ModLoader.IsModEnabled("configlib"))
        {
            _ = new ConfigLibCompatibility(api);
        }
        
        HarmonyInstance.PatchAllUncategorized();
    }

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TreeGrowthDescription", typeof(CollectibleBehaviorTreeGrowthDescription));
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client) return;

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null || obj.Code == null) continue;

            if (obj.Code.ToString().Contains("trader"))
            {
                obj.Attributes ??= new JsonObject(new JObject());
                _ = obj.Attributes.Token?["handbook"] ??= new JObject();
                _ = obj.Attributes.Token?["handbook"]?["exclude"] = JToken.FromObject(false);
            }
            if (obj is ItemTreeSeed or BlockPlant)
            {
                obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTreeGrowthDescription(obj));
            }
        }
    }

    public override void Dispose()
    {
        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }
}
