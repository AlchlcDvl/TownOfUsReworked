namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;

        public bool ReviveUsed;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => "Sacrifice Yourself To Save Another";
            TaskText = () => "You can revive a dead body at the cost of your own life";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Altruist;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Altruist;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSupport;
            AlignmentName = "Crew (Support)";
            AddToRoleHistory(RoleType);
        }
    }
}