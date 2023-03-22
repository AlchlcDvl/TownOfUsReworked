using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool WinTasksDone;
        public List<ArrowBehaviour> ImpArrows;

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = "- Finish your tasks before the game ends.";
            SymbolName = "µ";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Taskmaster : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Taskmaster;
            ImpArrows = new();
        }
    }
}