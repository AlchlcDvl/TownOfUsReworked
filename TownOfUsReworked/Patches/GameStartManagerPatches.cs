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

    [HarmonyPatch(nameof(GameStartManager.Update)), HarmonyPrefix]
    public static bool UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance || !GameManager.Instance)
            return false;

        __instance.UpdateMapImage((MapNames)GameManager.Instance.LogicOptions.MapId);
        __instance.CheckSettingsDiffs();
        __instance.RulesPresetText.text = TranslationController.Instance.GetString(GameOptionsManager.Instance.CurrentGameOptions.GetRulesPresetTitle());
        __instance.privatePublicPanelText.text = TranslationController.Instance.GetString(IsLocalGame()
                ? StringNames.LocalButton
                : (AmongUsClient.Instance.IsGamePublic
                    ? StringNames.PublicHeader
                    : StringNames.PrivateHeader));

        __instance.HostPrivateButton.gameObject.SetActive(!AmongUsClient.Instance.IsGamePublic);
        __instance.HostPublicButton.gameObject.SetActive(AmongUsClient.Instance.IsGamePublic);

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            ClipboardHelper.PutClipboardString(GameCode.IntToGameName(AmongUsClient.Instance.GameId));

        if (DiscordManager.InstanceExists)
        {
            __instance.ShareOnDiscordButton.gameObject.SetActive(AmongUsClient.Instance.AmHost && IsOnlineGame() && DiscordManager.Instance.CanShareGameOnDiscord() &&
                DiscordManager.Instance.HasValidPartyID());
        }

        if (GameData.Instance.PlayerCount != __instance.LastPlayerCount)
        {
            __instance.LastPlayerCount = GameData.Instance.PlayerCount;

            var text = "<color=#FF0000FF>";

            if (__instance.LastPlayerCount > __instance.MinPlayers)
                text = "<color=#00FF00FF>";
            else if (__instance.LastPlayerCount == __instance.MinPlayers)
                text = "<color=#FFFF00FF>";

            __instance.PlayerCounter.text = $"{text}{__instance.LastPlayerCount}/{GameSettings.LobbySize}";
            __instance.StartButton.SetButtonEnableState(__instance.LastPlayerCount >= __instance.MinPlayers);
            __instance.StartButtonGlyph?.SetColor(__instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear);

            if (__instance.LastPlayerCount >= __instance.MinPlayers)
            {
                __instance.StartButton.ChangeButtonText(TranslationController.Instance.GetString(StringNames.StartLabel));
                __instance.GameStartTextClient.text = TranslationController.Instance.GetString(StringNames.WaitingForHost);
            }
            else
            {
                __instance.StartButton.ChangeButtonText(TranslationController.Instance.GetString(StringNames.WaitingForPlayers));
                __instance.GameStartTextClient.text = TranslationController.Instance.GetString(StringNames.WaitingForPlayers);
            }

            if (DiscordManager.InstanceExists)
            {
                if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame)
                    DiscordManager.Instance.SetInLobbyHost(__instance.LastPlayerCount, GameManager.Instance.LogicOptions.MaxPlayers, AmongUsClient.Instance.GameId);
                else
                    DiscordManager.Instance.SetInLobbyClient(__instance.LastPlayerCount, GameManager.Instance.LogicOptions.MaxPlayers, AmongUsClient.Instance.GameId);
            }
        }

        if (AmongUsClient.Instance.AmHost)
        {
            if (__instance.startState == GameStartManager.StartingStates.Countdown)
            {
                var num = Mathf.CeilToInt(__instance.countDownTimer);
                __instance.countDownTimer -= Time.deltaTime;
                var num2 = Mathf.CeilToInt(__instance.countDownTimer);

                if (!__instance.GameStartTextParent.activeSelf)
                    SoundManager.Instance.PlaySound(__instance.gameStartSound, false);

                __instance.GameStartTextParent.SetActive(true);
                __instance.GameStartText.text = TranslationController.Instance.GetString(StringNames.GameStarting, num2);

                if (num != num2)
                    PlayerControl.LocalPlayer.RpcSetStartCounter(num2);

                if (num2 <= 0)
                    __instance.FinallyBegin();
            }
            else
            {
                __instance.GameStartTextParent.SetActive(false);
                __instance.GameStartText.text = string.Empty;
            }
        }

        if (__instance.LobbyInfoPane.gameObject.activeSelf && Chat().IsOpenOrOpening)
            __instance.LobbyInfoPane.DeactivatePane();

        __instance.LobbyInfoPane.gameObject.SetActive(!Chat().IsOpenOrOpening);
        var isCd = IsCountDown();

        if (isCd && (TownOfUsReworked.MciActive || Input.GetKeyDown(KeyCode.LeftShift)))
            __instance.countDownTimer = 0;

        CancelStartButton.gameObject.SetActive(isCd && __instance.countDownTimer < 4);
        return false;
    }

    // [HarmonyPatch(nameof(GameStartManager.UpdateMapImage))]
    // public static Exception Finalizer() => null;

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