using System.Collections.Generic;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new();
        public PlayerControl ClosestPlayer;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = "- You are aware of those close to you.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Radar : Colors.Ability;
            AbilityType = AbilityEnum.Radar;
            RadarArrow = new();
        }
    }
}