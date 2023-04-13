using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Objects;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using Object = UnityEngine.Object;
using System.Collections;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Retributionist : CrewRole
    {
        public Retributionist(PlayerControl player) : base(player)
        {
            Name = "Retributionist";
            StartText = "Mimic the Dead";
            AbilitiesText = "- You can mimic the abilities of dead selective <color=#8BFDFDFF>Crew</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Retributionist : Colors.Crew;
            RoleType = RoleEnum.Retributionist;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.DealsWithDead;
            Inspected = new();
            BodyArrows = new();
            MediatedPlayers = new();
            Bugs = new();
            TrackerArrows = new();
            OtherButtons = new();
            ListOfActives = new();
            UntransportablePlayers = new();
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            TransportMenu1 = new CustomMenu(Player, new CustomMenu.Select(Click1));
            TransportMenu2 = new CustomMenu(Player, new CustomMenu.Select(Click2));
            CompareUsesLeft = 0;
            TrackUsesLeft = CustomGameOptions.MaxTracks;
            BulletsLeft = CustomGameOptions.VigiBulletCount;
            AlertUsesLeft = CustomGameOptions.MaxAlerts;
            SwoopUsesLeft = CustomGameOptions.SwoopCount;
            TransportUsesLeft = CustomGameOptions.TransportMaxUses;
            Interrogated = new();
        }

        //Retributionist Stuff
        public readonly List<GameObject> OtherButtons = new();
        public readonly List<bool> ListOfActives = new();
        public Role RevivedRole;
        public PlayerControl Revived;
        public PlayerControl ClosestPlayer;
        public DeadBody CurrentTarget;

        public override void OnLobby()
        {
            BodyArrows.Values.DestroyAll();
            BodyArrows.Clear();

            MediatedPlayers.Values.DestroyAll();
            MediatedPlayers.Clear();

            TrackerArrows.Values.DestroyAll();
            TrackerArrows.Clear();
        }

        //Coroner Stuff
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public AbilityButton AutopsyButton;
        public AbilityButton CompareButton;
        public DateTime LastAutopsied;
        public int CompareUsesLeft;
        public bool CompareButtonUsable => CompareUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Coroner && ReferenceBody != null;
        public DeadPlayer ReferenceBody;
        public DateTime LastCompared;
        public bool IsCor => RevivedRole?.RoleType == RoleEnum.Coroner;

        public void DestroyCoronerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCompared;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float AutopsyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAutopsied;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AutopsyCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Detective Stuff
        public DateTime LastExamined;
        public AbilityButton ExamineButton;
        public bool IsDet => RevivedRole?.RoleType == RoleEnum.Detective;

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastExamined;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Inspector Stuff
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public AbilityButton InspectButton;
        public bool IsInsp => RevivedRole?.RoleType == RoleEnum.Inspector;

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInspected;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Medium Stuff
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new();
        public AbilityButton MediateButton;
        public bool IsMed => RevivedRole?.RoleType == RoleEnum.Medium;

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMediated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player == PlayerControl.LocalPlayer || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
                Utils.Flash(Colors.Medium, "There's a mediate in progress!");
            }

            MediatedPlayers.Add(playerId, arrow);
        }

        //Operative Stuff
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public List<RoleEnum> BuggedPlayers = new();
        public AbilityButton BugButton;
        public int BugUsesLeft;
        public bool BugButtonUsable => BugUsesLeft > 0;
        public bool IsOP => RevivedRole?.RoleType == RoleEnum.Operative;

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBugged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Sheriff Stuff
        public List<byte> Interrogated = new();
        public AbilityButton InterrogateButton;
        public DateTime LastInterrogated;
        public bool IsSher => RevivedRole?.RoleType == RoleEnum.Sheriff;

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInterrogated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Tracker Stuff
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new();
        public DateTime LastTracked;
        public AbilityButton TrackButton;
        public int TrackUsesLeft;
        public bool TrackButtonUsable => TrackUsesLeft > 0;
        public bool IsTrack => RevivedRole?.RoleType == RoleEnum.Tracker;

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTracked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

        public void DestroyTrackerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            TrackerArrows.Remove(arrow.Key);
        }

        //Vigilante Stuff
        public DateTime LastKilled;
        public AbilityButton ShootButton;
        public int BulletsLeft;
        public bool IsVig => RevivedRole?.RoleType == RoleEnum.Vigilante;
        public bool ShootButtonUsable => BulletsLeft > 0;

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Vampire Hunter Stuff
        public DateTime LastStaked;
        public AbilityButton StakeButton;
        public bool IsVH => RevivedRole?.RoleType == RoleEnum.VampireHunter;

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStaked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Veteran Stuff
        public bool AlertEnabled;
        public DateTime LastAlerted;
        public float AlertTimeRemaining;
        public bool OnAlert => AlertTimeRemaining > 0f;
        public AbilityButton AlertButton;
        public int AlertUsesLeft;
        public bool IsVet => RevivedRole?.RoleType == RoleEnum.Veteran;
        public bool AlertButtonUsable => AlertUsesLeft > 0;

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAlerted;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            AlertEnabled = true;
            AlertTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                AlertTimeRemaining = 0f;
        }

        public void UnAlert()
        {
            AlertEnabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        //Altruist Stuff
        public bool CurrentlyReviving;
        public bool ReviveUsed;
        public AbilityButton ReviveButton;
        public bool IsAlt => RevivedRole?.RoleType == RoleEnum.Altruist;

        //Medic Stuff
        public bool UsedMedicAbility => ShieldedPlayer != null || ExShielded != null;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public AbilityButton ShieldButton;
        public bool IsMedic => RevivedRole?.RoleType == RoleEnum.Medic;

        //Chameleon Stuff
        public bool SwoopEnabled;
        public DateTime LastSwooped;
        public float SwoopTimeRemaining;
        public bool IsSwooped => SwoopTimeRemaining > 0f;
        public AbilityButton SwoopButton;
        public int SwoopUsesLeft;
        public bool IsCham => RevivedRole?.RoleType == RoleEnum.Chameleon;
        public bool SwoopButtonUsable => SwoopUsesLeft > 0;

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSwooped;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InvisCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            SwoopEnabled = true;
            SwoopTimeRemaining -= Time.deltaTime;
            Utils.Invis(Player);

            if (MeetingHud.Instance || Player.Data.IsDead)
                SwoopTimeRemaining = 0f;
        }

        public void Uninvis()
        {
            SwoopEnabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }

        //Engineer Stuff
        public AbilityButton FixButton;
        public int FixUsesLeft;
        public bool FixButtonUsable => FixUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Engineer;
        public DateTime LastFixed;
        public bool IsEngi => RevivedRole?.RoleType == RoleEnum.Engineer;

        public float FixTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFixed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Mystic Stuff
        public DateTime LastRevealed;
        public AbilityButton RevealButton;
        public bool IsMys => RevivedRole?.RoleType == RoleEnum.Mystic;

        public float RevealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRevealed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Seer Stuff
        public DateTime LastSeered;
        public AbilityButton SeerButton;
        public bool IsSeer => RevivedRole?.RoleType == RoleEnum.Seer;

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSeered;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Escort Stuff
        public PlayerControl BlockTarget;
        public bool BlockEnabled;
        public DateTime LastBlock;
        public float BlockTimeRemaining;
        public AbilityButton BlockButton;
        public bool Blocking => BlockTimeRemaining > 0f;
        public bool IsEsc => RevivedRole?.RoleType == RoleEnum.Escort;

        public void UnBlock()
        {
            BlockEnabled = false;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
        }

        public void Block()
        {
            BlockEnabled = true;
            BlockTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || !BlockTarget.IsBlocked())
                BlockTimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlock;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.EscRoleblockCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Transporter Stuff
        public DateTime LastTransported;
        public PlayerControl TransportPlayer1;
        public PlayerControl TransportPlayer2;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public Dictionary<byte, DateTime> UntransportablePlayers = new();
        public AbilityButton TransportButton;
        public CustomMenu TransportMenu1;
        public CustomMenu TransportMenu2;
        public int TransportUsesLeft;
        public bool TransportButtonUsable => TransportUsesLeft > 0;
        public bool IsTrans => RevivedRole?.RoleType == RoleEnum.Transporter;

        public float TransportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTransported;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TransportCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator TransportPlayers()
        {
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            bool WasInVent1 = false;
            bool WasInVent2 = false;
            Vent Vent1 = null;
            Vent Vent2 = null;

            if (TransportPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(TransportPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (TransportPlayer2.Data.IsDead)
            {
                Player2Body = Utils.BodyById(TransportPlayer2.PlayerId);

                if (Player2Body == null)
                    yield break;
            }

            if (TransportPlayer1.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer1.MyPhysics.ExitAllVents();
                Vent1 = CustomButtons.GetClosestVent(TransportPlayer1);
                WasInVent1 = true;
            }

            if (TransportPlayer2.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer2.MyPhysics.ExitAllVents();
                Vent2 = CustomButtons.GetClosestVent(TransportPlayer2);
                WasInVent2 = true;
            }

            if (Player1Body == null && Player2Body == null)
            {
                TransportPlayer1.MyPhysics.ResetMoveState();
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                var TempFacing = TransportPlayer1.MyRend().flipX;
                TransportPlayer1.NetTransform.SnapTo(new Vector2(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
                TransportPlayer1.MyRend().flipX = TransportPlayer2.MyRend().flipX;
                TransportPlayer2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                TransportPlayer2.MyRend().flipX = TempFacing;

                if (SubmergedCompatibility.IsSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TransportPlayer1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                    if (PlayerControl.LocalPlayer.PlayerId == TransportPlayer2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }

                if (TransportPlayer1.CanVent(TransportPlayer1.Data) && Vent2 != null && WasInVent2)
                    TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

                if (TransportPlayer2.CanVent(TransportPlayer1.Data) && Vent1 != null && WasInVent1)
                    TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TransportPlayer2.GetTruePosition();
                TransportPlayer2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TransportPlayer2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                Utils.StopDragging(Player2Body.ParentId);
                TransportPlayer1.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TransportPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Utils.StopDragging(Player2Body.ParentId);
                (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
            }

            if (PlayerControl.LocalPlayer == TransportPlayer1 || PlayerControl.LocalPlayer == TransportPlayer2)
            {
                Utils.Flash(Colors.Transporter, "You were transported to a different location!");

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TransportPlayer1.moveable = true;
            TransportPlayer2.moveable = true;
            TransportPlayer1.Collider.enabled = true;
            TransportPlayer2.Collider.enabled = true;
            TransportPlayer1.NetTransform.enabled = true;
            TransportPlayer2.NetTransform.enabled = true;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
        }

        public void Click1(PlayerControl player)
        {
            Utils.Interact(Player, player);
            TransportPlayer1 = player;
            Utils.LogSomething($"1 - {TransportPlayer1.name}");
        }

        public void Click2(PlayerControl player)
        {
            Utils.Interact(Player, player);
            TransportPlayer2 = player;
            Utils.LogSomething($"2 - {TransportPlayer2.name}");
        }
    }
}