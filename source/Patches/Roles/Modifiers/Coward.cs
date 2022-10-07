namespace TownOfUs.Roles.Modifiers
{
    public class Coward : Modifier
    {
        public Coward(PlayerControl player) : base(player)
        {
            Name = "Coward";
            TaskText = () => "You are too afraid to report bodies";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Coward;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Coward;
        }
    }
}