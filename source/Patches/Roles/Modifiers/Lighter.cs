namespace TownOfUs.Roles.Modifiers
{
    public class Lighter : Modifier
    {
        public Lighter(PlayerControl player) : base(player)
        {
            Name = "Lighter";
            TaskText = () => "You can see more than others";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Lighter;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Lighter;
        }
    }
}