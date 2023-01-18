using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Insider : Ability
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public byte TargetId = byte.MaxValue;
        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public bool TasksDone => TasksLeft <= 0;

        public Insider(PlayerControl player) : base(player)
        {
            Name = "Insider";
            TaskText = TasksDone
                    ? "Learn votes of others!"
                    : "Do your tasks to be able to see the votes of others!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Insider : Colors.Ability;
            AbilityType = AbilityEnum.Insider;
            Hidden = !CustomGameOptions.InsiderKnows;
        }
    }
}