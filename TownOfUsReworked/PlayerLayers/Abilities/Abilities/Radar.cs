using System.Collections.Generic;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new List<ArrowBehaviour>();
        public PlayerControl ClosestPlayer;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = "Be on high alert";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Radar : Colors.Ability;
            AbilityType = AbilityEnum.Radar;
        }
    }
}