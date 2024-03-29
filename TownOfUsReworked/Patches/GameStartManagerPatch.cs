namespace TownOfUsReworked.Patches;

public static class GameStartManagerPatch
{
    // The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/GameStartManagerPatch.cs under GPL v3 with some
    // modifications from me to account for MCI and the horrid vanilla spam in the logs
    public static readonly Dictionary<int, PlayerVersion> PlayerVersions = [];
    private static float KickingTimer;
    private static bool VersionSent;

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
        public static bool Prefix(GameStartManager __instance)
        {
            try
            {
                StartPrefix(__instance);
            } catch {}

            return false;
        }

        private static void StartPrefix(GameStartManager __instance)
        {
            if (TutorialManager.InstanceExists)
                __instance.gameObject.Destroy();
            else
            {
                DataManager.Settings.Gameplay.OnStreamerModeChanged += new Action(__instance.UpdateStreamerModeUI);
                __instance.UpdateStreamerModeUI();

                if (GameCode.IntToGameName(AmongUsClient.Instance.GameId) != null)
                    __instance.GameRoomNameInfo.text = TranslationController.Instance.GetString(StringNames.RoomCodeInfo);
                else
                {
                    __instance.StartButton.transform.localPosition = new(0f, -0.2f, 0f);
                    __instance.PlayerCounter.transform.localPosition = new(0f, -0.8f, 0f);
                    __instance.GameRoomButton.gameObject.SetActive(false);
                }

                AmongUsClient.Instance.DisconnectHandlers.AddUnique(__instance.Cast<IDisconnectHandler>());

                if (!AmongUsClient.Instance.AmHost)
                {
                    __instance.StartButton.gameObject.SetActive(false);
                    __instance.MakePublicButtonBehaviour.enabled = false;
                    var componentInChildren = __instance.MakePublicButton.GetComponentInChildren<ActionMapGlyphDisplay>(true);

                    if (componentInChildren)
                        componentInChildren.gameObject.SetActive(false);
                }
                else
                {
                    LobbyBehaviour.Instance = UObject.Instantiate(__instance.LobbyPrefab);
                    AmongUsClient.Instance.Spawn(Lobby);
                }

                __instance.MakePublicButton.gameObject.SetActive(IsOnlineGame);
                __instance.ShareOnDiscordButton.gameObject.SetActive(false);
            }

            // Lobby size
            __instance.MinPlayers = 1;
            Generate.LobbySize.Set((float)TownOfUsReworked.NormalOptions?.MaxPlayers);

            if (!IsHnS)
            {
                // Trigger version refresh
                VersionSent = false;
                // Reset kicking timer
                KickingTimer = 0f;
                // Copy lobby code
                GUIUtility.systemCopyBuffer = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
                // Clear player versions
                PlayerVersions.Clear();
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
            if (!AmongUsClient.Instance || !GameData.Instance || !GameManager.Instance || IsInGame)
                return;

            __instance.MakePublicButton.sprite = AmongUsClient.Instance.IsGamePublic ? __instance.PublicGameImage : __instance.PrivateGameImage;
            __instance.privatePublicText.text = TranslationController.Instance.GetString(AmongUsClient.Instance.IsGamePublic ? StringNames.PublicHeader : StringNames.PrivateHeader);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C) && !Chat)
                GUIUtility.systemCopyBuffer = GameCode.IntToGameName(AmongUsClient.Instance.GameId);

            if (DiscordManager.InstanceExists)
            {
                __instance.ShareOnDiscordButton.gameObject.SetActive(AmongUsClient.Instance.AmHost && IsOnlineGame && DiscordManager.Instance.CanShareGameOnDiscord() &&
                    DiscordManager.Instance.HasValidPartyID());
            }

            if (GameData.Instance.PlayerCount != __instance.LastPlayerCount)
            {
                __instance.LastPlayerCount = GameData.Instance.PlayerCount;
                var str = "<color=#FF0000FF>";

                if (__instance.LastPlayerCount > __instance.MinPlayers)
                    str = "<color=#00FF00FF>";
                else if (__instance.LastPlayerCount == __instance.MinPlayers)
                    str = "<color=#FFFF00FF>";

                __instance.PlayerCounter.text = $"{str}{__instance.LastPlayerCount}/{CustomGameOptions.LobbySize}</color>";
                __instance.PlayerCounter.enableWordWrapping = false;
                __instance.StartButton.color = __instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear;
                __instance.startLabelText.color = __instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear;
                __instance.StartButtonGlyph?.SetColor(__instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear);

                if (DiscordManager.InstanceExists)
                {
                    if (AmongUsClient.Instance.AmHost)
                        DiscordManager.Instance.SetInLobbyHost(__instance.LastPlayerCount, CustomGameOptions.LobbySize, AmongUsClient.Instance.GameId);
                    else
                        DiscordManager.Instance.SetInLobbyClient(__instance.LastPlayerCount, CustomGameOptions.LobbySize, AmongUsClient.Instance.GameId);
                }
            }

            // Check version handshake infos
            var versionMismatch = false;
            var message = "";

            // Send version as soon as local player exists
            if (CustomPlayer.Local && !VersionSent)
            {
                VersionSent = true;
                ShareGameVersion();
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
                            LogWarning($"Guid - {pv.GuidMatches} Dev - {pv.DevMatches} Stream - {pv.StreamMatches} Dev Build - {pv.DevBuildMatches} Version String - {pv.VersionString} " +
                                $"Version - {pv.Version}");
                        }
                    }
                }

                // Display message to the host
                if (AmongUsClient.Instance.AmHost)
                {
                    if (versionMismatch)
                    {
                        __instance.StartButton.color = __instance.startLabelText.color = Palette.DisabledClear;
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                    }
                    else if (!IsCountDown)
                        __instance.StartButton.color = __instance.startLabelText.color = Palette.EnabledColor;
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

                if (num != Seconds)
                    CustomPlayer.Local.RpcSetStartCounter(Seconds);

                if (Seconds <= 0)
                {
                    __instance.FinallyBegin();

                    // Clear player versions right before the game starts
                    PlayerVersions.Clear();
                }
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
                    if (client.Character == null)
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

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ToggleButtonGlyphs))]
    public static class GlyphNullRefFix
    {
        public static bool Prefix(GameStartManager __instance, ref bool enabled)
        {
            __instance?.MakePublicButtonGlyph?.SetActive(enabled);
            __instance?.StartButtonGlyphContainer?.SetActive(enabled);
            return false;
        }
    }
}