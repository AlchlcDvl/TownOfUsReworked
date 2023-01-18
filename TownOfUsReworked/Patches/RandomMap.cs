using HarmonyLib;
using Hazel;
using System;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public class RandomMap
    {
        public static byte previousMap;
        public static float vision;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        [HarmonyPrefix]
        public static bool Prefix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                previousMap = PlayerControl.GameOptions.MapId;
                vision = CustomGameOptions.CrewVision;

                if (!(commonTasks == 0 && shortTasks == 0 && longTasks == 0))
                {
                    commonTasks = CustomGameOptions.CommonTasks;
                    shortTasks = CustomGameOptions.ShortTasks;
                    longTasks = CustomGameOptions.LongTasks;
                }

                byte map = PlayerControl.GameOptions.MapId;

                if (CustomGameOptions.RandomMapEnabled)
                {
                    map = GetRandomMap();
                    PlayerControl.GameOptions.MapId = map;
                }

                PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                PlayerControl.GameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                PlayerControl.GameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                PlayerControl.GameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                PlayerControl.GameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                PlayerControl.GameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                PlayerControl.GameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                PlayerControl.GameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                PlayerControl.GameOptions.TaskBarMode = (TaskBarMode)CustomGameOptions.TaskBarMode;
                PlayerControl.GameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                PlayerControl.GameOptions.VotingTime = CustomGameOptions.VotingTime;
                PlayerControl.GameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                PlayerControl.GameOptions.KillDistance = CustomGameOptions.InteractionDistance;
                PlayerControl.GameOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                PlayerControl.GameOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                PlayerControl.GameOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                PlayerControl.GameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                //PlayerControl.GameOptions.MaxPlayers = CustomGameOptions.LobbySize;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSettings, SendOption.Reliable, -1);
                writer.Write(map);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.AutoAdjustSettings)
                    AdjustSettings(map);
            }
            return true;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        [HarmonyPostfix]
        public static void Postfix(AmongUsClient __instance)
        {
            if (__instance.AmHost)
            {
                if (CustomGameOptions.AutoAdjustSettings)
                {
                    if (CustomGameOptions.SmallMapHalfVision)
                        PlayerControl.GameOptions.CrewLightMod = vision;

                    if (PlayerControl.GameOptions.MapId == 1)
                        AdjustCooldowns(CustomGameOptions.SmallMapDecreasedCooldown);

                    if (PlayerControl.GameOptions.MapId >= 4)
                        AdjustCooldowns(-CustomGameOptions.LargeMapIncreasedCooldown);
                }

                if (CustomGameOptions.RandomMapEnabled)
                    PlayerControl.GameOptions.MapId = previousMap;

                PlayerControl.GameOptions.NumCommonTasks = commonTasks;
                PlayerControl.GameOptions.NumShortTasks = shortTasks;
                PlayerControl.GameOptions.NumLongTasks = longTasks;
            }
        }

        public static byte GetRandomMap()
        {
            Random _rnd = new Random();
            float totalWeight = 0;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;

            if (SubmergedCompatibility.Loaded)
                totalWeight += CustomGameOptions.RandomMapSubmerged;

            if (totalWeight == 0)
                return (byte)PlayerControl.GameOptions.MapId;

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

            if (SubmergedCompatibility.Loaded && randomNumber < CustomGameOptions.RandomMapSubmerged)
                return 5;

            return PlayerControl.GameOptions.MapId;
        }

        public static void AdjustSettings(byte map)
        {
            if (map <= 1)
            {
                if (CustomGameOptions.SmallMapHalfVision)
                    PlayerControl.GameOptions.CrewLightMod *= 0.5f;

                PlayerControl.GameOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
                PlayerControl.GameOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            }

            if (map == 1)
                AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);

            if (map >= 4)
            {
                PlayerControl.GameOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
                PlayerControl.GameOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
                AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
            }

            return;
        }

        public static void AdjustCooldowns(float change)
        {
            Generate.InterrogateCooldown.Set((float)Generate.InterrogateCooldown.Value + change, false);
            Generate.TrackCooldown.Set((float)Generate.TrackCooldown.Value + change, false);
            Generate.BugCooldown.Set((float)Generate.BugCooldown.Value + change, false);
            Generate.VigiKillCd.Set((float)Generate.VigiKillCd.Value + change, false);
            Generate.AlertCooldown.Set((float)Generate.AlertCooldown.Value + change, false);
            Generate.RewindCooldown.Set((float)Generate.RewindCooldown.Value + change, false);
            Generate.TransportCooldown.Set((float)Generate.TransportCooldown.Value + change, false);
            Generate.ProtectCd.Set((float)Generate.ProtectCd.Value + change, false);
            Generate.VestCd.Set((float)Generate.VestCd.Value + change, false);
            Generate.DouseCooldown.Set((float)Generate.DouseCooldown.Value + change, false);
            Generate.InfectCooldown.Set((float)Generate.InfectCooldown.Value + change, false);
            Generate.PestKillCooldown.Set((float)Generate.PestKillCooldown.Value + change, false);
            Generate.HackCooldown.Set((float)Generate.HackCooldown.Value + change, false);
            Generate.MimicCooldown.Set((float)Generate.MimicCooldown.Value + change, false);
            Generate.GlitchKillCooldown.Set((float)Generate.GlitchKillCooldown.Value + change, false);
            Generate.JuggKillCooldown.Set((float)Generate.JuggKillCooldown.Value + change, false);
            Generate.BloodlustCooldown.Set((float)Generate.BloodlustCooldown.Value + change, false);
            Generate.GrenadeCooldown.Set((float)Generate.GrenadeCooldown.Value + change, false);
            Generate.MorphlingCooldown.Set((float)Generate.MorphlingCooldown.Value + change, false);
            Generate.InvisCooldown.Set((float)Generate.InvisCooldown.Value + change, false);
            Generate.PoisonCooldown.Set((float)Generate.PoisonCooldown.Value + change, false);
            Generate.MineCooldown.Set((float)Generate.MineCooldown.Value + change, false);
            Generate.DragCooldown.Set((float)Generate.DragCooldown.Value + change, false);
            Generate.JanitorCleanCd.Set((float)Generate.JanitorCleanCd.Value + change, false);
            Generate.DisguiseCooldown.Set((float)Generate.DisguiseCooldown.Value + change, false);
            Generate.GazeCooldown.Set((float)Generate.GazeCooldown.Value + change, false);
            Generate.IgniteCooldown.Set((float)Generate.IgniteCooldown.Value + change, false);
            Generate.RevealCooldown.Set((float)Generate.RevealCooldown.Value + change, false);
            Generate.IntruderKillCooldown.Set((float)Generate.IntruderKillCooldown.Value + change, false);

            if (change % 5 != 0)
            {
                if (change > 0)
                    change -= 2.5f;
                else if (change < 0)
                    change += 2.5f;
            }

            Generate.EmergencyButtonCooldown.Set((float)Generate.EmergencyButtonCooldown.Value + change, false);
            return;
        }
    }
}