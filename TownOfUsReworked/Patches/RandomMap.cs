using HarmonyLib;
using Hazel;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using AmongUs.GameOptions;
using TownOfUsReworked.Classes;

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
                previousMap = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                vision = CustomGameOptions.CrewVision;

                var map = (byte)((int)CustomGameOptions.Map == 3 ? 4 : ((int)CustomGameOptions.Map == 4 ? 5 : (int)CustomGameOptions.Map));

                if (CustomGameOptions.RandomMapEnabled || (map == 5 && !SubmergedCompatibility.Loaded))
                    map = GetRandomMap();

                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                GameOptionsManager.Instance.currentNormalGameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                GameOptionsManager.Instance.currentNormalGameOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)CustomGameOptions.TaskBarMode;
                GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                GameOptionsManager.Instance.currentNormalGameOptions.VotingTime = CustomGameOptions.VotingTime;
                GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                GameOptionsManager.Instance.currentNormalGameOptions.KillDistance = CustomGameOptions.InteractionDistance;
                GameOptionsManager.Instance.currentNormalGameOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                GameOptionsManager.Instance.currentNormalGameOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = CustomGameOptions.ShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = CustomGameOptions.LongTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = CustomGameOptions.ShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = map;

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
                        GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = vision;

                    if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1)
                        AdjustCooldowns(CustomGameOptions.SmallMapDecreasedCooldown);

                    if (GameOptionsManager.Instance.currentNormalGameOptions.MapId >= 4)
                        AdjustCooldowns(-CustomGameOptions.LargeMapIncreasedCooldown);
                }

                if (CustomGameOptions.RandomMapEnabled)
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = previousMap;
            }
        }

        public static byte GetRandomMap()
        {
            Random _rnd = new();
            float totalWeight = 0;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;

            if (SubmergedCompatibility.Loaded)
                totalWeight += CustomGameOptions.RandomMapSubmerged;

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

            if (SubmergedCompatibility.Loaded && randomNumber < CustomGameOptions.RandomMapSubmerged)
                return 5;

            return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        }

        public static void AdjustSettings(byte map)
        {
            if (map <= 1)
            {
                if (CustomGameOptions.SmallMapHalfVision)
                    GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod *= 0.5f;

                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
            }

            if (map == 1)
                AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);

            if (map >= 4)
            {
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
                AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
            }
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
        }
    }
}