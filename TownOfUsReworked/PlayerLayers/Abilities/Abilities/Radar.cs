using System.Collections.Generic;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new List<ArrowBehaviour>();
        public PlayerControl ClosestPlayer;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = () => "Be on high alert";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Radar;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Radar;
            AddToAbilityHistory(AbilityType);
        }
    }
}