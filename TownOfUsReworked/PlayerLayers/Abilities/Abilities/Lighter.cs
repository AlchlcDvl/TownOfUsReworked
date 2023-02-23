using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Lighter : Ability
    {
        public Lighter(PlayerControl player) : base(player)
        {
            Name = "Lighter";
            TaskText = "- You can see more than others.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Lighter : Colors.Ability;
            AbilityType = AbilityEnum.Lighter;
            AbilityDescription = "You are the Lighter! You can see more than normal people can see, use this to your advantage to scope out the arena!";
        }
    }
}