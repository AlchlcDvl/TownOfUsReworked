namespace TownOfUsReworked.PlayerLayers.Roles;

// TODO: Refactor this to instead use the props and abilities of their former roles directly
public sealed class PromotedGodfather : Intruder, IBlackmailer, IDragger, IDigger, IAmbusher, IFlasher, IMover, IBlocker, IMorpher
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Head;
        TeleportPoint = Vector3.zero;
        Investigated.Clear();
        FlashedPlayers = [];
        Vents.Clear();
        CopiedPlayer = null;
        DisguisedPlayer = null;
        MorphedPlayer = null;
        SampledPlayer = null;
        MeasuredPlayer = null;
        AmbushedPlayer = null;
        BombedPlayer = null;
        BlockTarget = null;
        BlackmailedPlayer = null;
    }

    // PromotedGodfather Stuff
    public Role FormerRole { get; init; }

    public override LayerEnum Type => LayerEnum.PromotedGodfather;
    public override Func<string> StartText => () => "Lead The <#FF1919FF>Intruders</color>";
    public override Func<string> Description => () => "- You have succeeded the former <#404C08FF>Godfather</color> and have a shorter cooldown on your former role's abilities"
        + (!FormerRole ? "" : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");
    public override bool RoleBlockImmune => FormerRole?.RoleBlockImmune ?? false;
    public override UColor Color
    {
        get
        {
            if (ClientOptions.CustomIntColors)
                return FormerRole?.Color ?? CustomColorManager.Godfather;

            return CustomColorManager.Intruder;
        }
    }
    public override bool CanVent => base.CanVent && FormerRole switch
    {
        Janitor => (int)Janitor.JanitorVentOptions is 3 || (CurrentlyDragging && (int)Janitor.JanitorVentOptions is 1) || (!CurrentlyDragging && (int)Janitor.JanitorVentOptions is 2),
        Morphling => Morphling.MorphlingVent,
        Wraith => Wraith.WraithVent,
        Grenadier => Grenadier.GrenadierVent,
        Teleporter => Teleporter.TeleVent,
        _ => true
    };

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (IsConsig && Investigated.Contains(player.PlayerId) && !revealed)
        {
            revealed = true;
            var revealRole = Consigliere.ConsigInfo == ConsigInfo.Role;
            var role = handler.CustomRole;
            removeFromConsig = role.SubFaction == SubFaction && role.SubFaction != SubFaction.None && revealRole;
            color = revealRole ? role.Color : role.FactionColor;
            name += revealRole ? $"\n{role}" : $"\n{role.FactionName}";
        }
        else if (IsBm && BlackmailedPlayer == player)
            name += " <#02A752FF>Φ</color>";
        else if (IsAmb && AmbushedPlayer == player)
            name += " <#2BD29CFF>人</color>";
        else if (IsGren && FlashedPlayers.Contains(player.PlayerId) && Grenadier.GrenadierIndicators)
            name += " <#85AA5BFF>ㅇ</color>";
    }

    protected override void OnTrueDeath()
    {
        if (DeadSeeEverything())
            Investigated.Clear();
    }

    public override void Reset(bool meeting, bool start)
    {
        BlackmailedPlayer = BlockTarget = MeasuredPlayer = DisguisedPlayer = CopiedPlayer = SampledPlayer = MorphedPlayer = AmbushedPlayer = BombedPlayer = null;
        CurrentlyDragging = null;
        TeleportPoint = Vector2.zero;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (IsCons && BlockTarget && !BlockButton.EffectActive)
            BlockTarget = null;

        Message("Removed a target");
    }

    public void OnRoleSelected()
    {
        if (IsBm)
        {
            BlackmailButton ??= new(this, "BLACKMAIL", new SpriteName("Blackmail"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Blackmail, new Cooldown(Blackmailer.BlackmailCd),
                (PlayerBodyExclusion)BmException, (UsableFunc)BmUsable);
        }
        else if (IsCamo)
        {
            CamouflageButton ??= new(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitCamouflage, (ConditionFunc)Camouflager.Condition,
                "CAMOUFLAGE", new Cooldown(Camouflager.CamouflageCd), new Duration(Camouflager.CamouflageDur), (EffectVoid)Camouflager.Camouflage, (EffectEndVoid)Camouflager.UnCamouflage, (UsableFunc)CamoUsable);
        }
        else if (IsGren)
        {
            FlashButton ??= new(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitFlash, new Cooldown(Grenadier.FlashCd), "FLASH",
                (EffectVoid)Flash, new Duration(Grenadier.FlashDur), (EffectStartVoid)StartFlash, (EffectEndVoid)UnFlash, (ConditionFunc)Grenadier.Condition, (UsableFunc)GrenUsable,
                new CanClickAgain(false));
        }
        else if (IsJani)
        {
            DragButton ??= new(this, new SpriteName("Drag"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)Drag, new Cooldown(Janitor.DragCd), "DRAG BODY", (UsableFunc)JaniUsable1);
            DropButton ??= new(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Drop, "DROP BODY", (UsableFunc)JaniUsable2);
            CleanButton ??= new(this, new SpriteName("Clean"), AbilityTypes.Body, KeybindType.Secondary, (OnClickBody)Clean, new Cooldown(Janitor.CleanCd), "CLEAN BODY",
                (UsableFunc)JaniUsable1, (DifferenceFunc)JaniDifference);
        }
        else if (IsDisg)
        {
            MeasureButton ??= new(this, new SpriteName("Measure"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Measure, new Cooldown(Disguiser.MeasureCd), "MEASURE",
                (PlayerBodyExclusion)MeasureException, (UsableFunc)DisgUsable1);
            DisguiseButton ??= new(this, new SpriteName("Disguise"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)HitDisguise, new Cooldown(Disguiser.DisguiseCd), "DISGUISE",
                new Duration(Disguiser.DisguiseDur), (EffectVoid)Disguise, (EffectEndVoid)UnDisguise, new Delay(Disguiser.DisguiseDelay), (PlayerBodyExclusion)DisgException, (EndFunc)DisgEnd,
                (UsableFunc)DisgUsable2);
        }
        else if (IsMorph)
        {
            SampleButton ??= new(this, new SpriteName("Sample"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Sample, new Cooldown (Morphling.SampleCd), "SAMPLE",
                (PlayerBodyExclusion)MorphException, (UsableFunc)MorphUsable1);
            MorphButton ??= new(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitMorph, new Cooldown(Morphling.MorphCd), "MORPH",
                (EffectVoid)Morph, new Duration(Morphling.MorphDur), (EffectEndVoid)UnMorph, (EndFunc)MorphEnd, (UsableFunc)MorphUsable2);
        }
        else if (IsWraith)
        {
            InvisButton ??= new(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitInvis, (UsableFunc)WraithUsable,
                (EffectVoid)Invis, new Cooldown(Wraith.InvisCd), new Duration(Wraith.InvisDur), (EffectEndVoid)UnInvis, (EndFunc)InvisEnd);
        }
        else if (IsAmb)
        {
            AmbushButton ??= new(this, new SpriteName("Ambush"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Ambush, new Cooldown(Ambusher.AmbushCd), "AMBUSH", (EndFunc)AmbEnd,
                (UsableFunc)AmbUsable, new Duration(Ambusher.AmbushDur), (EffectEndVoid)UnAmbush, (PlayerBodyExclusion)AmbushException);
        }
        else if (IsEnf)
        {
            BombButton ??= new(this, new SpriteName("Enforce"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Bomb, new Cooldown(Enforcer.EnforceCd), "SET BOMB", (EndFunc)BoomEnd,
                (UsableFunc)EnfUsable, new Duration(Enforcer.EnforceDur), (EffectStartVoid)BoomStart, (EffectStartVoid)UnBoom, new Delay(Enforcer.EnforceDelay), new CanClickAgain(false),
                (PlayerBodyExclusion)EnfException);
        }
        else if (IsConsig)
        {
            InvestigateButton ??= new(this, new SpriteName("Investigate"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Investigate, (UsableFunc)ConsigUsable, "INVESTIGATE",
                new Cooldown(Consigliere.InvestigateCd), (PlayerBodyExclusion)ConsigException);
        }
        else if (IsCons)
        {
            var wasnull = BlockButton == null;
            BlockButton ??= new(this, new SpriteName("ConsortRoleblock"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Roleblock, (UsableFunc)ConsUsable, (EndFunc)BlockEnd,
                (EffectEndVoid)UnBlock, new Cooldown(Consort.ConsortCd), new Duration(Consort.ConsortDur), (LabelFunc)ConsLabel, (EffectStartVoid)BlockStart);

            if (wasnull && BlockMenu == null)
                BlockMenu = new(Player, ConsClick, ConsException);
        }
        else if (IsMiner)
        {
            MineButton ??= new(this, new SpriteName(Miner.SpriteName), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Mine, new Cooldown(Miner.MineCd),
                (LabelFunc)Miner.Label, (ConditionFunc)MineCondition, (UsableFunc)MinerUsable);
        }
        else if (IsTele)
        {
            MarkButton ??= new(this, new SpriteName("Mark"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Mark, new Cooldown(Teleporter.TeleMarkCd),
                (ConditionFunc)MarkCondition, "MARK POSITION", (UsableFunc)TeleUsable1);
            TeleportButton ??= new(this, new SpriteName("Teleport"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Teleport, new Cooldown(Teleporter.TeleportCd),
                "TELEPORT", (UsableFunc)TeleUsable2, (ConditionFunc)TeleCondition);
        }

        Player.ResetButtons();
    }

    public override void ReadRPC(MessageReader reader)
    {
        var gfAction = reader.ReadEnum<GfActionsRPC>();

        switch (gfAction)
        {
            case GfActionsRPC.Morph:
            {
                MorphedPlayer = reader.ReadPlayer();
                break;
            }
            case GfActionsRPC.Disguise:
            {
                DisguisedPlayer = reader.ReadPlayer();
                CopiedPlayer = reader.ReadPlayer();
                break;
            }
            case GfActionsRPC.Roleblock:
            {
                BlockTarget = reader.ReadPlayer();
                break;
            }
            case GfActionsRPC.Blackmail:
            {
                BlackmailedPlayer = reader.ReadPlayer();
                break;
            }
            case GfActionsRPC.Drag:
            {
                CurrentlyDragging = reader.ReadBody();
                DragHandler.StartDrag(Player, CurrentlyDragging);
                break;
            }
            case GfActionsRPC.Ambush:
            {
                AmbushedPlayer = reader.ReadPlayer();
                break;
            }
            case GfActionsRPC.Teleport:
            {
                Coroutines.Start(Teleporter.TeleportPlayer(reader.ReadVector2(), this));
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {gfAction}");
                break;
            }
        }
    }

    protected override void Kill(PlayerControl target)
    {
        base.Kill(target);

        if (Janitor.JaniCooldownsLinked && IsJani)
            CleanButton.StartCooldown(CooldownType.Custom, KillButton.CooldownTime);
    }

    // Impostor Stuff
    // public bool IsImp => FormerRole is Impostor;

    // Blackmailer Stuff
    private CustomButton BlackmailButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target => BlackmailedPlayer;
    public PlayerControl BlackmailedPlayer { get; private set; }
    private bool IsBm => FormerRole is Blackmailer;

    private void Blackmail(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GfActionsRPC.Blackmail, BlackmailedPlayer);

            if (target.IsSilenced())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    private bool BmException(PlayerControl player) => player == BlackmailedPlayer || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Blackmailer.BlackmailMates) ||
        (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !Blackmailer.BlackmailMates);

    private bool BmUsable() => IsBm;

    // Camouflager Stuff
    private CustomButton CamouflageButton { get; set; }
    private bool IsCamo => FormerRole is Camouflager;

    private void HitCamouflage()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CamouflageButton);
        CamouflageButton.Begin();
    }

    private bool CamoUsable() => IsCamo;

    // Grenadier Stuff
    private CustomButton FlashButton { get; set; }
    public IEnumerable<byte> FlashedPlayers { get; private set; }
    private bool IsGren => FormerRole is Grenadier;

    private void Flash()
    {
        foreach (var id in FlashedPlayers)
        {
            var player = PlayerById(id);

            if (!player.AmOwner)
                continue;

            if (MapBehaviourPatches.MapActive)
                Map().Close();

            if (ActiveTask())
                ActiveTask().Close();
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => player.HasDied() || (((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None)) && !Meeting()) || player == Player || Meeting();

    private void UnFlash() => FlashedPlayers = [];

    private void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, FlashButton);
        FlashButton.Begin();
    }

    private void StartFlash()
    {
        FlashedPlayers = [ .. GetClosestPlayers(Player, Grenadier.FlashRadius, includeDead: true).Select(x => x.PlayerId), PlayerId];

        if (FlashedPlayers.Contains(CustomPlayer.Local.PlayerId))
            TransitionFlash(CustomColorManager.BlindVision, Grenadier.FlashDur, ShouldPlayerBeDimmed(CustomPlayer.Local) ? 0.4f : 1f);
    }

    private bool GrenUsable() => IsGren;

    // Janitor Stuff
    private CustomButton CleanButton { get; set; }
    private CustomButton DragButton { get; set; }
    private CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }
    public bool IsJani => FormerRole is Janitor;

    private void Clean(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        CleanButton.StartCooldown();

        if (Janitor.JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    private void Drag(DeadBody target)
    {
        CurrentlyDragging = target;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GfActionsRPC.Drag, CurrentlyDragging);
        DragHandler.StartDrag(Player, CurrentlyDragging);
    }

    public void Drop()
    {
        if (!CurrentlyDragging)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Drop, Player);
        DragHandler.StopDrag(Player);
        DragButton.StartCooldown();
    }

    private bool JaniUsable1() => !CurrentlyDragging && IsJani;

    private bool JaniUsable2() => CurrentlyDragging && IsJani;

    private float JaniDifference() => !Dead && Last(Faction) && Janitor.SoloBoost ? -Underdog.UnderdogCdBonus : 0;

    // Disguiser Stuff
    private CustomButton DisguiseButton { get; set; }
    private PlayerControl MeasuredPlayer { get; set; }
    private PlayerControl DisguisedPlayer { get; set; }
    private PlayerControl CopiedPlayer { get; set; }
    private CustomButton MeasureButton { get; set; }
    private bool IsDisg => FormerRole is Disguiser;

    private void Disguise() => MiscUtils.Morph(DisguisedPlayer, CopiedPlayer);

    private void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    private void HitDisguise(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, DisguiseButton, CopiedPlayer, DisguisedPlayer);
            DisguiseButton.Begin();
        }
        else
            DisguiseButton.StartCooldown(cooldown);

        if (Disguiser.DisgCooldownsLinked)
            MeasureButton.StartCooldown(cooldown);
    }

    private bool DisgException(PlayerControl player) => MeasureException(player) || (((player.Is(Faction) && Disguiser.DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction)
        && Disguiser.DisguiseTarget == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    private bool MeasureException(PlayerControl player) => player == MeasuredPlayer;

    private void Measure(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = target;

        MeasureButton.StartCooldown(cooldown);

        if (Disguiser.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    private bool DisgUsable1() => IsDisg;

    private bool DisgUsable2() => IsDisg && MeasuredPlayer;

    private bool DisgEnd() => (DisguisedPlayer && DisguisedPlayer.HasDied()) || Dead;

    // Morphling Stuff
    private CustomButton MorphButton { get; set; }
    public PlayerControl MorphedPlayer { get; private set; }
    private PlayerControl SampledPlayer { get; set; }
    private CustomButton SampleButton { get; set; }
    private bool IsMorph => FormerRole is Morphling;

    private void Morph() => MiscUtils.Morph(Player, MorphedPlayer);

    private void UnMorph()
    {
        MorphedPlayer = null;
        DefaultOutfit(Player);

        if (Morphling.MorphCooldownsLinked)
            SampleButton.StartCooldown();
    }

    private void HitMorph()
    {
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MorphButton, MorphedPlayer);
        MorphButton.Begin();
    }

    private void Sample(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = target;

        SampleButton.StartCooldown(cooldown);

        if (Morphling.MorphCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    private bool MorphException(PlayerControl player) => player == SampledPlayer;

    private bool MorphUsable1() => IsMorph;

    private bool MorphUsable2() => SampledPlayer && IsMorph;

    private bool MorphEnd() => Dead;

    // Wraith Stuff
    private CustomButton InvisButton { get; set; }
    private bool IsWraith => FormerRole is Wraith;

    private void Invis() => MiscUtils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    private void UnInvis() => DefaultOutfit(Player);

    private void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    private bool InvisEnd() => Dead;

    private bool WraithUsable() => IsWraith;

    // Miner Stuff
    private CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; } = [];
    private bool IsMiner => FormerRole is Miner;

    private void Mine()
    {
        RpcSpawnVent(this);
        MineButton.StartCooldown();
    }

    private bool MineCondition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5))
        && Player.moveable && !GetPlayerElevator(Player).IsInElevator && Vents.All(x => x.transform.position != Player.transform.position);

    private bool MinerUsable() => IsMiner;

    // Consigliere Stuff
    public List<byte> Investigated { get; } = [];
    private CustomButton InvestigateButton { get; set; }
    private bool IsConsig => FormerRole is Consigliere;

    private void Investigate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Investigated.Add(target.PlayerId);

        InvestigateButton.StartCooldown(cooldown);
    }

    private bool ConsigException(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) &&
        Rivals.RivalsRoles) || (player.Is<Mafia>() && Player.Is<Mafia>() && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);

    private bool ConsigUsable() => IsConsig;

    // Teleporter Stuff
    private CustomButton TeleportButton { get; set; }
    private Vector2 TeleportPoint { get; set; }
    private CustomButton MarkButton { get; set; }
    public bool Moving { get; set; }
    private bool IsTele => FormerRole is Teleporter;

    private void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (Teleporter.TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    private void Teleport()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GfActionsRPC.Teleport, TeleportPoint);
        Coroutines.Start(Teleporter.TeleportPlayer(TeleportPoint, this));
        TeleportButton.StartCooldown();

        if (Teleporter.TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    private bool MarkCondition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5))
        && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != (Vector2)Player.transform.position;

    private bool TeleUsable1() => IsTele;

    private bool TeleUsable2() => IsTele && TeleportPoint != Vector2.zero;

    private bool TeleCondition() => Vector2.Distance(Player.transform.position, TeleportPoint) <= 1f && !Moving;

    // Ambusher Stuff
    public PlayerControl AmbushedPlayer { get; private set; }
    public CustomButton AmbushButton { get; private set; }
    private bool IsAmb => FormerRole is Ambusher;

    private void UnAmbush() => AmbushedPlayer = null;

    private void Ambush(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            AmbushedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);
    }

    private bool AmbushException(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !Ambusher.AmbushMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Ambusher.AmbushMates);

    private bool AmbUsable() => IsAmb;

    private bool AmbEnd() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    // Consort Stuff
    private CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; private set; }
    private CustomPlayerMenu BlockMenu { get; set; }
    private bool IsCons => FormerRole is Consort;

    private bool ConsException(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

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

    private void ConsClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            BlockTarget = player;
        else
            BlockButton.StartCooldown(cooldown);
    }

    private void Roleblock()
    {
        if (!BlockTarget)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    private bool ConsUsable() => IsCons;

    private string ConsLabel() => BlockTarget ? "ROLEBLOCK" : "SET TARGET";

    private bool BlockEnd() => (BlockTarget && BlockTarget.HasDied()) || Dead;

    // Enforcer Stuff
    private CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }
    private bool IsEnf => FormerRole is Enforcer;

    private bool EnfException(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    private void BoomStart()
    {
        if (!BombedPlayer.AmOwner || Dead)
            return;

        MiscUtils.Flash(Color);
        BombedPlayer.GetRole().Bombed = true;
    }

    private void UnBoom()
    {
        if (!BombSuccessful)
            Enforcer.Explode(BombedPlayer, Player);

        BombedPlayer.GetRole().Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    private void Bomb(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else
            BombButton.StartCooldown(cooldown);
    }

    private bool EnfUsable() => IsEnf;

    private bool BoomEnd() => (BombedPlayer && BombedPlayer.HasDied()) || Dead || BombSuccessful;
}