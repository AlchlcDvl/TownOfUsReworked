namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(StatsManager))]
public static class StatsPatches
{
    [HarmonyPatch(nameof(StatsManager.SaveStats)), HarmonyPrefix]
    public static void SaveStatsPrefix(StatsManager __instance)
    {
        try
        {
            using var writer = new BinaryWriter(File.OpenWrite(Path.Combine(PlatformPaths.persistentDataPath, "reworkedStats")));
            CustomStatsManager.SerializeCustomStats(writer);
        }
        catch (Exception ex)
        {
            __instance.logger.Error($"Failed to write out stats reworked: {ex}");
        }
    }

    [HarmonyPatch(nameof(StatsManager.ResetStatDisplay))]
    public static void Postfix() => CustomStatsManager.Reset();

    [HarmonyPatch(nameof(StatsManager.IncrementStat)), HarmonyPrefix]
    public static bool IncrementStatPrefix(StringNames statName)
    {
        CustomStatsManager.IncrementStat(statName, out var success);
        return !success;
    }

    [HarmonyPatch(nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static void Postfix(out bool __result) => __result = false;
}

[HarmonyPatch(typeof(StatsPopup))]
public static class PatchPopup
{
    [HarmonyPatch(nameof(StatsPopup.DisplayGameStats))]
    [HarmonyPatch(nameof(StatsPopup.DisplayRoleStats))]
    public static bool Prefix() => false;

    [HarmonyPatch(nameof(StatsPopup.OnEnable))]
    public static void Postfix(StatsPopup __instance)
    {
        __instance.SelectableButtons.ForEach(x => x.gameObject.SetActive(false));
        __instance.EnsureComponent<StatsHandler>(); // Haha scroller simulator go brrr
    }
}

[HarmonyPatch(typeof(StatsManager.Stats), nameof(StatsManager.Stats.Deserialize))]
public static class TryMigrateStats
{
    public static void Postfix()
    {
        if (!CustomStatsManager.MigratedFromVanillaStats)
            CustomStatsManager.MigrateFromVanillaStats(StatsManager.Instance);
    }
}