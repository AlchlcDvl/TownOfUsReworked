using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Tiebreaker : Ability
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = () => "Your vote breaks ties";
            if (CustomGameOptions.CustomAbilityColors) Color = Colors.Tiebreaker;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Tiebreaker;
            AddToAbilityHistory(AbilityType);
        }
    }
}