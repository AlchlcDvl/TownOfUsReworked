namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Retributionist : Crew
    {
        public Retributionist(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSupport;
            Inspected = new();
            BodyArrows = new();
            MediatedPlayers = new();
            Bugs = new();
            TrackerArrows = new();
            UntransportablePlayers = new();
            Reported = new();
            PlayerNumbers = new();
            ReferenceBodies = new();
            Selected = null;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            _time = DateTime.UnixEpoch;
            UsesLeft = CustomGameOptions.MaxUses;
            Player1Body = null;
            Player2Body = null;
            WasInVent1 = false;
            WasInVent2 = false;
            Vent1 = null;
            Vent2 = null;
            Transport1 = new("RetTransport1") { layer = 5 };
            Transport2 = new("RetTransport2") { layer = 5 };
            Transport1.AddSubmergedComponent("ElevatorMover");
            Transport2.AddSubmergedComponent("ElevatorMover");
            Transport1.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
            Transport1.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying1 = Transport1.AddComponent<SpriteRenderer>();
            AnimationPlaying2 = Transport2.AddComponent<SpriteRenderer>();
            AnimationPlaying1.sprite = PortalAnimation[0];
            AnimationPlaying2.sprite = PortalAnimation[0];
            AnimationPlaying1.material = HatManager.Instance.PlayerMaterial;
            AnimationPlaying2.material = HatManager.Instance.PlayerMaterial;
            Transport1.SetActive(true);
            Transport2.SetActive(true);
            TransportMenu1 = new(Player, Click1, Exception7);
            TransportMenu2 = new(Player, Click2, Exception8);
            RevealButton = new(this, "Reveal", AbilityTypes.Direct, "ActionSecondary", Reveal);
            StakeButton = new(this, "Stake", AbilityTypes.Direct, "ActionSecondary", Stake);
            AutopsyButton = new(this, "Autopsy", AbilityTypes.Dead, "ActionSecondary", Autopsy);
            CompareButton = new(this, "Compare", AbilityTypes.Direct, "Secondary", Compare);
            ExamineButton = new(this, "Examine", AbilityTypes.Direct, "ActionSecondary", Examine);
            InspectButton = new(this, "Inspect", AbilityTypes.Direct, "ActionSecondary", Inspect, Exception1);
            MediateButton = new(this, "Mediate", AbilityTypes.Effect, "ActionSecondary", Mediate);
            BugButton = new(this, "Bug", AbilityTypes.Effect, "ActionSecondary", PlaceBug, true);
            SeerButton = new(this, "Seer", AbilityTypes.Direct, "ActionSecondary", See);
            InterrogateButton = new(this, "Interrogate", AbilityTypes.Direct, "ActionSecondary", Interrogate, Exception2);
            TrackButton = new(this, "Track", AbilityTypes.Direct, "ActionSecondary", Track, Exception3, true);
            AlertButton = new(this, "Alert", AbilityTypes.Effect, "ActionSecondary", HitAlert, true);
            ShootButton = new(this, "Shoot", AbilityTypes.Direct, "ActionSecondary", Shoot, Exception4, true);
            ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", HitRevive, true);
            ShieldButton = new(this, "Shield", AbilityTypes.Direct, "ActionSecondary", Protect, Exception5);
            SwoopButton = new(this, "Swoop", AbilityTypes.Effect, "ActionSecondary", HitSwoop, true);
            FixButton = new(this, "Fix", AbilityTypes.Effect, "ActionSecondary", Fix, true);
            BlockButton = new(this, "EscortRoleblock", AbilityTypes.Direct, "ActionSecondary", Roleblock, Exception6);
            TransportButton = new(this, "Transport", AbilityTypes.Effect, "ActionSecondary", Transport, true);
            RetMenu = new(Player, "RetActive", "RetDisabled", MeetingTypes.Toggle, CustomGameOptions.ReviveAfterVoting, SetActive, IsExempt, GenNumbers);
        }

        //Retributionist Stuff
        public PlayerVoteArea Selected;
        public PlayerControl Revived;
        public Role RevivedRole;
        public int UsesLeft;
        public float TimeRemaining;
        public bool ButtonUsable => UsesLeft > 0;
        public bool OnEffect => TimeRemaining > 0;
        public bool Enabled;
        public CustomMeeting RetMenu;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Retributionist : Colors.Crew;
        public override string Name => "Retributionist";
        public override LayerEnum Type => LayerEnum.Retributionist;
        public override RoleEnum RoleType => RoleEnum.Retributionist;
        public override Func<string> StartText => () => "Mimic the Dead";
        public override Func<string> AbilitiesText => () => "- You can mimic the abilities of dead <color=#8CFFFFFF>Crew</color>";
        public override InspectorResults InspectorResults => RevivedRole == null ? InspectorResults.DealsWithDead : RevivedRole.InspectorResults;

        public override void OnLobby()
        {
            base.OnLobby();

            BodyArrows.Values.ToList().DestroyAll();
            BodyArrows.Clear();

            MediateArrows.Values.ToList().DestroyAll();
            MediateArrows.Clear();
            MediatedPlayers.Clear();

            TrackerArrows.Values.ToList().DestroyAll();
            TrackerArrows.Clear();

            Bug.Clear(Bugs);
            Bugs.Clear();

            AllPrints.ForEach(x => x.Destroy());
            AllPrints.Clear();
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            if (IsTrack)
            {
                TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
                TrackerArrows.Remove(targetPlayerId);
            }
            else if (IsCor)
            {
                BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
                BodyArrows.Remove(targetPlayerId);
            }
        }

        private bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = PlayerByVoteArea(voteArea);
            return !voteArea.AmDead || player.Data.Disconnected || IsDead;
        }

        public void GenNumbers()
        {
            foreach (var voteArea in Meeting.playerStates)
            {
                var targetId = voteArea.TargetPlayerId;
                var nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
                nameText.transform.localPosition = new(-1.211f, -0.18f, -0.1f);
                nameText.text = $"{GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId}";
                PlayerNumbers.Add(targetId, nameText);
            }
        }

        private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
        {
            if (__instance.state == MeetingHud.VoteStates.Discussion)
                return;

            if (Selected != null)
                RetMenu.Actives[Selected.TargetPlayerId] = false;

            Selected = voteArea;
            RetMenu.Actives[voteArea.TargetPlayerId] = true;
        }

        public bool Exception1(PlayerControl player) => Inspected.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
            (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
            (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
            (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

        public bool Exception2(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction !=
            SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
            CustomGameOptions.RivalsRoles) || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
            CustomGameOptions.LinkedRoles);

        public bool Exception3(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

        public bool Exception4(PlayerControl player) => player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

        public bool Exception5(PlayerControl player) => player == ShieldedPlayer;

        public bool Exception6(PlayerControl player) => player == BlockTarget;

        public bool Exception7(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UntransportablePlayers.ContainsKey(player.PlayerId) ||
            (BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer2 || player.IsMoving();

        public bool Exception8(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UntransportablePlayers.ContainsKey(player.PlayerId) ||
            (BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer1 || player.IsMoving();

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system?.dummy.IsActive;
            var active = system?.specials.Any(s => s.IsActive);
            var condition = active == true && dummyActive == false;
            var flag1 = TransportPlayer1 == null;
            var flag2 = TransportPlayer2 == null;
            TransportButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "TRANSPORT"), TransportTimer(), CustomGameOptions.TransportCooldown, UsesLeft, OnEffect,
                TimeRemaining, CustomGameOptions.TransportDuration, true, ButtonUsable && IsTrans);
            FixButton.Update("FIX", FixTimer(), CustomGameOptions.FixCooldown, UsesLeft, condition && ButtonUsable, ButtonUsable && IsEngi);
            ShieldButton.Update("SHIELD", !UsedMedicAbility, !UsedMedicAbility && IsMedic);
            RevealButton.Update("REVEAL", RevealTimer(), CustomGameOptions.RevealCooldown, true, IsMys);
            StakeButton.Update("STAKE", StakeTimer(), CustomGameOptions.StakeCooldown, true, IsVH);
            AutopsyButton.Update("AUTOPSY", AutopsyTimer(), CustomGameOptions.AutopsyCooldown, true, IsCor);
            CompareButton.Update("COMPARE", CompareTimer(), CustomGameOptions.CompareCooldown, true, ReferenceBodies.Count > 0 && IsCor);
            ExamineButton.Update("EXAMINE", ExamineTimer(), CustomGameOptions.ExamineCd, true, IsDet);
            InspectButton.Update("INSPECT", InspectTimer(), CustomGameOptions.InspectCooldown, true, IsInsp);
            MediateButton.Update("MEDIATE", MediateTimer(), CustomGameOptions.MediateCooldown, true, IsMed);
            BugButton.Update("BUG", BugTimer(), CustomGameOptions.BugCooldown, UsesLeft, ButtonUsable, IsOP && ButtonUsable);
            SeerButton.Update("SEE", SeerTimer(), CustomGameOptions.SeerCooldown, true, IsSeer);
            InterrogateButton.Update("INTERROGATE", InterrogateTimer(), CustomGameOptions.InterrogateCd, true, IsSher);
            AlertButton.Update("ALERT", AlertTimer(), CustomGameOptions.AlertCd, UsesLeft, OnEffect, TimeRemaining, CustomGameOptions.AlertDuration, true, IsVet && ButtonUsable);
            ShootButton.Update("SHOOT", KillTimer(), CustomGameOptions.VigiKillCd, UsesLeft, ButtonUsable, ButtonUsable && IsVig);
            ReviveButton.Update("REVIVE", ReviveTimer(), CustomGameOptions.ReviveCooldown, UsesLeft, ButtonUsable,ButtonUsable && IsAlt);
            SwoopButton.Update("SWOOP", SwoopTimer(), CustomGameOptions.SwoopCooldown, UsesLeft, OnEffect, TimeRemaining, CustomGameOptions.SwoopDuration, true, IsCham);
            BlockButton.Update("ROLEBLOCK", RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown, OnEffect, TimeRemaining, CustomGameOptions.EscRoleblockDuration, true, IsEsc);
            TrackButton.Update("TRACK", TrackerTimer(), CustomGameOptions.TrackCd, UsesLeft, ButtonUsable, ButtonUsable && IsTrack);

            if (!IsDead)
            {
                if (IsCor)
                {
                    var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow <
                        y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration)));

                    foreach (var bodyArrow in BodyArrows.Keys)
                    {
                        if (!validBodies.Any(x => x.ParentId == bodyArrow))
                            DestroyArrow(bodyArrow);
                    }

                    foreach (var body in validBodies)
                    {
                        if (!BodyArrows.ContainsKey(body.ParentId))
                            BodyArrows.Add(body.ParentId, new(Player, Color));

                        BodyArrows[body.ParentId]?.Update(body.TruePosition);
                    }
                }
                else if (IsMed)
                {
                    foreach (var player in CustomPlayer.AllPlayers)
                    {
                        if (MediateArrows.ContainsKey(player.PlayerId))
                        {
                            MediateArrows[player.PlayerId]?.Update(player.transform.position, player.GetPlayerColor(false, CustomGameOptions.ShowMediatePlayer));
                            player.Visible = true;

                            if (!CustomGameOptions.ShowMediatePlayer)
                            {
                                player.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(player));
                                PlayerMaterial.SetColors(UColor.grey, player.MyRend());
                            }
                        }
                    }
                }
                else if (IsTrack)
                {
                    foreach (var pair in TrackerArrows)
                    {
                        var player = PlayerById(pair.Key);
                        var body = BodyById(pair.Key);

                        if (player == null || player.Data.Disconnected || (player.Data.IsDead && !body))
                            DestroyArrow(pair.Key);
                        else
                            pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor());
                    }
                }
                else if (IsDet)
                {
                    Time2 += Time.deltaTime;

                    if (Time2 >= CustomGameOptions.FootprintInterval)
                    {
                        Time2 -= CustomGameOptions.FootprintInterval;

                        foreach (var player in CustomPlayer.AllPlayers)
                        {
                            if (player.Data.IsDead || player.Data.Disconnected || player == CustomPlayer.Local)
                                continue;

                            if (!AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == player.PlayerId))
                                AllPrints.Add(new(player));
                        }

                        for (var i = 0; i < AllPrints.Count; i++)
                        {
                            try
                            {
                                var footprint = AllPrints[i];

                                if (footprint.Update())
                                    i--;
                            } catch { /*Assume footprint value is null and allow the loop to continue*/ }
                        }
                    }
                }
            }
            else
                OnLobby();

            foreach (var entry in UntransportablePlayers)
            {
                var player = PlayerById(entry.Key);

                if (player == null)
                    continue;

                if (player.Data.IsDead || player.Data.Disconnected)
                    continue;

                if (UntransportablePlayers.ContainsKey(player.PlayerId) && player.moveable && UntransportablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                    UntransportablePlayers.Remove(player.PlayerId);
            }
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);
            RetMenu.Voted();

            if (Selected != null)
            {
                Revived = PlayerByVoteArea(Selected);
                RevivedRole = Revived == null ? null : (Revived.Is(RoleEnum.Revealer) ? GetRole<Revealer>(Revived).FormerRole : GetRole(Revived));
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.RetributionistRevive, this, Selected);
            }
        }

        public override void UpdateMeeting(MeetingHud __instance)
        {
            base.UpdateMeeting(__instance);
            RetMenu.Update();
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            RetMenu.HideButtons();

            if (Selected != null)
            {
                Revived = PlayerByVoteArea(Selected);
                RevivedRole = Revived == null ? null : (Revived.Is(RoleEnum.Revealer) ? GetRole<Revealer>(Revived).FormerRole : GetRole(Revived));
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.RetributionistRevive, this, Selected);
            }
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            RetMenu.GenButtons(__instance);
            Selected = null;

            if (IsOP)
            {
                if (BuggedPlayers.Count == 0)
                    HUD.Chat.AddChat(CustomPlayer.Local, "No one triggered your bugs.");
                else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                    HUD.Chat.AddChat(CustomPlayer.Local, "Not enough players triggered your bugs.");
                else if (BuggedPlayers.Count == 1)
                    HUD.Chat.AddChat(CustomPlayer.Local, $"A {BuggedPlayers[0]} triggered your bug.");
                else
                {
                    var message = "The following roles triggered your bug: ";
                    var position = 0;
                    BuggedPlayers.Shuffle();

                    foreach (var role in BuggedPlayers)
                    {
                        if (position < BuggedPlayers.Count - 1)
                            message += $" {role},";
                        else
                            message += $" and {role}.";

                        position++;
                    }

                    HUD.Chat.AddChat(CustomPlayer.Local, message);
                }
            }
            else if (IsDet)
                AllPrints.ForEach(x => x.Destroy());
        }

        //Coroner Stuff
        public Dictionary<byte, CustomArrow> BodyArrows = new();
        public CustomButton AutopsyButton;
        public CustomButton CompareButton;
        public DateTime LastAutopsied;
        public List<DeadPlayer> ReferenceBodies = new();
        public DateTime LastCompared;
        public List<byte> Reported = new();
        public bool IsCor => RevivedRole?.RoleType == RoleEnum.Coroner;

        public float CompareTimer()
        {
            var timespan = DateTime.UtcNow - LastCompared;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float AutopsyTimer()
        {
            var timespan = DateTime.UtcNow - LastAutopsied;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AutopsyCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Autopsy()
        {
            if (IsTooFar(Player, AutopsyButton.TargetBody) || AutopsyTimer() != 0f)
                return;

            var playerId = AutopsyButton.TargetBody.ParentId;
            var player = PlayerById(playerId);
            Spread(Player, player);
            var matches = KilledPlayers.Where(x => x.PlayerId == playerId).ToArray();
            DeadPlayer killed = null;

            if (matches.Length > 0)
                killed = matches[0];

            if (killed == null)
                Flash(new(255, 0, 0, 255));
            else
            {
                ReferenceBodies.Add(killed);
                LastAutopsied = DateTime.UtcNow;
                Flash(Color);
            }
        }

        public void Compare()
        {
            if (ReferenceBodies.Count == 0 || IsTooFar(Player, CompareButton.TargetPlayer) || CompareTimer() != 0f)
                return;

            var interact = Interact(Player, CompareButton.TargetPlayer);

            if (interact[3])
            {
                if (ReferenceBodies.Any(x => CompareButton.TargetPlayer.PlayerId == x.KillerId) || CompareButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastCompared = DateTime.UtcNow;
            else if (interact[1])
                LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void OnBodyReport(GameData.PlayerInfo info)
        {
            if (info == null || !Local || !IsCor)
                return;

            base.OnBodyReport(info);
            var body = KilledPlayers.Find(x => x.PlayerId == info.PlayerId);

            if (body == null)
                return;

            Reported.Add(info.PlayerId);
            body.Reporter = Player;
            body.KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds;
            var reportMsg = body.ParseBodyReport();

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            //Only Coroner can see this
            if (HUD)
                HUD.Chat.AddChat(CustomPlayer.Local, reportMsg);
        }

        //Detective Stuff
        public DateTime LastExamined;
        public CustomButton ExamineButton;
        public bool IsDet => RevivedRole?.RoleType == RoleEnum.Detective;
        private static float Time2;

        public float ExamineTimer()
        {
            var timespan = DateTime.UtcNow - LastExamined;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

        public void Examine()
        {
            if (ExamineTimer() != 0f || IsTooFar(Player, ExamineButton.TargetPlayer))
                return;

            var interact = Interact(Player, ExamineButton.TargetPlayer);

            if (interact[3])
            {
                var hasKilled = false;

                foreach (var player in KilledPlayers)
                {
                    if (player.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - player.KillTime).TotalSeconds <= CustomGameOptions.RecentKill)
                        hasKilled = true;
                }

                if (hasKilled || ExamineButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastExamined = DateTime.UtcNow;
            else if (interact[1])
                LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Inspector Stuff
        public DateTime LastInspected;
        public List<byte> Inspected = new();
        public CustomButton InspectButton;
        public bool IsInsp => RevivedRole?.RoleType == RoleEnum.Inspector;

        public float InspectTimer()
        {
            var timespan = DateTime.UtcNow - LastInspected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InspectCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Inspect()
        {
            if (InspectTimer() != 0f || IsTooFar(Player, InspectButton.TargetPlayer) || Inspected.Contains(InspectButton.TargetPlayer.PlayerId))
                return;

            var interact = Interact(Player, InspectButton.TargetPlayer);

            if (interact[3])
                Inspected.Add(InspectButton.TargetPlayer.PlayerId);

            if (interact[0])
                LastInspected = DateTime.UtcNow;
            else if (interact[1])
                LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Medium Stuff
        public DateTime LastMediated;
        public Dictionary<byte, CustomArrow> MediateArrows = new();
        public CustomButton MediateButton;
        public CustomButton SeanceButton;
        public List<byte> MediatedPlayers = new();
        public bool IsMed => RevivedRole?.RoleType == RoleEnum.Medium;

        public float MediateTimer()
        {
            var timespan = DateTime.UtcNow - LastMediated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Mediate()
        {
            if (MediateTimer() != 0f)
                return;

            LastMediated = DateTime.UtcNow;
            var PlayersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

            if (PlayersDead.Count == 0)
                return;

            if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
            {
                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                foreach (var dead in PlayersDead)
                {
                    if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                    {
                        MediateArrows.Add(dead.PlayerId, new(Player, Color));
                        MediatedPlayers.Add(dead.PlayerId);
                        CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Mediate, this, dead.PlayerId);

                        if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                            break;
                    }
                }
            }
            else
            {
                PlayersDead.Shuffle();
                var dead = PlayersDead.Random();

                if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, Color));
                    MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Mediate, this, dead.PlayerId);
                }
            }
        }

        //Operative Stuff
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public List<RoleEnum> BuggedPlayers = new();
        public CustomButton BugButton;
        public bool IsOP => RevivedRole?.RoleType == RoleEnum.Operative;
        public Dictionary<byte, TMP_Text> PlayerNumbers = new();

        public float BugTimer()
        {
            var timespan = DateTime.UtcNow - LastBugged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void PlaceBug()
        {
            if (BugTimer() != 0f || !ButtonUsable)
                return;

            UsesLeft--;
            LastBugged = DateTime.UtcNow;
            Bugs.Add(new(Player.GetTruePosition()));
        }

        //Sheriff Stuff
        public CustomButton InterrogateButton;
        public DateTime LastInterrogated;
        public bool IsSher => RevivedRole?.RoleType == RoleEnum.Sheriff;

        public float InterrogateTimer()
        {
            var timespan = DateTime.UtcNow - LastInterrogated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InterrogateCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Interrogate()
        {
            if (InterrogateTimer() != 0f || IsTooFar(Player, InterrogateButton.TargetPlayer))
                return;

            var interact = Interact(Player, InterrogateButton.TargetPlayer);

            if (interact[3])
            {
                if (InterrogateButton.TargetPlayer.SeemsEvil())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastInterrogated = DateTime.UtcNow;
            else if (interact[1])
                LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Tracker Stuff
        public Dictionary<byte, CustomArrow> TrackerArrows = new();
        public DateTime LastTracked;
        public CustomButton TrackButton;
        public bool IsTrack => RevivedRole?.RoleType == RoleEnum.Tracker;
        private static DateTime _time = DateTime.UnixEpoch;

        public float TrackerTimer()
        {
            var timespan = DateTime.UtcNow - LastTracked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Track()
        {
            if (IsTooFar(Player, TrackButton.TargetPlayer) || TrackerTimer() != 0f)
                return;

            var interact = Interact(Player, TrackButton.TargetPlayer);

            if (interact[3])
            {
                TrackerArrows.Add(TrackButton.TargetPlayer.PlayerId, new(Player, TrackButton.TargetPlayer.GetPlayerColor(), CustomGameOptions.UpdateInterval));
                UsesLeft--;
            }

            if (interact[0])
                LastTracked = DateTime.UtcNow;
            else if (interact[1])
                LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Vigilante Stuff
        public DateTime LastKilled;
        public CustomButton ShootButton;
        public bool IsVig => RevivedRole?.RoleType == RoleEnum.Vigilante;

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Shoot()
        {
            if (IsTooFar(Player, ShootButton.TargetPlayer) || KillTimer() != 0f)
                return;

            var interact = Interact(Player, ShootButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        //Vampire Hunter Stuff
        public DateTime LastStaked;
        public CustomButton StakeButton;
        public bool IsVH => RevivedRole?.RoleType == RoleEnum.VampireHunter;

        public float StakeTimer()
        {
            var timespan = DateTime.UtcNow - LastStaked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Stake()
        {
            if (IsTooFar(Player, StakeButton.TargetPlayer) || StakeTimer() != 0f)
                return;

            var interact = Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());

            if (interact[3] || interact[0])
                LastStaked = DateTime.UtcNow;
            else if (interact[1])
                LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStaked.AddSeconds(CustomGameOptions.VestKCReset);
        }

        //Veteran Stuff
        public DateTime LastAlerted;
        public CustomButton AlertButton;
        public bool IsVet => RevivedRole?.RoleType == RoleEnum.Veteran;

        public float AlertTimer()
        {
            var timespan = DateTime.UtcNow - LastAlerted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void HitAlert()
        {
            if (!ButtonUsable || AlertTimer() != 0f || OnEffect)
                return;

            TimeRemaining = CustomGameOptions.AlertDuration;
            UsesLeft--;
            Alert();
            CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Alert, this);
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        //Altruist Stuff
        public CustomButton ReviveButton;
        public bool IsAlt => RevivedRole?.RoleType == RoleEnum.Altruist;
        public bool Reviving;
        public DeadBody RevivingBody;
        public bool Success;
        public DateTime LastRevived;

        public float ReviveTimer()
        {
            var timespan = DateTime.UtcNow - LastRevived;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ReviveCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Revive()
        {
            if (!Reviving && CustomPlayer.Local.PlayerId == ReviveButton.TargetBody.ParentId)
            {
                Flash(Color);

                if (CustomGameOptions.AltruistTargetBody)
                    ReviveButton.TargetBody?.gameObject.Destroy();
            }

            Reviving = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || IsDead)
            {
                Success = false;
                TimeRemaining = 0f;
            }
        }

        public void UnRevive()
        {
            Reviving = false;
            LastRevived = DateTime.UtcNow;

            if (Success)
                FinishRevive();
        }

        private void FinishRevive()
        {
            var player = PlayerByBody(RevivingBody);
            var targetRole = GetRole(player);
            var formerKiller = targetRole.KilledBy;
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + PlayerName;
            player.Revive();
            UsesLeft--;

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + PlayerName;
                lover.Revive();
            }

            if (UsesLeft == 0)
                RpcMurderPlayer(Player, Player);

            if (formerKiller.Contains(CustomPlayer.LocalCustom.Data.PlayerName))
            {
                LocalRole.AllArrows.Add(player.PlayerId, new(CustomPlayer.Local, Color));
                Flash(Color);
            }
        }

        public void HitRevive()
        {
            if (IsTooFar(Player, ReviveButton.TargetBody) || ReviveTimer() != 0f || !ButtonUsable)
                return;

            RevivingBody = ReviveButton.TargetBody;
            Spread(Player, PlayerByBody(RevivingBody));
            CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.AltruistRevive, this, RevivingBody);
            TimeRemaining = CustomGameOptions.AltReviveDuration;
            Success = true;
            Revive();
        }

        //Medic Stuff
        public bool UsedMedicAbility => ShieldedPlayer != null || ExShielded != null;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public CustomButton ShieldButton;
        public bool IsMedic => RevivedRole?.RoleType == RoleEnum.Medic;

        public void Protect()
        {
            if (IsTooFar(Player, ShieldButton.TargetPlayer))
                return;

            var interact = Interact(Player, ShieldButton.TargetPlayer);

            if (interact[3])
            {
                ShieldedPlayer = ShieldButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Protect, this, ShieldedPlayer);
            }
        }

        //Chameleon Stuff
        public DateTime LastSwooped;
        public CustomButton SwoopButton;
        public bool IsCham => RevivedRole?.RoleType == RoleEnum.Chameleon;

        public float SwoopTimer()
        {
            var timespan = DateTime.UtcNow - LastSwooped;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InvisCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void HitSwoop()
        {
            if (SwoopTimer() != 0f || OnEffect || !ButtonUsable)
                return;

            TimeRemaining = CustomGameOptions.SwoopDuration;
            Invis();
            UsesLeft--;
            CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Swoop, this);
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Invis(Player);

            if (Meeting || IsDead)
                TimeRemaining = 0f;
        }

        public void Uninvis()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            DefaultOutfit(Player);
        }

        //Engineer Stuff
        public CustomButton FixButton;
        public DateTime LastFixed;
        public bool IsEngi => RevivedRole?.RoleType == RoleEnum.Engineer;

        public float FixTimer()
        {
            var timespan = DateTime.UtcNow - LastFixed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FixCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Fix()
        {
            if (!ButtonUsable || FixTimer() != 0f)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.Any(s => s.IsActive);

            if (!sabActive || dummyActive)
                return;

            UsesLeft--;
            LastFixed = DateTime.UtcNow;
            FixExtentions.Fix();
        }

        //Mystic Stuff
        public DateTime LastRevealed;
        public CustomButton RevealButton;
        public bool IsMys => RevivedRole?.RoleType == RoleEnum.Mystic;

        public float RevealTimer()
        {
            var timespan = DateTime.UtcNow - LastRevealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Reveal()
        {
            if (RevealTimer() != 0f || IsTooFar(Player, RevealButton.TargetPlayer))
                return;

            var interact = Interact(Player, RevealButton.TargetPlayer);

            if (interact[3])
            {
                if ((!RevealButton.TargetPlayer.Is(SubFaction.None) && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) || RevealButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastRevealed = DateTime.UtcNow;
            else if (interact[1])
                LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Seer Stuff
        public DateTime LastSeered;
        public CustomButton SeerButton;
        public bool IsSeer => RevivedRole?.RoleType == RoleEnum.Seer;

        public float SeerTimer()
        {
            var timespan = DateTime.UtcNow - LastSeered;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void See()
        {
            if (SeerTimer() != 0f || IsTooFar(Player, SeerButton.TargetPlayer))
                return;

            var interact = Interact(Player, SeerButton.TargetPlayer);

            if (interact[3])
            {
                if (GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastSeered = DateTime.UtcNow;
            else if (interact[1])
                LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Escort Stuff
        public PlayerControl BlockTarget;
        public DateTime LastBlock;
        public CustomButton BlockButton;
        public bool IsEsc => RevivedRole?.RoleType == RoleEnum.Escort;

        public void UnBlock()
        {
            Enabled = false;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune;

            if (Meeting || IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var timespan = DateTime.UtcNow - LastBlock;
            var num = Player.GetModifiedCooldown(CustomGameOptions.EscRoleblockCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Roleblock()
        {
            if (RoleblockTimer() != 0f || IsTooFar(Player, BlockButton.TargetPlayer))
                return;

            var interact = Interact(Player, BlockButton.TargetPlayer);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                BlockTarget = BlockButton.TargetPlayer;
                Block();
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.EscRoleblock, this, BlockTarget);
            }
            else if (interact[0])
                LastBlock = DateTime.UtcNow;
            else if (interact[1])
                LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Transporter Stuff
        public DateTime LastTransported;
        public PlayerControl TransportPlayer1;
        public PlayerControl TransportPlayer2;
        public Dictionary<byte, DateTime> UntransportablePlayers = new();
        public CustomButton TransportButton;
        public CustomMenu TransportMenu1;
        public CustomMenu TransportMenu2;
        public SpriteRenderer AnimationPlaying1;
        public SpriteRenderer AnimationPlaying2;
        public GameObject Transport1;
        public GameObject Transport2;
        public DeadBody Player1Body;
        public DeadBody Player2Body;
        public bool WasInVent1;
        public bool WasInVent2;
        public Vent Vent1;
        public Vent Vent2;
        public bool IsTrans => RevivedRole?.RoleType == RoleEnum.Transporter;

        public float TransportTimer()
        {
            var timespan = DateTime.UtcNow - LastTransported;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TransportCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public IEnumerator TransportPlayers()
        {
            Player1Body = null;
            Player2Body = null;
            WasInVent1 = false;
            WasInVent2 = false;
            Vent1 = null;
            Vent2 = null;
            TimeRemaining = CustomGameOptions.TransportDuration;

            if (TransportPlayer1.Data.IsDead)
            {
                Player1Body = BodyById(TransportPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (TransportPlayer2.Data.IsDead)
            {
                Player2Body = BodyById(TransportPlayer2.PlayerId);

                if (Player2Body == null)
                    yield break;
            }

            if (TransportPlayer1.inVent)
            {
                while (GetInTransition())
                    yield return null;

                TransportPlayer1.MyPhysics.ExitAllVents();
                Vent1 = TransportPlayer1.GetClosestVent();
                WasInVent1 = true;
            }

            if (TransportPlayer2.inVent)
            {
                while (GetInTransition())
                    yield return null;

                TransportPlayer2.MyPhysics.ExitAllVents();
                Vent2 = TransportPlayer2.GetClosestVent();
                WasInVent2 = true;
            }

            TransportPlayer1.moveable = false;
            TransportPlayer2.moveable = false;
            TransportPlayer1.NetTransform.Halt();
            TransportPlayer2.NetTransform.Halt();

            if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
                Flash(Color, CustomGameOptions.TransportDuration);

            if (Player1Body == null && !WasInVent1)
                AnimateTransport1();

            if (Player2Body == null && !WasInVent2)
                AnimateTransport2();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.TransportDuration)
                {
                    TimeRemaining -= Time.deltaTime;
                    yield return null;
                }
                else
                    break;

                if (Meeting)
                    yield break;
            }

            if (Player1Body == null && Player2Body == null)
            {
                TransportPlayer1.MyPhysics.ResetMoveState();
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
                TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

                if (IsSubmerged)
                {
                    if (CustomPlayer.Local == TransportPlayer1)
                    {
                        ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                        CheckOutOfBoundsElevator(CustomPlayer.Local);
                    }

                    if (CustomPlayer.Local == TransportPlayer2)
                    {
                        ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                        CheckOutOfBoundsElevator(CustomPlayer.Local);
                    }
                }

                if (TransportPlayer1.CanVent() && Vent2 != null && WasInVent2)
                    TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

                if (TransportPlayer2.CanVent() && Vent1 != null && WasInVent1)
                    TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TransportPlayer2.GetTruePosition();
                TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

                if (IsSubmerged && CustomPlayer.Local == TransportPlayer2)
                {
                    ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                    CheckOutOfBoundsElevator(CustomPlayer.Local);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TransportPlayer1.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (IsSubmerged && CustomPlayer.Local == TransportPlayer1)
                {
                    ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                    CheckOutOfBoundsElevator(CustomPlayer.Local);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
            }

            if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TransportPlayer1.moveable = true;
            TransportPlayer2.moveable = true;
            TransportPlayer1.Collider.enabled = true;
            TransportPlayer2.Collider.enabled = true;
            TransportPlayer1.NetTransform.enabled = true;
            TransportPlayer2.NetTransform.enabled = true;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            TimeRemaining = 0; //Insurance
            LastTransported = DateTime.UtcNow;
        }

        public void AnimateTransport1()
        {
            Transport1.transform.position = new(TransportPlayer1.GetTruePosition().x, TransportPlayer1.GetTruePosition().y + 0.35f, (TransportPlayer1.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying1.flipX = TransportPlayer1.MyRend().flipX;
            AnimationPlaying1.transform.localScale *= 0.9f * TransportPlayer1.GetModifiedSize();

            HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDuration, new Action<float>(p =>
            {
                var index = (int)(p * PortalAnimation.Length);
                index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
                AnimationPlaying1.sprite = PortalAnimation[index];
                TransportPlayer1.SetPlayerMaterialColors(AnimationPlaying1);

                if (p == 1)
                    AnimationPlaying1.sprite = null;
            })));
        }

        public void AnimateTransport2()
        {
            Transport2.transform.position = new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.35f, (TransportPlayer2.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying2.flipX = TransportPlayer2.MyRend().flipX;
            AnimationPlaying2.transform.localScale *= 0.9f * TransportPlayer2.GetModifiedSize();

            HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDuration, new Action<float>(p =>
            {
                var index = (int)(p * PortalAnimation.Length);
                index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
                AnimationPlaying2.sprite = PortalAnimation[index];
                TransportPlayer2.SetPlayerMaterialColors(AnimationPlaying2);

                if (p == 1)
                    AnimationPlaying2.sprite = null;
            })));
        }

        public void Click1(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3])
                TransportPlayer1 = player;
            else if (interact[0])
                LastTransported = DateTime.UtcNow;
            else if (interact[1])
                LastTransported.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3])
                TransportPlayer2 = player;
            else if (interact[0])
                LastTransported = DateTime.UtcNow;
            else if (interact[1])
                LastTransported.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Transport()
        {
            if (TransportTimer() != 0f)
                return;

            if (TransportPlayer1 == null)
                TransportMenu1.Open();
            else if (TransportPlayer2 == null)
                TransportMenu2.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Transport, this, TransportPlayer1, TransportPlayer2);
                Coroutines.Start(TransportPlayers());
                UsesLeft--;
            }
        }
    }
}