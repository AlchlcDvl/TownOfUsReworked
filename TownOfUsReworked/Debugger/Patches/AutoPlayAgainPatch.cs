namespace TownOfUsReworked.Debugger.Patches;

[HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation))]
public static class AutoPlayAgainPatch
{
    public static void Postfix(EndGameNavigation __instance)
    {
        if (TownOfUsReworked.AutoPlayAgain.Value)
            __instance.NextGame();
    }
}