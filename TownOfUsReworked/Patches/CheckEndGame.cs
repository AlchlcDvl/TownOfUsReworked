namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    [HarmonyPriority(Priority.First)]
    public static class CheckEndGame
    {
        public static bool Prefix()
        {
            if (!AmongUsClient.Instance.AmHost || ConstantVariables.IsFreePlay)
                return false;

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
                PlayerLayer.NobodyWins = true;
                Utils.EndGame();
            }
            else if ((Utils.TasksDone() || GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) && crewexists)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Role.CrewWin = true;
                Utils.EndGame();
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
            }
            else
            {
                foreach (var layer in PlayerLayer.AllLayers)
                    layer.GameEnd();
            }

            return ConstantVariables.GameHasEnded;
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static class OverrideEndGame
    {
        public static void Postfix(ref bool __result) => __result = false;
    }
}