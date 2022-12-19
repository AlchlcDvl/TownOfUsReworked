using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;
using System;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Janitor : Role
    {
        private KillButton _cleanButton;
        public DeadBody CurrentTarget { get; set; }
        public DateTime LastCleaned;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            StartText = "Sanitise The Ship, By Any Means Neccessary";
            AbilitiesText = "Clean bodies to prevent the <color=#8BFDFDFF>Crew</color> from discovering them";
            Color = CustomGameOptions.CustomImpColors ? Colors.Janitor : Colors.Intruder;
            SubFaction = SubFaction.None;
            Base = false;
            IsRecruit = false;
            RoleType = RoleEnum.Janitor;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = "Intruder (Concealing)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = "The smell of chlorine indicates that the body is an Janitor!";
            CoronerKillerReport = "The body has a sharp sense of chlorine. They were killed by a Janitor!";
            Results = InspResults.CoroJaniUTMed;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            AddToRoleHistory(RoleType);
        }

        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCleaned;
            var num = (Utils.LastImp() ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
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