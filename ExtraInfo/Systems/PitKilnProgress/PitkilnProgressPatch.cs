using HarmonyLib;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.PitKilnProgress;

[HarmonyPatch(typeof(BlockEntityPitKiln), nameof(BlockEntityPitKiln.GetBlockInfo))]
public static class PitKilnProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityPitKiln __instance, StringBuilder dsc)
    {
        if (Config?.ShowPitKilnProgress != true) return;
        if (__instance?.Api == null || !__instance.Lit) return;

        ICoreAPI api = __instance.Api;

        double targetHours = __instance.BurningUntilTotalHours;
        double currentHours = api.World.Calendar.TotalHours;
        double hoursRemaining = targetHours - currentHours;

        if (hoursRemaining > 0)
        {
            var props = new TimeBasedProgressBarProperties()
            {
                HeaderKey = Lang.Get("extrainfo:WillFinishIn"),
                HoursTotal = __instance.BurnTimeHours,
                HoursLeft = hoursRemaining
            };

            string verticalBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(api, props);

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}