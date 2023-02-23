using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => Role.GetRole(Player).TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool WinTasksDone;
        public bool TaskmasterWins { get; set; }
        public List<ArrowBehaviour> ImpArrows;
        public Dictionary<byte, ArrowBehaviour> TMArrows;

        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = "- Finish your tasks before the game ends.";
            SymbolName = "µ";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Taskmaster : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Taskmaster;
            ObjectifierDescription = "You are a Taskmaster! You are a master of completion who wants to do everything in the ship! Finish your tasks before the game ends!";
            ImpArrows = new List<ArrowBehaviour>();
            TMArrows = new Dictionary<byte, ArrowBehaviour>();
        }

        public override void Wins()
        {
            TaskmasterWins = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;
                
            if (WinTasksDone)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.TaskmasterWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return true;
        }
    }
}