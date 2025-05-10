namespace TownOfUsReworked.Patches.Gameplay;

// For some reason something here was nulling, so I just overwrote the whole thing instead of just a few parts of it
[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
public static class MapPatches
{
    public static byte CurrentMap;

    public static bool Prefix(AmongUsClient __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (IsHnS())
            return true;

        __result = CoStartGameFix(__instance).WrapToIl2Cpp();
        return false;
    }

    private static IEnumerator CoStartGameFix(AmongUsClient __instance)
    {
        if (TownOfUsReworked.MciActive)
        {
            foreach (var client in __instance.allClients)
            {
                client.IsReady = true;
                client.Character.isDummy = false;
                client.Character.GetComponent<DummyBehaviour>().enabled = false;
            }
        }

        if (Lobby())
            Lobby().Despawn();

        if (!Ship())
        {
            CurrentMap = GetSelectedMap();

            try
            {
                TownOfUsReworked.NormalOptions.MapId = CurrentMap;
            }
            catch { }

            try
            {
                TownOfUsReworked.HnsOptions.MapId = CurrentMap;
            }
            catch { }

            SetDefaults();
            CallRpc(CustomRPC.Misc, MiscRPC.SetSettings, CurrentMap);
            AdjustSettings(true);
            // __instance.ShipLoadingAsyncHandle seems to be having an issue when setting its value; I wonder what's up with that
            var async = __instance.ShipPrefabs[CurrentMap].InstantiateAsync();
            yield return async;
            ShipStatus.Instance = async.Result.GetComponent<ShipStatus>();
            __instance.Spawn(Ship());
        }

        var timer = 0f;

        while (true)
        {
            var stopWaiting = true;
            var num2 = 10;

            if (CurrentMap is 5 or 4)
                num2 = 15;

            lock (__instance.allClients)
            {
                foreach (var clientData in __instance.allClients)
                {
                    if (clientData.Id == __instance.ClientId || clientData.IsReady)
                        continue;

                    if (timer < num2)
                        stopWaiting = false;
                    else
                    {
                        __instance.SendLateRejection(clientData.Id, DisconnectReasons.ClientTimeout);
                        clientData.IsReady = true;
                        __instance.OnPlayerLeft(clientData, DisconnectReasons.ClientTimeout);
                    }
                }
            }

            yield return null;

            if (stopWaiting)
                break;

            timer += Time.deltaTime;
        }

        RoleGenManager.BeginRoleGen();
        Ship().Begin();
        __instance.SendClientReady();
        __instance.StartCoroutine(HUD().CoShowIntro());
    }

    private static byte GetSelectedMap()
    {
        if (MapSettings.Map < MapEnum.Random)
            return (byte)MapSettings.Map;

        var randoms = new Dictionary<MapEnum, int>
        {
            { MapEnum.Skeld, MapSettings.RandomMapSkeld },
            { MapEnum.MiraHq, MapSettings.RandomMapMira },
            { MapEnum.Polus, MapSettings.RandomMapPolus },
            { MapEnum.dlekS, MapSettings.RandomMapdlekS },
            { MapEnum.Airship, MapSettings.RandomMapAirship },
            { MapEnum.Fungle, MapSettings.RandomMapFungle }
        };

        if (SubLoaded)
            randoms.Add(MapEnum.Submerged, MapSettings.RandomMapSubmerged);

        if (LiLoaded)
            randoms.Add(MapEnum.LevelImpostor, MapSettings.RandomMapLevelImpostor);

        var maxWeight = randoms.Values.Sum();

        if (maxWeight == 0)
            return (byte)randoms.Keys.Random();

        var random = URandom.RandomRangeInt(0, maxWeight);

        foreach (var (id, chance) in randoms)
        {
            if (random < chance)
                return (byte)id;

            random -= chance;
        }

        return 0;
    }

    public static void AdjustSettings(bool starting)
    {
        if (!MapSettings.AutoAdjustSettings || IsHnS())
            return;

        var direction = starting ? 1 : -1;

        if (CurrentMap is 0 or 1 or 3)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks += MapSettings.SmallMapIncreasedShortTasks * direction;
            TownOfUsReworked.NormalOptions.NumLongTasks += MapSettings.SmallMapIncreasedLongTasks * direction;
        }

        if (CurrentMap is not (4 or 5 or 6))
            return;

        TownOfUsReworked.NormalOptions.NumShortTasks -= MapSettings.LargeMapDecreasedShortTasks * direction;
        TownOfUsReworked.NormalOptions.NumLongTasks -= MapSettings.LargeMapDecreasedLongTasks * direction;
    }

    public static void SetDefaults()
    {
        if (IsHnS())
            return;

        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Noisemaker, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Phantom, 0, 0);
        TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Tracker, 0, 0);
        TownOfUsReworked.NormalOptions.CrewLightMod = CrewSettings.CrewVision;
        TownOfUsReworked.NormalOptions.ImpostorLightMod = IntruderSettings.IntruderVision;
        TownOfUsReworked.NormalOptions.AnonymousVotes = GameModifiers.AnonymousVoting != AnonVotes.Disabled;
        TownOfUsReworked.NormalOptions.VisualTasks = GameModifiers.VisualTasks;
        TownOfUsReworked.NormalOptions.PlayerSpeedMod = GameSettings.PlayerSpeed;
        TownOfUsReworked.NormalOptions.NumImpostors = IntruderSettings.IntruderCount;
        TownOfUsReworked.NormalOptions.TaskBarMode = GameSettings.TaskBarMode;
        TownOfUsReworked.NormalOptions.ConfirmImpostor = GameSettings.ConfirmEjects;
        TownOfUsReworked.NormalOptions.VotingTime = GameSettings.VotingTime;
        TownOfUsReworked.NormalOptions.DiscussionTime = GameSettings.DiscussionTime;
        TownOfUsReworked.NormalOptions.EmergencyCooldown = GameSettings.EmergencyButtonCooldown;
        TownOfUsReworked.NormalOptions.NumEmergencyMeetings = GameSettings.EmergencyButtonCount;
        TownOfUsReworked.NormalOptions.KillCooldown = IntruderSettings.IntKillCd;
        TownOfUsReworked.NormalOptions.GhostsDoTasks = TaskSettings.GhostTasksCountToWin;
        TownOfUsReworked.NormalOptions.MaxPlayers = GameSettings.LobbySize;
        TownOfUsReworked.NormalOptions.NumShortTasks = TaskSettings.ShortTasks;
        TownOfUsReworked.NormalOptions.NumLongTasks = TaskSettings.LongTasks;
        TownOfUsReworked.NormalOptions.NumCommonTasks = TaskSettings.CommonTasks;
    }
}