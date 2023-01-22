using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System.Collections.Generic;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft() <= CustomGameOptions.TMTasksRemaining;
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
        }

        public override void Wins()
        {
            TaskmasterWins = true;
        }

        internal override bool GameEnd(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;
                
            if (WinTasksDone)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
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