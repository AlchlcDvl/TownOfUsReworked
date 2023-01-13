using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Tunneler : Ability
    {
        public Tunneler(PlayerControl player) : base(player)
        {
            Name = "Tunneler";
            TaskText = "You can dig yourself into the ground";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tunneler : Colors.Ability;
            AbilityType = AbilityEnum.Tunneler;
            Hidden = !CustomGameOptions.TunnelerKnows;
        }
    }
}