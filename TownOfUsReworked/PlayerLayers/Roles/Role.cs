using Hazel;
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
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using System;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    [HarmonyPatch]
    public class Role : PlayerLayer
    {
        public static readonly List<Role> AllRoles = new();
        public static readonly List<GameObject> Buttons = new();
        public static readonly Dictionary<int, string> LightDarkColors = new();
        public static readonly List<PlayerControl> Cleaned = new();

        public readonly List<Vent> Vents = new();

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
        public static PlayerControl DriveHolder;

        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        #pragma warning restore

        public Color32 FactionColor = Colors.Faction;
        public Color32 SubFactionColor = Colors.SubFaction;
        public Faction Faction = Faction.None;
        public Faction BaseFaction = Faction.None;
        public RoleAlignment RoleAlignment = RoleAlignment.None;
        public SubFaction SubFaction = SubFaction.None;
        public InspectorResults InspectorResults = InspectorResults.None;
        public List<Role> RoleHistory = new();

        public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
        public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

        public string IntroSound => $"{Name}Intro";
        public bool IntroPlayed;

        public string StartText = "Woah The Game Started";
        public string AbilitiesText = "- None";

        public string AlignmentName = "None";
        public string FactionName => $"{Faction}";
        public string SubFactionName => $"{SubFaction}";

        public string Objectives = "- None";

        public bool Bombed;
        public bool Diseased;

        public bool Base;

        public readonly List<Footprint> AllPrints = new();

        public CustomButton BombKillButton;

        public CustomButton ZoomInButton;
        public CustomButton ZoomOutButton;
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

        public PlayerControl ClosestBoom;

        public override void UpdateHud(HudManager __instance)
        {
            if (ConstantVariables.Inactive || LobbyBehaviour.Instance || MeetingHud.Instance)
                return;

            __instance.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.ShowMediumToDead && AllRoles.Any(x => x.RoleType == RoleEnum.Medium && ((Medium)x).MediatedPlayers.
                ContainsKey(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role2 = (Medium)AllRoles.Find(x => x.RoleType == RoleEnum.Medium && ((Medium)x).MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId));
                role2.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role2.Player.transform.position;
            }

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.ShowMediumToDead && AllRoles.Any(x => x.RoleType == RoleEnum.Retributionist && ((Retributionist)x).IsMed &&
                ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role2 = (Retributionist)AllRoles.Find(x => x.RoleType == RoleEnum.Retributionist && ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.
                    LocalPlayer.PlayerId));
                role2.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role2.Player.transform.position;
            }

            foreach (var ret in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.RevivedRole?.RoleType != RoleEnum.Medic)
                    continue;

                var exPlayer = ret.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    ret.ExShielded = null;
                    continue;
                }

                var player = ret.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || ret.Player.Data.IsDead || ret.Player.Data.Disconnected)
                {
                    Retributionist.BreakShield(ret.Player.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = (int)CustomGameOptions.ShowShielded;

                if (showShielded is 3 || (PlayerControl.LocalPlayer == player && showShielded is 0 or 2) || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                {
                    player.MyRend().material.SetColor("_VisorColor", new Color32(0, 255, 255, 255));
                    player.MyRend().material.SetFloat("_Outline", 1f);
                }
            }

            foreach (var medic in GetRoles<Medic>(RoleEnum.Medic))
            {
                var exPlayer = medic.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    medic.ExShielded = null;
                    continue;
                }

                var player = medic.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || medic.Player.Data.IsDead || medic.Player.Data.Disconnected)
                {
                    Medic.BreakShield(medic.Player.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = (int)CustomGameOptions.ShowShielded;

                if (showShielded is 3 || (PlayerControl.LocalPlayer == player && showShielded is 0 or 2) || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                {
                    player.MyRend().material.SetColor("_VisorColor", new Color32(0, 255, 255, 255));
                    player.MyRend().material.SetFloat("_Outline", 1f);
                }
            }

            foreach (var haunter in GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (PlayerControl.LocalPlayer.Data.IsDead || haunter.Caught || LobbyBehaviour.Instance || MeetingHud.Instance)
                {
                    haunter.RevealerArrows.DestroyAll();
                    haunter.RevealerArrows.Clear();
                    haunter.ImpArrows.DestroyAll();
                    haunter.ImpArrows.Clear();
                }

                foreach (var arrow in haunter.ImpArrows)
                    arrow.target = haunter.Player.transform.position;

                foreach (var (arrow, target) in Utils.Zip(haunter.RevealerArrows, haunter.RevealerTargets))
                {
                    if (target.Data.IsDead)
                    {
                        arrow?.Destroy();
                        arrow?.gameObject?.Destroy();
                    }

                    arrow.target = target.transform.position;
                }
            }

            if (Arrow != null)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead || Target.Data.IsDead || ConstantVariables.Inactive)
                {
                    Arrow.gameObject.Destroy();
                    Target = null;
                }
                else
                    Arrow.target = Target.transform.position;
            }

            foreach (var ga in GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
            {
                var player = ga.TargetPlayer;

                if (player == null)
                    continue;

                if (ga.Protecting)
                {
                    var showProtected = (int)CustomGameOptions.ShowProtect;

                    if (showProtected is 3 || (PlayerControl.LocalPlayer == player && showProtected is 0 or 2) || (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && showProtected is 1
                        or 2))
                    {
                        player.MyRend().material.SetColor("_VisorColor", new Color32(255, 217, 0, 255));
                        player.MyRend().material.SetFloat("_Outline", 1f);
                    }
                    else
                    {
                        player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.MyRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else if (ga.TargetPlayer.IsShielded())
                {
                    var showShielded = (int)CustomGameOptions.ShowShielded;

                    if (showShielded is 3 || (PlayerControl.LocalPlayer == player && showShielded is 0 or 2) || (PlayerControl.LocalPlayer.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                    {
                        player.MyRend().material.SetColor("_VisorColor", new Color32(0, 255, 255, 255));
                        player.MyRend().material.SetFloat("_Outline", 1f);
                    }
                    else
                    {
                        player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                        player.MyRend().material.SetFloat("_Outline", 0f);
                    }
                }
                else
                {
                    player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    player.MyRend().material.SetFloat("_Outline", 0f);
                }
            }

            base.UpdateHud(__instance);
            var dead = (!(Player.Is(RoleEnum.Revealer) && !GetRole<Revealer>(Player).Caught) || (Player.Is(RoleEnum.Ghoul) && !GetRole<Ghoul>(Player).Caught) ||
                (Player.Is(RoleEnum.Banshee) && !GetRole<Banshee>(Player).Caught) || (Player.Is(RoleEnum.Phantom) && !GetRole<Phantom>(Player).Caught)) && IsDead;
            ZoomInButton.Update("SPECTATE", 0, 1, true, Zooming && dead);
            ZoomOutButton.Update("SPECTATE", 0, 1, true, !Zooming && dead);
            BombKillButton.Update("KILL", 0, 1, true, Bombed && !dead);
        }

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerByVoteArea(voteArea);
            return player?.Data.Disconnected == true || !CustomGameOptions.LighterDarker;
        }

        public static void GenButton(PlayerVoteArea voteArea)
        {
            if (IsExempt(voteArea))
                return;

            var targetId = voteArea.TargetPlayerId;
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            var playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == targetId);
            var ColorString = LightDarkColors[playerControl.GetDefaultOutfit().ColorId];

            if (ColorString == "lighter")
                renderer.sprite = AssetManager.Lighter;
            else if (ColorString == "darker")
                renderer.sprite = AssetManager.Darker;

            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener((Action)(() => {}));
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            Buttons.Add(newButton);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            if (CustomGameOptions.LighterDarker)
            {
                Buttons.Clear();

                foreach (var voteArea in __instance.playerStates)
                    GenButton(voteArea);
            }

            foreach (var mayor in GetRoles<Mayor>(RoleEnum.Mayor))
            {
                mayor.ExtraVotes.Clear();

                if (mayor.VoteBank < 0)
                    mayor.VoteBank = 0;

                mayor.VoteBank++;
                mayor.SelfVote = false;
                mayor.VotedOnce = false;
            }

            foreach (var role in GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
            {
                if (!role.IsPol)
                    continue;

                role.ExtraVotes.Clear();

                if (role.VoteBank < 0)
                    role.VoteBank = 0;

                role.VotedOnce = false;

                if (role.HoldsDrive)
                    role.VoteBank += CustomGameOptions.ChaosDriveVoteAdd;
            }

            foreach (var role in GetRoles<Politician>(RoleEnum.Politician))
            {
                role.ExtraVotes.Clear();

                if (role.VoteBank < 0)
                    role.VoteBank = 0;

                role.VotedOnce = false;

                if (role.HoldsDrive)
                    role.VoteBank += CustomGameOptions.ChaosDriveVoteAdd;
            }
        }

        public static readonly string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>";
        public static readonly string IS = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>";
        public static readonly string IC = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>";
        public static readonly string ID = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>";
        public static readonly string IK = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string IU = "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>";

        public static readonly string SyndicateWinCon = (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds\n" : "") + "- Cause chaos and kill anyone who opposes " +
            "the <color=#008000FF>Syndicate</color>";
        public static readonly string SU = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Utility</color>)</color>";
        public static readonly string SSu = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Support</color>)</color>";
        public static readonly string SD = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Disruption</color>)</color>";
        public static readonly string SyK = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Killing</color>)</color>";
        public static readonly string SP = "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Power</color>)</color>";

        public static readonly string CrewWinCon = "- Finish all tasks\n- Eject all <color=#FF0000FF>evildoers</color>";
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
            Color = Colors.Layer;
            LayerType = PlayerLayerEnum.Role;
            ZoomInButton = new(this, AssetManager.Plus, AbilityTypes.Effect, "ActionSecondary", Zoom, false, true);
            ZoomOutButton = new(this, AssetManager.Minus, AbilityTypes.Effect, "ActionSecondary", Zoom, false, true);
            BombKillButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", BombKill);
            AllRoles.Add(this);
        }

        public void Zoom()
        {
            Zooming = !Zooming;
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
        }

        public void BombKill()
        {
            if (Utils.IsTooFar(Player, ClosestBoom))
                return;

            if (!Bombed)
                return;

            var success = Utils.Interact(Player, ClosestBoom, true)[3];
            Player.GetEnforcer().BombSuccessful = success;
            Bombed = false;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.ForceKill);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(success);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

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

        public static Role GetRoleFromName(string name) => AllRoles.Find(x => x.Name == name);

        public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

        public static Role GetRole(PlayerVoteArea area) => GetRole(Utils.PlayerByVoteArea(area));

        public static List<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.RoleType == roletype).ToList();

        public static List<T> GetRoles<T>(RoleEnum roletype) where T : Role => GetRoles(roletype).Cast<T>().ToList();

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
                    if (player.Is(Faction.Crew) && player.CanDoTasks())
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
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);

                    if (CustomGameOptions.AltImps)
                    {
                        writer.Write((byte)WinLoseRPC.SyndicateWin);
                        SyndicateWin = true;
                    }
                    else
                    {
                        writer.Write((byte)WinLoseRPC.IntruderWin);
                        IntruderWin = true;
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
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

                if (ConstantVariables.IsHnS)
                    return;

                var player = __instance.__4__this;
                player.RegenTask();
            }
        }
    }
}