namespace TownOfUsReworked.PlayerLayers.Abilities;

public class CrewAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.CrewAssassin;
    public override string Name => "Bullseye";

    public CrewAssassin(PlayerControl player) : base(player) {}
}

public class IntruderAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.IntruderAssassin;
    public override string Name => "Hitman";

    public IntruderAssassin(PlayerControl player) : base(player) {}
}

public class NeutralAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.NeutralAssassin;
    public override string Name => "Slayer";

    public NeutralAssassin(PlayerControl player) : base(player) {}
}

public class SyndicateAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.NeutralAssassin;
    public override string Name => "Sniper";

    public SyndicateAssassin(PlayerControl player) : base(player) {}
}

public abstract class Assassin : Ability
{
    public Dictionary<string, Color> ColorMapping { get; set; }
    public Dictionary<string, Color> SortedColorMapping { get; set; }
    public static int RemainingKills { get; set; }
    public GameObject Phone { get; set; }
    public Transform SelectedButton { get; set; }
    public int Page { get; set; }
    public int MaxPage { get; set; }
    public Dictionary<int, List<Transform>> Buttons { get; set; }
    public Dictionary<int, KeyValuePair<string, Color>> Sorted { get; set; }
    public CustomMeeting AssassinMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.Assassin : Colors.Ability;
    public override Func<string> Description => () => "- You can guess players mid-meetings";

    protected Assassin(PlayerControl player) : base(player)
    {
        ColorMapping = new();
        SortedColorMapping = new();
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        Buttons = new();
        Sorted = new();
        AssassinMenu = new(Player, "Guess", CustomGameOptions.AssassinateAfterVoting, Guess, IsExempt, SetLists);
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        //Adds all the roles that have a non-zero chance of being in the game
        if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
        {
            ColorMapping.Add("Crewmate", Colors.Crew);

            if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
            {
                if (CustomGameOptions.MayorOn > 0) ColorMapping.Add("Mayor", Colors.Mayor);
                if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Engineer);
                if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Medic);
                if (CustomGameOptions.AltruistOn > 0) ColorMapping.Add("Altruist", Colors.Altruist);
                if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", Colors.Veteran);
                if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", Colors.Transporter);
                if (CustomGameOptions.ShifterOn > 0) ColorMapping.Add("Shifter", Colors.Shifter);
                if (CustomGameOptions.EscortOn > 0) ColorMapping.Add("Escort", Colors.Escort);
                if ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", Colors.Vigilante);
                if (CustomGameOptions.RetributionistOn > 0) ColorMapping.Add("Retributionist", Colors.Retributionist);
                if (CustomGameOptions.ChameleonOn > 0) ColorMapping.Add("Chameleon", Colors.Chameleon);
                if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.JackalOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.NecromancerOn > 0)) ColorMapping.Add("Mystic", Colors.Mystic);
                if (CustomGameOptions.MonarchOn > 0) ColorMapping.Add("Monarch", Colors.Monarch);
                if (CustomGameOptions.DictatorOn > 0) ColorMapping.Add("Dictator", Colors.Dictator);
                if (CustomGameOptions.BastionOn > 0) ColorMapping.Add("Bastion", Colors.Bastion);
                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);

                if (CustomGameOptions.AssassinGuessInvestigative)
                {
                    if (CustomGameOptions.SeerOn > 0 || CustomGameOptions.SheriffOn > 0) ColorMapping.Add("Sheriff", Colors.Sheriff);
                    if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
                    if (CustomGameOptions.OperativeOn > 0) ColorMapping.Add("Operative", Colors.Operative);
                    if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
                    if (CustomGameOptions.CoronerOn > 0) ColorMapping.Add("Coroner", Colors.Coroner);
                    if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", Colors.Detective);
                    if (CustomGameOptions.SeerOn > 0 || ColorMapping.ContainsKey("Mystic")) ColorMapping.Add("Seer", Colors.Seer);
                }
            }
        }

        if (!(Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)) && !CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
        {
            ColorMapping.Add("Impostor", Colors.Intruder);

            if (CustomGameOptions.IntruderMax > 0 && CustomGameOptions.IntruderMin > 0)
            {
                if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Janitor);
                if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Morphling);
                if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Miner);
                if (CustomGameOptions.WraithOn > 0) ColorMapping.Add("Wraith", Colors.Wraith);
                if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Grenadier);
                if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", Colors.Blackmailer);
                if (CustomGameOptions.CamouflagerOn > 0) ColorMapping.Add("Camouflager", Colors.Camouflager);
                if (CustomGameOptions.DisguiserOn > 0) ColorMapping.Add("Disguiser", Colors.Disguiser);
                if (CustomGameOptions.EnforcerOn > 0) ColorMapping.Add("Enforcer", Colors.Enforcer);
                if (CustomGameOptions.ConsigliereOn > 0) ColorMapping.Add("Consigliere", Colors.Consigliere);
                if (CustomGameOptions.ConsortOn > 0) ColorMapping.Add("Consort", Colors.Consort);

                if (CustomGameOptions.GodfatherOn > 0)
                {
                    ColorMapping.Add("Godfather", Colors.Godfather);
                    ColorMapping.Add("Mafioso", Colors.Mafioso);
                }
            }
        }

        if ((!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)) && CustomGameOptions.SyndicateCount > 0)
        {
            ColorMapping.Add("Anarchist", Colors.Syndicate);

            if (CustomGameOptions.SyndicateMax > 0 && CustomGameOptions.SyndicateMin > 0)
            {
                if (CustomGameOptions.WarperOn > 0) ColorMapping.Add("Warper", Colors.Warper);
                if (CustomGameOptions.ConcealerOn > 0) ColorMapping.Add("Concealer", Colors.Concealer);
                if (CustomGameOptions.ShapeshifterOn > 0) ColorMapping.Add("Shapeshifter", Colors.Shapeshifter);
                if (CustomGameOptions.FramerOn > 0) ColorMapping.Add("Framer", Colors.Framer);
                if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", Colors.Bomber);
                if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", Colors.Poisoner);
                if (CustomGameOptions.CrusaderOn > 0) ColorMapping.Add("Crusader", Colors.Crusader);
                if (CustomGameOptions.StalkerOn > 0) ColorMapping.Add("Stalker", Colors.Stalker);
                if (CustomGameOptions.ColliderOn > 0) ColorMapping.Add("Collider", Colors.Collider);
                if (CustomGameOptions.SpellslingerOn > 0) ColorMapping.Add("Spellslinger", Colors.Spellslinger);
                if (CustomGameOptions.TimekeeperOn > 0) ColorMapping.Add("Timekeeper", Colors.Timekeeper);
                if (CustomGameOptions.SilencerOn > 0) ColorMapping.Add("Silencer", Colors.Silencer);

                if (CustomGameOptions.RebelOn > 0)
                {
                    ColorMapping.Add("Rebel", Colors.Rebel);
                    ColorMapping.Add("Sidekick", Colors.Sidekick);
                }
            }
        }

        if (CustomGameOptions.NeutralMax > 0 && CustomGameOptions.NeutralMin > 0)
        {
            if (CustomGameOptions.ArsonistOn > 0 && !Player.Is(LayerEnum.Arsonist)) ColorMapping.Add("Arsonist", Colors.Arsonist);
            if (CustomGameOptions.GlitchOn > 0 && !Player.Is(LayerEnum.Glitch)) ColorMapping.Add("Glitch", Colors.Glitch);
            if (CustomGameOptions.SerialKillerOn > 0 && !Player.Is(LayerEnum.SerialKiller)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
            if (CustomGameOptions.JuggernautOn > 0 && !Player.Is(LayerEnum.Juggernaut)) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
            if (CustomGameOptions.MurdererOn > 0 && !Player.Is(LayerEnum.Murderer)) ColorMapping.Add("Murderer", Colors.Murderer);
            if (CustomGameOptions.CryomaniacOn > 0 && !Player.Is(LayerEnum.Murderer)) ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);
            if (CustomGameOptions.WerewolfOn > 0 && !Player.Is(LayerEnum.Werewolf)) ColorMapping.Add("Werewolf", Colors.Werewolf);

            if (CustomGameOptions.PlaguebearerOn > 0 && !Player.Is(Alignment.NeutralHarb) && !Player.Is(Alignment.NeutralApoc))
            {
                ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);

                if (CustomGameOptions.AssassinGuessPest)
                    ColorMapping.Add("Pestilence", Colors.Pestilence);
            }

            if (CustomGameOptions.DraculaOn > 0 && !Player.Is(SubFaction.Undead))
            {
                ColorMapping.Add("Dracula", Colors.Dracula);
                ColorMapping.Add("Bitten", Colors.Undead);
            }

            if (CustomGameOptions.JackalOn > 0 && !Player.Is(SubFaction.Cabal))
            {
                ColorMapping.Add("Jackal", Colors.Jackal);
                ColorMapping.Add("Recruit", Colors.Cabal);
            }

            if (CustomGameOptions.NecromancerOn > 0 && !Player.Is(SubFaction.Reanimated))
            {
                ColorMapping.Add("Necromancer", Colors.Necromancer);
                ColorMapping.Add("Resurrected", Colors.Reanimated);
            }

            if (CustomGameOptions.WhispererOn > 0 && !Player.Is(SubFaction.Sect))
            {
                ColorMapping.Add("Whisperer", Colors.Whisperer);
                ColorMapping.Add("Persuaded", Colors.Sect);
            }

            //Add certain Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                if (CustomGameOptions.SurvivorOn > 0 || CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Survivor", Colors.Survivor);
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                if (CustomGameOptions.ThiefOn > 0 || CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Thief", Colors.Thief);
            }

            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.CannibalOn > 0) ColorMapping.Add("Cannibal", Colors.Cannibal);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                if (CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Guesser", Colors.Guesser);
                if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", Colors.BountyHunter);
                if (CustomGameOptions.TrollOn > 0 || CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Troll", Colors.Troll);
                if (CustomGameOptions.ActorOn > 0 || CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Actor", Colors.Actor);
                if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Jester", Colors.Jester);
            }
        }

        //Add Modifiers if enabled
        if (CustomGameOptions.AssassinGuessModifiers)
        {
            if (CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", Colors.Bait);
            if (CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", Colors.Diseased);
            if (CustomGameOptions.ProfessionalOn > 0) ColorMapping.Add("Professional", Colors.Professional);
            if (CustomGameOptions.VIPOn > 0) ColorMapping.Add("VIP", Colors.VIP);
        }

        //Add Objectifiers if enabled
        if (CustomGameOptions.AssassinGuessObjectifiers)
        {
            if (CustomGameOptions.LoversOn > 0 && !Player.Is(LayerEnum.Lovers)) ColorMapping.Add("Lover", Colors.Lovers);
            if (CustomGameOptions.TaskmasterOn > 0) ColorMapping.Add("Taskmaster", Colors.Taskmaster);
            if (CustomGameOptions.CorruptedOn > 0) ColorMapping.Add("Corrupted", Colors.Corrupted);
            if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Traitor);
            if (CustomGameOptions.FanaticOn > 0) ColorMapping.Add("Fanatic", Colors.Fanatic);
            if (CustomGameOptions.RivalsOn > 0 && !Player.Is(LayerEnum.Rivals)) ColorMapping.Add("Rival", Colors.Rivals);
            if (CustomGameOptions.OverlordOn > 0) ColorMapping.Add("Overlord", Colors.Overlord);
            if (CustomGameOptions.AlliedOn > 0) ColorMapping.Add("Allied", Colors.Allied);
            if (CustomGameOptions.LinkedOn > 0 && !Player.Is(LayerEnum.Linked)) ColorMapping.Add("Linked", Colors.Linked);
            if (CustomGameOptions.MafiaOn > 0 && !Player.Is(LayerEnum.Mafia)) ColorMapping.Add("Mafia", Colors.Mafia);
            if (CustomGameOptions.DefectorOn > 0) ColorMapping.Add("Defector", Colors.Defector);
        }

        //Add Abilities if enabled
        if (CustomGameOptions.AssassinGuessAbilities)
        {
            if (CustomGameOptions.CrewAssassinOn > 0) ColorMapping.Add("Bullseye", Colors.Assassin);
            if (CustomGameOptions.IntruderAssassinOn > 0) ColorMapping.Add("Assassin", Colors.Assassin);
            if (CustomGameOptions.SyndicateAssassinOn > 0) ColorMapping.Add("Sniper", Colors.Assassin);
            if (CustomGameOptions.NeutralAssassinOn > 0) ColorMapping.Add("Slayer", Colors.Assassin);
            if (CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", Colors.Torch);
            if (CustomGameOptions.UnderdogOn > 0) ColorMapping.Add("Underdog", Colors.Underdog);
            if (CustomGameOptions.RadarOn > 0) ColorMapping.Add("Radar", Colors.Radar);
            if (CustomGameOptions.TiebreakerOn > 0) ColorMapping.Add("Tiebreaker", Colors.Tiebreaker);
            if (CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", Colors.Multitasker);
            if (CustomGameOptions.SnitchOn > 0 && !Player.Is(Faction.Crew)) ColorMapping.Add("Snitch", Colors.Snitch);
            if (CustomGameOptions.TunnelerOn > 0) ColorMapping.Add("Tunneler", Colors.Tunneler);
            if (CustomGameOptions.ButtonBarryOn > 0) ColorMapping.Add("Button Barry", Colors.ButtonBarry);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Swapper);
            if (CustomGameOptions.PoliticianOn > 0) ColorMapping.Add("Politician", Colors.Politician);
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
        var maskTemplate = voteArea.transform.FindChild("MaskArea");
        var textTemplate = voteArea.NameText;
        SelectedButton = null;
        var i = 0;
        var j = 0;

        for (var k = 0; k < SortedColorMapping.Count; k++)
        {
            var buttonParent = new GameObject("Guess").transform;
            buttonParent.SetParent(Phone.transform);
            var button = UObject.Instantiate(buttonTemplate, buttonParent);
            UObject.Instantiate(maskTemplate, buttonParent);
            var label = UObject.Instantiate(textTemplate, button);
            button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            if (!Buttons.ContainsKey(i))
                Buttons.Add(i, new());

            var row = j / 5;
            var col = j % 5;
            buttonParent.localPosition = new(-3.47f + (1.75f * col), 1.5f - (0.45f * row), -5f);
            buttonParent.localScale = new(0.55f, 0.55f, 1f);
            label.alignment = TextAlignmentOptions.Center;
            label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;
            label.text = Sorted[k].Key;
            label.color = Sorted[k].Value;
            var passive = button.GetComponent<PassiveButton>();
            passive.OnMouseOver = new();
            passive.OnMouseOver.AddListener((Action)(() => button.GetComponent<SpriteRenderer>().color = UColor.green));
            passive.OnMouseOut = new();
            passive.OnMouseOut.AddListener((Action)(() => button.GetComponent<SpriteRenderer>().color = SelectedButton == button ? UColor.red : UColor.white));
            passive.OnClick = new();
            passive.OnClick.AddListener((Action)(() =>
            {
                if (IsDead)
                    return;

                if (SelectedButton != button)
                {
                    if (SelectedButton != null)
                        SelectedButton.GetComponent<SpriteRenderer>().color = UColor.white;

                    SelectedButton = button;
                }
                else
                {
                    var focusedTarget = PlayerByVoteArea(voteArea);

                    if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null || RemainingKills <= 0 )
                        return;

                    var targetId = voteArea.TargetPlayerId;
                    var currentGuess = label.text;
                    var targetPlayer = PlayerById(targetId);

                    var playerRole = Role.GetRole(voteArea);
                    var playerAbility = GetAbility(voteArea);
                    var playerModifier = Modifier.GetModifier(voteArea);
                    var playerObjectifier = Objectifier.GetObjectifier(voteArea);

                    var roleflag = playerRole?.Name == currentGuess;
                    var modifierflag = playerModifier?.Name == currentGuess && CustomGameOptions.AssassinGuessModifiers;
                    var abilityflag = playerAbility?.Name == currentGuess && CustomGameOptions.AssassinGuessAbilities;
                    var objectifierflag = playerObjectifier?.Name == currentGuess && CustomGameOptions.AssassinGuessObjectifiers;
                    var recruitflag = targetPlayer.IsRecruit() && currentGuess == "Recruit";
                    var sectflag = targetPlayer.IsPersuaded() && currentGuess == "Persuaded";
                    var reanimatedflag = targetPlayer.IsResurrected() && currentGuess == "Resurrected";
                    var undeadflag = targetPlayer.IsBitten() && currentGuess == "Bitten";
                    var framedflag = targetPlayer.IsFramed();

                    var actGuessed = false;

                    if (targetPlayer.Is(LayerEnum.Actor) && currentGuess != "Actor")
                    {
                        var actor = Role.GetRole<Actor>(targetPlayer);

                        if (actor.PretendRoles.Any(x => x.Name == currentGuess))
                        {
                            actor.Guessed = true;
                            actGuessed = true;
                            CallRpc(CustomRPC.WinLose, WinLoseRPC.ActorWin, actor);
                        }
                    }

                    var flag = roleflag || modifierflag || abilityflag || objectifierflag || recruitflag || sectflag || reanimatedflag || undeadflag || framedflag || actGuessed;
                    var toDie = flag ? targetPlayer : Player;
                    RpcMurderPlayer(toDie, currentGuess, targetPlayer);

                    if (actGuessed && !CustomGameOptions.AvoidNeutralKingmakers)
                        RpcMurderPlayer(Player, currentGuess, targetPlayer);

                    Exit(__instance);

                    if (RemainingKills <= 0 || !CustomGameOptions.AssassinMultiKill)
                        AssassinMenu.HideButtons();
                    else
                        AssassinMenu.HideSingle(targetId);
                }
            }));

            Buttons[i].Add(button);
            j++;

            if (j >= 40)
            {
                i++;
                j -= 40;
            }
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && ((Player.GetFaction() != player.GetFaction()) || (Player.GetSubFaction() != Player.GetSubFaction()))) ||
            IsDead || (player == Player && player == CustomPlayer.Local) || (Player.GetFaction() == player.GetFaction() && Player.GetFaction() != Faction.Crew) | RemainingKills <= 0 ||
            (Player.GetSubFaction() == player.GetSubFaction() && Player.GetSubFaction() != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (Phone || __instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        AllVoteAreas.ForEach(x => x.gameObject.SetActive(false));
        __instance.TimerText.gameObject.SetActive(false);
        HUD.Chat.SetVisible(false);
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
        HUD.Chat.SetVisible(true);
        SelectedButton = null;
        __instance.TimerText.gameObject.SetActive(true);
        AllVoteAreas.ForEach(x => x.gameObject.SetActive(true));

        foreach (var pair in Buttons)
        {
            foreach (var item in pair.Value)
            {
                item.GetComponent<PassiveButton>().OnClick = new();
                item.GetComponent<PassiveButton>().OnMouseOut = new();
                item.GetComponent<PassiveButton>().OnMouseOver = new();
                item.gameObject.SetActive(false);
                item.gameObject.Destroy();
                item.Destroy();
            }
        }

        Buttons.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        AssassinMenu.GenButtons(__instance, RemainingKills > 0);
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        AssassinMenu.Update();

        if (Phone != null)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f) && MaxPage != 0)
                Page = CycleInt(MaxPage, 0, Page, true);
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f) && MaxPage != 0)
                Page = CycleInt(MaxPage, 0, Page, false);

            foreach (var pair in Buttons)
            {
                if (pair.Value.Count > 0)
                    pair.Value.ForEach(x => x?.gameObject?.SetActive(Page == pair.Key));

                Buttons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
            }
        }
    }

    public void RpcMurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        if (player.Is(LayerEnum.Indomitable) && player != Player)
        {
            if (Local)
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"You failed to assassinate {guessTarget.name}!");
            else if (player == CustomPlayer.Local)
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"Someone tried to assassinate you!");

            Flash(Colors.Indomitable);
            Modifier.GetModifier<Indomitable>(player).AttemptedGuess = true;
        }

        if (Player.Is(LayerEnum.Professional) && Player == player)
        {
            var modifier = Modifier.GetModifier<Professional>(Player);

            if (!modifier.LifeUsed)
            {
                modifier.LifeUsed = true;

                if (Local)
                {
                    Flash(modifier.Color);
                    Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess} and lost a life!");
                }
                else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything)
                    Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guess} and lost a life!");

                return;
            }
        }

        RemainingKills--;
        MarkMeetingDead(player, Player);

        if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var otherLover = player.GetOtherLover();

            if (!otherLover.Is(Alignment.NeutralApoc))
                RpcMurderPlayer(otherLover, guess, guessTarget);
        }

        if (Local)
        {
            if (Player != player)
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guess}!");
            else
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else if (Player != player && CustomPlayer.Local == player)
            Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guess}!");
        else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything)
        {
            if (Player != player)
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guess}!");
            else
                Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else
            Run(HUD.Chat, "<color=#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
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