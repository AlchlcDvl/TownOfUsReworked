namespace TownOfUsReworked.PlayerLayers.Roles;

public class Retributionist : Crew, IShielder, IVentBomber, ITrapper, IAlerter, ITransporter
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
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
    public PlayerVoteArea Selected { get; set; }
    public PlayerControl Revived { get; set; }
    public Role RevivedRole => Revived ? (Revived.TryGetLayer<Revealer>(out var rev) ? rev.FormerRole : Revived.GetRole()) : null;
    public CustomMeeting RetMenu { get; set; }

    public override UColor Color
    {
        get
        {
            if (ClientOptions.CustomCrewColors)
                return RevivedRole?.Color ?? CustomColorManager.Retributionist;
            else
                return CustomColorManager.Crew;
        }
    }
    public override LayerEnum Type => LayerEnum.Retributionist;
    public override Func<string> StartText => () => "Mimic the Dead";
    public override Func<string> Description => () => "- You can mimic the abilities of dead <#8CFFFFFF>Crew</color>" + (RevivedRole ? $"\n{RevivedRole.Description()}" : "");
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
    public override bool RoleBlockImmune => RevivedRole?.RoleBlockImmune ?? false;

    public override void Deinit()
    {
        base.Deinit();

        if (Bugs.Count > 0)
        {
            Bugs.ForEach(x => x?.gameObject?.Destroy());
            Bugs.Clear();
        }
    }

    public override void ClearArrows()
    {
        base.ClearArrows();

        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();

        MediateArrows.Values.DestroyAll();
        MediateArrows.Clear();
        MediatedPlayers.Clear();

        TrackerArrows.Values.DestroyAll();
        TrackerArrows.Clear();
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);
        ClearArrows();
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        if (IsCor)
        {
            if (BodyArrows.TryGetValue(targetPlayerId, out var arrow))
            {
                arrow.Destroy();
                BodyArrows.Remove(targetPlayerId);
            }
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
        base.UpdateHud(__instance);

        if (IsCor)
        {
            var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= Coroner.CoronerArrowDur));

            foreach (var bodyArrow in BodyArrows.Keys)
            {
                if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    DestroyArrow(bodyArrow);
            }

            foreach (var body in validBodies)
            {
                if (!BodyArrows.ContainsKey(body.ParentId))
                    BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
            }
        }
        else if (IsTrans)
        {
            if (KeyboardJoystick.player.GetButtonDown("Delete"))
            {
                if (!Transporting && TransportMenu.Selected.Count > 0)
                    TransportMenu.Selected.TakeLast();

                Message("Removed a target");
            }
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
            {
                __instance.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(__instance));
                PlayerMaterial.SetColors(UColor.grey, __instance.MyRend());
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var retAction = reader.ReadEnum<RetActionsRPC>();

        switch (retAction)
        {
            case RetActionsRPC.Revive:
            {
                Revived = reader.ReadPlayer();
                break;
            }
            case RetActionsRPC.Transport:
            {
                Coroutines.Start(Transporter.TransportPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));
                break;
            }
            case RetActionsRPC.Shield:
            {
                ShieldedPlayer = reader.ReadPlayer();
                break;
            }
            case RetActionsRPC.Mediate:
            {
                var playerid2 = reader.ReadByte();
                MediatedPlayers.Add(playerid2);

                if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.Dead && Medium.ShowMediumToDead == ShowMediumToDead.AllDead))
                    CustomPlayer.Local.GetRole().DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Player, Color, skipBody: true));

                break;
            }
            case RetActionsRPC.Roleblock:
            {
                BlockTarget = reader.ReadPlayer();
                break;
            }
            case RetActionsRPC.Bomb:
            {
                BombedIDs.Add(reader.ReadInt32());
                break;
            }
            case RetActionsRPC.Place:
            {
                Trapped.Add(reader.ReadByte());
                break;
            }
            case RetActionsRPC.Trigger:
            {
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                break;
            }
            case RetActionsRPC.AltRevive:
            {
                ParentId = reader.ReadByte();

                if (CustomPlayer.Local.PlayerId == ParentId)
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

    public override void UpdateMeeting(MeetingHud __instance) => RetMenu.Update(__instance);

    public override void VoteComplete(MeetingHud __instance)
    {
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
                BuggedPlayers.ForEach(role => message += $"{LayerDictionary[role].Name}, ");
                message = message[..^2];
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
                TriggeredRoles.ForEach(x => message += $"{x}, ");
                message = message[..^2];
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
            body.Reporter = Player;
            var reportMsg = body.ParseBodyReport();

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
            var wasnull = ReviveButton == null;
            ReviveButton ??= new(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Revive, new Cooldown(Altruist.ReviveCd),
                (EndFunc)ReviveEnd, new Duration(Altruist.ReviveDur), (EffectEndVoid)UponEnd, Altruist.MaxAltMana, (UsableFunc)AltUsable);
            ManaButton ??= new(this, "GAIN MANA", new SpriteName("AltManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(Altruist.AltManaCd),
                (UsableFunc)AltUsable1);

            if (wasnull)
                ReviveButton.uses = 0;
        }
        else if (IsMedic)
        {
            ShieldButton ??= new(this, "SHIELD", new SpriteName("Shield"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Protect, (PlayerBodyExclusion)MedicException,
                (UsableFunc)MedicUsable);
        }
        else if (IsTrap)
        {
            var wasnull = TrapButton == null;
            BuildButton ??= new(this, "BUILD TRAP", new SpriteName("Build"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)StartBuildling, (UsableFunc)BuildUsable,
                new Cooldown(Trapper.BuildCd), new Duration(Trapper.BuildDur), (EffectEndVoid)EndBuildling, new CanClickAgain(false));
            TrapButton ??= new(this, "PLACE TRAP", new SpriteName("Trap"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetTrap, (UsableFunc)TrapUsable, Trapper.MaxTraps,
                new Cooldown(Trapper.TrapCd), (PlayerBodyExclusion)TrapException);

            if (wasnull)
                TrapsMade = TrapButton.uses = 0;
        }
        else if (IsCham)
        {
            SwoopButton ??= new(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Swoop, (UsableFunc)ChamUsable,
                (EffectEndVoid)UnInvis, new Cooldown(Chameleon.SwoopCd), new Duration(Chameleon.SwoopDur), (EffectVoid)Invis, (EndFunc)SwoopEnd, Chameleon.MaxSwoops);
        }
        else if (IsEngi)
        {
            FixButton ??= new(this, "FIX SABOTAGE", new SpriteName("Fix"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Fix, Engineer.MaxFixes,
                (ConditionFunc)EngiCondition, new Cooldown(Engineer.FixCd), (UsableFunc)EngiUsable);
        }
        else if (IsEsc)
        {
            BlockButton ??= new(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Roleblock, (EffectVoid)Block,
                (EffectEndVoid)UnBlock, new Cooldown(Escort.EscortCd), new Duration(Escort.EscortDur), (EndFunc)BlockEnd, (EffectStartVoid)BlockStart, (UsableFunc)EscUsable);
        }
        else if (IsTrans)
        {
            var wasnull = TransportButton == null;
            TransportButton ??= new(this, new SpriteName("Transport"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Transport, Transporter.MaxTransports,
                (LabelFunc)TransLabel, new Cooldown(Transporter.TransportCd), (UsableFunc)TransUsable);

            if (wasnull && TransportMenu == null)
                TransportMenu = new(Player, Click, TransException);
        }

        Player.ResetButtons();
    }

    // Coroner Stuff
    public Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];
    public CustomButton AutopsyButton { get; set; }
    public CustomButton CompareButton { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; } = [];
    public List<byte> Reported { get; } = [];
    public bool IsCor => RevivedRole is Coroner;

    public void Autopsy(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == target.ParentId));
        AutopsyButton.StartCooldown();
    }

    public void Compare(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(ReferenceBodies.Any(x => target.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        CompareButton.StartCooldown(cooldown);
    }

    public bool CorUsable1() => IsCor;

    public bool CorUsable2() => ReferenceBodies.Any() && IsCor;

    // Detective Stuff
    public CustomButton ExamineButton { get; set; }
    public bool IsDet => RevivedRole is Detective;

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Flash(target.IsFramed() || KilledPlayers.Any(x => x.KillerId == target.PlayerId && x.KillAge <= Detective.RecentKill) ? UColor.red : UColor.green);
            target.EnsureComponent<FootprintHandler>();
        }

        ExamineButton.StartCooldown(cooldown);
    }

    public bool DetUsable() => IsDet;

    // Medium Stuff
    public Dictionary<byte, PlayerArrow> MediateArrows { get; } = [];
    public CustomButton MediateButton { get; set; }
    public List<byte> MediatedPlayers { get; } = [];
    public bool IsMed => RevivedRole is Medium;

    public void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

        if (playersDead.Count == 0)
            return;

        var bodies = AllBodies();

        if (Medium.DeadRevealed != DeadRevealed.Random)
        {
            if (Medium.DeadRevealed == DeadRevealed.Newest)
                playersDead.Reverse();

            foreach (var dead in playersDead)
            {
                if (bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
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

            if (bodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, PlayerById(dead.PlayerId), Color, skipBody: true));
                MediatedPlayers.Add(dead.PlayerId);
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Mediate, dead.PlayerId);
            }
        }
    }

    public bool MedUsable() => IsMed;

    // Operative Stuff
    public List<Bug> Bugs { get; } = [];
    public List<LayerEnum> BuggedPlayers { get; } = [];
    public CustomButton BugButton { get; set; }
    public bool IsOp => RevivedRole is Operative;

    public void PlaceBug()
    {
        Bugs.Add(Bug.CreateBug(Player));
        BugButton.StartCooldown();
    }

    public bool OpUsable() => IsOp;

    public bool OpCondition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    // Sheriff Stuff
    public CustomButton InterrogateButton { get; set; }
    public bool IsSher => RevivedRole is Sheriff;

    public void Interrogate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(target.SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool SherException(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None))
        && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) && Rivals.RivalsRoles) || (player.Is<Mafia>() &&
        Player.Is<Mafia>() && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);

    public bool SherUsable() => IsSher;

    // Tracker Stuff
    public Dictionary<byte, PlayerArrow> TrackerArrows { get; } = [];
    public CustomButton TrackButton { get; set; }
    public bool IsTrack => RevivedRole is Tracker;

    public void Track(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(), Tracker.UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public bool TrackException(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    public bool TrackUsable() => IsTrack;

    // Vigilante Stuff
    public CustomButton ShootButton { get; set; }
    public bool IsVig => RevivedRole is Vigilante;

    public void Shoot(PlayerControl target) => ShootButton.StartCooldown(Interact(Player, target, true));

    public bool VigiException(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    public bool VigUsable() => IsVig;

    // Veteran Stuff
    public CustomButton AlertButton { get; set; }
    public bool IsVet => RevivedRole is Veteran;

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AlertButton);
        AlertButton.Begin();
    }

    public bool VetUsable() => IsVet;

    public bool AlertEnd() => Dead;

    // Altruist Stuff
    public CustomButton ReviveButton { get; set; }
    public CustomButton ManaButton { get; set; }
    public byte ParentId { get; set; }
    public bool IsAlt => RevivedRole is Altruist;

    public bool AltUsable() => IsAlt;

    public bool AltUsable1() => AltUsable() && ReviveButton.uses != ReviveButton.maxUses;

    public void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    public bool ReviveEnd() => Dead;

    private void FinishRevive()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (Lovers.BothLoversDie && player.TryGetLayer<Lovers>(out var lovers))
        {
            var lover = lovers.OtherLover;
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (Local && player.IIs<ISovereign>())
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    public void Revive(DeadBody target)
    {
        ParentId = target.ParentId;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ReviveButton, RetActionsRPC.AltRevive, ParentId);
        ReviveButton.Begin();
        Flash(Color, Altruist.ReviveDur);

        if (Altruist.AltruistTargetBody)
            target?.gameObject.Destroy();
    }

    public void GainMana(DeadBody target)
    {
        ReviveButton.Uses += Altruist.AltManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
    }

    // Medic Stuff
    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    public CustomButton ShieldButton { get; set; }
    public bool IsMedic => RevivedRole is Medic;

    public void Protect(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ShieldedPlayer = ShieldedPlayer ? null : target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Shield, ShieldedPlayer?.PlayerId ?? 255);
        }

        ShieldButton.StartCooldown(cooldown);
    }

    public bool MedicException(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;
        else
            return player.TryGetILayer<IRevealer>(out var irev) && irev.Revealed;
    }

    public bool MedicUsable() => !ShieldBroken && IsMedic;

    // Chameleon Stuff
    public CustomButton SwoopButton { get; set; }
    public bool IsCham => RevivedRole is Chameleon;

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
    public bool IsEngi => RevivedRole is Engineer;

    public void Fix()
    {
        FixExtentions.Fix();
        FixButton.StartCooldown(CooldownType.Start);
    }

    public bool EngiUsable() => IsEngi && Engineer.MaxFixes > 0;

    public bool EngiCondition() => Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;

    // Mystic Stuff
    public CustomButton RevealButton { get; set; }
    public bool IsMys => RevivedRole is Mystic;

    public void Reveal(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash((!target.Is(SubFaction) && SubFaction != SubFaction.None && !target.Is(Alignment.NeutralNeo)) || target.IsFramed() ? UColor.red : UColor.green);

        RevealButton.StartCooldown(cooldown);
    }

    public bool MysticException(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;

    public bool MysUsable() => IsMys;

    // Seer Stuff
    public CustomButton SeerButton { get; set; }
    public bool IsSeer => RevivedRole is Seer;

    public void See(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(target.GetRole().RoleHistory.Any() || target.IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    public bool SeerUsable() => IsSeer;

    // Escort Stuff
    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }
    public bool IsEsc => RevivedRole is Escort;

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget.GetButtons().ForEach(x => x.BlockExposed = false);

        if (BlockTarget.AmOwner)
            Blocked.BlockExposed = false;

        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

    public void BlockStart()
    {
        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }

    public void Roleblock(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, RetActionsRPC.Roleblock, BlockTarget);
            BlockButton.Begin();
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public bool BlockEnd() => Dead || BlockTarget.HasDied();

    public bool EscUsable() => IsEsc;

    // Transporter Stuff
    public CustomButton TransportButton { get; set; }
    public CustomPlayerMenu TransportMenu { get; set; }
    public bool Transporting { get; set; }
    public bool IsTrans => RevivedRole is Transporter;

    public bool TransException(PlayerControl player) => (player == Player && !Transporter.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead);

    public bool TransUsable() => IsTrans;

    public string TransLabel() => TransportMenu.Selected.Count switch
    {
        0 => "FIRST TARGET",
        1 => "SECOND TARGET",
        _ =>  "TRANSPORT"
    };

    public bool Click(PlayerControl player, out bool shouldClose)
    {
        var cooldown = Interact(Player, player);
        shouldClose = false;

        if (cooldown != CooldownType.Fail)
            return true;
        else
            TransportButton.StartCooldown(cooldown);

        shouldClose = true;
        return false;
    }

    public void Transport()
    {
        if (TransportMenu.Selected.Count < 2)
        {
            TransportMenu.Open();
            TransportButton.Uses++;
        }
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Transport, TransportMenu.Selected[0], TransportMenu.Selected[1]);
            Coroutines.Start(Transporter.TransportPlayers(PlayerById(TransportMenu.Selected[0]), PlayerById(TransportMenu.Selected[1]), this));
            TransportMenu.Selected.Clear();
            TransportButton.StartCooldown();
        }
    }

    // Bastion Stuff
    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; } = [];
    public bool IsBast => RevivedRole is Bastion;

    public bool BastException(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb(Vent target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(target.Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Bomb, target);
        }

        BombButton.StartCooldown(cooldown);
    }

    public bool BastUsable() => IsBast;

    // Trapper Stuff
    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; } = [];
    private List<Role> TriggeredRoles { get; } = [];
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }
    public bool IsTrap => RevivedRole is Trapper;

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

    private void SetTrap(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Place, target.PlayerId);
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RetActionsRPC.Trigger, trapped, trigger, isAttack);

        TriggeredRoles.Add(trigger.GetRole());
        AttackedSomeone = isAttack;
        Trapped.Remove(trapped.PlayerId);
    }

    public bool TrapUsable() => IsTrap;

    public bool BuildUsable() => TrapUsable() && TrapsMade <= Trapper.MaxTraps;
}