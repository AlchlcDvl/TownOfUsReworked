namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Guesser : Evil
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuesserCanPickTargets { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuesserButton { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuessVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuessSwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuessTargetKnows { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MultipleGuesses { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxGuesses { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuesserAfterVoting { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuessToAct { get; set; } = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetGuessed { get; set; }
    public int RemainingGuesses { get; set; }
    public bool FactionHintGiven { get; set; }
    public bool AlignmentHintGiven { get; set; }
    private int LettersGiven { get; set; }
    private bool LettersExhausted { get; set; }
    private string RoleName { get; set; }
    public List<string> Letters { get; } = [];
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer ? (!TargetGuessed && (RemainingGuesses <= 0 || TargetPlayer.HasDied())) : Rounds > 2;
    public CustomMeeting GuessMenu { get; set; }
    public CustomRolesMenu GuessingMenu { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Guesser : FactionColor;
    public override string Name => "Guesser";
    public override LayerEnum Type => LayerEnum.Guesser;
    public override Func<string> StartText => () => "Guess What Someone Might Be";
    public override Func<string> Description => () => !TargetPlayer ? "- You can select a player to guess their role" : ((TargetGuessed ? "- You can guess player's roles without penalties" :
        $"- You can only try to guess {TargetPlayer?.name}") + $"\n- If {TargetPlayer?.name} dies without getting guessed by you, you will become an <#00ACC2FF>Actor</color>");
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => TargetGuessed;
    public override WinLose EndState => WinLose.GuesserWins;

    public override void Init()
    {
        base.Init();
        RemainingGuesses = MaxGuesses == 0 ? 10000 : MaxGuesses;
        Objectives = () => TargetGuessed ? $"- You have found out what {TargetPlayer.name} was" : (!TargetPlayer ? "- Find someone to be guessed by you" : ("- Guess " +
            $"{TargetPlayer?.name}'s role"));
        GuessMenu = new(Player, "Guess", GuesserAfterVoting, Guess, IsExempt, SetLists);
        Rounds = 0;
        Letters.Clear();
        GuessingMenu = new(Player, GuessPlayer);
    }

    public override void PostAssignment()
    {
        if (GuesserCanPickTargets || !TargetPlayer)
        {
            TargetButton ??= new(this, new SpriteName("GuessTarget"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SelectTarget, (PlayerBodyExclusion)Exception, "AGONISE",
                (UsableFunc)Usable);
        }
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.Add(TargetPlayer);
        return team;
    }

    public bool Exception(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.CrewInvest) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public void SelectTarget(PlayerControl target)
    {
        TargetPlayer = target;
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, TargetPlayer);
    }

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        GuessingMenu.Mapping.Add(LayerEnum.Crewmate);

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            for (var h = 0; h < 26; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Revealer or LayerEnum.Crewmate or LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or
                    LayerEnum.Sheriff or LayerEnum.Tracker)
                {
                    continue;
                }

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Vigilante && !GuessingMenu.Mapping.Contains(LayerEnum.Vigilante) && GuessingMenu.Mapping.Contains(LayerEnum.VampireHunter)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (!SyndicateSettings.AltImps && IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
        {
            GuessingMenu.Mapping.Add(LayerEnum.Impostor);

            for (var h = 52; h < 70; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Ghoul or LayerEnum.PromotedGodfather or LayerEnum.Impostor)
                    continue;

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Mafioso && !GuessingMenu.Mapping.Contains(LayerEnum.Mafioso) && GuessingMenu.Mapping.Contains(LayerEnum.Godfather)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (SyndicateSettings.SyndicateCount > 0)
        {
            GuessingMenu.Mapping.Add(LayerEnum.Anarchist);

            for (var h = 70; h < 88; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Banshee or LayerEnum.PromotedRebel or LayerEnum.Anarchist)
                    continue;

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sidekick && !GuessingMenu.Mapping.Contains(LayerEnum.Sidekick) && GuessingMenu.Mapping.Contains(LayerEnum.Rebel)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0)
        {
            for (var h = 26; h < 52; h++)
            {
                var layer = (LayerEnum)h;

                if (layer == LayerEnum.Phantom)
                    continue;

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Survivor && GuessingMenu.Mapping.Contains(LayerEnum.GuardianAngel) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Survivor)) || (layer == LayerEnum.Thief && GuessingMenu.Mapping.Contains(LayerEnum.Amnesiac) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Thief)) || (layer == LayerEnum.Troll && GuessingMenu.Mapping.Contains(LayerEnum.BountyHunter)
                    && !GuessingMenu.Mapping.Contains(LayerEnum.Troll)) || (layer == LayerEnum.Actor && GuessingMenu.Mapping.Contains(LayerEnum.Guesser) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Actor)) || (layer == LayerEnum.Jester && GuessingMenu.Mapping.Contains(LayerEnum.Executioner) &&
                    !GuessingMenu.Mapping.Contains(LayerEnum.Jester)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }

            if (GuessingMenu.Mapping.Contains(LayerEnum.Whisperer))
                GuessingMenu.Mapping.Add(LayerEnum.Sect);

            if (GuessingMenu.Mapping.Contains(LayerEnum.Necromancer))
                GuessingMenu.Mapping.Add(LayerEnum.Reanimated);

            if (GuessingMenu.Mapping.Contains(LayerEnum.Jackal))
                GuessingMenu.Mapping.Add(LayerEnum.Cabal);

            if (GuessingMenu.Mapping.Contains(LayerEnum.Dracula))
                GuessingMenu.Mapping.Add(LayerEnum.Undead);
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, LayerEnum guess)
    {
        if (Dead || Meeting().state == MeetingHud.VoteStates.Discussion || !panel || RemainingGuesses <= 0)
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
            var subfactionflag = $"{player.GetSubFaction()}" == $"{guess}";

            var flag = layerflag || subfactionflag;
            var toDie = flag ? player : Player;
            TargetGuessed = flag;
            RpcMurderPlayer(toDie, guess, player);

            if (RemainingGuesses <= 0 || !MultipleGuesses)
                GuessMenu.HideButtons();
            else
                GuessMenu.HideSingle(player.PlayerId);

            GuessingMenu.SelectedPanel = null;
        }
    }

    public void TurnAct()
    {
        Role role = IsRoleList() ? new Jester() : new Actor();
        role.RoleUpdate(this);
    }

    public bool Usable() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Failed && !Dead && !GuessToAct)
        {
            if (GuesserCanPickTargets)
            {
                TargetPlayer = null;
                Rounds = 0;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
            }
            else
                Utils.RpcMurderPlayer(Player);
        }
    }

    public override void UpdatePlayer()
    {
        if (Failed && !Dead && GuessToAct)
            TurnAct();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (TargetPlayer.HasDied() || Dead)
            return;

        GuessMenu.GenButtons(__instance, RemainingGuesses > 0);

        var targetRole = TargetPlayer.GetRole();
        var something = "";
        var newRoleName = targetRole.Name;
        var rolechanged = false;

        if (RoleName != newRoleName && !IsNullEmptyOrWhiteSpace(RoleName))
        {
            rolechanged = true;
            RoleName = newRoleName;
        }
        else if (RoleName?.Length == 0)
            RoleName = newRoleName;

        if (rolechanged)
        {
            something = "Your target's role changed!";
            LettersGiven = 0;
            LettersExhausted = false;
            Letters.Clear();
            FactionHintGiven = false;
        }
        else if (!LettersExhausted)
        {
            var random = URandom.RandomRangeInt(0, RoleName.Length);
            var random2 = URandom.RandomRangeInt(0, RoleName.Length);
            var random3 = URandom.RandomRangeInt(0, RoleName.Length);

            if (LettersGiven <= RoleName.Length - 3)
            {
                while (random == random2 || random2 == random3 || random == random3 || Letters.Contains($"{RoleName[random]}") || Letters.Contains($"{RoleName[random2]}") ||
                    Letters.Contains($"{RoleName[random3]}"))
                {
                    if (random == random2 || Letters.Contains($"{RoleName[random2]}"))
                        random2 = URandom.RandomRangeInt(0, RoleName.Length);

                    if (random2 == random3 || Letters.Contains($"{RoleName[random3]}"))
                        random3 = URandom.RandomRangeInt(0, RoleName.Length);

                    if (random == random3 || Letters.Contains($"{RoleName[random]}"))
                        random = URandom.RandomRangeInt(0, RoleName.Length);
                }

                something = $"Your target's role as the Letters {RoleName[random]}, {RoleName[random2]} and {RoleName[random3]} in it!";
            }
            else if (LettersGiven == RoleName.Length - 2)
            {
                while (random == random2 || Letters.Contains($"{RoleName[random]}") || Letters.Contains($"{RoleName[random2]}"))
                {
                    if (Letters.Contains($"{RoleName[random2]}"))
                        random2 = URandom.RandomRangeInt(0, RoleName.Length);

                    if (Letters.Contains($"{RoleName[random]}"))
                        random = URandom.RandomRangeInt(0, RoleName.Length);

                    if (random == random2)
                        random = URandom.RandomRangeInt(0, RoleName.Length);
                }

                something = $"Your target's role as the Letters {RoleName[random]} and {RoleName[random2]} in it!";
            }
            else if (LettersGiven == RoleName.Length - 1)
            {
                while (Letters.Contains($"{RoleName[random]}"))
                    random = URandom.RandomRangeInt(0, RoleName.Length);

                something = $"Your target's role as the letter {RoleName[random]} in it!";
            }
            else if (LettersGiven == RoleName.Length)
                LettersExhausted = true;

            if (!LettersExhausted)
            {
                if (LettersGiven <= RoleName.Length - 3)
                {
                    Letters.Add($"{RoleName[random]}");
                    Letters.Add($"{RoleName[random2]}");
                    Letters.Add($"{RoleName[random3]}");
                    LettersGiven += 3;
                }
                else if (LettersGiven == RoleName.Length - 2)
                {
                    Letters.Add($"{RoleName[random]}");
                    Letters.Add($"{RoleName[random2]}");
                    LettersGiven += 2;
                }
                else if (LettersGiven == RoleName.Length - 1)
                {
                    Letters.Add($"{RoleName[random]}");
                    LettersGiven++;
                }
            }
        }
        else if (!FactionHintGiven && LettersExhausted)
        {
            something = $"Your target belongs to the {targetRole.FactionName}!";
            FactionHintGiven = true;
        }
        else if (!AlignmentHintGiven && LettersExhausted)
        {
            something = $"Your target's role belongs to {targetRole.Alignment.AlignmentName()} alignment!";
            AlignmentHintGiven = true;
        }

        // Ensures only the Guesser sees this
        if (!IsNullEmptyOrWhiteSpace(something))
            Run("<#EEE5BEFF>〖 Guess Hint 〗</color>", something);
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (player != TargetPlayer && !TargetGuessed) || player.AmOwner || RemainingGuesses <= 0 || Dead || Player.IsLinkedTo(player) || (TargetGuessed
            && NeutralSettings.AvoidNeutralKingmakers);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update(__instance);

    public void RpcMurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Player != player)
        {
            TargetGuessed = true;

            if (CanAttack(AttackVal, player.GetDefenseValue(Player)))
            {
                MarkMeetingDead(player, Player);

                if (Lovers.BothLoversDie && AmongUsClient.Instance.AmHost && player.TryGetLayer<Lovers>(out var lovers) && !lovers.OtherLover.Is(Alignment.NeutralApoc) &&
                    !lovers.OtherLover.Data.IsDead)
                {
                    RpcMurderPlayer(lovers.OtherLover, guess, guessTarget);
                }
            }

            if (player.AmOwner)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessString}!");
            else if (DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guessString}!");
            else if (CanAttack(AttackVal, player.GetDefenseValue(Player)))
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
        }
        else if (Player == player)
        {
            if (!TargetGuessed)
            {
                RemainingGuesses--;

                if (DeadSeeEverything() && !Local)
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{PlayerName} incorrectly guessed {guessTarget.name} as {guessString}!");
                else if (Local && !TargetGuessed)
                    Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString}!");
            }
            else if (DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{PlayerName} incorrectly guessed {guessTarget.name} as {guessString}!");
            else if (Local && !TargetGuessed)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString}!");
        }
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        GuessMenu.HideButtons();
        GuessingMenu.Close();
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        GuessMenu.Voted();
        GuessingMenu.Close();
    }

    public override void ReadRPC(MessageReader reader) => MurderPlayer(reader.ReadPlayer(), reader.ReadEnum<LayerEnum>(), reader.ReadPlayer());
}