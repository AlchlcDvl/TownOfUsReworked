using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Objects;
using Reactor.Utilities.Extensions;
using System.Reflection;
using TMPro;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Retributionist : Role
    {
        public Retributionist(PlayerControl player) : base(player)
        {
            Name = "Retributionist";
            StartText = "Mimic the Dead";
            AbilitiesText = "- You can mimic the abilities of dead selective <color=#8BFDFDFF>Crew</color>.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Retributionist : Colors.Crew;
            RoleType = RoleEnum.Retributionist;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            RoleDescription = "Your are a Retributionist! You are a sorcerer who can mimic the skills of the true-hearted dead! Use your plethora of abilities to find all evildoers!";
            Objectives = CrewWinCon;
            InspectorResults = InspectorResults.DealsWithDead;
            Inspected = new List<byte>();
            BodyArrows = new Dictionary<byte, ArrowBehaviour>();
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
            Bugs = new List<Bug>();
            TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
            OtherButtons = new List<GameObject>();
            ListOfActives = new List<bool>();
            Interrogated = new List<byte>();
        }

        //Retributionist Stuff
        public readonly List<GameObject> OtherButtons;
        public readonly List<bool> ListOfActives;
        public Role RevivedRole;
        public PlayerControl Revived;
        public PlayerControl ClosestPlayer;

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsIntTraitor || IsIntFanatic) && Utils.IntrudersWin())
            {
                IntruderWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsSynTraitor || IsSynFanatic) && Utils.SyndicateWins())
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                SectWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                UndeadWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.CrewWins() && NotDefective)
            {
                CrewWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        //Coroner Stuff
        public Dictionary<byte, ArrowBehaviour> BodyArrows;
        private KillButton _autopsyButton;
        private KillButton _compareButton;

        public void DestroyCoronerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);
                
            BodyArrows.Remove(arrow.Key);
        }

        public KillButton AutopsyButton
        {
            get => _autopsyButton;
            set
            {
                _autopsyButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public KillButton CompareButton
        {
            get => _compareButton;
            set
            {
                _compareButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        //Detective Stuff
        public DateTime LastExamined { get; set; }
        private KillButton _examineButton;

        public KillButton ExamineButton
        {
            get => _examineButton;
            set
            {
                _examineButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;
                
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Inspector Stuff
        public DateTime LastInspected { get; set; }
        public List<byte> Inspected;
        private KillButton _inspectButton;

        public KillButton InspectButton
        {
            get => _inspectButton;
            set
            {
                _inspectButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInspected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;
                
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Medium Stuff
        public DateTime LastMediated { get; set; }
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers;
        public static Sprite Arrow => TownOfUsReworked.Arrow;
        private KillButton _mediateButton;

        public KillButton MediateButton
        {
            get => _mediateButton;
            set
            {
                _mediateButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMediated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new UnityEngine.GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
            }
            
            MediatedPlayers.Add(playerId, arrow);
            Coroutines.Start(Utils.FlashCoroutine(Color));
        }

        //Operative Stuff
        public static AssetBundle Bundle = LoadBundle();
        public static Material BugMaterial = Bundle.LoadAsset<Material>("trap").DontUnload();
        public List<Bug> Bugs;
        public DateTime LastBugged { get; set; }
        public List<RoleEnum> BuggedPlayers;
        private KillButton _bugButton;
        public int BugUsesLeft;
        public TextMeshPro BugUsesText;
        public bool BugButtonUsable => BugUsesLeft != 0 && RevivedRole?.RoleType == RoleEnum.Operative;

        public KillButton BugButton
        {
            get => _bugButton;
            set
            {
                _bugButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBugged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public static AssetBundle LoadBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("TownOfUsReworked.Resources.Sounds.operativeshader");
            var assets = stream.ReadFully();
            return AssetBundle.LoadFromMemory(assets);
        }

        //Sheriff Stuff
        public List<byte> Interrogated;
        private KillButton _interrogateButton;
        public DateTime LastInterrogated { get; set; }

        public KillButton InterrogateButton
        {
            get => _interrogateButton;
            set
            {
                _interrogateButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInterrogated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Tracker Stuff
        public Dictionary<byte, ArrowBehaviour> TrackerArrows;
        public DateTime LastTracked { get; set; }
        private KillButton _trackButton;
        public int TrackUsesLeft;
        public TextMeshPro TrackUsesText;
        public bool TrackButtonUsable => TrackUsesLeft != 0 && RevivedRole?.RoleType == RoleEnum.Tracker;

        public KillButton TrackButton
        {
            get => _trackButton;
            set
            {
                _trackButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }
        
        public void DestroyTrackerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);
                
            TrackerArrows.Remove(arrow.Key);
        }

        //Vigilante Stuff
        public DateTime LastKilled { get; set; }
        private KillButton _shootButton;

        public KillButton ShootButton
        {
            get => _shootButton;
            set
            {
                _shootButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Vampire Hunter Stuff
        public DateTime LastStaked { get; set; }
        private KillButton _stakeButton;
        public bool VampsDead => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && x.Is(SubFaction.Undead)) == 0;

        public KillButton StakeButton
        {
            get => _stakeButton;
            set
            {
                _stakeButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStaked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        //Veteran Stuff
        public bool AlertEnabled;
        public DateTime LastAlerted;
        public float AlertTimeRemaining;
        public bool OnAlert => AlertTimeRemaining > 0f;
        private KillButton _alertButton;
        public int AlertUsesLeft;
        public TextMeshPro AlertUsesText;
        public bool AlertButtonUsable => AlertUsesLeft != 0 && RevivedRole?.RoleType == RoleEnum.Veteran;

        public KillButton AlertButton
        {
            get => _alertButton;
            set
            {
                _alertButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            AlertEnabled = true;
            AlertTimeRemaining -= Time.deltaTime;
        }

        public void UnAlert()
        {
            AlertEnabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        //Altruist Stuff
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public bool ReviveUsed = false;
        private KillButton _reviveButton;

        public KillButton ReviveButton
        {
            get => _reviveButton;
            set
            {
                _reviveButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        //Medic Stuff
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }
        private KillButton _shieldButton;

        public KillButton ShieldButton
        {
            get => _shieldButton;
            set
            {
                _shieldButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        //Time Lord Stuff
        public DateTime StartRewind { get; set; }
        public DateTime FinishRewind { get; set; }
        private KillButton _rewindButton;
        public int RewindUsesLeft;
        public TextMeshPro RewindUsesText;
        public bool RewindButtonUsable => RewindUsesLeft != 0 && RevivedRole?.RoleType == RoleEnum.TimeLord;

        public KillButton RewindButton
        {
            get => _rewindButton;
            set
            {
                _rewindButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;
            TimeSpan timespan;
            float num;

            if (RecordRewind.rewinding)
            {
                timespan = utcNow - StartRewind;
                num = CustomGameOptions.RewindDuration * 1000f / 3f;
            }
            else
            {
                timespan = utcNow - FinishRewind;
                num = Utils.GetModifiedCooldown(CustomGameOptions.RewindCooldown) * 1000f;
            }

            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float GetCooldown()
        {
            return RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;
        }

        //Chameleon Stuff
        public bool SwoopEnabled;
        public DateTime LastSwooped;
        public float SwoopTimeRemaining;
        public bool IsSwooped => SwoopTimeRemaining > 0f;
        private KillButton _swoopButton;

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSwooped;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InvisCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            SwoopEnabled = true;
            SwoopTimeRemaining -= Time.deltaTime;
            
            if (Player.Data.IsDead)
                SwoopTimeRemaining = 0f;

            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Data.IsDead)
                color.a = 26;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });

                Player.myRend().color = color;
                Player.nameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public KillButton SwoopButton
        {
            get => _swoopButton;
            set
            {
                _swoopButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Uninvis()
        {
            SwoopEnabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.myRend().color = new Color32(255, 255, 255, 255);
        }

        //Engineer Stuff
        private KillButton _fixButton;
        public int FixUsesLeft;
        public TextMeshPro FixUsesText;
        public bool FixButtonUsable => RewindUsesLeft != 0 && RevivedRole?.RoleType == RoleEnum.Engineer;

        public KillButton FixButton
        {
            get => _fixButton;
            set
            {
                _fixButton = value;
                AddToAbilityButtons(value, this);
            }
        }
    }
}