using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
public static class MapPatches
{
    public static byte CurrentMap;

    public static void Prefix()
    {
        if (IsHnS())
            return;

        if (AmongUsClient.Instance.AmHost)
        {
            CurrentMap = GetSelectedMap();
            TownOfUsReworked.NormalOptions.MapId = CurrentMap;
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
            TownOfUsReworked.NormalOptions.GhostsDoTasks = CrewSettings.GhostTasksCountToWin;
            TownOfUsReworked.NormalOptions.MaxPlayers = GameSettings.LobbySize;
            TownOfUsReworked.NormalOptions.NumShortTasks = TaskSettings.ShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks = TaskSettings.LongTasks;
            TownOfUsReworked.NormalOptions.NumCommonTasks = TaskSettings.CommonTasks;
            AllPlayers.ForEach(x => x.MaxReportDistance = GameSettings.ReportDistance);
            CallRpc(CustomRPC.Misc, MiscRPC.SetSettings, CurrentMap);
            AdjustSettings();
        }
    }

    private static byte GetSelectedMap()
    {
        var map = (byte)MapSettings.Map;

        if (map < 8)
            return map;

        var totalWeight = 0;
        totalWeight += MapSettings.RandomMapSkeld;
        totalWeight += MapSettings.RandomMapMira;
        totalWeight += MapSettings.RandomMapPolus;
        totalWeight += MapSettings.RandomMapdlekS;
        totalWeight += MapSettings.RandomMapAirship;
        totalWeight += MapSettings.RandomMapFungle;
        totalWeight += MapSettings.RandomMapSubmerged;
        totalWeight += MapSettings.RandomMapLevelImpostor;
        var maps = new List<byte>() { 0, 1, 2, 3, 4, 5 };

        if (SubLoaded)
            maps.Add(6);

        if (LILoaded)
            maps.Add(7);

        maps.Shuffle();

        if (totalWeight == 0)
            return maps.Random();

        var randoms = new List<byte>();
        var num = MapSettings.RandomMapSkeld / 5;

        while (num > 0)
        {
            randoms.Add(0);
            num--;
        }

        num = MapSettings.RandomMapMira / 5;

        while (num > 0)
        {
            randoms.Add(1);
            num--;
        }

        num = MapSettings.RandomMapPolus / 5;

        while (num > 0)
        {
            randoms.Add(2);
            num--;
        }

        num = MapSettings.RandomMapdlekS / 5;

        while (num > 0)
        {
            randoms.Add(3);
            num--;
        }

        num = MapSettings.RandomMapAirship / 5;

        while (num > 0)
        {
            randoms.Add(4);
            num--;
        }

        num = MapSettings.RandomMapFungle / 5;

        while (num > 0)
        {
            randoms.Add(5);
            num--;
        }

        if (SubLoaded)
        {
            num = MapSettings.RandomMapSubmerged / 5;

            while (num > 0)
            {
                randoms.Add(6);
                num--;
            }
        }

        if (LILoaded)
        {
            num = MapSettings.RandomMapLevelImpostor / 5;

            while (num > 0)
            {
                randoms.Add(7);
                num--;
            }
        }

        randoms.Shuffle();
        return (randoms.Any() ? randoms : maps).Random();
    }

    public static void AdjustSettings()
    {
        if (!MapSettings.AutoAdjustSettings)
            return;

        if (CurrentMap is 0 or 1 or 3)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks += MapSettings.SmallMapIncreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks += MapSettings.SmallMapIncreasedLongTasks;
            AdjustCooldowns(-MapSettings.SmallMapDecreasedCooldown);
        }

        if (CurrentMap is 4 or 5 or 6)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks -= MapSettings.LargeMapDecreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks -= MapSettings.LargeMapDecreasedLongTasks;
            AdjustCooldowns(MapSettings.LargeMapIncreasedCooldown);
        }
    }

    private static void AdjustCooldowns(float change)
    {
        foreach (var option in OptionAttribute.AllOptions.Where(x => x.Name.Contains("Cooldown") && !x.Name.Contains("Increase") && !x.Name.Contains("Decrease")))
        {
            if (option.Type == CustomOptionType.Number)
                option.Set((float)option.Value + change);
        }
    }
}

// For some reason something here was nulling, so I just overrided the whole thing instead of just a few parts of it
[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
public static class AmongUsClientCoStartHostPatch2
{
    public static bool Prefix(ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = CoStartGameFix().WrapToIl2Cpp();
        return false;
    }

    private static IEnumerator CoStartGameFix()
    {
        if (Lobby)
            Lobby.Despawn();

        if (!Ship)
        {
            AmongUsClient.Instance.ShipLoadingAsyncHandle = AmongUsClient.Instance.ShipPrefabs[MapPatches.CurrentMap].InstantiateAsync();
            yield return AmongUsClient.Instance.ShipLoadingAsyncHandle;
            var result = AmongUsClient.Instance.ShipLoadingAsyncHandle.Result;
            AmongUsClient.Instance.ShipLoadingAsyncHandle = default;
            ShipStatus.Instance = result.GetComponent<ShipStatus>();
            AmongUsClient.Instance.Spawn(Ship);
        }

        var timer = 0f;

        while (true)
        {
            var stopWaiting = true;
            var num2 = 10;

            if (MapPatches.CurrentMap is 5 or 4)
                num2 = 15;

            lock (AmongUsClient.Instance.allClients)
            {
                for (var i = 0; i < AmongUsClient.Instance.allClients.Count; i++)
                {
                    var clientData = AmongUsClient.Instance.allClients[i];

                    if (clientData.Id != AmongUsClient.Instance.ClientId && !clientData.IsReady)
                    {
                        if (timer < num2)
                            stopWaiting = false;
                        else
                        {
                            AmongUsClient.Instance.SendLateRejection(clientData.Id, DisconnectReasons.ClientTimeout);
                            clientData.IsReady = true;
                            AmongUsClient.Instance.OnPlayerLeft(clientData, DisconnectReasons.ClientTimeout);
                        }
                    }
                }
            }

            yield return EndFrame();

            if (stopWaiting)
                break;

            timer += Time.deltaTime;
        }

        DestroyableSingleton<RoleManager>.Instance.SelectRoles();
        ShipStatus.Instance.Begin();
        AmongUsClient.Instance.SendClientReady();
    }
}