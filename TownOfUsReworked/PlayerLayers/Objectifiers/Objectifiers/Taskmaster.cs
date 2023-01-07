using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
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
            SymbolName = "µ";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Taskmaster : Colors.Objectifier;
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
            if (!WinTasksDone || !Player.Data.IsDead && !Player.Data.Disconnected)
                return true;

            Utils.EndGame();
            return false;
        }
    }
}