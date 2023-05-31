namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class RandomMap
    {
        private static byte previousMap;
        private static float vision;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        public static bool Prefix()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                RoleGen.ResetEverything();
                previousMap = TownOfUsReworked.VanillaOptions.MapId;
                vision = CustomGameOptions.CrewVision;
                byte[] maps = { 0, 1, 2, 4, 5, 6 };
                var map = maps[(int)CustomGameOptions.Map];
                byte[] tbMode = { 1, 0, 2 };

                if (CustomGameOptions.RandomMapEnabled || (map == 5 && !ModCompatibility.SubLoaded) || (map == 6 && !ModCompatibility.LILoaded))
                    map = GetRandomMap();

                TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                TownOfUsReworked.VanillaOptions.CrewLightMod = CustomGameOptions.CrewVision;
                TownOfUsReworked.VanillaOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                TownOfUsReworked.VanillaOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                TownOfUsReworked.VanillaOptions.VisualTasks = CustomGameOptions.VisualTasks;
                TownOfUsReworked.VanillaOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                TownOfUsReworked.VanillaOptions.NumImpostors = CustomGameOptions.IntruderCount;
                //TownOfUsReworked.VanillaOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                TownOfUsReworked.VanillaOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)tbMode[(int)CustomGameOptions.TaskBarMode];
                TownOfUsReworked.VanillaOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                TownOfUsReworked.VanillaOptions.VotingTime = CustomGameOptions.VotingTime;
                TownOfUsReworked.VanillaOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                TownOfUsReworked.VanillaOptions.KillDistance = CustomGameOptions.InteractionDistance;
                TownOfUsReworked.VanillaOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                TownOfUsReworked.VanillaOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                TownOfUsReworked.VanillaOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                TownOfUsReworked.VanillaOptions.NumShortTasks = CustomGameOptions.ShortTasks;
                TownOfUsReworked.VanillaOptions.NumLongTasks = CustomGameOptions.LongTasks;
                TownOfUsReworked.VanillaOptions.NumCommonTasks = CustomGameOptions.ShortTasks;
                TownOfUsReworked.VanillaOptions.MapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 6 ? (byte)6 : map;
                GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.VanillaOptions;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSettings, SendOption.Reliable);
                writer.Write(map);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.AutoAdjustSettings)
                    AdjustSettings(map);
            }

            return true;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        public static void Postfix(AmongUsClient __instance)
        {
            if (__instance.AmHost)
            {
                if (CustomGameOptions.AutoAdjustSettings)
                {
                    if (CustomGameOptions.SmallMapHalfVision)
                        TownOfUsReworked.VanillaOptions.CrewLightMod = vision;

                    if (TownOfUsReworked.VanillaOptions.MapId == 1)
                        AdjustCooldowns(CustomGameOptions.SmallMapDecreasedCooldown);

                    if (TownOfUsReworked.VanillaOptions.MapId >= 4)
                        AdjustCooldowns(-CustomGameOptions.LargeMapIncreasedCooldown);
                }

                if (CustomGameOptions.RandomMapEnabled)
                    TownOfUsReworked.VanillaOptions.MapId = previousMap;
            }
        }

        public static byte GetRandomMap()
        {
            var _rnd = new SRandom();
            var totalWeight = 0f;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;

            if (ModCompatibility.SubLoaded)
                totalWeight += CustomGameOptions.RandomMapSubmerged;

            /*if (ModCompatibility.LILoaded)
                totalWeight += CustomGameOptions.RandomMapSubmerged;*/

            if (totalWeight == 0)
                return (byte)((int)CustomGameOptions.Map == 3 ? 4 : ((int)CustomGameOptions.Map == 4 ? 5 : (int)CustomGameOptions.Map));

            float randomNumber = _rnd.Next(0, (int)totalWeight);

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

            if (ModCompatibility.SubLoaded && randomNumber < CustomGameOptions.RandomMapSubmerged)
                return 5;

            return TownOfUsReworked.VanillaOptions.MapId;
        }

        public static void AdjustSettings(byte map)
        {
            if (map <= 1)
            {
                if (CustomGameOptions.SmallMapHalfVision)
                    TownOfUsReworked.VanillaOptions.CrewLightMod *= 0.5f;

                TownOfUsReworked.VanillaOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
                TownOfUsReworked.VanillaOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            }

            if (map == 1)
                AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);

            if (map >= 4)
            {
                TownOfUsReworked.VanillaOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
                TownOfUsReworked.VanillaOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
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
}