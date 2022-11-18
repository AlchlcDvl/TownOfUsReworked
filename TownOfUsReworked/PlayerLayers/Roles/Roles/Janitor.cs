using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Janitor : Role
    {
        public KillButton _cleanButton;
        public DeadBody CurrentTarget { get; set; }
        public DateTime LastCleaned;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => "Sanitise The Ship, By Any Means Neccessary";
            TaskText = () => "Clean bodies to prevent the <color=#8BFDFDFF>Crew</color> from discovering them";
            Color = CustomGameOptions.CustomImpColors ? Colors.Janitor : Colors.Intruder;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Janitor;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = () => "Intruder (Concealing)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = "The smell of chlorine indicates that the body is an Janitor!";
            CoronerKillerReport = "The body has a sharp sense of chlorine. They were killed by a Janitor!";
            Results = InspResults.CoroJaniUTMed;
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
    }
}