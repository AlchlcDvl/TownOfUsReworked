using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

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
            Type = AbilityEnum.Radar;
            RadarArrow = new();
        }

        public override void OnLobby() => RadarArrow.DestroyAll();
    }
}