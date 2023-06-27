namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Assassin : Ability
    {
        public Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping = new();
        public static int RemainingKills = CustomGameOptions.AssassinKills;
        private static bool AssassinOn => CustomGameOptions.CrewAssassinOn > 0 || CustomGameOptions.IntruderAssassinOn > 0 || CustomGameOptions.SyndicateAssassinOn > 0 ||
            CustomGameOptions.NeutralAssassinOn > 0;
        public GameObject Phone;
        public Dictionary<byte, GameObject> OtherButtons = new();
        public Transform SelectedButton;
        public int Page;
        public int MaxPage;
        public Dictionary<int, List<Transform>> Buttons = new();
        public Dictionary<int, KeyValuePair<string, Color>> Sorted = new();

        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            TaskText = () => "- You can guess players mid-meetings";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Assassin : Colors.Ability;
            AbilityType = AbilityEnum.Assassin;
            ColorMapping = new();
            SortedColorMapping = new();
            SelectedButton = null;
            Page = 0;
            MaxPage = 0;
            Buttons = new();
            OtherButtons = new();
            Sorted = new();
            Type = LayerEnum.Assassin;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        private void SetLists()
        {
            ColorMapping.Clear();
            SortedColorMapping.Clear();
            Sorted.Clear();

            //Adds all the roles that have a non-zero chance of being in the game
            if (!Player.Is(Faction.Crew))
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
                    if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", Colors.Vigilante);
                    if (CustomGameOptions.RetributionistOn > 0) ColorMapping.Add("Retributionist", Colors.Retributionist);
                    if (CustomGameOptions.ChameleonOn > 0) ColorMapping.Add("Chameleon", Colors.Chameleon);
                    if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Mystic);
                    if (CustomGameOptions.MonarchOn > 0) ColorMapping.Add("Monarch", Colors.Monarch);
                    if (CustomGameOptions.DictatorOn > 0) ColorMapping.Add("Dictator", Colors.Dictator);
                    if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);

                    if (CustomGameOptions.AssassinGuessInvestigative)
                    {
                        if (CustomGameOptions.SheriffOn > 0) ColorMapping.Add("Sheriff", Colors.Sheriff);
                        if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
                        if (CustomGameOptions.OperativeOn > 0) ColorMapping.Add("Operative", Colors.Operative);
                        if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
                        if (CustomGameOptions.CoronerOn > 0) ColorMapping.Add("Coroner", Colors.Coroner);
                        if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", Colors.Detective);
                        if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", Colors.Seer);
                        if (CustomGameOptions.InspectorOn > 0) ColorMapping.Add("Inspector", Colors.Inspector);
                    }
                }
            }

            if (!(Player.Is(Faction.Intruder) || CustomGameOptions.AltImps))
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
                    if (CustomGameOptions.DisguiserOn > 0) ColorMapping.Add("Consigliere", Colors.Consigliere);
                    if (CustomGameOptions.ConsortOn > 0) ColorMapping.Add("Consort", Colors.Consort);

                    if (CustomGameOptions.GodfatherOn > 0)
                    {
                        ColorMapping.Add("Godfather", Colors.Godfather);
                        ColorMapping.Add("Mafioso", Colors.Mafioso);
                    }
                }
            }

            if (!Player.Is(Faction.Syndicate) && CustomGameOptions.SyndicateCount > 0)
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
                if (CustomGameOptions.ArsonistOn > 0 && !Player.Is(RoleEnum.Arsonist) && Player.Is(SubFaction.None)) ColorMapping.Add("Arsonist", Colors.Arsonist);
                if (CustomGameOptions.GlitchOn > 0 && !Player.Is(RoleEnum.Glitch) && Player.Is(SubFaction.None)) ColorMapping.Add("Glitch", Colors.Glitch);
                if (CustomGameOptions.SerialKillerOn > 0 && !Player.Is(RoleEnum.SerialKiller) && Player.Is(SubFaction.None)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                if (CustomGameOptions.JuggernautOn > 0 && !Player.Is(RoleEnum.Juggernaut) && Player.Is(SubFaction.None)) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                if (CustomGameOptions.MurdererOn > 0 && !Player.Is(RoleEnum.Murderer) && Player.Is(SubFaction.None)) ColorMapping.Add("Murderer", Colors.Murderer);
                if (CustomGameOptions.CryomaniacOn > 0 && !Player.Is(RoleEnum.Murderer) && Player.Is(SubFaction.None)) ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);
                if (CustomGameOptions.WerewolfOn > 0 && !Player.Is(RoleEnum.Werewolf) && Player.Is(SubFaction.None)) ColorMapping.Add("Werewolf", Colors.Werewolf);

                if (CustomGameOptions.PlaguebearerOn > 0 && Player.Is(SubFaction.None))
                {
                    if (!Player.Is(RoleEnum.Pestilence) && CustomGameOptions.AssassinGuessPest)
                        ColorMapping.Add("Pestilence", Colors.Pestilence);

                    if (!Player.Is(RoleEnum.Plaguebearer))
                        ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
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
                if (CustomGameOptions.LoversOn > 0 && !Player.Is(ObjectifierEnum.Lovers)) ColorMapping.Add("Lover", Colors.Lovers);
                if (CustomGameOptions.TaskmasterOn > 0) ColorMapping.Add("Taskmaster", Colors.Taskmaster);
                if (CustomGameOptions.CorruptedOn > 0) ColorMapping.Add("Corrupted", Colors.Corrupted);
                if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Traitor);
                if (CustomGameOptions.FanaticOn > 0) ColorMapping.Add("Fanatic", Colors.Fanatic);
                if (CustomGameOptions.RivalsOn > 0 && !Player.Is(ObjectifierEnum.Rivals)) ColorMapping.Add("Rival", Colors.Rivals);
                if (CustomGameOptions.OverlordOn > 0) ColorMapping.Add("Overlord", Colors.Overlord);
                if (CustomGameOptions.AlliedOn > 0) ColorMapping.Add("Allied", Colors.Allied);
                if (CustomGameOptions.MafiaOn > 0 && !Player.Is(ObjectifierEnum.Mafia)) ColorMapping.Add("Mafia", Colors.Mafia);
                if (CustomGameOptions.DefectorOn > 0) ColorMapping.Add("Defector", Colors.Defector);
            }

            //Add Abilities if enabled
            if (CustomGameOptions.AssassinGuessAbilities)
            {
                if (AssassinOn) ColorMapping.Add("Assassin", Colors.Assassin);
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
                button.GetComponent<SpriteRenderer>().sprite = HatManager.Instance.GetNamePlateById("nameplate_NoPlate")?.CreateAddressableAsset()?.GetAsset()?.Image;

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
                        SelectedButton = button;
                    else
                    {
                        var focusedTarget = Utils.PlayerByVoteArea(voteArea);

                        if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null || RemainingKills <= 0 )
                            return;

                        var targetId = voteArea.TargetPlayerId;
                        var currentGuess = label.text;
                        var targetPlayer = Utils.PlayerById(targetId);

                        var playerRole = Role.GetRole(voteArea);
                        var playerAbility = GetAbility(voteArea);
                        var playerModifier = Modifier.GetModifier(voteArea);
                        var playerObjectifier = Objectifier.GetObjectifier(voteArea);

                        var roleflag = playerRole != null && playerRole.Name == currentGuess;
                        var modifierflag = playerModifier != null && playerModifier.Name == currentGuess && CustomGameOptions.AssassinGuessModifiers;
                        var abilityflag = playerAbility != null && playerAbility.Name == currentGuess && CustomGameOptions.AssassinGuessAbilities;
                        var objectifierflag = playerObjectifier != null && playerObjectifier.Name == currentGuess && CustomGameOptions.AssassinGuessObjectifiers;
                        var recruitflag = targetPlayer.IsRecruit() && currentGuess == "Recruit";
                        var sectflag = targetPlayer.IsPersuaded() && currentGuess == "Persuaded";
                        var reanimatedflag = targetPlayer.IsResurrected() && currentGuess == "Resurrected";
                        var undeadflag = targetPlayer.IsBitten() && currentGuess == "Bitten";
                        var framedflag = targetPlayer.IsFramed();

                        var actGuessed = false;

                        if (targetPlayer.Is(RoleEnum.Actor) && currentGuess != "Actor")
                        {
                            var actor = Role.GetRole<Actor>(targetPlayer);

                            if (Role.GetRoles(actor.PretendRoles).Any(x => x.Name == currentGuess))
                            {
                                actor.Guessed = true;
                                actGuessed = true;
                                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                                writer.Write((byte)WinLoseRPC.ActorWin);
                                writer.Write(targetPlayer.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                            }
                        }

                        var flag = roleflag || modifierflag || abilityflag || objectifierflag || recruitflag || sectflag || reanimatedflag || undeadflag || framedflag || actGuessed;
                        var toDie = flag ? playerRole.Player : Player;
                        RpcMurderPlayer(toDie, currentGuess);

                        if (actGuessed && !CustomGameOptions.AvoidNeutralKingmakers)
                            RpcMurderPlayer(Player, currentGuess);

                        Exit(__instance);

                        if (RemainingKills < 0 || !CustomGameOptions.AssassinMultiKill)
                            HideButtons();
                        else
                            HideSingle(targetId);
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
            var player = Utils.PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || (player == Player &&
                player == CustomPlayer.Local) || Player.GetFaction() == player.GetFaction() || player == Player.GetOtherLover() || player == Player.GetOtherRival() ||
                RemainingKills <= 0 || IsDead;
        }

        public void GenButton(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (IsExempt(voteArea))
            {
                OtherButtons.Add(voteArea.TargetPlayerId, null);
                return;
            }

            var targetBox = UObject.Instantiate(voteArea.Buttons.transform.Find("CancelButton").gameObject, voteArea.transform);
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
            __instance.TimerText.gameObject.SetActive(false);
            Utils.HUD.Chat.SetVisible(false);
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
            exitButton.GetComponent<PassiveButton>().OnClick = new();
            exitButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => Exit(__instance)));
            SetButtons(__instance, voteArea);
        }

        public void Exit(MeetingHud __instance)
        {
            Phone.Destroy();
            Utils.HUD.Chat.SetVisible(true);
            __instance.TimerText.gameObject.SetActive(true);
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));

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

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            if (RemainingKills <= 0)
                return;

            SetLists();

            foreach (var voteArea in __instance.playerStates)
                GenButton(voteArea, __instance);
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

                foreach (var pair in Buttons)
                {
                    if (pair.Value.Count > 0)
                    {
                        foreach (var item in pair.Value)
                            item?.gameObject?.SetActive(Page == pair.Key);
                    }

                    Buttons[Page].ForEach(x => x.GetComponent<SpriteRenderer>().color = x == SelectedButton ? UColor.red : UColor.white);
                }
            }
        }

        public void RpcMurderPlayer(PlayerControl player, string guess)
        {
            MurderPlayer(player, guess);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.AssassinKill);
            writer.Write(PlayerId);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void MurderPlayer(PlayerControl player, string guess)
        {
            var hudManager = Utils.HUD;

            if (player != Player && player.Is(ModifierEnum.Indomitable))
            {
                if (player == CustomPlayer.Local)
                    Utils.Flash(Colors.Indomitable);

                return;
            }

            if (Player.Is(ModifierEnum.Professional) && Player == player)
            {
                var modifier = Modifier.GetModifier<Professional>(Player);

                if (!modifier.LifeUsed)
                {
                    modifier.LifeUsed = true;
                    Utils.Flash(modifier.Color);
                    return;
                }
            }

            RemainingKills--;
            Utils.MarkMeetingDead(player, Player);

            if (AmongUsClient.Instance.AmHost && player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var otherLover = player.GetOtherLover();

                if (!otherLover.Is(RoleEnum.Pestilence))
                    RpcMurderPlayer(otherLover, guess);
            }

            if (Local)
            {
                if (Player != player)
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You guessed {player.name} as {guess}!");
                else if (Player.Is(ModifierEnum.Professional))
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess} and lost a life!");
                else
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess} and died!");
            }
            else if (Player != player && CustomPlayer.Local == player)
                hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} guessed you as {guess}!");
            else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) ||
                ConstantVariables.DeadSeeEverything)
            {
                if (Player != player)
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} guessed {player.name} as {guess}!");
                else if (Player.Is(ModifierEnum.Professional))
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} incorrectly guessed {player.name} as {guess} and lost a life!");
                else
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} incorrectly guessed {player.name} as {guess} and died!");
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

            if (!CustomGameOptions.AssassinateAfterVoting)
                HideButtons();
        }
    }
}