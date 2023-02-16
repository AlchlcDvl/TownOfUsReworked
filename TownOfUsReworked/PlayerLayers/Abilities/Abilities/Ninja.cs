using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Ninja : Ability
    {
        public Ninja(PlayerControl player) : base(player)
        {
            Name = "Ninja";
            TaskText = "You do not lunge";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Ninja : Colors.Ability;
            AbilityType = AbilityEnum.Ninja;
        }
    }
}