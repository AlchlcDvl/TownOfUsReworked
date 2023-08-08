namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class MapPatches
{
    private static byte PreviousMap;
    private static float Vision;

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public static void Prefix()
    {
        if (IsHnS)
            return;

        if (AmongUsClient.Instance.AmHost)
        {
            PreviousMap = TownOfUsReworked.NormalOptions.MapId;
            Vision = CustomGameOptions.CrewVision;
            byte[] maps = { 0, 1, 2, 4, 5, 6, 7 };
            byte[] tbModes = { 1, 0, 2 };
            var map = maps[(int)CustomGameOptions.Map];
            var tbMode = tbModes[(int)CustomGameOptions.TaskBarMode];

            if (CustomGameOptions.Map == MapEnum.Random || (map == 5 && !SubLoaded) || (map == 6 && !LILoaded) || map == 7)
                map = GetRandomMap();

            TownOfUsReworked.NormalOptions.MapId = map;
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
            TownOfUsReworked.NormalOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
            TownOfUsReworked.NormalOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
            TownOfUsReworked.NormalOptions.MaxPlayers = CustomGameOptions.LobbySize;
            GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.NormalOptions;
            PreviousMap = map;
            CallRpc(CustomRPC.Misc, MiscRPC.SetSettings, map, tbMode);

            if (CustomGameOptions.AutoAdjustSettings)
                AdjustSettings(map);
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static void Postfix(AmongUsClient __instance)
    {
        if (__instance.AmHost)
        {
            if (CustomGameOptions.AutoAdjustSettings)
            {
                if (CustomGameOptions.SmallMapHalfVision && CustomGameOptions.Map is MapEnum.Skeld or MapEnum.MiraHQ)
                    TownOfUsReworked.NormalOptions.CrewLightMod = Vision / 2;

                if (TownOfUsReworked.NormalOptions.MapId == 1)
                    AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);

                if (TownOfUsReworked.NormalOptions.MapId >= 4)
                    AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
            }

            if (CustomGameOptions.Map == MapEnum.Random)
                TownOfUsReworked.NormalOptions.MapId = PreviousMap;
        }
    }

    public static byte GetRandomMap()
    {
        var random = new SRandom();
        var totalWeight = 0f;
        totalWeight += CustomGameOptions.RandomMapSkeld;
        totalWeight += CustomGameOptions.RandomMapMira;
        totalWeight += CustomGameOptions.RandomMapPolus;
        totalWeight += CustomGameOptions.RandomMapAirship;

        if (SubLoaded)
            totalWeight += CustomGameOptions.RandomMapSubmerged;

        if (LILoaded)
            totalWeight += CustomGameOptions.RandomMapLevelImpostor;

        if (totalWeight == 0)
            return PreviousMap;

        float randomNumber = random.Next(0, (int)totalWeight);

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

        return PreviousMap;
    }

    public static void AdjustSettings(byte map)
    {
        if (map <= 1)
        {
            if (CustomGameOptions.SmallMapHalfVision)
                TownOfUsReworked.NormalOptions.CrewLightMod *= 0.5f;

            TownOfUsReworked.NormalOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
        }

        if (map == 1)
            AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);

        if (map >= 4)
        {
            TownOfUsReworked.NormalOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
            TownOfUsReworked.NormalOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
            AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
        }
    }

    public static void AdjustCooldowns(float change)
    {
        foreach (var option in CustomOption.AllOptions.Where(x => x.Name.Contains("Cooldown")))
        {
            if (option.Type == CustomOptionType.Number)
                option.Set((float)option.Value + change);
        }
    }
}