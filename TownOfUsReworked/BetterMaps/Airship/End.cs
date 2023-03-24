using HarmonyLib;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class EndGamePatch
    {
        public static void Prefix() => SpawnInMinigamePatch.ResetGlobalVariable();
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGameManagerPatch
    {
        public static bool Prefix()
        {
            SpawnInMinigamePatch.ResetGlobalVariable();
            return true;
        }
    }
}