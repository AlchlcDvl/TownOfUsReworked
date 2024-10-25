namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Retributionist : Crew
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ReviveAfterVoting { get; set; } = true;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        BodyArrows = [];
        MediatedPlayers = [];
        Bugs = [];
        TrackerArrows = [];
        Reported = [];
        ReferenceBodies = [];
        AllPrints = [];
        BombedIDs = [];
        Investigated = [];
        BuggedPlayers = [];
        Trapped = [];
        TriggeredRoles = [];
        MediateArrows = [];
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
        RetMenu = new(Player, "RetActive", "RetDisabled", ReviveAfterVoting, SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
        TrapsMade = 0;
        TrapButton.Uses = 0;
    }

    // Retributionist Stuff
    public PlayerVoteArea Selected { get; set; }
    public PlayerControl Revived { get; set; }
    public Role RevivedRole => Revived ? (Revived.Is(LayerEnum.Revealer) ? Revived.GetLayer<Revealer>().FormerRole : Revived.GetRole()) : null;
    public CustomMeeting RetMenu { get; set; }

    public override UColor Color
    {
        get
        {
            if (!ClientOptions.CustomCrewColors)
                return CustomColorManager.Crew;
            else if (RevivedRole)
                return RevivedRole.Color;
            else
                return CustomColorManager.Retributionist;
        }
    }
    public override string Name => "Retributionist";
    public override LayerEnum Type => LayerEnum.Retributionist;
    public override Func<string> StartText => () => "Mimic the Dead";
    public override Func<string> Description => () => "- You can mimic the abilities of dead <color=#8CFFFFFF>Crew</color>" + (RevivedRole ? $"\n{RevivedRole.Description()}" : "");
    public override AttackEnum AttackVal
    {
        get
        {
            if (IsBast || (IsVet && AlertButton.EffectActive))
                return AttackEnum.Powerful;
            else if (IsVig)
                return AttackEnum.Basic;
            else
                return AttackEnum.None;
        }
    }
    public override DefenseEnum DefenseVal
    {
        get
        {
            if (IsVet && AlertButton.EffectActive)
                return DefenseEnum.Basic;
            else
                return DefenseEnum.None;
        }
    }

    public override void Deinit()
    {
        base.Deinit();

        if (BodyArrows.Count > 0)
        {
            BodyArrows.Values.ToList().DestroyAll();
            BodyArrows.Clear();
        }

        if (MediateArrows.Count > 0)
        {
            MediateArrows.Values.ToList().DestroyAll();
            MediateArrows.Clear();
            MediatedPlayers.Clear();
        }

        if (TrackerArrows.Count > 0)
        {
            TrackerArrows.Values.ToList().DestroyAll();
            TrackerArrows.Clear();
        }

        if (Bugs.Count > 0)
        {
            Bugs.ForEach(x => x?.gameObject?.Destroy());
            Bugs.Clear();
        }

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

    private bool IsExempt(PlayerVoteArea voteArea) => !voteArea.AmDead || PlayerByVoteArea(voteArea).HasDied() || Dead || !voteArea.IsBase(Faction.Crew);

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion)
            return;

        if (Selected)
            RetMenu.Actives[Selected.TargetPlayerId] = false;

        Selected = voteArea;
        RetMenu.Actives[voteArea.TargetPlayerId] = true;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Dead)
            Deinit();
        else if (IsCor)
        {
            var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow < y.KillTime.AddSeconds(Coroner.CoronerArrowDur)));

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
            foreach (var player in AllPlayers())
            {
                if (MediateArrows.ContainsKey(player.PlayerId))
                {
                    MediateArrows[player.PlayerId]?.Update(player.transform.position, player.GetPlayerColor(false, Medium.ShowMediatePlayer));

                    if (!Medium.ShowMediatePlayer)
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

                if (!player || player.Data.Disconnected || (player.Data.IsDead && !body))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor());
            }
        }
        else if (IsDet)
        {
            _time += Time.deltaTime;

            if (_time >= Detective.FootprintInterval)
            {
                _time -= Detective.FootprintInterval;

                foreach (var id in Investigated)
                {
                    var player = PlayerById(id);

                    if (player.HasDied() || player.AmOwner)
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
                    if (TransportPlayer2)
                        TransportPlayer2 = null;
                    else if (TransportPlayer1)
                        TransportPlayer1 = null;
                }

                Message("Removed a target");
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var retAction = reader.ReadEnum<RetActionsRPC>();

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

            case RetActionsRPC.ProtectAdd:
                ShieldedPlayer = reader.ReadPlayer();
                break;

            case RetActionsRPC.ProtectRemove:
                ShieldedPlayer = null;
                break;

            case RetActionsRPC.Mediate:
                var playerid2 = reader.ReadByte();
                MediatedPlayers.Add(playerid2);

                if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.Dead && Medium.ShowMediumToDead == ShowMediumToDead.AllDead))
                    LocalRole.DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Color));

                break;

            case RetActionsRPC.Roleblock:
                BlockTarget = reader.ReadPlayer();
                break;

            case RetActionsRPC.Bomb:
                BombedIDs.Add(reader.ReadInt32());
                break;

            case RetActionsRPC.Place:
                Trapped.Add(reader.ReadByte());
                break;

            case RetActionsRPC.Trigger:
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                break;

            default:
                Error($"Received unknown RPC - {(int)retAction}");
                break;
        }
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        RetMenu.Voted();
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        RetMenu.Update(__instance);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);
        RetMenu.HideButtons();

        if (Selected && !Dead)
        {
            Revived = PlayerByVoteArea(Selected);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Revive, Selected);
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        RetMenu.GenButtons(__instance);
        Selected = null;

        if (IsOp)
        {
            var message = "";

            if (BuggedPlayers.Count == 0)
                message = "No one triggered your bugs.";
            else if (BuggedPlayers.Count < Operative.MinAmountOfPlayersInBug)
                message = "Not enough players triggered your bugs.";
            else if (BuggedPlayers.Count == 1)
            {
                var result = BuggedPlayers[0];
                var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Operative or LayerEnum.Amnesiac or LayerEnum.Actor or LayerEnum.Arsonist or
                    LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
                message = $"A{a_an} {result} triggered your bug.";
            }
            else if (Operative.PreciseOperativeInfo)
            {
                message = "Your bugs returned the following results:";
                Bugs.ForEach(bug => message += $"\n{bug.GetResults()}");
            }
            else
            {
                message = "The following roles triggered your bugs: ";
                BuggedPlayers.Shuffle();
                BuggedPlayers.ForEach(role => message += $"{GetLayers(role)[0]}, ");
                message = message.Remove(message.Length - 2);
            }

            // Only Retributionist-Operative can see this
            if (HUD())
                Run("<color=#8D0F8CFF>〖 Bug Results 〗</color>", message);
        }
        else if (IsDet)
            ClearFootprints();
        else if (IsTrap)
        {
            if (!AttackedSomeone && TriggeredRoles.Any())
            {
                var message = "Your trap detected the following roles: ";
                TriggeredRoles.Shuffle();
                TriggeredRoles.ForEach(x => message += $"{x}, ");
                message = message.Remove(message.Length - 2);

                if (IsNullEmptyOrWhiteSpace(message))
                    return;

                // Only Retributionist-Trapper can see this
                if (HUD())
                    Run("<color=#8D0F8CFF>〖 Trap Triggers 〗</color>", message);
            }
            else if (AttackedSomeone && HUD())
                Run("<color=#8D0F8CFF>〖 Trap Triggers 〗</color>", "Your trap attacked someone!");

            TriggeredRoles.Clear();
        }

        Revived = null;
    }

    public override void OnBodyReport(NetworkedPlayerInfo info)
    {
        base.OnBodyReport(info);

        if (info == null || !Local)
            return;

        if (IsCor)
        {
            if (!KilledPlayers.TryFinding(x => x.PlayerId == info.PlayerId, out var body))
                return;

            Reported.Add(info.PlayerId);
            body.Reporter = Player;
            body.KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds;
            var reportMsg = body.ParseBodyReport();

            if (IsNullEmptyOrWhiteSpace(reportMsg))
                return;

            // Only Retributionist-Coroner can see this
            if (HUD())
                Run("<color=#8D0F8CFF>〖 Autopsy Results 〗</color>", reportMsg);
        }
    }

    public void OnRoleSelected()
    {
        if (!Revived)
            return;

        if (IsMys)
        {
            RevealButton ??= CreateButton(this, "REVEAL", new SpriteName("MysticReveal"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Reveal, (UsableFunc)MysUsable,
                (PlayerBodyExclusion)MysticException, new Cooldown(Mystic.RevealCd));
        }
        else if (IsVH)
        {
            StakeButton ??= CreateButton(this, "STAKE", new SpriteName("Stake"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Stake, new Cooldown(VampireHunter.StakeCd),
                (UsableFunc)VHUsable);
        }
        else if (IsCor)
        {
            AutopsyButton ??= CreateButton(this, "AUTOPSY", new SpriteName("Autopsy"), AbilityType.Dead, KeybindType.ActionSecondary, (OnClick)Autopsy, (UsableFunc)CorUsable1,
                new Cooldown(Coroner.AutopsyCd));
            CompareButton ??= CreateButton(this, "COMPARE", new SpriteName("Compare"), AbilityType.Alive, KeybindType.Secondary, (OnClick)Compare, new Cooldown(Coroner.CompareCd),
                (UsableFunc)CorUsable2);
        }
        else if (IsDet)
        {
            ExamineButton ??= CreateButton(this, "EXAMINE", new SpriteName("Examine"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Examine, (UsableFunc)DetUsable,
                new Cooldown(Detective.ExamineCd));
        }
        else if (IsMed)
        {
            MediateButton ??= CreateButton(this, "MEDIATE", new SpriteName("Mediate"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Mediate, (UsableFunc)MedUsable,
                new Cooldown(Medium.MediateCd));/*
            SeanceButton ??= CreateButton(this, "SEANCE", new SpriteName("Seance"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Seance, (UsableFunc)MedUsable,
                new Cooldown(Medium.SeanceCd), new PostDeath(true));*/
        }
        else if (IsOp)
        {
            BugButton ??= CreateButton(this, "BUG", new SpriteName("Bug"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)PlaceBug, new Cooldown(Operative.BugCd),
                Operative.MaxBugs, (ConditionFunc)OpCondition, (UsableFunc)OpUsable);
        }
        else if (IsSeer)
        {
            SeerButton ??= CreateButton(this, "ENVISION", new SpriteName("Seer"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)See, new Cooldown(Seer.SeerCd),
                (UsableFunc)SeerUsable);
        }
        else if (IsSher)
        {
            InterrogateButton ??= CreateButton(this, "INTERROGATE", new SpriteName("Interrogate"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Interrogate,
                new Cooldown(Sheriff.InterrogateCd), (PlayerBodyExclusion)SherException, (UsableFunc)SherUsable);
        }
        else if (IsTrack)
        {
            TrackButton ??= CreateButton(this, "TRACK", new SpriteName("Track"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Track, new Cooldown(Tracker.TrackCd),
                (PlayerBodyExclusion)TrackException, Tracker.MaxTracks, (UsableFunc)TrackUsable);
        }
        else if (IsBast)
        {
            BombButton ??= CreateButton(this, "PLACE BOMB", new SpriteName($"{Bastion.SpriteName}VentBomb"), AbilityType.Vent, KeybindType.ActionSecondary, (OnClick)Bomb,
                new Cooldown(Bastion.BastionCd), (VentExclusion)BastException, Bastion.MaxBombs, (UsableFunc)BastUsable);
        }
        else if (IsVet)
        {
            AlertButton ??= CreateButton(this, "ALERT", new SpriteName("Alert"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Alert, (UsableFunc)VetUsable,
                new Cooldown(Veteran.AlertCd), new Duration(Veteran.AlertDur), Veteran.MaxAlerts, (EndFunc)AlertEnd);
        }
        else if (IsVig)
        {
            ShootButton ??= CreateButton(this, "SHOOT", new SpriteName("Shoot"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Shoot, new Cooldown(Vigilante.ShootCd),
                (PlayerBodyExclusion)VigiException, Vigilante.MaxBullets, (UsableFunc)VigUsable);
        }
        else if (IsAlt)
        {
            ReviveButton ??= CreateButton(this, "REVIVE", new SpriteName("Revive"), AbilityType.Dead, KeybindType.ActionSecondary, (OnClick)Revive, new Cooldown(Altruist.ReviveCd),
                new Duration(Altruist.ReviveDur), (EffectEndVoid)UponEnd, Altruist.MaxRevives, (EndFunc)ReviveEnd, (UsableFunc)AltUsable);
        }
        else if (IsMedic)
        {
            ShieldButton ??= CreateButton(this, "SHIELD", new SpriteName("Shield"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Protect, (PlayerBodyExclusion)MedicException,
                (UsableFunc)MedicUsable);
        }
        else if (IsTrap)
        {
            BuildButton ??= CreateButton(this, "BUILD TRAP", new SpriteName("Build"), AbilityType.Targetless, KeybindType.Secondary, (OnClick)StartBuildling, (UsableFunc)TrapUsable1,
                new Cooldown(Trapper.BuildCd), new Duration(Trapper.BuildDur), (EffectEndVoid)EndBuildling, new CanClickAgain(false));
            var wasnull = TrapButton == null;
            TrapButton ??= CreateButton(this, "PLACE TRAP", new SpriteName("Trap"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)SetTrap, (UsableFunc)TrapUsable2,
                new Cooldown(Trapper.TrapCd), (PlayerBodyExclusion)TrapException, Trapper.MaxTraps);

            if (wasnull)
                TrapButton.Uses = 0;
        }
        else if (IsCham)
        {
            SwoopButton ??= CreateButton(this, "SWOOP", new SpriteName("Swoop"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Swoop, (UsableFunc)ChamUsable,
                new Cooldown(Chameleon.SwoopCd), new Duration(Chameleon.SwoopDur), (EffectVoid)Invis, (EffectEndVoid)UnInvis, (EndFunc)SwoopEnd, Chameleon.MaxSwoops);
        }
        else if (IsEngi)
        {
            FixButton ??= CreateButton(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Fix, Engineer.MaxFixes,
                new Cooldown(Engineer.FixCd), (ConditionFunc)EngiCondition, (UsableFunc)EngiUsable);
        }
        else if (IsTrans)
        {
            TransportButton ??= CreateButton(this, new SpriteName("Transport"), AbilityType.Targetless, KeybindType.ActionSecondary, (OnClick)Transport, Transporter.MaxTransports,
                (LabelFunc)TransLabel, new Cooldown(Transporter.TransportCd), (UsableFunc)TransUsable);
        }
    }

    // Coroner Stuff
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton AutopsyButton { get; set; }
    public CustomButton CompareButton { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; set; }
    public List<byte> Reported { get; set; }
    public bool IsCor => RevivedRole?.Type == LayerEnum.Coroner;

    public void Autopsy()
    {
        var playerId = AutopsyButton.GetTarget<DeadBody>().ParentId;
        Spread(Player, PlayerById(playerId));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == playerId));
        AutopsyButton.StartCooldown();
    }

    public void Compare()
    {
        var cooldown = Interact(Player, CompareButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            Flash(ReferenceBodies.Any(x => CompareButton.GetTarget<PlayerControl>().PlayerId == x.KillerId) ? UColor.red : UColor.green);

        CompareButton.StartCooldown(cooldown);
    }

    public bool CorUsable1() => IsCor;

    public bool CorUsable2() => ReferenceBodies.Any() && IsCor;

    // Detective Stuff
    public CustomButton ExamineButton { get; set; }
    private static float _time;
    public List<Footprint> AllPrints { get; set; }
    public List<byte> Investigated { get; set; }
    public bool IsDet => RevivedRole?.Type == LayerEnum.Detective;

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        var cooldown = Interact(Player, ExamineButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            Flash(ExamineButton.GetTarget<PlayerControl>().IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.GetTarget<PlayerControl>().PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                Detective.RecentKill) ? UColor.red : UColor.green);
            Investigated.Add(ExamineButton.GetTarget<PlayerControl>().PlayerId);
        }

        ExamineButton.StartCooldown(cooldown);
    }

    public void ClearFootprints()
    {
        if (AllPrints.Count > 0)
        {
            AllPrints.ForEach(x => x.Destroy());
            AllPrints.Clear();
        }
    }

    public bool DetUsable() => IsDet;

    // Medium Stuff
    public Dictionary<byte, CustomArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    // public CustomButton SeanceButton { get; set; }
    public List<byte> MediatedPlayers { get; set; }
    public bool IsMed => RevivedRole?.Type == LayerEnum.Medium;

    public void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

        if (playersDead.Count == 0)
            return;

        if (Medium.DeadRevealed != DeadRevealed.Random)
        {
            if (Medium.DeadRevealed == DeadRevealed.Newest)
                playersDead.Reverse();

            foreach (var dead in playersDead)
            {
                if (AllBodies().Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, Color));
                    MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Mediate, dead.PlayerId);

                    if (Medium.DeadRevealed != DeadRevealed.Everyone)
                        break;
                }
            }
        }
        else
        {
            var dead = playersDead.Random();

            if (AllBodies().Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, Color));
                MediatedPlayers.Add(dead.PlayerId);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Mediate, dead.PlayerId);
            }
        }
    }

    // private static void Seance() { Currently blank, gonna work on this later }

    public bool MedUsable() => IsMed;

    // Operative Stuff
    public List<Bug> Bugs { get; set; }
    public List<LayerEnum> BuggedPlayers { get; set; }
    public CustomButton BugButton { get; set; }
    public bool IsOp => RevivedRole?.Type == LayerEnum.Operative;

    public void PlaceBug()
    {
        Bugs.Add(Bug.CreateBug(Player));
        BugButton.StartCooldown();
    }

    public bool OpUsable() => IsOp;

    public bool OpCondition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    // Sheriff Stuff
    public CustomButton InterrogateButton { get; set; }
    public bool IsSher => RevivedRole?.Type == LayerEnum.Sheriff;

    public void Interrogate()
    {
        var cooldown = Interact(Player, InterrogateButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            Flash(InterrogateButton.GetTarget<PlayerControl>().SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool SherException(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None))
        && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) && Rivals.RivalsRoles) || (player.Is(LayerEnum.Mafia) &&
        Player.Is(LayerEnum.Mafia) && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);

    public bool SherUsable() => IsSher;

    // Tracker Stuff
    public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
    public CustomButton TrackButton { get; set; }
    public bool IsTrack => RevivedRole?.Type == LayerEnum.Tracker;

    public void Track()
    {
        var cooldown = Interact(Player, TrackButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(TrackButton.GetTarget<PlayerControl>().PlayerId, new(Player, TrackButton.GetTarget<PlayerControl>().GetPlayerColor(), Tracker.UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public bool TrackException(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    public bool TrackUsable() => IsTrack;

    // Vigilante Stuff
    public CustomButton ShootButton { get; set; }
    public bool IsVig => RevivedRole?.Type == LayerEnum.Vigilante;

    public void Shoot()
    {
        var cooldown = Interact(Player, ShootButton.GetTarget<PlayerControl>(), true);

        ShootButton.StartCooldown(cooldown);
    }

    public bool VigiException(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None)
        || Player.IsLinkedTo(player);

    public bool VigUsable() => IsVig;

    // Vampire Hunter Stuff
    public CustomButton StakeButton { get; set; }
    public bool IsVH => RevivedRole?.Type == LayerEnum.VampireHunter;

    public void Stake() => StakeButton.StartCooldown(Interact(Player, StakeButton.GetTarget<PlayerControl>(), StakeButton.GetTarget<PlayerControl>().Is(SubFaction.Undead) || StakeButton.GetTarget<PlayerControl>().IsFramed()));

    public bool VHUsable() => IsVH && !VampireHunter.VampsDead;

    // Veteran Stuff
    public CustomButton AlertButton { get; set; }
    public bool IsVet => RevivedRole?.Type == LayerEnum.Veteran;

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AlertButton);
        AlertButton.Begin();
    }

    public bool VetUsable() => IsVet;

    public bool AlertEnd() => Dead;

    // Altruist Stuff
    public CustomButton ReviveButton { get; set; }
    public DeadBody RevivingBody { get; set; }
    public bool Success { get; set; }
    public bool IsAlt => RevivedRole?.Type == LayerEnum.Altruist;

    public bool AltUsable() => IsAlt;

    public void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    public bool ReviveEnd() => Dead;

    private void FinishRevive()
    {
        var player = PlayerByBody(RevivingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = lover.GetRole();
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
        RevivingBody = ReviveButton.GetTarget<DeadBody>();
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ReviveButton, RetActionsRPC.Revive, RevivingBody);
        ReviveButton.Begin();
        Flash(Color, Altruist.ReviveDur);

        if (Altruist.AltruistTargetBody)
            ReviveButton.GetTarget<DeadBody>()?.gameObject.Destroy();
    }

    // Medic Stuff
    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    public CustomButton ShieldButton { get; set; }
    public bool IsMedic => RevivedRole?.Type == LayerEnum.Medic;

    public void Protect()
    {
        if (Interact(Player, ShieldButton.GetTarget<PlayerControl>()) != CooldownType.Fail)
        {
            if (ShieldedPlayer)
            {
                ShieldedPlayer = null;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.ProtectRemove);
            }
            else
            {
                ShieldedPlayer = ShieldButton.GetTarget<PlayerControl>();
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.ProtectAdd, ShieldedPlayer);
            }
        }
    }

    public bool MedicException(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;
        else
            return (player.Is(LayerEnum.Mayor) && player.GetLayer<Mayor>().Revealed) || (player.Is(LayerEnum.Dictator) && player.GetLayer<Dictator>().Revealed);
    }

    public bool MedicUsable() => !ShieldBroken && IsMedic;

    // Chameleon Stuff
    public CustomButton SwoopButton { get; set; }
    public bool IsCham => RevivedRole?.Type == LayerEnum.Chameleon;

    public void Invis() => Utils.Invis(Player);

    public bool SwoopEnd() => Dead;

    public void UnInvis() => DefaultOutfit(Player);

    public void Swoop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, SwoopButton);
        SwoopButton.Begin();
    }

    public bool ChamUsable() => IsCham;

    // Engineer Stuff
    public CustomButton FixButton { get; set; }
    public bool IsEngi => RevivedRole?.Type == LayerEnum.Engineer;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }

    public bool EngiUsable() => IsEngi && Engineer.MaxFixes > 0;

    public bool EngiCondition() => Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    // Mystic Stuff
    public CustomButton RevealButton { get; set; }
    public bool IsMys => RevivedRole?.Type == LayerEnum.Mystic;

    public void Reveal()
    {
        var cooldown = Interact(Player, RevealButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            Flash((!RevealButton.GetTarget<PlayerControl>().Is(SubFaction) && SubFaction != SubFaction.None && !RevealButton.GetTarget<PlayerControl>().Is(Alignment.NeutralNeo)) || RevealButton.GetTarget<PlayerControl>().IsFramed()
                ? UColor.red : UColor.green);
        }

        RevealButton.StartCooldown(cooldown);
    }

    public bool MysticException(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;

    public bool MysUsable() => IsMys;

    // Seer Stuff
    public CustomButton SeerButton { get; set; }
    public bool IsSeer => RevivedRole?.Type == LayerEnum.Seer;

    public void See()
    {
        var cooldown = Interact(Player, SeerButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            Flash(SeerButton.GetTarget<PlayerControl>().GetRole().RoleHistory.Any() || SeerButton.GetTarget<PlayerControl>().IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    public bool SeerUsable() => IsSeer;

    // Escort Stuff
    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }
    public bool IsEsc => RevivedRole?.Type == LayerEnum.Escort;

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget.GetButtons().ForEach(x => x.BlockExposed = false);
        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

    public void Roleblock()
    {
        var cooldown = Interact(Player, BlockButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = BlockButton.GetTarget<PlayerControl>();
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, RetActionsRPC.Roleblock, BlockTarget);
            BlockButton.Begin();
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public bool BlockEnd() => Dead || BlockTarget.HasDied();

    public bool EscUsable() => IsEsc;

    // Transporter Stuff
    public PlayerControl TransportPlayer1 { get; set; }
    public PlayerControl TransportPlayer2 { get; set; }
    public CustomButton TransportButton { get; set; }
    public CustomPlayerMenu TransportMenu1 { get; set; }
    public CustomPlayerMenu TransportMenu2 { get; set; }
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

    public bool TransException1(PlayerControl player) => (player == Player && !Transporter.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead) || player == TransportPlayer2;

    public bool TransException2(PlayerControl player) => (player == Player && !Transporter.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead) || player == TransportPlayer1;

    public bool TransUsable() => IsTrans;

    public string TransLabel()
    {
        if (TransportPlayer1 && TransportPlayer2)
            return "TRANSPORT";
        else if (TransportPlayer1)
            return "SECOND TARGET";
        else
            return "FIRST TARGET";
    }

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

            if (!Player1Body)
                yield break;
        }

        if (TransportPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(TransportPlayer2.PlayerId);

            if (!Player2Body)
                yield break;
        }

        if (TransportPlayer1.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            TransportPlayer1.MyPhysics.ExitAllVents();
            Vent1 = TransportPlayer1.GetClosestVent();
            WasInVent1 = true;
        }

        if (TransportPlayer2.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            TransportPlayer2.MyPhysics.ExitAllVents();
            Vent2 = TransportPlayer2.GetClosestVent();
            WasInVent2 = true;
        }

        Transporting = true;

        if (!TransportPlayer1.HasDied())
        {
            TransportPlayer1.moveable = false;
            TransportPlayer1.NetTransform.Halt();
        }

        if (!TransportPlayer2.HasDied())
        {
            TransportPlayer2.moveable = false;
            TransportPlayer2.NetTransform.Halt();
        }

        if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
            Flash(Color, Transporter.TransportDur);

        if (!Player1Body && !WasInVent1)
            AnimateTransport1();

        if (!Player2Body && !WasInVent2)
            AnimateTransport2();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < Transporter.TransportDur)
                yield return EndFrame();
            else
                break;

            if (Meeting())
            {
                AnimationPlaying1.sprite = AnimationPlaying2.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (!Player1Body && !Player2Body)
        {
            TransportPlayer1.MyPhysics.ResetMoveState();
            TransportPlayer2.MyPhysics.ResetMoveState();
            var TempPosition = TransportPlayer1.GetTruePosition();
            TransportPlayer1.CustomSnapTo(new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
            TransportPlayer2.CustomSnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

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

            if (TransportPlayer1.CanVent() && Vent2 && WasInVent2)
                TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

            if (TransportPlayer2.CanVent() && Vent1 && WasInVent1)
                TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
        }
        // TODO: Merge the next two cases
        else if (Player1Body && !Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            TransportPlayer2.MyPhysics.ResetMoveState();
            var TempPosition = Player1Body.TruePosition;
            Player1Body.transform.position = TransportPlayer2.GetTruePosition();
            TransportPlayer2.CustomSnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer2)
            {
                ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (!Player1Body && Player2Body)
        {
            StopDragging(Player2Body.ParentId);
            TransportPlayer1.MyPhysics.ResetMoveState();
            var TempPosition = TransportPlayer1.GetTruePosition();
            TransportPlayer1.CustomSnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
            Player2Body.transform.position = TempPosition;

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer1)
            {
                ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body && Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            StopDragging(Player2Body.ParentId);
            (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
        }

        if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (MapPatch.MapActive)
                Map().Close();
        }

        TransportPlayer1 = null;
        TransportPlayer2 = null;
        Transporting = false;
        yield break;
    }

    public void AnimateTransport1()
    {
        Transport1.transform.position = new(TransportPlayer1.GetTruePosition().x, TransportPlayer1.GetTruePosition().y + 0.35f, (TransportPlayer1.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying1.flipX = TransportPlayer1.MyRend().flipX;
        AnimationPlaying1.transform.localScale *= 0.9f * TransportPlayer1.GetModifiedSize();

        Coroutines.Start(PerformTimedAction(Transporter.TransportDur, p =>
        {
            var index = (int)(p * PortalAnimation.Count);
            index = Mathf.Clamp(index, 0, PortalAnimation.Count - 1);
            AnimationPlaying1.sprite = PortalAnimation[index];
            TransportPlayer1.SetPlayerMaterialColors(AnimationPlaying1);

            if (p == 1)
                AnimationPlaying1.sprite = PortalAnimation[0];
        }));
    }

    public void AnimateTransport2()
    {
        Transport2.transform.position = new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.35f, (TransportPlayer2.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying2.flipX = TransportPlayer2.MyRend().flipX;
        AnimationPlaying2.transform.localScale *= 0.9f * TransportPlayer2.GetModifiedSize();

        Coroutines.Start(PerformTimedAction(Transporter.TransportDur, p =>
        {
            var index = (int)(p * PortalAnimation.Count);
            index = Mathf.Clamp(index, 0, PortalAnimation.Count - 1);
            AnimationPlaying2.sprite = PortalAnimation[index];
            TransportPlayer2.SetPlayerMaterialColors(AnimationPlaying2);

            if (p == 1)
                AnimationPlaying2.sprite = PortalAnimation[0];
        }));
    }

    public void Click1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            TransportPlayer1 = player;
        else
            TransportButton.StartCooldown(cooldown);
    }

    public void Click2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            TransportPlayer2 = player;
        else
            TransportButton.StartCooldown(cooldown);
    }

    public void Transport()
    {
        if (!TransportPlayer1)
        {
            TransportMenu1.Open();
            TransportButton.Uses++;
        }
        else if (!TransportPlayer2)
        {
            TransportMenu2.Open();
            TransportButton.Uses++;
        }
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Transport, TransportPlayer1, TransportPlayer2);
            Coroutines.Start(TransportPlayers());
            TransportButton.StartCooldown();
        }
    }

    // Bastion Stuff
    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; set; }
    public bool IsBast => RevivedRole?.Type == LayerEnum.Bastion;

    public bool BastException(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb()
    {
        var cooldown = Interact(Player, BombButton.GetTarget<Vent>());

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(BombButton.GetTarget<Vent>().Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Bomb, BombButton.GetTarget<Vent>());
        }

        BombButton.StartCooldown(cooldown);
    }

    public bool BastUsable() => IsBast;

    // Trapper Stuff
    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; set; }
    private List<Role> TriggeredRoles { get; set; }
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }
    public bool IsTrap => RevivedRole?.Type == LayerEnum.Trapper;

    private void StartBuildling()
    {
        BuildButton.Begin();
        Building = true;
    }

    private void EndBuildling()
    {
        TrapButton.Uses++;
        TrapsMade++;
        Building = false;
    }

    private void SetTrap()
    {
        var cooldown = Interact(Player, TrapButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Place, TrapButton.GetTarget<PlayerControl>().PlayerId);
            Trapped.Add(TrapButton.GetTarget<PlayerControl>().PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool TrapException(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (!isAttack)
        {
            TriggeredRoles.Add(trigger.GetRole());
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Trigger, trapped, trigger, isAttack);
        }
        else
            AttackedSomeone = true;

        Trapped.Remove(trapped.PlayerId);
    }

    public bool TrapUsable1() => IsTrap;

    public bool TrapUsable2() => IsTrap && TrapsMade < Trapper.MaxTraps;
}