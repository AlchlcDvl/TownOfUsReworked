using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Inspector : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }
        public bool CrewmateWin;
        public List<PlayerControl> Examined = new List<PlayerControl>();

        public Inspector(PlayerControl player) : base(player)
        {
            Name = "Inspector";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Inspector;
            ImpostorText = () => "Inspect Player For Their Roles";
            TaskText = () => "See if people really are what they claim to be";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
            SubFaction = SubFaction.None;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "There are documents pertaining to everyone's identities on the body. They must be an Inspector!";
            CoronerKillerReport = "";
            Results = InspResults.SherConsigInspBm;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.InspectCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;
                
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}