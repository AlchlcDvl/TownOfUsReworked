namespace TownOfUs.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => "Hop In, Hop Out, Daaaaaamn Sabo Fixed";
            TaskText = () => "Vent around and fix sabotages";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Engineer;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Engineer;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSupport;
            AlignmentName = "Crew (Support)";
            AddToRoleHistory(RoleType);
        }

        public bool UsedThisRound { get; set; } = false;
    }
}