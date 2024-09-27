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

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Thief : CustomColorManager.Neutral;
    public override string Name => "Thief";
    public override LayerEnum Type => LayerEnum.Thief;
    public override Func<string> StartText => () => "Steal From The Killers";
    public override Func<string> Description => () => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralBen;
        StealButton = CreateButton(this, new SpriteName("Steal"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Steal, new Cooldown(StealCd), "STEAL",
            (PlayerBodyExclusion)Exception);
        ColorMapping = [];
        SortedColorMapping = [];
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        GuessButtons = [];
        Sorted = [];
        GuessMenu = new(Player, "Guess", ThiefCanGuessAfterVoting, Guess, IsExempt, SetLists);
        SetLists();
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            var nks = new List<LayerEnum>() { LayerEnum.VampireHunter, LayerEnum.Bastion, LayerEnum.Veteran, LayerEnum.Vigilante };

            foreach (var layer in nks)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    var entry = LayerDictionary[layer];
                    ColorMapping.Add(entry.Name, entry.Color);
                }
                else if (layer == LayerEnum.Vigilante && !ColorMapping.ContainsKey("Vigilante") && ColorMapping.ContainsKey("Vampire Hunter"))
                    ColorMapping.Add("Vigilante", CustomColorManager.Vigilante);
            }
        }

        if (!SyndicateSettings.AltImps && IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
        {
            for (var h = 52; h < 70; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Ghoul or LayerEnum.PromotedGodfather)
                    continue;

                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                    ColorMapping.Add(entry.Name, entry.Color);
                else if (layer == LayerEnum.Mafioso && !ColorMapping.ContainsKey("Mafioso") && ColorMapping.ContainsKey("Godfather"))
                    ColorMapping.Add("Mafioso", CustomColorManager.Mafioso);
            }

            if (ColorMapping.ContainsKey("Miner") && MapPatches.CurrentMap == 5)
            {
                ColorMapping["Herbalist"] = ColorMapping["Miner"];
                ColorMapping.Remove("Miner");
            }
        }

        if (SyndicateSettings.SyndicateCount > 0)
        {
            for (var h = 70; h < 88; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Banshee or LayerEnum.PromotedRebel)
                    continue;

                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                    ColorMapping.Add(entry.Name, entry.Color);
                else if (layer == LayerEnum.Sidekick && !ColorMapping.ContainsKey("Sidekick") && ColorMapping.ContainsKey("Rebel"))
                    ColorMapping.Add("Sidekick", CustomColorManager.Sidekick);
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0)
        {
            var nks = new List<LayerEnum>() { LayerEnum.Arsonist, LayerEnum.Glitch, LayerEnum.SerialKiller, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Cryomaniac,
                LayerEnum.Werewolf, LayerEnum.BountyHunter, LayerEnum.Plaguebearer };

            foreach (var layer in nks)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    var entry = LayerDictionary[layer];
                    ColorMapping.Add(entry.Name, entry.Color);
                }
            }
        }

        // Sorts the list alphabetically.
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
                GuessButtons.Add(i, []);

            var row = j / 5;
            var col = j % 5;
            var guess = Sorted[k].Key;
            var buttonParent = new GameObject($"Guess{i}").transform;
            buttonParent.SetParent(Phone.transform);
            var button = UObject.Instantiate(buttonTemplate, buttonParent);
            MakeTheButton(button, buttonParent, voteArea, new(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -5f), guess, Sorted[k].Value, () =>
            {
                if (Dead)
                    return;

                if (SelectedButton != button)
                {
                    if (SelectedButton)
                        SelectedButton.GetComponent<SpriteRenderer>().color = UColor.white;

                    SelectedButton = button;
                    SelectedButton.GetComponent<SpriteRenderer>().color = UColor.red;
                }
                else
                {
                    var focusedTarget = PlayerByVoteArea(voteArea);

                    if (__instance.state == MeetingHud.VoteStates.Discussion || !focusedTarget)
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
        rend.sprite = Ship().CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
        buttonParent.localPosition = position;
        buttonParent.localScale = new(0.55f, 0.55f, 1f);
        label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
        label.transform.localScale *= 1.7f;
        label.text = title;
        label.color = color;
        var passive = button.GetComponent<PassiveButton>();
        passive.OverrideOnMouseOverListeners(() => rend.color = UColor.green);
        passive.OverrideOnMouseOutListeners(() => rend.color = SelectedButton == button ? UColor.red : UColor.white);
        passive.OverrideOnClickListeners(onClick);
        passive.ClickSound = GetAudio("Click");
        passive.HoverSound = GetAudio("Hover");
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || Dead || (player == Player && player.AmOwner) ||
            (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (Phone || __instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        AllVoteAreas().ForEach(x => x.gameObject.SetActive(false));
        __instance.TimerText.gameObject.SetActive(false);
        Chat().SetVisible(false);
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
        exitButton.GetComponent<PassiveButton>().OverrideOnClickListeners(() => Exit(__instance));
        SetButtons(__instance, voteArea);
    }

    public void Exit(MeetingHud __instance)
    {
        if (!Phone)
            return;

        Phone.Destroy();
        Chat().SetVisible(true);
        SelectedButton = null;
        __instance.TimerText.gameObject.SetActive(true);
        AllVoteAreas().ForEach(x => x.gameObject.SetActive(true));

        foreach (var pair in GuessButtons)
        {
            foreach (var item in pair.Value)
            {
                if (!item)
                    continue;

                item.GetComponent<PassiveButton>().WipeListeners();
                item.gameObject.SetActive(false);
                item.gameObject.Destroy();
                item.Destroy();
            }
        }

        GuessButtons.Clear();

        if (MaxPage > 0)
        {
            Next.GetComponent<PassiveButton>().WipeListeners();
            Next.gameObject.SetActive(false);
            Next.gameObject.Destroy();
            Next.Destroy();

            Back.GetComponent<PassiveButton>().WipeListeners();
            Back.gameObject.SetActive(false);
            Back.gameObject.Destroy();
            Back.Destroy();
        }
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
                MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
                break;

            default:
                Error($"Received unknown RPC - {(int)thiefAction}");
                break;
        }
    }

    public void Steal()
    {
        var cooldown = Interact(Player, StealButton.TargetPlayer, true);

        if (cooldown != CooldownType.Fail)
        {
            if (StealButton.TargetPlayer.GetFaction() is Faction.Intruder or Faction.Syndicate || StealButton.TargetPlayer.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                Alignment.NeutralPros or Alignment.CrewKill || StealButton.TargetPlayer.GetRole() is VampireHunter)
            {
                Utils.RpcMurderPlayer(Player, StealButton.TargetPlayer);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ThiefActionsRPC.Steal, StealButton.TargetPlayer);
                Steal(StealButton.TargetPlayer);
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

        if (CustomPlayer.Local == other || CustomPlayer.Local == player)
        {
            Flash(Color);
            role.OnLobby();
            OnLobby();
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
            ((Jackal)role).Recruit2 = null;
            ((Jackal)role).Recruit1 = null;
            ((Jackal)role).Recruit3 = null;
        }

        if (ThiefSteals)
        {
            if (CustomPlayer.Local == other && other.Is(Faction.Intruder))
                other.Data.Role.TeamType = RoleTeamTypes.Crewmate;

            new Thief().Start<Role>(other).RoleUpdate(role, true);
        }

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && Snitch.SnitchSeesNeutrals))
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && CustomPlayer.Local == player)
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
                if (pair.Value.Any())
                    pair.Value.ForEach(x => x?.gameObject?.SetActive(Page == pair.Key));

                GuessButtons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
            }
        }
    }

    public void RpcMurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ThiefActionsRPC.Guess, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);

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
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guess}!");
            else
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else if (Player != player && CustomPlayer.Local == player)
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