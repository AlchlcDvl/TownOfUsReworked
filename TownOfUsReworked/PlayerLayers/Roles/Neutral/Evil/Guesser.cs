namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Guesser : NeutralRole
    {
        public PlayerControl TargetPlayer;
        public bool TargetGuessed;
        public int RemainingGuesses;
        public bool FactionHintGiven;
        public bool AlignmentHintGiven;
        public bool InspectorGiven;
        public bool Failed => TargetPlayer == null || (!TargetGuessed && (RemainingGuesses <= 0f || TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected));
        private int lettersGiven;
        private bool lettersExhausted;
        private string roleName = "";
        public List<string> letters = new();
        public Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping = new();
        public GameObject Phone;
        public Dictionary<byte, GameObject> OtherButtons = new();
        public Transform SelectedButton;
        public int Page;
        public int MaxPage;
        public Dictionary<int, List<Transform>> GuessButtons = new();
        public Dictionary<int, KeyValuePair<string, Color>> Sorted = new();

        public Guesser(PlayerControl player) : base(player)
        {
            Name = "Guesser";
            StartText = "Guess What Your Target Might Be";
            RoleType = RoleEnum.Guesser;
            RoleAlignment = RoleAlignment.NeutralEvil;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Guesser : Colors.Neutral;
            RemainingGuesses = CustomGameOptions.GuessCount;
            OtherButtons = new();
            SortedColorMapping = new();
            SelectedButton = null;
            Page = 0;
            MaxPage = 0;
            GuessButtons = new();
            Sorted = new();
            ColorMapping = new();
            Objectives = "- Guess your target's role";
            AbilitiesText = "- You can guess player's roles without penalties after you have successfully guessed your target's role\n- If your target dies without getting guessed by " +
                "you, you will become an <color=#00ACC2FF>Actor</color>";
            Type = LayerEnum.Guesser;
            InspectorResults = InspectorResults.IsCold;
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
                if (CustomGameOptions.MayorOn > 0)
                    ColorMapping.Add("Mayor", Colors.Mayor);

                if (CustomGameOptions.SheriffOn > 0)
                    ColorMapping.Add("Sheriff", Colors.Sheriff);

                if (CustomGameOptions.EngineerOn > 0)
                    ColorMapping.Add("Engineer", Colors.Engineer);

                if (CustomGameOptions.MedicOn > 0)
                    ColorMapping.Add("Medic", Colors.Medic);

                if (CustomGameOptions.AltruistOn > 0)
                    ColorMapping.Add("Altruist", Colors.Altruist);

                if (CustomGameOptions.VeteranOn > 0)
                    ColorMapping.Add("Veteran", Colors.Veteran);

                if (CustomGameOptions.TrackerOn > 0)
                    ColorMapping.Add("Tracker", Colors.Tracker);

                if (CustomGameOptions.OperativeOn > 0)
                    ColorMapping.Add("Operative", Colors.Operative);

                if (CustomGameOptions.TransporterOn > 0)
                    ColorMapping.Add("Transporter", Colors.Transporter);

                if (CustomGameOptions.MediumOn > 0)
                    ColorMapping.Add("Medium", Colors.Medium);

                if (CustomGameOptions.CoronerOn > 0)
                    ColorMapping.Add("Coroner", Colors.Coroner);

                if (CustomGameOptions.DetectiveOn > 0)
                    ColorMapping.Add("Detective", Colors.Detective);

                if (CustomGameOptions.ShifterOn > 0)
                    ColorMapping.Add("Shifter", Colors.Shifter);

                if (CustomGameOptions.InspectorOn > 0)
                    ColorMapping.Add("Inspector", Colors.Inspector);

                if (CustomGameOptions.EscortOn > 0)
                    ColorMapping.Add("Escort", Colors.Escort);

                if (CustomGameOptions.VigilanteOn > 0)
                    ColorMapping.Add("Vigilante", Colors.Vigilante);

                if (CustomGameOptions.RetributionistOn > 0)
                    ColorMapping.Add("Retributionist", Colors.Retributionist);

                if (CustomGameOptions.ChameleonOn > 0)
                    ColorMapping.Add("Chameleon", Colors.Chameleon);

                if (CustomGameOptions.SeerOn > 0)
                    ColorMapping.Add("Seer", Colors.Seer);

                if (CustomGameOptions.MysticOn > 0)
                    ColorMapping.Add("Mystic", Colors.Mystic);

                if (CustomGameOptions.MonarchOn > 0)
                    ColorMapping.Add("Monarch", Colors.Monarch);

                if (CustomGameOptions.DictatorOn > 0)
                    ColorMapping.Add("Dictator", Colors.Dictator);

                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                    ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);
            }

            if (!CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
            {
                ColorMapping.Add("Impostor", Colors.Intruder);

                if (CustomGameOptions.IntruderMax > 0 && CustomGameOptions.IntruderMin > 0)
                {
                    if (CustomGameOptions.JanitorOn > 0)
                        ColorMapping.Add("Janitor", Colors.Janitor);

                    if (CustomGameOptions.MorphlingOn > 0)
                        ColorMapping.Add("Morphling", Colors.Morphling);

                    if (CustomGameOptions.MinerOn > 0)
                        ColorMapping.Add("Miner", Colors.Miner);

                    if (CustomGameOptions.WraithOn > 0)
                        ColorMapping.Add("Wraith", Colors.Wraith);

                    if (CustomGameOptions.GrenadierOn > 0)
                        ColorMapping.Add("Grenadier", Colors.Grenadier);

                    if (CustomGameOptions.BlackmailerOn > 0)
                        ColorMapping.Add("Blackmailer", Colors.Blackmailer);

                    if (CustomGameOptions.CamouflagerOn > 0)
                        ColorMapping.Add("Camouflager", Colors.Camouflager);

                    if (CustomGameOptions.DisguiserOn > 0)
                        ColorMapping.Add("Disguiser", Colors.Disguiser);

                    if (CustomGameOptions.DisguiserOn > 0)
                        ColorMapping.Add("Consigliere", Colors.Consigliere);

                    if (CustomGameOptions.ConsortOn > 0)
                        ColorMapping.Add("Consort", Colors.Consort);

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
                    if (CustomGameOptions.WarperOn > 0)
                        ColorMapping.Add("Warper", Colors.Warper);

                    if (CustomGameOptions.ConcealerOn > 0)
                        ColorMapping.Add("Concealer", Colors.Concealer);

                    if (CustomGameOptions.ShapeshifterOn > 0)
                        ColorMapping.Add("Shapeshifter", Colors.Shapeshifter);

                    if (CustomGameOptions.FramerOn > 0)
                        ColorMapping.Add("Framer", Colors.Framer);

                    if (CustomGameOptions.BomberOn > 0)
                        ColorMapping.Add("Bomber", Colors.Bomber);

                    if (CustomGameOptions.PoisonerOn > 0)
                        ColorMapping.Add("Poisoner", Colors.Poisoner);

                    if (CustomGameOptions.CrusaderOn > 0)
                        ColorMapping.Add("Crusader", Colors.Crusader);

                    if (CustomGameOptions.StalkerOn > 0)
                        ColorMapping.Add("Stalker", Colors.Stalker);

                    if (CustomGameOptions.ColliderOn > 0)
                        ColorMapping.Add("Collider", Colors.Collider);

                    if (CustomGameOptions.SpellslingerOn > 0)
                        ColorMapping.Add("Spellslinger", Colors.Spellslinger);

                    if (CustomGameOptions.TimeKeeperOn > 0)
                        ColorMapping.Add("Time Keeper", Colors.TimeKeeper);

                    if (CustomGameOptions.RebelOn > 0)
                    {
                        ColorMapping.Add("Rebel", Colors.Rebel);
                        ColorMapping.Add("Sidekick", Colors.Sidekick);
                    }
                }
            }

            if (CustomGameOptions.NeutralMax > 0 && CustomGameOptions.NeutralMin > 0)
            {
                if (CustomGameOptions.ArsonistOn > 0)
                    ColorMapping.Add("Arsonist", Colors.Arsonist);

                if (CustomGameOptions.GlitchOn > 0)
                    ColorMapping.Add("Glitch", Colors.Glitch);

                if (CustomGameOptions.SerialKillerOn > 0)
                    ColorMapping.Add("Serial Killer", Colors.SerialKiller);

                if (CustomGameOptions.JuggernautOn > 0)
                    ColorMapping.Add("Juggernaut", Colors.Juggernaut);

                if (CustomGameOptions.MurdererOn > 0)
                    ColorMapping.Add("Murderer", Colors.Murderer);

                if (CustomGameOptions.CryomaniacOn > 0)
                    ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);

                if (CustomGameOptions.WerewolfOn > 0)
                    ColorMapping.Add("Werewolf", Colors.Werewolf);

                if (CustomGameOptions.PlaguebearerOn > 0)
                {
                    ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    ColorMapping.Add("Pestilence", Colors.Pestilence);
                }

                if (CustomGameOptions.DraculaOn > 0)
                    ColorMapping.Add("Dracula", Colors.Dracula);

                if (CustomGameOptions.JackalOn > 0)
                    ColorMapping.Add("Jackal", Colors.Jackal);

                if (CustomGameOptions.NecromancerOn > 0)
                    ColorMapping.Add("Necromancer", Colors.Necromancer);

                if (CustomGameOptions.WhispererOn > 0)
                    ColorMapping.Add("Whisperer", Colors.Whisperer);

                if (CustomGameOptions.AmnesiacOn > 0)
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);

                if (CustomGameOptions.SurvivorOn > 0 || CustomGameOptions.GuardianAngelOn > 0)
                    ColorMapping.Add("Survivor", Colors.Survivor);

                if (CustomGameOptions.GuardianAngelOn > 0)
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);

                if (CustomGameOptions.ThiefOn > 0)
                    ColorMapping.Add("Thief", Colors.Thief);

                if (CustomGameOptions.CannibalOn > 0)
                    ColorMapping.Add("Cannibal", Colors.Cannibal);

                if (CustomGameOptions.ExecutionerOn > 0)
                    ColorMapping.Add("Executioner", Colors.Executioner);

                if (CustomGameOptions.GuesserOn > 0)
                    ColorMapping.Add("Guesser", Colors.Guesser);

                if (CustomGameOptions.BountyHunterOn > 0)
                    ColorMapping.Add("Bounty Hunter", Colors.BountyHunter);

                if (CustomGameOptions.TrollOn > 0 || CustomGameOptions.BountyHunterOn > 0)
                    ColorMapping.Add("Troll", Colors.Troll);

                if (CustomGameOptions.ActorOn > 0 || CustomGameOptions.GuesserOn > 0)
                    ColorMapping.Add("Actor", Colors.Actor);

                if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0)
                    ColorMapping.Add("Jester", Colors.Jester);
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
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
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
                button.GetComponent<SpriteRenderer>().sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;

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
                button.GetComponent<PassiveButton>().OnMouseOver = new();
                button.GetComponent<PassiveButton>().OnMouseOver.AddListener((Action)(() => button.GetComponent<SpriteRenderer>().color = UColor.green));
                button.GetComponent<PassiveButton>().OnMouseOut = new();
                button.GetComponent<PassiveButton>().OnMouseOut.AddListener((Action)(() => button.GetComponent<SpriteRenderer>().color = SelectedButton == button ? UColor.red :
                    UColor.white));
                button.GetComponent<PassiveButton>().OnClick = new();
                button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() =>
                {
                    if (IsDead)
                        return;

                    if (SelectedButton != button)
                        SelectedButton = button;
                    else
                    {
                        var focusedTarget = Utils.PlayerByVoteArea(voteArea);

                        if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null || RemainingGuesses <= 0)
                            return;

                        var targetId = voteArea.TargetPlayerId;
                        var currentGuess = label.text;
                        var targetPlayer = Utils.PlayerById(targetId);

                        var playerRole = GetRole(voteArea);

                        var roleflag = playerRole != null && playerRole.Name == currentGuess;
                        var recruitflag = targetPlayer.IsRecruit() && currentGuess == "Recruit";
                        var sectflag = targetPlayer.IsPersuaded() && currentGuess == "Persuaded";
                        var reanimatedflag = targetPlayer.IsResurrected() && currentGuess == "Resurrected";
                        var undeadflag = targetPlayer.IsBitten() && currentGuess == "Bitten";

                        var flag = roleflag || recruitflag || sectflag || reanimatedflag || undeadflag;
                        var toDie = flag ? playerRole.Player : Player;
                        TargetGuessed = flag;
                        Exit(__instance);
                        RpcMurderPlayer(toDie, currentGuess);

                        if (RemainingGuesses <= 0 || !CustomGameOptions.MultipleGuesses)
                            HideButtons();
                        else
                            HideSingle(targetId);
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

        public void TurnAct()
        {
            var targetRole = GetRole(TargetPlayer);
            var newRole  = new Actor(Player) { PretendRoles = targetRole == null ? InspectorResults.IsBasic : targetRole.InspectorResults };
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Actor);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Failed && !IsDead)
            {
                TurnAct();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnAct);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            if (RemainingGuesses <= 0)
                return;

            var targetRole = GetRole(TargetPlayer);
            var something = "";
            var newRoleName = targetRole.Name;
            var rolechanged = false;

            if (roleName != newRoleName && roleName != "")
            {
                rolechanged = true;
                roleName = newRoleName;
            }
            else if (roleName?.Length == 0)
                roleName = newRoleName;

            if (rolechanged)
            {
                something = "Your target's role changed!";
                lettersGiven = 0;
                lettersExhausted = false;
                letters.Clear();
                FactionHintGiven = false;
                InspectorGiven = false;
            }
            else if (!lettersExhausted)
            {
                var random = URandom.RandomRangeInt(0, roleName.Length);
                var random2 = URandom.RandomRangeInt(0, roleName.Length);
                var random3 = URandom.RandomRangeInt(0, roleName.Length);

                if (lettersGiven <= roleName.Length - 3)
                {
                    while (random == random2 || random2 == random3 || random == random3 || letters.Contains($"{roleName[random]}") || letters.Contains($"{roleName[random2]}") ||
                        letters.Contains($"{roleName[random3]}"))
                    {
                        if (random == random2 || letters.Contains($"{roleName[random2]}"))
                            random2 = URandom.RandomRangeInt(0, roleName.Length);

                        if (random2 == random3 || letters.Contains($"{roleName[random3]}"))
                            random3 = URandom.RandomRangeInt(0, roleName.Length);

                        if (random == random3 || letters.Contains($"{roleName[random]}"))
                            random = URandom.RandomRangeInt(0, roleName.Length);
                    }

                    something = $"Your target's role as the letters {roleName[random]}, {roleName[random2]} and {roleName[random3]} in it!";
                }
                else if (lettersGiven == roleName.Length - 2)
                {
                    while (random == random2 || letters.Contains($"{roleName[random]}") || letters.Contains($"{roleName[random2]}"))
                    {
                        if (letters.Contains($"{roleName[random2]}"))
                            random2 = URandom.RandomRangeInt(0, roleName.Length);

                        if (letters.Contains($"{roleName[random]}"))
                            random = URandom.RandomRangeInt(0, roleName.Length);

                        if (random == random2)
                            random = URandom.RandomRangeInt(0, roleName.Length);
                    }

                    something = $"Your target's role as the letters {roleName[random]} and {roleName[random2]} in it!";
                }
                else if (lettersGiven == roleName.Length - 1)
                {
                    while (letters.Contains($"{roleName[random]}"))
                        random = URandom.RandomRangeInt(0, roleName.Length);

                    something = $"Your target's role as the letter {roleName[random]} in it!";
                }
                else if (lettersGiven == roleName.Length)
                    lettersExhausted = true;

                if (!lettersExhausted)
                {
                    if (lettersGiven <= roleName.Length - 3)
                    {
                        letters.Add($"{roleName[random]}");
                        letters.Add($"{roleName[random2]}");
                        letters.Add($"{roleName[random3]}");
                        lettersGiven += 3;
                    }
                    else if (lettersGiven == roleName.Length - 2)
                    {
                        letters.Add($"{roleName[random]}");
                        letters.Add($"{roleName[random2]}");
                        lettersGiven += 2;
                    }
                    else if (lettersGiven == roleName.Length - 1)
                    {
                        letters.Add($"{roleName[random]}");
                        lettersGiven++;
                    }
                }
            }
            else if (!FactionHintGiven && lettersExhausted)
            {
                something = $"Your target belongs to the {targetRole.FactionName}!";
                FactionHintGiven = true;
            }
            else if (!AlignmentHintGiven && lettersExhausted)
            {
                something = $"Your target's role belongs to {targetRole.RoleAlignment.AlignmentName()} alignment!";
                AlignmentHintGiven = true;
            }
            else if (!InspectorGiven && lettersExhausted)
            {
                something = $"Your target belongs to the {targetRole.InspectorResults} role list!";
                InspectorGiven = true;
            }

            if (string.IsNullOrEmpty(something))
                return;

            //Ensures only the Guesser sees this
            if (HudManager.Instance && something != "")
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);

            foreach (var state in __instance.playerStates)
                GenButton(state, __instance);
        }

        private bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (player != TargetPlayer && !TargetGuessed) || player == PlayerControl.LocalPlayer || RemainingGuesses <= 0 ||
                IsDead || player == Player.GetOtherLover() || player == Player.GetOtherRival();
        }

        public void GenButton(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (IsExempt(voteArea))
            {
                OtherButtons.Add(voteArea.TargetPlayerId, null);
                return;
            }

            var template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = UObject.Instantiate(template, voteArea.transform);
            targetBox.name = "GuessButton";
            targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetManager.GetSprite("Guess");
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick = new();
            button.OnClick.AddListener((Action)(() => Guess(voteArea, __instance)));
            button.OnMouseOut = new();
            button.OnMouseOut.AddListener((Action)(() => renderer.color = UColor.white));
            button.OnMouseOver = new();
            button.OnMouseOver.AddListener((Action)(() => renderer.color = UColor.red));
            var collider = targetBox.GetComponent<BoxCollider2D>();
            collider.size = renderer.sprite.bounds.size;
            collider.offset = Vector2.zero;
            targetBox.transform.GetChild(0).gameObject.Destroy();
            OtherButtons.Add(voteArea.TargetPlayerId, targetBox);
        }

        private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (Phone != null || __instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
                return;

            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));
            __instance.SkipVoteButton.gameObject.SetActive(false);
            __instance.TimerText.gameObject.SetActive(false);
            HudManager.Instance.Chat.SetVisible(false);
            Page = 0;
            var PhoneUI = UObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            var container = UObject.Instantiate(PhoneUI, __instance.transform);
            container.transform.localPosition = new(0, 0, -5f);
            Phone = container.gameObject;
            var exitButtonParent = new GameObject("CustomExitButton").transform;
            exitButtonParent.SetParent(container);
            var exitButton = UObject.Instantiate(voteArea.transform.FindChild("votePlayerBase").transform, exitButtonParent);
            var exitButtonMask = UObject.Instantiate(voteArea.transform.FindChild("MaskArea"), exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = voteArea.Buttons.transform.Find("CancelButton").GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new(2.725f, 2.1f, -5);
            exitButtonParent.transform.localScale = new(0.217f, 0.9f, 1);
            exitButton.GetComponent<PassiveButton>().OnClick = new();
            exitButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => Exit(__instance)));
            SetButtons(__instance, voteArea);
        }

        public void Exit(MeetingHud __instance)
        {
            Phone.Destroy();
            HudManager.Instance.Chat.SetVisible(true);
            __instance.SkipVoteButton.gameObject.SetActive(true);
            __instance.TimerText.gameObject.SetActive(true);
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));

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

        public void HideButtons()
        {
            foreach (var pair in OtherButtons)
                HideSingle(pair.Key);
        }

        public void HideSingle(byte targetId)
        {
            var button = OtherButtons[targetId];

            if (button == null)
                return;

            button.SetActive(false);
            button.GetComponent<PassiveButton>().OnClick = new();
            button.Destroy();
            OtherButtons[targetId] = null;
        }

        public override void UpdateMeeting(MeetingHud __instance)
        {
            base.UpdateMeeting(__instance);

            if (Phone != null)
            {
                if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f) && MaxPage != 0)
                {
                    Page++;

                    if (Page > MaxPage)
                        Page = 0;
                }
                else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f) && MaxPage != 0)
                {
                    Page--;

                    if (Page < 0)
                        Page = MaxPage;
                }

                foreach (var pair in GuessButtons)
                {
                    if (pair.Value.Count > 0)
                    {
                        foreach (var item in pair.Value)
                            item?.gameObject?.SetActive(Page == pair.Key);
                    }

                    GuessButtons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
                }
            }

            if (Failed && !IsDead)
            {
                Exit(__instance);
                TurnAct();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnAct);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public void RpcMurderPlayer(PlayerControl player, string guess)
        {
            MurderPlayer(player, guess);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.GuesserKill);
            writer.Write(PlayerId);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void MurderPlayer(PlayerControl player, string guess)
        {
            if (Player != player)
            {
                if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie && AmongUsClient.Instance.AmHost)
                {
                    var otherLover = player.GetOtherLover();

                    if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Data.IsDead)
                        RpcMurderPlayer(otherLover, guess);
                }

                TargetGuessed = true;
                Utils.MarkMeetingDead(player, Player, false);
            }
            else
            {
                var hudManager = HudManager.Instance;

                if (Player == player)
                {
                    if (!TargetGuessed)
                    {
                        RemainingGuesses--;

                        if (ConstantVariables.DeadSeeEverything && Player != PlayerControl.LocalPlayer)
                            hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{PlayerName} incorrectly guessed {player.name} as {guess} and lost a guess!");
                        else if (Player == PlayerControl.LocalPlayer && !TargetGuessed)
                            hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess} and lost a guess!");
                    }
                    else if (ConstantVariables.DeadSeeEverything && Player != PlayerControl.LocalPlayer)
                        hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{PlayerName} incorrectly guessed {player.name} as {guess}!");
                    else if (Player == PlayerControl.LocalPlayer && !TargetGuessed)
                        hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess}!");
                }
            }
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            HideButtons();
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);

            if (!CustomGameOptions.GuesserAfterVoting)
                HideButtons();
        }
    }
}