using HarmonyLib;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.BloomeryProgress;

[HarmonyPatch(typeof(BlockEntityBloomery), nameof(BlockEntityBloomery.GetBlockInfo))]
public static class BloomeryProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBloomery __instance, StringBuilder dsc, double ___burningUntilTotalDays, double ___burningStartTotalDays, bool ___burning)
    {
        if (Config?.ShowBloomeryProgress != true) return;

        if (__instance?.Api == null) return;

        if (!___burning) return;

        float hoursPerDay = __instance.Api.World.Calendar.HoursPerDay;
        double totalDurationHours = (___burningUntilTotalDays - ___burningStartTotalDays) * hoursPerDay;
        double hoursLeft = (___burningUntilTotalDays - __instance.Api.World.Calendar.TotalDays) * hoursPerDay;

        if (hoursLeft > 0 && totalDurationHours > 0)
        {
            TimeBasedProgressBarProperties barProps = new()
            {
                HeaderKey = Lang.Get("extrainfo:WillFinishIn"),
                HoursTotal = totalDurationHours,
                HoursLeft = hoursLeft
            };

            string verticalBlock = TimeFormatter.BuildTimeBlockPlusProgressBar(__instance.Api, barProps);
            
            dsc.AppendLine().Append(verticalBlock);
        }
    }
}