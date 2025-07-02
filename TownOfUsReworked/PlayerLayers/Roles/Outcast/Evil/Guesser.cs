namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Guesser)]
public sealed class Guesser : Evil, IGuesser, ITargeter
{
    [ToggleOption]
    public static bool GuesserCanPickTargets = false;

    [ToggleOption]
    public static bool GuesserButton = true;

    [ToggleOption]
    private static bool GuessVent = false;

    [ToggleOption]
    private static bool GuessSwitchVent = false;

    [ToggleOption]
    public static bool GuessTargetKnows = false;

    [ToggleOption]
    private static bool MultipleGuesses = true;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number MaxGuesses = 5;

    [ToggleOption]
    private static bool GuessToAct = true;

    public PlayerControl TargetPlayer { get; set; }
    private bool Failed => TargetPlayer ? (!TargetGuessed && (RemainingGuesses <= 0 || TargetPlayer.HasDied())) : Rounds > 2;
    public CustomMeeting GuessMenu { get; private set; }
    public CustomGuessingMenu GuessingMenu { get; private set; }

    private bool TargetGuessed;
    private int RemainingGuesses;
    private bool FactionHintGiven;
    private bool AlignmentHintGiven;
    private bool LettersExhausted;
    private string RoleName;
    private readonly HashSet<char> Letters = [];
    private int Rounds;
    private CustomButton TargetButton;

    protected override UColor MainColor => CustomColorManager.Guesser;
    public override Layer Type => Layer.Guesser;
    public override string StartText => "Guess What Someone Might Be";
    public override string Description => !TargetPlayer ? "- You can select a player to guess their role" : ((TargetGuessed ? "- You can guess player's roles without penalties" :
        $"- You can only try to guess {TargetPlayer?.name}") + $"\n- If {TargetPlayer?.name} dies without getting guessed by you, you will become an <#00ACC2FF>Actor</color>");
    public override Attack Attack => Attack.Unstoppable;
    public override bool HasWon => TargetGuessed;
    public override bool CanVent => base.CanVent && GuessVent;
    public override bool CanSwitchVents => GuessSwitchVent;
    protected override WinLose EndState => WinLose.GuesserWins;

    public override void Init()
    {
        RemainingGuesses = MaxGuesses == 0 ? 10000 : MaxGuesses;
        Objectives = () => TargetGuessed ? $"- You have found out what {TargetPlayer.name} was" : (!TargetPlayer ? "- Find someone to be guessed by you" : ("- Guess " +
            $"{TargetPlayer?.name}'s role"));
        GuessMenu = new(Player, "Guess", Guess, IsExempt, SetLists);
        Rounds = 0;
        Letters.Clear();
        GuessingMenu = new(Player, GuessPlayer);

        if (GuesserCanPickTargets || !TargetPlayer)
        {
            TargetButton ??= new(this, new SpriteName("GuessTarget"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SelectTarget, (PlayerBodyExclusion)Exception, "AGONISE",
                (UsableFunc)Usable);
        }
    }

    public override void Reset(bool meeting, bool start)
    {
        if (meeting && !TargetPlayer)
            Rounds++;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (player == TargetPlayer)
            name += " <#EEE5BEFF>π</color>";
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.Add(TargetPlayer);
        return team;
    }

    private bool Exception(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.Investigative);

    private void SelectTarget(PlayerControl target)
    {
        TargetPlayer = target;
        CallRpc(MiscRpc.SetTarget, this, TargetPlayer);
    }

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        GuessingMenu.Mapping.Add(Layer.Crewmate);

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            foreach (var layer in GetValuesFromTo(Layer.Altruist, Layer.Vigilante, x => x is not (Layer.Revealer or Layer.Crewmate) && !RoleGenManager.CI.Contains(x)))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive())
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (IntruderSettings.IntruderCount > 0)
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

        if (SyndicateSettings.SyndicateCount > 0)
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

        if (SyndicateSettings.SyndicateCount > 0)
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

        if (ApocalypseSettings.ApocalypseCount > 0)
        {
            GuessingMenu.Mapping.Add(Layer.Cultist);

            if (ApocalypseSettings.ApocalypseMax > 0 && ApocalypseSettings.ApocalypseMin > 0)
            {
                GuessingMenu.Mapping.AddRange(GetValuesFromTo(Layer.Cannibal, Layer.Void, x => x != Layer.Cultist && !RoleGenManager.AD.Contains(x))
                    .Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));
            }
        }

        if (OutcastSettings.OutcastMax > 0 && OutcastSettings.OutcastMin > 0)
        {
            foreach (var layer in GetValuesFromTo(Layer.Actor, Layer.Whisperer, x => x is not Layer.Phantom))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == Layer.Survivor && GuessingMenu.Mapping.Contains(Layer.GuardianAngel)) || (layer == Layer.Thief &&
                    GuessingMenu.Mapping.Contains(Layer.Amnesiac)) || (layer == Layer.Troll && GuessingMenu.Mapping.Contains(Layer.BountyHunter)) || (layer == Layer.Actor &&
                    GuessingMenu.Mapping.Contains(Layer.Guesser)) || (layer == Layer.Jester && GuessingMenu.Mapping.Contains(Layer.Executioner)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, Layer guess)
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
            var layerFlag = player.GetLayers().Any(x => x.Type == guess);
            var promoterFlag = player.Is<IPromoter>(out var promoter) && ((promoter.UnderlingType == guess && promoter.IsUnderling) || (promoter.PromoterType == guess && promoter.IsPromoted));

            var flag = layerFlag || promoterFlag;
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

    private void TurnAct()
    {
        Role role = IsList() ? new Jester() : new Actor();
        role.RoleUpdate(this);
    }

    private bool Usable() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        if (!Failed || Dead || GuessToAct)
            return;

        if (GuesserCanPickTargets)
        {
            TargetPlayer = null;
            Rounds = 0;
            CallRpc(MiscRpc.SetTarget, this, 255);
        }
        else
            Player.RpcSuicide();
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (Failed && !Dead && GuessToAct)
            TurnAct();
    }

    public override void LocalOnMeetingStart(MeetingHud __instance)
    {
        if (TargetPlayer.HasDied() || Dead)
            return;

        GuessMenu.GenButtons(__instance, RemainingGuesses > 0);

        var targetRole = TargetPlayer.GetRole();
        var something = "";
        var newRoleName = targetRole.Name;
        var roleChanged = RoleName != newRoleName && !IsNullEmptyOrWhiteSpace(RoleName);
        RoleName = newRoleName;

        if (roleChanged)
        {
            something = "Your target's role changed!";
            LettersExhausted = false;
            Letters.Clear();
            FactionHintGiven = false;
        }
        else if (!LettersExhausted)
        {
            var random = RoleName.Random(x => !Letters.Contains(x));

            if (random != '\0')
                Letters.Add(random);

            var random2 = RoleName.Random(x => !Letters.Contains(x));

            if (random2 != '\0')
                Letters.Add(random2);

            if (Letters.Count == RoleName.Length)
                LettersExhausted = true;
        }
        else if (!FactionHintGiven && LettersExhausted)
        {
            something = $"Your target belongs to the {targetRole.FactionName}!";
            FactionHintGiven = true;
        }
        else if (!AlignmentHintGiven && LettersExhausted)
        {
            something = $"Your target's role belongs to the {targetRole.Alignment} alignment!";
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
            && OutcastSettings.AvoidOutcastKingmakers);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update();

    private void RpcMurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        PerformRpcAction(player, guess, guessTarget);
    }

    private void MurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Player != player)
        {
            TargetGuessed = true;

            if (CanAttack(Attack, player.GetDefenseValue(Player)))
            {
                MarkMeetingDead(player, Player);

                if (Lovers.BothLoversDie && AmongUsClient.Instance.AmHost && player.Is<Lovers>(out var lovers) && !lovers.Other.Is(Alignment.Deity) &&
                    !lovers.Other.Data.IsDead)
                {
                    RpcMurderPlayer(lovers.Other, guess, guessTarget);
                }
            }

            if (player.AmOwner)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessString}!");
            else if (DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guessString}!");
            else if (CanAttack(Attack, player.GetDefenseValue(Player)))
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

    public override void ReadRPC(RpcReader reader) => MurderPlayer(reader.ReadPlayer(), reader.Read<Layer>(), reader.ReadPlayer());
}