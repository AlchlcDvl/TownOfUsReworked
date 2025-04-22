namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Bullseye : Assassin
{
    public override LayerEnum Type => LayerEnum.Bullseye;
}

public sealed class Hitman : Assassin
{
    public override LayerEnum Type => LayerEnum.Hitman;
}

public sealed class Slayer : Assassin
{
    public override LayerEnum Type => LayerEnum.Slayer;
}

public sealed class Sniper : Assassin
{
    public override LayerEnum Type => LayerEnum.Sniper;
}

public sealed class Ritualist : Assassin
{
    public override LayerEnum Type => LayerEnum.Ritualist;
}

[LayerHeaderOption(LayerEnum.Assassin)]
public abstract class Assassin : Ability, IGuesser
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number AssassinKills = 0;

    [ToggleOption]
    private static bool AssassinChance = false;

    [NumberOption(1, 15, 1)]
    private static Number AssassinChances = 0;

    [ToggleOption]
    private static bool AssassinMultiKill = false;

    [ToggleOption]
    private static bool AssassinGuessNeutralBenign = false;

    [ToggleOption]
    private static bool AssassinGuessNeutralEvil = false;

    [ToggleOption]
    private static bool AssassinGuessInvestigative = false;

    [ToggleOption]
    private static bool AssassinGuessApoc = false;

    [ToggleOption]
    private static bool AssassinGuessModifiers = false;

    [ToggleOption]
    private static bool AssassinGuessDispositions = false;

    [ToggleOption]
    private static bool AssassinGuessAbilities = false;

    public static int RemainingKills { get; set; }

    public CustomMeeting GuessMenu { get; private set; }
    public CustomGuessingMenu GuessingMenu { get; private set; }
    private int Lives { get; set; }

    protected override UColor MainColor => CustomColorManager.Assassin;
    public override Func<string> Description => () => "- You can guess players mid-meetings";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    protected override void Init()
    {
        GuessMenu = new(Player, "Guess", Guess, IsExempt, SetLists);
        GuessingMenu = new(Player, GuessPlayer);
        Lives = AssassinChance ? AssassinChances : 0;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GuessMenu.GenButtons(__instance, RemainingKills > 0);
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update();

    public override void VoteComplete(MeetingHud __instance)
    {
        GuessMenu.HideButtons();
        GuessingMenu.Close();
    }

    public override void ReadRPC(NetData reader) => MurderPlayer(reader.ReadPlayer(), reader.Read<LayerEnum>(), reader.ReadPlayer());

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0 && (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None)))
        {
            if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Crewmate);

            foreach (var layer in GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Vigilante, x => x is not (LayerEnum.Revealer or LayerEnum.Crewmate)))
            {
                if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker &&
                    !AssassinGuessInvestigative)
                {
                    continue;
                }

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sheriff && !GuessingMenu.Mapping.Contains(LayerEnum.Sheriff) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Seer)) || (layer == LayerEnum.Seer && !GuessingMenu.Mapping.Contains(LayerEnum.Seer) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Mystic)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        if (IntruderSettings.IntruderCount > 0 && (!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)))
        {
            if (!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Impostor);

            if (IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
            {
                foreach (var layer in GetValuesFromTo(LayerEnum.Ambusher, LayerEnum.Wraith, x => x is not (LayerEnum.Ghoul or LayerEnum.Mafioso or
                    LayerEnum.Impostor)))
                {
                    if (RoleGenManager.GetSpawnItem(layer).IsActive())
                    {
                        GuessingMenu.Mapping.Add(layer);

                        if (layer == LayerEnum.Godfather)
                            GuessingMenu.Mapping.Add(LayerEnum.Mafioso);
                    }
                }
            }
        }

        if (SyndicateSettings.SyndicateCount > 0 && (!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)))
        {
            if (!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Anarchist);

            foreach (var layer in GetValuesFromTo(LayerEnum.Anarchist, LayerEnum.Warper, x => x is not (LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Banshee)))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive())
                {
                    GuessingMenu.Mapping.Add(layer);

                    if (layer == LayerEnum.Rebel)
                        GuessingMenu.Mapping.Add(LayerEnum.Sidekick);
                }
            }
        }

        if (ApocalypseSettings.ApocalypseCount > 0 && (!Player.Is(Faction.Apocalypse) || !Player.Is(SubFaction.None)))
        {
            if (!Player.Is(Faction.Apocalypse) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Cultist);

            if (RoleGenManager.GetSpawnItem(LayerEnum.Plaguebearer).IsActive())
            {
                GuessingMenu.Mapping.Add(LayerEnum.Plaguebearer);

                if (AssassinGuessApoc)
                    GuessingMenu.Mapping.Add(LayerEnum.Pestilence);
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0 && !Player.Is(Faction.Neutral))
        {
            GuessingMenu.Mapping.AddRange(new[] { LayerEnum.Arsonist, LayerEnum.Glitch, LayerEnum.SerialKiller, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Cryomaniac, LayerEnum.Werewolf }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive() && !Player.Is(layer)));

            if (RoleGenManager.GetSpawnItem(LayerEnum.Dracula).IsActive() && !Player.Is(SubFaction.Undead))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Dracula);
                GuessingMenu.Mapping.Add(LayerEnum.Undead);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Jackal).IsActive() && !Player.Is(SubFaction.Cabal))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Jackal);
                GuessingMenu.Mapping.Add(LayerEnum.Cabal);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Necromancer).IsActive() && !Player.Is(SubFaction.Reanimated))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Necromancer);
                GuessingMenu.Mapping.Add(LayerEnum.Reanimated);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Whisperer).IsActive() && !Player.Is(SubFaction.Cult))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Whisperer);
                GuessingMenu.Mapping.Add(LayerEnum.Cult);
            }

            // Add certain Neutral roles if enabled
            if (AssassinGuessNeutralBenign)
            {
                GuessingMenu.Mapping.AddRange(new[] { LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Survivor && GuessingMenu.Mapping.Contains(LayerEnum.GuardianAngel) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Survivor)) || (layer  == LayerEnum.Thief && GuessingMenu.Mapping.Contains(LayerEnum.Amnesiac) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Thief))));
            }

            if (AssassinGuessNeutralEvil)
            {
                GuessingMenu.Mapping.AddRange(new[] { LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.BountyHunter, LayerEnum.Troll, LayerEnum.Actor, LayerEnum.Jester }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Troll && GuessingMenu.Mapping.Contains(LayerEnum.BountyHunter) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Troll)) || (layer == LayerEnum.Actor && GuessingMenu.Mapping.Contains(LayerEnum.Guesser) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Actor)) || (layer == LayerEnum.Jester && GuessingMenu.Mapping.Contains(LayerEnum.Executioner) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Jester))));
            }
        }

        // Add Modifiers if enabled
        if (AssassinGuessModifiers)
            GuessingMenu.Mapping.AddRange(new[] { LayerEnum.Bait, LayerEnum.Diseased, LayerEnum.Vip }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));

        // Add Dispositions if enabled
        if (AssassinGuessDispositions)
        {
            foreach (var layer in GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor))
            {
                if (!RoleGenManager.GetSpawnItem(layer).IsActive() || (Player.Is(layer) && (layer is LayerEnum.Lovers or LayerEnum.Rivals or LayerEnum.Linked or LayerEnum.Mafia or
                    LayerEnum.Corrupted)))
                {
                    continue;
                }

                GuessingMenu.Mapping.Add(layer);
            }

            if (GuessingMenu.Mapping.ContainsAny(LayerEnum.Traitor, LayerEnum.Fanatic))
                GuessingMenu.Mapping.Add(LayerEnum.Betrayer);
        }

        // Add Abilities if enabled
        if (!AssassinGuessAbilities)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Bullseye, LayerEnum.Underdog))
        {
            if (!RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Hitman && Player.Is(Faction.Intruder)) || (layer == LayerEnum.Sniper && Player.Is(Faction.Syndicate)) ||
                (layer is LayerEnum.Bullseye or LayerEnum.Snitch && Player.Is(Faction.Crew)) || (layer == LayerEnum.Ritualist && Player.Is(Faction.Apocalypse)))
            {
                continue;
            }

            GuessingMenu.Mapping.Add(layer);
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
            var layerFlag = player.GetLayers().Any(x => x.Type == guess);
            var subfactionFlag = $"{player.GetSubFaction()}" == $"{guess}";
            var framedFlag = player.IsFramed();
            var promoterFlag = player.Is<IPromoter>(out var promoter) && ((promoter.UnderlingType == guess && promoter.IsUnderling) || (promoter.PromoterType == guess && promoter.IsPromoted));
            var actorGuessed = false;

            if (guess != LayerEnum.Actor && player.Is<Actor>(out var actor) && actor.PretendRoles.Any(x => x.Type == guess))
            {
                actor.Guessed = true;
                actorGuessed = true;
            }

            var flag = layerFlag || subfactionFlag || framedFlag || promoterFlag || actorGuessed;
            var toDie = flag ? player : Player;
            RpcMurderPlayer(toDie, guess, player);

            if (!NeutralSettings.AvoidNeutralKingmakers && player != Player && actorGuessed)
                RpcMurderPlayer(Player, guess, player);

            if (RemainingKills <= 0 || !AssassinMultiKill)
                GuessMenu.HideButtons();
            else
                GuessMenu.HideSingle(player.PlayerId);

            GuessingMenu.SelectedPanel = null;
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && ((Player.GetFaction() != player.GetFaction()) || (Player.GetSubFaction() != player.GetSubFaction()))) || Dead ||
            (player == Player && Local) || (Player.GetFaction() == player.GetFaction() && Player.GetFaction() != Faction.Crew) || RemainingKills <= 0 || (Player.GetSubFaction() ==
            player.GetSubFaction() && Player.GetSubFaction() != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    private void RpcMurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player, guess, guessTarget);
    }

    private void MurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Local && player == Player)
            GuessMenu.HideButtons();

        if (player != Player)
        {
            if (player.Is<Indomitable>(out var ind))
            {
                if (Local)
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You failed to assassinate {guessTarget.name}!");
                else if (player.AmOwner)
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", "Someone tried to assassinate you!");

                Flash(CustomColorManager.Indomitable);
                ind.AttemptedGuess = true;
            }

            if (guess != LayerEnum.Actor && player.Is<Actor>(out var act) && act.PretendRoles.Any(x => x.Type == guess))
                    act.Guessed = true;
        }

        if (Player == player && Lives <= 0)
        {
            Lives--;
            GuessMenu.HideSingle(guessTarget.PlayerId);

            if (Local)
            {
                Flash(Color);
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");
            }
            else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is not (Faction.Crew or Faction.Neutral))) || DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");

            return;
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            RemainingKills--;
            MarkMeetingDead(player, Player);

            if (Lovers.BothLoversDie && AmongUsClient.Instance.AmHost && player.Is<Lovers>(out var lovers) && (!lovers.Other.Is(Alignment.Deity) || AssassinGuessApoc))
                RpcMurderPlayer(lovers.Other, guess, guessTarget);
        }

        if (Local)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", Player != player ? $"You guessed {guessTarget.name} as {guessString}!" : $"You incorrectly guessed {guessTarget.name} as {guessString} and died!");
        else if (Player != player && player.AmOwner)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessString}!");
        else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is not (Faction.Crew or Faction.Neutral))) || DeadSeeEverything())
            Run("<#EC1C45FF>∮ Assassination ∮</color>", Player != player ? $"{Player.name} guessed {guessTarget.name} as {guessString}!" : $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and died!");
        else
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
    }
}