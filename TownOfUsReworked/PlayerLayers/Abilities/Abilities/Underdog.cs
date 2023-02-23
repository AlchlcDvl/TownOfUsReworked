using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Underdog : Ability
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = Utils.Last(Player)
                ? "- You have shortened cooldowns."
                : CustomGameOptions.UnderdogIncreasedKC
                    ? "- You have long cooldowns until you're not alone."
                    : "- You have short cooldowns when you're alone.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            Hidden = !CustomGameOptions.UnderdogKnows;
            AbilityDescription = "You are an Underdog! You have a powerful comback power to save your faction!";
        }
    }
}
