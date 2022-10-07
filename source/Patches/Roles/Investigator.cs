using System.Collections.Generic;
using TownOfUs.CrewmateRoles.InvestigatorMod;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();


        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Examine Footprints To Find The <color=#FF0000FF>Intruders</color>";
            TaskText = () => "You can see everyone's footprints";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Investigator;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Investigator;
            Scale = 1.4f;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }
    }
}