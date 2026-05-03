namespace ExtraInfo.Systems.BloomeryProgress;

[HarmonyPatch(typeof(BlockEntityBloomery), nameof(BlockEntityBloomery.GetBlockInfo))]
public static class BloomeryProgressPatch
{
    [HarmonyPostfix]
    public static void Postfix(BlockEntityBloomery __instance, StringBuilder dsc, double ___burningUntilTotalDays, double ___burningStartTotalDays)
    {
        if (Core.Config?.ShowBloomeryProgress != true) return;
        if (__instance?.Api == null) return;

        if (__instance.GetField<bool>("burning"))
        {
            ICoreAPI api = __instance.Api;

            double totalDurationDays = ___burningUntilTotalDays - ___burningStartTotalDays;
            double remainingDays = ___burningUntilTotalDays - api.World.Calendar.TotalDays;

            if (remainingDays > 0 && totalDurationDays > 0)
            {
                double elapsedDays = api.World.Calendar.TotalDays - ___burningStartTotalDays;
                float completedPercent = (float)Math.Clamp((elapsedDays / totalDurationDays) * 100, 0, 100);

                double igSeconds = remainingDays * api.World.Calendar.HoursPerDay * 3600;
                float speedOfTime = api.World.Calendar.SpeedOfTime;

                string verticalBlock = TimeFormatter.BuildVerticalTimeBlock(igSeconds, speedOfTime, completedPercent);

                dsc.AppendLine().Append(verticalBlock);
            }
        }
    }
}