using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Tiebreaker : Ability
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = "- Your votes break ties.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tiebreaker : Colors.Ability;
            Type = AbilityEnum.Tiebreaker;
            Hidden = !CustomGameOptions.TiebreakerKnows;
        }
    }
}