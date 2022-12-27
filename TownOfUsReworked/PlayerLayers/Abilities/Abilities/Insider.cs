using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Insider : Ability
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public byte TargetId = byte.MaxValue;

        public Insider(PlayerControl player) : base(player)
        {
            Name = "Insider";
            TaskText = TasksDone
                    ? "Learn votes of others!"
                    : "";
            Color = Colors.Insider;
            AbilityType = AbilityEnum.Insider;
        }
    }
}