namespace TownOfUs.Roles
{
    public class Traitor : Role
    {
        public RoleEnum formerRole = new RoleEnum();
        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            ImpostorText = () => "Sussy";
            TaskText = () => "Betray the <color=#8BFDFDFF>Crew</color>";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Traitor;
            else Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Traitor;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }
    }
}