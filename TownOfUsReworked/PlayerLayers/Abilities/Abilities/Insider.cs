using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Insider : Ability
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public byte TargetId = byte.MaxValue;

        public Insider(PlayerControl player) : base(player)
        {
            Name = "Insider";
            TaskText = "Do your tasks to be able to see the votes of others!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Insider : Colors.Ability;
            AbilityType = AbilityEnum.Insider;
            Hidden = !CustomGameOptions.InsiderKnows;
        }
    }
}