namespace ExtraInfo.Systems.PileTotalItems;

[HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
public static class PileTotalItemsPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowPileTotalItems != true) return;
        if (__instance == null) return;
        if (__instance.StorageProps?.Layout != EnumGroundStorageLayout.Stacking) return;
        if (__instance.Inventory?.Count == 0) return;

        ICoreAPI api = __instance.Api;
        BlockPos centerPos = __instance.Pos;

        int totalAmount = __instance.GetTotalAmount();
        int totalAmountSame = totalAmount;

        for (int y = centerPos.Y - 1; ; y--)
        {
            BlockPos pos = new(centerPos.X, y, centerPos.Z, centerPos.dimension);
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage? blockEntityGroundStorage))
            {
                if (blockEntityGroundStorage == null) break;

                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (__instance.HasSameContent(blockEntityGroundStorage))
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
            if (api.World.IsGroundStorage(pos, out BlockEntityGroundStorage? blockEntityGroundStorage))
            {
                if (blockEntityGroundStorage == null) break;

                totalAmount += blockEntityGroundStorage.GetTotalAmount();

                if (__instance.HasSameContent(blockEntityGroundStorage))
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
        dsc.Append(ColorText("extrainfo:tabname-general")); // Everything
        dsc.Append(": ");
        dsc.Append(totalAmount).AppendLine();

        dsc.Append(ColorText("extrainfo:Current"));
        dsc.Append(": ");
        dsc.Append(totalAmountSame).AppendLine();
    }
}