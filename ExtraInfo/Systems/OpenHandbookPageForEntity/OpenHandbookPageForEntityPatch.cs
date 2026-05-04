using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.OpenHandbookPageForEntity;

[HarmonyPatch(typeof(ModSystemSurvivalHandbook), methodName: "OnSurvivalHandbookHotkey")]
public static class OpenHandbookPageForEntityPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ModSystemSurvivalHandbook __instance, ref bool __result, GuiDialogHandbook ___dialog, ICoreClientAPI ___capi)
    {
        if (Config?.OpenHandbookPageForEntity != true) return true;
        if (___capi.World.Player.CurrentEntitySelection == null) return true;

        Entity entity = ___capi.World.Player.CurrentEntitySelection.Entity;

        ItemStack? stack = ___capi.World.GetEntityType(entity.Code)?.GetCreatureStack(___capi);
        if (stack == null)
        {
            __result = true;
            return false;
        }

        if (!___dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(stack)))
        {
            ___dialog.OpenDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(new ItemStack(stack.Collectible)));
        }
        return true;
    }
}