namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(StatsManager))]
public static class StatsPatches
{
    [HarmonyPatch(nameof(StatsManager.LoadStats)), HarmonyPrefix]
    public static bool LoadStatsPrefix(StatsManager __instance)
    {
        if (__instance.loadedStats)
            return false;

        __instance.loadedStats = true;
        __instance.logger.Info("LoadStats - Custom");
        __instance.stats = new();
        var path = Path.Combine(PlatformPaths.persistentDataPath, "reworkedStats");
        var end = false;

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                CustomStatsManager.DeserializeCustomStats(reader);
            } catch {}

            end = true;
        }

        if (!CustomStatsManager.MigratedFromVanillaStats)
        {
            var path2 = Path.Combine(PlatformPaths.persistentDataPath, "playerStats3");

            if (File.Exists(path2))
            {
                try
                {
                    var reader = new IlIO.BinaryReader(IlIO.File.OpenRead(path2));
                    __instance.stats.Deserialize(reader);
                    CustomStatsManager.MigrateFromVanillaStats(__instance);
                    reader.Dispose();
                } catch {}

                end = true;
            }
        }

        if (!end)
            __instance.ResetStatDisplay();

        return false;
    }

    [HarmonyPatch(nameof(StatsManager.SaveStats)), HarmonyPrefix]
    public static bool SaveStatsPrefix(StatsManager __instance)
    {
        __instance.logger.Info("SaveStats - Custom");

        try
        {
            using var writer = new BinaryWriter(File.OpenWrite(Path.Combine(PlatformPaths.persistentDataPath, "reworkedStats")));
            CustomStatsManager.SerializeCustomStats(writer);
        }
        catch (Exception ex)
        {
            __instance.logger.Error($"Failed to write out stats: {ex}");
        }

        return false;
    }

    [HarmonyPatch(nameof(StatsManager.ResetStatDisplay))]
    public static void Postfix() => CustomStatsManager.Reset();

    [HarmonyPatch(nameof(StatsManager.IncrementStat)), HarmonyPrefix]
    public static bool IncrementStatPrefix(StringNames statName)
    {
        CustomStatsManager.IncrementStat(statName);
        return false;
    }

    [HarmonyPatch(nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static void Postfix(out bool __result) => __result = false;
}

[HarmonyPatch(typeof(StatsPopup))]
public static class PatchPopup
{
    [HarmonyPatch(nameof(StatsPopup.DisplayGameStats))]
    public static bool Prefix(StatsPopup __instance)
    {
        var sb = new Il2CppSystem.Text.StringBuilder();

        foreach (var ordered in CustomStatsManager.OrderedStats)
            StatsPopup.AppendStat(sb, ordered, CustomStatsManager.GetStat(ordered));

        StatsPopup.AppendStat(sb, StringNames.StatsFastestCrewmateWin_HideAndSeek, StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekCrewmateWin()));
        StatsPopup.AppendStat(sb, StringNames.StatsFastestImpostorWin_HideAndSeek, StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekImpostorWin()));
        StatsPopup.AppendStat(sb, StringNames.StatsHideAndSeekCrewmateVictory, StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByTimer));
        StatsPopup.AppendStat(sb, StringNames.StatsHideAndSeekImpostorVictory, StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByKills));
        __instance.StatsText.SetText(sb);
        return false;
    }
}