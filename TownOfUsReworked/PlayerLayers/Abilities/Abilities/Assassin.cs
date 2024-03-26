namespace TownOfUsReworked.PlayerLayers.Abilities;

public class CrewAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.CrewAssassin;
    public override string Name => "Bullseye";
}

public class IntruderAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.IntruderAssassin;
    public override string Name => "Hitman";
}

public class NeutralAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.NeutralAssassin;
    public override string Name => "Slayer";
}

public class SyndicateAssassin : Assassin
{
    public override LayerEnum Type => LayerEnum.NeutralAssassin;
    public override string Name => "Sniper";
}

public abstract class Assassin : Ability
{
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

    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Assassin : CustomColorManager.Ability;
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
        AssassinMenu = new(Player, "Guess", CustomGameOptions.AssassinateAfterVoting, Guess, IsExempt, SetLists);
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
        {
            ColorMapping.Add("Crewmate", CustomColorManager.Crew);

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
                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Vampire Hunter", CustomColorManager.VampireHunter);
                if (ColorMapping.ContainsKey("Vampire Hunter") || CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", CustomColorManager.Vigilante);
                if (CustomGameOptions.RetributionistOn > 0) ColorMapping.Add("Retributionist", CustomColorManager.Retributionist);
                if (CustomGameOptions.ChameleonOn > 0) ColorMapping.Add("Chameleon", CustomColorManager.Chameleon);
                if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.JackalOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.NecromancerOn > 0)) ColorMapping.Add("Mystic", CustomColorManager.Mystic);
                if (CustomGameOptions.MonarchOn > 0) ColorMapping.Add("Monarch", CustomColorManager.Monarch);
                if (CustomGameOptions.DictatorOn > 0) ColorMapping.Add("Dictator", CustomColorManager.Dictator);
                if (CustomGameOptions.BastionOn > 0) ColorMapping.Add("Bastion", CustomColorManager.Bastion);

                if (CustomGameOptions.AssassinGuessInvestigative)
                {
                    if (CustomGameOptions.SeerOn > 0 || ColorMapping.ContainsKey("Mystic")) ColorMapping.Add("Seer", CustomColorManager.Seer);
                    if (ColorMapping.ContainsKey("Seer") || CustomGameOptions.SheriffOn > 0) ColorMapping.Add("Sheriff", CustomColorManager.Sheriff);
                    if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", CustomColorManager.Tracker);
                    if (CustomGameOptions.OperativeOn > 0) ColorMapping.Add("Operative", CustomColorManager.Operative);
                    if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", CustomColorManager.Medium);
                    if (CustomGameOptions.CoronerOn > 0) ColorMapping.Add("Coroner", CustomColorManager.Coroner);
                    if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", CustomColorManager.Detective);
                }
            }
        }

        if (!(Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)) && !CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
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
                if (CustomGameOptions.ConsigliereOn > 0) ColorMapping.Add("Consigliere", CustomColorManager.Consigliere);
                if (CustomGameOptions.ConsortOn > 0) ColorMapping.Add("Consort", CustomColorManager.Consort);

                if (CustomGameOptions.GodfatherOn > 0)
                {
                    ColorMapping.Add("Godfather", CustomColorManager.Godfather);
                    ColorMapping.Add("Mafioso", CustomColorManager.Mafioso);
                }
            }
        }

        if ((!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)) && CustomGameOptions.SyndicateCount > 0)
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
            if (CustomGameOptions.ArsonistOn > 0 && !Player.Is(LayerEnum.Arsonist)) ColorMapping.Add("Arsonist", CustomColorManager.Arsonist);
            if (CustomGameOptions.GlitchOn > 0 && !Player.Is(LayerEnum.Glitch)) ColorMapping.Add("Glitch", CustomColorManager.Glitch);
            if (CustomGameOptions.SerialKillerOn > 0 && !Player.Is(LayerEnum.SerialKiller)) ColorMapping.Add("Serial Killer", CustomColorManager.SerialKiller);
            if (CustomGameOptions.JuggernautOn > 0 && !Player.Is(LayerEnum.Juggernaut)) ColorMapping.Add("Juggernaut", CustomColorManager.Juggernaut);
            if (CustomGameOptions.MurdererOn > 0 && !Player.Is(LayerEnum.Murderer)) ColorMapping.Add("Murderer", CustomColorManager.Murderer);
            if (CustomGameOptions.CryomaniacOn > 0 && !Player.Is(LayerEnum.Murderer)) ColorMapping.Add("Cryomaniac", CustomColorManager.Cryomaniac);
            if (CustomGameOptions.WerewolfOn > 0 && !Player.Is(LayerEnum.Werewolf)) ColorMapping.Add("Werewolf", CustomColorManager.Werewolf);

            if (CustomGameOptions.PlaguebearerOn > 0 && !Player.Is(Alignment.NeutralHarb) && !Player.Is(Alignment.NeutralApoc))
            {
                ColorMapping.Add("Plaguebearer", CustomColorManager.Plaguebearer);

                if (CustomGameOptions.AssassinGuessPest)
                    ColorMapping.Add("Pestilence", CustomColorManager.Pestilence);
            }

            if (CustomGameOptions.DraculaOn > 0 && !Player.Is(SubFaction.Undead))
            {
                ColorMapping.Add("Dracula", CustomColorManager.Dracula);
                ColorMapping.Add("Bitten", CustomColorManager.Undead);
            }

            if (CustomGameOptions.JackalOn > 0 && !Player.Is(SubFaction.Cabal))
            {
                ColorMapping.Add("Jackal", CustomColorManager.Jackal);
                ColorMapping.Add("Recruit", CustomColorManager.Cabal);
            }

            if (CustomGameOptions.NecromancerOn > 0 && !Player.Is(SubFaction.Reanimated))
            {
                ColorMapping.Add("Necromancer", CustomColorManager.Necromancer);
                ColorMapping.Add("Resurrected", CustomColorManager.Reanimated);
            }

            if (CustomGameOptions.WhispererOn > 0 && !Player.Is(SubFaction.Sect))
            {
                ColorMapping.Add("Whisperer", CustomColorManager.Whisperer);
                ColorMapping.Add("Persuaded", CustomColorManager.Sect);
            }

            // Add certain Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Amnesiac", CustomColorManager.Amnesiac);
                if (CustomGameOptions.SurvivorOn > 0 || CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Survivor", CustomColorManager.Survivor);
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", CustomColorManager.GuardianAngel);
                if (CustomGameOptions.ThiefOn > 0 || CustomGameOptions.AmnesiacOn > 0) ColorMapping.Add("Thief", CustomColorManager.Thief);
            }

            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.CannibalOn > 0) ColorMapping.Add("Cannibal", CustomColorManager.Cannibal);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", CustomColorManager.Executioner);
                if (CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Guesser", CustomColorManager.Guesser);
                if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", CustomColorManager.BountyHunter);
                if (CustomGameOptions.TrollOn > 0 || CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Troll", CustomColorManager.Troll);
                if (CustomGameOptions.ActorOn > 0 || CustomGameOptions.GuesserOn > 0) ColorMapping.Add("Actor", CustomColorManager.Actor);
                if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Jester", CustomColorManager.Jester);
            }
        }

        // Add Modifiers if enabled
        if (CustomGameOptions.AssassinGuessModifiers)
        {
            if (CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", CustomColorManager.Bait);
            if (CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", CustomColorManager.Diseased);
            if (CustomGameOptions.ProfessionalOn > 0) ColorMapping.Add("Professional", CustomColorManager.Professional);
            if (CustomGameOptions.VIPOn > 0) ColorMapping.Add("VIP", CustomColorManager.VIP);
        }

        // Add Objectifiers if enabled
        if (CustomGameOptions.AssassinGuessObjectifiers)
        {
            if (CustomGameOptions.LoversOn > 0 && !Player.Is(LayerEnum.Lovers)) ColorMapping.Add("Lover", CustomColorManager.Lovers);
            if (CustomGameOptions.TaskmasterOn > 0) ColorMapping.Add("Taskmaster", CustomColorManager.Taskmaster);
            if (CustomGameOptions.CorruptedOn > 0) ColorMapping.Add("Corrupted", CustomColorManager.Corrupted);
            if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", CustomColorManager.Traitor);
            if (CustomGameOptions.FanaticOn > 0) ColorMapping.Add("Fanatic", CustomColorManager.Fanatic);
            if (CustomGameOptions.RivalsOn > 0 && !Player.Is(LayerEnum.Rivals)) ColorMapping.Add("Rival", CustomColorManager.Rivals);
            if (CustomGameOptions.OverlordOn > 0) ColorMapping.Add("Overlord", CustomColorManager.Overlord);
            if (CustomGameOptions.AlliedOn > 0) ColorMapping.Add("Allied", CustomColorManager.Allied);
            if (CustomGameOptions.LinkedOn > 0 && !Player.Is(LayerEnum.Linked)) ColorMapping.Add("Linked", CustomColorManager.Linked);
            if (CustomGameOptions.MafiaOn > 0 && !Player.Is(LayerEnum.Mafia)) ColorMapping.Add("Mafia", CustomColorManager.Mafia);
            if (CustomGameOptions.DefectorOn > 0) ColorMapping.Add("Defector", CustomColorManager.Defector);
        }

        // Add Abilities if enabled
        if (CustomGameOptions.AssassinGuessAbilities)
        {
            if (CustomGameOptions.CrewAssassinOn > 0) ColorMapping.Add("Bullseye", CustomColorManager.Assassin);
            if (CustomGameOptions.IntruderAssassinOn > 0) ColorMapping.Add("Assassin", CustomColorManager.Assassin);
            if (CustomGameOptions.SyndicateAssassinOn > 0) ColorMapping.Add("Sniper", CustomColorManager.Assassin);
            if (CustomGameOptions.NeutralAssassinOn > 0) ColorMapping.Add("Slayer", CustomColorManager.Assassin);
            if (CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", CustomColorManager.Torch);
            if (CustomGameOptions.UnderdogOn > 0) ColorMapping.Add("Underdog", CustomColorManager.Underdog);
            if (CustomGameOptions.RadarOn > 0) ColorMapping.Add("Radar", CustomColorManager.Radar);
            if (CustomGameOptions.TiebreakerOn > 0) ColorMapping.Add("Tiebreaker", CustomColorManager.Tiebreaker);
            if (CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", CustomColorManager.Multitasker);
            if (CustomGameOptions.SnitchOn > 0 && !Player.Is(Faction.Crew)) ColorMapping.Add("Snitch", CustomColorManager.Snitch);
            if (CustomGameOptions.TunnelerOn > 0) ColorMapping.Add("Tunneler", CustomColorManager.Tunneler);
            if (CustomGameOptions.ButtonBarryOn > 0) ColorMapping.Add("Button Barry", CustomColorManager.ButtonBarry);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", CustomColorManager.Swapper);
            if (CustomGameOptions.PoliticianOn > 0) ColorMapping.Add("Politician", CustomColorManager.Politician);
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
                    if (SelectedButton != null)
                        SelectedButton.GetComponent<SpriteRenderer>().color = UColor.white;

                    SelectedButton = button;
                    SelectedButton.GetComponent<SpriteRenderer>().color = UColor.red;
                }
                else
                {
                    var focusedTarget = PlayerByVoteArea(voteArea);

                    if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null || RemainingKills <= 0)
                        return;

                    var targetId = voteArea.TargetPlayerId;
                    var targetPlayer = PlayerById(targetId);

                    var playerRole = voteArea.GetRole();
                    var playerAbility = voteArea.GetAbility();
                    var playerModifier = voteArea.GetModifier();
                    var playerObjectifier = voteArea.GetObjectifier();

                    var roleflag = playerRole?.Name == guess;
                    var modifierflag = playerModifier?.Name == guess && CustomGameOptions.AssassinGuessModifiers;
                    var abilityflag = playerAbility?.Name == guess && CustomGameOptions.AssassinGuessAbilities;
                    var objectifierflag = playerObjectifier?.Name == guess && CustomGameOptions.AssassinGuessObjectifiers;
                    var recruitflag = targetPlayer.IsRecruit() && guess == "Recruit";
                    var sectflag = targetPlayer.IsPersuaded() && guess == "Persuaded";
                    var reanimatedflag = targetPlayer.IsResurrected() && guess == "Resurrected";
                    var undeadflag = targetPlayer.IsBitten() && guess == "Bitten";
                    var framedflag = targetPlayer.IsFramed();

                    if (targetPlayer.Is(LayerEnum.Actor) && guess != "Actor")
                    {
                        var actor = targetPlayer.GetRole<Actor>();

                        if (actor.PretendRoles.Any(x => x.Name == guess))
                        {
                            actor.Guessed = true;
                            CallRpc(CustomRPC.WinLose, WinLoseRPC.ActorWin, actor);

                            if (!CustomGameOptions.AvoidNeutralKingmakers)
                                RpcMurderPlayer(Player, guess, targetPlayer);
                        }
                    }

                    var flag = roleflag || modifierflag || abilityflag || objectifierflag || recruitflag || sectflag || reanimatedflag || undeadflag || framedflag;
                    var toDie = flag ? targetPlayer : Player;
                    RpcMurderPlayer(toDie, guess, targetPlayer);
                    Exit(__instance);

                    if (RemainingKills <= 0 || !CustomGameOptions.AssassinMultiKill)
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

        foreach (var pair in Buttons)
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

        Buttons.Clear();

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
            player.GetModifier<Indomitable>().AttemptedGuess = true;
        }

        if (Player.Is(LayerEnum.Professional) && Player == player)
        {
            var modifier = Player.GetModifier<Professional>();

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

            if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
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