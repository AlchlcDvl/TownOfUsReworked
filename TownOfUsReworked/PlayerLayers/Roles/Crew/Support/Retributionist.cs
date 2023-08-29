namespace TownOfUsReworked.PlayerLayers.Roles;

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
    public PlayerVoteArea Selected { get; set; }
    public PlayerControl Revived { get; set; }
    public Role RevivedRole { get; set; }
    public int UsesLeft { get; set; }
    public float TimeRemaining { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public bool OnEffect => TimeRemaining > 0;
    public bool Enabled { get; set; }
    public CustomMeeting RetMenu { get; set; }

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Retributionist : Colors.Crew;
    public override string Name => "Retributionist";
    public override LayerEnum Type => LayerEnum.Retributionist;
    public override Func<string> StartText => () => "Mimic the Dead";
    public override Func<string> Description => () => "- You can mimic the abilities of dead <color=#8CFFFFFF>Crew</color>" + (RevivedRole == null ? "" : $"\n{RevivedRole.Description()}");
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
        if (!DataManager.Settings.Accessibility.ColorBlindMode)
            return;

        foreach (var voteArea in AllVoteAreas)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new(-1.211f, -0.18f, -0.1f);
            nameText.text = $"{targetId}";
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
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public bool Exception2(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
        CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
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
        TransportButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "TRANSPORT"), TransportTimer, CustomGameOptions.TransportCd, UsesLeft, OnEffect, TimeRemaining,
            CustomGameOptions.TransportDur, true, ButtonUsable && IsTrans);
        FixButton.Update("FIX", FixTimer, CustomGameOptions.FixCd, UsesLeft, condition && ButtonUsable, ButtonUsable && IsEngi);
        ShieldButton.Update("SHIELD", !UsedMedicAbility, !UsedMedicAbility && IsMedic);
        RevealButton.Update("REVEAL", RevealTimer, CustomGameOptions.MysticRevealCd, true, IsMys);
        StakeButton.Update("STAKE", StakeTimer, CustomGameOptions.StakeCd, true, IsVH);
        AutopsyButton.Update("AUTOPSY", AutopsyTimer, CustomGameOptions.AutopsyCd, true, IsCor);
        CompareButton.Update("COMPARE", CompareTimer, CustomGameOptions.CompareCd, true, ReferenceBodies.Count > 0 && IsCor);
        ExamineButton.Update("EXAMINE", ExamineTimer, CustomGameOptions.ExamineCd, true, IsDet);
        InspectButton.Update("INSPECT", InspectTimer, CustomGameOptions.InspectCd, true, IsInsp);
        MediateButton.Update("MEDIATE", MediateTimer, CustomGameOptions.MediateCd, true, IsMed);
        BugButton.Update("BUG", BugTimer, CustomGameOptions.BugCd, UsesLeft, ButtonUsable, IsOP && ButtonUsable);
        SeerButton.Update("SEE", SeerTimer, CustomGameOptions.SeerCd, true, IsSeer);
        InterrogateButton.Update("INTERROGATE", InterrogateTimer, CustomGameOptions.InterrogateCd, true, IsSher);
        AlertButton.Update("ALERT", AlertTimer, CustomGameOptions.AlertCd, UsesLeft, OnEffect, TimeRemaining, CustomGameOptions.AlertDur, true, IsVet && ButtonUsable);
        ShootButton.Update("SHOOT", KillTimer, CustomGameOptions.ShootCd, UsesLeft, ButtonUsable, ButtonUsable && IsVig);
        ReviveButton.Update("REVIVE", ReviveTimer, CustomGameOptions.ReviveCd, UsesLeft, ButtonUsable,ButtonUsable && IsAlt);
        SwoopButton.Update("SWOOP", SwoopTimer, CustomGameOptions.SwoopCd, UsesLeft, OnEffect, TimeRemaining, CustomGameOptions.SwoopDur, true, IsCham);
        BlockButton.Update("ROLEBLOCK", BlockTimer, CustomGameOptions.EscortCd, OnEffect, TimeRemaining, CustomGameOptions.EscortDur, true, IsEsc);
        TrackButton.Update("TRACK", TrackTimer, CustomGameOptions.TrackCd, UsesLeft, ButtonUsable, ButtonUsable && IsTrack);

        if (!IsDead)
        {
            if (IsCor)
            {
                var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow <
                    y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDur)));

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
                _time += Time.deltaTime;

                if (_time >= CustomGameOptions.FootprintInterval)
                {
                    _time -= CustomGameOptions.FootprintInterval;

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
            RevivedRole = Revived == null ? null : (Revived.Is(LayerEnum.Revealer) ? GetRole<Revealer>(Revived).FormerRole : GetRole(Revived));
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
            RevivedRole = Revived == null ? null : (Revived.Is(LayerEnum.Revealer) ? GetRole<Revealer>(Revived).FormerRole : GetRole(Revived));
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
            var message = "";

            if (BuggedPlayers.Count == 0)
                message = "No one triggered your bugs.";
            else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                message = "Not enough players triggered your bugs.";
            else if (BuggedPlayers.Count == 1)
            {
                var result = BuggedPlayers[0];
                var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Inspector or LayerEnum.Operative or LayerEnum.Actor or LayerEnum.Amnesiac or
                    LayerEnum.Arsonist or LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
                message = $"A{a_an} {result} triggered your bug.";
            }
            else if (CustomGameOptions.PreciseOperativeInfo)
            {
                message = "Your bugs returned the following results:";
                Bugs.ForEach(bug => message += $"\n{bug.GetResults()}");
            }
            else
            {
                message = "The following roles triggered your bugs: ";
                BuggedPlayers.Shuffle();
                BuggedPlayers.ForEach(role => message += $"{GetRoles(role)[0]}, ");
                message = message.Remove(message.Length - 2);
            }

            if (HUD)
                Run(HUD.Chat, "<color=#8D0F8CFF>〖 Bug Results 〗</color>", message);
        }
        else if (IsDet)
        {
            AllPrints.ForEach(x => x.Destroy());
            AllPrints.Clear();
        }
    }

    //Coroner Stuff
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton AutopsyButton { get; set; }
    public CustomButton CompareButton { get; set; }
    public DateTime LastAutopsied { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; set; }
    public DateTime LastCompared { get; set; }
    public List<byte> Reported { get; set; }
    public bool IsCor => RevivedRole?.Type == LayerEnum.Coroner;
    public float AutopsyTimer => ButtonUtils.Timer(Player, LastAutopsied, CustomGameOptions.AutopsyCd);
    public float CompareTimer => ButtonUtils.Timer(Player, LastCompared, CustomGameOptions.CompareCd);

    public void Autopsy()
    {
        if (IsTooFar(Player, AutopsyButton.TargetBody) || AutopsyTimer != 0f)
            return;

        var playerId = AutopsyButton.TargetBody.ParentId;
        var player = PlayerById(playerId);
        Spread(Player, player);
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == playerId));
        LastAutopsied = DateTime.UtcNow;
    }

    public void Compare()
    {
        if (ReferenceBodies.Count == 0 || IsTooFar(Player, CompareButton.TargetPlayer) || CompareTimer != 0f)
            return;

        var interact = Interact(Player, CompareButton.TargetPlayer);

        if (interact[3])
            Flash(ReferenceBodies.Any(x => CompareButton.TargetPlayer.PlayerId == x.KillerId) ? UColor.red : UColor.green);

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

        //Only Retributionist-Coroner can see this
        if (HUD)
            Run(HUD.Chat, "<color=#8D0F8CFF>〖 Autopsy Results 〗</color>", reportMsg);
    }

    //Detective Stuff
    public DateTime LastExamined { get; set; }
    public CustomButton ExamineButton { get; set; }
    public bool IsDet => RevivedRole?.Type == LayerEnum.Detective;
    public float ExamineTimer => ButtonUtils.Timer(Player, LastExamined, CustomGameOptions.ExamineCd);
    private static float _time;

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        if (ExamineTimer != 0f || IsTooFar(Player, ExamineButton.TargetPlayer))
            return;

        var interact = Interact(Player, ExamineButton.TargetPlayer);

        if (interact[3])
        {
            Flash(ExamineButton.TargetPlayer.IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                CustomGameOptions.RecentKill) ? UColor.red : UColor.green);
        }

        if (interact[0])
            LastExamined = DateTime.UtcNow;
        else if (interact[1])
            LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Inspector Stuff
    public DateTime LastInspected { get; set; }
    public List<byte> Inspected { get; set; }
    public CustomButton InspectButton { get; set; }
    public float InspectTimer => ButtonUtils.Timer(Player, LastInspected, CustomGameOptions.InspectCd);
    public bool IsInsp => RevivedRole?.Type == LayerEnum.Inspector;

    public void Inspect()
    {
        if (InspectTimer != 0f || IsTooFar(Player, InspectButton.TargetPlayer) || Inspected.Contains(InspectButton.TargetPlayer.PlayerId))
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
    public DateTime LastMediated { get; set; }
    public Dictionary<byte, CustomArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    //public CustomButton SeanceButton { get; set; }
    public List<byte> MediatedPlayers { get; set; }
    public float MediateTimer => ButtonUtils.Timer(Player, LastMediated, CustomGameOptions.MediateCd);
    public bool IsMed => RevivedRole?.Type == LayerEnum.Medium;

    public void Mediate()
    {
        if (MediateTimer != 0f)
            return;

        LastMediated = DateTime.UtcNow;
        var playersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

        if (playersDead.Count == 0)
            return;

        if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
        {
            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                playersDead.Reverse();

            foreach (var dead in playersDead)
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
            playersDead.Shuffle();
            var dead = playersDead.Random();

            if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, Color));
                MediatedPlayers.Add(dead.PlayerId);
                CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Mediate, this, dead.PlayerId);
            }
        }
    }

    //Operative Stuff
    public List<Bug> Bugs { get; set; }
    public DateTime LastBugged { get; set; }
    public List<LayerEnum> BuggedPlayers { get; set; }
    public CustomButton BugButton { get; set; }
    public Dictionary<byte, TMP_Text> PlayerNumbers { get; set; }
    public bool IsOP => RevivedRole?.Type == LayerEnum.Operative;
    public float BugTimer => ButtonUtils.Timer(Player, LastBugged, CustomGameOptions.BugCd);

    public void PlaceBug()
    {
        if (BugTimer != 0f || !ButtonUsable)
            return;

        UsesLeft--;
        LastBugged = DateTime.UtcNow;
        Bugs.Add(new(Player));
    }

    //Sheriff Stuff
    public CustomButton InterrogateButton { get; set; }
    public DateTime LastInterrogated { get; set; }
    public bool IsSher => RevivedRole?.Type == LayerEnum.Sheriff;
    public float InterrogateTimer => ButtonUtils.Timer(Player, LastInterrogated, CustomGameOptions.InterrogateCd);

    public void Interrogate()
    {
        if (InterrogateTimer != 0f || IsTooFar(Player, InterrogateButton.TargetPlayer))
            return;

        var interact = Interact(Player, InterrogateButton.TargetPlayer);

        if (interact[3])
            Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

        if (interact[0])
            LastInterrogated = DateTime.UtcNow;
        else if (interact[1])
            LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Tracker Stuff
    public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
    public DateTime LastTracked { get; set; }
    public CustomButton TrackButton { get; set; }
    public bool IsTrack => RevivedRole?.Type == LayerEnum.Tracker;
    public float TrackTimer => ButtonUtils.Timer(Player, LastTracked, CustomGameOptions.TrackCd);

    public void Track()
    {
        if (IsTooFar(Player, TrackButton.TargetPlayer) || TrackTimer != 0f)
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
    public DateTime LastKilled { get; set; }
    public CustomButton ShootButton { get; set; }
    public bool IsVig => RevivedRole?.Type == LayerEnum.Vigilante;
    public float KillTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.ShootCd);

    public void Shoot()
    {
        if (IsTooFar(Player, ShootButton.TargetPlayer) || KillTimer != 0f)
            return;

        var interact = Interact(Player, ShootButton.TargetPlayer, true);

        if (interact[3] || interact[0])
        {
            LastKilled = DateTime.UtcNow;
            UsesLeft--;
        }
        else if (interact[1])
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    //Vampire Hunter Stuff
    public DateTime LastStaked { get; set; }
    public CustomButton StakeButton { get; set; }
    public bool IsVH => RevivedRole?.Type == LayerEnum.VampireHunter;
    public float StakeTimer => ButtonUtils.Timer(Player, LastStaked, CustomGameOptions.StakeCd);

    public void Stake()
    {
        if (IsTooFar(Player, StakeButton.TargetPlayer) || StakeTimer != 0f)
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
    public DateTime LastAlerted { get; set; }
    public CustomButton AlertButton { get; set; }
    public bool IsVet => RevivedRole?.Type == LayerEnum.Veteran;
    public float AlertTimer => ButtonUtils.Timer(Player, LastAlerted, CustomGameOptions.AlertCd);

    public void HitAlert()
    {
        if (!ButtonUsable || AlertTimer != 0f || OnEffect)
            return;

        TimeRemaining = CustomGameOptions.AlertDur;
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
    public CustomButton ReviveButton { get; set; }
    public bool IsAlt => RevivedRole?.Type == LayerEnum.Altruist;
    public bool Reviving { get; set; }
    public DeadBody RevivingBody { get; set; }
    public bool Success { get; set; }
    public DateTime LastRevived { get; set; }
    public float ReviveTimer => ButtonUtils.Timer(Player, LastRevived, CustomGameOptions.ReviveCd);

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

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
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
        if (IsTooFar(Player, ReviveButton.TargetBody) || ReviveTimer != 0f || !ButtonUsable)
            return;

        RevivingBody = ReviveButton.TargetBody;
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.AltruistRevive, this, RevivingBody);
        TimeRemaining = CustomGameOptions.ReviveDur;
        Success = true;
        Revive();
    }

    //Medic Stuff
    public bool UsedMedicAbility => ShieldedPlayer != null || ExShielded != null;
    public PlayerControl ShieldedPlayer { get; set; }
    public PlayerControl ExShielded { get; set; }
    public CustomButton ShieldButton { get; set; }
    public bool IsMedic => RevivedRole?.Type == LayerEnum.Medic;

    public void Protect()
    {
        if (IsTooFar(Player, ShieldButton.TargetPlayer) || UsedMedicAbility)
            return;

        var interact = Interact(Player, ShieldButton.TargetPlayer);

        if (interact[3])
        {
            ShieldedPlayer = ShieldButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.Protect, this, ShieldedPlayer);
        }
    }

    //Chameleon Stuff
    public DateTime LastSwooped { get; set; }
    public CustomButton SwoopButton { get; set; }
    public bool IsCham => RevivedRole?.Type == LayerEnum.Chameleon;
    public float SwoopTimer => ButtonUtils.Timer(Player, LastSwooped, CustomGameOptions.SwoopCd);

    public void HitSwoop()
    {
        if (SwoopTimer != 0f || OnEffect || !ButtonUsable)
            return;

        TimeRemaining = CustomGameOptions.SwoopDur;
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
    public CustomButton FixButton { get; set; }
    public DateTime LastFixed { get; set; }
    public bool IsEngi => RevivedRole?.Type == LayerEnum.Engineer;
    public float FixTimer => ButtonUtils.Timer(Player, LastFixed, CustomGameOptions.FixCd);

    public void Fix()
    {
        if (!ButtonUsable || FixTimer != 0f)
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
    public DateTime LastRevealed { get; set; }
    public CustomButton RevealButton { get; set; }
    public float RevealTimer => ButtonUtils.Timer(Player, LastRevealed, CustomGameOptions.MysticRevealCd);
    public bool IsMys => RevivedRole?.Type == LayerEnum.Mystic;

    public void Reveal()
    {
        if (RevealTimer != 0f || IsTooFar(Player, RevealButton.TargetPlayer))
            return;

        var interact = Interact(Player, RevealButton.TargetPlayer);

        if (interact[3])
        {
            Flash((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) ||
                RevealButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);
        }

        if (interact[0])
            LastRevealed = DateTime.UtcNow;
        else if (interact[1])
            LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Seer Stuff
    public DateTime LastSeered { get; set; }
    public CustomButton SeerButton { get; set; }
    public bool IsSeer => RevivedRole?.Type == LayerEnum.Seer;
    public float SeerTimer => ButtonUtils.Timer(Player, LastSeered, CustomGameOptions.SeerCd);

    public void See()
    {
        if (SeerTimer != 0f || IsTooFar(Player, SeerButton.TargetPlayer))
            return;

        var interact = Interact(Player, SeerButton.TargetPlayer);

        if (interact[3])
            Flash(GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);

        if (interact[0])
            LastSeered = DateTime.UtcNow;
        else if (interact[1])
            LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Escort Stuff
    public PlayerControl BlockTarget { get; set; }
    public DateTime LastBlocked { get; set; }
    public CustomButton BlockButton { get; set; }
    public bool IsEsc => RevivedRole?.Type == LayerEnum.Escort;
    public float BlockTimer => ButtonUtils.Timer(Player, LastBlocked, CustomGameOptions.EscortCd);

    public void UnBlock()
    {
        Enabled = false;
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
        LastBlocked = DateTime.UtcNow;
    }

    public void Block()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

        if (Meeting || IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected)
            TimeRemaining = 0f;
    }

    public void Roleblock()
    {
        if (BlockTimer != 0f || IsTooFar(Player, BlockButton.TargetPlayer))
            return;

        var interact = Interact(Player, BlockButton.TargetPlayer);

        if (interact[3])
        {
            TimeRemaining = CustomGameOptions.EscortDur;
            BlockTarget = BlockButton.TargetPlayer;
            Block();
            CallRpc(CustomRPC.Action, ActionsRPC.RetributionistAction, RetributionistActionsRPC.EscRoleblock, this, BlockTarget);
        }
        else if (interact[0])
            LastBlocked = DateTime.UtcNow;
        else if (interact[1])
            LastBlocked.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Transporter Stuff
    public DateTime LastTransported { get; set; }
    public PlayerControl TransportPlayer1 { get; set; }
    public PlayerControl TransportPlayer2 { get; set; }
    public Dictionary<byte, DateTime> UntransportablePlayers { get; set; }
    public CustomButton TransportButton { get; set; }
    public CustomMenu TransportMenu1 { get; set; }
    public CustomMenu TransportMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying1 { get; set; }
    public SpriteRenderer AnimationPlaying2 { get; set; }
    public GameObject Transport1 { get; set; }
    public GameObject Transport2 { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent1 { get; set; }
    public bool WasInVent2 { get; set; }
    public Vent Vent1 { get; set; }
    public Vent Vent2 { get; set; }
    public bool IsTrans => RevivedRole?.Type == LayerEnum.Transporter;
    public float TransportTimer => ButtonUtils.Timer(Player, LastTransported, CustomGameOptions.TransportCd);

    public IEnumerator TransportPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent1 = false;
        WasInVent2 = false;
        Vent1 = null;
        Vent2 = null;
        TimeRemaining = CustomGameOptions.TransportDur;

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
            Flash(Color, CustomGameOptions.TransportDur);

        if (Player1Body == null && !WasInVent1)
            AnimateTransport1();

        if (Player2Body == null && !WasInVent2)
            AnimateTransport2();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var now = DateTime.UtcNow;
            var seconds = (now - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.TransportDur)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }
            else
                break;

            if (Meeting)
            {
                TimeRemaining = 0;
                yield break;
            }
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

            if (Map)
                Map.Close();
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

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDur, new Action<float>(p =>
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

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDur, new Action<float>(p =>
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
        if (TransportTimer != 0f)
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