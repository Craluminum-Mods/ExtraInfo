using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ExtraInfo;

public partial class HarmonyPatches
{
    [HarmonyPatch(typeof(BlockEntityShelf), nameof(BlockEntityShelf.CrockInfoCompact))]
    public static class CrockInfoCompactShelfPatch
    {
        public static void Postfix(ref string __result, ItemSlot inSlot)
        {
            __result = __result.GetCrockSealedInName(inSlot);
        }
    }
}