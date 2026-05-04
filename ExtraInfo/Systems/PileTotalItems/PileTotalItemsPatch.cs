using HarmonyLib;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.PileTotalItems;

[HarmonyPatch(typeof(BlockEntityGroundStorage), nameof(BlockEntityGroundStorage.GetBlockInfo))]
public static class PileTotalItemsPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityGroundStorage __instance, StringBuilder dsc)
    {
        if (Config?.ShowPileTotalItems != true) return;
        if (__instance?.Inventory == null || __instance.StorageProps?.Layout != EnumGroundStorageLayout.Stacking) return;
        if (__instance.Inventory.Empty) return;

        ICoreAPI api = __instance.Api;

        BlockPos searchPos = __instance.Pos.Copy();

        int totalAmount = __instance.Inventory.Sum(x => x.StackSize);
        int totalAmountSame = totalAmount;

        int[] dirs = { -1, 1 };

        foreach (int dir in dirs)
        {
            searchPos.Y = __instance.Pos.Y;

            for (int i = 0; i < 64; i++)
            {
                searchPos.Y += dir;

                if (api.World.BlockAccessor.GetBlockEntity(searchPos) is BlockEntityGroundStorage begs)
                {
                    int amount = begs.Inventory.Sum(x => x.StackSize);
                    totalAmount += amount;

                    if (__instance.HasSameContent(begs))
                    {
                        totalAmountSame += amount;
                    }
                }
                else break;
            }
        }

        dsc.AppendLine();
        dsc.AppendLine(Lang.Get("extrainfo:ground-storage-total-pile-item-count", totalAmount));

        if (totalAmountSame != totalAmount)
        {
            dsc.AppendLine(Lang.Get("extrainfo:ground-storage-current-pile-type-count", totalAmountSame));
        }
    }

    private static bool HasSameContent(this BlockEntityGroundStorage a, BlockEntityGroundStorage b)
    {
        ItemStack? stackA = a.Inventory.FirstNonEmptySlot?.Itemstack;
        ItemStack? stackB = b.Inventory.FirstNonEmptySlot?.Itemstack;

        if (stackA == null || stackB == null) return false;

        return stackA.Collectible.Equals(stackA, stackB, GlobalConstants.IgnoredStackAttributes);
    }
}