namespace TownOfUsReworked.PlayerLayers.Roles;

public class Thief : Neutral
{
    public CustomButton StealButton { get; set; }
    public Dictionary<string, UColor> ColorMapping { get; set; }
    public Dictionary<string, UColor> SortedColorMapping { get; set; }
    public GameObject Phone { get; set; }
    public Transform SelectedButton { get; set; }
    public int Page { get; set; }
    public int MaxPage { get; set; }
    public Dictionary<int, List<Transform>> GuessButtons { get; set; }
    public Dictionary<int, KeyValuePair<string, UColor>> Sorted { get; set; }
    public CustomMeeting GuessMenu { get; set; }
    private Transform Next;
    private Transform Back;

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Thief : CustomColorManager.Neutral;
    public override string Name => "Thief";
    public override LayerEnum Type => LayerEnum.Thief;
    public override Func<string> StartText => () => "Steal From The Killers";
    public override Func<string> Description => () => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public Thief() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.NeutralBen;
        StealButton = new(this, "Steal", AbilityTypes.Alive, "ActionSecondary", Steal, CustomGameOptions.StealCd, Exception);
        ColorMapping = new();
        SortedColorMapping = new();
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        GuessButtons = new();
        Sorted = new();
        GuessMenu = new(Player, "Guess", CustomGameOptions.ThiefCanGuessAfterVoting, Guess, IsExempt, SetLists);
        SetLists();
        return this;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StealButton.Update2("STEAL");
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        //Adds all the roles that have a non-zero chance of being in the game
        if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
        {
            if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", CustomColorManager.Veteran);
            if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", CustomColorManager.Vigilante);
            if (CustomGameOptions.BastionOn > 0) ColorMapping.Add("Bastion", CustomColorManager.Bastion);
            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Vampire Hunter", CustomColorManager.VampireHunter);
        }

        if (!CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
        {
            ColorMapping.Add("Impostor", CustomColorManager.Intruder);

            if (CustomGameOptions.IntruderMax > 0 && CustomGameOptions.IntruderMin > 0)
            {
                if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", CustomColorManager.Janitor);
                if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", CustomColorManager.Morphling);
                if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", CustomColorManager.Miner);
                if (CustomGameOptions.WraithOn > 0) ColorMapping.Add("Wraith", CustomColorManager.Wraith);
                if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", CustomColorManager.Grenadier);
                if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", CustomColorManager.Blackmailer);
                if (CustomGameOptions.CamouflagerOn > 0) ColorMapping.Add("Camouflager", CustomColorManager.Camouflager);
                if (CustomGameOptions.DisguiserOn > 0) ColorMapping.Add("Disguiser", CustomColorManager.Disguiser);
                if (CustomGameOptions.DisguiserOn > 0) ColorMapping.Add("Consigliere", CustomColorManager.Consigliere);
                if (CustomGameOptions.ConsortOn > 0) ColorMapping.Add("Consort", CustomColorManager.Consort);

                if (CustomGameOptions.GodfatherOn > 0)
                {
                    ColorMapping.Add("Godfather", CustomColorManager.Godfather);
                    ColorMapping.Add("Mafioso", CustomColorManager.Mafioso);
                }
            }
        }

        if (CustomGameOptions.SyndicateCount > 0)
        {
            ColorMapping.Add("Anarchist", CustomColorManager.Syndicate);

            if (CustomGameOptions.SyndicateMax > 0 && CustomGameOptions.SyndicateMin > 0)
            {
                if (CustomGameOptions.WarperOn > 0) ColorMapping.Add("Warper", CustomColorManager.Warper);
                if (CustomGameOptions.ConcealerOn > 0) ColorMapping.Add("Concealer", CustomColorManager.Concealer);
                if (CustomGameOptions.ShapeshifterOn > 0) ColorMapping.Add("Shapeshifter", CustomColorManager.Shapeshifter);
                if (CustomGameOptions.FramerOn > 0) ColorMapping.Add("Framer", CustomColorManager.Framer);
                if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", CustomColorManager.Bomber);
                if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", CustomColorManager.Poisoner);
                if (CustomGameOptions.CrusaderOn > 0) ColorMapping.Add("Crusader", CustomColorManager.Crusader);
                if (CustomGameOptions.StalkerOn > 0) ColorMapping.Add("Stalker", CustomColorManager.Stalker);
                if (CustomGameOptions.ColliderOn > 0) ColorMapping.Add("Collider", CustomColorManager.Collider);
                if (CustomGameOptions.SpellslingerOn > 0) ColorMapping.Add("Spellslinger", CustomColorManager.Spellslinger);
                if (CustomGameOptions.TimekeeperOn > 0) ColorMapping.Add("Timekeeper", CustomColorManager.Timekeeper);
                if (CustomGameOptions.SilencerOn > 0) ColorMapping.Add("Silencer", CustomColorManager.Silencer);

                if (CustomGameOptions.RebelOn > 0)
                {
                    ColorMapping.Add("Rebel", CustomColorManager.Rebel);
                    ColorMapping.Add("Sidekick", CustomColorManager.Sidekick);
                }
            }
        }

        if (CustomGameOptions.NeutralMax > 0 && CustomGameOptions.NeutralMin > 0)
        {
            if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", CustomColorManager.Arsonist);
            if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", CustomColorManager.Glitch);
            if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", CustomColorManager.SerialKiller);
            if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", CustomColorManager.Juggernaut);
            if (CustomGameOptions.MurdererOn > 0) ColorMapping.Add("Murderer", CustomColorManager.Murderer);
            if (CustomGameOptions.CryomaniacOn > 0) ColorMapping.Add("Cryomaniac", CustomColorManager.Cryomaniac);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", CustomColorManager.Werewolf);
            if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", CustomColorManager.Plaguebearer);
            if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", CustomColorManager.BountyHunter);
        }

        //Sorts the list alphabetically.
        SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        var i = 0;
        var j = 0;
        var k = 0;

        foreach (var pair in SortedColorMapping)
        {
            Sorted.Add(j, pair);
            j++;
            k++;

            if (k >= 40)
            {
                i++;
                k -= 40;
            }
        }

        MaxPage = i;
    }

    private void SetButtons(MeetingHud __instance, PlayerVoteArea voteArea)
    {
        var buttonTemplate = voteArea.transform.FindChild("votePlayerBase");
        SelectedButton = null;
        var i = 0;
        var j = 0;

        for (var k = 0; k < SortedColorMapping.Count; k++)
        {
            if (!GuessButtons.ContainsKey(i))
                GuessButtons.Add(i, new());

            var row = j / 5;
            var col = j % 5;
            var guess = Sorted[k].Key;
            var buttonParent = new GameObject($"Guess{i}").transform;
            buttonParent.SetParent(Phone.transform);
            var button = UObject.Instantiate(buttonTemplate, buttonParent);
            MakeTheButton(button, buttonParent, voteArea, new(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -5f), guess, Sorted[k].Value, () =>
            {
                if (IsDead)
                    return;

                if (SelectedButton != button)
                {
                    if (SelectedButton != null)
                        SelectedButton.GetComponent<SpriteRenderer>().color = UColor.white;

                    SelectedButton = button;
                    SelectedButton.GetComponent<SpriteRenderer>().color = UColor.red;
                }
                else
                {
                    var focusedTarget = PlayerByVoteArea(voteArea);

                    if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null)
                        return;

                    var targetId = voteArea.TargetPlayerId;
                    var targetPlayer = PlayerById(targetId);

                    var playerRole = voteArea.GetRole();

                    var roleflag = playerRole?.Name == guess;
                    var recruitflag = targetPlayer.IsRecruit() && guess == "Recruit";
                    var sectflag = targetPlayer.IsPersuaded() && guess == "Persuaded";
                    var reanimatedflag = targetPlayer.IsResurrected() && guess == "Resurrected";
                    var undeadflag = targetPlayer.IsBitten() && guess == "Bitten";

                    var flag = roleflag || recruitflag || sectflag || reanimatedflag || undeadflag;
                    var toDie = flag ? targetPlayer : Player;
                    RpcMurderPlayer(toDie, guess, targetPlayer);
                    Exit(__instance);
                }
            });

            GuessButtons[i].Add(button);
            j++;

            if (j >= 40)
            {
                i++;
                j -= 40;
            }
        }

        if (MaxPage > 0)
        {
            var nextParent = new GameObject("GuessNext").transform;
            nextParent.SetParent(Phone.transform);
            Next = UObject.Instantiate(buttonTemplate, nextParent);
            MakeTheButton(Next, nextParent, voteArea, new(3.53f, -2.13f, -5f), "Next Page", UColor.white, () => Page = CycleInt(MaxPage, 0, Page, true));

            var backParent = new GameObject("GuessBack").transform;
            backParent.SetParent(Phone.transform);
            Back = UObject.Instantiate(buttonTemplate, backParent);
            MakeTheButton(Back, backParent, voteArea, new(-3.47f, -2.13f, -5f), "Back Page", UColor.white, () => Page = CycleInt(MaxPage, 0, Page, false));
        }
    }

    private void MakeTheButton(Transform button, Transform buttonParent, PlayerVoteArea voteArea, Vector3 position, string title, UColor color, Action onClick)
    {
        UObject.Instantiate(voteArea.transform.FindChild("MaskArea"), buttonParent);
        var label = UObject.Instantiate(voteArea.NameText, button);
        var rend = button.GetComponent<SpriteRenderer>();
        rend.sprite = Ship.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
        buttonParent.localPosition = position;
        buttonParent.localScale = new(0.55f, 0.55f, 1f);
        label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
        label.transform.localScale *= 1.7f;
        label.text = title;
        label.color = color;
        var passive = button.GetComponent<PassiveButton>();
        passive.OnMouseOver = new();
        passive.OnMouseOver.AddListener((Action)(() => rend.color = UColor.green));
        passive.OnMouseOut = new();
        passive.OnMouseOut.AddListener((Action)(() => rend.color = SelectedButton == button ? UColor.red : UColor.white));
        passive.OnClick = new();
        passive.OnClick.AddListener(onClick);
        passive.ClickSound = SoundEffects["Click"];
        passive.HoverSound = SoundEffects["Hover"];
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || IsDead || (player == Player && player == CustomPlayer.Local) ||
            (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (Phone || __instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        AllVoteAreas.ForEach(x => x.gameObject.SetActive(false));
        __instance.TimerText.gameObject.SetActive(false);
        Chat.SetVisible(false);
        Page = 0;
        var container = UObject.Instantiate(UObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI"), __instance.transform);
        container.transform.localPosition = new(0, 0, -5f);
        Phone = container.gameObject;
        var exitButtonParent = new GameObject("CustomExitButton").transform;
        exitButtonParent.SetParent(container);
        var exitButton = UObject.Instantiate(voteArea.transform.FindChild("votePlayerBase").transform, exitButtonParent);
        UObject.Instantiate(voteArea.transform.FindChild("MaskArea"), exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = voteArea.Buttons.transform.Find("CancelButton").GetComponent<SpriteRenderer>().sprite;
        exitButtonParent.transform.localPosition = new(2.725f, 2.1f, -5);
        exitButtonParent.transform.localScale = new(0.217f, 0.9f, 1);
        var button = exitButton.GetComponent<PassiveButton>();
        button.OnClick = new();
        button.OnClick.AddListener((Action)(() => Exit(__instance)));
        SetButtons(__instance, voteArea);
    }

    public void Exit(MeetingHud __instance)
    {
        if (!Phone)
            return;

        Phone.Destroy();
        Chat.SetVisible(true);
        SelectedButton = null;
        __instance.TimerText.gameObject.SetActive(true);
        AllVoteAreas.ForEach(x => x.gameObject.SetActive(true));

        foreach (var pair in GuessButtons)
        {
            foreach (var item in pair.Value)
            {
                if (!item)
                    continue;

                item.GetComponent<PassiveButton>().OnClick = new();
                item.GetComponent<PassiveButton>().OnMouseOut = new();
                item.GetComponent<PassiveButton>().OnMouseOver = new();
                item.gameObject.SetActive(false);
                item.gameObject.Destroy();
                item.Destroy();
            }
        }

        GuessButtons.Clear();

        if (MaxPage > 0)
        {
            Next.GetComponent<PassiveButton>().OnClick = new();
            Next.GetComponent<PassiveButton>().OnMouseOut = new();
            Next.GetComponent<PassiveButton>().OnMouseOver = new();
            Next.gameObject.SetActive(false);
            Next.gameObject.Destroy();
            Next.Destroy();

            Back.GetComponent<PassiveButton>().OnClick = new();
            Back.GetComponent<PassiveButton>().OnMouseOut = new();
            Back.GetComponent<PassiveButton>().OnMouseOver = new();
            Back.gameObject.SetActive(false);
            Back.gameObject.Destroy();
            Back.Destroy();
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GuessMenu.GenButtons(__instance, CustomGameOptions.ThiefCanGuess);
    }

    public override void ReadRPC(MessageReader reader)
    {
        var thiefAction = (ThiefActionsRPC)reader.ReadByte();

        switch (thiefAction)
        {
            case ThiefActionsRPC.Steal:
                Steal(reader.ReadPlayer());
                break;

            case ThiefActionsRPC.Guess:
                MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
                break;

            default:
                LogError($"Received unknown RPC - {thiefAction}");
                break;
        }
    }

    public void Steal()
    {
        var cooldown = Interact(Player, StealButton.TargetPlayer, true);

        if (cooldown != CooldownType.Fail)
        {
            if (!(StealButton.TargetPlayer.GetFaction() is Faction.Intruder or Faction.Syndicate || StealButton.TargetPlayer.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo
                or Alignment.NeutralPros or Alignment.CrewKill))
            {
                Utils.RpcMurderPlayer(Player);
            }
            else
            {
                Utils.RpcMurderPlayer(Player, StealButton.TargetPlayer);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, ThiefActionsRPC.Steal, StealButton.TargetPlayer);
                Steal(StealButton.TargetPlayer);
            }
        }

        StealButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public void Steal(PlayerControl other)
    {
        var role = other.GetRole();
        var target = other.GetTarget();
        var leader = other.GetLeader();
        var player = Player;

        if (CustomPlayer.Local == other || CustomPlayer.Local == player)
        {
            Flash(Color);
            role.OnLobby();
            OnLobby();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(),
            LayerEnum.Arsonist => new Arsonist() { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer() { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(),
            LayerEnum.Camouflager => new Camouflager(),
            LayerEnum.Concealer => new Concealer(),
            LayerEnum.Consigliere => new Consigliere(),
            LayerEnum.Consort => new Consort(),
            LayerEnum.Cryomaniac => new Cryomaniac() { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Disguiser => new Disguiser(),
            LayerEnum.Dracula => new Dracula() { Converted = ((Dracula)role).Converted },
            LayerEnum.Framer => new Framer() { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(),
            LayerEnum.Enforcer => new Enforcer(),
            LayerEnum.Godfather => new Godfather(),
            LayerEnum.Grenadier => new Grenadier(),
            LayerEnum.Impostor => new Impostor(),
            LayerEnum.Juggernaut => new Juggernaut(),
            LayerEnum.Mafioso => new Mafioso() { Godfather = (Godfather)leader },
            LayerEnum.PromotedGodfather => new PromotedGodfather() { BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer },
            LayerEnum.Miner => new Miner(),
            LayerEnum.Morphling => new Morphling(),
            LayerEnum.Rebel => new Rebel(),
            LayerEnum.Sidekick => new Sidekick() { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(),
            LayerEnum.Murderer => new Murderer(),
            LayerEnum.Plaguebearer => new Plaguebearer() { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(),
            LayerEnum.SerialKiller => new SerialKiller(),
            LayerEnum.Werewolf => new Werewolf(),
            LayerEnum.Janitor => new Janitor(),
            LayerEnum.Poisoner => new Poisoner(),
            LayerEnum.Teleporter => new Teleporter(),
            LayerEnum.VampireHunter => new VampireHunter(),
            LayerEnum.Veteran => new Veteran(),
            LayerEnum.Vigilante => new Vigilante(),
            LayerEnum.Warper => new Warper(),
            LayerEnum.Wraith => new Wraith(),
            LayerEnum.BountyHunter => new BountyHunter() { TargetPlayer = target },
            LayerEnum.Bastion => new Bastion() { BombedIDs = ((Bastion)role).BombedIDs },
            LayerEnum.Jackal => new Jackal()
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Necromancer => new Necromancer() { Resurrected = ((Necromancer)role).Resurrected },
            LayerEnum.Whisperer => new Whisperer() { Persuaded = ((Whisperer)role).Persuaded },
            LayerEnum.Betrayer => new Betrayer() { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(),
            LayerEnum.Crusader => new Crusader(),
            LayerEnum.PromotedRebel => new PromotedRebel()
            {
                Framed = ((PromotedRebel)role).Framed,
                SilencedPlayer = ((PromotedRebel)role).SilencedPlayer,
            },
            LayerEnum.Stalker => new Stalker(),
            LayerEnum.Silencer => new Silencer() { SilencedPlayer = ((Silencer)role).SilencedPlayer, },
            _ => new Thief(),
        };

        newRole.Start<Role>(player).RoleUpdate(this, Faction == Faction.Neutral);

        if (other.Is(LayerEnum.Dracula))
            ((Dracula)role).Converted.Clear();
        else if (other.Is(LayerEnum.Whisperer))
            ((Whisperer)role).Persuaded.Clear();
        else if (other.Is(LayerEnum.Necromancer))
            ((Necromancer)role).Resurrected.Clear();
        else if (other.Is(LayerEnum.Jackal))
        {
            ((Jackal)role).Recruited.Clear();
            ((Jackal)role).EvilRecruit = null;
            ((Jackal)role).GoodRecruit = null;
            ((Jackal)role).BackupRecruit = null;
        }

        if (CustomGameOptions.ThiefSteals)
        {
            if (CustomPlayer.Local == other && other.Is(Faction.Intruder))
                other.Data.Role.TeamType = RoleTeamTypes.Crewmate;

            new Thief().Start<Role>(other).RoleUpdate(role, true);
        }

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == player)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(player, CustomColorManager.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    snitch.Player.GetRole().AllArrows.Add(player.PlayerId, new(snitch.Player, CustomColorManager.Snitch));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && CustomPlayer.Local == player)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(player, CustomColorManager.Revealer));
            }
        }
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        GuessMenu.Update(__instance);

        if (Phone)
        {
            if (MaxPage > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
                    Page = CycleInt(MaxPage, 0, Page, true);
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
                    Page = CycleInt(MaxPage, 0, Page, false);
            }

            foreach (var pair in GuessButtons)
            {
                if (pair.Value.Count > 0)
                    pair.Value.ForEach(x => x?.gameObject?.SetActive(Page == pair.Key));

                GuessButtons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
            }
        }
    }

    public void RpcMurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, ThiefActionsRPC.Guess, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);

        if (Local && player == Player)
            GuessMenu.HideButtons();

        if (player != Player && player.Is(LayerEnum.Indomitable))
        {
            if (player == CustomPlayer.Local)
                Flash(CustomColorManager.Indomitable);

            return;
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            MarkMeetingDead(player, Player);

            if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var otherLover = player.GetOtherLover();

                if (!otherLover.Is(LayerEnum.Pestilence))
                    RpcMurderPlayer(otherLover, guess, guessTarget);
            }
        }

        if (Local)
        {
            if (Player != player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guess}!");
            else
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else if (Player != player && CustomPlayer.Local == player)
            Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessTarget}!");
        else if (DeadSeeEverything)
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
        base.VoteComplete(__instance);
        GuessMenu.HideButtons();
        Exit(__instance);
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        GuessMenu.Voted();
        Exit(__instance);
    }
}