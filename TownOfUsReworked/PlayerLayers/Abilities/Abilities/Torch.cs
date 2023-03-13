using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Torch : Ability
    {
        public Torch(PlayerControl player) : base(player)
        {
            Name = "Torch";
            TaskText = "- You can see in the dark.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Torch : Colors.Ability;
            AbilityType = AbilityEnum.Torch;
            AbilityDescription = "You are a Torch! You see more than anyone can!";
        }
    }
}