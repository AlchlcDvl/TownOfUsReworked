namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameStartManager))]
public static class GameStartManagerPatches
{
    // The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/GameStartManagerPatch.cs under GPL v3 with some
    // modifications from me to account for MCI and the horrid vanilla spam in the logs

    [HarmonyPatch(nameof(GameStartManager.Start))]
    public static void Postfix(GameStartManager __instance)
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
    }

    private static int Seconds;

    [HarmonyPatch(nameof(GameStartManager.Update))]
    public static bool Prefix(GameStartManager __instance)
    {
        try
        {
            UpdatePrefix(__instance);
        } catch {}

        return false;
    }

    private static void UpdatePrefix(GameStartManager __instance)
    {
        if (!AmongUsClient.Instance)
            return;

        __instance.UpdateMapImage((MapNames)MapSettings.Map);
        __instance.CheckSettingsDiffs();
        __instance.RulesPresetText.text = TranslationController.Instance.GetString(GameOptionsManager.Instance.CurrentGameOptions.GetRulesPresetTitle());
        __instance.privatePublicPanelText.text = TranslationController.Instance.GetString(GameCode.IntToGameName(AmongUsClient.Instance.GameId) == null ? StringNames.LocalButton :
            (AmongUsClient.Instance.IsGamePublic ? StringNames.PublicHeader : StringNames.PrivateHeader));
        __instance.HostPrivateButton.gameObject.SetActive(!AmongUsClient.Instance.IsGamePublic);
        __instance.HostPublicButton.gameObject.SetActive(AmongUsClient.Instance.IsGamePublic);

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C) && !Chat().IsOpenOrOpening)
            ClipboardHelper.PutClipboardString(GameCode.IntToGameName(AmongUsClient.Instance.GameId));

        if (DiscordManager.InstanceExists)
        {
            __instance.ShareOnDiscordButton.gameObject.SetActive(AmongUsClient.Instance.AmHost && IsOnlineGame() && DiscordManager.Instance.CanShareGameOnDiscord() &&
                DiscordManager.Instance.HasValidPartyID());
        }

        if (GameData.Instance?.PlayerCount != __instance.LastPlayerCount)
        {
            __instance.LastPlayerCount = GameData.Instance.PlayerCount;
            var arg = "FF00";

            if (__instance.LastPlayerCount > __instance.MinPlayers)
                arg = "00FF";
            else if (__instance.LastPlayerCount == __instance.MinPlayers)
                arg = "FFFF";

            __instance.PlayerCounter.text = $"<#{arg}00FF>{__instance.LastPlayerCount}/{GameSettings.LobbySize}</color>";
            __instance.PlayerCounter.enableWordWrapping = false;
            __instance.StartButton.SetButtonEnableState(__instance.LastPlayerCount >= __instance.MinPlayers);
            __instance.StartButtonGlyph?.SetColor(__instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear);
            __instance.StartButton.ChangeButtonText(TranslationController.Instance.GetString(__instance.LastPlayerCount >= __instance.MinPlayers ? StringNames.StartLabel :
                StringNames.WaitingForPlayers));

            if (DiscordManager.InstanceExists)
            {
                if (AmongUsClient.Instance.AmHost && IsOnlineGame())
                    DiscordManager.Instance.SetInLobbyHost(__instance.LastPlayerCount, GameSettings.LobbySize, AmongUsClient.Instance.GameId);
                else
                    DiscordManager.Instance.SetInLobbyClient(__instance.LastPlayerCount, GameSettings.LobbySize, AmongUsClient.Instance.GameId);
            }
        }

        __instance.GameStartText.text = IsCountDown() ? TranslationController.Instance.GetString(StringNames.GameStarting, Seconds) : "";
        __instance.GameStartTextParent.SetActive(IsCountDown());

        if (__instance.LobbyInfoPane.gameObject.activeSelf && Chat().IsOpenOrOpening)
            __instance.LobbyInfoPane.DeactivatePane();

        __instance.LobbyInfoPane.gameObject.SetActive(!Chat().IsOpenOrOpening);

        if (!AmongUsClient.Instance.AmHost)
            return;

        if (IsCountDown())
        {
            if (TownOfUsReworked.MCIActive)
                Seconds = 0;
            else
            {
                var num = Mathf.CeilToInt(__instance.countDownTimer);
                __instance.countDownTimer -= Time.deltaTime;
                Seconds = Mathf.CeilToInt(__instance.countDownTimer);

                if (Input.GetKeyDown(KeyCode.LeftShift))
                    __instance.countDownTimer = 0;

                if (Input.GetKeyDown(KeyCode.LeftControl))
                    __instance.ResetStartState();

                if (!__instance.GameStartTextParent.activeSelf)
                    SoundManager.Instance.PlaySound(__instance.gameStartSound, false);

                __instance.GameStartTextParent.SetActive(true);
                __instance.GameStartText.text = TranslationController.Instance.GetString(StringNames.GameStarting, Seconds);

                if (num != Seconds)
                    CustomPlayer.Local.RpcSetStartCounter(Seconds);
            }

            if (Seconds <= 0)
                __instance.FinallyBegin();
        }
        else
        {
            __instance.GameStartTextParent.SetActive(false);
            __instance.GameStartText.text = "";
        }
    }
}