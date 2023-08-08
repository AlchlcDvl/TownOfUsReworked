namespace TownOfUsReworked.PlayerLayers.Roles;

public class Thief : Neutral
{
    public DateTime LastStolen { get; set; }
    public CustomButton StealButton { get; set; }
    public Dictionary<string, Color> ColorMapping { get; set; }
    public Dictionary<string, Color> SortedColorMapping { get; set; }
    public GameObject Phone { get; set; }
    public Transform SelectedButton { get; set; }
    public int Page { get; set; }
    public int MaxPage { get; set; }
    public Dictionary<int, List<Transform>> GuessButtons { get; set; }
    public Dictionary<int, KeyValuePair<string, Color>> Sorted { get; set; }
    public CustomMeeting GuessMenu { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
    public override string Name => "Thief";
    public override LayerEnum Type => LayerEnum.Thief;
    public override Func<string> StartText => () => "Steal From The Killers";
    public override Func<string> Description => () => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill";
    public override InspectorResults InspectorResults => InspectorResults.BringsChaos;
    public float Timer => ButtonUtils.Timer(Player, LastStolen, CustomGameOptions.ThiefKillCooldown);

    public Thief(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.NeutralBen;
        StealButton = new(this, "Steal", AbilityTypes.Direct, "ActionSecondary", Steal, Exception);
        ColorMapping = new();
        SortedColorMapping = new();
        SelectedButton = null;
        Page = 0;
        MaxPage = 0;
        GuessButtons = new();
        Sorted = new();
        GuessMenu = new(Player, "Guess", MeetingTypes.Click, CustomGameOptions.ThiefCanGuessAfterVoting, Guess, IsExempt, SetLists);
        SetLists();
    }

    private void SetLists()
    {
        ColorMapping.Clear();
        SortedColorMapping.Clear();
        Sorted.Clear();

        ColorMapping.Add("Crewmate", Colors.Crew);

        //Adds all the roles that have a non-zero chance of being in the game
        if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
        {
            if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", Colors.Veteran);
            if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", Colors.Vigilante);
            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);
        }

        if (!CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
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
                if (CustomGameOptions.DisguiserOn > 0) ColorMapping.Add("Consigliere", Colors.Consigliere);
                if (CustomGameOptions.ConsortOn > 0) ColorMapping.Add("Consort", Colors.Consort);

                if (CustomGameOptions.GodfatherOn > 0)
                {
                    ColorMapping.Add("Godfather", Colors.Godfather);
                    ColorMapping.Add("Mafioso", Colors.Mafioso);
                }
            }
        }

        if (CustomGameOptions.SyndicateCount > 0)
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
                if (CustomGameOptions.TimeKeeperOn > 0) ColorMapping.Add("Time Keeper", Colors.TimeKeeper);
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
            if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
            if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", Colors.Glitch);
            if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
            if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
            if (CustomGameOptions.MurdererOn > 0) ColorMapping.Add("Murderer", Colors.Murderer);
            if (CustomGameOptions.CryomaniacOn > 0) ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
            if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
            if (CustomGameOptions.ThiefOn > 0) ColorMapping.Add("Thief", Colors.Thief);
            if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", Colors.BountyHunter);
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

            if (!GuessButtons.ContainsKey(i))
                GuessButtons.Add(i, new());

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
                    SelectedButton = button;
                else
                {
                    var focusedTarget = PlayerByVoteArea(voteArea);

                    if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null)
                        return;

                    var targetId = voteArea.TargetPlayerId;
                    var currentGuess = label.text;
                    var targetPlayer = PlayerById(targetId);

                    var playerRole = GetRole(voteArea);

                    var roleflag = playerRole != null && playerRole.Name == currentGuess;
                    var recruitflag = targetPlayer.IsRecruit() && currentGuess == "Recruit";
                    var sectflag = targetPlayer.IsPersuaded() && currentGuess == "Persuaded";
                    var reanimatedflag = targetPlayer.IsResurrected() && currentGuess == "Resurrected";
                    var undeadflag = targetPlayer.IsBitten() && currentGuess == "Bitten";

                    var flag = roleflag || recruitflag || sectflag || reanimatedflag || undeadflag;
                    var toDie = flag ? targetPlayer : Player;
                    RpcMurderPlayer(toDie, currentGuess, targetPlayer);
                    Exit(__instance);
                }
            }));

            GuessButtons[i].Add(button);
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
        return player.Data.IsDead || player.Data.Disconnected || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || IsDead || (player == Player &&
            player == CustomPlayer.Local) || Player.GetFaction() == player.GetFaction() || Player.IsLinkedTo(player);
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
        Phone.Destroy();
        HUD.Chat.SetVisible(true);
        SelectedButton = null;
        __instance.TimerText.gameObject.SetActive(true);
        AllVoteAreas.ForEach(x => x.gameObject.SetActive(true));

        foreach (var pair in GuessButtons)
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

        GuessButtons.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GuessMenu.GenButtons(__instance, CustomGameOptions.ThiefCanGuess);
    }

    public void Steal()
    {
        if (IsTooFar(Player, StealButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, StealButton.TargetPlayer, true);

        if (interact[3])
        {
            if (!(StealButton.TargetPlayer.Is(Faction.Intruder) || StealButton.TargetPlayer.Is(Faction.Syndicate) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralKill) ||
                StealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralPros) || StealButton.TargetPlayer.Is(RoleAlignment.CrewKill)))
            {
                Utils.RpcMurderPlayer(Player, Player);
            }
            else
            {
                Utils.RpcMurderPlayer(Player, StealButton.TargetPlayer);
                CallRpc(CustomRPC.Action, ActionsRPC.Steal, this, StealButton.TargetPlayer);
                Steal(this, StealButton.TargetPlayer);
            }
        }

        if (interact[0])
            LastStolen = DateTime.UtcNow;
        else if (interact[1])
            LastStolen.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastStolen.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
        || Player.IsLinkedTo(player);

    public static void Steal(Thief thiefRole, PlayerControl other)
    {
        var role = GetRole(other);
        var thief = thiefRole.Player;
        var target = other.GetTarget();
        var leader = other.GetLeader();

        if (CustomPlayer.Local == other || CustomPlayer.Local == thief)
        {
            Flash(thiefRole.Color);
            role.OnLobby();
            thiefRole.OnLobby();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Anarchist => new Anarchist(thief),
            LayerEnum.Arsonist => new Arsonist(thief) { Doused = ((Arsonist)role).Doused },
            LayerEnum.Blackmailer => new Blackmailer(thief) { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
            LayerEnum.Bomber => new Bomber(thief),
            LayerEnum.Camouflager => new Camouflager(thief),
            LayerEnum.Concealer => new Concealer(thief),
            LayerEnum.Consigliere => new Consigliere(thief) { Investigated = ((Consigliere)role).Investigated },
            LayerEnum.Consort => new Consort(thief),
            LayerEnum.Cryomaniac => new Cryomaniac(thief) { Doused = ((Cryomaniac)role).Doused },
            LayerEnum.Disguiser => new Disguiser(thief),
            LayerEnum.Dracula => new Dracula(thief) { Converted = ((Dracula)role).Converted },
            LayerEnum.Framer => new Framer(thief) { Framed = ((Framer)role).Framed },
            LayerEnum.Glitch => new Glitch(thief),
            LayerEnum.Enforcer => new Enforcer(thief),
            LayerEnum.Godfather => new Godfather(thief),
            LayerEnum.Grenadier => new Grenadier(thief),
            LayerEnum.Impostor => new Impostor(thief),
            LayerEnum.Juggernaut => new Juggernaut(thief) { JuggKills = ((Juggernaut)role).JuggKills },
            LayerEnum.Mafioso => new Mafioso(thief) { Godfather = (Godfather)leader },
            LayerEnum.PromotedGodfather => new PromotedGodfather(thief)
            {
                Investigated = ((PromotedGodfather)role).Investigated,
                BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer
            },
            LayerEnum.Miner => new Miner(thief),
            LayerEnum.Morphling => new Morphling(thief),
            LayerEnum.Rebel => new Rebel(thief),
            LayerEnum.Sidekick => new Sidekick(thief) { Rebel = (Rebel)leader },
            LayerEnum.Shapeshifter => new Shapeshifter(thief),
            LayerEnum.Murderer => new Murderer(thief),
            LayerEnum.Plaguebearer => new Plaguebearer(thief) { Infected = ((Plaguebearer)role).Infected },
            LayerEnum.Pestilence => new Pestilence(thief),
            LayerEnum.SerialKiller => new SerialKiller(thief),
            LayerEnum.Werewolf => new Werewolf(thief),
            LayerEnum.Janitor => new Janitor(thief),
            LayerEnum.Poisoner => new Poisoner(thief),
            LayerEnum.Teleporter => new Teleporter(thief),
            LayerEnum.VampireHunter => new VampireHunter(thief),
            LayerEnum.Veteran => new Veteran(thief) { UsesLeft = ((Veteran)role).UsesLeft },
            LayerEnum.Vigilante => new Vigilante(thief) { UsesLeft = ((Vigilante)role).UsesLeft },
            LayerEnum.Warper => new Warper(thief),
            LayerEnum.Wraith => new Wraith(thief),
            LayerEnum.BountyHunter => new BountyHunter(thief) { TargetPlayer = target },
            LayerEnum.Jackal => new Jackal(thief)
            {
                Recruited = ((Jackal)role).Recruited,
                EvilRecruit = ((Jackal)role).EvilRecruit,
                GoodRecruit = ((Jackal)role).GoodRecruit,
                BackupRecruit = ((Jackal)role).BackupRecruit
            },
            LayerEnum.Necromancer => new Necromancer(thief)
            {
                Resurrected = ((Necromancer)role).Resurrected,
                KillCount = ((Necromancer)role).KillCount,
                ResurrectedCount = ((Necromancer)role).ResurrectedCount
            },
            LayerEnum.Whisperer => new Whisperer(thief)
            {
                Persuaded = ((Whisperer)role).Persuaded,
                WhisperCount = ((Whisperer)role).WhisperCount,
                WhisperConversion = ((Whisperer)role).WhisperConversion
            },
            LayerEnum.Betrayer => new Betrayer(thief) { Faction = role.Faction },
            LayerEnum.Ambusher => new Ambusher(thief),
            LayerEnum.Crusader => new Crusader(thief),
            LayerEnum.PromotedRebel => new PromotedRebel(thief) { Framed = ((PromotedRebel)role).Framed },
            LayerEnum.Stalker => new Stalker(thief) { StalkerArrows = ((Stalker)role).StalkerArrows },
            _ => new Thief(thief),
        };

        newRole.RoleUpdate(thiefRole);

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

        thief.Data.SetImpostor(thief.Is(Faction.Intruder) || (thief.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

        if (CustomGameOptions.ThiefSteals)
        {
            if (CustomPlayer.Local == other && other.Is(Faction.Intruder))
                other.Data.Role.TeamType = RoleTeamTypes.Crewmate;

            var newRole2 = new Thief(other);
            newRole2.RoleUpdate(role);
        }

        if (thief.Is(Faction.Intruder) || thief.Is(Faction.Syndicate) || (thief.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
        {
            foreach (var snitch in Ability.GetAbilities<Snitch>(LayerEnum.Snitch))
            {
                if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && CustomPlayer.Local == thief)
                    LocalRole.AllArrows.Add(snitch.PlayerId, new(thief, Colors.Snitch));
                else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                    GetRole(snitch.Player).AllArrows.Add(thief.PlayerId, new(snitch.Player, Colors.Snitch));
            }

            foreach (var revealer in GetRoles<Revealer>(LayerEnum.Revealer))
            {
                if (revealer.Revealed && CustomPlayer.Local == thief)
                    LocalRole.AllArrows.Add(revealer.PlayerId, new(thief, Colors.Revealer));
            }
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StealButton.Update("STEAL", Timer, CustomGameOptions.ThiefKillCooldown);
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        GuessMenu.Update();

        if (Phone != null)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f) && MaxPage != 0)
                Page = CycleInt(MaxPage, 0, Page, true);
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f) && MaxPage != 0)
                Page = CycleInt(MaxPage, 0, Page, false);

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
        CallRpc(CustomRPC.Action, ActionsRPC.ThiefKill, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, string guess, PlayerControl guessTarget)
    {
        if (player != Player && player.Is(LayerEnum.Indomitable))
        {
            if (player == CustomPlayer.Local)
                Flash(Colors.Indomitable);

            return;
        }

        MarkMeetingDead(player, Player);

        if (Player != player)
            Steal(this, player);

        if (AmongUsClient.Instance.AmHost && player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var otherLover = player.GetOtherLover();

            if (!otherLover.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(otherLover, guess, guessTarget);
        }

        if (Local)
        {
            if (Player != player)
            {
                HUD.Chat.AddChat(CustomPlayer.Local, $"You guessed {guessTarget.name} as {guess}!");
                GuessMenu.HideButtons();
            }
            else
                HUD.Chat.AddChat(CustomPlayer.Local, $"You incorrectly guessed {guessTarget.name} as {guess} and died!");
        }
        else if (Player != player && CustomPlayer.Local == player)
            HUD.Chat.AddChat(CustomPlayer.Local, $"{Player.name} guessed you as {guessTarget}!");
        else if (DeadSeeEverything)
        {
            if (Player != player)
                HUD.Chat.AddChat(CustomPlayer.Local, $"{Player.name} guessed {player.name} as {guessTarget} and stole their role!");
            else
                HUD.Chat.AddChat(CustomPlayer.Local, $"{Player.name} incorrectly guessed {player.name} as {guessTarget} and died!");
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
}