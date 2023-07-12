namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class GameStartManagerPatch
    {
        //The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/GameStartManagerPatch.cs under GPL v3 with some
        //Modifications from me to account for MCI and the horrid vanilla spam in the logs
        public readonly static Dictionary<int, PlayerVersion> PlayerVersions = new();
        private static float KickingTimer;
        private static bool VersionSent;

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public static class AmongUsClientOnPlayerJoinedPatch
        {
            public static void Postfix()
            {
                if (CustomPlayer.Local && !TownOfUsReworked.MCIActive)
                    Utils.ShareGameVersion();
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public static class GameStartManagerStartPatch
        {
            public static void Postfix(GameStartManager __instance)
            {
                if (!__instance)
                    return;

                //Trigger version refresh
                VersionSent = false;
                //Reset kicking timer
                KickingTimer = 0f;
                //Copy lobby code
                GUIUtility.systemCopyBuffer = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
                //Lobby size
                __instance.MinPlayers = 1;
                Generate.LobbySize.Set((float)TownOfUsReworked.VanillaOptions.MaxPlayers);
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
                if (!__instance || !AmongUsClient.Instance || !GameData.Instance || !GameManager.Instance || ConstantVariables.IsInGame)
                    return;

                __instance.MakePublicButton.sprite = AmongUsClient.Instance.IsGamePublic ? __instance.PublicGameImage : __instance.PrivateGameImage;
                __instance.privatePublicText.text = TranslationController.Instance.GetString(AmongUsClient.Instance.IsGamePublic ? StringNames.PublicHeader : StringNames.PrivateHeader);

                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C) && !Utils.HUD.Chat)
                    GUIUtility.systemCopyBuffer = GameCode.IntToGameName(AmongUsClient.Instance.GameId);

                if (DiscordManager.InstanceExists)
                {
                    __instance.ShareOnDiscordButton.gameObject.SetActive(AmongUsClient.Instance.AmHost && ConstantVariables.IsOnlineGame &&
                        DiscordManager.Instance.CanShareGameOnDiscord() && DiscordManager.Instance.HasValidPartyID());
                }

                if (GameData.Instance.PlayerCount != __instance.LastPlayerCount)
                {
                    __instance.LastPlayerCount = GameData.Instance.PlayerCount;
                    var str = "<color=#FF0000FF>";

                    if (__instance.LastPlayerCount > __instance.MinPlayers)
                        str = "<color=#00FF00FF>";
                    else if (__instance.LastPlayerCount == __instance.MinPlayers)
                        str = "<color=#FFFF00FF>";

                    __instance.PlayerCounter.text = $"{str}{__instance.LastPlayerCount}/{CustomGameOptions.LobbySize}";
                    __instance.StartButton.color = __instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear;
                    __instance.startLabelText.color = __instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear;
                    __instance.StartButtonGlyph?.SetColor(__instance.LastPlayerCount >= __instance.MinPlayers ? Palette.EnabledColor : Palette.DisabledClear);

                    if (DiscordManager.InstanceExists)
                    {
                        if (AmongUsClient.Instance.AmHost && ConstantVariables.IsOnlineGame)
                            DiscordManager.Instance.SetInLobbyHost(__instance.LastPlayerCount, CustomGameOptions.LobbySize, AmongUsClient.Instance.GameId);
                        else
                            DiscordManager.Instance.SetInLobbyClient(__instance.LastPlayerCount, CustomGameOptions.LobbySize, AmongUsClient.Instance.GameId);
                    }
                }

                if (__instance.LastPlayerCount != TownOfUsReworked.VanillaOptions.MaxPlayers)
                    __instance.LastPlayerCount = TownOfUsReworked.VanillaOptions.MaxPlayers;

                //Check version handshake infos
                var versionMismatch = false;
                var message = "";

                if (!TownOfUsReworked.MCIActive)
                {
                    //Send version as soon as CustomPlayer.Local exists
                    if (CustomPlayer.Local && !VersionSent)
                    {
                        VersionSent = true;
                        Utils.ShareGameVersion();
                    }

                    foreach (var client in AmongUsClient.Instance.allClients)
                    {
                        if (client.Character == null)
                            continue;

                        if (!PlayerVersions.ContainsKey(client.Id))
                        {
                            versionMismatch = true;
                            message += $"{client.Character.Data.PlayerName} has a different or no version of Town Of Us Reworked\n";
                        }
                        else
                        {
                            var pv = PlayerVersions[client.Id];
                            var diff = TownOfUsReworked.Version.CompareTo(pv.Version);

                            if (diff > 0)
                            {
                                versionMismatch = true;
                                message += $"{client.Character.Data.PlayerName} has an older version of Town Of Us Reworked (v{PlayerVersions[client.Id].Version})\n";
                            }
                            else if (diff < 0)
                            {
                                versionMismatch = true;
                                message += $"{client.Character.Data.PlayerName} has a newer version of Town Of Us Reworked (v{PlayerVersions[client.Id].Version})\n";
                            }
                            else if (!pv.GuidMatches)
                            {
                                //Version presumably matches, check if Guid matches
                                versionMismatch = true;
                                message += $"{client.Character.Data.PlayerName} has a modified version of Town Of Us Reworked v{PlayerVersions[client.Id].Version} ({pv.Guid})\n";
                            }
                        }
                    }

                    //Display message to the host
                    if (AmongUsClient.Instance.AmHost)
                    {
                        if (versionMismatch)
                        {
                            __instance.StartButton.color = __instance.startLabelText.color = Palette.DisabledClear;
                            __instance.GameStartText.text = message;
                            __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                        }
                    }
                    //Client update with handshake infos
                    else if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || TownOfUsReworked.Version.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId].Version) != 0)
                    {
                        KickingTimer += Time.deltaTime;

                        if (KickingTimer > 10)
                        {
                            KickingTimer = 0;
                            AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
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
                    __instance.GameStartText.text = ConstantVariables.IsCountDown ? TranslationController.Instance.GetString(StringNames.GameStarting, Seconds) : "";

                if (!AmongUsClient.Instance.AmHost)
                    return;

                if (ConstantVariables.IsCountDown)
                {
                    var num = Mathf.CeilToInt(__instance.countDownTimer);
                    __instance.countDownTimer -= Time.deltaTime;
                    var secondsLeft = Mathf.CeilToInt(__instance.countDownTimer);
                    Seconds = secondsLeft;

                    if (Input.GetKeyDown(KeyCode.LeftShift))
                        __instance.countDownTimer = 0;

                    if (Input.GetKeyDown(KeyCode.LeftControl))
                        __instance.ResetStartState();

                    if (num != secondsLeft)
                        CustomPlayer.Local.RpcSetStartCounter(secondsLeft);

                    if (secondsLeft <= 0)
                        __instance.FinallyBegin();
                }
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        public static class GameStartManagerBeginGame
        {
            public static bool Prefix()
            {
                //Block game start if not everyone has the same mod version
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

                        if (!PlayerVersions.ContainsKey(client.Id))
                        {
                            continueStart = false;
                            break;
                        }

                        var PV = PlayerVersions[client.Id];
                        var diff = TownOfUsReworked.Version.CompareTo(PV.Version);

                        if (diff != 0 || !PV.GuidMatches)
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
}