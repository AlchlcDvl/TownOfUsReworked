namespace TownOfUsReworked.PlayerLayers.Roles;

// TODO: Change how this works by using its substituted roles rather than copy pasted code
public sealed class Retributionist : CSupport, IShielder, IVentBomber, ITrapper, IAlerter, IMover, IBlocker, IExaminer, IBugger, IShaman
{
    public override void Init()
    {
        base.Init();
        BodyArrows.Clear();
        MediatedPlayers.Clear();
        Bugs.Clear();
        TrackerArrows.Clear();
        Reported.Clear();
        ReferenceBodies.Clear();
        BombedIDs.Clear();
        BuggedPlayers.Clear();
        Trapped.Clear();
        TriggeredRoles.Clear();
        MediateArrows.Clear();
        Selected = null;
        BlockTarget = null;
        RetMenu = new(Player, "RetActive", "RetDisabled", SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
    }

    // Retributionist Stuff
    private PlayerVoteArea Selected;
    private PlayerControl Revived;
    private Role RevivedRole => Revived ? (Revived.Is<Revealer>(out var rev) ? rev.FormerRole : Revived.GetRole()) : null;
    public CustomMeeting RetMenu;
    private bool ClickedAgain;

    protected override UColor MainColor => RevivedRole?.Color ?? CustomColorManager.Retributionist;
    public override Layer Type => Layer.Retributionist;
    public override string StartText => "Mimic the Dead";
    public override string Description => "- You can mimic the abilities of dead <#8CFFFFFF>Crew</color>" + (RevivedRole ? $"\n{RevivedRole.Description}" : "");
    public override Attack Attack
    {
        get
        {
            if (IsBast || (IsVet && AlertButton.EffectActive))
                return Attack.Powerful;

            return IsVig ? Attack.Basic : Attack.None;
        }
    }
    public override Defense Defense
    {
        get
        {
            if (IsVet && AlertButton.EffectActive)
                return Defense.Basic;

            return Defense.None;
        }
    }
    public override bool RoleBlockImmune => RevivedRole?.RoleBlockImmune ?? false;

    private void ClickAgain() => ClickedAgain = true;

    public override void Reset(bool meeting, bool start)
    {
        BuggedPlayers.Clear();
        BlockTarget = null;
        MediateArrows.Values.DestroyAll();
        MediateArrows.Clear();
        MediatedPlayers.Clear();

        if (Operative.BugsRemoveOnNewRound && meeting)
        {
            Bugs.ForEach(x => x?.gameObject?.Destroy());
            Bugs.Clear();
        }

        if (Tracker.ResetOnNewRound)
        {
            TrackerArrows.Values.DestroyAll();
            TrackerArrows.Clear();
            TrackButton.Uses = TrackButton.MaxUses;
        }
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (ShieldedPlayer == player && Medic.ShowShielded == ShieldOptions.Medic)
            name += " <#006600FF>✚</color>";

        if (Trapped.Contains(player.PlayerId))
            name += " <#BE1C8CFF>∮</color>";

        if (!Reported.Contains(player.PlayerId) || revealed || !meeting)
            return;

        var role = handler.CurrentRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
    }

    protected override void Deinit()
    {
        if (!Bugs.Any())
            return;

        Bugs.ForEach(x => x?.gameObject?.Destroy());
        Bugs.Clear();
    }

    protected override void ClearArrows()
    {
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();

        MediateArrows.Values.DestroyAll();
        MediateArrows.Clear();
        MediatedPlayers.Clear();

        TrackerArrows.Values.DestroyAll();
        TrackerArrows.Clear();
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer) => ClearArrows();

    private void DestroyArrow(byte targetPlayerId)
    {
        if (IsCor)
        {
            if (BodyArrows.Remove(targetPlayerId, out var arrow))
                arrow.Destroy();
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return !voteArea.AmDead || !player.HasDied() || Dead || !player.Is<Crew>();
    }

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
        if (IsCor)
        {
            var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= Coroner.CoronerArrowDur));
            BodyArrows.Keys.Where(bodyArrow => validBodies.All(x => x.ParentId != bodyArrow)).Do(DestroyArrow);

            foreach (var body in validBodies)
            {
                if (!BodyArrows.ContainsKey(body.ParentId))
                    BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
            }
        }
        else if (IsTrans)
        {
            if (!KeyboardJoystick.player.GetButtonDown("Delete"))
                return;

            if (!Moving && TransportMenu.Selected.Count > 0)
                TransportMenu.Selected.TakeLast();

            Message("Removed a target");
        }
    }

    public override void UpdatePlayer(PlayerControl __instance)
    {
        if (Dead)
            return;

        if (IsMed)
        {
            if (!MediateArrows.TryGetValue(__instance.PlayerId, out var arrow))
                return;

            arrow?.Update(__instance.GetPlayerColor(false, !Medium.ShowMediatePlayer));

            if (!Medium.ShowMediatePlayer)
                __instance.OverrideOutfit(CamoOutfit(__instance), CustomPlayerOutfitType.Camouflage);
        }
    }

    public override void ReadRPC(RpcReader reader)
    {
        var retAction = reader.Read<RetActionsRpc>();

        switch (retAction)
        {
            case RetActionsRpc.Revive:
            {
                Revived = reader.ReadPlayer();
                break;
            }
            case RetActionsRpc.Transport:
            {
                Coroutines.Start(Transporter.TransportPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));
                break;
            }
            case RetActionsRpc.Shield:
            {
                ShieldedPlayer = reader.ReadPlayer();
                break;
            }
            case RetActionsRpc.Mediate:
            {
                var playerid2 = reader.ReadByte();
                MediatedPlayers.Add(playerid2);

                if (LocalPlayer.PlayerId == playerid2 || (LocalPlayer.HasDied() && Medium.ShowMediumToDead == ShowMediumToDead.AllDead))
                    LayerHandler.Handlers[LocalPlayer.PlayerId].DeadArrows.Add(PlayerId, new(LocalPlayer, Player, Color, skipBody: true));

                break;
            }
            case RetActionsRpc.Roleblock:
            {
                BlockTarget = reader.ReadPlayer();

                if (BlockTarget.AmOwner)
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);

                break;
            }
            case RetActionsRpc.Bomb:
            {
                BombedIDs.Add(reader.ReadInt());
                break;
            }
            case RetActionsRpc.Place:
            {
                Trapped.Add(reader.ReadByte());
                break;
            }
            case RetActionsRpc.Trigger:
            {
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBool());
                break;
            }
            case RetActionsRpc.AltRevive:
            {
                ParentId = reader.ReadByte();

                if (LocalPlayer.PlayerId == ParentId)
                    Flash(CustomColorManager.Altruist, Altruist.ReviveDur);

                if (Altruist.AltruistTargetBody)
                    BodyById(ParentId).gameObject.Destroy();

                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {retAction}");
                break;
            }
        }
    }

    public override void UpdateMeeting(MeetingHud __instance) => RetMenu.Update();

    public override void VoteComplete(MeetingHud __instance)
    {
        RetMenu.HideButtons();

        if (!Selected || Dead)
            return;

        Revived = PlayerByVoteArea(Selected);
        PerformRpcAction(RetActionsRpc.Revive, Selected);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        RetMenu.GenButtons(__instance);
        Selected = null;

        if (IsOp)
        {
            string message;

            if (BuggedPlayers.Count == 0)
                message = "No one triggered your bugs.";
            else if (BuggedPlayers.Count < Operative.MinAmountOfPlayersInBug)
                message = "Not enough players triggered your bugs.";
            else if (BuggedPlayers.Count == 1)
            {
                var result = BuggedPlayers[0];
                var aAn = result is Layer.Altruist or Layer.Engineer or Layer.Escort or Layer.Operative or Layer.Amnesiac or Layer.Actor or Layer.Arsonist or
                    Layer.Executioner or Layer.Ambusher or Layer.Enforcer or Layer.Impostor or Layer.Anarchist ? "n" : "";
                message = $"A{aAn} {result} triggered your bug.";
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
                message += Join(", ", BuggedPlayers.Select(role => LayerDictionary[role].Name));
            }

            if (!IsNullEmptyOrWhiteSpace(message))
                Run("<#8D0F8CFF>〖 Bug Results 〗</color>", message);
        }
        else if (IsTrap)
        {
            var message = "";

            if (AttackedSomeone)
                message = "Your trap attacked someone!";
            else if (TriggeredRoles.Any())
            {
                message = "Your trap detected the following roles: ";
                TriggeredRoles.Shuffle();
                message += Join(", ", TriggeredRoles.Select(x => LayerDictionary[x].Name));
            }

            if (!IsNullEmptyOrWhiteSpace(message))
                Run("<#8D0F8CFF>〖 Trap Triggers 〗</color>", message);

            TriggeredRoles.Clear();
        }
        else if (IsAlt)
            ReviveButton.Uses += Altruist.PassiveAltManaGain;

        Revived = null;
    }

    public override void OnBodyReport(NetworkedPlayerInfo info)
    {
        if (IsCor)
        {
            if (!KilledPlayers.TryFinding(x => x.PlayerId == info.PlayerId, out var body))
                return;

            Reported.Add(info.PlayerId);
            var reportMsg = body.ParseBodyReport(Player);

            if (!IsNullEmptyOrWhiteSpace(reportMsg))
                Run("<#8D0F8CFF>〖 Autopsy Results 〗</color>", reportMsg);
        }
    }

    public void OnRoleSelected()
    {
        if (!Revived)
            return;

        if (IsMys)
        {
            RevealButton ??= new(this, "REVEAL", new SpriteName("MysticReveal"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Reveal, (UsableFunc)MysUsable,
                (PlayerBodyExclusion)MysticException, new Cooldown(Mystic.RevealCd));
        }
        else if (IsCor)
        {
            AutopsyButton ??= new(this, "AUTOPSY", new SpriteName("Autopsy"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Autopsy, (UsableFunc)CorUsable1,
                new Cooldown(Coroner.AutopsyCd));
            CompareButton ??= new(this, "COMPARE", new SpriteName("Compare"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Compare, new Cooldown(Coroner.CompareCd),
                (UsableFunc)CorUsable2);
        }
        else if (IsDet)
        {
            ExamineButton ??= new(this, "EXAMINE", new SpriteName("Examine"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Examine, (UsableFunc)DetUsable,
                new Cooldown(Detective.ExamineCd));
        }
        else if (IsMed)
        {
            MediateButton ??= new(this, "MEDIATE", new SpriteName("Mediate"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Mediate, (UsableFunc)MedUsable,
                new Cooldown(Medium.MediateCd));
        }
        else if (IsOp)
        {
            BugButton ??= new(this, "BUG", new SpriteName("Bug"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)PlaceBug, new Cooldown(Operative.BugCd),
                Operative.MaxBugs, (ConditionFunc)OpCondition, (UsableFunc)OpUsable);
        }
        else if (IsSeer)
        {
            SeerButton ??= new(this, "ENVISION", new SpriteName("Seer"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)See, new Cooldown(Seer.SeerCd),
                (UsableFunc)SeerUsable);
        }
        else if (IsSher)
        {
            InterrogateButton ??= new(this, "INTERROGATE", new SpriteName("Interrogate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Interrogate, (UsableFunc)SherUsable,
                new Cooldown(Sheriff.InterrogateCd), (PlayerBodyExclusion)SherException);
        }
        else if (IsTrack)
        {
            TrackButton ??= new(this, "TRACK", new SpriteName("Track"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Track, new Cooldown(Tracker.TrackCd),
                (UsableFunc)TrackUsable, (PlayerBodyExclusion)TrackException, Tracker.MaxTracks);
        }
        else if (IsBast)
        {
            BombButton ??= new(this, "PLACE BOMB", new SpriteName($"{Bastion.SpriteName}VentBomb"), AbilityTypes.Vent, KeybindType.ActionSecondary, (OnClickVent)Bomb,
                (VentExclusion)BastException, new Cooldown(Bastion.BastionCd), Bastion.MaxBombs, (UsableFunc)BastUsable);
        }
        else if (IsVet)
        {
            AlertButton ??= new(this, "ALERT", new SpriteName("Alert"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Alert, (UsableFunc)VetUsable,
                Veteran.MaxAlerts, new Cooldown(Veteran.AlertCd), new Duration(Veteran.AlertDur), (EndFunc)AlertEnd);
        }
        else if (IsVig)
        {
            ShootButton ??= new(this, "SHOOT", new SpriteName("Shoot"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Shoot, new Cooldown(Vigilante.ShootCd),
                Vigilante.MaxBullets, (PlayerBodyExclusion)VigiException, (UsableFunc)VigUsable);
        }
        else if (IsAlt)
        {
            var wasNull = ReviveButton is null;
            ReviveButton ??= new(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Revive, new Cooldown(Altruist.ReviveCd),
                (EndFunc)ReviveEnd, new Duration(Altruist.ReviveDur), (EffectEndVoid)UponEnd, Altruist.MaxAltMana, (UsableFunc)AltUsable);
            ManaButton ??= new(this, "GAIN MANA", new SpriteName("AltManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(Altruist.AltManaCd),
                (UsableFunc)AltUsable1);

            if (wasNull)
                ReviveButton.UsesCount = 0;
        }
        else if (IsMedic)
        {
            ShieldButton ??= new(this, "SHIELD", new SpriteName("Shield"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Protect, (PlayerBodyExclusion)MedicException,
                (UsableFunc)MedicUsable);
        }
        else if (IsTrap)
        {
            var wasNull = TrapButton is null;
            BuildButton ??= new(this, "BUILD TRAP", new SpriteName("Build"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)StartBuilding, (UsableFunc)BuildUsable,
                new Cooldown(Trapper.BuildCd), new Duration(Trapper.BuildDur), (EffectEndVoid)EndBuilding, new CanClickAgain(false));
            TrapButton ??= new(this, "PLACE TRAP", new SpriteName("Trap"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetTrap, (UsableFunc)TrapUsable, Trapper.MaxTraps,
                new Cooldown(Trapper.TrapCd), (PlayerBodyExclusion)TrapException);

            if (wasNull)
                TrapsMade = TrapButton.UsesCount = 0;
        }
        else if (IsCham)
        {
            SwoopButton ??= new(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Swoop, (UsableFunc)ChamUsable,
                (EffectEndVoid)UnInvis, new Cooldown(Chameleon.SwoopCd), new Duration(Chameleon.SwoopDur), (EffectVoid)Invis, (EndFunc)SwoopEnd, Chameleon.MaxSwoops, (ClickedAgainVoid)ClickAgain);
        }
        else if (IsEngi)
        {
            FixButton ??= new(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Fix, Engineer.MaxFixes,
                (ConditionFunc)Engineer.Condition, new Cooldown(Engineer.FixCd), (UsableFunc)EngiUsable);
        }
        else if (IsEsc)
        {
            BlockButton ??= new(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Roleblock, new Cooldown(Escort.EscortCd),
                (EffectEndVoid)UnBlock, new Duration(Escort.EscortDur), (EndFunc)BlockEnd, (EffectStartVoid)BlockStart, (UsableFunc)EscUsable);
        }
        else if (IsTrans)
        {
            var wasNull = TransportButton is null;
            TransportButton ??= new(this, new SpriteName("Transport"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Transport, Transporter.MaxTransports,
                (LabelFunc)TransLabel, new Cooldown(Transporter.TransportCd), (UsableFunc)TransUsable);

            if (wasNull && TransportMenu is null)
                TransportMenu = new(Player, Click, Color, TransException);
        }

        Player.ResetButtons();
    }

    // Coroner Stuff
    private readonly Dictionary<byte, PositionalArrow> BodyArrows = [];
    private CustomButton AutopsyButton;
    private CustomButton CompareButton;
    private readonly List<DeadPlayer> ReferenceBodies = [];
    private readonly HashSet<byte> Reported = [];
    private bool IsCor => RevivedRole is Coroner;

    private void Autopsy(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == target.ParentId));
        AutopsyButton.StartCooldown();
    }

    private void Compare(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(ReferenceBodies.Any(x => target.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        CompareButton.StartCooldown(cooldown);
    }

    private bool CorUsable1() => IsCor;

    private bool CorUsable2() => ReferenceBodies.Any() && IsCor;

    // Detective Stuff
    private CustomButton ExamineButton;
    private bool IsDet => RevivedRole is Detective;

    private void Examine(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Flash(target.IsFramed() || KilledPlayers.Any(x => x.KillerId == target.PlayerId && x.KillAge <= Detective.RecentKill) ? UColor.red : UColor.green);
            target.EnsureComponent<FootprintHandler>();
        }

        ExamineButton.StartCooldown(cooldown);
    }

    private bool DetUsable() => IsDet;

    // Medium Stuff
    public HashSet<byte> MediatedPlayers { get; } = [];
    private readonly Dictionary<byte, PlayerArrow> MediateArrows = [];
    private CustomButton MediateButton;
    private bool IsMed => RevivedRole is Medium;

    private void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.Clone();

        if (!playersDead.Any())
            return;

        var bodies = AllBodies();

        switch (Medium.DeadRevealed)
        {
            case DeadRevealed.Random:
            {
                MediatePlayer(playersDead.Random(), bodies);
                break;
            }
            case DeadRevealed.Everyone:
            {
                playersDead.Do(x => MediatePlayer(x, bodies));
                break;
            }
            default:
            {
                if (Medium.DeadRevealed == DeadRevealed.Newest)
                    playersDead = playersDead.Reverse();

                MediatePlayer(playersDead.First(), bodies);
                break;
            }
        }
    }

    private void MediatePlayer(DeadPlayer dead, IEnumerable<DeadBody> bodies)
    {
        if (!bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            return;

        MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
        MediatedPlayers.Add(dead.PlayerId);
        PerformRpcAction(RetActionsRpc.Mediate, dead.PlayerId);
    }

    private bool MedUsable() => IsMed;

    // Operative Stuff
    private readonly List<Bug> Bugs = [];
    public List<Layer> BuggedPlayers { get; } = [];
    private CustomButton BugButton;
    public bool IsOp => RevivedRole is Operative;

    private void PlaceBug()
    {
        Bugs.Add(Bug.CreateBug(Player));
        BugButton.StartCooldown();
    }

    private bool OpUsable() => IsOp;

    private bool OpCondition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    // Sheriff Stuff
    private CustomButton InterrogateButton;
    private bool IsSher => RevivedRole is Sheriff;

    private void Interrogate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(target.SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    private bool SherException(PlayerControl player) => Player.KnowsRoleOf(player);

    private bool SherUsable() => IsSher;

    // Tracker Stuff
    private readonly Dictionary<byte, PlayerArrow> TrackerArrows = [];
    private CustomButton TrackButton;
    private bool IsTrack => RevivedRole is Tracker;

    private void Track(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(), Tracker.UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    private bool TrackException(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    private bool TrackUsable() => IsTrack;

    // Vigilante Stuff
    private CustomButton ShootButton;
    private bool IsVig => RevivedRole is Vigilante;

    private void Shoot(PlayerControl target) => ShootButton.StartCooldown(Interact(Player, target, true));

    private bool VigiException(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool VigUsable() => IsVig;

    // Veteran Stuff
    public CustomButton AlertButton { get; private set; }
    private bool IsVet => RevivedRole is Veteran;

    private void Alert() => AlertButton.TriggerRpcAndBegin();

    private bool VetUsable() => IsVet;

    private bool AlertEnd() => Dead;

    // Altruist Stuff
    private CustomButton ReviveButton;
    private CustomButton ManaButton;
    private byte ParentId;
    private bool IsAlt => RevivedRole is Altruist;

    private bool AltUsable() => IsAlt;

    private bool AltUsable1() => AltUsable() && ReviveButton.UsesCount != ReviveButton.Max;

    private void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    private bool ReviveEnd() => Dead;

    private void FinishRevive()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead || !LayerHandler.Handlers.TryGetValue(player.PlayerId, out var targetHandler))
            return;

        targetHandler.DeathReason = DeathReasonEnum.Revived;
        targetHandler.KilledBy = " By " + PlayerName;
        player.Revive();

        if (Lovers.BothLoversDie && player.Is<Lovers>(out var lovers) && LayerHandler.Handlers.TryGetValue(lovers.Other.PlayerId, out var loverHandler))
        {
            loverHandler.DeathReason = DeathReasonEnum.Revived;
            loverHandler.KilledBy = " By " + PlayerName;
            lovers.Other.Revive();

            if (Local && lovers.Other.Is<Sovereign>(out var loverSov) && !loverSov.Revealed)
                CustomAchievementManager.UnlockAchievement("RekindledPower");
        }

        if (Local && player.Is<Sovereign>(out var sov) && !sov.Revealed)
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    private void Revive(DeadBody target)
    {
        ParentId = target.ParentId;
        Spread(Player, PlayerByBody(target));
        ReviveButton.TriggerRpcAndBegin(RetActionsRpc.AltRevive, ParentId);
        Flash(Color, Altruist.ReviveDur);

        if (Altruist.AltruistTargetBody)
            target?.gameObject.Destroy();
    }

    private void GainMana(DeadBody target)
    {
        ReviveButton.Uses += Altruist.AltManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(ActionsRpc.FadeBody, target);
        FadeBody(target);
    }

    // Medic Stuff
    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    private CustomButton ShieldButton;
    private bool IsMedic => RevivedRole is Medic;

    private void Protect(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ShieldedPlayer = ShieldedPlayer ? null : target;
            PerformRpcAction(RetActionsRpc.Shield, ShieldedPlayer?.PlayerId ?? 255);
        }

        ShieldButton.StartCooldown(cooldown);
    }

    private bool MedicException(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;

        return player.Is<Sovereign>(out var irev) && irev.Revealed;
    }

    private bool MedicUsable() => !ShieldBroken && IsMedic;

    // Chameleon Stuff
    private CustomButton SwoopButton;
    private bool IsCham => RevivedRole is Chameleon;

    private void Invis() => MiscUtils.Invis(Player, Chameleon.SwoopDur, SwoopEnd);

    private void UnInvis() => ClickedAgain = false;

    private bool SwoopEnd() => Dead || ClickedAgain;

    private void Swoop() => SwoopButton.TriggerRpcAndBegin();

    private bool ChamUsable() => IsCham;

    // Engineer Stuff
    private CustomButton FixButton;
    private bool IsEngi => RevivedRole is Engineer;

    private void Fix()
    {
        FixUtils.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }

    private bool EngiUsable() => IsEngi && Engineer.MaxFixes > 0;

    // Mystic Stuff
    private CustomButton RevealButton;
    private bool IsMys => RevivedRole is Mystic;

    private void Reveal(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash((!target.Is(Handler.CurrentFaction) && !target.Is(Faction.Crew)) || target.IsFramed() ? UColor.red : UColor.green);

        RevealButton.StartCooldown(cooldown);
    }

    private bool MysticException(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool MysUsable() => IsMys;

    // Seer Stuff
    private CustomButton SeerButton;
    private bool IsSeer => RevivedRole is Seer;

    private void See(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(LayerHandler.Handlers[target.PlayerId].History.Any() || target.IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    private bool SeerUsable() => IsSeer;

    // Escort Stuff
    public PlayerControl BlockTarget { get; private set; }
    private CustomButton BlockButton;
    private bool IsEsc => RevivedRole is Escort;

    private void UnBlock()
    {
        if (BlockTarget.AmOwner)
            BlockExposed = false;

        BlockTarget = null;
    }

    private void BlockStart()
    {
        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }

    private void Roleblock(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = target;
            BlockButton.TriggerRpcAndBegin(BlockTarget);
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    private bool BlockEnd() => Dead || BlockTarget.HasDied();

    private bool EscUsable() => IsEsc;

    // Transporter Stuff
    private CustomButton TransportButton;
    private CustomPlayerMenu TransportMenu;
    public bool Moving { get; set; }
    private bool IsTrans => RevivedRole is Transporter;

    private bool TransException(PlayerControl player) => (player == Player && !Transporter.TransSelf) || UninteractablePlayers.ContainsKey(player.PlayerId) || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead);

    private bool TransUsable() => IsTrans;

    private string TransLabel() => TransportMenu.Selected.Count switch
    {
        < 2 => "SELECT TARGETS",
        _ =>  "TRANSPORT"
    };

    private bool Click(PlayerControl player, out bool shouldClose)
    {
        shouldClose = false;

        if (player.IsMoving())
            return false;

        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            return true;

        TransportButton.StartCooldown(cooldown);
        shouldClose = true;
        return false;
    }

    private void Transport()
    {
        if (TransportMenu.Selected.Count < 2)
        {
            TransportMenu.Open();
            TransportButton.Uses++;
        }
        else
        {
            PerformRpcAction(RetActionsRpc.Transport, TransportMenu.Selected[0], TransportMenu.Selected[1]);
            Coroutines.Start(Transporter.TransportPlayers(PlayerById(TransportMenu.Selected[0]), PlayerById(TransportMenu.Selected[1]), this));
            TransportMenu.Selected.Clear();
            TransportButton.StartCooldown();
        }
    }

    // Bastion Stuff
    private CustomButton BombButton;
    public List<int> BombedIDs { get; } = [];
    private bool IsBast => RevivedRole is Bastion;

    private bool BastException(Vent vent) => BombedIDs.Contains(vent.Id);

    private void Bomb(Vent target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(target.Id);
            PerformRpcAction(RetActionsRpc.Bomb, target);
        }

        BombButton.StartCooldown(cooldown);
    }

    private bool BastUsable() => IsBast;

    // Trapper Stuff
    public bool Building { get; private set; }
    public HashSet<byte> Trapped { get; } = [];
    private CustomButton BuildButton;
    private CustomButton TrapButton;
    private readonly List<Layer> TriggeredRoles = [];
    private int TrapsMade;
    private bool AttackedSomeone;
    private bool IsTrap => RevivedRole is Trapper;

    private void StartBuilding()
    {
        BuildButton.Begin();
        Building = true;
    }

    private void EndBuilding()
    {
        TrapButton.Uses++;
        TrapsMade++;
        Building = false;
    }

    private void SetTrap(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            PerformRpcAction(RetActionsRpc.Place, target.PlayerId);
            Trapped.Add(target.PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool TrapException(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (trigger.AmOwner)
            PerformRpcAction(RetActionsRpc.Trigger, trapped, trigger, isAttack);

        TriggeredRoles.Add(trigger.GetRole().Type);
        AttackedSomeone = isAttack;
        Trapped.Remove(trapped.PlayerId);
    }

    private bool TrapUsable() => IsTrap;

    private bool BuildUsable() => TrapUsable() && TrapsMade <= Trapper.MaxTraps;
}