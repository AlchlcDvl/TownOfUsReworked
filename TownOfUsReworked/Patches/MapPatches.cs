namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
public static class MapPatches
{
    public static byte CurrentMap;
    private static readonly byte[] TBModes = { 1, 0, 2 };

    public static void Prefix()
    {
        if (IsHnS)
            return;

        if (AmongUsClient.Instance.AmHost)
        {
            var tbMode = TBModes[(int)CustomGameOptions.TaskBarMode];
            CurrentMap = GetSelectedMap();
            TownOfUsReworked.NormalOptions.MapId = CurrentMap;
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
            TownOfUsReworked.NormalOptions.CrewLightMod = CustomGameOptions.CrewVision;
            TownOfUsReworked.NormalOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
            TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting != AnonVotes.Disabled;
            TownOfUsReworked.NormalOptions.VisualTasks = CustomGameOptions.VisualTasks;
            TownOfUsReworked.NormalOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
            TownOfUsReworked.NormalOptions.NumImpostors = CustomGameOptions.IntruderCount;
            TownOfUsReworked.NormalOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)tbMode;
            TownOfUsReworked.NormalOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
            TownOfUsReworked.NormalOptions.VotingTime = CustomGameOptions.VotingTime;
            TownOfUsReworked.NormalOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
            TownOfUsReworked.NormalOptions.KillDistance = CustomGameOptions.InteractionDistance;
            TownOfUsReworked.NormalOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
            TownOfUsReworked.NormalOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
            TownOfUsReworked.NormalOptions.KillCooldown = CustomGameOptions.IntKillCd;
            TownOfUsReworked.NormalOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
            TownOfUsReworked.NormalOptions.MaxPlayers = CustomGameOptions.LobbySize;
            TownOfUsReworked.NormalOptions.NumShortTasks = CustomGameOptions.ShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks = CustomGameOptions.LongTasks;
            TownOfUsReworked.NormalOptions.NumCommonTasks = CustomGameOptions.CommonTasks;
            GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.NormalOptions;
            CustomPlayer.AllPlayers.ForEach(x => x.MaxReportDistance = CustomGameOptions.ReportDistance);
            CallRpc(CustomRPC.Misc, MiscRPC.SetSettings, CurrentMap, tbMode);
            AdjustSettings();
        }
    }

    private static byte GetSelectedMap()
    {
        var map = (byte)CustomGameOptions.Map;

        if (!((CurrentMap == 6 && !SubLoaded) || (CurrentMap == 7 && !LILoaded) || CurrentMap == 8))
            return map;

        var random = new SRandom();
        var totalWeight = 0;
        totalWeight += CustomGameOptions.RandomMapSkeld;
        totalWeight += CustomGameOptions.RandomMapMira;
        totalWeight += CustomGameOptions.RandomMapPolus;
        totalWeight += CustomGameOptions.RandomMapdlekS;
        totalWeight += CustomGameOptions.RandomMapAirship;
        totalWeight += CustomGameOptions.RandomMapFungle;
        totalWeight += CustomGameOptions.RandomMapSubmerged;
        totalWeight += CustomGameOptions.RandomMapLevelImpostor;
        var maps = new List<byte>() { 0, 1, 2, 3, 4, 5 };

        if (SubLoaded)
            maps.Add(6);

        if (LILoaded)
            maps.Add(7);

        if (totalWeight == 0)
            return maps.Random();

        var randomNumber = random.Next(0, totalWeight);

        if (randomNumber < CustomGameOptions.RandomMapSkeld)
            return 0;

        randomNumber -= CustomGameOptions.RandomMapSkeld;

        if (randomNumber < CustomGameOptions.RandomMapMira)
            return 1;

        randomNumber -= CustomGameOptions.RandomMapMira;

        if (randomNumber < CustomGameOptions.RandomMapPolus)
            return 2;

        randomNumber -= CustomGameOptions.RandomMapPolus;

        if (randomNumber < CustomGameOptions.RandomMapdlekS)
            return 3;

        randomNumber -= CustomGameOptions.RandomMapdlekS;

        if (randomNumber < CustomGameOptions.RandomMapAirship)
            return 4;

        randomNumber -= CustomGameOptions.RandomMapAirship;

        if (randomNumber < CustomGameOptions.RandomMapFungle)
            return 5;

        randomNumber -= CustomGameOptions.RandomMapFungle;

        if (SubLoaded && randomNumber < CustomGameOptions.RandomMapSubmerged)
            return 6;

        randomNumber -= CustomGameOptions.RandomMapSubmerged;

        if (LILoaded && randomNumber < CustomGameOptions.RandomMapLevelImpostor)
            return 7;

        return maps.Random();
    }

    public static void AdjustSettings()
    {
        if (!CustomGameOptions.AutoAdjustSettings)
            return;

        if (CurrentMap is 0 or 1 or 3)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);
        }

        if (CurrentMap is 4 or 5 or 6)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
            AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
        }
    }

    private static void AdjustCooldowns(float change)
    {
        foreach (var option in CustomOption.AllOptions.Where(x => x.Name.Contains("Cooldown") && !x.Name.Contains("Increase") && !x.Name.Contains("Decrease")))
        {
            if (option.Type == CustomOptionType.Number)
                option.Set((float)option.Value + change);
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient._CoStartGameHost_d__30), nameof(AmongUsClient._CoStartGameHost_d__30.MoveNext))]
public static class AmongUsClientCoStartHostPatch
{
    public static bool Prefix(AmongUsClient._CoStartGameHost_d__30 __instance, ref bool __result)
    {
        if (__instance.__1__state != 0)
            return true;

        __instance.__1__state = -1;

        if (Lobby)
            Lobby.Despawn();

        if (Ship)
        {
            __instance.__2__current = null;
            __instance.__1__state = 2;
            __result = true;
            return false;
        }

        __instance.__2__current = __instance.__4__this.ShipLoadingAsyncHandle = __instance.__4__this.ShipPrefabs[MapPatches.CurrentMap].InstantiateAsync();
        __instance.__1__state = 1;
        __result = true;
        return false;
    }
}