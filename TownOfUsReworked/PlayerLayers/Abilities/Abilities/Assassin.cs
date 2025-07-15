namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Assassin)]
public sealed class Assassin : Ability, IGuesser
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
    private static bool AssassinGuessOutcastBenign = false;

    [ToggleOption]
    private static bool AssassinGuessOutcastEvil = false;

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

    public static int RemainingKills;

    public CustomMeeting GuessMenu { get; private set; }
    public CustomGuessingMenu GuessingMenu { get; private set; }
    private int Lives;

    protected override UColor MainColor => CustomColorManager.Assassin;
    public override string Description => "- You can guess players mid-meetings";
    public override Attack Attack => Attack.Powerful;
    public override Layer Type => Handler?.CurrentFaction switch
    {
        Faction.Crew => Layer.Bullseye,
        Faction.Intruder => Layer.Hitman,
        Faction.Syndicate => Layer.Sniper,
        Faction.Pandorica => Layer.Ranger,
        Faction.Compliance => Layer.Marksman,
        Faction.Illuminati => Layer.Deadshot,
        _ when Handler && Handler.CurrentFaction.IsOutcast() => Layer.Sniper,
        _ => Layer.Assassin
    };

    public override void Init()
    {
        GuessMenu = new(Player, "Guess", Guess, IsExempt, SetLists);
        GuessingMenu = new(Player, GuessPlayer);
        Lives = AssassinChance ? AssassinChances : 0;
    }

    public override void LocalOnMeetingStart(MeetingHud __instance) => GuessMenu.GenButtons(__instance, RemainingKills > 0);

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update();

    public override void VoteComplete(MeetingHud __instance)
    {
        GuessMenu.HideButtons();
        GuessingMenu.Close();
    }

    public override void ReadRPC(RpcReader reader) => MurderPlayer(reader.ReadPlayer(), reader.Read<Layer>(), reader.ReadPlayer());

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (!Player.Is(Faction.Crew))
        {
            GuessingMenu.Mapping.Add(Layer.Crewmate);

            if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
            {
                foreach (var layer in GetValuesFromTo(Layer.Altruist, Layer.Vigilante, x => x is not (Layer.Revealer or Layer.Crewmate)))
                {
                    if (layer is Layer.Coroner or Layer.Detective or Layer.Medium or Layer.Operative or Layer.Seer or Layer.Sheriff or Layer.Tracker &&
                        !AssassinGuessInvestigative)
                    {
                        continue;
                    }

                    if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == Layer.Sheriff && !GuessingMenu.Mapping.Contains(Layer.Sheriff) &&
                        GuessingMenu.Mapping.Contains(Layer.Seer)) || (layer == Layer.Seer && !GuessingMenu.Mapping.Contains(Layer.Seer) &&
                        GuessingMenu.Mapping.Contains(Layer.Mystic)))
                    {
                        GuessingMenu.Mapping.Add(layer);
                    }
                }
            }
        }

        if (IntruderSettings.IntruderCount > 0 && !Player.Is(Faction.Intruder, Faction.Pandorica, Faction.Illuminati))
        {
            GuessingMenu.Mapping.Add(Layer.Impostor);

            if (IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
            {
                GuessingMenu.Mapping.AddRange(GetValuesFromTo(Layer.Ambusher, Layer.Wraith, x => x is not (Layer.Ghoul or Layer.Mafioso or Layer.Impostor))
                    .Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));

                if (GuessingMenu.Mapping.Contains(Layer.Godfather))
                    GuessingMenu.Mapping.Add(Layer.Mafioso);
            }
        }

        if (SyndicateSettings.SyndicateCount > 0 && !Player.Is(Faction.Syndicate, Faction.Pandorica, Faction.Illuminati))
        {
            GuessingMenu.Mapping.Add(Layer.Anarchist);

            if (SyndicateSettings.SyndicateMax > 0 && SyndicateSettings.SyndicateMin > 0)
            {
                GuessingMenu.Mapping.AddRange(GetValuesFromTo(Layer.Anarchist, Layer.Warper, x => x is not (Layer.Anarchist or Layer.Sidekick or Layer.Banshee))
                    .Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));

                if (GuessingMenu.Mapping.Contains(Layer.Rebel))
                    GuessingMenu.Mapping.Add(Layer.Sidekick);
            }
        }

        if (ApocalypseSettings.ApocalypseCount > 0 && !Player.Is(Faction.Apocalypse, Faction.Pandorica, Faction.Illuminati))
        {
            GuessingMenu.Mapping.Add(Layer.Cultist);

            if (ApocalypseSettings.ApocalypseMax > 0 && ApocalypseSettings.ApocalypseMin > 0)
            {
                GuessingMenu.Mapping.AddRange(GetValuesFromTo(Layer.Cannibal, Layer.Void, x => x != Layer.Cultist && !RoleGenManager.AD.Contains(x)).Where(layer =>
                    RoleGenManager.GetSpawnItem(layer).IsActive()));
            }

            if (AssassinGuessApoc)
                GuessingMenu.Mapping.AddRange(GuessingMenu.Mapping.WhereSelect<Layer, Layer>(Apocalypse.HarbingerToDeityMap.TryGetValue));
        }

        if (OutcastSettings.OutcastMax > 0 && OutcastSettings.OutcastMin > 0)
        {
            GuessingMenu.Mapping.AddRange(new[] { Layer.Arsonist, Layer.Cryomaniac, Layer.Glitch, Layer.SerialKiller, Layer.Juggernaut, Layer.Murderer, Layer.Werewolf,
                Layer.Dracula, Layer.Jackal, Layer.Necromancer, Layer.Whisperer, Layer.Zealot }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive() && !Player.Is(layer)));

            // Add certain Outcast roles if enabled
            if (AssassinGuessOutcastBenign)
            {
                GuessingMenu.Mapping.AddRange(new[] { Layer.Amnesiac, Layer.GuardianAngel, Layer.Survivor, Layer.Thief }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive() || (layer ==
                    Layer.Survivor && GuessingMenu.Mapping.Contains(Layer.GuardianAngel)) || (layer  == Layer.Thief && GuessingMenu.Mapping.Contains(Layer.Amnesiac))));
            }

            if (AssassinGuessOutcastEvil)
            {
                GuessingMenu.Mapping.AddRange(new[] { Layer.Executioner, Layer.Guesser, Layer.BountyHunter, Layer.Troll, Layer.Actor, Layer.Jester }.Where(layer =>
                    RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == Layer.Troll && GuessingMenu.Mapping.Contains(Layer.BountyHunter)) || (layer == Layer.Actor &&
                    GuessingMenu.Mapping.Contains(Layer.Guesser)) || (layer == Layer.Jester && GuessingMenu.Mapping.Contains(Layer.Executioner))));
            }
        }

        // Add Modifiers if enabled
        if (AssassinGuessModifiers)
            GuessingMenu.Mapping.AddRange(new[] { Layer.Bait, Layer.Diseased, Layer.Vip }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));

        // Add Dispositions if enabled
        if (AssassinGuessDispositions)
        {
            foreach (var layer in GetValuesFromTo(Layer.Allied, Layer.Traitor))
            {
                if (!RoleGenManager.GetSpawnItem(layer).IsActive() || (Player.Is(layer) && (layer is Layer.Lovers or Layer.Rivals or Layer.Linked or Layer.Mafia or
                    Layer.Corrupted)))
                {
                    continue;
                }

                GuessingMenu.Mapping.Add(layer);
            }

            if (GuessingMenu.Mapping.ContainsAny(Layer.Traitor, Layer.Fanatic))
                GuessingMenu.Mapping.Add(Layer.Betrayer);
        }

        // Add Abilities if enabled
        if (!AssassinGuessAbilities)
            return;

        foreach (var layer in GetValuesFromTo(Layer.Bullseye, Layer.Underdog))
        {
            if (!RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == Layer.Hitman && Player.Is(Faction.Intruder)) || (layer == Layer.Sniper && Player.Is(Faction.Syndicate)) ||
                (layer is Layer.Bullseye or Layer.Snitch && Player.Is(Faction.Crew)) || (layer == Layer.Ritualist && Player.Is(Faction.Apocalypse)))
            {
                continue;
            }

            GuessingMenu.Mapping.Add(layer);
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, Layer guess)
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
            var framedFlag = player.IsFramed();
            var promoterFlag = player.Is<IPromoter>(out var promoter) && ((promoter.UnderlingType == guess && promoter.IsUnderling) || (promoter.PromoterType == guess && promoter.IsPromoted));
            var actorGuessed = false;

            if (guess != Layer.Actor && player.Is<Actor>(out var actor) && actor.PretendRoles.Contains(guess))
            {
                actor.Guessed = true;
                actorGuessed = true;
            }

            var flag = layerFlag || framedFlag || promoterFlag || actorGuessed;
            var toDie = flag ? player : Player;
            RpcMurderPlayer(toDie, guess, player);

            if (!OutcastSettings.AvoidOutcastKingmakers && player != Player && actorGuessed)
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
        return player.HasDied() || Dead || Player.IsLinkedTo(player) || voteArea.NameText.text.Contains('\n') || (player == Player && Local) || Player.IsBuddyWith(player, Player.GetFaction()) ||
            RemainingKills <= 0;
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state != MeetingHud.VoteStates.Discussion && !IsExempt(voteArea))
            GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    private void RpcMurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        PerformRpcAction(player, guess, guessTarget);
    }

    private void MurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
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

            if (guess != Layer.Actor && player.Is<Actor>(out var act) && act.PretendRoles.Contains(guess))
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
            else if (Player.IsBuddyWith(LocalPlayer, Player.GetFaction()) || DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");

            return;
        }

        if (CanAttack(Attack, player.GetDefenseValue(Player)) || player == Player)
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
        else if (Player.IsBuddyWith(LocalPlayer, Player.GetFaction()) || DeadSeeEverything())
            Run("<#EC1C45FF>∮ Assassination ∮</color>", Player != player ? $"{Player.name} guessed {guessTarget.name} as {guessString}!" : $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and died!");
        else
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
    }
}