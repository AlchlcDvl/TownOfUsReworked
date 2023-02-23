using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Insider : Ability
    {
        public Insider(PlayerControl player) : base(player)
        {
            Name = "Insider";
            TaskText = "- You can finish your tasks to see the votes of others.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Insider : Colors.Ability;
            AbilityType = AbilityEnum.Insider;
            Hidden = !CustomGameOptions.InsiderKnows;
            AbilityDescription = "You are the Insider! Finish your tasks to be able to see who's voting who!";
        }
    }
}