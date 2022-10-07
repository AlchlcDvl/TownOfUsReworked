namespace TownOfUs.Roles
{
    public class Janitor : Role
    {
        public KillButton _cleanButton;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => "Sanitise The Ship, By Any Means Neccessary";
            TaskText = () => "Clean bodies to prevent the <color=#8BFDFDFF>Crew</color> from discovering them";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Janitor;
            else Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Janitor;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderConceal;
            AlignmentName = "Intruder (Concealing)";
            AddToRoleHistory(RoleType);
        }

        public DeadBody CurrentTarget { get; set; }

        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}