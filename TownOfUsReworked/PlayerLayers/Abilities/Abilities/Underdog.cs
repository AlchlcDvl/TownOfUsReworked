using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Underdog : Ability
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = LayerExtentions.Last(Player)
                ? "- You have shortened cooldowns."
                : (CustomGameOptions.UnderdogIncreasedKC
                    ? "- You have long cooldowns until you're not alone."
                    : "- You have short cooldowns when you're alone.");
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            Hidden = !CustomGameOptions.UnderdogKnows && !LayerExtentions.Last(Player);
        }
    }
}