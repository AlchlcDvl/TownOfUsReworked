namespace TownOfUs.Roles.Modifiers
{
    public class Bait : Modifier
    {
        public Bait(PlayerControl player) : base(player)
        {
            Name = "Bait";
            TaskText = () => "Killing you causes an instant self-report";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Bait;
            else Color = Patches.Colors.Modifier;
            Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Bait;
            //AddToModifierHistory(ModifierHistory);
        }
    }
}