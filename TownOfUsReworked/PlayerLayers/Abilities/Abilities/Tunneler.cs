using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Tunneler : Ability
    {
        public Tunneler(PlayerControl player) : base(player)
        {
            Name = "Tunneler";
            TaskText = "- You can finish tasks to be able to vent.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tunneler : Colors.Ability;
            Type = AbilityEnum.Tunneler;
            Hidden = !CustomGameOptions.TunnelerKnows;
        }
    }
}