using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Crewmate : Role
    {
        public bool CrewmateWin;

        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Crewmate;
            ImpostorText = () => "Imagine Being Boring Crewmate";
            TaskText = () => "Imagine Being Boring Crewmate";
            Color = Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewUtil;
            AlignmentName = () => "Crew (Utility)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.CrewImpAnMurd;
            SubFaction = SubFaction.None;
            IntroSound = TownOfUsReworked.CrewmateIntro;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}