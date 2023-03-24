using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Snitch : Ability
    {
        public List<ArrowBehaviour> ImpArrows = new();
        public Dictionary<byte, ArrowBehaviour> SnitchArrows = new();

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = "- You can finish your tasks to get information on who's evil.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Snitch : Colors.Ability;
            AbilityType = AbilityEnum.Snitch;
            Hidden = !CustomGameOptions.SnitchKnows;
            ImpArrows = new();
            SnitchArrows = new();
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