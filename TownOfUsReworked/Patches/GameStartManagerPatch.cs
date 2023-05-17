namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class GameStartManagerPatch
    {
        //The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Patches/GameStartManagerPatch.cs under GPL v3 with some
        //modifications from me to account for MCI
        public readonly static Dictionary<int, PlayerVersion> PlayerVersions = new();
        private static float kickingTimer;
        private static bool versionSent;

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public static class AmongUsClientOnPlayerJoinedPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.LocalPlayer && !TownOfUsReworked.MCIActive)
                    Utils.ShareGameVersion();
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public static class GameStartManagerStartPatch
        {
            public static void Postfix(GameStartManager __instance)
            {
                if (__instance == null)
                    return;

                //Trigger version refresh
                versionSent = false;
                //Reset kicking timer
                kickingTimer = 0f;
                //Copy lobby code
                GUIUtility.systemCopyBuffer = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
                //lobby size
                Generate.LobbySize.Set((float)TownOfUsReworked.VanillaOptions.MaxPlayers);
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static class GameStartManagerUpdatePatch
        {
            private static float startingTimer;

            public static void Prefix(GameStartManager __instance)
            {
                if (ConstantVariables.NoLobby || ConstantVariables.IsInGame)
                    return;

                if (GameData.Instance && __instance)
                    __instance.MinPlayers = 1;
            }

            public static void Postfix(GameStartManager __instance)
            {
                if (!__instance || !GameData.Instance || ConstantVariables.NoLobby || ConstantVariables.IsInGame)
                    return;

                if (__instance.LastPlayerCount != TownOfUsReworked.VanillaOptions.MaxPlayers)
                    __instance.LastPlayerCount = TownOfUsReworked.VanillaOptions.MaxPlayers;

                if (!TownOfUsReworked.MCIActive)
                {
                    //Send version as soon as PlayerControl.LocalPlayer exists
                    if (PlayerControl.LocalPlayer != null && !versionSent)
                    {
                        versionSent = true;
                        Utils.ShareGameVersion();
                    }

                    //Check version handshake infos
                    var versionMismatch = false;
                    var message = "";

                    foreach (var client in AmongUsClient.Instance.allClients.ToArray())
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
                            var PV = PlayerVersions[client.Id];
                            var diff = TownOfUsReworked.Version.CompareTo(PV.Version);

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
                            else if (!PV.GuidMatches)
                            {
                                //Version presumably matches, check if Guid matches
                                versionMismatch = true;
                                message += $"{client.Character.Data.PlayerName} has a modified version of Town Of Us Reworked v{PlayerVersions[client.Id].Version} ({PV.Guid})\n";
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
                        else
                        {
                            __instance.StartButton.color = __instance.startLabelText.color = (__instance.LastPlayerCount >= __instance.MinPlayers) ? Palette.EnabledColor :
                                Palette.DisabledClear;
                            __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                        }
                    }
                    //Client update with handshake infos
                    else if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || TownOfUsReworked.Version.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId].Version) != 0)
                    {
                        kickingTimer += Time.deltaTime;

                        if (kickingTimer > 10)
                        {
                            kickingTimer = 0;
                            AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                            SceneChanger.ChangeScene("MainMenu");
                        }

                        __instance.GameStartText.text = $"You or the host has no or a different version of Town Of Us Reworked\nYou will be kicked in {Math.Round(10 - kickingTimer)}s.";
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                    }
                    else if (versionMismatch)
                    {
                        __instance.GameStartText.text = "Players With Different Versions:\n" + message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;

                        if (__instance.startState != GameStartManager.StartingStates.Countdown && startingTimer <= 0)
                            __instance.GameStartText.text = string.Empty;
                        else
                        {
                            __instance.GameStartText.text = $"Starting in {(int)startingTimer + 1}";

                            if (startingTimer <= 0)
                                __instance.GameStartText.text = string.Empty;
                        }
                    }
                }

                //Start Timer
                if (startingTimer > 0)
                    startingTimer -= Time.deltaTime;
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
                    foreach (var client in AmongUsClient.Instance.allClients.GetFastEnumerator())
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