using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Ruthless : Ability
    {
        public Ruthless(PlayerControl player) : base(player)
        {
            Name = "Ruthless";
            TaskText = "- Your attacks cannot be stopped.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Ruthless : Colors.Ability;
            Type = AbilityEnum.Ruthless;
            Hidden = !CustomGameOptions.RuthlessKnows;
        }
    }
}