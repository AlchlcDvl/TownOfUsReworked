using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Tiebreaker : Ability
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = "- Your votes break ties.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tiebreaker : Colors.Ability;
            AbilityType = AbilityEnum.Tiebreaker;
            Hidden = !CustomGameOptions.TiebreakerKnows;
            AbilityDescription = "You are the tiebreaker! Your vote is what breaks locks so use your power wisely!";
        }
    }
}