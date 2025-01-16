namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedGodfather : Intruder, IBlackmailer, IDragger, IDigger, IAmbusher, IFlasher, ITeleporter
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderHead;
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
    public Role FormerRole { get; set; }

    public override LayerEnum Type => LayerEnum.PromotedGodfather;
    public override Func<string> StartText => () => "Lead The <#FF1919FF>Intruders</color>";
    public override Func<string> Description => () => "- You have succeeded the former <#404C08FF>Godfather</color> and have a shorter cooldown on your former role's abilities"
        + (!FormerRole ? "" : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");
    public override bool RoleBlockImmune => FormerRole?.RoleBlockImmune ?? false;
    public override UColor Color
    {
        get
        {
            if (!ClientOptions.CustomIntColors)
                return CustomColorManager.Intruder;
            else
                return FormerRole?.Color ?? CustomColorManager.Godfather;
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButtonDown("Delete"))
        {
            if (BlockTarget && !BlockButton.EffectActive && IsCons)
                BlockTarget = null;

            Message("Removed a target");
        }
    }

    public void OnRoleSelected()
    {
        if (IsBM)
        {
            BlackmailButton ??= new(this, "BLACKMAIL", new SpriteName("Blackmail"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Blackmail, new Cooldown(Blackmailer.BlackmailCd),
                (PlayerBodyExclusion)BMException, (UsableFunc)BMUsable);
        }
        else if (IsCamo)
        {
            CamouflageButton ??= new(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitCamouflage, (ConditionFunc)CamoCondition,
                "CAMOUFLAGE", new Cooldown(Camouflager.CamouflageCd), new Duration(Camouflager.CamouflageDur), (EffectVoid)Camouflage, (EffectEndVoid)UnCamouflage, (UsableFunc)CamoUsable);
        }
        else if (IsGren)
        {
            FlashButton ??= new(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitFlash, new Cooldown(Grenadier.FlashCd), "FLASH",
                (EffectVoid)Flash, new Duration(Grenadier.FlashDur), (EffectStartVoid)StartFlash, (EffectEndVoid)UnFlash, (ConditionFunc)GrenCondition, (UsableFunc)GrenUsable);
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
                (EffectEndVoid)UnBlock, new Cooldown(Consort.ConsortCd), new Duration(Consort.ConsortDur), (EffectVoid)Block, (LabelFunc)ConsLabel, (EffectStartVoid)BlockStart);

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
        var gfAction = reader.ReadEnum<GFActionsRPC>();

        switch (gfAction)
        {
            case GFActionsRPC.Morph:
            {
                MorphedPlayer = reader.ReadPlayer();
                break;
            }
            case GFActionsRPC.Disguise:
            {
                DisguisedPlayer = reader.ReadPlayer();
                CopiedPlayer = reader.ReadPlayer();
                break;
            }
            case GFActionsRPC.Roleblock:
            {
                BlockTarget = reader.ReadPlayer();
                break;
            }
            case GFActionsRPC.Blackmail:
            {
                BlackmailedPlayer = reader.ReadPlayer();
                break;
            }
            case GFActionsRPC.Drag:
            {
                CurrentlyDragging = reader.ReadBody();
                DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
                break;
            }
            case GFActionsRPC.Ambush:
            {
                AmbushedPlayer = reader.ReadPlayer();
                break;
            }
            case GFActionsRPC.Teleport:
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

    public override void Kill(PlayerControl target)
    {
        base.Kill(target);

        if (Janitor.JaniCooldownsLinked && IsJani)
            CleanButton.StartCooldown(CooldownType.Custom, KillButton.CooldownTime);
    }

    // Impostor Stuff
    public bool IsImp => FormerRole is Impostor;

    // Blackmailer Stuff
    public CustomButton BlackmailButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; set; }
    public PlayerControl BlackmailedPlayer
    {
        get => Target;
        set => Target = value;
    }
    public bool IsBM => FormerRole is Blackmailer;

    public void Blackmail(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GFActionsRPC.Blackmail, BlackmailedPlayer);

            if (target.IsSilenced())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    public bool BMException(PlayerControl player) => player == BlackmailedPlayer || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Blackmailer.BlackmailMates) ||
        (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !Blackmailer.BlackmailMates);

    public bool BMUsable() => IsBM;

    // Camouflager Stuff
    public CustomButton CamouflageButton { get; set; }
    public bool IsCamo => FormerRole is Camouflager;

    public void Camouflage()
    {
        HudHandler.Instance.GodfatherEnabled = true;
        Utils.Camouflage();
    }

    public void UnCamouflage()
    {
        HudHandler.Instance.GodfatherEnabled = false;
        DefaultOutfitAll();
    }

    public void HitCamouflage()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CamouflageButton);
        CamouflageButton.Begin();
    }

    public bool CamoUsable() => IsCamo;

    public bool CamoCondition() => !HudHandler.Instance.IsCamoed;

    // Grenadier Stuff
    public CustomButton FlashButton { get; set; }
    public IEnumerable<byte> FlashedPlayers { get; set; }
    public bool IsGren => FormerRole is Grenadier;

    public void Flash()
    {
        var hud = HUD();

        foreach (var id in FlashedPlayers)
        {
            var player = PlayerById(id);

            if (player.AmOwner)
            {
                if (FlashButton.EffectTime > Grenadier.FlashDur - 0.5f)
                {
                    var fade = (FlashButton.EffectTime - Grenadier.FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.DimVision, fade);
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, Grenadier.FlashDur - 0.5f, true, true))
                {
                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = CustomColorManager.BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = CustomColorManager.DimVision;
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2) + 1;

                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.BlindVision, CustomColorManager.NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.DimVision, CustomColorManager.NormalVision, fade2);
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }

                if (MapBehaviourPatches.MapActive)
                    Map().Close();

                if (ActiveTask())
                    ActiveTask().Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => player.HasDied() || (((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None)) && !Meeting()) || player == Player || Meeting();

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player);

    public void UnFlash()
    {
        FlashedPlayers = [];
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, FlashButton);
        FlashButton.Begin();
    }

    public void StartFlash() => FlashedPlayers = GetClosestPlayers(Player, Grenadier.FlashRadius, includeDead: true).Select(x => x.PlayerId);

    public bool GrenCondition() => !Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !Grenadier.SaboFlash;

    public bool GrenUsable() => IsGren;

    // Janitor Stuff
    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }
    public bool IsJani => FormerRole is Janitor;

    public void Clean(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        CleanButton.StartCooldown();

        if (Janitor.JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    public void Drag(DeadBody target)
    {
        CurrentlyDragging = target;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GFActionsRPC.Drag, CurrentlyDragging);
        DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
    }

    public void Drop()
    {
        if (!CurrentlyDragging)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Drop, Player);
        DragHandler.Instance.StopDrag(Player);
        CurrentlyDragging = null;
        DragButton.StartCooldown();
    }

    public bool JaniUsable1() => !CurrentlyDragging && IsJani;

    public bool JaniUsable2() => CurrentlyDragging && IsJani;

    public float JaniDifference() => LastImp() && Janitor.SoloBoost && !Dead ? -Underdog.UnderdogCdBonus : 0;

    // Disguiser Stuff
    public CustomButton DisguiseButton { get; set; }
    public PlayerControl MeasuredPlayer { get; set; }
    public PlayerControl DisguisedPlayer { get; set; }
    public PlayerControl CopiedPlayer { get; set; }
    public CustomButton MeasureButton { get; set; }
    public bool IsDisg => FormerRole is Disguiser;

    public void Disguise() => Utils.Morph(DisguisedPlayer, CopiedPlayer);

    public void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    public void HitDisguise(PlayerControl target)
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

    public bool DisgException(PlayerControl player) => MeasureException(player) || (((player.Is(Faction) && Disguiser.DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction)
        && Disguiser.DisguiseTarget == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    public bool MeasureException(PlayerControl player) => player == MeasuredPlayer;

    public void Measure(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = target;

        MeasureButton.StartCooldown(cooldown);

        if (Disguiser.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public bool DisgUsable1() => IsDisg;

    public bool DisgUsable2() => IsDisg && MeasuredPlayer;

    public bool DisgEnd() => (DisguisedPlayer && DisguisedPlayer.HasDied()) || Dead;

    // Morphling Stuff
    public CustomButton MorphButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public CustomButton SampleButton { get; set; }
    public bool IsMorph => FormerRole is Morphling;

    public void Morph() => Utils.Morph(Player, MorphedPlayer);

    public void UnMorph()
    {
        MorphedPlayer = null;
        DefaultOutfit(Player);

        if (Morphling.MorphCooldownsLinked)
            SampleButton.StartCooldown();
    }

    public void HitMorph()
    {
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MorphButton, MorphedPlayer);
        MorphButton.Begin();
    }

    public void Sample(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = target;

        SampleButton.StartCooldown(cooldown);

        if (Morphling.MorphCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    public bool MorphException(PlayerControl player) => player == SampledPlayer;

    public bool MorphUsable1() => IsMorph;

    public bool MorphUsable2() => SampledPlayer && IsMorph;

    public bool MorphEnd() => Dead;

    // Wraith Stuff
    public CustomButton InvisButton { get; set; }
    public bool IsWraith => FormerRole is Wraith;

    public void Invis() => Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    public void UnInvis() => DefaultOutfit(Player);

    public void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    public bool InvisEnd() => Dead;

    public bool WraithUsable() => IsWraith;

    // Miner Stuff
    public CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; } = [];
    public bool IsMiner => FormerRole is Miner;

    public void Mine()
    {
        RpcSpawnVent(this);
        MineButton.StartCooldown();
    }

    public bool MineCondition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5))
        && Player.moveable && !GetPlayerElevator(Player).IsInElevator && !Vents.Any(x => x.transform.position == Player.transform.position);

    public bool MinerUsable() => IsMiner;

    // Consigliere Stuff
    public List<byte> Investigated { get; } = [];
    public CustomButton InvestigateButton { get; set; }
    public bool IsConsig => FormerRole is Consigliere;

    public void Investigate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Investigated.Add(target.PlayerId);

        InvestigateButton.StartCooldown(cooldown);
    }

    public bool ConsigException(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) &&
        Rivals.RivalsRoles) || (player.Is<Mafia>() && Player.Is<Mafia>() && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);

    public bool ConsigUsable() => IsConsig;

    // Teleporter Stuff
    public CustomButton TeleportButton { get; set; }
    public Vector2 TeleportPoint { get; set; }
    public CustomButton MarkButton { get; set; }
    public bool Teleporting { get; set; }
    public bool IsTele => FormerRole is Teleporter;

    public void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (Teleporter.TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    public void Teleport()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GFActionsRPC.Teleport, TeleportPoint);
        Coroutines.Start(Teleporter.TeleportPlayer(TeleportPoint, this));
        TeleportButton.StartCooldown();

        if (Teleporter.TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    public bool MarkCondition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5))
        && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != (Vector2)Player.transform.position;

    public bool TeleUsable1() => IsTele;

    public bool TeleUsable2() => IsTele && TeleportPoint != Vector2.zero;

    public bool TeleCondition() => (Vector2)Player.transform.position != TeleportPoint && !Teleporting;

    // Ambusher Stuff
    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }
    public bool IsAmb => FormerRole is Ambusher;

    public void UnAmbush() => AmbushedPlayer = null;

    public void Ambush(PlayerControl target)
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

    public bool AmbushException(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !Ambusher.AmbushMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Ambusher.AmbushMates);

    public bool AmbUsable() => IsAmb;

    public bool AmbEnd() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    // Consort Stuff
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomPlayerMenu BlockMenu { get; set; }
    public bool IsCons => FormerRole is Consort;

    public bool ConsException(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

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

    public void ConsClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            BlockTarget = player;
        else
            BlockButton.StartCooldown(cooldown);
    }

    public void Roleblock()
    {
        if (!BlockTarget)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    public bool ConsUsable() => IsCons;

    public string ConsLabel() => BlockTarget ? "ROLEBLOCK" : "SET TARGET";

    public bool BlockEnd() => (BlockTarget && BlockTarget.HasDied()) || Dead;

    // Enforcer Stuff
    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }
    public bool IsEnf => FormerRole is Enforcer;

    public bool EnfException(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void BoomStart()
    {
        if (BombedPlayer.AmOwner && !Dead)
        {
            Utils.Flash(Color);
            BombedPlayer.GetRole().Bombed = true;
        }
    }

    public void UnBoom()
    {
        if (!BombSuccessful)
            Enforcer.Explode(BombedPlayer, Player);

        BombedPlayer.GetRole().Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    public void Bomb(PlayerControl target)
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

    public bool EnfUsable() => IsEnf;

    public bool BoomEnd() => (BombedPlayer && BombedPlayer.HasDied()) || Dead || BombSuccessful;
}