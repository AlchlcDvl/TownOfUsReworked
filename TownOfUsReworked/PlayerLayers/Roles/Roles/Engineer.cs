using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Engineer : Role
    {
        public bool EngiWin;
        public bool UsedThisRound = false;

        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => "Hop In, Hop Out, Daaaaaamn Sabo Fixed";
            TaskText = () => "Vent around and fix sabotages";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Engineer : Colors.Crew;
            RoleType = RoleEnum.Engineer;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = () => "Crew (Support)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The wrenches indicate that this body is an Engineer!";
            CoronerKillerReport = "";
            Results = InspResults.EngiAmneThiefCann;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}