using AmongUs.Data.Player;

namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(PlayerStatsData), nameof(PlayerStatsData.IncrementStat))]
public static class StatsPatches
{
    public static void Postfix(StatID stat) => CustomStatsManager.IncrementStat(stat);
}

[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
public static class BanPatch
{
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
        __instance.SelectableButtons.Do(x => x.gameObject.SetActive(false));
        __instance.EnsureComponent<StatsHandler>(); // Haha scroller simulator go brrr
    }
}