using Assets.CoreScripts;

namespace TownOfUsReworked.Patches;

public static class OnGameEndPatches
{
    private static readonly List<SummaryInfo> PlayerRoles = [];
    public static readonly List<SummaryInfo> Disconnected = [];
    private static readonly Dictionary<string, IEnumerable<PlayerLayer>> Winners = [];

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClient_OnGameEnd
    {
        public static void Postfix()
        {
            PlayerRoles.Clear();
            // There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
            // AD says "Done".
            AllPlayers().ForEach(x => AddSummaryInfo(x));
            CameraEffect.Instance?.Materials?.Clear();
            EndGameResult.CachedGameOverReason = (GameOverReason)9;
            EndGameResult.CachedWinners.Clear();
            Winners.Clear();

            if (MapSettings.AutoAdjustSettings)
            {
                if (MapPatches.CurrentMap is 0 or 1 or 3)
                {
                    TownOfUsReworked.NormalOptions.NumShortTasks -= MapSettings.SmallMapIncreasedShortTasks;
                    TownOfUsReworked.NormalOptions.NumLongTasks -= MapSettings.SmallMapIncreasedLongTasks;
                    MapPatches.AdjustCooldowns(MapSettings.SmallMapDecreasedCooldown);
                }

                if (MapPatches.CurrentMap is 4 or 5 or 6)
                {
                    TownOfUsReworked.NormalOptions.NumShortTasks += MapSettings.LargeMapDecreasedShortTasks;
                    TownOfUsReworked.NormalOptions.NumLongTasks += MapSettings.LargeMapDecreasedLongTasks;
                    MapPatches.AdjustCooldowns(-MapSettings.LargeMapIncreasedCooldown);
                }
            }

            if (WinState == WinLose.AllNeutralsWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Neutral))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Neutral)
                        Winners[defect.PlayerName] = defect.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.AllNKsWin)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralKill))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Neutral)
                        Winners[defect.PlayerName] = defect.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.CrewWins)
            {
                foreach (var role2 in Role.GetRoles(Faction.Crew))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Crew)
                        Winners[ally.PlayerName] = ally.Player.GetLayers();
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Crew && defect.Turned)
                        Winners[defect.PlayerName] = defect.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.IntrudersWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Intruder))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Intruder)
                        Winners[ally.PlayerName] = ally.Player.GetLayers();
                }

                foreach (var traitor in PlayerLayer.GetLayers<Traitor>())
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                        Winners[traitor.PlayerName] = traitor.Player.GetLayers();
                }

                foreach (var fanatic in PlayerLayer.GetLayers<Fanatic>())
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                        Winners[fanatic.PlayerName] = fanatic.Player.GetLayers();
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Intruder && defect.Turned)
                        Winners[defect.PlayerName] = defect.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.SyndicateWins)
            {
                foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                        Winners[ally.PlayerName] = ally.Player.GetLayers();
                }

                foreach (var traitor in PlayerLayer.GetLayers<Traitor>())
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                        Winners[traitor.PlayerName] = traitor.Player.GetLayers();
                }

                foreach (var fanatic in PlayerLayer.GetLayers<Fanatic>())
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                        Winners[fanatic.PlayerName] = fanatic.Player.GetLayers();
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Syndicate && defect.Turned)
                        Winners[defect.PlayerName] = defect.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.UndeadWins)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.CabalWins)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.SectWins)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.ReanimatedWins)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.ApocalypseWins)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralApoc))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }

                foreach (var role2 in Role.GetRoles(Alignment.NeutralHarb))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.GlitchWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Glitch>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.JuggernautWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Juggernaut>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.ArsonistWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Arsonist>())
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.SerialKillerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<SerialKiller>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.MurdererWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Murderer>())
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.WerewolfWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Werewolf>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.CryomaniacWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Cryomaniac>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.PhantomWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Phantom>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.TasksDone && !role2.Caught)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.TaskRunnerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Runner>())
                {
                    if (!role2.Disconnected && role2.Winner)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.HunterWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Hunter>())
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.HuntedWin)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Hunted>())
                {
                    if (!role2.Disconnected)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.BetrayerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Betrayer>())
                {
                    if (!role2.Disconnected && role2.Faction == Faction.Neutral)
                        Winners[role2.PlayerName] = role2.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.LoveWins)
            {
                foreach (var lover in PlayerLayer.GetLayers<Lovers>())
                {
                    if (!lover.Disconnected && lover.Winner)
                        Winners[lover.PlayerName] = lover.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.RivalWins)
            {
                foreach (var rival in PlayerLayer.GetLayers<Rivals>())
                {
                    if (!rival.Disconnected && rival.Winner)
                        Winners[rival.PlayerName] = rival.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.TaskmasterWins)
            {
                foreach (var tm in PlayerLayer.GetLayers<Taskmaster>())
                {
                    if (!tm.Disconnected && tm.Winner)
                        Winners[tm.PlayerName] = tm.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.OverlordWins)
            {
                foreach (var ov in PlayerLayer.GetLayers<Overlord>())
                {
                    if (ov.Winner)
                        Winners[ov.PlayerName] = ov.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.CorruptedWins)
            {
                foreach (var corr in PlayerLayer.GetLayers<Corrupted>())
                {
                    if (!corr.Disconnected && corr.Winner)
                        Winners[corr.PlayerName] = corr.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.MafiaWins)
            {
                foreach (var maf in PlayerLayer.GetLayers<Mafia>())
                {
                    if (!maf.Disconnected)
                        Winners[maf.PlayerName] = maf.Player.GetLayers();
                }
            }
            else if (WinState == WinLose.DefectorWins)
            {
                foreach (var def in PlayerLayer.GetLayers<Defector>())
                {
                    if (!def.Disconnected && def.Side == Faction.Neutral)
                        Winners[def.PlayerName] = def.Player.GetLayers();
                }
            }

            if (!Disposition.DispositionWins)
            {
                if (WinState is not (WinLose.ActorWins or WinLose.BountyHunterWins or WinLose.CannibalWins or WinLose.ExecutionerWins or WinLose.GuesserWins or WinLose.JesterWins or
                    WinLose.TrollWins) || !NeutralEvilSettings.NeutralEvilsEndGame)
                {
                    foreach (var surv in PlayerLayer.GetLayers<Survivor>())
                    {
                        if (surv.Alive)
                            Winners[surv.PlayerName] = surv.Player.GetLayers();
                    }

                    foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
                    {
                        if (!ga.Failed && ga.TargetPlayer && ga.TargetAlive)
                            Winners[ga.PlayerName] = ga.Player.GetLayers();
                    }
                }

                foreach (var jest in PlayerLayer.GetLayers<Jester>())
                {
                    if (jest.VotedOut && !jest.Disconnected)
                        Winners[jest.PlayerName] = jest.Player.GetLayers();
                }

                foreach (var exe in PlayerLayer.GetLayers<Executioner>())
                {
                    if (exe.TargetVotedOut && !exe.Disconnected)
                        Winners[exe.PlayerName] = exe.Player.GetLayers();
                }

                foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
                {
                    if (bh.TargetKilled && !bh.Disconnected)
                        Winners[bh.PlayerName] = bh.Player.GetLayers();
                }

                foreach (var act in PlayerLayer.GetLayers<Actor>())
                {
                    if (act.Guessed && !act.Disconnected)
                        Winners[act.PlayerName] = act.Player.GetLayers();
                }

                foreach (var cann in PlayerLayer.GetLayers<Cannibal>())
                {
                    if (cann.Eaten && !cann.Disconnected)
                        Winners[cann.PlayerName] = cann.Player.GetLayers();
                }

                foreach (var guess in PlayerLayer.GetLayers<Guesser>())
                {
                    if (guess.TargetGuessed && !guess.Disconnected)
                        Winners[guess.PlayerName] = guess.Player.GetLayers();
                }

                foreach (var troll in PlayerLayer.GetLayers<Troll>())
                {
                    if (troll.Killed && !troll.Disconnected)
                        Winners[troll.PlayerName] = troll.Player.GetLayers();
                }

                foreach (var link in PlayerLayer.GetLayers<Linked>())
                {
                    if (Winners.ContainsKey(link.PlayerName) && !Winners.ContainsKey(link.OtherLink.name))
                        Winners[link.PlayerName] = link.OtherLink.GetLayers();
                }
            }

            EndGameResult.CachedWinners.AddRange(Winners.Select(x => new CachedPlayerData(x.Value.First().Data)));

            if (CustomPlayer.Local.CanKill() && EndGameResult.CachedWinners.Any(x => x.PlayerName == CustomPlayer.Local.name))
            {
                if (!KillCounts.TryGetValue(CustomPlayer.Local.PlayerId, out var count) || count == 0)
                    CustomAchievementManager.UnlockAchievement("Pacifist");
                else if (count >= KillCounts.Values.Max())
                    CustomAchievementManager.UnlockAchievement("Bloodthirsty");
            }

            if (AllPlayers().Count(x => !x.HasDied()) == 1 && !CustomPlayer.Local.HasDied())
                CustomAchievementManager.UnlockAchievement("LastOneStanding");
        }
    }

    [HarmonyPatch(typeof(EndGameManager))]
    public static class EndGameManagerPatches
    {
        [HarmonyPatch(nameof(EndGameManager.SetEverythingUp))]
        public static bool Prefix(EndGameManager __instance)
        {
            if (IsHnS())
                return true;

            CustomStatsManager.IncrementStat(StringNames.StatsGamesFinished);
            __instance.Navigation.HideButtons();
            var cachedPlayerData = EndGameResult.CachedWinners.ToSystem().Find(h => h.IsYou);

            if (cachedPlayerData != null)
            {
                CustomStatsManager.AddWin(MapPatches.CurrentMap, Winners[cachedPlayerData.PlayerName]);
                AchievementManager.Instance.SetWinMap(MapPatches.CurrentMap);
                UnityTelemetry.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId,
                    cachedPlayerData.NamePlateId);
                __instance.WinText.SetText(TranslationController.Instance.GetString(StringNames.Victory));
                __instance.WinText.color = UColor.blue;
            }
            else if (WinState == WinLose.NobodyWins)
            {
                CustomStatsManager.AddDraw();
                __instance.WinText.SetText("Stalemate");
                __instance.WinText.color = UColor.white;
            }
            else
            {
                CustomStatsManager.AddLoss();
                __instance.WinText.SetText(TranslationController.Instance.GetString(StringNames.Defeat));
                __instance.WinText.color = UColor.red;
            }

            var list = EndGameResult.CachedWinners.ToSystem().OrderBy(b => !b.IsYou);

            for (var i = 0; i < list.Count(); i++)
            {
                var cachedPlayerData2 = list.ElementAt(i);
                var num2 = i % 2 != 0 ? 1 : -1;
                var num3 = (i + 1) / 2;
                var t = num3 / (float)8;
                var num4 = Mathf.Lerp(1f, 0.75f, t);
                var num5 = i == 0 ? -8 : -1;
                var poolablePlayer = UObject.Instantiate(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num4, FloatRange.SpreadToEdges(-1.125f, 0f, num3, 8), (num5 + num3) * 0.01f) * 0.9f;
                var num6 = Mathf.Lerp(1f, 0.65f, t) * 0.9f;
                var vector = new Vector3(num6, num6, 1f);
                poolablePlayer.transform.localScale = vector;

                if (cachedPlayerData2.IsDead)
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                }
                else
                    poolablePlayer.SetFlipX(i % 2 == 0);

                poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData2.IsDead, true);
                poolablePlayer.ToggleName(true);
                var role = Winners[cachedPlayerData2.PlayerName].First();
                poolablePlayer.SetName($"<size=75%>{role}</size>\n<size=90%>{cachedPlayerData2.PlayerName}</size>", vector.Inv(), role.Color, -15f);
                poolablePlayer.SetNamePosition(new(0f, -1.31f, -0.5f));
            }

            AddAsset("CrewWin", __instance.CrewStinger);
            AddAsset("IntruderWin", __instance.ImpostorStinger);
            AddAsset("Stalemate", __instance.DisconnectStinger);

            if (WinState == WinLose.NobodyWins)
            {
                Play("Stalemate");
                __instance.BackgroundBar.material.color = UColor.white;
            }
            else
            {
                var text = UObject.Instantiate(__instance.WinText, __instance.WinText.transform.parent);
                var (texttext, winsound, color) = WinState switch
                {
                    WinLose.ActorWins => ("Actor Wins", "IntruderWin", CustomColorManager.Actor),
                    WinLose.JesterWins => ("Jester Wins", "IntruderWin", CustomColorManager.Jester),
                    WinLose.ExecutionerWins => ("Executioner Wins", "IntruderWin", CustomColorManager.Executioner),
                    WinLose.CannibalWins => ("Cannibal Wins", "IntruderWin", CustomColorManager.Cannibal),
                    WinLose.TrollWins => ("Troll Wins", "IntruderWin", CustomColorManager.Troll),
                    WinLose.PhantomWins => ("Phantom Wins", "IntruderWin", CustomColorManager.Phantom),
                    WinLose.GuesserWins => ("Guesser Wins", "IntruderWin", CustomColorManager.Guesser),
                    WinLose.BountyHunterWins => ("Bounty Hunter Wins", "IntruderWin", CustomColorManager.BountyHunter),
                    WinLose.BetrayerWins => ("Betrayer Wins", "IntruderWin", CustomColorManager.Betrayer),
                    WinLose.CrewWins => ("The Crew Wins", "CrewWin", CustomColorManager.Crew),
                    WinLose.IntrudersWin => ("Intruders Win", "IntruderWin", CustomColorManager.Intruder),
                    WinLose.SyndicateWins => ("Syndicate Wins", "IntruderWin", CustomColorManager.Syndicate),
                    WinLose.AllNeutralsWin => ("Neutrals Win", "IntruderWin", CustomColorManager.Neutral),
                    WinLose.UndeadWins => ("The Undead Win", "IntruderWin", CustomColorManager.Undead),
                    WinLose.CabalWins => ("The Cabal Wins", "IntruderWin", CustomColorManager.Cabal),
                    WinLose.ReanimatedWins => ("The Reanimated Win", "IntruderWin", CustomColorManager.Reanimated),
                    WinLose.SectWins => ("The Sect Wins", "IntruderWin", CustomColorManager.Sect),
                    WinLose.AllNKsWin => ("Neutral Killers Win", "IntruderWin", CustomColorManager.Intruder),
                    WinLose.ApocalypseWins => ("The Apocalypse Is Neigh", "IntruderWin", CustomColorManager.Apocalypse),
                    WinLose.ArsonistWins => ("Arsonist Wins", "IntruderWin", CustomColorManager.Arsonist),
                    WinLose.CryomaniacWins => ("Cryomaniac Wins", "IntruderWin", CustomColorManager.Cryomaniac),
                    WinLose.GlitchWins => ("Glitch Wins", "IntruderWin", CustomColorManager.Glitch),
                    WinLose.JuggernautWins => ("Juggernaut Wins", "IntruderWin", CustomColorManager.Juggernaut),
                    WinLose.MurdererWins => ("Murderer Wins", "IntruderWin", CustomColorManager.Murderer),
                    WinLose.SerialKillerWins => ("Serial Killer Wins", "SerialKillerWin", CustomColorManager.SerialKiller),
                    WinLose.WerewolfWins => ("Werewolf Wins", "IntruderWin", CustomColorManager.Werewolf),
                    WinLose.LoveWins => ("Love Wins", "IntruderWin", CustomColorManager.Lovers),
                    WinLose.TaskmasterWins => ("Taskmaster Wins", "IntruderWin", CustomColorManager.Taskmaster),
                    WinLose.RivalWins => ("The Rival Wins", "IntruderWin", CustomColorManager.Rivals),
                    WinLose.CorruptedWins => ("The Corrupted Win", "IntruderWin", CustomColorManager.Corrupted),
                    WinLose.OverlordWins => ("Overlords Win", "IntruderWin", CustomColorManager.Overlord),
                    WinLose.MafiaWins => ("The Mafia Wins", "IntruderWin", CustomColorManager.Mafia),
                    WinLose.DefectorWins => ("Defector Wins", "IntruderWin", CustomColorManager.Defector),
                    WinLose.TaskRunnerWins => ("Tasks Completed", "IntruderWin", CustomColorManager.TaskRace),
                    WinLose.HunterWins => ("Hunters Win", "IntruderWin", CustomColorManager.Hunter),
                    WinLose.HuntedWin => ("The Hunted Win", "IntruderWin", CustomColorManager.Hunted),
                    WinLose.EveryoneWins => ("Everyone Wins", "Stalemate", CustomColorManager.Stalemate),
                    _ => ("Stalemate", "Stalemate", CustomColorManager.Stalemate)
                };
                __instance.BackgroundBar.material.color = text.color = color;
                __instance.WinText.transform.localPosition += new Vector3(0f, 1.5f, 0f);
                text.SetText($"<size=50%>{texttext}!</size>");
                Play(winsound);
            }

            var position = Camera.main.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
            var roleSummary = UObject.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
            roleSummary.transform.localScale = new(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            var roleSummaryCache = new StringBuilder();
            var winnersText = new StringBuilder();
            var winnersCache = new StringBuilder();
            var losersText = new StringBuilder();
            var losersCache = new StringBuilder();
            var discText = new StringBuilder();
            var discCache = new StringBuilder();

            var winnerCount = 0;
            var loserCount = 0;

            roleSummaryText.AppendLine("<size=125%><u><b>Game Summary</b></u>:</size>");
            roleSummaryText.AppendLine();
            roleSummaryCache.AppendLine("Game Summary:");
            roleSummaryCache.AppendLine();
            winnersText.AppendLine("<size=105%><#00FF00FF><b>◈ - Winners - ◈</b></color></size>");
            losersText.AppendLine("<size=105%><#FF0000FF><b>◆ - Losers - ◆</b></color></size>");
            discText.AppendLine("<size=105%><#0000FFFF><b>◇ - Disconnected - ◇</b></color></size>");
            winnersCache.AppendLine("◈ - Winners - ◈");
            losersCache.AppendLine("◆ - Losers - ◆");
            discCache.AppendLine("◇ - Disconnected - ◇");

            foreach (var data in PlayerRoles)
            {
                var dataString = $"<size=75%>{data.PlayerName} - {data.History}</size>";
                var dataCache = $"{data.PlayerName} - {data.CachedHistory}";

                if (data.PlayerName.IsWinner())
                {
                    winnersText.AppendLine(dataString);
                    winnersCache.AppendLine(dataCache);
                    winnerCount++;
                }
                else
                {
                    losersText.AppendLine(dataString);
                    losersCache.AppendLine(dataCache);
                    loserCount++;
                }
            }

            foreach (var data in Disconnected)
            {
                var dataString = $"<size=75%>{data.PlayerName} - {data.History}</size>";
                var dataCache = $"{data.PlayerName} - {data.CachedHistory}";
                discText.AppendLine(dataString);
                discCache.AppendLine(dataCache);
            }

            if (winnerCount == 0)
            {
                winnersText.AppendLine("<size=75%>No One Won</size>");
                winnersCache.AppendLine("No One Won");
            }

            if (loserCount == 0)
            {
                losersText.AppendLine("<size=75%>No One Lost</size>");
                losersCache.AppendLine("No One Lost");
            }

            roleSummaryText.Append(winnersText);
            roleSummaryText.AppendLine();
            roleSummaryText.Append(losersText);
            roleSummaryCache.Append(winnersCache);
            roleSummaryCache.AppendLine();
            roleSummaryCache.Append(losersCache);

            if (Disconnected.Any())
            {
                roleSummaryText.AppendLine();
                roleSummaryText.Append(discText);
                roleSummaryCache.AppendLine();
                roleSummaryCache.Append(discCache);
            }

            var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
            roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = UColor.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;
            roleSummaryTextMesh.SetText($"{roleSummaryText}");
            roleSummaryTextMesh.GetComponent<RectTransform>().anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
            SaveText("Summary", $"{roleSummaryCache}", TownOfUsReworked.Other);
            PlayerRoles.Clear();
            Disconnected.Clear();
            return false;
        }
    }

    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation))]
    public static class AutoPlayAgainPatch
    {
        public static void Postfix(EndGameNavigation __instance)
        {
            if (ClientOptions.AutoPlayAgain)
                __instance.NextGame();
        }
    }

    public static void AddSummaryInfo(PlayerControl player, bool disconnected = false)
    {
        if (!player || !player.Data || Disconnected.Any(x => x.PlayerName == player.name) || PlayerRoles.Any(x => x.PlayerName == player.name))
            return;

        var summary = "";
        var cache = "";

        var info = player.GetLayers().ToList();

        if (info.Count != 4)
            return;

        var role = info[0] as Role;
        var modifier = info[1] as Modifier;
        var ability = info[2] as Ability;
        var disposition = info[3] as Disposition;

        if (role.Type != LayerEnum.NoneRole)
        {
            if (role.RoleHistory.Any())
            {
                foreach (var role2 in role.RoleHistory)
                {
                    var part = TranslationManager.Translate($"Layer.{role2}");

                    if (LayerDictionary.TryGetValue(role2, out var entry))
                        summary += $"<color=#{entry.Color.ToHtmlStringRGBA()}>{part}</color> → ";

                    cache += $"{part} → ";
                }
            }

            summary += $"{role.ColorString}{role.Name}</color>";
            cache += role.Name;

            if (role.SubFaction != SubFaction.None && !player.Is(Alignment.NeutralNeo))
            {
                summary += $" {role.SubFactionColorString}{role.SubFactionSymbol}</color>";
                cache += $" {role.SubFactionSymbol}";
            }
        }

        if (disposition.Type != LayerEnum.NoneDisposition)
        {
            summary += $" {disposition.ColoredSymbol}";
            cache += $" {disposition.Symbol}";
        }

        if (modifier.Type != LayerEnum.NoneModifier)
        {
            summary += $" ({modifier.ColorString}{modifier.Name}</color>)";
            cache += $" ({modifier.Name})";
        }

        if (ability.Type != LayerEnum.NoneAbility)
        {
            summary += $" [{ability.ColorString}{ability.Name}</color>]";
            cache += $" [{ability.Name}]";
        }

        if (player.IsGATarget())
        {
            summary += " <#FFFFFFFF>★</color>";
            cache += " ★";
        }

        if (player.IsExeTarget())
        {
            summary += " <#CCCCCCFF>§</color>";
            cache += " §";
        }

        if (player.IsBHTarget())
        {
            summary += " <#B51E39FF>Θ</color>";
            cache += " Θ";
        }

        if (player.IsGuessTarget())
        {
            summary += " <#EEE5BEFF>π</color>";
            cache += " π";
        }

        if (player == Syndicate.DriveHolder)
        {
            summary += " <#008000FF>Δ</color>";
            cache += " Δ";
        }

        if (player.CanDoTasks() && role)
        {
            if (!role.TasksDone)
            {
                summary += $" <{role.TasksCompleted}/{role.TotalTasks}>";
                cache += $" <{role.TasksCompleted}/{role.TotalTasks}>";
            }
            else
            {
                summary += $" {(char)0x25A0}";
                cache += $" {(char)0x25A0}";
            }
        }

        if (!disconnected)
        {
            summary += player.DeathReason();
            cache += player.DeathReason();
            PlayerRoles.Add(new(player.name, summary, cache));
        }
        else
            Disconnected.Add(new(player.name, summary, cache));
    }

    private static bool IsWinner(this string playerName) => EndGameResult.CachedWinners.Any(x => x.PlayerName == playerName);

    private static string DeathReason(this PlayerControl player)
    {
        if (!player)
            return "";

        var role = player.GetRole();

        if (!role)
            return "";

        var die = role.DeathReason is not DeathReasonEnum.Alive ? $" | {role.DeathReason}" : "";

        if (role.DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Ejected or DeathReasonEnum.Suicide or DeathReasonEnum.Escaped) && !IsNullEmptyOrWhiteSpace(role.KilledBy))
            die += role.KilledBy;

        return die;
    }
}