using Assets.CoreScripts;

namespace TownOfUsReworked.Patches.Core.GameFlow;

[HarmonyPatch]
public static class OnGameEndPatches
{
    private static readonly List<SummaryInfo> PlayerRoles = [];
    public static readonly List<SummaryInfo> Disconnected = [];
    private static readonly Dictionary<string, IEnumerable<PlayerLayer>> Winners = [];

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClientOnGameEnd
    {
        public static void Postfix()
        {
            var players = AllPlayers();
            PlayerRoles.Clear();
            // There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
            // AD says "Done".
            players.Do(x => AddSummaryInfo(x));
            EndGameResult.CachedGameOverReason = (GameOverReason)9;
            EndGameResult.CachedWinners.Clear();
            Winners.Clear();
            MapPatches.AdjustSettings(false);

            foreach (var role in PlayerLayer.GetLayers<Role>())
            {
                if (role is { Disconnected: false, Winner: true })
                    Winners[role.PlayerName] = role.Player.GetLayers();
            }

            EndGameResult.CachedWinners.AddRange(Winners.Select(x => new CachedPlayerData(x.Value.First().Data)));

            if (EndGameResult.CachedWinners.Any(x => x.PlayerName == CustomPlayer.Local.name))
            {
                if (CustomPlayer.Local.CanKill())
                {
                    if (!KillCounts.TryGetValue(CustomPlayer.Local.PlayerId, out var count) || count == 0)
                        CustomAchievementManager.UnlockAchievement("Pacifist");
                    else if (count >= KillCounts.Values.Max())
                        CustomAchievementManager.UnlockAchievement("Bloodthirsty");
                }

                if (CustomPlayer.Local.Is<Corrupted>() && CustomPlayer.Local.Is<Mayor>())
                    CustomAchievementManager.UnlockAchievement("JustPolitics");
            }

            if (players.Count(x => !x.HasDied()) == 1 && !CustomPlayer.Local.HasDied())
                CustomAchievementManager.UnlockAchievement("LastOneStanding");

            if ((WinState is (> WinLose.BountyHunterWins and < WinLose.ArsonistWins) or WinLose.HuntedWin &&! players.Any(x => x.Is(CustomPlayer.Local.GetFaction()) && x.HasDied())) ||
                (WinState is > WinLose.ReanimatedWins and < WinLose.LoveWins && !players.Any(x => x.Is(CustomPlayer.Local.GetRole().Type) && x.HasDied()) && NeutralKillingSettings.WinSolo) ||
                (WinState == WinLose.HuntedWin && GameModeSettings.HnSMode == HnSMode.Classic && !players.Any(x => x.Is<Hunted>(out var hunted) && !hunted.Alive)))
            {
                CustomAchievementManager.UnlockAchievement("BloodOfTheCovenant");
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGameManagerPatches
    {
        public static bool Prefix(EndGameManager __instance)
        {
            if (IsHnS())
                return true;

            CustomStatsManager.IncrementStat(StringNames.StatsGamesFinished);
            __instance.Navigation.HideButtons();
            var cachedPlayerData = EndGameResult.CachedWinners.ToSystem().Find(h => h.IsYou);

            if (cachedPlayerData is not null)
            {
                CustomStatsManager.AddWin(MapPatches.CurrentMap, Winners[cachedPlayerData.PlayerName]);
                AchievementManager.Instance.SetWinMap(MapPatches.CurrentMap);
                UnityTelemetry.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId,
                    cachedPlayerData.NamePlateId);
                __instance.WinText.text = TranslationController.Instance.GetString(StringNames.Victory);
                __instance.WinText.color = UColor.blue;
            }
            else if (WinState == WinLose.NobodyWins)
            {
                CustomStatsManager.AddDraw();
                __instance.WinText.text = "Stalemate";
                __instance.WinText.color = UColor.white;
            }
            else
            {
                CustomStatsManager.AddLoss();
                __instance.WinText.text = TranslationController.Instance.GetString(StringNames.Defeat);
                __instance.WinText.color = UColor.red;
            }

            var list = EndGameResult.CachedWinners.ToSystem().OrderBy(b => !b.IsYou);

            foreach (var (i, cachedPlayerData2) in list.Indexed())
            {
                var num2 = i % 2 != 0 ? 1 : -1;
                var num3 = (i + 1) / 2;
                var t = num3 / 8f;
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

            if (!Winners.Any())
                WinState = WinLose.NobodyWins;

            if (WinState == WinLose.NobodyWins)
            {
                Play("Stalemate");
                __instance.BackgroundBar.material.color = UColor.white;
            }
            else
            {
                var text = UObject.Instantiate(__instance.WinText, __instance.WinText.transform.parent);
                var (textText, winSound, color) = WinState switch
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
                    WinLose.UndeadWins => ("The Undead Win", "IntruderWin", CustomColorManager.Undead),
                    WinLose.CabalWins => ("The Cabal Wins", "IntruderWin", CustomColorManager.Cabal),
                    WinLose.ReanimatedWins => ("The Reanimated Win", "IntruderWin", CustomColorManager.Reanimated),
                    WinLose.CultWins => ("The Cult Wins", "IntruderWin", CustomColorManager.Cult),
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
                    WinLose.CorruptedWins => ("The Corrupt Win", "IntruderWin", CustomColorManager.Corrupted),
                    WinLose.OverlordWins => ("Overlords Win", "IntruderWin", CustomColorManager.Overlord),
                    WinLose.MafiaWins => ("The Mafia Wins", "IntruderWin", CustomColorManager.Mafia),
                    WinLose.DefectorWins => ("Defector Wins", "IntruderWin", CustomColorManager.Defector),
                    WinLose.TaskRunnerWins => ("Tasks Completed", "IntruderWin", CustomColorManager.TaskRace),
                    WinLose.HunterWins => ("Hunters Win", "IntruderWin", CustomColorManager.Hunter),
                    WinLose.HuntedWin => ("The Hunted Win", "IntruderWin", CustomColorManager.Hunted),
                    WinLose.PandoricaWins => ("The Pandorica Wins", "IntruderWin", CustomColorManager.Pandorica),
                    WinLose.IlluminatiWins => ("The Illuminati Wins", "IntruderWin", CustomColorManager.Illuminati),
                    WinLose.ComplianceWins => ("The Compliance Wins", "IntruderWin", CustomColorManager.Compliance),
                    WinLose.NeutralsWin => ("Neutrals Wins", "IntruderWin", CustomColorManager.Neutral),
                    _ => ("Stalemate", "Stalemate", CustomColorManager.Stalemate)
                };
                __instance.BackgroundBar.material.color = text.color = color;
                __instance.WinText.transform.localPosition += new Vector3(0f, 1.5f, 0f);
                text.text = $"<size=50%>{textText}!</size>";
                Play(winSound);
            }

            var position = Camera.main!.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
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
            roleSummaryTextMesh.text = $"{roleSummaryText}";
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

        if (role!.Type != LayerEnum.NoneRole)
        {
            foreach (var role2 in role.RoleHistory)
            {
                var part = TranslationManager.Translate($"Layer.{role2}");

                if (LayerDictionary.TryGetValue(role2, out var entry))
                    summary += $"<color=#{entry.Color.ToHtmlStringRGBA()}>{part}</color> → ";

                cache += $"{part} → ";
            }

            summary += $"{role.ColorString}{role.Name}</color>";
            cache += role.Name;

            if (role.SubFaction != SubFaction.None && !player.Is(Alignment.Neophyte))
            {
                summary += $" {role.SubFactionColorString}{role.SubFactionSymbol}</color>";
                cache += $" {role.SubFactionSymbol}";
            }
        }

        if (disposition!.Type != LayerEnum.NoneDisposition)
        {
            summary += $" {disposition.ColoredSymbol}";
            cache += $" {disposition.Symbol}";
        }

        if (modifier!.Type != LayerEnum.NoneModifier)
        {
            summary += $" ({modifier.ColorString}{modifier.Name}</color>)";
            cache += $" ({modifier.Name})";
        }

        if (ability!.Type != LayerEnum.NoneAbility)
        {
            summary += $" [{ability.ColorString}{ability.Name}</color>]";
            cache += $" [{ability.Name}]";
        }

        if (player.IsGaTarget())
        {
            summary += " <#FFFFFFFF>★</color>";
            cache += " ★";
        }

        if (player.IsExeTarget())
        {
            summary += " <#CCCCCCFF>§</color>";
            cache += " §";
        }

        if (player.IsBhTarget())
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