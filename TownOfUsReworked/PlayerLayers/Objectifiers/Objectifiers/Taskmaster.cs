using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool WinTasksDone;

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = "- Finish your tasks before the game ends";
            SymbolName = "µ";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Taskmaster : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Taskmaster;
            Type = LayerEnum.Taskmaster;
        }
    }
}