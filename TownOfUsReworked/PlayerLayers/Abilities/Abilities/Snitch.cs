using System.Collections.Generic;
using TownOfUsReworked.Extensions;
using UnityEngine;
using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Snitch : Ability
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();
        public bool SnitchWin;

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = () => TasksDone
                    ? "Follow the arrows pointing to the <color=#FF0000FF>Intruders</color>!"
                    : "Complete all your tasks to discover the <color=#FF0000FF>Intruders</color>!";
            if (CustomGameOptions.CustomCrewColors) Color = Colors.Snitch;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Snitch;
            AddToAbilityHistory(AbilityType);
        }

        public bool Revealed => TasksLeft <= CustomGameOptions.SnitchTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;
        
        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = SnitchArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SnitchArrows.Remove(arrow.Key);
        }
    }
}