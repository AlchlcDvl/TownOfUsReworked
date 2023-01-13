using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Snitch : Ability
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();
        public bool Revealed => TasksLeft() <= CustomGameOptions.SnitchTasksRemaining;

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = TasksDone
                ? "Follow the arrows pointing to the <color=#FF0000FF>Intruders</color>!"
                : "Complete all your tasks to discover the <color=#FF0000FF>Intruders</color>!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Snitch : Colors.Ability;
            AbilityType = AbilityEnum.Snitch;
            Hidden = !CustomGameOptions.SnitchKnows;
        }
        
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