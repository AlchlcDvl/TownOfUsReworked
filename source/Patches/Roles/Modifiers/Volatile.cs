namespace TownOfUs.Roles.Modifiers
{
    public class Volatile : Modifier
    {
        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = () => "You might see/hear things and lash out.";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Volatile;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Volatile;
        }
    }
}