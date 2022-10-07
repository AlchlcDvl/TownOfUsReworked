namespace TownOfUs.Roles.Modifiers
{
    public class ButtonBarry : Modifier
    {
        public KillButton ButtonButton;

        public bool ButtonUsed;

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = () => "Call a button from anywhere!";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.ButtonBarry;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.ButtonBarry;
        }
    }
}