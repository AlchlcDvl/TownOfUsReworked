namespace TownOfUs.Roles
{
    public class Agent : Role
    {
        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            ImpostorText = () => "Snoop Around And Find Stuff Out";
            TaskText = () => "Gain extra information on the Admin Table";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Agent;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }
    }
}