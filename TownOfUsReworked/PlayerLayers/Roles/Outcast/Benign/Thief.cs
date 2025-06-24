namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Thief)]
public sealed class Thief : Benign, IGuesser
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number StealCd = 25;

    [ToggleOption]
    private static bool ThiefSteals = false;

    [ToggleOption]
    private static bool ThiefCanGuess = false;

    [ToggleOption]
    private static bool ThiefVent = false;

    private CustomButton StealButton;
    public CustomMeeting GuessMenu { get; private set; }
    public CustomGuessingMenu GuessingMenu { get; private set; }

    protected override UColor MainColor => CustomColorManager.Thief;
    public override Layer Type => Layer.Thief;
    public override string StartText => "Steal From The Killers";
    public override string Description => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill";
    public override Attack Attack => Attack.Powerful;
    public override bool CanVent => base.CanVent && ThiefVent;

    public override void Init()
    {
        StealButton ??= new(this, new SpriteName("Steal"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Steal, new Cooldown(StealCd), "STEAL", (PlayerBodyExclusion)Exception);
        GuessingMenu = new(Player, GuessPlayer);

        if (ThiefCanGuess)
            GuessMenu = new(Player, "Guess", Guess, IsExempt, SetLists);
    }

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
            GuessingMenu.Mapping.AddRange(new[] { Layer.Bastion, Layer.Veteran, Layer.Vigilante }.Where(layer => RoleGenManager.GetSpawnItem(layer).IsActive()));

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
            GuessingMenu.Mapping.AddRange(new[] { Layer.Arsonist, Layer.Glitch, Layer.SerialKiller, Layer.Juggernaut, Layer.Murderer, Layer.Cryomaniac, Layer.Jackal,
                Layer.Werewolf, Layer.BountyHunter, Layer.Dracula, Layer.Whisperer, Layer.Zealot, Layer.Necromancer }.Where(layer =>
                RoleGenManager.GetSpawnItem(layer).IsActive()));
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, Layer guess)
    {
        if (Dead || Meeting().state == MeetingHud.VoteStates.Discussion || !panel)
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
            RpcMurderPlayer(toDie, guess, player);
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || Dead || (player == Player && player.AmOwner) ||
            player.IsBuddyWith(Player, Handler.CurrentFaction);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public override void OnMeetingStart(MeetingHud __instance) => GuessMenu.GenButtons(__instance, ThiefCanGuess);

    public override void ReadRPC(RpcReader reader)
    {
        var thiefAction = reader.Read<ThiefActionsRpc>();

        switch (thiefAction)
        {
            case ThiefActionsRpc.Steal:
            {
                Steal(reader.ReadPlayer());
                break;
            }
            case ThiefActionsRpc.Guess:
            {
                MurderPlayer(reader.ReadPlayer(), reader.Read<Layer>(), reader.ReadPlayer());
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {thiefAction}");
                break;
            }
        }
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private void Steal(PlayerControl target)
    {
        var allowed = true;

        if (Local && !Meeting())
        {
            var cooldown = Interact(Player, target, true, delayed: true);

            if (cooldown != CooldownType.Fail)
            {
                if (target.GetRole() is OKilling or IPromoter or Neophyte or Harbinger or Betrayer or CKilling)
                {
                    Player.RpcMurderPlayer(target);
                    PerformRpcAction(ThiefActionsRpc.Steal, target);
                }
                else
                {
                    Player.RpcSuicide();
                    allowed = false;
                }
            }

            StealButton.StartCooldown(cooldown);
        }

        if (!allowed)
            return;

        var role = target.GetRole();
        var player = Player;

        Role newRole = role switch
        {
            // Crew roles
            Bastion => new Bastion(),
            Veteran => new Veteran(),
            Vigilante => new Vigilante(),

            // Outcast roles
            Arsonist => new Arsonist(),
            Betrayer => new Betrayer(),
            Cryomaniac => new Cryomaniac(),
            Glitch => new Glitch(),
            Juggernaut => new Juggernaut(),
            Murderer => new Murderer(),
            SerialKiller => new SerialKiller(),
            Thief => new Thief(),
            Werewolf => new Werewolf(),

            // Intruder roles
            Ambusher => new Ambusher(),
            Blackmailer => new Blackmailer(),
            Camouflager => new Camouflager(),
            Consigliere => new Consigliere(),
            Consort => new Consort(),
            Disguiser => new Disguiser(),
            Enforcer => new Enforcer(),
            Godfather => new Godfather(),
            Grenadier => new Grenadier(),
            Impostor => new Impostor(),
            Janitor => new Janitor(),
            Miner => new Miner(),
            Morphling => new Morphling(),
            Teleporter => new Teleporter(),
            Wraith => new Wraith(),

            // Syndicate roles
            Anarchist => new Anarchist(),
            Bomber => new Bomber(),
            Collider => new Collider(),
            Concealer => new Concealer(),
            Crusader => new Crusader(),
            Drunkard => new Drunkard(),
            Framer => new Framer(),
            Poisoner => new Poisoner(),
            Rebel => new Rebel(),
            Shapeshifter => new Shapeshifter(),
            Silencer => new Silencer(),
            Spellslinger => new Spellslinger(),
            Stalker => new Stalker(),
            Timekeeper => new Timekeeper(),
            Warper => new Warper(),

            // Apocalypse roles
            Cultist => new Cultist(),
            Plaguebearer => new Plaguebearer(),
            Cannibal => new Cannibal(),

            // Whatever else
            Thief or _ => new Thief()
        };

        newRole.RoleUpdate(this, player, Handler.CurrentFaction == Faction.Outcast);

        if (ThiefSteals)
            new Thief().RoleUpdate(role, target);

        var local = LayerHandler.Handlers[LocalPlayer.PlayerId];
        var faction = local.CurrentFaction;
        var neut = faction.IsOutcast();

        if (faction.IsFactionedEvil() || (faction.IsOutcast() && (Snitch.SnitchSeesOutcasts || Revealer.RevealerRevealsOutcasts)))
        {
            if (!neut || Snitch.SnitchSeesOutcasts)
            {
                foreach (var snitch in GetLayers<Snitch>())
                {
                    if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && player.AmOwner)
                        local.AllArrows.Add(snitch.PlayerId, new(player, snitch.Player, snitch.Color));
                    else if (snitch.TasksDone && snitch.Local)
                        LayerHandler.Handlers[snitch.PlayerId].AllArrows.Add(player.PlayerId, new(snitch.Player, player, snitch.Color));
                }
            }

            if (!neut || Revealer.RevealerRevealsOutcasts)
            {
                foreach (var revealer in GetLayers<Revealer>())
                {
                    if (revealer.Revealed && player.AmOwner)
                        local.AllArrows.Add(revealer.PlayerId, new(player, revealer.Player, revealer.Color));
                }
            }
        }

        if (target.AmOwner)
            Flash(Color);
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update();

    private void RpcMurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        PerformRpcAction(ThiefActionsRpc.Guess, player, guess, guessTarget);
    }

    private void MurderPlayer(PlayerControl player, Layer guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Local && player == Player)
            GuessMenu.HideButtons();

        if (player != Player && player.Is<Indomitable>())
        {
            if (player.AmOwner)
                Flash(CustomColorManager.Indomitable);

            return;
        }

        if (CanAttack(Attack, player.GetDefenseValue(Player)) || player == Player)
        {
            MarkMeetingDead(player, Player);

            if (Lovers.BothLoversDie && AmongUsClient.Instance.AmHost && player.Is<Lovers>(out var lovers) && !lovers.Other.Is(Alignment.Deity) &&
                !lovers.Other.Data.IsDead)
            {
                RpcMurderPlayer(lovers.Other, guess, guessTarget);
            }
        }

        if (Local)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", Player != player ? $"You guessed {guessTarget.name} as {guessString}!" : $"You incorrectly guessed {guessTarget.name} as {guessString} and died!");
        else if (Player != player && player.AmOwner)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessTarget}!");
        else if (DeadSeeEverything())
            Run("<#EC1C45FF>∮ Assassination ∮</color>", Player != player ? $"{Player.name} guessed {player.name} as {guessTarget} and stole their role!" : $"{Player.name} incorrectly guessed {player.name} as {guessTarget} and died!");
        else
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");

        if (Player != player && CanAttack(Attack, player.GetDefenseValue(Player)))
            Steal(player);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        GuessMenu.HideButtons();
        GuessingMenu.Close();
    }
}