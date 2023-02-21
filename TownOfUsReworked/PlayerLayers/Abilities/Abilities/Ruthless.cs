using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Ruthless : Ability
    {
        public Ruthless(PlayerControl player) : base(player)
        {
            Name = "Ruthless";
            TaskText = "Your attacks cannot be stopped";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Ruthless : Colors.Ability;
            AbilityType = AbilityEnum.Ruthless;
            Hidden = !CustomGameOptions.RuthlessKnows;
        }
    }
}