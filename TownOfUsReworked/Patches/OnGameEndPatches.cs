namespace TownOfUsReworked.Patches;

public static class OnGameEndPatch
{
    private static readonly List<WinningPlayerData> PotentialWinners = new();

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClient_OnGameEnd
    {
        public static void Postfix()
        {
            PotentialWinners.Clear();
            CustomPlayer.AllPlayers.ForEach(x => PotentialWinners.Add(new(x.Data)));
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class ShipStatus_SetEverythingUp
    {
        public static void Prefix()
        {
            var winners = new List<WinningPlayerData>();

            if (Role.AllNeutralsWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Neutral))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.NKWins)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralKill))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CrewWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Crew))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Crew)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Crew && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.IntruderWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Intruder))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(LayerEnum.Traitor))
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == traitor.PlayerName));
                }

                foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(LayerEnum.Fanatic))
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == fanatic.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Intruder && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.SyndicateWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(LayerEnum.Traitor))
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == traitor.PlayerName));
                }

                foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(LayerEnum.Fanatic))
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == fanatic.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Syndicate && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.UndeadWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CabalWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.SectWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ReanimatedWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ApocalypseWins)
            {
                foreach (var role2 in Role.GetRoles(Alignment.NeutralApoc))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var role2 in Role.GetRoles(Alignment.NeutralHarb))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.GlitchWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Glitch))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.JuggernautWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Juggernaut))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ArsonistWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Arsonist))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.SerialKillerWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.SerialKiller))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.MurdererWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Murderer))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.WerewolfWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Werewolf))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CryomaniacWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Cryomaniac))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.PhantomWins)
            {
                foreach (var role2 in Role.GetRoles<Phantom>(LayerEnum.Phantom))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.CompletedTasks)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.TaskRunnerWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Runner))
                {
                    if (!role2.Disconnected && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.HunterWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Hunter))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.HuntedWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Hunted))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Objectifier.LoveWins)
            {
                foreach (var lover in Objectifier.GetObjectifiers(LayerEnum.Lovers))
                {
                    if (!lover.Disconnected && lover.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == lover.PlayerName));
                }
            }
            else if (Objectifier.RivalWins)
            {
                foreach (var rival in Objectifier.GetObjectifiers(LayerEnum.Rivals))
                {
                    if (!rival.Disconnected && rival.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == rival.PlayerName));
                }
            }
            else if (Objectifier.TaskmasterWins)
            {
                foreach (var tm in Objectifier.GetObjectifiers(LayerEnum.Taskmaster))
                {
                    if (!tm.Disconnected && tm.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == tm.PlayerName));
                }
            }
            else if (Objectifier.OverlordWins)
            {
                foreach (var ov in Objectifier.GetObjectifiers(LayerEnum.Overlord))
                {
                    if (ov.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ov.PlayerName));
                }
            }
            else if (Objectifier.CorruptedWins)
            {
                foreach (var corr in Objectifier.GetObjectifiers(LayerEnum.Corrupted))
                {
                    if (!corr.Disconnected && corr.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == corr.PlayerName));
                }
            }
            else if (Objectifier.MafiaWins)
            {
                foreach (var maf in Objectifier.GetObjectifiers(LayerEnum.Mafia))
                {
                    if (!maf.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == maf.PlayerName));
                }
            }

            if (!Objectifier.ObjectifierWins)
            {
                if (!(Role.ActorWins || Role.BountyHunterWins || Role.CannibalWins || Role.ExecutionerWins || Role.GuesserWins || Role.JesterWins || Role.TrollWins) ||
                    !CustomGameOptions.NeutralEvilsEndGame)
                {
                    foreach (var surv in Role.GetRoles<Survivor>(LayerEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(PotentialWinners.First(x => x.PlayerName == surv.PlayerName));
                    }

                    foreach (var ga in Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel))
                    {
                        if (!ga.Failed && ga.TargetPlayer != null && ga.TargetAlive)
                            winners.Add(PotentialWinners.First(x => x.PlayerName == ga.PlayerName));
                    }
                }

                foreach (var jest in Role.GetRoles<Jester>(LayerEnum.Jester))
                {
                    if (jest.VotedOut && !jest.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == jest.PlayerName));
                }

                foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
                {
                    if (exe.TargetVotedOut && !exe.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == exe.PlayerName));
                }

                foreach (var bh in Role.GetRoles<BountyHunter>(LayerEnum.BountyHunter))
                {
                    if (bh.TargetKilled && !bh.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == bh.PlayerName));
                }

                foreach (var act in Role.GetRoles<Actor>(LayerEnum.Actor))
                {
                    if (act.Guessed && !act.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == act.PlayerName));
                }

                foreach (var cann in Role.GetRoles<Cannibal>(LayerEnum.Cannibal))
                {
                    if (cann.Eaten && !cann.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == cann.PlayerName));
                }

                foreach (var guess in Role.GetRoles<Guesser>(LayerEnum.Guesser))
                {
                    if (guess.TargetGuessed && !guess.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == guess.PlayerName));
                }

                foreach (var troll in Role.GetRoles<Troll>(LayerEnum.Troll))
                {
                    if (troll.Killed && !troll.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == troll.PlayerName));
                }

                foreach (var link in Objectifier.GetObjectifiers<Linked>(LayerEnum.Linked))
                {
                    if (winners.Any(x => x.PlayerName == link.PlayerName) && !winners.Any(x => x.PlayerName == link.OtherLink.Data.PlayerName))
                        winners.Add(PotentialWinners.First(x => x.PlayerName == link.OtherLink.Data.PlayerName));
                }
            }

            winners = winners.Distinct().ToList();
            TempData.winners.Clear();
            TempData.winners = winners.SystemToIl2Cpp();
        }
    }
}

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
[HarmonyPriority(Priority.First)]
public static class Outro
{
    public static void Postfix(EndGameManager __instance)
    {
        if (!SoundEffects.ContainsKey("CrewWin"))
            SoundEffects.Add("CrewWin", __instance.CrewStinger);

        if (!SoundEffects.ContainsKey("IntruderWin"))
            SoundEffects.Add("IntruderWin", __instance.ImpostorStinger);

        if (!SoundEffects.ContainsKey("Stalemate"))
            SoundEffects.Add("Stalemate", __instance.DisconnectStinger);

        if (!GameHasEnded)
            return;

        var text = UObject.Instantiate(__instance.WinText, __instance.WinText.transform.parent);
        SoundManager.Instance.StopSound(__instance.ImpostorStinger);
        var winsound = "IntruderWin";
        var color = Colors.Stalemate;
        var texttext = "Stalemate";

        foreach (var player in UObject.FindObjectsOfType<PoolablePlayer>())
        {
            var local = Role.AllRoles.Find(x => x.PlayerName == player.NameText().text);
            player.NameText().text = $"{local.ColorString}<size=75%>{local}</size>\n<size=90%>{player.NameText().text}</size></color>";
        }

        if (PlayerLayer.NobodyWins)
            winsound = "Stalemate";
        else if (Role.SyndicateWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Syndicate && x.Faithful);
            texttext = "The Syndicate Wins";
            color = role.FactionColor;
        }
        else if (Role.IntruderWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Intruder && x.Faithful);
            texttext = "Intruders Win";
            color = role.FactionColor;
        }
        else if (Role.AllNeutralsWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Neutral && x.Faithful);
            texttext = "Neutrals Win";
            color = role.FactionColor;
        }
        else if (Role.CrewWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Crew && x.Faithful);
            texttext = "Crew Wins";
            color = role.FactionColor;
            winsound = "CrewWin";
        }
        else if (Role.NKWins)
        {
            var role = Role.AllRoles.Find(x => x.Alignment == Alignment.NeutralKill && x.Winner);
            texttext = "Neutral Killers Win";
            color = Colors.Alignment;
        }
        else if (Role.ApocalypseWins)
        {
            var role = Role.AllRoles.Find(x => x.Alignment is Alignment.NeutralApoc or Alignment.NeutralHarb && x.Faithful);
            texttext = "The Apocalypse Is Nigh";
            color = Colors.Apocalypse;
        }
        else if (Role.UndeadWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Undead && Role.UndeadWin);
            texttext = "The Undead Win";
            color = role.SubFactionColor;
        }
        else if (Role.CabalWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Cabal && Role.CabalWin);
            texttext = "The Cabal Wins";
            color = role.SubFactionColor;
        }
        else if (Role.SectWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Sect && Role.SectWin);
            texttext = "The Sect Wins";
            color = role.SubFactionColor;
        }
        else if (Role.ReanimatedWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Reanimated && Role.ReanimatedWin);
            texttext = "The Reanimated Win";
            color = role.SubFactionColor;
        }
        else if (Role.CryomaniacWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Cryomaniac && x.Winner);
            texttext = "Cryomaniac Wins";
            color = role.Color;
        }
        else if (Role.ArsonistWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Arsonist && x.Winner);
            texttext = "Aronist Wins";
            color = role.Color;
        }
        else if (Role.GlitchWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Glitch && x.Winner);
            texttext = "Glitch Wins";
            color = role.Color;
        }
        else if (Role.JuggernautWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Juggernaut && x.Winner);
            texttext = "Juggernaut Wins";
            color = role.Color;
        }
        else if (Role.MurdererWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Murderer && x.Winner);
            texttext = "Murderer Wins";
            color = role.Color;
        }
        else if (Role.SerialKillerWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.SerialKiller && x.Winner);
            texttext = "Serial Killer Wins";
            color = role.Color;
        }
        else if (Role.WerewolfWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Werewolf && x.Winner);
            texttext = "Werewolf Wins";
            color = role.Color;
        }
        else if (Role.PhantomWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Phantom && x.Winner);
            texttext = "Phantom Wins";
            color = role.Color;
        }
        else if (Role.ActorWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Actor && x.Winner);
            texttext = "Actor Wins";
            color = role.Color;
        }
        else if (Role.BountyHunterWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.BountyHunter && x.Winner);
            texttext = "Bounty Hunter Wins";
            color = role.Color;
        }
        else if (Role.CannibalWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Cannibal && x.Winner);
            texttext = "Cannibal Wins";
            color = role.Color;
        }
        else if (Role.ExecutionerWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Executioner && x.Winner);
            texttext = "Executioner Wins";
            color = role.Color;
        }
        else if (Role.GuesserWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Guesser && x.Winner);
            texttext = "Guesser Wins";
            color = role.Color;
        }
        else if (Role.JesterWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Jester && x.Winner);
            texttext = "Jester Wins";
            color = role.Color;
        }
        else if (Role.TrollWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Troll && x.Winner);
            texttext = "Troll Wins";
            color = role.Color;
        }
        else if (Role.TaskRunnerWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Runner && x.Winner);
            texttext = "Tasks Completed";
            color = role.Color;
        }
        else if (Role.HuntedWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Hunted);
            texttext = "The Hunted Survived";
            color = role.Color;
        }
        else if (Role.HunterWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Hunter);
            texttext = "Every One Was Hunted";
            color = role.Color;
        }
        else if (Objectifier.CorruptedWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Corrupted && x.Winner);
            texttext = "Corrupted Wins";
            color = obj.Color;
        }
        else if (Objectifier.LoveWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Lovers && x.Winner);
            texttext = "Love Wins";
            color = obj.Color;
        }
        else if (Objectifier.RivalWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Rivals && x.Winner);
            texttext = "Rival Wins";
            color = obj.Color;
        }
        else if (Objectifier.TaskmasterWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Taskmaster && x.Winner);
            texttext = "Taskmaster Wins";
            color = obj.Color;
        }
        else if (Objectifier.OverlordWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Overlord && x.Winner);
            texttext = "Overlord Wins";
            color = obj.Color;
        }
        else if (Objectifier.MafiaWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Mafia && x.Winner);
            texttext = "The Mafia Wins";
            color = obj.Color;
        }

        __instance.BackgroundBar.material.color = text.color = color;
        var pos = __instance.WinText.transform.localPosition;
        pos.y += 1.5f;
        __instance.WinText.transform.localPosition = pos;
        text.text = $"<size=50%>{texttext}!</size>";
        Play(winsound);
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
public static class EndGame
{
    public static void Reset()
    {
        foreach (var role in Role.AllRoles)
        {
            role.AllPrints.ForEach(x => x.Destroy());
            role.AllPrints.Clear();
        }
    }

    public static void Prefix() => Reset();
}

public static class Summary
{
    private static readonly List<SummaryInfo> PlayerRoles = new();
    public static readonly List<SummaryInfo> Disconnected = new();

    public static void AddSummaryInfo(PlayerControl player, bool disconnected = false)
    {
        var summary = "";
        var cache = "";

        var info = player.AllPlayerInfo();
        var role = info[0] as Role;
        var modifier = info[1] as Modifier;
        var ability = info[2] as Ability;
        var objectifier = info[3] as Objectifier;

        if (info[0])
        {
            if (role.RoleHistory.Count != 0)
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

        if (objectifier?.Type != LayerEnum.None)
        {
            summary += $" {objectifier?.ColoredSymbol}";
            cache += $" {objectifier?.Symbol}";
        }

        if (modifier?.Type != LayerEnum.None)
        {
            summary += $" ({modifier?.ColorString}{modifier?.Name}</color>)";
            cache += $" ({modifier?.Name})";
        }

        if (ability?.Type != LayerEnum.None)
        {
            summary += $" [{ability?.ColorString}{ability?.Name}</color>]";
            cache += $" [{ability?.Name}]";
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

        if (player == Role.DriveHolder && !CustomGameOptions.GlobalDrive)
        {
            summary += " <color=#008000FF>Δ</color>";
            cache += " Δ";
        }

        if (player.CanDoTasks())
        {
            if (role.TasksCompleted != role.TotalTasks)
            {
                summary += $" <{role.TasksCompleted}/{role.TotalTasks}>";
                cache += $" <{role.TasksCompleted}/{role.TotalTasks}>";
            }
            else
            {
                summary += $" ✔";
                cache += $" ✔";
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
            //There's a better way of doing this e.g. switch statement or dictionary. But this works for now.
            //AD says "Done".
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
            var discCount = 0;

            roleSummaryText.AppendLine("<size=125%><u><b>End Game Summary</b></u>:</size>");
            roleSummaryText.AppendLine();
            roleSummaryCache.AppendLine("End Game Summary:");
            roleSummaryCache.AppendLine();
            winnersText.AppendLine("<size=105%><b>Winners</b></size>");
            losersText.AppendLine("<size=105%><b>Losers</b></size>");
            discText.AppendLine("<size=105%><b>Disconnected</b></size>");
            winnersCache.AppendLine("Winners");
            losersCache.AppendLine("Losers");
            discCache.AppendLine("Disconnected");

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
                discCount++;
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

            if (discCount > 0)
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
            SaveText("Summary", roleSummaryCache.ToString());
            PlayerRoles.Clear();
            Disconnected.Clear();
        }
    }

    private static bool IsWinner(this string playerName) => TempData.winners.Any(x => x.PlayerName == playerName);

    private static string DeathReason(this PlayerControl player)
    {
        if (player == null)
            return "";

        var role = Role.GetRole(player);

        if (role == null)
            return "";

        var die = role.DeathReason is not DeathReasonEnum.Alive ? $" | {role.DeathReason}" : "";

        if (role.DeathReason is not DeathReasonEnum.Alive and not DeathReasonEnum.Ejected and not DeathReasonEnum.Suicide and not DeathReasonEnum.Escaped)
            die += role.KilledBy;

        return die;
    }
}