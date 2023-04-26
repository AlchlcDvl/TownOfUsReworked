using HarmonyLib;
using Hazel;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles;
using System.Linq;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static class CheckEndGame
    {
        public static bool Prefix()
        {
            if (!AmongUsClient.Instance.AmHost || ConstantVariables.IsFreePlay)
                return false;

            if (ConstantVariables.IsHnS)
                return true;

            var crewexists = false;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew) && player.CanDoTasks())
                    crewexists = true;
            }

            if (ConstantVariables.NoOneWins)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.NobodyWins);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                Role.NobodyWins = true;
                Objectifier.NobodyWins = true;
                return true;
            }
            else if ((Utils.TasksDone() || GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) && crewexists)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                Role.CrewWin = true;
                return true;
            }
            else if (Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);

                if (CustomGameOptions.AltImps)
                {
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    Role.SyndicateWin = true;
                }
                else
                {
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    Role.IntruderWin = true;
                }

                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return true;
            }
            else
                return PlayerLayer.AllLayers.All(layer => layer.GameEnd()) || ConstantVariables.GameHasEnded;
        }
    }
}