using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Tiebreaker : Ability
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = "Your vote breaks ties";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tiebreaker : Colors.Ability;
            AbilityType = AbilityEnum.Tiebreaker;
            Hidden = !CustomGameOptions.TiebreakerKnows;
        }
    }
}