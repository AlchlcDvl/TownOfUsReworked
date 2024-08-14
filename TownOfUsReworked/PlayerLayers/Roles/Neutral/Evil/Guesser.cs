namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Guesser : Neutral
{
    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuesserCanPickTargets { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuesserButton { get; set; } = true;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuessVent { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuessSwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuessTargetKnows { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool MultipleGuesses { get; set; } = true;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxGuesses { get; set; } = 5;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool GuesserAfterVoting { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool VigiKillsGuesser { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
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
        RemainingGuesses = CustomGameOptions.MaxGuesses;
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
        GuessMenu = new(Player, "Guess", CustomGameOptions.GuesserAfterVoting, Guess, IsExempt, SetLists);
        Rounds = 0;
        Letters = [];

        if (CustomGameOptions.GuesserCanPickTargets)
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
        if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
        {
            if (CustomGameOptions.MayorOn > 0) ColorMapping.Add("Mayor", CustomColorManager.Mayor);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", CustomColorManager.Engineer);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", CustomColorManager.Medic);
            if (CustomGameOptions.AltruistOn > 0) ColorMapping.Add("Altruist", CustomColorManager.Altruist);
            if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", CustomColorManager.Veteran);
            if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", CustomColorManager.Transporter);
            if (CustomGameOptions.ShifterOn > 0) ColorMapping.Add("Shifter", CustomColorManager.Shifter);
            if (CustomGameOptions.EscortOn > 0) ColorMapping.Add("Escort", CustomColorManager.Escort);
            if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", CustomColorManager.Vigilante);
            if (CustomGameOptions.RetributionistOn > 0) ColorMapping.Add("Retributionist", CustomColorManager.Retributionist);
            if (CustomGameOptions.ChameleonOn > 0) ColorMapping.Add("Chameleon", CustomColorManager.Chameleon);
            if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.JackalOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.NecromancerOn > 0)) ColorMapping.Add("Mystic", CustomColorManager.Mystic);
            if (CustomGameOptions.MonarchOn > 0) ColorMapping.Add("Monarch", CustomColorManager.Monarch);
            if (CustomGameOptions.DictatorOn > 0) ColorMapping.Add("Dictator", CustomColorManager.Dictator);
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
                if (CustomGameOptions.EnforcerOn > 0) ColorMapping.Add("Enforcer", CustomColorManager.Enforcer);
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
            if (CustomGameOptions.GlitchOn > 0 ) ColorMapping.Add("Glitch", CustomColorManager.Glitch);
            if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", CustomColorManager.SerialKiller);
            if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", CustomColorManager.Juggernaut);
            if (CustomGameOptions.MurdererOn > 0) ColorMapping.Add("Murderer", CustomColorManager.Murderer);
            if (CustomGameOptions.CryomaniacOn > 0) ColorMapping.Add("Cryomaniac", CustomColorManager.Cryomaniac);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", CustomColorManager.Werewolf);
            if (CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Dracula", CustomColorManager.Dracula);
            if (CustomGameOptions.JackalOn > 0) ColorMapping.Add("Jackal", CustomColorManager.Jackal);
            if (CustomGameOptions.NecromancerOn > 0) ColorMapping.Add("Necromancer", CustomColorManager.Necromancer);
            if (CustomGameOptions.WhispererOn > 0) ColorMapping.Add("Whisperer", CustomColorManager.Whisperer);
            if (CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Amnesiac", CustomColorManager.Amnesiac);
            if (CustomGameOptions.SurvivorOn > 0 || CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Survivor", CustomColorManager.Survivor);
            if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", CustomColorManager.GuardianAngel);
            if (CustomGameOptions.ThiefOn > 0 || CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Thief", CustomColorManager.Thief);
            if (CustomGameOptions.CannibalOn > 0) ColorMapping.Add("Cannibal", CustomColorManager.Cannibal);
            if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", CustomColorManager.Executioner);
            if (CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Guesser", CustomColorManager.Guesser);
            if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", CustomColorManager.BountyHunter);
            if (CustomGameOptions.TrollOn > 0 || CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Troll", CustomColorManager.Troll);
            if (CustomGameOptions.ActorOn > 0 || CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Actor", CustomColorManager.Actor);
            if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Jester", CustomColorManager.Jester);

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                ColorMapping.Add("Plaguebearer", CustomColorManager.Plaguebearer);
                ColorMapping.Add("Pestilence", CustomColorManager.Pestilence);
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

                    if (RemainingGuesses <= 0 || !CustomGameOptions.MultipleGuesses)
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
        passive.ClickSound = SoundEffects["Click"];
        passive.HoverSound = SoundEffects["Hover"];
    }

    public void TurnAct()
    {
        Role role = IsRoleList ? new Jester() : new Actor();
        role.Start<Role>(Player).RoleUpdate(this);
    }

    public bool Usable() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Failed && !Dead)
        {
            if (CustomGameOptions.GuessToAct)
            {
                CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
                TurnAct();
            }
            else if (CustomGameOptions.GuesserCanPickTargets)
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
            && CustomGameOptions.AvoidNeutralKingmakers);
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

                if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie && AmongUsClient.Instance.AmHost)
                {
                    var otherLover = player.GetOtherLover();

                    if (!otherLover.Is(LayerEnum.Pestilence) && !otherLover.Data.IsDead)
                        RpcMurderPlayer(otherLover, guess, guessTarget);
                }
            }

            if (CustomPlayer.Local == player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guess}!");
            else if (DeadSeeEverything)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guess}!");
            else if (CanAttack(AttackVal, player.GetDefenseValue(Player)))
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
        }
        else if (Player == player)
        {
            if (!TargetGuessed)
            {
                RemainingGuesses--;

                if (DeadSeeEverything && !Local)
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{PlayerName} incorrectly guessed {guessTarget.name} as {guess}!");
                else if (Local && !TargetGuessed)
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess}!");
            }
            else if (DeadSeeEverything)
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