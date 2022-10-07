namespace TownOfUs.Roles.Modifiers
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "Inverrrrrted contrrrrols";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Drunk;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Drunk;
        }
    }
}