using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Objects;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Role : PlayerLayer
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static List<GameObject> Buttons = new List<GameObject>();
        public static readonly Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();
        public readonly List<Footprint> AllPrints = new List<Footprint>();

        public virtual void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance) {}

        public static bool NobodyWins;

        public static bool UndeadWin;
        public static bool CabalWin;
        public static bool ReanimatedWin;
        public static bool SectWin;
        public static bool InfectorsWin;

        public static bool NKWins;

        public static bool CrewWin;
        public static bool IntruderWin;
        public static bool SyndicateWin;
        public static bool AllNeutralsWin;

        public static bool GlitchWins;
        public static bool JuggernautWins;
        public static bool SerialKillerWins;
        public static bool ArsonistWins;
        public static bool CryomaniacWins;
        public static bool MurdererWins;
        public static bool WerewolfWins;

        public static int ChaosDriveMeetingTimerCount;
        public static bool SyndicateHasChaosDrive;

        protected internal Color32 FactionColor = Colors.Faction;
        protected internal Color32 SubFactionColor = Colors.SubFaction;
        protected internal RoleEnum RoleType = RoleEnum.None;
        protected internal Faction Faction = Faction.None;
        protected internal Faction BaseFaction = Faction.None;
        protected internal RoleAlignment RoleAlignment = RoleAlignment.None;
        protected internal SubFaction SubFaction = SubFaction.None;
        protected internal InspectorResults InspectorResults = InspectorResults.None;
        protected internal List<Role> RoleHistory = new List<Role>();

        protected internal string IntroSound => $"{Name}Intro";
        protected internal bool IntroPlayed = false;

        protected internal string StartText = "Woah The Game Started";
        protected internal string AbilitiesText = "- None.";

        protected internal string AlignmentName = "None";
        protected internal string FactionName => $"{Faction}";
        protected internal string SubFactionName => $"{SubFaction}";

        protected internal string Objectives = "- None.";

        protected internal bool RoleBlockImmune = false;
        protected internal bool IsBlocked = false;

        protected internal bool Base = false;

        protected internal AbilityButton SpectateButton;

        protected internal bool IsRecruit = false;
        protected internal bool IsResurrected = false;
        protected internal bool IsPersuaded = false;
        protected internal bool IsBitten = false;
        protected internal bool IsIntTraitor = false;
        protected internal bool IsIntAlly = false;
        protected internal bool IsIntFanatic = false;
        protected internal bool IsSynTraitor = false;
        protected internal bool IsSynAlly = false;
        protected internal bool IsSynFanatic = false;
        protected internal bool IsCrewAlly = false;
        protected internal bool NotDefective => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && !IsIntAlly && !IsIntFanatic && !IsIntTraitor && !IsSynAlly && !IsSynTraitor &&
            !IsSynFanatic && !IsCrewAlly;

        protected internal bool Winner = false;

        public static string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds.\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>.";
        public static string IS = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>";
        public static string IC = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>";
        public static string ID = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>";
        public static string IK = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string IU = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>";

        public static string SyndicateWinCon = (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds.\n" : "") + "- Cause chaos and kill anyone who opposes " +
            "the <color=#008000FF>Syndicate</color>.";
        public static string SU = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Utility</color>)</color>";
        public static string SSu = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Support</color>)</color>";
        public static string SD = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Disruption</color>)</color>";
        public static string SyK = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string SP = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Power</color>)</color>";

        public static string CrewWinCon = "- Finish all tasks.\n- Eject all <color=#FF0000FF>evildoers</color>.";
        public static string CP = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Protective</color>)</color>";
        public static string CI = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Investigative</color>)</color>";
        public static string CU = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Utility</color>)</color>";
        public static string CS = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Support</color>)</color>";
        public static string CA = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Auditor</color>)</color>";
        public static string CK = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string CSv = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Sovereign</color>)</color>";

        public static string NB = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Benign</color>)</color>";
        public static string NE = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Evil</color>)</color>";
        public static string NK = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string NN = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Neophyte</color>)</color>";
        public static string NP = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Proselyte</color>)</color>";

        protected Role(PlayerControl player) : base(player)
        {
            Player = player;

            if (RoleDictionary.ContainsKey(player.PlayerId))
                RoleDictionary.Remove(player.PlayerId);

            RoleDictionary.Add(player.PlayerId, this);
            Color = Colors.Role;
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();

        public string FactionColorString => "<color=#" + FactionColor.ToHtmlStringRGBA() + ">";
        public string SubFactionColorString => "<color=#" + SubFactionColor.ToHtmlStringRGBA() + ">";

        private bool Equals(Role other) => Equals(Player, other.Player) && RoleType == other.RoleType;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Role))
                return false;

            return Equals((Role)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)RoleType);

        public static void SetColors()
        {
            LightDarkColors.Clear();
            LightDarkColors.Add(0, "darker"); // Red
            LightDarkColors.Add(1, "darker"); // Blue
            LightDarkColors.Add(2, "darker"); // Green
            LightDarkColors.Add(3, "lighter"); // Pink
            LightDarkColors.Add(4, "lighter"); // Orange
            LightDarkColors.Add(5, "lighter"); // Yellow
            LightDarkColors.Add(6, "darker"); // Black
            LightDarkColors.Add(7, "lighter"); // White
            LightDarkColors.Add(8, "darker"); // Purple
            LightDarkColors.Add(9, "darker"); // Brown
            LightDarkColors.Add(10, "lighter"); // Cyan
            LightDarkColors.Add(11, "lighter"); // Lime
            LightDarkColors.Add(12, "darker"); // Maroon
            LightDarkColors.Add(13, "lighter"); // Rose
            LightDarkColors.Add(14, "lighter"); // Banana
            LightDarkColors.Add(15, "darker"); // Grey
            LightDarkColors.Add(16, "darker"); // Tan
            LightDarkColors.Add(17, "lighter"); // Coral
            LightDarkColors.Add(18, "darker"); // Watermelon
            LightDarkColors.Add(19, "darker"); // Chocolate
            LightDarkColors.Add(20, "lighter"); // Sky Blue
            LightDarkColors.Add(21, "lighter"); // Biege
            LightDarkColors.Add(22, "lighter"); // Hot Pink
            LightDarkColors.Add(23, "lighter"); // Turquoise
            LightDarkColors.Add(24, "lighter"); // Lilac
            LightDarkColors.Add(25, "darker"); // Olive
            LightDarkColors.Add(26, "lighter"); //Azure
            LightDarkColors.Add(27, "darker"); // Plum
            LightDarkColors.Add(28, "darker"); // Jungle
            LightDarkColors.Add(29, "lighter"); // Mint
            LightDarkColors.Add(30, "lighter"); // Chartreuse
            LightDarkColors.Add(31, "darker"); // Macau
            LightDarkColors.Add(32, "darker"); // Tawny
            LightDarkColors.Add(33, "lighter"); // Gold
            LightDarkColors.Add(34, "lighter"); // Panda
            LightDarkColors.Add(35, "darker"); // Contrast
            LightDarkColors.Add(36, "darker"); // Chroma
            LightDarkColors.Add(37, "darker"); // Mantle
            LightDarkColors.Add(38, "lighter"); // Fire
            LightDarkColors.Add(39, "lighter"); // Galaxy
            LightDarkColors.Add(40, "lighter"); // Monochrome
            LightDarkColors.Add(41, "lighter"); // Rainbow
        }

        public static T GenRole<T>(Type type, PlayerControl player, int id)
        {
            var role = (T)((object)Activator.CreateInstance(type, new object[] { player }));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRole, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return role;
        }

        public static Role GetRole(PlayerControl player)
        {
            if (player == null)
                return null;

            foreach (var role in AllRoles)
            {
                if (role.Player == player)
                    return role;
            }

            return null;
        }

        public static Role GetRoleValue(RoleEnum roleEnum)
        {
            foreach (var role in AllRoles)
            {
                if (role.RoleType == roleEnum)
                    return role;
            }

            return null;
        }

        public static Role GetRoleFromName(string name)
        {
            foreach (var role in AllRoles)
            {
                if (role.Name == name)
                    return role;
            }

            return null;
        }

        public static bool operator == (Role a, Role b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator != (Role a, Role b) => !(a == b);

        public static T GetRoleValue<T>(RoleEnum roleEnum) where T : Role => GetRoleValue(roleEnum) as T;

        public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

        public static Role GetRole(PlayerVoteArea area) => GetRole(Utils.PlayerByVoteArea(area));

        public static IEnumerable<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.RoleType == roletype && x.NotDefective);

        public static IEnumerable<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && x.NotDefective);

        public static IEnumerable<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra && x.NotDefective);

        public static IEnumerable<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction);

        public static IEnumerable<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results);

        public static IEnumerable<Role> GetRoles(string name) => AllRoles.Where(x => x.Name == name);

        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public static class CheckEndGame
        {
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (GameStates.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                bool crewexists = false;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Crew) && !player.NotOnTheSameSide())
                        crewexists = true;
                }

                if (Utils.NoOneWins())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.Stalemate);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.NobodyWins = true;
                    return true;
                }
                else if ((Utils.TasksDone() || GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) && crewexists)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CrewWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.CrewWin = true;
                    return true;
                }
                else if (Utils.Sabotaged())
                {
                    if (CustomGameOptions.AltImps)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                        writer.Write((byte)WinLoseRPC.SyndicateWin);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        Utils.EndGame();
                        Role.SyndicateWin = true;
                    }
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                        writer.Write((byte)WinLoseRPC.IntruderWin);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        Utils.EndGame();
                        Role.IntruderWin = true;
                    }

                    return true;
                }
                else
                {
                    foreach (var role in AllRoles)
                    {
                        if (!role.GameEnd(__instance))
                            return false;
                    }

                    return Utils.GameHasEnded();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__113), nameof(PlayerControl._CoSetTasks_d__113.MoveNext))]
        private static class PlayerControl_SetTasks
        {
            private static void Postfix(PlayerControl._CoSetTasks_d__113 __instance)
            {
                if (__instance == null)
                    return;

                var player = __instance.__4__this;

                try
                {
                    foreach (var task2 in player.myTasks.ToArray())
                    {
                        var importantTextTask = task2.Cast<ImportantTextTask>();

                        if (importantTextTask.Text.Contains("Sabotage and kill everyone") || importantTextTask.Text.Contains("Fake Tasks") || importantTextTask.Text.Contains("Role") ||
                            importantTextTask.Text.Contains("tasks to win"))
                            player.myTasks.Remove(importantTextTask);
                    }
                } catch {}

                var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = player.GetTaskList();
                player.myTasks.Insert(0, task);
                player.RegenTask();
            }
        }
    }
}