namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
public static class CheckEndGame
{
    [HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static bool Prefix(LogicGameFlowNormal __instance)
    {
        if (!AmongUsClient.Instance.AmHost || WinState == WinLose.None)
            return false;

        if (IsFreePlay())
        {
            HUD().ShowPopUp("The game would have normally ended here, but since this is practice mode, we've reversed everything!");
            __instance.Manager.ReviveEveryoneFreeplay();
            WinState = WinLose.None;
            Ship().Begin();
        }
        else
            EndGame();

        return false;
    }

    public static void CheckEnd()
    {
        var players = AllPlayers();
        var hex = PlayerLayer.GetILayers<IHexer>().Find(hexer =>
        {
            var faction = hexer.Faction;
            Func<PlayerControl, bool> factionCheck = faction switch
            {
                Faction.Syndicate or Faction.Crew or Faction.Intruder => x => x.Is(faction),
                _ => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => x => x.Is(faction),
                    NoSolo.AllNKs => x => x.Is(Alignment.NeutralKill),
                    _ => hexer.LinkedDisposition switch
                    {
                        LayerEnum.Mafia => x => x.Is(LayerEnum.Mafia),
                        LayerEnum.Lovers => x => x.IsOtherLover(hexer.Player),
                        _ => x => x == hexer.Player
                    }
                }
            };
            return !hexer.Player.HasDied() && hexer.Spelled.Count == players.Count(plr => !plr.HasDied() && !factionCheck(plr));
        });

        if (TasksDone())
        {
            WinState = IsCustomHnS() ? WinLose.HuntedWin : WinLose.CrewWins;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (Sabotaged() && IntruderSettings.IntrudersCanSabotage)
        {
            WinState = SyndicateSettings.AltImps ? WinLose.SyndicateWins : WinLose.IntrudersWin;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (hex != null)
        {
            WinState = hex.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Neutral => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                    NoSolo.AllNKs => WinLose.AllNKsWin,
                    _ => hex.LinkedDisposition switch
                    {
                        LayerEnum.Mafia => WinLose.MafiaWins,
                        LayerEnum.Lovers => WinLose.LoveWins,
                        _ => WinLose.DefectorWins
                    }
                },
                Faction.Syndicate => WinLose.SyndicateWins,
                _ => WinLose.NobodyWins
            };

            CallRpc(CustomRPC.WinLose, WinState);
        }
        else
        {
            DetectStalemate();
            PlayerLayer.GetLayers<Role>().ForEach(x => x.GameEnd());
            PlayerLayer.GetLayers<Disposition>().ForEach(x => x.GameEnd());
        }
    }

    // Stalemate detector for unwinnable situations
    private static void DetectStalemate()
    {
        var players = AllPlayers().Where(x => !x.HasDied());

        if (players.Count() == 2)
        {
            var player1 = players.First();
            var player2 = players.Last();
            var nosolo = NeutralSettings.NoSolo == NoSolo.Never;
            var nobuttons1 = player1.RemainingEmergencies == 0;
            var nobuttons2 = player2.RemainingEmergencies == 0;
            var nobuttons = nobuttons1 && nobuttons2;
            var onehasbutton = !nobuttons1 || !nobuttons2;
            var knighted1 = player1.IsKnighted() || player1.Is<Tiebreaker>();
            var knighted2 = player2.IsKnighted() || player2.Is<Tiebreaker>();
            var neitherknighted = (knighted1 && knighted2) || (!knighted1 && !knighted2);
            var onisknighted = !knighted1 || !knighted2;
            var pol1 = player1.Is<Politician>();
            var pol2 = player2.Is<Politician>();
            var cankill1 = player1.CanKill();
            var cankill2 = player2.CanKill();
            var cantkill = !cankill1 && !cankill2;
            var rival1 = player1.Is<Rivals>();
            var rival2 = player2.Is<Rivals>();
            var rivals = rival1 && rival2;
            var cryo = player1.Is<Cryomaniac>() && player2.Is<Cryomaniac>();

            // NK vs NK when neither can kill each other and Neutrals don't win together
            if ((cryo && nosolo && nobuttons && neitherknighted) || rivals || (cantkill && nobuttons))
                PerformStalemate();
        }
        else if (!players.Any())
            PerformStalemate();
    }

    public static void PerformStalemate()
    {
        CallRpc(CustomRPC.WinLose, WinLose.NobodyWins);
        WinState = WinLose.NobodyWins;
    }

    private static bool TasksDone()
    {
        GameData.Instance.RecomputeTaskCounts();

        if ((int)TaskSettings.LongTasks + (int)TaskSettings.CommonTasks + (int)TaskSettings.ShortTasks == 0)
            return IsCustomHnS();

        var allCrew = new List<PlayerControl>();
        var crewWithNoTasks = new List<PlayerControl>();
        var roles = IsCustomHnS() ? PlayerLayer.GetLayers<Hunted>() : Role.GetRoles(Faction.Crew).Where(x => x.Player.CanDoTasks());

        foreach (var role in roles)
        {
            if (role.Dead && !TaskSettings.GhostTasksCountToWin)
                continue;

            allCrew.Add(role.Player);

            if (role.TasksDone)
                crewWithNoTasks.Add(role.Player);
        }

        return allCrew.Count == crewWithNoTasks.Count;
    }

    private static bool Sabotaged()
    {
        foreach (var sab in Ship().Systems.Values)
        {
            if (sab.TryCast<LifeSuppSystemType>(out var life) && life.Countdown <= 0f)
                return true;
            else if (sab.TryCast<ICriticalSabotage>(out var crit) && crit.Countdown <= 0f)
                return true;
        }

        return false;
    }

    [HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static bool Prefix(ref bool __result) => __result = false;

    [HarmonyPatch(nameof(LogicGameFlowNormal.EndGameForSabotage))]
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(GameManager))]
public static class OverrideTaskEndGame
{
    [HarmonyPatch(nameof(GameManager.CheckEndGameViaTasks))]
    [HarmonyPatch(nameof(GameManager.CheckTaskCompletion))]
    public static bool Prefix(ref bool __result)
    {
        GameData.Instance.RecomputeTaskCounts();
        return __result = IsHnS();
    }
}