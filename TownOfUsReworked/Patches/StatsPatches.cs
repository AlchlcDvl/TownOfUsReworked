using AmongUs.Data.Player;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerStatsData))]
public static class StatsPatches
{
    [HarmonyPatch(nameof(PlayerStatsData.IncrementStat))]
    public static void Postfix(StatID stat) => CustomStatsManager.IncrementStat(stat);

    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static bool Prefix(out bool __result) => __result = false;
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