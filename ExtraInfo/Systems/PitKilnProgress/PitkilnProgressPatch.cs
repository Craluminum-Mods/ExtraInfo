namespace ExtraInfo.Systems.PitKilnProgress;

[HarmonyPatch(typeof(BlockEntityPitKiln), nameof(BlockEntityPitKiln.GetBlockInfo))]
public static class PitKilnProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityPitKiln __instance, StringBuilder dsc)
    {
        if (Core.Config?.ShowPitKilnProgress != true) return;
        if (__instance?.Api == null || !__instance.Lit) return;

        ICoreAPI api = __instance.Api;

        double targetHours = __instance.BurningUntilTotalHours;
        double currentHours = api.World.Calendar.TotalHours;
        double hoursRemaining = targetHours - currentHours;

        if (hoursRemaining > 0)
        {
            float totalDuration = __instance.BurnTimeHours;

            double startHours = targetHours - totalDuration;
            double elapsed = currentHours - startHours;

            float completedPercent = (float)Math.Clamp((elapsed / totalDuration) * 100, 0, 100);

            double igSeconds = hoursRemaining * 3600;

            float speedOfTime = api.World.Calendar.SpeedOfTime;

            string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(
                igSeconds: igSeconds,
                speedOfTime: speedOfTime,
                completedPercent: completedPercent,
                headerKey: Lang.Get("extrainfo:WillFinishIn"));

            dsc.AppendLine().Append(verticalBlock);
        }
    }
}