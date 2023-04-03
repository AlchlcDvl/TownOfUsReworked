using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Objects;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    [HarmonyPatch]
    public abstract class Role : PlayerLayer
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new();
        public static List<Role> AllRoles => RoleDictionary.Values.ToList();
        public static readonly List<GameObject> Buttons = new();
        public static readonly Dictionary<int, string> LightDarkColors = new();

        public virtual void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance) {}

        #pragma warning disable
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

        public static bool PhantomWins;

        public static bool RoleWins => UndeadWin || CabalWin || InfectorsWin || ReanimatedWin || SectWin || NKWins || CrewWin || IntruderWin || SyndicateWin || AllNeutralsWin || GlitchWins
            || JuggernautWins || SerialKillerWins || ArsonistWins || CryomaniacWins || MurdererWins || PhantomWins || WerewolfWins;

        public static int ChaosDriveMeetingTimerCount;
        public static bool SyndicateHasChaosDrive;
        #pragma warning restore

        public Color32 FactionColor = Colors.Faction;
        public Color32 SubFactionColor = Colors.SubFaction;
        public RoleEnum Type = RoleEnum.None;
        public Faction Faction = Faction.None;
        public Faction BaseFaction = Faction.None;
        public RoleAlignment RoleAlignment = RoleAlignment.None;
        public SubFaction SubFaction = SubFaction.None;
        public InspectorResults InspectorResults = InspectorResults.None;
        public List<Role> RoleHistory = new();

        public string IntroSound => $"{Name}Intro";
        public bool IntroPlayed;

        public string StartText = "Woah The Game Started";
        public string AbilitiesText = "- None.";

        public string AlignmentName = "None";
        public string FactionName => $"{Faction}";
        public string SubFactionName => $"{SubFaction}";

        public string Objectives = "- None.";

        public bool Base;

        public readonly List<Footprint> AllPrints = new();

        public AbilityButton SpectateButton;

        public AbilityButton ZoomButton;
        public bool Zooming;

        public bool IsRecruit;
        public bool IsResurrected;
        public bool IsPersuaded;
        public bool IsBitten;
        public bool IsIntTraitor;
        public bool IsIntAlly;
        public bool IsIntFanatic;
        public bool IsSynTraitor;
        public bool IsSynAlly;
        public bool IsSynFanatic;
        public bool IsCrewAlly;
        public bool NotDefective => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && !IsIntAlly && !IsIntFanatic && !IsIntTraitor && !IsSynAlly && !IsSynTraitor &&
            !IsSynFanatic && !IsCrewAlly;

        public bool Winner;

        public override void UpdateHud(HudManager __instance)
        {
            if (Player.Data.IsDead)
            {
                var ghostRole = (Player.Is(RoleEnum.Revealer) && !GetRole<Revealer>(Player).Caught) || (Player.Is(RoleEnum.Ghoul) && !GetRole<Ghoul>(Player).Caught) ||
                    (Player.Is(RoleEnum.Banshee) && !GetRole<Banshee>(Player).Caught) || (Player.Is(RoleEnum.Phantom) && !GetRole<Phantom>(Player).Caught);

                if (!ghostRole)
                {
                    if (SpectateButton == null)
                        SpectateButton = CustomButtons.InstantiateButton();

                    SpectateButton.UpdateButton(this, "SPECTATE", 0, 1, AssetManager.Placeholder, AbilityTypes.Effect, "ActionSecondary", null, !ghostRole, !ghostRole, false, 0, 1,
                        false, 0, !ghostRole);

                    if (ZoomButton == null)
                        ZoomButton = CustomButtons.InstantiateButton();

                    ZoomButton.UpdateButton(this, "SPECTATE", 0, 1, Zooming ? AssetManager.Minus : AssetManager.Plus, AbilityTypes.Effect, "Secondary", null, !ghostRole, !ghostRole,
                        false, 0, 1, false, 0, !ghostRole);
                }
            }
        }

        public static readonly string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds.\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>.";
        public static readonly string IS = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>";
        public static readonly string IC = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>";
        public static readonly string ID = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>";
        public static readonly string IK = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string IU = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>";

        public static readonly string SyndicateWinCon = (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds.\n" : "") + "- Cause chaos and kill anyone who opposes " +
            "the <color=#008000FF>Syndicate</color>.";
        public static readonly string SU = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Utility</color>)</color>";
        public static readonly string SSu = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Support</color>)</color>";
        public static readonly string SD = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Disruption</color>)</color>";
        public static readonly string SyK = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string SP = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Power</color>)</color>";

        public static readonly string CrewWinCon = "- Finish all tasks.\n- Eject all <color=#FF0000FF>evildoers</color>.";
        public static readonly string CP = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Protective</color>)</color>";
        public static readonly string CI = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Investigative</color>)</color>";
        public static readonly string CU = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Utility</color>)</color>";
        public static readonly string CS = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Support</color>)</color>";
        public static readonly string CA = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Auditor</color>)</color>";
        public static readonly string CK = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string CSv = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Sovereign</color>)</color>";

        public static readonly string NB = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Benign</color>)</color>";
        public static readonly string NE = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Evil</color>)</color>";
        public static readonly string NK = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string NN = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Neophyte</color>)</color>";
        public static readonly string NP = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Proselyte</color>)</color>";

        protected Role(PlayerControl player) : base(player)
        {
            if (RoleDictionary.ContainsKey(player.PlayerId))
                RoleDictionary.Remove(player.PlayerId);

            RoleDictionary.Add(player.PlayerId, this);
            Color = Colors.Layer;
        }

        public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
        public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

        private bool Equals(Role other) => Equals(Player, other.Player) && Type == other.Type;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Role))
                return false;

            return Equals((Role)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)Type);

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

        public static Role GetRole(PlayerControl player) => AllRoles.Find(x => x.Player == player);

        public static List<Role> GetRoleValue(RoleEnum roleEnum) => AllRoles.Where(x => x.Type == roleEnum).ToList();

        public static Role GetRoleFromName(string name) => AllRoles.Find(x => x.Name == name);

        public static bool operator == (Role a, Role b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Type == b.Type && a.Player == b.Player;
        }

        public static bool operator != (Role a, Role b) => !(a == b);

        public static T GetRoleValue<T>(RoleEnum roleEnum) where T : Role => GetRoleValue(roleEnum) as T;

        public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

        public static Role GetRole(PlayerVoteArea area) => GetRole(Utils.PlayerByVoteArea(area));

        public static List<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.Type == roletype && x.NotDefective).ToList();

        public static List<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && x.NotDefective).ToList();

        public static List<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra && x.NotDefective).ToList();

        public static List<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction).ToList();

        public static List<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results).ToList();

        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public static class CheckEndGame
        {
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (ConstantVariables.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                var crewexists = false;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Crew) && !player.NotOnTheSameSide())
                        crewexists = true;
                }

                if (ConstantVariables.NoOneWins)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.NobodyWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    NobodyWins = true;
                    Objectifier.NobodyWins = true;
                    return true;
                }
                else if ((Utils.TasksDone() || GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) && crewexists)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CrewWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    CrewWin = true;
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
                        SyndicateWin = true;
                    }
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                        writer.Write((byte)WinLoseRPC.IntruderWin);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        Utils.EndGame();
                        IntruderWin = true;
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

                    return ConstantVariables.GameHasEnded;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__114), nameof(PlayerControl._CoSetTasks_d__114.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__114 __instance)
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
                        {
                            player.myTasks.Remove(importantTextTask);
                        }
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