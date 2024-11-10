namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Thief : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number StealCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ThiefSteals { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ThiefCanGuess { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ThiefCanGuessAfterVoting { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ThiefVent { get; set; } = false;

    public CustomButton StealButton { get; set; }
    public CustomMeeting GuessMenu { get; set; }
    public CustomRolesMenu GuessingMenu { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Thief : CustomColorManager.Neutral;
    public override string Name => "Thief";
    public override LayerEnum Type => LayerEnum.Thief;
    public override Func<string> StartText => () => "Steal From The Killers";
    public override Func<string> Description => () => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralBen;
        StealButton ??= CreateButton(this, new SpriteName("Steal"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Steal, new Cooldown(StealCd), "STEAL",
            (PlayerBodyExclusion)Exception);
        GuessMenu = new(Player, "Guess", ThiefCanGuessAfterVoting, Guess, IsExempt, SetLists);
        GuessingMenu = new(Player, GuessPlayer);
    }

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            var nks = new List<LayerEnum>() { LayerEnum.VampireHunter, LayerEnum.Bastion, LayerEnum.Veteran, LayerEnum.Vigilante };

            foreach (var layer in nks)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Vigilante && !GuessingMenu.Mapping.Contains(LayerEnum.Vigilante) && GuessingMenu.Mapping.Contains(LayerEnum.VampireHunter)))
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

                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Mafioso && !GuessingMenu.Mapping.Contains(LayerEnum.Mafioso) && GuessingMenu.Mapping.Contains(LayerEnum.Godfather)))
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

                if (RoleGen.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sidekick && !GuessingMenu.Mapping.Contains(LayerEnum.Sidekick) && GuessingMenu.Mapping.Contains(LayerEnum.Rebel)))
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0)
        {
            var nks = new List<LayerEnum>() { LayerEnum.Arsonist, LayerEnum.Glitch, LayerEnum.SerialKiller, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Cryomaniac,
                LayerEnum.Werewolf, LayerEnum.BountyHunter, LayerEnum.Plaguebearer };

            foreach (var layer in nks)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive())
                    GuessingMenu.Mapping.Add(layer);
            }
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, LayerEnum guess)
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
            var layerflag = player.GetLayers().Any(x => x.Type == guess);
            var subfactionflag = player.GetSubFaction().ToString() == guess.ToString();

            var flag = layerflag || subfactionflag;
            var toDie = flag ? player : Player;
            RpcMurderPlayer(toDie, guess, player);
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || Dead || (player == Player && player.AmOwner) ||
            (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GuessMenu.GenButtons(__instance, ThiefCanGuess);
    }

    public override void ReadRPC(MessageReader reader)
    {
        var thiefAction = reader.ReadEnum<ThiefActionsRPC>();

        switch (thiefAction)
        {
            case ThiefActionsRPC.Steal:
                Steal(reader.ReadPlayer());
                break;

            case ThiefActionsRPC.Guess:
                MurderPlayer(reader.ReadPlayer(), reader.ReadEnum<LayerEnum>(), reader.ReadPlayer());
                break;

            default:
                Error($"Received unknown RPC - {(int)thiefAction}");
                break;
        }
    }

    public void Steal()
    {
        var cooldown = Interact(Player, StealButton.GetTarget<PlayerControl>(), true);

        if (cooldown != CooldownType.Fail)
        {
            if (StealButton.GetTarget<PlayerControl>().GetFaction() is Faction.Intruder or Faction.Syndicate || StealButton.GetTarget<PlayerControl>().GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros or Alignment.CrewKill || StealButton.GetTarget<PlayerControl>().GetRole() is VampireHunter)
            {
                Utils.RpcMurderPlayer(Player, StealButton.GetTarget<PlayerControl>());
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ThiefActionsRPC.Steal, StealButton.GetTarget<PlayerControl>());
                Steal(StealButton.GetTarget<PlayerControl>());
            }
            else
                Utils.RpcMurderPlayer(Player);
        }

        StealButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public void Steal(PlayerControl other)
    {
        var role = other.GetRole();
        var player = Player;

        if (other.AmOwner || player.AmOwner)
        {
            Flash(Color);
            role.Deinit();
            Deinit();
        }

        Role newRole = role.Type switch
        {
            // Crew roles
            LayerEnum.Bastion => new Bastion(),
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),

            // Neutral roles
            LayerEnum.Arsonist => new Arsonist(),
            LayerEnum.Betrayer => new Betrayer() { Faction = role.Faction },
            LayerEnum.Cannibal => new Cannibal(),
            LayerEnum.Cryomaniac => new Cryomaniac(),
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.Juggernaut => new Juggernaut(),
            LayerEnum.Murderer => new Murderer(),
            LayerEnum.Plaguebearer or LayerEnum.Pestilence => new Plaguebearer(),
            LayerEnum.SerialKiller => new SerialKiller(),
            LayerEnum.Thief => new Thief(),
            LayerEnum.Werewolf => new Werewolf(),

            // Intruder roles
            LayerEnum.Ambusher => new Ambusher(),
            LayerEnum.Blackmailer => new Blackmailer(),
            LayerEnum.Camouflager => new Camouflager(),
            LayerEnum.Consigliere => new Consigliere(),
            LayerEnum.Consort => new Consort(),
            LayerEnum.Disguiser => new Disguiser(),
            LayerEnum.Enforcer => new Enforcer(),
            LayerEnum.Godfather => new Godfather(),
            LayerEnum.PromotedGodfather => new PromotedGodfather(),
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Janitor => new Janitor(),
            LayerEnum.Mafioso => new Mafioso() { Godfather = ((Mafioso)role).Godfather },
            LayerEnum.Miner => new Miner(),
            LayerEnum.Morphling => new Morphling(),
            LayerEnum.Teleporter => new Teleporter(),
            LayerEnum.Wraith => new Wraith(),

            // Syndicate roles
            LayerEnum.Anarchist => new Anarchist(),
            LayerEnum.Bomber => new Bomber(),
            LayerEnum.Collider => new Collider(),
            LayerEnum.Concealer => new Concealer(),
            LayerEnum.Crusader => new Crusader(),
            LayerEnum.Drunkard => new Drunkard(),
            LayerEnum.Framer => new Framer(),
            LayerEnum.Poisoner => new Poisoner(),
            LayerEnum.Rebel => new Rebel(),
            LayerEnum.PromotedRebel => new PromotedRebel(),
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Sidekick => new Sidekick() { Rebel = ((Sidekick)role).Rebel },
            LayerEnum.Silencer => new Silencer(),
            LayerEnum.Spellslinger => new Spellslinger(),
            LayerEnum.Stalker => new Stalker(),
            LayerEnum.Timekeeper => new Timekeeper(),
            LayerEnum.Warper => new Warper(),

            // Whatever else
            LayerEnum.Thief or _ => new Thief()
        };

        newRole.RoleUpdate(this, player, Faction == Faction.Neutral);

        if (other.Is(LayerEnum.Dracula))
            ((Dracula)role).Converted.Clear();
        else if (other.Is(LayerEnum.Whisperer))
            ((Whisperer)role).Persuaded.Clear();
        else if (other.Is(LayerEnum.Necromancer))
            ((Necromancer)role).Resurrected.Clear();
        else if (other.Is(LayerEnum.Jackal))
        {
            ((Jackal)role).Recruited.Clear();
            ((Jackal)role).Recruit2 = null;
            ((Jackal)role).Recruit1 = null;
            ((Jackal)role).Recruit3 = null;
        }

        if (ThiefSteals)
        {
            if (other.AmOwner && other.Is(Faction.Intruder))
                other.Data.Role.TeamType = RoleTeamTypes.Crewmate;

            new Thief().RoleUpdate(role, other, true);
        }

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && Snitch.SnitchSeesNeutrals))
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && player.AmOwner)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(player.PlayerId, new(snitch.Player, CustomColorManager.Snitch));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && player.AmOwner)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(player, CustomColorManager.Revealer));
            }
        }
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update(__instance);

    public void RpcMurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ThiefActionsRPC.Guess, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Local && player == Player)
            GuessMenu.HideButtons();

        if (player != Player && player.Is(LayerEnum.Indomitable))
        {
            if (player.AmOwner)
                Flash(CustomColorManager.Indomitable);

            return;
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            MarkMeetingDead(player, Player);

            if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
            {
                var otherLover = player.GetOtherLover();

                if (!otherLover.Is(LayerEnum.Pestilence))
                    RpcMurderPlayer(otherLover, guess, guessTarget);
            }
        }

        if (Local)
        {
            if (Player != player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guessString}!");
            else
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and died!");
        }
        else if (Player != player && player.AmOwner)
            Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessTarget}!");
        else if (DeadSeeEverything())
        {
            if (Player != player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {player.name} as {guessTarget} and stole their role!");
            else
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {player.name} as {guessTarget} and died!");
        }
        else
            Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");

        if (Player != player && CanAttack(AttackVal, player.GetDefenseValue(Player)))
            Steal(player);
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
}