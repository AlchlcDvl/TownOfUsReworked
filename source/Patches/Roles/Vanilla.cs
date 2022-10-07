namespace TownOfUs.Roles
{
    public class Impostor : Role
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            Hidden = true;
            Faction = Faction.Intruders;
            RoleType = RoleEnum.Impostor;
            ImpostorText = () => "Imagine Being Boring Impostor";
            TaskText = () => "Imagine Being Boring Impostor";
            Color = Patches.Colors.Impostor;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderUtil;
            AlignmentName = "Intruder (Utility)";
            AddToRoleHistory(RoleType);
        }
    }

    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            Hidden = true;
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Crewmate;
            ImpostorText = () => "Imagine Being Boring Crewmate";
            TaskText = () => "Imagine Being Boring Crewmate";
            Color = Patches.Colors.Crew;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewUtil;
            AlignmentName = "Crew (Utility)";
            AddToRoleHistory(RoleType);
        }
    }
}