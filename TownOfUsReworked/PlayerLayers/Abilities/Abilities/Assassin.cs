namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Bullseye : Assassin
{
    public override LayerEnum Type => LayerEnum.Bullseye;
    public override string Name => "Bullseye";
}

public class Hitman : Assassin
{
    public override LayerEnum Type => LayerEnum.Hitman;
    public override string Name => "Hitman";
}

public class Slayer : Assassin
{
    public override LayerEnum Type => LayerEnum.Slayer;
    public override string Name => "Slayer";
}

public class Sniper : Assassin
{
    public override LayerEnum Type => LayerEnum.Sniper;
    public override string Name => "Sniper";
}

[HeaderOption(MultiMenu.LayerSubOptions)]
public abstract class Assassin : Ability
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number AssassinKills { get; set; } = new(0);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinMultiKill { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessNeutralBenign { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessNeutralEvil { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessInvestigative { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessApoc { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessModifiers { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessDispositions { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessAbilities { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinateAfterVoting { get; set; } = false;

    public static int RemainingKills { get; set; }

    public CustomMeeting AssassinMenu { get; set; }
    public CustomRolesMenu GuessingMenu { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Assassin : CustomColorManager.Ability;
    public override Func<string> Description => () => "- You can guess players mid-meetings";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        AssassinMenu = new(Player, "Guess", AssassinateAfterVoting, Guess, IsExempt, SetLists);
        GuessingMenu = new(Player, GuessPlayer);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        AssassinMenu.GenButtons(__instance, RemainingKills > 0);
    }

    public override void UpdateMeeting(MeetingHud __instance) => AssassinMenu.Update(__instance);

    public override void VoteComplete(MeetingHud __instance)
    {
        AssassinMenu.HideButtons();
        GuessingMenu.Close();
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        AssassinMenu.Voted();
        GuessingMenu.Close();
    }

    public override void ReadRPC(MessageReader reader) => MurderPlayer(reader.ReadPlayer(), reader.ReadEnum<LayerEnum>(), reader.ReadPlayer());

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if ((!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None)) && CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Crewmate);

            for (var h = 0; h < 26; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Revealer or LayerEnum.Crewmate || (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or
                    LayerEnum.Sheriff or LayerEnum.Tracker && !AssassinGuessInvestigative))
                {
                    continue;
                }

                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Vigilante && !GuessingMenu.Mapping.Contains(LayerEnum.Vigilante) && GuessingMenu.Mapping.Contains(LayerEnum.VampireHunter)) || (layer ==
                    LayerEnum.Sheriff && !GuessingMenu.Mapping.Contains(LayerEnum.Sheriff) && GuessingMenu.Mapping.Contains(LayerEnum.Seer)) || (layer == LayerEnum.Seer && !GuessingMenu.Mapping.Contains(LayerEnum.Seer) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Mystic)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        if ((!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)) && !SyndicateSettings.AltImps && IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
        {
            if (!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Impostor);

            for (var h = 52; h < 70; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Ghoul or LayerEnum.PromotedGodfather or LayerEnum.Impostor)
                    continue;

                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Mafioso && !GuessingMenu.Mapping.Contains(LayerEnum.Mafioso) && GuessingMenu.Mapping.Contains(LayerEnum.Godfather)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if ((!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)) && SyndicateSettings.SyndicateCount > 0)
        {
            if (!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Anarchist);

            for (var h = 70; h < 88; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Banshee or LayerEnum.PromotedRebel or LayerEnum.Anarchist)
                    continue;

                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sidekick && !GuessingMenu.Mapping.Contains(LayerEnum.Sidekick) && GuessingMenu.Mapping.Contains(LayerEnum.Rebel)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0 && !(Player.Is(Faction.Neutral) && NeutralSettings.NoSolo == NoSolo.AllNeutrals))
        {
            var nks = new List<LayerEnum>() { LayerEnum.Arsonist, LayerEnum.Glitch, LayerEnum.SerialKiller, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Cryomaniac,
                LayerEnum.Werewolf };

            foreach (var layer in nks)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive() && (!Player.Is(layer) || NeutralSettings.NoSolo == NoSolo.Never))
                    GuessingMenu.Mapping.Add(layer);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Plaguebearer).IsActive() && !Player.Is(Alignment.NeutralHarb) && !Player.Is(Alignment.NeutralApoc))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Plaguebearer);

                if (AssassinGuessApoc)
                    GuessingMenu.Mapping.Add(LayerEnum.Pestilence);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Dracula).IsActive() && !Player.Is(SubFaction.Undead))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Dracula);
                GuessingMenu.Mapping.Add(LayerEnum.Undead);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Jackal).IsActive() && !Player.Is(SubFaction.Cabal))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Jackal);
                GuessingMenu.Mapping.Add(LayerEnum.Cabal);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Necromancer).IsActive() && !Player.Is(SubFaction.Reanimated))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Necromancer);
                GuessingMenu.Mapping.Add(LayerEnum.Reanimated);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Whisperer).IsActive() && !Player.Is(SubFaction.Sect))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Whisperer);
                GuessingMenu.Mapping.Add(LayerEnum.Sect);
            }

            // Add certain Neutral roles if enabled
            if (AssassinGuessNeutralBenign)
            {
                var nbs = new List<LayerEnum>() { LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief };

                foreach (var layer in nbs)
                {
                    if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Survivor && GuessingMenu.Mapping.Contains(LayerEnum.GuardianAngel) && !GuessingMenu.Mapping.Contains(LayerEnum.Survivor)) || (layer
                        == LayerEnum.Thief && GuessingMenu.Mapping.Contains(LayerEnum.Amnesiac) && !GuessingMenu.Mapping.Contains(LayerEnum.Thief)))
                    {
                        GuessingMenu.Mapping.Add(layer);
                    }
                }
            }

            if (AssassinGuessNeutralEvil)
            {
                var nes = new List<LayerEnum>() { LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.BountyHunter, LayerEnum.Troll, LayerEnum.Actor, LayerEnum.Jester };

                foreach (var layer in nes)
                {
                    if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Troll && GuessingMenu.Mapping.Contains(LayerEnum.BountyHunter) && !GuessingMenu.Mapping.Contains(LayerEnum.Troll)) || (layer ==
                        LayerEnum.Actor && GuessingMenu.Mapping.Contains(LayerEnum.Guesser) && !GuessingMenu.Mapping.Contains(LayerEnum.Actor)) || (layer == LayerEnum.Jester && GuessingMenu.Mapping.Contains(LayerEnum.Executioner) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Jester)))
                    {
                        GuessingMenu.Mapping.Add(layer);
                    }
                }
            }
        }

        // Add Modifiers if enabled
        if (AssassinGuessModifiers)
        {
            var mods = new List<LayerEnum>() { LayerEnum.Bait, LayerEnum.Diseased, LayerEnum.Professional, LayerEnum.VIP };

            foreach (var layer in mods)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive())
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        // Add Dispositions if enabled
        if (AssassinGuessDispositions)
        {
            for (var h = 107; h < 118; h++)
            {
                var layer = (LayerEnum)h;

                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    if (Player.Is(layer) && ((layer is LayerEnum.Lovers or LayerEnum.Rivals or LayerEnum.Linked or LayerEnum.Mafia) || (layer == LayerEnum.Corrupted &&
                        Corrupted.AllCorruptedWin)))
                    {
                        continue;
                    }

                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        // Add Abilities if enabled
        if (AssassinGuessAbilities)
        {
            for (var h = 107; h < 118; h++)
            {
                var layer = (LayerEnum)h;

                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    if ((layer == LayerEnum.Hitman && Player.Is(Faction.Intruder)) || (layer == LayerEnum.Sniper && Player.Is(Faction.Syndicate)) || (layer is LayerEnum.Bullseye or
                        LayerEnum.Snitch && Player.Is(Faction.Crew)))
                    {
                        continue;
                    }

                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, LayerEnum guess)
    {
        if (Dead || Meeting().state == MeetingHud.VoteStates.Discussion || !panel || RemainingKills <= 0)
            return;

        if (GuessingMenu.SelectedPanel != panel)
        {
            if (GuessingMenu.SelectedPanel)
                GuessingMenu.SelectedPanel.Background.color = UColor.white;

            GuessingMenu.SelectedPanel = panel;
            GuessingMenu.SelectedPanel.Background.color = LayerDictionary[guess].Color.Alternate(0.4f);
        }
        else
        {
            var layerflag = player.GetLayers().Any(x => x.Type == guess);
            var subfactionflag = player.GetSubFaction().ToString() == guess.ToString();
            var framedflag = player.IsFramed();

            if (player.Is(LayerEnum.Actor) && guess != LayerEnum.Actor)
            {
                var actor = player.GetLayer<Actor>();

                if (actor.PretendRoles.Any(x => x.Type == guess))
                {
                    actor.Guessed = true;
                    CallRpc(CustomRPC.WinLose, WinLose.ActorWins, actor);

                    if (!NeutralSettings.AvoidNeutralKingmakers)
                        RpcMurderPlayer(Player, guess, player);
                }
            }

            var flag = layerflag || subfactionflag || framedflag;
            var toDie = flag ? player : Player;
            RpcMurderPlayer(toDie, guess, player);

            if (RemainingKills <= 0 || !AssassinMultiKill)
                AssassinMenu.HideButtons();
            else
                AssassinMenu.HideSingle(player.PlayerId);

            GuessingMenu.SelectedPanel = null;
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && ((Player.GetFaction() != player.GetFaction()) || (Player.GetSubFaction() != Player.GetSubFaction()))) || Dead ||
            (player == Player && Local) || (Player.GetFaction() == player.GetFaction() && Player.GetFaction() != Faction.Crew) | RemainingKills <= 0 || (Player.GetSubFaction() ==
            player.GetSubFaction() && Player.GetSubFaction() != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public void RpcMurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Local && player == Player)
            AssassinMenu.HideButtons();

        if (player.Is(LayerEnum.Indomitable) && player != Player)
        {
            if (Local)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You failed to assassinate {guessTarget.name}!");
            else if (player.AmOwner)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"Someone tried to assassinate you!");

            Flash(CustomColorManager.Indomitable);
            player.GetLayer<Indomitable>().AttemptedGuess = true;
        }

        if (Player.Is(LayerEnum.Professional) && Player == player)
        {
            var modifier = Player.GetLayer<Professional>();

            if (!modifier.LifeUsed)
            {
                modifier.LifeUsed = true;
                AssassinMenu.HideSingle(guessTarget.PlayerId);

                if (Local)
                {
                    Flash(modifier.Color);
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");
                }
                else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything())
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");

                return;
            }
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            RemainingKills--;
            MarkMeetingDead(player, Player);

            if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
            {
                var otherLover = player.GetOtherLover();

                if (!otherLover.Is(Alignment.NeutralApoc))
                    RpcMurderPlayer(otherLover, guess, guessTarget);
            }
        }

        if (Local)
        {
            if (Player != player)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guessString}!");
            else
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and died!");
        }
        else if (Player != player && player.AmOwner)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessString}!");
        else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything())
        {
            if (Player != player)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guessString}!");
            else
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and died!");
        }
        else
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
    }
}