namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Thief : NeutralRole
    {
        public DateTime LastStolen;
        public CustomButton StealButton;
        public Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping = new();
        public GameObject Phone;
        public Dictionary<byte, GameObject> OtherButtons = new();
        public Transform SelectedButton;
        public int Page;
        public int MaxPage;
        public Dictionary<int, List<Transform>> GuessButtons = new();
        public Dictionary<int, KeyValuePair<string, Color>> Sorted = new();

        public Thief(PlayerControl player) : base(player)
        {
            Name = "Thief";
            StartText = () => "Steal From The Killers";
            AbilitiesText = () => "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
            RoleType = RoleEnum.Thief;
            RoleAlignment = RoleAlignment.NeutralBen;
            Type = LayerEnum.Thief;
            StealButton = new(this, "Steal", AbilityTypes.Direct, "ActionSecondary", Steal);
            InspectorResults = InspectorResults.BringsChaos;
            SetLists();

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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
                if (CustomGameOptions.VeteranOn > 0)
                    ColorMapping.Add("Veteran", Colors.Veteran);

                if (CustomGameOptions.VigilanteOn > 0)
                    ColorMapping.Add("Vigilante", Colors.Vigilante);

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

                    if (CustomGameOptions.SilencerOn > 0)
                        ColorMapping.Add("Silencer", Colors.Silencer);

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
                    ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);

                if (CustomGameOptions.ThiefOn > 0)
                    ColorMapping.Add("Thief", Colors.Thief);

                if (CustomGameOptions.BountyHunterOn > 0)
                    ColorMapping.Add("Bounty Hunter", Colors.BountyHunter);
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

        public float StealTimer()
        {
            var timespan = DateTime.UtcNow - LastStolen;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ThiefKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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

                        if (__instance.state == MeetingHud.VoteStates.Discussion || focusedTarget == null)
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
                        RpcMurderPlayer(toDie, currentGuess);
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
            var player = Utils.PlayerByVoteArea(voteArea);
            return player.Data.IsDead || player.Data.Disconnected || (voteArea.NameText.text.Contains('\n') && Player.GetFaction() != player.GetFaction()) || (player == Player &&
                player == PlayerControl.LocalPlayer) || Player.GetFaction() == player.GetFaction() || player == Player.GetOtherLover() || player == Player.GetOtherRival() || IsDead;
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

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            SetLists();

            foreach (var voteArea in __instance.playerStates)
                GenButton(voteArea, __instance);
        }

        public void Steal()
        {
            if (Utils.IsTooFar(Player, StealButton.TargetPlayer) || StealTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StealButton.TargetPlayer, true);

            if (interact[3])
            {
                if (!(StealButton.TargetPlayer.Is(Faction.Intruder) || StealButton.TargetPlayer.Is(Faction.Syndicate) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralKill) ||
                    StealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralPros) || StealButton.TargetPlayer.Is(RoleAlignment.CrewKill)))
                    Utils.RpcMurderPlayer(Player, Player);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Steal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(StealButton.TargetPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.RpcMurderPlayer(Player, StealButton.TargetPlayer);
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

        public bool Exception(PlayerControl player) => player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player == Player.GetOtherLover() || player ==
            Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = GetRole(other);
            var thief = thiefRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();
            thief.DisableButtons();
            other.DisableButtons();

            if (PlayerControl.LocalPlayer == other || PlayerControl.LocalPlayer == thief)
            {
                Utils.Flash(thiefRole.Color);
                role.OnLobby();
                thiefRole.OnLobby();
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Anarchist => new Anarchist(thief),
                RoleEnum.Arsonist => new Arsonist(thief) { Doused = ((Arsonist)role).Doused },
                RoleEnum.Blackmailer => new Blackmailer(thief) { BlackmailedPlayer = ((Blackmailer)role).BlackmailedPlayer },
                RoleEnum.Bomber => new Bomber(thief),
                RoleEnum.Camouflager => new Camouflager(thief),
                RoleEnum.Concealer => new Concealer(thief),
                RoleEnum.Consigliere => new Consigliere(thief) { Investigated = ((Consigliere)role).Investigated },
                RoleEnum.Consort => new Consort(thief),
                RoleEnum.Cryomaniac => new Cryomaniac(thief) { Doused = ((Cryomaniac)role).Doused },
                RoleEnum.Disguiser => new Disguiser(thief),
                RoleEnum.Dracula => new Dracula(thief) { Converted = ((Dracula)role).Converted },
                RoleEnum.Framer => new Framer(thief) { Framed = ((Framer)role).Framed },
                RoleEnum.Glitch => new Glitch(thief),
                RoleEnum.Enforcer => new Enforcer(thief),
                RoleEnum.Godfather => new Godfather(thief),
                RoleEnum.Grenadier => new Grenadier(thief),
                RoleEnum.Impostor => new Impostor(thief),
                RoleEnum.Juggernaut => new Juggernaut(thief) { JuggKills = ((Juggernaut)role).JuggKills },
                RoleEnum.Mafioso => new Mafioso(thief) { Godfather = (Godfather)leader },
                RoleEnum.PromotedGodfather => new PromotedGodfather(thief)
                {
                    Investigated = ((PromotedGodfather)role).Investigated,
                    BlackmailedPlayer = ((PromotedGodfather)role).BlackmailedPlayer
                },
                RoleEnum.Miner => new Miner(thief),
                RoleEnum.Morphling => new Morphling(thief),
                RoleEnum.Rebel => new Rebel(thief),
                RoleEnum.Sidekick => new Sidekick(thief) { Rebel = (Rebel)leader },
                RoleEnum.Shapeshifter => new Shapeshifter(thief),
                RoleEnum.Murderer => new Murderer(thief),
                RoleEnum.Plaguebearer => new Plaguebearer(thief) { Infected = ((Plaguebearer)role).Infected },
                RoleEnum.Pestilence => new Pestilence(thief),
                RoleEnum.SerialKiller => new SerialKiller(thief),
                RoleEnum.Werewolf => new Werewolf(thief),
                RoleEnum.Janitor => new Janitor(thief),
                RoleEnum.Poisoner => new Poisoner(thief),
                RoleEnum.Teleporter => new Teleporter(thief),
                RoleEnum.VampireHunter => new VampireHunter(thief),
                RoleEnum.Veteran => new Veteran(thief) { UsesLeft = ((Veteran)role).UsesLeft },
                RoleEnum.Vigilante => new Vigilante(thief) { UsesLeft = ((Vigilante)role).UsesLeft },
                RoleEnum.Warper => new Warper(thief),
                RoleEnum.Wraith => new Wraith(thief),
                RoleEnum.BountyHunter => new BountyHunter(thief) { TargetPlayer = target },
                RoleEnum.Jackal => new Jackal(thief)
                {
                    Recruited = ((Jackal)role).Recruited,
                    EvilRecruit = ((Jackal)role).EvilRecruit,
                    GoodRecruit = ((Jackal)role).GoodRecruit,
                    BackupRecruit = ((Jackal)role).BackupRecruit
                },
                RoleEnum.Necromancer => new Necromancer(thief)
                {
                    Resurrected = ((Necromancer)role).Resurrected,
                    KillCount = ((Necromancer)role).KillCount,
                    ResurrectedCount = ((Necromancer)role).ResurrectedCount
                },
                RoleEnum.Whisperer => new Whisperer(thief)
                {
                    Persuaded = ((Whisperer)role).Persuaded,
                    WhisperCount = ((Whisperer)role).WhisperCount,
                    WhisperConversion = ((Whisperer)role).WhisperConversion
                },
                RoleEnum.Betrayer => new Betrayer(thief) { Faction = role.Faction },
                RoleEnum.Ambusher => new Ambusher(thief),
                RoleEnum.Crusader => new Crusader(thief),
                RoleEnum.PromotedRebel => new PromotedRebel(thief) { Framed = ((PromotedRebel)role).Framed },
                RoleEnum.Stalker => new Stalker(thief) { StalkerArrows = ((Stalker)role).StalkerArrows },
                _ => new Thief(thief),
            };

            newRole.RoleUpdate(thiefRole);

            if (other.Is(RoleEnum.Dracula))
                ((Dracula)role).Converted.Clear();
            else if (other.Is(RoleEnum.Whisperer))
                ((Whisperer)role).Persuaded.Clear();
            else if (other.Is(RoleEnum.Necromancer))
                ((Necromancer)role).Resurrected.Clear();
            else if (other.Is(RoleEnum.Jackal))
            {
                ((Jackal)role).Recruited.Clear();
                ((Jackal)role).EvilRecruit = null;
                ((Jackal)role).GoodRecruit = null;
                ((Jackal)role).BackupRecruit = null;
            }

            thief.Data.SetImpostor(thief.Is(Faction.Intruder) || (thief.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

            if (CustomGameOptions.ThiefSteals)
            {
                if (PlayerControl.LocalPlayer == other && other.Is(Faction.Intruder))
                {
                    HudManager.Instance.SabotageButton.gameObject.SetActive(false);
                    other.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                }

                var newRole2 = new Thief(other);
                newRole2.RoleUpdate(role);
            }

            if (thief.Is(Faction.Intruder) || thief.Is(Faction.Syndicate) || (thief.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
            {
                foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && PlayerControl.LocalPlayer == thief)
                        LocalRole.AllArrows.Add(snitch.PlayerId, new(thief, Colors.Snitch, 0));
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == snitch.Player)
                        GetRole(snitch.Player).AllArrows.Add(thief.PlayerId, new(snitch.Player, Colors.Snitch));
                }

                foreach (var revealer in GetRoles<Revealer>(RoleEnum.Revealer))
                {
                    if (revealer.Revealed && PlayerControl.LocalPlayer == thief)
                        LocalRole.AllArrows.Add(revealer.PlayerId, new(thief, Colors.Revealer, 0));
                }
            }

            thief.EnableButtons();
            other.EnableButtons();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StealButton.Update("STEAL", StealTimer(), CustomGameOptions.ThiefKillCooldown);
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
        }

        public void RpcMurderPlayer(PlayerControl player, string guess)
        {
            MurderPlayer(player, guess);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.ThiefKill);
            writer.Write(PlayerId);
            writer.Write(player.PlayerId);
            writer.Write(guess);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void MurderPlayer(PlayerControl player, string guess)
        {
            var hudManager = HudManager.Instance;

            if (player != Player && player.Is(ModifierEnum.Indomitable))
            {
                if (player == PlayerControl.LocalPlayer)
                    Utils.Flash(Colors.Indomitable);

                return;
            }

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
                else
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"You incorrectly guessed {player.name} as {guess} and died!");
            }
            else if (Player != player && PlayerControl.LocalPlayer == player)
                hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} guessed you as {guess}!");
            else if ((Player.GetFaction() == PlayerControl.LocalPlayer.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) ||
                ConstantVariables.DeadSeeEverything)
            {
                if (Player != player)
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} guessed {player.name} as {player}!");
                else
                    hudManager.Chat.AddChat(PlayerControl.LocalPlayer, $"{Player.name} incorrectly guessed {player.name} as {player} and died!");
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