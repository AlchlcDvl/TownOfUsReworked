namespace TownOfUs.Roles.Modifiers
{
    public class Tiebreaker : Modifier
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = () => "Your vote breaks ties";
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Tiebreaker;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Tiebreaker;
        }
    }
}