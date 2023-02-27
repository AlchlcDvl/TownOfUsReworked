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
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static List<GameObject> Buttons = new List<GameObject>();
        public static readonly Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();
        public readonly List<Footprint> AllPrints = new List<Footprint>();

        public static bool NobodyWins;
        
        public static bool UndeadWin;
        public static bool CabalWin;
        public static bool ReanimatedWin;
        public static bool SectWin;

        public static bool NKWins;
        
        public static bool CrewWin;
        public static bool IntruderWin;
        public static bool SyndicateWin;
        public static bool AllNeutralsWin;

        public static int ChaosDriveMeetingTimerCount;
        public static bool SyndicateHasChaosDrive;

        public virtual void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance) {}

        protected internal Color32 Color { get; set; } = Colors.Role;
        protected internal Color32 FactionColor { get; set; } = Colors.Faction;
        protected internal Color32 SubFactionColor { get; set; } = Colors.SubFaction;
        protected internal RoleEnum RoleType { get; set; } = RoleEnum.None;
        protected internal Faction Faction { get; set; } = Faction.None;
        protected internal RoleAlignment RoleAlignment { get; set; } = RoleAlignment.None;
        protected internal SubFaction SubFaction { get; set; } = SubFaction.None;
        protected internal InspectorResults InspectorResults { get; set; } = InspectorResults.None;
        protected internal string IntroSound { get; set; } = "";
        protected internal List<Role> RoleHistory { get; set; } = new List<Role>();
        protected internal List<KillButton> AbilityButtons { get; set; } = new List<KillButton>();
        protected internal KillButton PrimaryButton { get; set; } = null;
        protected internal KillButton SecondaryButton { get; set; } = null;
        protected internal KillButton TertiaryButton { get; set; } = null;
        protected internal KillButton QuartnaryButton { get; set; } = null;

        protected internal string StartText { get; set; } = "Woah The Game Started";
        protected internal string AbilitiesText { get; set; } = " - None.";

        protected internal string Name { get; set; } = "Roleless";
        protected internal string AlignmentName { get; set; } = "None";
        protected internal string FactionName { get; set; } = "None";
        protected internal string SubFactionName { get; set; } = "None";

        protected internal string RoleDescription { get; set; } = "You are a role!";

        protected internal string Objectives { get; set; } = "- None.";

        protected internal string KilledBy { get; set; } = "";
        protected internal DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;

        protected internal bool RoleBlockImmune { get; set; } = false;
        protected internal bool IsBlocked { get; set; } = false;

        protected internal bool Base { get; set; } = false;

        protected internal bool IsRecruit { get; set; } = false;
        protected internal bool IsResurrected { get; set; } = false;
        protected internal bool IsPersuaded { get; set; } = false;
        protected internal bool IsBitten { get; set; } = false;
        protected internal bool IsIntTraitor { get; set; } = false;
        protected internal bool IsIntAlly { get; set; } = false;
        protected internal bool IsIntFanatic { get; set; } = false;
        protected internal bool IsSynTraitor { get; set; } = false;
        protected internal bool IsSynAlly { get; set; } = false;
        protected internal bool IsSynFanatic { get; set; } = false;
        protected internal bool IsCrewAlly { get; set; } = false;
        protected internal bool NotDefective => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && !IsIntAlly && !IsIntFanatic && !IsIntTraitor && !IsSynAlly && !IsSynTraitor &&
            !IsSynFanatic && !IsCrewAlly;

        public static string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds.\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>.";
        public static string IntrudersObjective = "Have a critical sabotage reach 0 seconds or kill off all Syndicate, Unfaithful Intruders, Crew, Neutral Killers, Proselytes and " +
            "Neophytes.";
        public static string IntrudersFactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do " +
            "anything to ensure your victory over others.";
        public static string ISDescription = "You are an Intruder (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Help your team in " +
            "everyway possible.";
        public static string ICDescription = "You are an Intruder (Concealing) role! It's your primary job to ensure no information incriminating you or your mates" + 
            " is revealed to the rest of the crew. Do as much as possible to ensure as little information is leaked.";
        public static string IDDescription = "You are an Intruder (Deception) role! It's your job to ensure there's only false information spreading around about you. Keep the " +
            "misinformation circulating, for it can be advantageous to completely fool even one crew member.";
        public static string IUDescription = "You are a Intruder (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
        public static string IKDescription = "You are a Intruder (Killing) role! You have a ruthless ability to kill people with no mercy. Kill off the crew as fast as possible " + 
            "with your abilities!";
        public static string IS = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>";
        public static string IC = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>";
        public static string ID = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>";
        public static string IK = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string IU = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>";

        public static string SyndicateWinCon = (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds.\n" : "") + "- Cause chaos and kill anyone who opposes " +
            "the <color=#008000FF>Syndicate</color>.";
        public static string SyndicateObjective = (CustomGameOptions.AltImps ? "Have a critical sabotage reach 0 seconds or k" : "K") + "ill off all Intruders, Unfaithful Syndicate, " +
            "Crew and Neutral Killers, Proselytes and Neophytes.";
        public static string SyndicateFactionDescription = "Your faction is the Syndicate! Your faction has low killing power, is instead geared towards delaying the wins of other " +
            "factions and causing some good old chaos. After a certain number of meetings, the Syndicate will recieve the \"Chaos Drive\" which will enhance every member's powers and " +
            "give you an additional ability to kill.";
        public static string SUDescription = "You are a Syndicate (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
        public static string SSuDescription = "You are a Syndicate (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Cooperation is key " +
            "for your role to be used to its maximum potential.";
        public static string SDDescription = "You are a Syndicate (Disruption) role! Your main abilities lie in disrupting the flow of the game via unconventional and indirect means." + 
            " Create extreme levels of chaos by just the click of a button!";
        public static string SyKDescription = "You are a Syndicate (Killing) role! It's your job to ensure that the crew dies while you achieve your ulterior motives.";
        public static string SPDescription = "You are a Syndicate (Power) role! You are a powerful role who's only goal is to chaos and destruction. Ensure that the crew cannot get " +
            "their wits and information in order!";
        public static string SU = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Utility</color>)</color>";
        public static string SSu = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Support</color>)</color>";
        public static string SD = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Disruption</color>)</color>";
        public static string SyK = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string SP = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Power</color>)</color>";

        public static string CrewWinCon = "- Finish all tasks.\n- Eject all <color=#FF0000FF>evildoers</color>.";
        public static string CrewObjective = "Finish your tasks along with other Crew or kill off all Intruders, Syndicate, Unfaithful Crew, Neutral Killers, Proselytes and Neophytes.";
        public static string CrewFactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
            " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
        public static string CPDescription = "You are a Crew (Protective) role! You have the capability to stop someone from losing their life, and quite possibly" +
                " even gain information from the dead!";
        public static string CIDescription = "You are a Crew (Investigative) role! You can gain information via special methods and using that acquired info, you" +
            " can deduce who is good and who is not.";
        public static string CUDescription = "You are a Crew (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
        public static string CSDescription = "You are a Crew (Support) role! You have a miscellaneous ability that cannot be classified on its own. Use your abilities to their " +
            "fullest extent to bring about a Crew victory!";
        public static string CADescription = "You are a Crew (Auditor) role! You have a special goal. Find and eliminate those who stray from their path!";
        public static string CKDescription = "You are a Crew (Killing) role! You have no aversion to killing for the better good, even if it costs your life! Elimiate the evildoers" +
            " and save the Crew!";
        public static string CSvDescription = "You are a Crew (Sovereign) role! You are a democrat who has no issues with influencing the ballots to get what you want! Stay in power" +
            " get rid of any and all evildoers who threaten your position!";
        public static string CP = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Protective</color>)</color>";
        public static string CI = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Investigative</color>)</color>";
        public static string CU = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Utility</color>)</color>";
        public static string CS = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Support</color>)</color>";
        public static string CA = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Auditor</color>)</color>";
        public static string CK = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string CSv = "<color=#8BFDFDFF>Crew (<color=#1D7CF2FF>Sovereign</color>)</color>";
            
        public static string NeutralFactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
            " a certain objective.";
        public static string NBDescription = "You are a Neutral (Benign) role! You can win with anyone as long as a certain condition has been fulfilled for you.";
        public static string NKDescription = "You are a Neutral (Killing) role! You side with no one and can only win by yourself. You have a special way to kill and gain a large body" +
            " count. Make sure no one survives.";
        public static string NEDescription = "You are a Neutral (Evil) role! You have a conflicting win condition over others. Finish your objective before the others finish you!";
        public static string NPDescription = "You are now a Neutral (Proselyte) role! You are no longer the original you, and instead win as your new self!";
        public static string NNDescription = "You are a Neutral (Neophyte) role! You are the leader of your little faction and your main purpose is to convert others to your cause. " +
            "Gain a majority and overrun the crew!";
        public static string NB = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Benign</color>)</color>";
        public static string NE = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Evil</color>)</color>";
        public static string NK = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Killing</color>)</color>";
        public static string NN = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Neophyte</color>)</color>";
        public static string NP = "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Proselyte</color>)</color>";

        protected Role(PlayerControl player)
        {
            Player = player;

            if (RoleDictionary.ContainsKey(player.PlayerId))
                RoleDictionary.Remove(player.PlayerId);

            RoleDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                    _player.nameText().color = new Color32(255, 255, 255, 255);
                
                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.ToArray().Count();
        protected internal bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";
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

        public void AddToAbilityButtons(KillButton button, Role role)
        {
            if (!role.AbilityButtons.Contains(button))
                role.AbilityButtons.Add(button);
        }

        public static void SetColors()
        {
            
            LightDarkColors.Clear();
            LightDarkColors.Add(0, "darker"); //Red
            LightDarkColors.Add(1, "darker"); //Blue
            LightDarkColors.Add(2, "darker"); //Green
            LightDarkColors.Add(3, "lighter"); //Pink
            LightDarkColors.Add(4, "lighter"); //Orange
            LightDarkColors.Add(5, "lighter"); //Yellow
            LightDarkColors.Add(6, "darker"); //Black
            LightDarkColors.Add(7, "lighter"); //White
            LightDarkColors.Add(8, "darker"); //Purple
            LightDarkColors.Add(9, "darker"); //Brown
            LightDarkColors.Add(10, "lighter"); //Cyan
            LightDarkColors.Add(11, "lighter"); //Lime
            LightDarkColors.Add(12, "darker"); //Maroon
            LightDarkColors.Add(13, "lighter"); //Rose
            LightDarkColors.Add(14, "lighter"); //Banana
            LightDarkColors.Add(15, "darker"); //Grey
            LightDarkColors.Add(16, "darker"); //Tan
            LightDarkColors.Add(17, "lighter"); //Coral
            LightDarkColors.Add(18, "darker"); //Watermelon
            LightDarkColors.Add(19, "darker"); //Chocolate
            LightDarkColors.Add(20, "lighter"); //Sky Blue
            LightDarkColors.Add(21, "lighter"); //Biege
            LightDarkColors.Add(22, "lighter"); //Hot Pink
            LightDarkColors.Add(23, "lighter"); //Turquoise
            LightDarkColors.Add(24, "lighter"); //Lilac
            LightDarkColors.Add(25, "darker"); //Olive
            LightDarkColors.Add(26, "lighter"); //Azure
            LightDarkColors.Add(27, "darker"); // Plum
            LightDarkColors.Add(28, "darker"); // Jungle
            LightDarkColors.Add(29, "lighter"); // Mint
            LightDarkColors.Add(30, "lighter"); // Chartreuse
            LightDarkColors.Add(31, "darker"); // Macau
            LightDarkColors.Add(32, "darker"); // Tawny
            LightDarkColors.Add(33, "lighter"); // Gold
            LightDarkColors.Add(34, "lighter"); // Rainbow
        }

        internal virtual bool GameEnd(LogicGameFlowNormal __instance) => true;

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

        public static IEnumerable<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.RoleType == roletype);

        public static IEnumerable<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction);

        public static IEnumerable<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra);

        public static IEnumerable<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction);

        public static IEnumerable<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results);
        
        [HarmonyPatch]
        public static class CheckEndGame
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
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

                    return false;
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
                var text = player.GetTaskList();

                try
                {
                    var firstText = player.myTasks.ToArray()[0].Cast<ImportantTextTask>();

                    if (firstText.Text.Contains("Sabotage and kill everyone"))
                        player.myTasks.Remove(firstText);

                    firstText = player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                    
                    if (firstText.Text.Contains("Fake"))
                        player.myTasks.Remove(firstText);
                } catch {}

                var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = text;
                player.myTasks.Insert(0, task);
            }
        }
    }
}