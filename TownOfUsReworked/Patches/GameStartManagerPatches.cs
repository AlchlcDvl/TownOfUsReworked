namespace TownOfUsReworked.Patches;

public static class GameStartManagerPatches
{
    // The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/GameStartManagerPatch.cs under GPL v3 with some
    // modifications from me to account for MCI and the horrid vanilla spam in the logs
    public static readonly Dictionary<int, PlayerVersion> PlayerVersions = [];
    private static float KickingTimer;
    private static bool VersionSent;
    private static PlayerVersion SelfVersion;

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix()
        {
            if (CustomPlayer.Local && !TownOfUsReworked.MCIActive)
                ShareGameVersion();
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public static class GameStartManagerStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            // Lobby size requirements
            __instance.MinPlayers = 1;

            if (!IsHnS)
            {
                // Trigger version refresh
                VersionSent = false;
                // Reset kicking timer
                KickingTimer = 0f;
                // Clear player versions
                PlayerVersions.Clear();
                // Clear self player version
                SelfVersion = null;
            }

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
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagerUpdatePatch
    {
        private static int Seconds;

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
            if (!AmongUsClient.Instance || !GameData.Instance || !GameManager.Instance)
                return;

            __instance.UpdateMapImage((MapNames)MapSettings.Map);
            __instance.CheckSettingsDiffs();
            __instance.RulesPresetText.text = TranslationController.Instance.GetString(GameOptionsManager.Instance.CurrentGameOptions.GetRulesPresetTitle());
            __instance.privatePublicPanelText.text = TranslationController.Instance.GetString(GameCode.IntToGameName(AmongUsClient.Instance.GameId) == null ? StringNames.LocalButton :
                (AmongUsClient.Instance.IsGamePublic ? StringNames.PublicHeader : StringNames.PrivateHeader));
            __instance.HostPrivateButton.gameObject.SetActive(!AmongUsClient.Instance.IsGamePublic);
            __instance.HostPublicButton.gameObject.SetActive(AmongUsClient.Instance.IsGamePublic);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C) && !Chat.IsOpenOrOpening)
                ClipboardHelper.PutClipboardString(GameCode.IntToGameName(AmongUsClient.Instance.GameId));

            if (DiscordManager.InstanceExists)
            {
                __instance.ShareOnDiscordButton.gameObject.SetActive(AmongUsClient.Instance.AmHost && IsOnlineGame && DiscordManager.Instance.CanShareGameOnDiscord() &&
                    DiscordManager.Instance.HasValidPartyID());
            }

            if (GameData.Instance.PlayerCount != __instance.LastPlayerCount)
            {
                __instance.LastPlayerCount = GameData.Instance.PlayerCount;
                var arg = "#FF0000FF";

                if (__instance.LastPlayerCount > __instance.MinPlayers)
                    arg = "#00FF00FF>";
                else if (__instance.LastPlayerCount == __instance.MinPlayers)
                    arg = "#FFFF00FF";

                __instance.PlayerCounter.text = $"<color={arg}>{__instance.LastPlayerCount}/{GameSettings.LobbySize}</color>";
                __instance.PlayerCounter.enableWordWrapping = false;
                __instance.StartButton.SetButtonEnableState(__instance.LastPlayerCount >= __instance.MinPlayers);
                __instance.StartButtonGlyph?.SetColor(__instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear);
                __instance.StartButton.ChangeButtonText(TranslationController.Instance.GetString(__instance.LastPlayerCount >= __instance.MinPlayers ? StringNames.StartLabel :
                    StringNames.WaitingForPlayers));

                if (DiscordManager.InstanceExists)
                {
                    if (AmongUsClient.Instance.AmHost && IsOnlineGame)
                        DiscordManager.Instance.SetInLobbyHost(__instance.LastPlayerCount, GameSettings.LobbySize, AmongUsClient.Instance.GameId);
                    else
                        DiscordManager.Instance.SetInLobbyClient(__instance.LastPlayerCount, GameSettings.LobbySize, AmongUsClient.Instance.GameId);
                }
            }

            // Check version handshake infos
            var versionMismatch = false;
            var message = "";

            // Send version as soon as local player exists
            if (CustomPlayer.Local && !VersionSent)
            {
                VersionSent = true;
                SelfVersion = ShareGameVersion();
            }

            if (!TownOfUsReworked.MCIActive && !IsHnS)
            {
                foreach (var client in AmongUsClient.Instance.allClients)
                {
                    if (!client.Character)
                        continue;

                    if (PlayerVersions.TryGetValue(client.Id, out var pv))
                    {
                        if (pv.Diff > 0)
                        {
                            versionMismatch = true;
                            message += $"{client.PlayerName} has an older version of Town Of Us Reworked (v{pv.VersionString})\n";
                        }
                        else if (pv.Diff < 0)
                        {
                            versionMismatch = true;
                            message += $"{client.PlayerName} has a newer version of Town Of Us Reworked (v{pv.VersionString})\n";
                        }
                        else if (!pv.DevMatches || !pv.StreamMatches || !pv.DevBuildMatches)
                        {
                            // Version presumably matches, check if Dev/Stream matches
                            versionMismatch = true;
                            message += $"{client.PlayerName} has a mismatching non-public version of Town Of Us Reworked (v{pv.VersionString})\n";
                        }
                        else if (!pv.GuidMatches || !pv.VersionStringMatches)
                        {
                            // Version presumably matches, check if Guid or Version string matches
                            versionMismatch = true;
                            message += $"{client.PlayerName} has a modified version of Town Of Us Reworked (v{pv.VersionString})\n";
                        }
                        else if (!pv.EverythingMatches)
                        {
                            // Somehow something's still not matching, so just display that the player has something wrong
                            versionMismatch = true;
                            message += $"You or {client.PlayerName} somehow still has a version mismatch, please share logs\n";
                            LogWarning($"{client.PlayerName}\nGuid - {pv.GuidMatches} Dev - {pv.DevMatches} Stream - {pv.StreamMatches} Dev Build - {pv.DevBuildMatches} Version String - " +
                                $"{pv.VersionString} Version - {pv.Version}\n\n{CustomPlayer.Local.Data.PlayerName}\nGuid - {SelfVersion.GuidMatches} Dev - {SelfVersion.DevMatches} Stream" +
                                $" - {SelfVersion.StreamMatches} Dev Build - {SelfVersion.DevBuildMatches} Version String - {SelfVersion.VersionString} Version - {SelfVersion.Version}");
                        }
                    }
                }

                // Display message to the host
                if (AmongUsClient.Instance.AmHost)
                {
                    if (versionMismatch)
                    {
                        __instance.StartButton.SetButtonEnableState(false);
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                    }
                    else if (!IsCountDown)
                        __instance.StartButton.SetButtonEnableState(true);
                }
                // Client update with handshake infos
                else if (!PlayerVersions.TryGetValue(AmongUsClient.Instance.HostId, out var pv) || pv.Diff != 0)
                {
                    KickingTimer += Time.deltaTime;

                    if (KickingTimer > 10)
                    {
                        KickingTimer = 0;
                        AmongUsClient.Instance.ExitGame(DisconnectReasons.Kicked);
                        SceneChanger.ChangeScene("MainMenu");
                    }

                    __instance.GameStartText.text = $"You or the host has no or a different version of Town Of Us Reworked\nYou will be kicked in {Math.Round(10 - KickingTimer)}s";
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                }
                else if (versionMismatch)
                {
                    __instance.GameStartText.text = "Players with different versions:\n" + message;
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                }
            }

            if (!versionMismatch)
                __instance.GameStartText.text = IsCountDown ? TranslationController.Instance.GetString(StringNames.GameStarting, Seconds) : "";

            if (__instance.LobbyInfoPane.gameObject.activeSelf && Chat.IsOpenOrOpening)
                __instance.LobbyInfoPane.DeactivatePane();

            __instance.LobbyInfoPane.gameObject.SetActive(!Chat.IsOpenOrOpening);

            if (!AmongUsClient.Instance.AmHost)
                return;

            if (IsCountDown)
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

                if (!versionMismatch)
                {
                    __instance.GameStartTextParent.SetActive(true);
                    __instance.GameStartText.text = TranslationController.Instance.GetString(StringNames.GameStarting, Seconds);
                }

                if (num != Seconds)
                    CustomPlayer.Local.RpcSetStartCounter(Seconds);

                if (Seconds <= 0)
                {
                    __instance.FinallyBegin();
                    // Clear player versions right before the game starts
                    PlayerVersions.Clear();
                }
            }
            else
            {
                __instance.GameStartTextParent.SetActive(false);
                __instance.GameStartText.text = string.Empty;
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public static class GameStartManagerBeginGame
    {
        public static bool Prefix()
        {
            // Block game start if not everyone has the same mod version
            var continueStart = true;

            if (!TownOfUsReworked.MCIActive && AmongUsClient.Instance.AmHost)
            {
                foreach (var client in AmongUsClient.Instance.allClients)
                {
                    if (!client.Character)
                        continue;

                    var dummyComponent = client.Character.GetComponent<DummyBehaviour>();

                    if (dummyComponent?.enabled == true)
                        continue;

                    if (!PlayerVersions.TryGetValue(client.Id, out var pv))
                    {
                        continueStart = false;
                        break;
                    }

                    if (!pv.EverythingMatches)
                    {
                        continueStart = false;
                        break;
                    }
                }
            }

            return continueStart;
        }
    }
}