namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
public static class CheckEndGame
{
    [HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static bool Prefix(LogicGameFlowNormal __instance)
    {
        CheckEnd(__instance);
        return false;
    }

    private static void CheckEnd(LogicGameFlowNormal __instance)
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        if (Sabotaged())
        {
            WinState = SyndicateSettings.AltImps ? WinLose.SyndicateWins : WinLose.IntrudersWin;
            CallRpc(CustomRPC.WinLose, WinState);
        }

        if (WinState == WinLose.None)
            return;

        if (IsFreePlay())
        {
            HUD().ShowPopUp("The game would have normally ended here, but since this is practice mode, we've reversed everything!");
            __instance.Manager.ReviveEveryoneFreeplay();
            WinState = WinLose.None;
            Ship().Begin();
        }
        else
            EndGame();
    }

    public static void CheckPlayerWins()
    {
        DetectStalemate();

        if (WinState != WinLose.None) // Skipping subsequent checks because a condition was already fulfilled
            return;

        CheckFactionWin();

        if (WinState != WinLose.None)
            return;

        CheckSubFactionWin();

        if (WinState != WinLose.None)
            return;

        PlayerLayer.GetLayers<Role>().ForEach(x => x.GameEnd());

        if (WinState != WinLose.None)
            return;

        PlayerLayer.GetLayers<Disposition>().ForEach(x => x.GameEnd());
    }

    public static void CheckSpellWin(IHexer hexer)
    {
        if (hexer.Player.HasDied())
            return;

        var players = AllPlayers();
        var faction = hexer.Faction;
        Func<PlayerControl, bool> factionCheck = faction switch
        {
            Faction.Syndicate or Faction.Crew or Faction.Intruder or Faction.Illuminati or Faction.Compliance or Faction.Pandorica => x => x.Is(faction),
            _ => NeutralSettings.NoSolo switch
            {
                NoSolo.AllNeutrals => x => x.Is(faction),
                NoSolo.AllNKs => x => x.Is(Alignment.Killing),
                _ => hexer.LinkedDisposition switch
                {
                    LayerEnum.Mafia => x => x.Is(LayerEnum.Mafia),
                    LayerEnum.Lovers => x => x.IsOtherLover(hexer.Player),
                    _ => x => x == hexer.Player
                }
            }
        };

        if (hexer.Spelled.Count == players.Count(plr => !plr.HasDied() && !factionCheck(plr)))
        {
            WinState = hexer.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Neutral => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                    NoSolo.AllNKs => WinLose.AllNKsWin,
                    _ => hexer.LinkedDisposition switch
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
    }

    private static void CheckFactionWin()
    {
        if (SyndicateWins())
            WinState = WinLose.SyndicateWins;
        else if (IntrudersWin())
            WinState = WinLose.IntrudersWin;
        else if (CrewWins())
            WinState = WinLose.CrewWins;
        else if (ApocWins())
            WinState = WinLose.ApocalypseWins;
        else if (AllNeutralsWin())
            WinState = WinLose.AllNeutralsWin;
        else if (AllNKsWin())
            WinState = WinLose.AllNKsWin;
        else
            return;

        CallRpc(CustomRPC.WinLose, WinState);
    }

    private static void CheckSubFactionWin()
    {
        if (CabalWin())
            WinState = WinLose.CabalWins;
        else if (CultWin())
            WinState = WinLose.CultWins;
        else if (UndeadWin())
            WinState = WinLose.UndeadWins;
        else if (ReanimatedWin())
            WinState = WinLose.ReanimatedWins;
        else
            return;

        CallRpc(CustomRPC.WinLose, WinState);
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

    public static bool TasksDone()
    {
        if ((int)TaskSettings.LongTasks + (int)TaskSettings.CommonTasks + (int)TaskSettings.ShortTasks == 0)
            return IsCustomHnS() || IsTaskRace();

        if (IsTaskRace())
            return PlayerLayer.GetLayers<Runner>().First(x => x.TasksDone);

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
        if (!IntruderSettings.IntrudersCanSabotage)
            return false;

        foreach (var sab in Ship().Systems?.Values)
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