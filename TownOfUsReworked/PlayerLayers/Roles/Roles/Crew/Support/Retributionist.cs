using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Objects;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Retributionist : CrewRole
    {
        public Retributionist(PlayerControl player) : base(player)
        {
            Name = "Retributionist";
            StartText = "Mimic the Dead";
            AbilitiesText = "- You can mimic the abilities of dead selective <color=#8BFDFDFF>Crew</color>.";
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
            Interrogated = new();
            Used = new();
        }

        //Retributionist Stuff
        public readonly List<GameObject> OtherButtons = new();
        public readonly List<bool> ListOfActives = new();
        public Role RevivedRole;
        public PlayerControl Revived;
        public PlayerControl ClosestPlayer;
        public List<byte> Used = new();

        //Coroner Stuff
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public AbilityButton AutopsyButton;
        public AbilityButton CompareButton;
        public DateTime LastAutopsied;
        public int CompareUsesLeft;
        public bool CompareButtonUsable => CompareUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Coroner && ReferenceBody != null;
        public DeadPlayer ReferenceBody;
        public DateTime LastCompared;

        public void DestroyCoronerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                UnityEngine.Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                UnityEngine.Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCompared;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float AutopsyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAutopsied;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AutopsyCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Detective Stuff
        public DateTime LastExamined;
        public AbilityButton ExamineButton;

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastExamined;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Inspector Stuff
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public AbilityButton InspectButton;

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInspected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Medium Stuff
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new();
        public AbilityButton MediateButton;

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMediated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
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
            }

            MediatedPlayers.Add(playerId, arrow);
            Coroutines.Start(Utils.FlashCoroutine(Color));
        }

        //Operative Stuff
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public List<RoleEnum> BuggedPlayers = new();
        public AbilityButton BugButton;
        public int BugUsesLeft;
        public bool BugButtonUsable => BugUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Operative;

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBugged;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Sheriff Stuff
        public List<byte> Interrogated = new();
        public AbilityButton InterrogateButton;
        public DateTime LastInterrogated;

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInterrogated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Tracker Stuff
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new();
        public DateTime LastTracked;
        public AbilityButton TrackButton;
        public int TrackUsesLeft;
        public bool TrackButtonUsable => TrackUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Tracker;

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTracked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

        public void DestroyTrackerArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                UnityEngine.Object.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                UnityEngine.Object.Destroy(arrow.Value.gameObject);

            TrackerArrows.Remove(arrow.Key);
        }

        //Vigilante Stuff
        public DateTime LastKilled;
        public AbilityButton ShootButton;
        public int BulletsLeft;
        public bool ShootButtonUsable => BulletsLeft > 0 && RevivedRole?.RoleType == RoleEnum.Vigilante;

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Vampire Hunter Stuff
        public DateTime LastStaked;
        public AbilityButton StakeButton;

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStaked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
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
        public bool AlertButtonUsable => AlertUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Veteran;

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAlerted;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
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
        public DeadBody CurrentTarget;
        public bool ReviveUsed;
        public AbilityButton ReviveButton;

        //Medic Stuff
        public bool UsedAbility;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public AbilityButton ShieldButton;

        //Chameleon Stuff
        public bool SwoopEnabled;
        public DateTime LastSwooped;
        public float SwoopTimeRemaining;
        public bool IsSwooped => SwoopTimeRemaining > 0f;
        public AbilityButton SwoopButton;

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSwooped;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InvisCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            SwoopEnabled = true;
            SwoopTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead)
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

                Player.MyRend().color = color;
                Player.NameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public void Uninvis()
        {
            SwoopEnabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.MyRend().color = new Color32(255, 255, 255, 255);
        }

        //Engineer Stuff
        public AbilityButton FixButton;
        public int FixUsesLeft;
        public bool FixButtonUsable => FixUsesLeft > 0 && RevivedRole?.RoleType == RoleEnum.Engineer;
        public DateTime LastFixed;

        public float FixTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFixed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Mystic Stuff
        public DateTime LastRevealed;
        public AbilityButton RevealButton;

        public float RevealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRevealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        //Seer Stuff
        public DateTime LastSeered;
        public AbilityButton SeerButton;

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSeered;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}