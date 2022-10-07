namespace TownOfUs.Roles.Modifiers
{
    public class Torch : Modifier
    {
        public Torch(PlayerControl player) : base(player)
        {
            Name = "Torch";
            TaskText = () => "You can see in the dark";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Torch;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Torch;
        }
    }
}