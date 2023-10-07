namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
public static class MapPatches
{
    public static byte CurrentMap;

    public static void Prefix()
    {
        if (IsHnS)
            return;

        if (AmongUsClient.Instance.AmHost)
        {
            byte[] maps = { 0, 1, 2, 4, 5, 6, 7 };
            byte[] tbModes = { 1, 0, 2 };
            CurrentMap = maps[(int)CustomGameOptions.Map];
            var tbMode = tbModes[(int)CustomGameOptions.TaskBarMode];

            if (CustomGameOptions.Map == MapEnum.Random || (CurrentMap == 5 && !SubLoaded) || (CurrentMap == 6 && !LILoaded) || CurrentMap == 7)
                CurrentMap = GetRandomMap();

            TownOfUsReworked.NormalOptions.MapId = CurrentMap;
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
            TownOfUsReworked.NormalOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
            TownOfUsReworked.NormalOptions.CrewLightMod = CustomGameOptions.CrewVision;
            TownOfUsReworked.NormalOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
            TownOfUsReworked.NormalOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
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
            CustomOption.SaveSettings("LastUsedSettings");
            AdjustSettings(CurrentMap);
            CallRpc(CustomRPC.Misc, MiscRPC.SetSettings, CurrentMap, tbMode);
        }
    }

    private static byte GetRandomMap()
    {
        var random = new SRandom();
        var totalWeight = 0;
        totalWeight += CustomGameOptions.RandomMapSkeld;
        totalWeight += CustomGameOptions.RandomMapMira;
        totalWeight += CustomGameOptions.RandomMapPolus;
        totalWeight += CustomGameOptions.RandomMapAirship;
        totalWeight += CustomGameOptions.RandomMapSubmerged;
        totalWeight += CustomGameOptions.RandomMapLevelImpostor;
        var maps = new List<byte>() { 0, 1, 2, 4 };

        if (SubLoaded)
            maps.Add(5);

        if (LILoaded)
            maps.Add(6);

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

        if (randomNumber < CustomGameOptions.RandomMapAirship)
            return 4;

        randomNumber -= CustomGameOptions.RandomMapAirship;

        if (SubLoaded && randomNumber < CustomGameOptions.RandomMapSubmerged)
            return 5;

        randomNumber -= CustomGameOptions.RandomMapSubmerged;

        if (LILoaded && randomNumber < CustomGameOptions.RandomMapLevelImpostor)
            return 6;

        return maps.Random();
    }

    public static void AdjustSettings(byte map)
    {
        if (!CustomGameOptions.AutoAdjustSettings)
            return;

        if (map is 0 or 1 or 3)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);
        }

        if (map is 4 or 5)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
            AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
        }
    }

    private static void AdjustCooldowns(float change)
    {
        foreach (var option in CustomOption.AllOptions.Where(x => x.Name.Contains("Cooldown")))
        {
            if (option.Type == CustomOptionType.Number)
                option.Set((float)option.Value + change);
        }
    }
}