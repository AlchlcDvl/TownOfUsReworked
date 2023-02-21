using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Underdog : Ability
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            TaskText = Utils.LastImp()
                ? "You have a shortened kill cooldown!"
                : "You have a long kill cooldown until you're alone";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Underdog : Colors.Ability;
            AbilityType = AbilityEnum.Underdog;
            Hidden = !CustomGameOptions.UnderdogKnows;
        }
    }
}
