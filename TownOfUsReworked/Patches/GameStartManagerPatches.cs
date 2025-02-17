namespace TownOfUsReworked.Patches;

// GameStartManager is a BITCH
[HarmonyPatch(typeof(GameStartManager))]
public static class GameStartManagerPatches
{
    private static PassiveButton CancelStartButton;

    [HarmonyPatch(nameof(GameStartManager.Start)), HarmonyPostfix]
    public static void StartPostfix(GameStartManager __instance)
    {
        // Lobby size requirements
        __instance.MinPlayers = 1;

        if (!__instance.AllMapIcons.Any(x => x.Name == MapNames.Dleks))
        {
            __instance.AllMapIcons.Add(new()
            {
                Name = MapNames.Dleks,
                MapImage = GetSprite("DleksBackground"),
                MapIcon = GetSprite("DleksLobby"),
                NameImage = GetSprite("Dleks")
            });
        }

        if (!__instance.AllMapIcons.Any(x => x.Name == (MapNames)8))
        {
            __instance.AllMapIcons.Add(new()
            {
                Name = (MapNames)8,
                MapImage = GetSprite("RandomMapBackground"),
                MapIcon = GetSprite("RandomMapLobby"),
                NameImage = GetSprite("Random")
            });
        }

        if (CancelStartButton)
            return;

        CancelStartButton = UObject.Instantiate(__instance.StartButton, __instance.transform);
        CancelStartButton.name = "CancelButton";

        var cancelLabel = CancelStartButton.buttonText;
        cancelLabel.GetComponent<TextTranslatorTMP>()?.OnDestroy();
        cancelLabel.text = "Cancel";

        CancelStartButton.inactiveSprites.GetComponent<SpriteRenderer>().color = UColor.black;
        CancelStartButton.activeSprites.GetComponent<SpriteRenderer>().color = UColor.red;

        CancelStartButton.activeTextColor = CancelStartButton.inactiveTextColor = UColor.white;
        CancelStartButton.OverrideOnClickListeners(__instance.ResetStartState);

        __instance.GameStartText.transform.SetLocalY(2f);
    }

    [HarmonyPatch(nameof(GameStartManager.Update)), HarmonyPostfix]
    public static void UpdatePostfix(GameStartManager __instance)
    {
        if (IsCountDown() && (TownOfUsReworked.MciActive || Input.GetKeyDown(KeyCode.LeftShift)))
            __instance.countDownTimer = 0;

        CancelStartButton.gameObject.SetActive(IsCountDown());
    }

    [HarmonyPatch(nameof(GameStartManager.Update))]
    public static Exception Finalizer() => null;

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ResetStartState))]
    public static void Prefix(GameStartManager __instance)
    {
        if (IsCountDown())
            SoundManager.Instance.StopSound(__instance.gameStartSound);
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.SetStartCounter))]
    public static void Prefix(GameStartManager __instance, sbyte sec)
    {
        if (sec == -1)
            SoundManager.Instance.StopSound(__instance.gameStartSound);
    }
}