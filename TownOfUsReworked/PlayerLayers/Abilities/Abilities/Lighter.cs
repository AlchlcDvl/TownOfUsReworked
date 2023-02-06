using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Lighter : Ability
    {
        public Lighter(PlayerControl player) : base(player)
        {
            Name = "Lighter";
            TaskText = "You can see more than others";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Lighter : Colors.Ability;
            AbilityType = AbilityEnum.Lighter;
        }
    }
}