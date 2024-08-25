using Assets.CoreScripts;

namespace TownOfUsReworked.Patches;

public static class OnGameEndPatches
{
    private static readonly List<SummaryInfo> PlayerRoles = [];
    public static readonly List<SummaryInfo> Disconnected = [];
    private static readonly List<NetworkedPlayerInfo> Winners = [];

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClient_OnGameEnd
    {
        public static void Postfix()
        {
            if (CameraEffect.Instance)
                CameraEffect.Instance.Materials.Clear();

            EndGameResult.CachedGameOverReason = (GameOverReason)9;
            EndGameResult.CachedWinners.Clear();
            Winners.Clear();

            if (Role.AllNeutralsWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Neutral))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Neutral && !defect.Player.IsBase(defect.Side))
                        Winners.Add(defect.Data);
                }
            }
            else if (Role.NKWins)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralKill))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Neutral && !defect.Player.IsBase(defect.Side))
                        Winners.Add(defect.Data);
                }
            }
            else if (Role.CrewWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Crew))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Crew)
                        Winners.Add(ally.Data);
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Crew && !defect.Player.IsBase(defect.Side))
                        Winners.Add(defect.Data);
                }
            }
            else if (Role.IntruderWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Intruder))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Intruder)
                        Winners.Add(ally.Data);
                }

                foreach (var traitor in PlayerLayer.GetLayers<Traitor>())
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                        Winners.Add(traitor.Data);
                }

                foreach (var fanatic in PlayerLayer.GetLayers<Fanatic>())
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                        Winners.Add(fanatic.Data);
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Intruder && !defect.Player.IsBase(defect.Side))
                        Winners.Add(defect.Data);
                }
            }
            else if (Role.SyndicateWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var ally in PlayerLayer.GetLayers<Allied>())
                {
                    if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                        Winners.Add(ally.Data);
                }

                foreach (var traitor in PlayerLayer.GetLayers<Traitor>())
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                        Winners.Add(traitor.Data);
                }

                foreach (var fanatic in PlayerLayer.GetLayers<Fanatic>())
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                        Winners.Add(fanatic.Data);
                }

                foreach (var defect in PlayerLayer.GetLayers<Defector>())
                {
                    if (!defect.Disconnected && defect.Side == Faction.Syndicate && !defect.Player.IsBase(defect.Side))
                        Winners.Add(defect.Data);
                }
            }
            else if (Role.UndeadWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.CabalWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.SectWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.ReanimatedWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.ApocalypseWins)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralApoc))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }

                foreach (var role2 in Role.GetRoles(Alignment.NeutralHarb))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.GlitchWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Glitch>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.JuggernautWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Juggernaut>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.ArsonistWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Arsonist>())
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.SerialKillerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<SerialKiller>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.MurdererWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Murderer>())
                {
                    if (!role2.Disconnected && role2.Faithful)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.WerewolfWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Werewolf>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.CryomaniacWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Cryomaniac>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.PhantomWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Phantom>())
                {
                    if (!role2.Disconnected && role2.Faithful && role2.TasksDone && !role2.Caught)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.TaskRunnerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Runner>())
                {
                    if (!role2.Disconnected && role2.Winner)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.HunterWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Hunter>())
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.HuntedWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Hunted>())
                {
                    if (!role2.Disconnected)
                        Winners.Add(role2.Data);
                }
            }
            else if (Role.BetrayerWins)
            {
                foreach (var role2 in PlayerLayer.GetLayers<Betrayer>())
                {
                    if (!role2.Disconnected && role2.Faction == Faction.Neutral)
                        Winners.Add(role2.Data);
                }
            }
            else if (Objectifier.LoveWins)
            {
                foreach (var lover in PlayerLayer.GetLayers<Lovers>())
                {
                    if (!lover.Disconnected && lover.Winner)
                        Winners.Add(lover.Data);
                }
            }
            else if (Objectifier.RivalWins)
            {
                foreach (var rival in PlayerLayer.GetLayers<Rivals>())
                {
                    if (!rival.Disconnected && rival.Winner)
                        Winners.Add(rival.Data);
                }
            }
            else if (Objectifier.TaskmasterWins)
            {
                foreach (var tm in PlayerLayer.GetLayers<Taskmaster>())
                {
                    if (!tm.Disconnected && tm.Winner)
                        Winners.Add(tm.Data);
                }
            }
            else if (Objectifier.OverlordWins)
            {
                foreach (var ov in PlayerLayer.GetLayers<Overlord>())
                {
                    if (ov.Winner)
                        Winners.Add(ov.Data);
                }
            }
            else if (Objectifier.CorruptedWins)
            {
                foreach (var corr in PlayerLayer.GetLayers<Corrupted>())
                {
                    if (!corr.Disconnected && corr.Winner)
                        Winners.Add(corr.Data);
                }
            }
            else if (Objectifier.MafiaWins)
            {
                foreach (var maf in PlayerLayer.GetLayers<Mafia>())
                {
                    if (!maf.Disconnected)
                        Winners.Add(maf.Data);
                }
            }
            else if (Objectifier.DefectorWins)
            {
                foreach (var def in PlayerLayer.GetLayers<Defector>())
                {
                    if (!def.Disconnected && def.Side == Faction.Neutral)
                        Winners.Add(def.Data);
                }
            }

            if (!Objectifier.ObjectifierWins)
            {
                if (!(Role.ActorWins || Role.BountyHunterWins || Role.CannibalWins || Role.ExecutionerWins || Role.GuesserWins || Role.JesterWins || Role.TrollWins) ||
                    !NeutralEvilSettings.NeutralEvilsEndGame)
                {
                    foreach (var surv in PlayerLayer.GetLayers<Survivor>())
                    {
                        if (surv.Alive)
                            Winners.Add(surv.Data);
                    }

                    foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
                    {
                        if (!ga.Failed && ga.TargetPlayer && ga.TargetAlive)
                            Winners.Add(ga.Data);
                    }
                }

                foreach (var jest in PlayerLayer.GetLayers<Jester>())
                {
                    if (jest.VotedOut && !jest.Disconnected)
                        Winners.Add(jest.Data);
                }

                foreach (var exe in PlayerLayer.GetLayers<Executioner>())
                {
                    if (exe.TargetVotedOut && !exe.Disconnected)
                        Winners.Add(exe.Data);
                }

                foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
                {
                    if (bh.TargetKilled && !bh.Disconnected)
                        Winners.Add(bh.Data);
                }

                foreach (var act in PlayerLayer.GetLayers<Actor>())
                {
                    if (act.Guessed && !act.Disconnected)
                        Winners.Add(act.Data);
                }

                foreach (var cann in PlayerLayer.GetLayers<Cannibal>())
                {
                    if (cann.Eaten && !cann.Disconnected)
                        Winners.Add(cann.Data);
                }

                foreach (var guess in PlayerLayer.GetLayers<Guesser>())
                {
                    if (guess.TargetGuessed && !guess.Disconnected)
                        Winners.Add(guess.Data);
                }

                foreach (var troll in PlayerLayer.GetLayers<Troll>())
                {
                    if (troll.Killed && !troll.Disconnected)
                        Winners.Add(troll.Data);
                }

                foreach (var link in PlayerLayer.GetLayers<Linked>())
                {
                    if (EndGameResult.CachedWinners.Any(x => x.PlayerName == link.PlayerName) && !EndGameResult.CachedWinners.Any(x => x.PlayerName == link.OtherLink.Data.PlayerName))
                        Winners.Add(link.OtherLink.Data);
                }
            }

            EndGameResult.CachedWinners = Winners.Select(x => new CachedPlayerData(x)).ToList().ToIl2Cpp();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class ShipStatus_SetEverythingUp
    {
        public static bool Prefix(EndGameManager __instance)
        {
            StatsManager.Instance.IncrementStat(StringNames.StatsGamesFinished);
            __instance.Navigation.HideButtons();
            var cachedPlayerData = EndGameResult.CachedWinners.ToSystem().FirstOrDefault(h => h.IsYou);

            if (cachedPlayerData != null)
            {
				StatsManager.Instance.AddWinReason((GameOverReason)9, MapPatches.CurrentMap, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                AchievementManager.Instance.SetWinMap(MapPatches.CurrentMap);
                UnityTelemetry.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId,
                    cachedPlayerData.NamePlateId);
				__instance.WinText.text = TranslationController.Instance.GetString(StringNames.Victory);
                __instance.WinText.color = UColor.blue;
            }
            else
            {
				StatsManager.Instance.AddLoseReason((GameOverReason)10);
				__instance.WinText.text = TranslationController.Instance.GetString(StringNames.Defeat);
                __instance.WinText.color = UColor.red;
            }

            var num = Mathf.CeilToInt(7.5f);
            var list = EndGameResult.CachedWinners.ToSystem().OrderBy(b => b.IsYou).ToList();

            for (var i = 0; i < list.Count; i++)
            {
                var cachedPlayerData2 = list[i];
                var num2 = i % 2 != 0 ? 1 : -1;
                var num3 = (i + 1) / 2;
                var t = num3 / (float)num;
                var num4 = Mathf.Lerp(1f, 0.75f, t);
                var num5 = i == 0 ? -8 : -1;
                var poolablePlayer = UObject.Instantiate(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num4, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), (num5 + num3) * 0.01f) * 0.9f;
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
                var playerInfo = Winners[i];
                var role = Role.AllRoles.Find(x => x.Data == playerInfo);
                poolablePlayer.SetName($"<size=75%>{role}</size>\n<size=90%>{cachedPlayerData2.PlayerName}</size>", vector.Inv(), role.Color, -15f);
                poolablePlayer.SetNamePosition(new(0f, -1.31f, -0.5f));
            }

            AddAsset("CrewWin", __instance.CrewStinger);
            AddAsset("IntruderWin", __instance.ImpostorStinger);
            AddAsset("Stalemate", __instance.DisconnectStinger);
            var text = UObject.Instantiate(__instance.WinText, __instance.WinText.transform.parent);
            var winsound = "IntruderWin";
            UColor? color = CustomColorManager.Stalemate;
            var texttext = "Stalemate";

            if (PlayerLayer.NobodyWins)
                winsound = "Stalemate";
            else if (Role.SyndicateWin)
            {
                texttext = "The Syndicate Wins";
                color = CustomColorManager.Syndicate;
            }
            else if (Role.IntruderWin)
            {
                texttext = "Intruders Win";
                color = CustomColorManager.Intruder;
            }
            else if (Role.AllNeutralsWin)
            {
                texttext = "Neutrals Win";
                color = CustomColorManager.Neutral;
            }
            else if (Role.CrewWin)
            {
                texttext = "Crew Wins";
                color = CustomColorManager.Crew;
                winsound = "CrewWin";
            }
            else if (Role.NKWins)
            {
                texttext = "Neutral Killers Win";
                color = CustomColorManager.Alignment;
            }
            else if (Role.ApocalypseWins)
            {
                texttext = "The Apocalypse Is Nigh";
                color = CustomColorManager.Apocalypse;
            }
            else if (Role.UndeadWin)
            {
                texttext = "The Undead Win";
                color = CustomColorManager.Undead;
            }
            else if (Role.CabalWin)
            {
                texttext = "The Cabal Wins";
                color = CustomColorManager.Cabal;
            }
            else if (Role.SectWin)
            {
                texttext = "The Sect Wins";
                color = CustomColorManager.Sect;
            }
            else if (Role.ReanimatedWin)
            {
                texttext = "The Reanimated Win";
                color = CustomColorManager.Reanimated;
            }
            else if (Role.CryomaniacWins)
            {
                texttext = "Cryomaniac Wins";
                color = CustomColorManager.Cryomaniac;
            }
            else if (Role.ArsonistWins)
            {
                texttext = "Aronist Wins";
                color = CustomColorManager.Arsonist;
            }
            else if (Role.GlitchWins)
            {
                texttext = "Glitch Wins";
                color = CustomColorManager.Glitch;
            }
            else if (Role.JuggernautWins)
            {
                texttext = "Juggernaut Wins";
                color = CustomColorManager.Juggernaut;
            }
            else if (Role.MurdererWins)
            {
                texttext = "Murderer Wins";
                color = CustomColorManager.Murderer;
            }
            else if (Role.SerialKillerWins)
            {
                texttext = "Serial Killer Wins";
                color = CustomColorManager.SerialKiller;
            }
            else if (Role.WerewolfWins)
            {
                texttext = "Werewolf Wins";
                color = CustomColorManager.Werewolf;
            }
            else if (Role.PhantomWins)
            {
                texttext = "Phantom Wins";
                color = CustomColorManager.Phantom;
            }
            else if (Role.ActorWins)
            {
                texttext = "Actor Wins";
                color = CustomColorManager.Actor;
            }
            else if (Role.BountyHunterWins)
            {
                texttext = "Bounty Hunter Wins";
                color = CustomColorManager.BountyHunter;
            }
            else if (Role.CannibalWins)
            {
                texttext = "Cannibal Wins";
                color = CustomColorManager.Cannibal;
            }
            else if (Role.ExecutionerWins)
            {
                texttext = "Executioner Wins";
                color = CustomColorManager.Executioner;
            }
            else if (Role.GuesserWins)
            {
                texttext = "Guesser Wins";
                color = CustomColorManager.Guesser;
            }
            else if (Role.JesterWins)
            {
                texttext = "Jester Wins";
                color = CustomColorManager.Jester;
            }
            else if (Role.TrollWins)
            {
                texttext = "Troll Wins";
                color = CustomColorManager.Troll;
            }
            else if (Role.TaskRunnerWins)
            {
                texttext = "Tasks Completed";
                color = CustomColorManager.TaskRace;
            }
            else if (Role.HuntedWins)
            {
                texttext = "The Hunted Survived";
                color = CustomColorManager.Hunted;
            }
            else if (Role.HunterWins)
            {
                texttext = "Every One Was Hunted";
                color = CustomColorManager.Hunter;
            }
            else if (Objectifier.CorruptedWins)
            {
                texttext = "Corrupted Wins";
                color = CustomColorManager.Corrupted;
            }
            else if (Objectifier.LoveWins)
            {
                texttext = "Love Wins";
                color = CustomColorManager.Lovers;
            }
            else if (Objectifier.RivalWins)
            {
                texttext = "Rival Wins";
                color = CustomColorManager.Rivals;
            }
            else if (Objectifier.TaskmasterWins)
            {
                texttext = "Taskmaster Wins";
                color = CustomColorManager.Taskmaster;
            }
            else if (Objectifier.OverlordWins)
            {
                texttext = "Overlord Wins";
                color = CustomColorManager.Overlord;
            }
            else if (Objectifier.MafiaWins)
            {
                texttext = "The Mafia Wins";
                color = CustomColorManager.Mafia;
            }
            else if (Objectifier.DefectorWins)
            {
                texttext = "Defectors Win";
                color = CustomColorManager.Defector;
            }

            __instance.BackgroundBar.material.color = text.color = color ?? CustomColorManager.Stalemate;
            __instance.WinText.transform.localPosition += new Vector3(0f, 1.5f, 0f);
            text.text = $"<size=50%>{texttext}!</size>";
            Play(winsound);
            return false;
        }
    }

    public static void AddSummaryInfo(PlayerControl player, bool disconnected = false)
    {
        if (!player || Disconnected.Any(x => x.PlayerName == player.Data.PlayerName) || PlayerRoles.Any(x => x.PlayerName == player.Data.PlayerName))
            return;

        var summary = "";
        var cache = "";

        var info = player.GetLayers();

        if (info.Count != 4)
            return;

        var role = info[0] as Role;
        var modifier = info[1] as Modifier;
        var ability = info[2] as Ability;
        var objectifier = info[3] as Objectifier;

        if (info[0])
        {
            if (role.RoleHistory.Any())
            {
                role.RoleHistory.Reverse();

                foreach (var role2 in role.RoleHistory)
                {
                    summary += $"{role2.ColorString}{role2.Name}</color> → ";
                    cache += $"{role2.Name} → ";
                }
            }

            summary += $"{role?.ColorString}{role?.Name}</color>";
            cache += role.Name;

            if (role.SubFaction != SubFaction.None && !player.Is(Alignment.NeutralNeo))
            {
                summary += $" {role?.SubFactionColorString}{role?.SubFactionSymbol}</color>";
                cache += $" {role?.SubFactionSymbol}";
            }
        }

        if (objectifier.Type != LayerEnum.NoneObjectifier)
        {
            summary += $" {objectifier.ColoredSymbol}";
            cache += $" {objectifier.Symbol}";
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
            summary += " <color=#FFFFFFFF>★</color>";
            cache += " ★";
        }

        if (player.IsExeTarget())
        {
            summary += " <color=#CCCCCCFF>§</color>";
            cache += " §";
        }

        if (player.IsBHTarget())
        {
            summary += " <color=#B51E39FF>Θ</color>";
            cache += " Θ";
        }

        if (player.IsGuessTarget())
        {
            summary += " <color=#EEE5BEFF>π</color>";
            cache += " π";
        }

        if (player == Role.DriveHolder && !SyndicateSettings.GlobalDrive)
        {
            summary += " <color=#008000FF>Δ</color>";
            cache += " Δ";
        }

        if (player.CanDoTasks() && info[0])
        {
            if (!role.TasksDone)
            {
                summary += $" <{role.TasksCompleted}/{role.TotalTasks}>";
                cache += $" <{role.TasksCompleted}/{role.TotalTasks}>";
            }
            else
            {
                summary += " ✔";
                cache += " ✔";
            }
        }

        if (!disconnected)
        {
            summary += player.DeathReason();
            cache += player.DeathReason();
            PlayerRoles.Add(new(player.Data.PlayerName, summary, cache));
        }
        else
            Disconnected.Add(new(player.Data.PlayerName, summary, cache));
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class EndGameSummary
    {
        public static void Postfix()
        {
            PlayerRoles.Clear();
            // There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
            // AD says "Done".
            CustomPlayer.AllPlayers.ForEach(x => AddSummaryInfo(x));
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class ShipStatusSetUpPatch
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (IsHnS)
                return;

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
            winnersText.AppendLine("<size=105%><color=#00FF00FF><b>◈ - Winners - ◈</b></color></size>");
            losersText.AppendLine("<size=105%><color=#FF0000FF><b>◆ - Losers - ◆</b></color></size>");
            discText.AppendLine("<size=105%><color=#0000FFFF><b>◇ - Disconnected - ◇</b></color></size>");
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
            SaveText("Summary", roleSummaryCache.ToString(), TownOfUsReworked.Other);
            PlayerRoles.Clear();
            Disconnected.Clear();
        }
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