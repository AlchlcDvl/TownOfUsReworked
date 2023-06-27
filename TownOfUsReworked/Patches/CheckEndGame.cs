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

            if (ConstantVariables.NoOneWins)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.NobodyWins);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                PlayerLayer.NobodyWins = true;
                Utils.EndGame();
            }
            else if (LayerExtentions.TasksDone() && Role.GetRoles(Faction.Crew).Any(x => x.Player.CanDoTasks()))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Role.CrewWin = true;
                Utils.EndGame();
            }
            else if (LayerExtentions.Sabotaged() && CustomGameOptions.IntrudersCanSabotage)
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

                DetectStalemate();
            }

            return false;
        }

        private static void DetectStalemate()
        {
            var notDead = CustomPlayer.AllPlayers.Where(x => !x.Data.IsDead && !x.Data.Disconnected);
            var pureCrew = notDead.Count(x => x.Is(Faction.Crew) && x.Is(SubFaction.None));
            var nonPureCrew = notDead.Count(x => x.Is(Faction.Crew) && !x.Is(SubFaction.None));
            var purInt = notDead.Count(x => x.Is(Faction.Intruder) && x.Is(SubFaction.None));

            /*if (false)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.NobodyWins);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                PlayerLayer.NobodyWins = true;
                Utils.EndGame();
            }*/
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static class OverrideEndGame
    {
        public static void Postfix(ref bool __result) => __result = false;
    }
}