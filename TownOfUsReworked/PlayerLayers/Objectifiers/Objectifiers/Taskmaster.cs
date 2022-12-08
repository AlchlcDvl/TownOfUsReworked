using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool TasksDone => TasksLeft <= 0;
        public int TasksToBeDone;
        public bool WinTasksDone;
        public bool TaskmasterWins { get; set; }
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
        public Dictionary<byte, ArrowBehaviour> TMArrows = new Dictionary<byte, ArrowBehaviour>();

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = "Do something that no one has ever done before! Finish your tasks";
            SymbolName = "";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Taskmaster : Colors.Neutral;
            ObjectifierType = ObjectifierEnum.Taskmaster;
            AddToObjectifierHistory(ObjectifierType);
        }

        public void Wins()
        {
            TaskmasterWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!WinTasksDone | !Player.Data.IsDead && !Player.Data.Disconnected)
                return true;

            Utils.EndGame();
            return false;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TMArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            TMArrows.Remove(arrow.Key);
        }
    }
}