namespace TownOfUs.Roles.Modifiers
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
            Name = "Diseased";
            TaskText = () => "Killing you gives Impostors a high cooldown";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Diseased;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Diseased;
        }
    }
}