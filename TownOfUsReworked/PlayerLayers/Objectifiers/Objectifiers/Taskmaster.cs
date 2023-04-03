using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool WinTasksDone;
        public List<ArrowBehaviour> ImpArrows = new();

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = "- Finish your tasks before the game ends.";
            SymbolName = "µ";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Taskmaster : Colors.Objectifier;
            Type = ObjectifierEnum.Taskmaster;
            ImpArrows = new();
        }

        public override void OnLobby() => ImpArrows.DestroyAll();
    }
}