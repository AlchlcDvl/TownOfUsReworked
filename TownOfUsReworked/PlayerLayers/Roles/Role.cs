using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public List<KillButton> ExtraButtons = new List<KillButton>();
        //public List<CustomButton> OtherButtons = new List<CustomButton>();
        public static List<GameObject> Buttons = new List<GameObject>();
        public static readonly Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();

        public static bool NobodyWins;
        public static bool NeutralsWin;
        
        public static bool UndeadWin;
        public static bool CabalWin;

        public static bool NKWins;
        
        public static bool CrewWin;
        public static bool IntruderWin;
        public static bool SyndicateWin;
        public static bool AllNeutralsWin;

        public static int ChaosDriveMeetingTimerCount;
        public static bool SyndicateHasChaosDrive;

        public virtual void Loses()
        {
            LostByRPC = true;
        }

        public virtual void Wins() {}

        public virtual void HudUpdate() {}
        protected virtual void AddCustomButtons() {}

        protected internal Color32 Color { get; set; } = Colors.Role;
        protected internal Color32 FactionColor { get; set; } = Colors.Faction;
        protected internal Color32 SubFactionColor { get; set; } = Colors.Clear;
        protected internal RoleEnum RoleType { get; set; } = RoleEnum.None;
        protected internal Faction Faction { get; set; } = Faction.None;
        protected internal RoleAlignment RoleAlignment { get; set; } = RoleAlignment.None;
        protected internal InspResults Results { get; set; } = InspResults.None;
        protected internal SubFaction SubFaction { get; set; } = SubFaction.None;
        protected internal AttackEnum Attack { get; set; } = AttackEnum.None;
        protected internal DefenseEnum Defense { get; set; } = DefenseEnum.None;
        protected internal DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;
        protected internal AudioClip IntroSound { get; set; } = null;
        protected internal List<Role> RoleHistory { get; set; } = new List<Role>();
        protected internal string StartText { get; set; } = "";
        protected internal string AbilitiesText { get; set; } = " - None.";
        protected internal string AttributesText { get; set; } = " - None.";
        protected internal string Name { get; set; } = "";
        protected internal string AlignmentName { get; set; } = "";
        protected internal string FactionName { get; set; } = "";
        protected internal string SubFactionName { get; set; } = "";
        protected internal string AttackString { get; set; } = "None";
        protected internal string DefenseString { get; set; } = "None";
        protected internal string FactionDescription { get; set; } = "";
        protected internal string RoleDescription { get; set; } = "";
        protected internal string AlignmentDescription { get; set; } = "";
        protected internal string Objectives { get; set; } = "";
        protected internal string KilledBy { get; set; } = "";
        protected internal bool Base { get; set; } = false;
        protected internal bool IsRecruit { get; set; } = false;
        protected internal bool IsRevived { get; set; } = false;
        protected internal bool IsPursuaded { get; set; } = false;
        protected internal bool IsIntTraitor { get; set; } = false;
        protected internal bool IsSynTraitor { get; set; } = false;
        protected internal bool IsIntFanatic { get; set; } = false;
        protected internal bool IsSynFanatic { get; set; } = false;
        protected internal bool IsBlocked { get; set; } = false;
        protected internal bool RoleBlockImmune { get; set; } = false;

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        public static string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds.\n   or\n- Kill: <color=#008000FF>Syndicate</color>, <color=#575657FF>Recruited</color> " +
            "<color=#FF0000FF>Intruders</color>, <color=#8BFDFDFF>Crew</color>, <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
            "<color=#1D7CF2FF>Neophytes</color>.";
        public static string IntruderFactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do " +
            "anything to ensure your victory over others.";
        public static string ISDescription = "You are an Intruder (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Support them in " +
            "everyway possible.";
        public static string ICDescription = "You are an Intruder (Concealing) role! It's your primary job to ensure no information incriminating you or your mates" + 
            " is revealed to the rest of the crew. Do as much as possible to ensure as little information is leaked.";
        public static string IDDescription = "You are an Intruder (Deception) role! It's your job to ensure there's only false information spreading around about you. Keep the " +
            "misinformation circulating, for it can be advantageous to completely fool even one player.";
        public static string IUDescription = "You are a Intruder (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
        public static string IKDescription = "You are a Intruder (Killing) role! You have a ruthless ability to kill people with no mercy. Kill off the crew as fast as possible " + 
            "with your abilities!";

        public static string SyndicateWinCon = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#575657FF>Recruited</color> <color=#008000FF>Syndicate</color>, " +
            "<color=#8BFDFDFF>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
            "<color=#1D7CF2FF>Neophytes</color>.";
        public static string SyndicateFactionDescription = "Your faction is the Syndicate! Your faction has low killing power and is instead geared towards delaying the wins of other " +
            "factions and causing some good old chaos. After a certain number of meeting, one of you will recieve the \"Chaos Drive\" which will enhance your powers and " +
            "give you the ability to kill, if you didn't already.";
        public static string SUDescription = "You are a Syndicate (Utility) role! You usually have no special ability and cannot even appear under natural conditions.";
        public static string SSuDescription = "You are a Syndicate (Support) role! It is your job to ensure no one bats an eye to the things you or your mates do. Cooperation is key " +
            "for your role to be used to its maximum potential.";
        public static string SDDescription = "You are a Syndicate (Disruption) role! Your main abilities lie in disrupting the flow of the game via unconventional and indirect means." + 
            " Create extreme levels of chaos by just the click of a button!";
        public static string SyKDescription = "You are a Syndicate (Killing) role! It's your job to ensure that the crew dies while you achieve your ulterior motives.";
        public static string SPDescription = "You are a Syndicate (Power) role! You are a powerful role who's only goal is to chaos and destruction. Ensure that the crew cannot get " +
            "their wits and information in order!";

        public static string CrewFactionDescription = "Your faction is the Crew! You do not know who the other members of your faction are. It is your job to deduce" + 
            " who is evil and who is not. Eject or kill all evils or finish all of your tasks to win!";
        public static string CPDescription = "You are a Crew (Protective) role! You have the capability to stop someone from losing their life, and quite possibly" +
                " even gain information from the dead!";
        public static string CIDescription = "You are a Crew (Investigative) role! You can gain information via special methods and using that acquired info, you" +
            " can deduce who is good and who is not.";
        public static string CUDescription = "You are a Crewmate! Your role is the base role for the Crew faction. You have no special abilities and should probably do your tasks.";
        public static string CrewWinCon = "- Finish your tasks along with other <color=#8BFDFDFF>Crew</color>.\n   or\n- Kill: <color=#FF0000FF>Intruders</color>, " +
            "<color=#008000FF>Syndicate</color>, <color=#370D43FF>Traitor</color>, <color=#4545FFFF>Corrupted</color> and <color=#575657FF>Recruited</color> " +
            "<color=#8BFDFDFF>Crew</color>,\n<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
            "<color=#1D7CF2FF>Neophytes</color>.";
        public static string CSDescription = "You are a Crew (Support) role! You have a miscellaneous ability that cannot be classified on its own. Use your abilities to their " +
            "fullest extent to bring about a Crew victory!";
        public static string CADescription = "You are a Crew (Auditor) role! You have a miscellaneous ability that cannot be classified on its own. Use your abilities to their " +
            "fullest extent to bring about a Crew victory!";
            
        public static string NeutralFactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
            " a certain objective.";
        public static string NBDescription = "You are a Neutral (Benign) role! You can win with anyone as long as a certain condition has been fulfilled for you.";
        public static string NKWinCon = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color>, <color=#008000FF>Syndicate</color>, <color=#575657FF>Recruited</color> " +
            "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color> of the same type, other <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, " +
            "<color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>";
        public static string NKDescription = "You are a Neutral (Killing) role! You side with no one and can only win by yourself. You have a special way to kill and gain a large body" +
            " count. Make sure no one survives.";
        public static string NEDescription = "You are a Neutral (Evil) role! You have a conflicting win condition over others and upon achieving it will most likely end the game. " +
            "Finish your objective before the others finish you!";
        public static string NPDescription = "You are now a Neutral (Proselyte) role! You are no longer the original you, and instead win with the one who converted you! Help your leader" + 
            " gain a quick majority!";
        public static string JackalWinCon = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFDFF>Crew</color>, <color=#008000FF>Syndicate</color>, <color=#B3B3B3FF>Neutral</color>" +
            " <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and other <color=#1D7CF2FF>Neophytes</color> excluding the other <color=#575657FF>Recruits</color>";
        public static string UndeadWinCon = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFDFF>Crew</color>, <color=#008000FF>Syndicate</color> and " +
            "Non-<color=#7B8968FF>Undead</color> <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and " +
            "<color=#1D7CF2FF>Neophytes</color>.";
        public static string NNDescription = "You are a Neutral (Neophyte) role! You are the leader of your little faction and your main purpose is to convert others to your cause. " +
            "Gain a majority and overrun the crew!";

        protected Role(PlayerControl player)
        {
            Player = player;

            if (!RoleDictionary.ContainsKey(player.PlayerId))
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

        public bool LostByRPC { get; protected set; }

        private Tuple<int, int> Tasks => Utils.TaskInfo(Player);
        protected internal int TasksLeft => Tasks.Item1;
        protected internal int TotalTasks => Tasks.Item2;
        protected internal bool TasksDone => TasksLeft <= 0;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        public string FactionColorString => "<color=#" + FactionColor.ToHtmlStringRGBA() + ">";
        public string SubFactionColorString => "<color=#" + SubFactionColor.ToHtmlStringRGBA() + ">";

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        protected virtual void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance) {}

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }

        public static void NeutralsOnlyWin()
        {
            NeutralsWin = true;
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
            LightDarkColors.Add(21, "darker"); //Biege
            LightDarkColors.Add(22, "lighter"); //Hot Pink
            LightDarkColors.Add(23, "lighter"); //Turquoise
            LightDarkColors.Add(24, "lighter"); //Lilac
            LightDarkColors.Add(25, "darker"); //Olive
            LightDarkColors.Add(26, "lighter"); //Azure
            LightDarkColors.Add(27, "lighter"); //Tomato
            LightDarkColors.Add(28, "darker"); //backrooms
            LightDarkColors.Add(29, "darker"); //Gold
            LightDarkColors.Add(30, "darker"); //Space
            LightDarkColors.Add(31, "lighter"); //Ice
            LightDarkColors.Add(32, "lighter"); //Mint
            LightDarkColors.Add(33, "darker"); //BTS
            LightDarkColors.Add(34, "darker"); //Forest Green
            LightDarkColors.Add(35, "lighter"); //Donation
            LightDarkColors.Add(36, "darker"); //Cherry
            LightDarkColors.Add(37, "lighter"); //Toy
            LightDarkColors.Add(38, "lighter"); //Pizzaria
            LightDarkColors.Add(39, "lighter"); //Starlight
            LightDarkColors.Add(40, "lighter"); //Softball
            LightDarkColors.Add(41, "darker"); //Dark Jester
            LightDarkColors.Add(42, "darker"); //FRESH
            LightDarkColors.Add(43, "darker"); //Goner
            LightDarkColors.Add(44, "lighter"); //Psychic Friend
            LightDarkColors.Add(45, "lighter"); //Frost
            LightDarkColors.Add(46, "darker"); //Abyss Green
            LightDarkColors.Add(47, "darker"); //Midnight
            LightDarkColors.Add(48, "darker"); //<3
            LightDarkColors.Add(49, "lighter"); //Heat From Fire
            LightDarkColors.Add(50, "lighter"); //Fire From Heat
            LightDarkColors.Add(51, "lighter"); //Determination
            LightDarkColors.Add(52, "lighter"); //Patience
            LightDarkColors.Add(53, "darker"); //Bravery
            LightDarkColors.Add(54, "darker"); //Integrity
            LightDarkColors.Add(55, "darker"); //Perserverance
            LightDarkColors.Add(56, "darker"); //Kindness
            LightDarkColors.Add(57, "lighter"); //Bravery
            LightDarkColors.Add(58, "darker"); //Purple Plumber
            LightDarkColors.Add(59, "lighter"); //Rainbow
        }

        internal virtual bool GameEnd(ShipStatus __instance)
        {
            return true;
        }
        
        /*protected void RegisterCustomButton(CustomButton button)
		{
			button.OwnerId = Player.PlayerId;
			OtherButtons.Add(button);
		}*/

        internal virtual bool DeadCriteria()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything)
                return Utils.ShowDeadBodies;

            return false;
        }

        internal virtual bool FactionCriteria()
        {
            var syndicateFlag = (Faction == Faction.Syndicate || IsSynTraitor) && (PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.IsSynTraitor());
            var intruderFlag = (Faction == Faction.Intruder || IsIntTraitor) && (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.IsIntTraitor());
            var cabalFlag = ((SubFaction == SubFaction.Cabal || IsRecruit) && (PlayerControl.LocalPlayer.Is(SubFaction.Cabal) || PlayerControl.LocalPlayer.IsRecruit()));
            var sectFlag = ((SubFaction == SubFaction.Sect || IsPursuaded) && (PlayerControl.LocalPlayer.Is(SubFaction.Sect) || PlayerControl.LocalPlayer.IsRecruit()));
            var reanimatedFlag = ((SubFaction == SubFaction.Reanimated || IsRevived) && (PlayerControl.LocalPlayer.Is(SubFaction.Reanimated) || PlayerControl.LocalPlayer.IsRecruit()));
            var undeadFlag = SubFaction == SubFaction.Undead && PlayerControl.LocalPlayer.Is(SubFaction.Undead);

            var mainFlag = (syndicateFlag || intruderFlag || cabalFlag || sectFlag || reanimatedFlag || undeadFlag) && CustomGameOptions.FactionSeeRoles;
            return mainFlag;
        }

        internal virtual bool Criteria()
        {
            if (Player == null)
                return false;

            Player.nameText().transform.localPosition = new Vector3(0f, Player.Data.DefaultOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);
            return (DeadCriteria() || FactionCriteria() || SelfCriteria() || RoleCriteria() || TargetCriteria() || Local || LoverRivalRoleCriteria()) && !LobbyBehaviour.Instance;
        }

        internal virtual bool RoleCriteria()
        {
            var coronerFlag = PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && GetRole<Coroner>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId);
            var consigFlag = PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere) && GetRole<Consigliere>(PlayerControl.LocalPlayer).Investigated.Contains(Player.PlayerId) &&
                CustomGameOptions.ConsigInfo == ConsigInfo.Role;
            return coronerFlag || consigFlag;
        }

        internal virtual bool SelfCriteria()
        {
            var role = GetRole(PlayerControl.LocalPlayer);

            if (role == null)
                return false;

            return role == this;
        }

        internal virtual bool ColorCriteria()
        {
            return (SelfCriteria() || DeadCriteria() || FactionCriteria() || RoleCriteria() || TargetCriteria() || LoverRivalRoleCriteria()) && !LobbyBehaviour.Instance;
        }
        
        internal virtual bool TargetCriteria()
        {
            bool flag = false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var role = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                PlayerControl target = role.TargetPlayer;
                flag = target != null && CustomGameOptions.GAKnowsTargetRole && Player == target;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
            {
                var role = GetRole<Executioner>(PlayerControl.LocalPlayer);
                PlayerControl target = role.TargetPlayer;
                flag = target != null && CustomGameOptions.ExeKnowsTargetRole && Player == target;
            }

            return flag;
        }
        
        internal virtual bool LoverRivalRoleCriteria()
        {
            bool flag = false;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers))
            {
                var role = Objectifier.GetObjectifier<Lovers>(PlayerControl.LocalPlayer);
                PlayerControl otherlover = role.OtherLover;
                flag = otherlover != null && CustomGameOptions.LoversRoles && Player == otherlover;
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals))
            {
                var role = Objectifier.GetObjectifier<Rivals>(PlayerControl.LocalPlayer);
                PlayerControl otherrival = role.OtherRival;
                flag = otherrival != null && CustomGameOptions.RivalsRoles && Player == otherrival;
            }

            return flag;
        }
        
        internal virtual bool LoverRivalCriteria()
        {
            bool flag = false;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Lovers))
            {
                var role = Objectifier.GetObjectifier<Lovers>(PlayerControl.LocalPlayer);
                PlayerControl otherlover = role.OtherLover;
                flag = otherlover != null && Player == otherlover;
            }
            else if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Rivals))
            {
                var role = Objectifier.GetObjectifier<Rivals>(PlayerControl.LocalPlayer);
                PlayerControl otherrival = role.OtherRival;
                flag = otherrival != null && Player == otherrival;
            }

            return flag;
        }

        protected virtual string NameText(bool revealTasks, bool revealRole, bool revealObjectifier, PlayerVoteArea player = null)
        {
            if (Player == null || CamouflageUnCamouflage.IsCamoed || (CustomGameOptions.NoNames && !Local))
                return "";

            string PlayerName = Player.GetDefaultOutfit().PlayerName;
            
            if (LobbyBehaviour.Instance)
                return PlayerName;
            
            if (!Local)
                return PlayerName;

            var objectifier = Objectifier.GetObjectifier(Player);

            if (objectifier != null && revealObjectifier)
                PlayerName += $" {objectifier.GetColoredSymbol()}";

            if (player != null && PlayerControl.LocalPlayer.CanDoTasks() && !TasksDone && CustomGameOptions.SeeTasks && revealTasks)
                PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";

            if (Local && CustomGameOptions.GATargetKnows && PlayerControl.LocalPlayer.IsGATarget())
                PlayerName += " <color=#FFFFFFFF> ★</color>";

            if (Local && CustomGameOptions.ExeTargetKnows && PlayerControl.LocalPlayer.IsExeTarget())
                PlayerName += " <color=#CCCCCCFF> §</color>";

            if ((Player.IsRecruit() || Player.Is(RoleEnum.Jackal)) && (PlayerControl.LocalPlayer.IsRecruit() || PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)))
                PlayerName += " <color=#575657FF> $</color>";

            if (player != null && MeetingHud.Instance && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding || MeetingHud.Instance.state == MeetingHud.VoteStates.Results))
                return PlayerName;

            if (!revealRole)
                return PlayerName;

            Player.nameText().transform.localPosition = new Vector3(0f, Player.CurrentOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);
            return PlayerName + "\n" + Name;
        }

        public void RegenTask()
        {
            bool createTask;
            string tasks = $"{ColorString}Role: {Name}\nAlignment: {AlignmentName}\nObjective:\n{Objectives}\nAttack/Defense:" +
                $" {AttackString}/{DefenseString}\nAbilities:\n{AbilitiesText}\nAttributes:\n{AttributesText}</color>";

            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = tasks;
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = tasks;
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] {player});
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)rpc, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return role;
        }
        
        public static Role GetRole(PlayerControl player)
        {
            if (player == null)
                return null;

            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

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

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }
        
        public static T GetRoleValue<T>(RoleEnum roleEnum) where T : Role
        {
            return GetRoleValue(roleEnum) as T;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = Utils.PlayerById(area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static IEnumerable<Role> GetRoles(Faction faction)
        {
            return AllRoles.Where(x => x.Faction == faction);
        }

        public static IEnumerable<Role> GetRoles(SubFaction subfaction)
        {
            return AllRoles.Where(x => x.SubFaction == subfaction);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro StatusText;
            public static float Scale;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);
                    var flag = modifier == null && ability == null && objectifier == null;

                    StatusText = !flag ? Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false) : null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);
                    var flag = modifier == null && ability == null && objectifier == null;

                    StatusText = !flag ? Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false) : null;
                }
            }
            
            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
            public static class IntroCutscene_CoBegin_d__19
            {
                public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.TeamTitle.outlineColor = new Color32(0, 0, 0, 255);
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                        __instance.__4__this.ImpostorText.text = " ";
                        __instance.__4__this.RoleBlurbText.text = role.StartText;

                        if (role.IntroSound != null)
                        {
                            try
                            {
                                SoundManager.Instance.PlaySound(role.IntroSound, false, 1f);
                            } catch {}
                        }

                        var flag = !role.Base && ((CustomGameOptions.CustomCrewColors && PlayerControl.LocalPlayer.Is(Faction.Crew)) || 
                            (CustomGameOptions.CustomIntColors && PlayerControl.LocalPlayer.Is(Faction.Intruder)) ||
                            (CustomGameOptions.CustomSynColors && PlayerControl.LocalPlayer.Is(Faction.Syndicate)) ||
                            (CustomGameOptions.CustomNeutColors && PlayerControl.LocalPlayer.Is(Faction.Neutral)));

                        if (flag)
                            __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                    }

                    if (StatusText != null)
                    {
                        var player = PlayerControl.LocalPlayer;
                        var modifier = Modifier.GetModifier(player);
                        var objectifier = Objectifier.GetObjectifier(player);
                        var ability = Ability.GetAbility(player);

                        var statusString = "";
                        var status = "";

                        if (modifier != null && !modifier.Hidden)
                            status += $" {modifier.ColorString}{modifier.Name}</color>";

                        if (objectifier != null && !objectifier.Hidden)
                            status += $" {objectifier.ColorString}{objectifier.Name}</color>";

                        if (ability != null && !ability.Hidden)
                            status += $" {ability.ColorString}{ability.Name}</color>";

                        if (status.Length != 0)
                            statusString = "<size=4><color=#" + Colors.Status.ToHtmlStringRGBA() + ">Status</color>:" + statusString + status + "</size>";

                        StatusText.text = statusString;
                        StatusText.outlineColor = Colors.Status;

                        StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        StatusText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__21), nameof(IntroCutscene._ShowTeam_d__21.MoveNext))]
            public static class IntroCutscene_ShowTeam__d_21
            {
                public static void Prefix(IntroCutscene._ShowTeam_d__21 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                        role.IntroPrefix(__instance);
                }

                public static void Postfix(IntroCutscene._ShowTeam_d__21 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.TeamTitle.outlineColor = new Color32(0, 0, 0, 255);
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                        __instance.__4__this.ImpostorText.text = " ";
                    }

                    if (StatusText != null)
                    {
                        var player = PlayerControl.LocalPlayer;
                        var modifier = Modifier.GetModifier(player);
                        var objectifier = Objectifier.GetObjectifier(player);
                        var ability = Ability.GetAbility(player);

                        var statusString = "";
                        var status = "";

                        if (modifier != null && !modifier.Hidden)
                            status += $" {modifier.ColorString}{modifier.Name}</color>";

                        if (objectifier != null && !objectifier.Hidden)
                            status += $" {objectifier.ColorString}{objectifier.Name}</color>";

                        if (ability != null && !ability.Hidden)
                            status += $" {ability.ColorString}{ability.Name}</color>";

                        if (status.Length != 0)
                            statusString = "<size=4><color=#" + Colors.Status.ToHtmlStringRGBA() + ">Status</color>:" + statusString + status + "</size>";

                        StatusText.text = statusString;
                        StatusText.outlineColor = Colors.Status;

                        StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        StatusText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__24), nameof(IntroCutscene._ShowRole_d__24.MoveNext))]
            public static class IntroCutscene_ShowRole_d__24
            {
                public static void Postfix(IntroCutscene._ShowRole_d__24 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.StartText;
                        __instance.__4__this.RoleBlurbText.color = role.Color;

                        SoundManager.Instance.StopAllSound();

                        if (role.IntroSound != null)
                        {
                            try
                            {
                                SoundManager.Instance.PlaySound(role.IntroSound, false, 1f);
                            } catch {}
                        }

                        var flag = !role.Base && ((CustomGameOptions.CustomCrewColors && PlayerControl.LocalPlayer.Is(Faction.Crew)) || 
                            (CustomGameOptions.CustomIntColors && PlayerControl.LocalPlayer.Is(Faction.Intruder)) ||
                            (CustomGameOptions.CustomSynColors && PlayerControl.LocalPlayer.Is(Faction.Syndicate)) ||
                            (CustomGameOptions.CustomNeutColors && PlayerControl.LocalPlayer.Is(Faction.Neutral)));

                        if (flag)
                            __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__110), nameof(PlayerControl._CoSetTasks_d__110.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__110 __instance)
            {
                if (__instance == null)
                    return;

                var player = __instance.__4__this;
                var role = GetRole(player);
                var modifier = Modifier.GetModifier(player);
                var objectifier = Objectifier.GetObjectifier(player);
                var ability = Ability.GetAbility(player);

                if (role != null)
                {
                    var task = new GameObject("OtherTask").AddComponent<ImportantTextTask>();
                    task.transform.SetParent(player.transform, false);
                    task.Text = "Other Things:";

                    if (role.IsRecruit)
                    {
                        var jackal = role.Player.GetJackal();
                        task.Text += $"\n- <color=#" + Colors.Cabal.ToHtmlStringRGBA() + $">You are a member of the Cabal! Help {jackal.Player.name} in taking over the mission!</color>";
                    }
                    
                    if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
                        task.Text += "\n- <color=#FFFFFFFF>You are a Guardian Angel's target!</color>";
                    
                    if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
                        task.Text += "\n- <color=#CCCCCCFF>You are an Executioner's target!</color>";

                    if (!role.IsRecruit && !player.IsGATarget() && !player.IsExeTarget())
                        task.Text = "";

                    if (!player.CanDoTasks() && !player.Is(Faction.Intruder))
                        task.Text += "\nFake Tasks:";
                    
                    if (task.Text != "")
                        player.myTasks.Insert(0, task);
                }

                if (ability != null)
                {
                    var avTask = new GameObject(ability.Name + "Task").AddComponent<ImportantTextTask>();
                    avTask.transform.SetParent(player.transform, false);
                    avTask.Text = $"{ability.ColorString}Ability: {ability.Name}\n{ability.TaskText}</color>";
                    player.myTasks.Insert(0, avTask);
                }

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text = $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (objectifier != null)
                {
                    var obTask = new GameObject(objectifier.Name + "Task").AddComponent<ImportantTextTask>();
                    obTask.transform.SetParent(player.transform, false);
                    obTask.Text = $"{objectifier.ColorString}Objectifier: {objectifier.Name}\n{objectifier.TaskText}</color>";
                    player.myTasks.Insert(0, obTask);
                }

                if (role != null)
                {
                    var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                    task.transform.SetParent(player.transform, false);
                    task.Text = $"{role.ColorString}Role: {role.Name}\nAlignment: {role.AlignmentName}\nObjective:\n{(role.IsRecruit ? JackalWinCon : role.Objectives)}\nAttack/Defense:" +
                        $" {role.AttackString}/{role.DefenseString}\nAbilities:\n{role.AbilitiesText}\nAttributes:\n{role.AttributesText}</color>";
                        
                    player.myTasks.Insert(0, task);
                }
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                foreach (var role in AllRoles)
                {
                    var roleIsEnd = role.GameEnd(__instance);

                    if (!roleIsEnd)
                        return false;
                }

                return true;
            }
        }
        
        public PlayerControl GetTarget(bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer =
            null, bool evenWhenDead = false)
		{
			PlayerControl result = null;
			float num = GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];

			if (!ShipStatus.Instance)
				return result;

			if (targetingPlayer == null)
				targetingPlayer = PlayerControl.LocalPlayer;

			if (!evenWhenDead && targetingPlayer.Data.IsDead)
				return result;

			Vector2 truePosition = targetingPlayer.GetTruePosition();

			foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
			{
				if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead &&  !playerInfo.Role.IsImpostor)
				{
					PlayerControl @object = playerInfo.Object;

					if ((untargetablePlayers == null || !untargetablePlayers.Any((PlayerControl x) => x == @object)) && @object && (!@object.inVent || targetPlayersInVents))
					{
						Vector2 vector = @object.GetTruePosition() - truePosition;
						float magnitude = vector.magnitude;

						if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
						{
							result = @object;
							num = magnitude;
						}
					}
				}
			}

			return result;
		}

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }

                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
                {
                    ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                    ((Amnesiac)role).BodyArrows.Clear();
                }

                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Cannibal))
                {
                    ((Cannibal)role).BodyArrows.Values.DestroyAll();
                    ((Cannibal)role).BodyArrows.Clear();
                }

                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
                {
                    ((Medium)role).MediatedPlayers.Values.DestroyAll();
                    ((Medium)role).MediatedPlayers.Clear();
                }

                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Coroner))
                {
                    ((Coroner)role).BodyArrows.Values.DestroyAll();
                    ((Coroner)role).BodyArrows.Clear();
                }

                foreach (var role in Objectifier.AllObjectifiers.Where(x => x.ObjectifierType == ObjectifierEnum.Taskmaster))
                {
                    ((Taskmaster)role).ImpArrows.DestroyAll();
                    ((Taskmaster)role).TMArrows.Values.DestroyAll();
                    ((Taskmaster)role).TMArrows.Clear();
                }

                foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.Values.DestroyAll();
                    ((Snitch)role).SnitchArrows.Clear();
                }

                foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Radar))
                    ((Radar)role).RadarArrow.DestroyAll();

                RoleDictionary.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
                Objectifier.ObjectifierDictionary.Clear();
            }
        }
        
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    var role = GetRole(player);

                    if (role != null && role.Criteria())
                    {
                        bool selfFlag = role.SelfCriteria();
                        bool deadFlag = role.DeadCriteria();
                        bool factionFlag = role.FactionCriteria();
                        bool targetFlag = role.TargetCriteria();
                        bool roleFlag = role.RoleCriteria();
                        bool localFlag = role.Local;
                        bool loverrivalFlag1 = role.LoverRivalCriteria();
                        bool loverrivalFlag2 = role.LoverRivalRoleCriteria();

                        player.NameText.text = role.NameText(selfFlag || roleFlag || deadFlag || localFlag, selfFlag || deadFlag || roleFlag || factionFlag || targetFlag ||
                            loverrivalFlag2, selfFlag || deadFlag || loverrivalFlag1, player);

                        if (role.ColorCriteria())
                            player.NameText.color = role.Color;
                    }
                    else
                    {
                        try
                        {
                            player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                        } catch {}
                    }
                        
                    if (CustomGameOptions.PlayerNumbers)
                        player.NameText.text = $"[{player.TargetPlayerId}] " + player.NameText.text;
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (LobbyBehaviour.Instance)
                    return;
                    
                if (MeetingHud.Instance)
                    UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                    return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && ((player.Is(Faction.Intruder) && PlayerControl.LocalPlayer.Is(Faction.Intruder)) ||
                        (player.Is(Faction.Syndicate) && PlayerControl.LocalPlayer.Is(Faction.Syndicate)))))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = new Color32(255, 255, 255, 255);
                    }

                    var role = GetRole(player);

                    if (role != null && role.Criteria())
                    {
                        bool selfFlag = role.SelfCriteria();
                        bool deadFlag = role.DeadCriteria();
                        bool factionFlag = role.FactionCriteria();
                        bool targetFlag = role.TargetCriteria();
                        bool roleFlag = role.RoleCriteria();
                        bool localFlag = role.Local;
                        bool loverrivalFlag1 = role.LoverRivalCriteria();
                        bool loverrivalFlag2 = role.LoverRivalRoleCriteria();

                        player.nameText().text = role.NameText(selfFlag || roleFlag || deadFlag || localFlag, selfFlag || deadFlag || roleFlag || factionFlag || targetFlag ||
                            loverrivalFlag2, selfFlag || deadFlag || loverrivalFlag1);

                        if (role.ColorCriteria() && !LobbyBehaviour.Instance)
                            player.nameText().color = role.Color;
                    }
                }
            }
        }
    }
}