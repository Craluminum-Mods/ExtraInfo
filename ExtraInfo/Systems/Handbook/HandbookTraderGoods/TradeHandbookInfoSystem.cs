using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookTraderGoods;

/// <summary>
/// Copy pasted from <see cref="TradeHandbookInfo"/>
/// </summary>
public class TradeHandbookInfoSystem : ModSystem
{
    public static Dictionary<AssetLocation, TradeProperties> unresolvedTradeProps = [];
#nullable disable
    private ICoreClientAPI capi;
#nullable enable
    public override double ExecuteOrder() => 0.15;

    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Client;

    public override void StartClientSide(ICoreClientAPI api)
    {
        capi = api;
        api.Event.LevelFinalize += Event_LevelFinalize;
    }

    private void Event_LevelFinalize()
    {
        foreach (EntityProperties entitytype in capi.World.EntityTypes)
        {
            TradeProperties? tradeProps = null;
            string? stringpath = entitytype.Attributes?["tradePropsFile"].AsString();
            AssetLocation? filepath = null;
            JsonObject? attributes = entitytype.Attributes;
            if ((attributes != null && attributes["tradeProps"].Exists) || stringpath != null)
            {
                try
                {
                    filepath = stringpath == null ? null : AssetLocation.Create(stringpath, entitytype.Code.Domain);
                    tradeProps = filepath == null ? entitytype.Attributes?["tradeProps"].AsObject<TradeProperties>(null, entitytype.Code.Domain) : capi.Assets.Get(filepath.WithPathAppendixOnce(".json")).ToObject<TradeProperties>();
                }
                catch (Exception e)
                {
                    LoggerUtil.Error(capi, this, $"Failed deserializing tradeProps attribute for entitiy {entitytype.Code}, exception logged to verbose debug");
                    LoggerUtil.Error(capi, this, e.ToString());
                    LoggerUtil.Verbose(capi, this, "Failed deserializing TradeProperties:");
                    LoggerUtil.Verbose(capi, this, "=================");
                    LoggerUtil.Verbose(capi, this, "Tradeprops json:");
                    if (filepath != null)
                    {
                        LoggerUtil.Verbose(capi, this, $"File path {filepath}:");
                    }
                    LoggerUtil.Verbose(capi, this, $"{entitytype.Server?.Attributes["tradeProps"].ToJsonToken()}");
                }
            }
            if (tradeProps != null)
            {
                string traderCode = entitytype.Code.Domain + ":creature-" + entitytype.Code.Path;
                unresolvedTradeProps.Add(traderCode, tradeProps);
            }
        }
        LoggerUtil.Verbose(capi, this, "Done traders handbook stuff");
    }

    public override void Dispose()
    {
        unresolvedTradeProps?.Clear();
    }
}