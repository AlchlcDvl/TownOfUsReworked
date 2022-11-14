using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        public bool InvWin;

        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Examine Footprints To Find The <color=#FF0000FF>Intruders</color>";
            TaskText = () => "You can see everyone's footprints";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Investigator : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Investigator;
            Scale = 1.4f;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The body has documentated footprints of everyone. They must be an Investigator!";
            CoronerKillerReport = "";
            Results = InspResults.JestJuggWWInv;
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