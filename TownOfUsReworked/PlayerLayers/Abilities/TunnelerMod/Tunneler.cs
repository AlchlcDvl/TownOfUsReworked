using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.TunnelerMod
{
    public class Tunneler : Ability
    {
        public bool TasksDone;
        
        public Tunneler(PlayerControl player) : base(player)
        {
            Name = "Tunneler";
            TaskText = () => "You can dig yourself into the ground";
            if (CustomGameOptions.CustomAbilityColors) Color = Colors.Tunneler;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Tunneler;
            AddToAbilityHistory(AbilityType);
        }
    }
}