using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace ExtraInfo.Systems.Handbook.HandbookTraderGoods;

public class Core : ModSystem
{
    public override void AssetsFinalize(ICoreAPI api)
    {
        if (api.Side != EnumAppSide.Client) return;

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj == null || obj.Code == null) continue;
            if (!obj.Code.ToString().Contains("trader")) continue;

            obj.Attributes ??= new JsonObject(new JObject());
            _ = obj.Attributes.Token?["handbook"] ??= new JObject();
            _ = obj.Attributes.Token?["handbook"]?["exclude"] = JToken.FromObject(false);
        }
    }
}