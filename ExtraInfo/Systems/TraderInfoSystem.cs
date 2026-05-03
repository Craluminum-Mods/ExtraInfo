namespace ExtraInfo;

/// <summary>
/// Copy pasted from TradeHandbookInfo
/// </summary>
public class TraderInfoSystem : ModSystem
{
    public static Dictionary<AssetLocation, TradeProperties> unresolvedTradeProps = new();

    private ICoreClientAPI capi;

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
            TradeProperties tradeProps = null;
            string stringpath = entitytype.Attributes?["tradePropsFile"].AsString();
            AssetLocation filepath = null;
            JsonObject attributes = entitytype.Attributes;
            if ((attributes != null && attributes["tradeProps"].Exists) || stringpath != null)
            {
                try
                {
                    filepath = ((stringpath == null) ? null : AssetLocation.Create(stringpath, entitytype.Code.Domain));
                    tradeProps = ((!(filepath != null)) ? entitytype.Attributes["tradeProps"].AsObject<TradeProperties>(null, entitytype.Code.Domain) : capi.Assets.Get(filepath.WithPathAppendixOnce(".json")).ToObject<TradeProperties>());
                }
                catch (Exception e)
                {
                    capi.World.Logger.Error("Failed deserializing tradeProps attribute for entitiy {0}, exception logged to verbose debug", entitytype.Code);
                    capi.World.Logger.Error(e);
                    capi.World.Logger.VerboseDebug("Failed deserializing TradeProperties:");
                    capi.World.Logger.VerboseDebug("=================");
                    capi.World.Logger.VerboseDebug("Tradeprops json:");
                    if (filepath != null)
                    {
                        capi.World.Logger.VerboseDebug("File path {0}:", filepath);
                    }
                    capi.World.Logger.VerboseDebug("{0}", entitytype.Server?.Attributes["tradeProps"].ToJsonToken());
                }
            }
            if (tradeProps != null)
            {
                string traderCode = entitytype.Code.Domain + ":creature-" + entitytype.Code.Path;
                unresolvedTradeProps.Add(traderCode, tradeProps);
            }
        }
        capi.Logger.VerboseDebug("[Extra Info] Done traders handbook stuff");
    }

    public override void Dispose()
    {
        unresolvedTradeProps?.Clear();
    }
}