namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Guesser : Neutral
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
    public static int MaxGuesses { get; set; } = 5;

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
    public List<string> Letters { get; set; }
    public Dictionary<string, UColor> ColorMapping { get; set; }
    public Dictionary<string, UColor> SortedColorMapping { get; set; }
    public GameObject Phone { get; set; }
    public Transform SelectedButton { get; set; }
    public int Page { get; set; }
    public int MaxPage { get; set; }
    public Dictionary<int, List<Transform>> GuessButtons { get; set; }
    public Dictionary<int, KeyValuePair<string, UColor>> Sorted { get; set; }
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer ? (!TargetGuessed && (RemainingGuesses <= 0 || TargetPlayer.HasDied())) : Rounds > 2;
    public CustomMeeting GuessMenu { get; set; }
    private Transform Next;
    private Transform Back;

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Guesser : CustomColorManager.Neutral;
    public override string Name => "Guesser";
    public override LayerEnum Type => LayerEnum.Guesser;
    public override Func<string> StartText => () => "Guess What Someone Might Be";
    public override Func<string> Description => () => !TargetPlayer ? "- You can select a player to guess their role" : ((TargetGuessed ? "- You can guess player's roles without penalties" :
        $"- You can only try to guess {TargetPlayer?.name}") + $"\n- If {TargetPlayer?.name} dies without getting guessed by you, you will become an <color=#00ACC2FF>Actor</color>");
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralEvil;
        RemainingGuesses = MaxGuesses == 0 ? 10000 : MaxGuesses;
        SortedColorMapping = [];
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        GuessButtons = [];
        Sorted = [];
        ColorMapping = [];
        Objectives = () => TargetGuessed ? $"- You have found out what {TargetPlayer.Data.PlayerName} was" : (!TargetPlayer ? "- Find someone to be guessed by you" : ("- Guess " +
            $"{TargetPlayer?.name}'s role"));
        SetLists();
        GuessMenu = new(Player, "Guess", GuesserAfterVoting, Guess, IsExempt, SetLists);
        Rounds = 0;
        Letters = [];

        if (GuesserCanPickTargets)
        {
            TargetButton = CreateButton(this, new SpriteName("GuessTarget"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)SelectTarget, (PlayerBodyExclusion)Exception,
                (UsableFunc)Usable, "AGONISE");
        }

    }

    public bool Exception(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.CrewInvest) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public void SelectTarget()
    {
        TargetPlayer = TargetButton.TargetPlayer;
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, TargetPlayer);
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        ColorMapping.Add("Crewmate", CustomColorManager.Crew);

        // Adds all the roles that have a non-zero chance of being in the game
        if (CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            for (var h = 0; h < 26; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Revealer or LayerEnum.Crewmate)
                    continue;

                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker)
                        continue;

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
            for (var h = 26; h < 52; h++)
            {
                var layer = (LayerEnum)h;

                if (layer == LayerEnum.Phantom)
                    continue;

                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                    ColorMapping.Add(entry.Name, entry.Color);
                else if (layer == LayerEnum.Survivor && ColorMapping.ContainsKey("Guardian Angel") && !ColorMapping.ContainsKey("Survivor"))
                    ColorMapping.Add("Survivor", CustomColorManager.Survivor);
                else if (layer == LayerEnum.Thief && ColorMapping.ContainsKey("Amnesiac") && !ColorMapping.ContainsKey("Thief"))
                    ColorMapping.Add("Thief", CustomColorManager.Thief);
                else if (layer == LayerEnum.Troll && ColorMapping.ContainsKey("Bounty Hunter") && !ColorMapping.ContainsKey("Troll"))
                    ColorMapping.Add("Troll", CustomColorManager.Troll);
                else if (layer == LayerEnum.Actor && ColorMapping.ContainsKey("Guesser") && !ColorMapping.ContainsKey("Actor"))
                    ColorMapping.Add("Actor", CustomColorManager.Actor);
                else if (layer == LayerEnum.Jester && ColorMapping.ContainsKey("Executioner") && !ColorMapping.ContainsKey("Jester"))
                    ColorMapping.Add("Jester", CustomColorManager.Jester);
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

                    if (__instance.state == MeetingHud.VoteStates.Discussion || !focusedTarget || RemainingGuesses <= 0)
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
                    TargetGuessed = flag;
                    Exit(__instance);
                    RpcMurderPlayer(toDie, guess, targetPlayer);

                    if (RemainingGuesses <= 0 || !MultipleGuesses)
                        GuessMenu.HideButtons();
                    else
                        GuessMenu.HideSingle(targetId);
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
        passive.OverrideOnMouseOverListeners(() => rend.color = UColor.green);
        passive.OverrideOnMouseOutListeners(() => rend.color = SelectedButton == button ? UColor.red : UColor.white);
        passive.OverrideOnClickListeners(onClick);
        passive.ClickSound = GetAudio("Click");
        passive.HoverSound = GetAudio("Hover");
    }

    public void TurnAct()
    {
        Role role = IsRoleList() ? new Jester() : new Actor();
        role.Start<Role>(Player).RoleUpdate(this);
    }

    public bool Usable() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Failed && !Dead)
        {
            if (GuessToAct)
            {
                CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
                TurnAct();
            }
            else if (GuesserCanPickTargets)
            {
                TargetPlayer = null;
                Rounds = 0;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
            }
            else
                Utils.RpcMurderPlayer(Player);
        }
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

        if (RoleName != newRoleName && RoleName != "")
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

        if (IsNullEmptyOrWhiteSpace(something))
            return;

        // Ensures only the Guesser sees this
        if (HUD && something != "")
            Run("<color=#EEE5BEFF>〖 Guess Hint 〗</color>", something);
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (player != TargetPlayer && !TargetGuessed) || player == CustomPlayer.Local || RemainingGuesses <= 0 || Dead || Player.IsLinkedTo(player) || (TargetGuessed
            && NeutralSettings.AvoidNeutralKingmakers);
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
        var exitButtonMask = UObject.Instantiate(voteArea.transform.FindChild("MaskArea"), exitButtonParent);
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);

        if (Player != player)
        {
            TargetGuessed = true;

            if (CanAttack(AttackVal, player.GetDefenseValue(Player)))
            {
                MarkMeetingDead(player, Player);

                if (player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie && AmongUsClient.Instance.AmHost)
                {
                    var otherLover = player.GetOtherLover();

                    if (!otherLover.Is(LayerEnum.Pestilence) && !otherLover.Data.IsDead)
                        RpcMurderPlayer(otherLover, guess, guessTarget);
                }
            }

            if (CustomPlayer.Local == player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guess}!");
            else if (DeadSeeEverything())
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guess}!");
            else if (CanAttack(AttackVal, player.GetDefenseValue(Player)))
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
        }
        else if (Player == player)
        {
            if (!TargetGuessed)
            {
                RemainingGuesses--;

                if (DeadSeeEverything() && !Local)
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{PlayerName} incorrectly guessed {guessTarget.name} as {guess}!");
                else if (Local && !TargetGuessed)
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess}!");
            }
            else if (DeadSeeEverything())
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{PlayerName} incorrectly guessed {guessTarget.name} as {guess}!");
            else if (Local && !TargetGuessed)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess}!");
        }
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

    public override void ReadRPC(MessageReader reader) => MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
}