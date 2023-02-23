using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles;
using Hazel;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class CheckEndGame
    {
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            if (GameStates.IsHnS)
                return true;
            
            bool crewexists = false;
            
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew) && !player.NotOnTheSameSide())
                    crewexists = true;
            }
            
            if (Utils.NoOneWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.Stalemate);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                Role.NobodyWins = true;
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
                if (CustomGameOptions.AltImps)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.SyndicateWin = true;
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.IntruderWin = true;
                }

                return true;
            }
            else
            {
                foreach (var role in Role.AllRoles)
                {
                    if (!role.GameEnd(__instance))
                        return false;
                }
                
                foreach (var obj in Objectifier.AllObjectifiers)
                {
                    if (!obj.GameEnd(__instance))
                        return false;
                }

                return true;
            }
        }
    }
}