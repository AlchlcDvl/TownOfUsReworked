namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Bullseye : Assassin
{
    public override LayerEnum Type => LayerEnum.Bullseye;
    public override string Name => "Bullseye";
}

public class Hitman : Assassin
{
    public override LayerEnum Type => LayerEnum.Hitman;
    public override string Name => "Hitman";
}

public class Slayer : Assassin
{
    public override LayerEnum Type => LayerEnum.Slayer;
    public override string Name => "Slayer";
}

public class Sniper : Assassin
{
    public override LayerEnum Type => LayerEnum.Sniper;
    public override string Name => "Sniper";
}

[HeaderOption(MultiMenu.LayerSubOptions)]
public abstract class Assassin : Ability
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static int AssassinKills { get; set; } = 1;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinMultiKill { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessNeutralBenign { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessNeutralEvil { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessInvestigative { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessApoc { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessModifiers { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessObjectifiers { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinGuessAbilities { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AssassinateAfterVoting { get; set; } = false;

    private Dictionary<string, UColor> ColorMapping { get; set; }
    private Dictionary<string, UColor> SortedColorMapping { get; set; }
    public static int RemainingKills { get; set; }
    public GameObject Phone { get; set; }
    private Transform SelectedButton { get; set; }
    private int Page { get; set; }
    private int MaxPage { get; set; }
    private Dictionary<int, List<Transform>> Buttons { get; set; }
    private Dictionary<int, KeyValuePair<string, UColor>> Sorted { get; set; }
    public CustomMeeting AssassinMenu { get; set; }
    private Transform Next;
    private Transform Back;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Assassin : CustomColorManager.Ability;
    public override Func<string> Description => () => "- You can guess players mid-meetings";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        ColorMapping = [];
        SortedColorMapping = [];
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        Buttons = [];
        Sorted = [];
        AssassinMenu = new(Player, "Guess", AssassinateAfterVoting, Guess, IsExempt, SetLists);
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if ((!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None)) && CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
                ColorMapping.Add("Crewmate", CustomColorManager.Crew);

            for (var h = 0; h < 26; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Revealer or LayerEnum.Crewmate || (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or
                    LayerEnum.Sheriff or LayerEnum.Tracker && !AssassinGuessInvestigative))
                {
                    continue;
                }

                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                    ColorMapping.Add(entry.Name, entry.Color);
                else if (layer == LayerEnum.Vigilante && !ColorMapping.ContainsKey("Vigilante") && ColorMapping.ContainsKey("Vampire Hunter"))
                    ColorMapping.Add("Vigilante", CustomColorManager.Vigilante);
                else if (layer == LayerEnum.Sheriff && !ColorMapping.ContainsKey("Sheriff") && ColorMapping.ContainsKey("Seer"))
                    ColorMapping.Add("Sheriff", CustomColorManager.Sheriff);
                else if (layer == LayerEnum.Seer && !ColorMapping.ContainsKey("Seer") && ColorMapping.ContainsKey("Mystic"))
                    ColorMapping.Add("Seer", CustomColorManager.Seer);
            }
        }

        if ((!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)) && !SyndicateSettings.AltImps && IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
        {
            if (!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None))
                ColorMapping.Add("Impostor", CustomColorManager.Intruder);

            for (var h = 52; h < 70; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Ghoul or LayerEnum.PromotedGodfather or LayerEnum.Impostor)
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

        if ((!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)) && SyndicateSettings.SyndicateCount > 0)
        {
            if (!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None))
                ColorMapping.Add("Anarchist", CustomColorManager.Syndicate);

            for (var h = 70; h < 88; h++)
            {
                var layer = (LayerEnum)h;

                if (layer is LayerEnum.Banshee or LayerEnum.PromotedRebel or LayerEnum.Anarchist)
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
                LayerEnum.Werewolf };

            foreach (var layer in nks)
            {
                if (!Player.Is(layer) && RoleGen.GetSpawnItem(layer).IsActive())
                {
                    var entry = LayerDictionary[layer];
                    ColorMapping.Add(entry.Name, entry.Color);
                }
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Plaguebearer).IsActive() && !Player.Is(Alignment.NeutralHarb) && !Player.Is(Alignment.NeutralApoc))
            {
                ColorMapping.Add("Plaguebearer", CustomColorManager.Plaguebearer);

                if (AssassinGuessApoc)
                    ColorMapping.Add("Pestilence", CustomColorManager.Pestilence);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Dracula).IsActive() && !Player.Is(SubFaction.Undead))
            {
                ColorMapping.Add("Dracula", CustomColorManager.Dracula);
                ColorMapping.Add("Bitten", CustomColorManager.Undead);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Jackal).IsActive() && !Player.Is(SubFaction.Cabal))
            {
                ColorMapping.Add("Jackal", CustomColorManager.Jackal);
                ColorMapping.Add("Recruit", CustomColorManager.Cabal);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Necromancer).IsActive() && !Player.Is(SubFaction.Reanimated))
            {
                ColorMapping.Add("Necromancer", CustomColorManager.Necromancer);
                ColorMapping.Add("Resurrected", CustomColorManager.Reanimated);
            }

            if (RoleGen.GetSpawnItem(LayerEnum.Whisperer).IsActive() && !Player.Is(SubFaction.Sect))
            {
                ColorMapping.Add("Whisperer", CustomColorManager.Whisperer);
                ColorMapping.Add("Persuaded", CustomColorManager.Sect);
            }

            // Add certain Neutral roles if enabled
            if (AssassinGuessNeutralBenign)
            {
                var nbs = new List<LayerEnum>() { LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief };

                foreach (var layer in nbs)
                {
                    var entry = LayerDictionary[layer];

                    if (RoleGen.GetSpawnItem(layer).IsActive())
                        ColorMapping.Add(entry.Name, entry.Color);
                    else if (layer == LayerEnum.Survivor && ColorMapping.ContainsKey("Guardian Angel") && !ColorMapping.ContainsKey("Survivor"))
                        ColorMapping.Add("Survivor", CustomColorManager.Survivor);
                    else if (layer == LayerEnum.Thief && ColorMapping.ContainsKey("Amnesiac") && !ColorMapping.ContainsKey("Thief"))
                        ColorMapping.Add("Thief", CustomColorManager.Thief);
                }
            }

            if (AssassinGuessNeutralEvil)
            {
                var nes = new List<LayerEnum>() { LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.BountyHunter, LayerEnum.Troll, LayerEnum.Actor, LayerEnum.Jester };

                foreach (var layer in nes)
                {
                    if (RoleGen.GetSpawnItem(layer).IsActive())
                    {
                        var entry = LayerDictionary[layer];
                        ColorMapping.Add(entry.Name, entry.Color);
                    }
                    else if (layer == LayerEnum.Troll && ColorMapping.ContainsKey("Bounty Hunter") && !ColorMapping.ContainsKey("Troll"))
                        ColorMapping.Add("Troll", CustomColorManager.Troll);
                    else if (layer == LayerEnum.Actor && ColorMapping.ContainsKey("Guesser") && !ColorMapping.ContainsKey("Actor"))
                        ColorMapping.Add("Actor", CustomColorManager.Actor);
                    else if (layer == LayerEnum.Jester && ColorMapping.ContainsKey("Executioner") && !ColorMapping.ContainsKey("Jester"))
                        ColorMapping.Add("Jester", CustomColorManager.Jester);
                }
            }
        }

        // Add Modifiers if enabled
        if (AssassinGuessModifiers)
        {
            var mods = new List<LayerEnum>() { LayerEnum.Bait, LayerEnum.Diseased, LayerEnum.Professional, LayerEnum.VIP };

            foreach (var layer in mods)
            {
                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    var entry = LayerDictionary[layer];
                    ColorMapping.Add(entry.Name, entry.Color);
                }
            }
        }

        // Add Objectifiers if enabled
        if (AssassinGuessObjectifiers)
        {
            for (var h = 107; h < 118; h++)
            {
                var layer = (LayerEnum)h;
                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    if ((layer is LayerEnum.Lovers or LayerEnum.Rivals or LayerEnum.Linked or LayerEnum.Mafia && Player.Is(layer)) || (Player.Is(layer) && layer == LayerEnum.Corrupted &&
                        Corrupted.AllCorruptedWin))
                    {
                        continue;
                    }

                    ColorMapping.Add(entry.Name, entry.Color);
                }
            }
        }

        // Add Abilities if enabled
        if (AssassinGuessAbilities)
        {
            for (var h = 107; h < 118; h++)
            {
                var layer = (LayerEnum)h;
                var entry = LayerDictionary[layer];

                if (RoleGen.GetSpawnItem(layer).IsActive())
                {
                    if ((layer == LayerEnum.Hitman && Player.Is(Faction.Intruder)) || (layer == LayerEnum.Sniper && Player.Is(Faction.Syndicate)) || (layer == LayerEnum.Slayer &&
                        Player.Is(Alignment.NeutralKill)) || (layer is LayerEnum.Bullseye or LayerEnum.Snitch && Player.Is(Faction.Intruder)))
                    {
                        continue;
                    }

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
            if (!Buttons.ContainsKey(i))
                Buttons.Add(i, []);

            var row = j / 5;
            var col = j % 5;
            var guess = Sorted[k].Key;
            var buttonParent = new GameObject($"Guess{guess}").transform;
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

                    if (__instance.state == MeetingHud.VoteStates.Discussion || !focusedTarget || RemainingKills <= 0)
                        return;

                    var targetId = voteArea.TargetPlayerId;
                    var targetPlayer = PlayerById(targetId);

                    var playerRole = voteArea.GetRole();
                    var playerAbility = voteArea.GetAbility();
                    var playerModifier = voteArea.GetModifier();
                    var playerObjectifier = voteArea.GetObjectifier();

                    var roleflag = playerRole?.Name == guess;
                    var modifierflag = playerModifier?.Name == guess;
                    var abilityflag = playerAbility?.Name == guess;
                    var objectifierflag = playerObjectifier?.Name == guess;
                    var recruitflag = targetPlayer.IsRecruit() && guess == "Recruit";
                    var sectflag = targetPlayer.IsPersuaded() && guess == "Persuaded";
                    var reanimatedflag = targetPlayer.IsResurrected() && guess == "Resurrected";
                    var undeadflag = targetPlayer.IsBitten() && guess == "Bitten";
                    var framedflag = targetPlayer.IsFramed();

                    if (targetPlayer.Is(LayerEnum.Actor) && guess != "Actor")
                    {
                        var actor = targetPlayer.GetLayer<Actor>();

                        if (actor.PretendRoles.Any(x => x.Name == guess))
                        {
                            actor.Guessed = true;
                            CallRpc(CustomRPC.WinLose, WinLose.ActorWins, actor);

                            if (!NeutralSettings.AvoidNeutralKingmakers)
                                RpcMurderPlayer(Player, guess, targetPlayer);
                        }
                    }

                    var flag = roleflag || modifierflag || abilityflag || objectifierflag || recruitflag || sectflag || reanimatedflag || undeadflag || framedflag;
                    var toDie = flag ? targetPlayer : Player;
                    RpcMurderPlayer(toDie, guess, targetPlayer);
                    Exit(__instance);

                    if (RemainingKills <= 0 || !AssassinMultiKill)
                        AssassinMenu.HideButtons();
                    else
                        AssassinMenu.HideSingle(targetId);
                }
            });

            Buttons[i].Add(button);
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

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && ((Player.GetFaction() != player.GetFaction()) || (Player.GetSubFaction() != Player.GetSubFaction()))) ||
            Dead || (player == Player && player == CustomPlayer.Local) || (Player.GetFaction() == player.GetFaction() && Player.GetFaction() != Faction.Crew) | RemainingKills <= 0 ||
            (Player.GetSubFaction() == player.GetSubFaction() && Player.GetSubFaction() != SubFaction.None) || Player.IsLinkedTo(player);
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

        foreach (var pair in Buttons)
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

        Buttons.Clear();

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
        AssassinMenu.GenButtons(__instance, RemainingKills > 0);
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        AssassinMenu.Update(__instance);

        if (Phone)
        {
            if (MaxPage > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
                    Page = CycleInt(MaxPage, 0, Page, true);
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
                    Page = CycleInt(MaxPage, 0, Page, false);
            }

            foreach (var pair in Buttons)
            {
                if (pair.Value.Any())
                    pair.Value.ForEach(x => x?.gameObject?.SetActive(Page == pair.Key));

                Buttons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
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

        if (Local && player == Player)
            AssassinMenu.HideButtons();

        if (player.Is(LayerEnum.Indomitable) && player != Player)
        {
            if (Local)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You failed to assassinate {guessTarget.name}!");
            else if (player == CustomPlayer.Local)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"Someone tried to assassinate you!");

            Flash(CustomColorManager.Indomitable);
            player.GetLayer<Indomitable>().AttemptedGuess = true;
        }

        if (Player.Is(LayerEnum.Professional) && Player == player)
        {
            var modifier = Player.GetLayer<Professional>();

            if (!modifier.LifeUsed)
            {
                modifier.LifeUsed = true;
                AssassinMenu.HideSingle(guessTarget.PlayerId);

                if (Local)
                {
                    Flash(modifier.Color);
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess} and lost a life!");
                }
                else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything)
                    Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guess} and lost a life!");

                return;
            }
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            RemainingKills--;
            MarkMeetingDead(player, Player);

            if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
            {
                var otherLover = player.GetOtherLover();

                if (!otherLover.Is(Alignment.NeutralApoc))
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
            Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guess}!");
        else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything)
        {
            if (Player != player)
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guess}!");
            else
                Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else
            Run("<color=#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);
        AssassinMenu.HideButtons();
        Exit(__instance);
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        AssassinMenu.Voted();
        Exit(__instance);
    }

    public override void ReadRPC(MessageReader reader) => MurderPlayer(reader.ReadPlayer(), reader.ReadString(), reader.ReadPlayer());
}