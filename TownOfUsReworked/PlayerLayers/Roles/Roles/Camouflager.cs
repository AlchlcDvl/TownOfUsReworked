using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Camouflager : Role
    {
        private KillButton _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged { get; set; }
        public float TimeRemaining;
        public bool Camouflaged => TimeRemaining > 0f;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            StartText = "Disrupt Everyone's Vision To Hinder The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = "- You can disrupt everyone's vision, causing them to be unable to tell players apart.";
            AttributesText = "- When camouflaged, everyone will appear grey with no name or cosmetics.\n- If the Colorblind Meeting options is on, this effect " +
                "carries over into the meeting if a meeting is called during a camouflage.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Camouflager : Colors.Intruder;
            RoleType = RoleEnum.Camouflager;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Concealing)";
            IntroText = "Kill those who opposes you";
            CoronerDeadReport = "There is a strange device on the body that seems to be making the body appear grey. They must be a Camouflager!";
            CoronerKillerReport = "The body's appearance cannot be distinguised properly. They were killed by a Camouflager!";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            IntroSound = null;
            AlignmentDescription = "You are an Intruder (Concealing) role! It's your primary job to ensure no information incriminating you or your mates" + 
                " is revealed to the rest of the crew. Do as much as possible to ensure as little information is leaked.";
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            RoleDescription = "You are a Camouflager! You can choose to disrupt everyone's vision, causing them to be unable to recognise others. Use this to your " +
                "advantage and kill while unsuspected in front of everyone!";
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public override void Wins()
        {
            IntruderWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}