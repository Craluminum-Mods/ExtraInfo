global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using Vintagestory.API.Client;
global using Vintagestory.API.Common;
global using Vintagestory.API.Common.Entities;
global using Vintagestory.API.Config;
global using Vintagestory.API.Datastructures;
global using Vintagestory.API.MathTools;
global using Vintagestory.API.Util;
global using Vintagestory.Client.NoObf;
global using Vintagestory.GameContent;
global using static ExtraInfo.TextExtensions;
global using static ExtraInfo.Constants;
using ExtraInfo.Configuration;

namespace ExtraInfo;

public class Core : ModSystem
{
    public static Config Config { get; set; }

    public override void StartPre(ICoreAPI api)
    {
        Config = ModConfig.ReadConfig(api);

        if (api.ModLoader.IsModEnabled("configlib"))
        {
            _ = new ConfigLibCompatibility(api);
        }
    }

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("ExtraInfo:TreeGrowthDescription", typeof(CollectibleBehaviorTreeGrowthDescription));
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client)
        {
            return;
        }

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj is ItemTreeSeed or BlockPlant)
            {
                obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorTreeGrowthDescription(obj));
            }
        }
    }
}
