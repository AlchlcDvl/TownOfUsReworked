namespace TownOfUsReworked.PlayerLayers.Roles;

public class Retributionist : Crew
{
    public Retributionist(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        BodyArrows = new();
        MediatedPlayers = new();
        Bugs = new();
        TrackerArrows = new();
        Reported = new();
        ReferenceBodies = new();
        AllPrints = new();
        BombedIDs = new();
        Investigated = new();
        BuggedPlayers = new();
        Selected = null;
        TransportPlayer1 = null;
        TransportPlayer2 = null;
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
        Transport2.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying1 = Transport1.AddComponent<SpriteRenderer>();
        AnimationPlaying2 = Transport2.AddComponent<SpriteRenderer>();
        AnimationPlaying1.sprite = AnimationPlaying2.sprite = PortalAnimation[0];
        AnimationPlaying1.material = AnimationPlaying2.material = HatManager.Instance.PlayerMaterial;
        Transport1.SetActive(true);
        Transport2.SetActive(true);
        TransportMenu1 = new(Player, Click1, TransException1);
        TransportMenu2 = new(Player, Click2, TransException2);
        RevealButton = new(this, "MysticReveal", AbilityTypes.Target, "ActionSecondary", Reveal, CustomGameOptions.MysticRevealCd, MysticException);
        StakeButton = new(this, "Stake", AbilityTypes.Target, "ActionSecondary", Stake, CustomGameOptions.StakeCd);
        AutopsyButton = new(this, "Autopsy", AbilityTypes.Dead, "ActionSecondary", Autopsy, CustomGameOptions.AutopsyCd);
        CompareButton = new(this, "Compare", AbilityTypes.Target, "Secondary", Compare, CustomGameOptions.CompareCd);
        ExamineButton = new(this, "Examine", AbilityTypes.Target, "ActionSecondary", Examine, CustomGameOptions.ExamineCd);
        MediateButton = new(this, "Mediate", AbilityTypes.Targetless, "ActionSecondary", Mediate, CustomGameOptions.MediateCd);
        BugButton = new(this, "Bug", AbilityTypes.Targetless, "ActionSecondary", PlaceBug, CustomGameOptions.BugCd, CustomGameOptions.MaxBugs);
        SeerButton = new(this, "Seer", AbilityTypes.Target, "ActionSecondary", See, CustomGameOptions.SeerCd);
        InterrogateButton = new(this, "Interrogate", AbilityTypes.Target, "ActionSecondary", Interrogate, CustomGameOptions.InterrogateCd, SherException);
        TrackButton = new(this, "Track", AbilityTypes.Target, "ActionSecondary", Track, CustomGameOptions.TrackCd, TrackException, CustomGameOptions.MaxTracks);
        AlertButton = new(this, "Alert", AbilityTypes.Targetless, "ActionSecondary", Alert, CustomGameOptions.AlertCd, CustomGameOptions.AlertDur, CustomGameOptions.MaxAlerts);
        ShootButton = new(this, "Shoot", AbilityTypes.Target, "ActionSecondary", Shoot, CustomGameOptions.ShootCd, VigiException, CustomGameOptions.MaxBullets);
        ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", Revive, CustomGameOptions.ReviveCd, CustomGameOptions.ReviveDur, UponEnd, CustomGameOptions.MaxRevives);
        ShieldButton = new(this, "Shield", AbilityTypes.Target, "ActionSecondary", Protect, MedicException);
        SwoopButton = new(this, "Swoop", AbilityTypes.Targetless, "ActionSecondary", Swoop, CustomGameOptions.SwoopCd, CustomGameOptions.SwoopDur, Invis, UnInvis,
            CustomGameOptions.MaxSwoops);
        FixButton = new(this, "Fix", AbilityTypes.Targetless, "ActionSecondary", Fix, CustomGameOptions.FixCd, CustomGameOptions.MaxFixes);
        BlockButton = new(this, "EscortRoleblock", AbilityTypes.Target, "ActionSecondary", Roleblock, CustomGameOptions.EscortCd, CustomGameOptions.EscortDur,
            (CustomButton.EffectVoid)Block, UnBlock);
        TransportButton = new(this, "Transport", AbilityTypes.Targetless, "ActionSecondary", Transport, CustomGameOptions.TransportCd, CustomGameOptions.MaxTransports);
        BombButton = new(this, $"{Bastion.SpriteName}VentBomb", AbilityTypes.Vent, "ActionSecondary", Bomb, CustomGameOptions.BastionCd, CustomGameOptions.MaxBombs, BastException);
        RetMenu = new(Player, "RetActive", "RetDisabled", CustomGameOptions.ReviveAfterVoting, SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
    }

    //Retributionist Stuff
    public PlayerVoteArea Selected { get; set; }
    public PlayerControl Revived { get; set; }
    public Role RevivedRole => Revived == null ? null : (Revived.Is(LayerEnum.Revealer) ? GetRole<Revealer>(Revived).FormerRole : GetRole(Revived));
    public CustomMeeting RetMenu { get; set; }

    public override Color Color
    {
        get
        {
            if (!ClientGameOptions.CustomCrewColors)
                return Colors.Crew;
            else if (RevivedRole != null)
                return RevivedRole.Color;
            else
                return Colors.Retributionist;
        }
    }
    public override string Name => "Retributionist";
    public override LayerEnum Type => LayerEnum.Retributionist;
    public override Func<string> StartText => () => "Mimic the Dead";
    public override Func<string> Description => () => "- You can mimic the abilities of dead <color=#8CFFFFFF>Crew</color>" + (RevivedRole == null ? "" : $"\n{RevivedRole.Description()}");

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

        ClearFootprints();
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

    private bool IsExempt(PlayerVoteArea voteArea) => !voteArea.AmDead || PlayerByVoteArea(voteArea).Data.Disconnected || IsDead || !PlayerByVoteArea(voteArea).Is(Faction.Crew);

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion)
            return;

        if (Selected != null)
            RetMenu.Actives[Selected.TargetPlayerId] = false;

        Selected = voteArea;
        RetMenu.Actives[voteArea.TargetPlayerId] = true;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        TransportButton.Update2(TransportPlayer1 == null ? "FIRST TARGET" : (TransportPlayer2 == null ? "SECOND TARGET" : "TRANSPORT"), IsTrans);
        FixButton.Update2("FIX SABOTAGE", IsEngi, Condition());
        ShieldButton.Update2("SHIELD", ShieldedPlayer == null && ExShielded == null && IsMedic);
        RevealButton.Update2("REVEAL", IsMys);
        StakeButton.Update2("STAKE", IsVH);
        AutopsyButton.Update2("AUTOPSY", IsCor);
        CompareButton.Update2("COMPARE", ReferenceBodies.Count > 0 && IsCor);
        ExamineButton.Update2("EXAMINE", IsDet);
        MediateButton.Update2("MEDIATE", IsMed);
        BugButton.Update2("BUG", IsOP, !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2));
        SeerButton.Update2("SEE", IsSeer);
        InterrogateButton.Update2("INTERROGATE", IsSher);
        AlertButton.Update2("ALERT", IsVet);
        ShootButton.Update2("SHOOT", IsVig);
        ReviveButton.Update2("REVIVE", IsAlt);
        SwoopButton.Update2("SWOOP", IsCham);
        BlockButton.Update2("ROLEBLOCK", IsEsc);
        TrackButton.Update2("TRACK", IsTrack);
        BombButton.Update2("PLACE BOMB", IsBast);

        if (IsDead)
            OnLobby();
        else if (IsCor)
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

                foreach (var id in Investigated)
                {
                    var player = PlayerById(id);

                    if (player.HasDied() || player == CustomPlayer.Local)
                        continue;

                    if (!AllPrints.Any(print => Vector2.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == player.PlayerId))
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
        else if (IsTrans)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!Transporting)
                {
                    if (TransportPlayer2 != null)
                        TransportPlayer2 = null;
                    else if (TransportPlayer1 != null)
                        TransportPlayer1 = null;
                }

                LogInfo("Removed a target");
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var retAction = (RetActionsRPC)reader.ReadByte();

        switch (retAction)
        {
            case RetActionsRPC.Revive:
                Revived = reader.ReadPlayer();
                break;

            case RetActionsRPC.Transport:
                var retRole3 = reader.ReadLayer<Retributionist>();
                retRole3.TransportPlayer1 = reader.ReadPlayer();
                retRole3.TransportPlayer2 = reader.ReadPlayer();
                Coroutines.Start(retRole3.TransportPlayers());
                break;

            case RetActionsRPC.Protect:
                ShieldedPlayer = reader.ReadPlayer();
                break;

            case RetActionsRPC.Mediate:
                var playerid2 = reader.ReadByte();
                MediatedPlayers.Add(playerid2);

                if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
                    LocalRole.DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Colors.Retributionist));

                break;

            case RetActionsRPC.Roleblock:
                BlockTarget = reader.ReadPlayer();
                break;

            case RetActionsRPC.Bomb:
                BombedIDs.Add(reader.ReadInt32());
                break;

            default:
                LogError($"Received unknown RPC - {retAction}");
                break;
        }
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        RetMenu.Voted();

        if (Selected != null)
        {
            Revived = PlayerByVoteArea(Selected);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Revive, Selected);
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Revive, Selected);
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
                var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Operative or LayerEnum.Amnesiac or LayerEnum.Actor or LayerEnum.Arsonist or
                    LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
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
            ClearFootprints();

        Revived = null;
    }

    public override void OnBodyReport(GameData.PlayerInfo info)
    {
        if (info == null || !Local)
            return;

        if (IsCor)
        {
            base.OnBodyReport(info);
            var body = KilledPlayers.Find(x => x.PlayerId == info.PlayerId);

            if (body == null)
                return;

            Reported.Add(info.PlayerId);
            body.Reporter = Player;
            body.KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds;
            var reportMsg = body.ParseBodyReport();

            if (IsNullEmptyOrWhiteSpace(reportMsg))
                return;

            //Only Retributionist-Coroner can see this
            if (HUD)
                Run(HUD.Chat, "<color=#8D0F8CFF>〖 Autopsy Results 〗</color>", reportMsg);
        }
    }

    //Coroner Stuff
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton AutopsyButton { get; set; }
    public CustomButton CompareButton { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; set; }
    public List<byte> Reported { get; set; }
    public bool IsCor => RevivedRole?.Type == LayerEnum.Coroner;

    public void Autopsy()
    {
        var playerId = AutopsyButton.TargetBody.ParentId;
        Spread(Player, PlayerById(playerId));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == playerId));
        AutopsyButton.StartCooldown();
    }

    public void Compare()
    {
        var interact = Interact(Player, CompareButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Flash(ReferenceBodies.Any(x => CompareButton.TargetPlayer.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        CompareButton.StartCooldown(cooldown);
    }

    public bool CorUsable() => ReferenceBodies.Count > 0 && IsCor;

    //Detective Stuff
    public CustomButton ExamineButton { get; set; }
    public bool IsDet => RevivedRole?.Type == LayerEnum.Detective;
    private static float _time;
    public List<Footprint> AllPrints { get; set; }
    public List<byte> Investigated { get; set; }

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        var interact = Interact(Player, ExamineButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            Flash(ExamineButton.TargetPlayer.IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                CustomGameOptions.RecentKill) ? UColor.red : UColor.green);
            Investigated.Add(ExamineButton.TargetPlayer.PlayerId);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        ExamineButton.StartCooldown(cooldown);
    }

    public void ClearFootprints()
    {
        AllPrints.ForEach(x => x.Destroy());
        AllPrints.Clear();
    }

    //Medium Stuff
    public Dictionary<byte, CustomArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    public List<byte> MediatedPlayers { get; set; }
    public bool IsMed => RevivedRole?.Type == LayerEnum.Medium;

    public void Mediate()
    {
        MediateButton.StartCooldown();
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
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Mediate, dead.PlayerId);

                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                        break;
                }
            }
        }
        else
        {
            var dead = playersDead.Random();

            if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, Color));
                MediatedPlayers.Add(dead.PlayerId);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Mediate, dead.PlayerId);
            }
        }
    }

    //private static void Seance() { Currently blank, gonna work on this later }

    //Operative Stuff
    public List<Bug> Bugs { get; set; }
    public List<LayerEnum> BuggedPlayers { get; set; }
    public CustomButton BugButton { get; set; }
    public bool IsOP => RevivedRole?.Type == LayerEnum.Operative;

    public void PlaceBug()
    {
        Bugs.Add(new(Player));
        BugButton.StartCooldown();
    }

    //Sheriff Stuff
    public CustomButton InterrogateButton { get; set; }
    public bool IsSher => RevivedRole?.Type == LayerEnum.Sheriff;

    public void Interrogate()
    {
        var interact = Interact(Player, InterrogateButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool SherException(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
        CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
        CustomGameOptions.LinkedRoles);

    //Tracker Stuff
    public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
    public CustomButton TrackButton { get; set; }
    public bool IsTrack => RevivedRole?.Type == LayerEnum.Tracker;

    public void Track()
    {
        var interact = Interact(Player, TrackButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            TrackerArrows.Add(TrackButton.TargetPlayer.PlayerId, new(Player, TrackButton.TargetPlayer.GetPlayerColor(), CustomGameOptions.UpdateInterval));

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        TrackButton.StartCooldown(cooldown);
    }

    public bool TrackException(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    //Vigilante Stuff
    public CustomButton ShootButton { get; set; }
    public bool IsVig => RevivedRole?.Type == LayerEnum.Vigilante;

    public void Shoot()
    {
        var interact = Interact(Player, ShootButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        ShootButton.StartCooldown(cooldown);
    }

    public bool VigiException(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None)
        || Player.IsLinkedTo(player);

    //Vampire Hunter Stuff
    public CustomButton StakeButton { get; set; }
    public bool IsVH => RevivedRole?.Type == LayerEnum.VampireHunter;

    public void Stake()
    {
        var interact = Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        StakeButton.StartCooldown(cooldown);
    }

    //Veteran Stuff
    public CustomButton AlertButton { get; set; }
    public bool IsVet => RevivedRole?.Type == LayerEnum.Veteran;

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AlertButton);
        AlertButton.Begin();
    }

    //Altruist Stuff
    public CustomButton ReviveButton { get; set; }
    public bool IsAlt => RevivedRole?.Type == LayerEnum.Altruist;
    public DeadBody RevivingBody { get; set; }
    public bool Success { get; set; }

    public void UponEnd()
    {
        if (!(Meeting || IsDead))
            FinishRevive();
    }

    public bool ReviveEnd() => IsDead;

    private void FinishRevive()
    {
        var player = PlayerByBody(RevivingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = GetRole(player);
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = GetRole(lover);
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (ReviveButton.Uses == 0 && Local)
            RpcMurderPlayer(Player);

        if (formerKiller.Contains(CustomPlayer.LocalCustom.PlayerName))
        {
            LocalRole.AllArrows.Add(player.PlayerId, new(CustomPlayer.Local, Color));
            Flash(Color);
        }
    }

    public void Revive()
    {
        RevivingBody = ReviveButton.TargetBody;
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ReviveButton, RevivingBody);
        ReviveButton.Begin();
        Flash(Color, CustomGameOptions.ReviveDur);

        if (CustomGameOptions.AltruistTargetBody)
            ReviveButton.TargetBody?.gameObject.Destroy();
    }

    //Medic Stuff
    public PlayerControl ShieldedPlayer { get; set; }
    public PlayerControl ExShielded { get; set; }
    public CustomButton ShieldButton { get; set; }
    public bool IsMedic => RevivedRole?.Type == LayerEnum.Medic;

    public void Protect()
    {
        var interact = Interact(Player, ShieldButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            ShieldedPlayer = ShieldButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Protect, ShieldedPlayer);
        }
    }

    public bool MedicException(PlayerControl player) => player == ShieldedPlayer;

    //Chameleon Stuff
    public CustomButton SwoopButton { get; set; }
    public bool IsCham => RevivedRole?.Type == LayerEnum.Chameleon;

    public void Invis() => Utils.Invis(Player);

    public bool SwoopEnd() => IsDead;

    public void UnInvis() => DefaultOutfit(Player);

    public void Swoop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, SwoopButton);
        SwoopButton.Begin();
    }

    //Engineer Stuff
    public CustomButton FixButton { get; set; }
    public bool IsEngi => RevivedRole?.Type == LayerEnum.Engineer;

    public bool Condition() => ShipStatus.Instance.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>()?.AnyActive == true;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }

    //Mystic Stuff
    public CustomButton RevealButton { get; set; }
    public bool IsMys => RevivedRole?.Type == LayerEnum.Mystic;

    public void Reveal()
    {
        var interact = Interact(Player, RevealButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            Flash((!RevealButton.TargetPlayer.Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.TargetPlayer.Is(Alignment.NeutralNeo)) || RevealButton.TargetPlayer.IsFramed()
                ? UColor.red : UColor.green);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        RevealButton.StartCooldown(cooldown);
    }

    public bool MysticException(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;

    //Seer Stuff
    public CustomButton SeerButton { get; set; }
    public bool IsSeer => RevivedRole?.Type == LayerEnum.Seer;

    public void See()
    {
        var interact = Interact(Player, SeerButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Flash(GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        SeerButton.StartCooldown(cooldown);
    }

    //Escort Stuff
    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }
    public bool IsEsc => RevivedRole?.Type == LayerEnum.Escort;

    public void UnBlock()
    {
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

    public void Roleblock()
    {
        var interact = Interact(Player, BlockButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            BlockTarget = BlockButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
        else if (interact.Reset)
            BlockButton.StartCooldown();
        else if (interact.Protected)
            BlockButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public bool BlockEnd() => IsDead || BlockTarget.HasDied();

    //Transporter Stuff
    public PlayerControl TransportPlayer1 { get; set; }
    public PlayerControl TransportPlayer2 { get; set; }
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
    public bool Transporting { get; set; }
    public bool IsTrans => RevivedRole?.Type == LayerEnum.Transporter;

    public bool TransException1(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer2 || player.IsMoving();

    public bool TransException2(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer1 || player.IsMoving();

    public IEnumerator TransportPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent1 = false;
        WasInVent2 = false;
        Vent1 = null;
        Vent2 = null;

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

        Transporting = true;
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
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.TransportDur)
                yield return null;
            else
                break;

            if (Meeting)
            {
                AnimationPlaying1.sprite = AnimationPlaying2.sprite = PortalAnimation[0];
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

            if (IsSubmerged())
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

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer2)
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

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer1)
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
            if (ActiveTask)
                ActiveTask.Close();

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
        Transporting = false;
        yield break;
    }

    public string Label() => TransportPlayer1 == null ? "FIRST TARGET" : (TransportPlayer2 == null ? "SECOND TARGET" : "TRANSPORT");

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

        if (interact.AbilityUsed)
            TransportPlayer1 = player;
        else if (interact.Reset)
            TransportButton.StartCooldown();
        else if (interact.Protected)
            TransportButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Click2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            TransportPlayer2 = player;
        else if (interact.Reset)
            TransportButton.StartCooldown();
        else if (interact.Protected)
            TransportButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Transport()
    {
        if (TransportPlayer1 == null)
            TransportMenu1.Open();
        else if (TransportPlayer2 == null)
            TransportMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Transport, TransportPlayer1, TransportPlayer2);
            Coroutines.Start(TransportPlayers());
            TransportButton.StartCooldown();
        }
    }

    //Bastion Stuff
    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; set; }
    public bool IsBast => RevivedRole?.Type == LayerEnum.Bastion;

    public bool BastException(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb()
    {
        var interact = Interact(Player, BombButton.TargetVent);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            BombedIDs.Add(BombButton.TargetVent.Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RetActionsRPC.Bomb, BombButton.TargetVent);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        BombButton.StartCooldown(cooldown);
    }
}