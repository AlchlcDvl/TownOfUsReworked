using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Tunneler : Ability
    {
        public Tunneler(PlayerControl player) : base(player)
        {
            Name = "Tunneler";
            TaskText = "- You can finish tasks to be able to vent.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Tunneler : Colors.Ability;
            AbilityType = AbilityEnum.Tunneler;
            Hidden = !CustomGameOptions.TunnelerKnows;
            AbilityDescription = "You are a Tunneler! Finish your tasks so you can sqeeze your way through vents!";
        }
    }
}