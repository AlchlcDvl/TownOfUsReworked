namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedGodfather : Intruder
{
    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderHead;
        BlockMenu = new(Player, ConsClick, ConsException);
        TeleportPoint = Vector3.zero;
        Investigated = [];
        FlashedPlayers = [];
        Vents = [];
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

    public override UColor Color
    {
        get
        {
            if (!ClientOptions.CustomIntColors)
                return CustomColorManager.Intruder;
            else
                return FormerRole?.Color ?? CustomColorManager.Rebel;
        }
    }
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.PromotedGodfather;
    public override Func<string> StartText => () => "Lead The <color=#FF1919FF>Intruders</color>";
    public override Func<string> Description => () => "- You have succeeded the former <color=#404C08FF>Godfather</color> and have a shorter cooldown on your former role's abilities"
        + (!FormerRole ? "" : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (BlockTarget && !BlockButton.EffectActive && IsCons)
                BlockTarget = null;

            LogMessage("Removed a target");
        }
    }

    public void OnRoleSelected()
    {
        if (IsBM)
        {
            BlackmailButton ??= CreateButton(this, "BLACKMAIL", "Blackmail", AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Blackmail, new Cooldown(CustomGameOptions.BlackmailCd),
                (PlayerBodyExclusion)BMException, (UsableFunc)BMUsable);
        }
        else if (IsCamo)
        {
            CamouflageButton ??= CreateButton(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitCamouflage, (ConditionFunc)CamoCondition,
                "CAMOUFLAGE", new Cooldown(CustomGameOptions.CamouflagerCd), new Duration(CustomGameOptions.CamouflageDur), (EffectVoid)Camouflage, (EffectEndVoid)UnCamouflage,
                (UsableFunc)CamoUsable);
        }
        else if (IsGren)
        {
            FlashButton ??= CreateButton(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitFlash, new Cooldown(CustomGameOptions.FlashCd),
                new Duration(CustomGameOptions.FlashDur), (EffectVoid)Flash, (EffectStartVoid)StartFlash, (EffectEndVoid)UnFlash, (ConditionFunc)GrenCondition, "FLASH",
                (UsableFunc)GrenUsable);
        }
        else if (IsJani)
        {
            DragButton ??= CreateButton(this, new SpriteName("Drag"), AbilityTypes.Dead, KeybindType.Tertiary, (OnClick)Drag, new Cooldown(CustomGameOptions.DragCd), "DRAG BODY",
                (UsableFunc)JaniUsable1);
            DropButton ??= CreateButton(this, new SpriteName("Drop"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClick)Drop, "DROP BODY", (UsableFunc)JaniUsable2);
            CleanButton ??= CreateButton(this, new SpriteName("Clean"), AbilityTypes.Dead, KeybindType.Secondary, (OnClick)Clean, new Cooldown(CustomGameOptions.CleanCd), "CLEAN BODY",
                (UsableFunc)JaniUsable1, (DifferenceFunc)JaniDifference);
        }
        else if (IsDisg)
        {
            MeasureButton ??= CreateButton(this, new SpriteName("Measure"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Measure, new Cooldown(CustomGameOptions.MeasureCd), "MEASURE",
                (PlayerBodyExclusion)MeasureException, (UsableFunc)DisgUsable1);
            DisguiseButton ??= CreateButton(this, new SpriteName("Disguise"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)HitDisguise, new Cooldown(CustomGameOptions.DisguiseCd),
                new Duration(CustomGameOptions.DisguiseDur), (EffectVoid)Disguise, (EffectEndVoid)UnDisguise, new Delay(CustomGameOptions.DisguiseDelay), (PlayerBodyExclusion)DisgException,
                (UsableFunc)DisgUsable2, (EndFunc)DisgEnd, "DISGUISE");
        }
        else if (IsMorph)
        {
            SampleButton ??= CreateButton(this, new SpriteName("Sample"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Sample, new Cooldown (CustomGameOptions.SampleCd), "SAMPLE",
                (PlayerBodyExclusion)MorphException, (UsableFunc)MorphUsable1);
            MorphButton ??= CreateButton(this, new SpriteName("Morph"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitMorph, new Cooldown(CustomGameOptions.MorphCd), "MORPH",
                new Duration(CustomGameOptions.MorphDur), (EffectVoid)Morph, (EffectEndVoid)UnMorph, (EndFunc)MorphEnd, (UsableFunc)MorphUsable2);
        }
        else if (IsWraith)
        {
            InvisButton ??= CreateButton(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitInvis, (UsableFunc)WraithUsable,
                new Cooldown(CustomGameOptions.InvisCd), (EffectVoid)Invis, new Duration(CustomGameOptions.InvisDur), (EffectEndVoid)UnInvis, (EndFunc)InvisEnd);
        }
        else if (IsAmb)
        {
            AmbushButton ??= CreateButton(this, new SpriteName("Ambush"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Ambush, new Cooldown(CustomGameOptions.AmbushCd), "AMBUSH",
                (EndFunc)AmbEnd, new Duration(CustomGameOptions.AmbushDur), (EffectEndVoid)UnAmbush, (PlayerBodyExclusion)AmbushException, (UsableFunc)AmbUsable);
        }
        else if (IsEnf)
        {
            BombButton ??= CreateButton(this, new SpriteName("Enforce"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Bomb, new Cooldown(CustomGameOptions.EnforceCd),
                (UsableFunc)EnfUsable, new Duration(CustomGameOptions.EnforceDur), (EffectStartVoid)BoomStart, (EffectStartVoid)UnBoom, new Delay(CustomGameOptions.EnforceDelay),
                (PlayerBodyExclusion)EnfException, new CanClickAgain(false), (EndFunc)BoomEnd, "SET BOMB");
        }
        else if (IsConsig)
        {
            InvestigateButton ??= CreateButton(this, new SpriteName("Investigate"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Investigate, (UsableFunc)ConsigUsable, "INVESTIGATE",
                new Cooldown(CustomGameOptions.InvestigateCd), (PlayerBodyExclusion)ConsigException);
        }
        else if (IsCons)
        {
            BlockButton ??= CreateButton(this, new SpriteName("ConsortRoleblock"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Roleblock, (UsableFunc)ConsUsable,
                new Cooldown(CustomGameOptions.ConsortCd), new Duration(CustomGameOptions.ConsortDur), (EffectVoid)Block, (EffectEndVoid)UnBlock, (LabelFunc)ConsLabel);
        }
        else if (IsMiner)
        {
            MineButton ??= CreateButton(this, new SpriteName(Miner.SpriteName), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Mine, new Cooldown(CustomGameOptions.MineCd),
                (LabelFunc)Miner.Label, (ConditionFunc)MineCondition, (UsableFunc)MinerUsable);
        }
        else if (IsTele)
        {
            MarkButton ??= CreateButton(this, new SpriteName("Mark"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Mark, new Cooldown(CustomGameOptions.TeleMarkCd),
                "MARK POSITION", (ConditionFunc)MarkCondition, (UsableFunc)TeleUsable1);
            TeleportButton ??= CreateButton(this, new SpriteName("Teleport"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Teleport, new Cooldown(CustomGameOptions.TeleportCd),
                "TELEPORT", (UsableFunc)TeleUsable2, (ConditionFunc)TeleCondition);
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var gfAction = (GFActionsRPC)reader.ReadByte();

        switch (gfAction)
        {
            case GFActionsRPC.Morph:
                MorphedPlayer = reader.ReadPlayer();
                break;

            case GFActionsRPC.Disguise:
                DisguisedPlayer = reader.ReadPlayer();
                CopiedPlayer = reader.ReadPlayer();
                break;

            case GFActionsRPC.Roleblock:
                BlockTarget = reader.ReadPlayer();
                break;

            case GFActionsRPC.Blackmail:
                BlackmailedPlayer = reader.ReadPlayer();
                break;

            case GFActionsRPC.Drag:
                CurrentlyDragging = reader.ReadBody();
                DragHandler.Instance.StartDrag(Player, CurrentlyDragging);
                break;

            case GFActionsRPC.Ambush:
                AmbushedPlayer = reader.ReadPlayer();
                break;

            default:
                LogError($"Received unknown RPC - {(int)gfAction}");
                break;
        }
    }

    // Impostor Stuff
    public bool IsImp => FormerRole?.Type == LayerEnum.Impostor;

    // Blackmailer Stuff
    public CustomButton BlackmailButton { get; set; }
    public PlayerControl BlackmailedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public UColor? PrevColor { get; set; }
    public bool IsBM => FormerRole?.Type == LayerEnum.Blackmailer;

    public void Blackmail()
    {
        var cooldown = Interact(Player, BlackmailButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, GFActionsRPC.Blackmail, BlackmailedPlayer);
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    public bool BMException(PlayerControl player) => player == BlackmailedPlayer || (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.BlackmailMates) ||
        (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && CustomGameOptions.BlackmailMates);

    public bool BMUsable() => IsBM;

    // Camouflager Stuff
    public CustomButton CamouflageButton { get; set; }
    public bool IsCamo => FormerRole?.Type == LayerEnum.Camouflager;

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
    public List<byte> FlashedPlayers { get; set; }
    public bool IsGren => FormerRole?.Type == LayerEnum.Grenadier;

    public void Flash()
    {
        foreach (var id in FlashedPlayers)
        {
            var player = PlayerById(id);

            if (CustomPlayer.Local == player)
            {
                if (FlashButton.EffectTime > CustomGameOptions.FlashDur - 0.5f)
                {
                    var fade = (FlashButton.EffectTime - CustomGameOptions.FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.DimVision, fade);
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, CustomGameOptions.FlashDur - 0.5f, true, true))
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = CustomColorManager.BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = CustomColorManager.DimVision;
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2) + 1;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.BlindVision, CustomColorManager.NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.DimVision, CustomColorManager.NormalVision, fade2);
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }

                if (MapPatch.MapActive)
                    Map.Close();

                if (ActiveTask)
                    ActiveTask.Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) =>  (((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && !Meeting) || player == Player;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player) && !Meeting;

    public void UnFlash()
    {
        FlashedPlayers.Clear();
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, FlashButton);
        FlashButton.Begin();
    }

    public void StartFlash() => FlashedPlayers = [ .. GetClosestPlayers(Player.transform.position, CustomGameOptions.FlashRadius).Select(x => x.PlayerId) ];

    public bool GrenCondition() => !Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !CustomGameOptions.SaboFlash;

    public bool GrenUsable() => IsGren;

    // Janitor Stuff
    public CustomButton CleanButton { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }
    public bool IsJani => FormerRole?.Type == LayerEnum.Janitor;

    public void Clean()
    {
        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, CleanButton.TargetBody);
        FadeBody(CleanButton.TargetBody);
        CleanButton.StartCooldown();

        if (CustomGameOptions.JaniCooldownsLinked)
            KillButton.StartCooldown();
    }

    public void Drag()
    {
        CurrentlyDragging = DragButton.TargetBody;
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

    public float JaniDifference() => LastImp && CustomGameOptions.SoloBoost && !Dead ? -CustomGameOptions.UnderdogKillBonus : 0;

    // Disguiser Stuff
    public CustomButton DisguiseButton { get; set; }
    public PlayerControl MeasuredPlayer { get; set; }
    public PlayerControl DisguisedPlayer { get; set; }
    public PlayerControl CopiedPlayer { get; set; }
    public CustomButton MeasureButton { get; set; }
    public DateTime LastMeasured { get; set; }
    public bool IsDisg => FormerRole?.Type == LayerEnum.Disguiser;

    public void Disguise() => Utils.Morph(DisguisedPlayer, CopiedPlayer);

    public void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    public void HitDisguise()
    {
        var cooldown = Interact(Player, DisguiseButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = DisguiseButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, DisguiseButton, CopiedPlayer, DisguisedPlayer);
            DisguiseButton.Begin();
        }
        else
            DisguiseButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            MeasureButton.StartCooldown(cooldown);
    }

    public bool DisgException(PlayerControl player) => MeasureException(player) || (((player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) ||
        (!player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    public bool MeasureException(PlayerControl player) => player == MeasuredPlayer;

    public void Measure()
    {
        var cooldown = Interact(Player, MeasureButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = MeasureButton.TargetPlayer;

        MeasureButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public bool DisgUsable1() => IsDisg;

    public bool DisgUsable2() => IsDisg && MeasuredPlayer;

    public bool DisgEnd() => (DisguisedPlayer && DisguisedPlayer.HasDied()) || Dead;

    // Morphling Stuff
    public CustomButton MorphButton { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public DateTime LastSampled { get; set; }
    public CustomButton SampleButton { get; set; }
    public bool IsMorph => FormerRole?.Type == LayerEnum.Morphling;

    public void Morph() => Utils.Morph(Player, MorphedPlayer);

    public void UnMorph()
    {
        MorphedPlayer = null;
        DefaultOutfit(Player);

        if (CustomGameOptions.MorphCooldownsLinked)
            SampleButton.StartCooldown();
    }

    public void HitMorph()
    {
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MorphButton, MorphedPlayer);
        MorphButton.Begin();
    }

    public void Sample()
    {
        var cooldown = Interact(Player, SampleButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            SampledPlayer = SampleButton.TargetPlayer;

        SampleButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            MorphButton.StartCooldown(cooldown);
    }

    public bool MorphException(PlayerControl player) => player == SampledPlayer;

    public bool MorphUsable1() => IsMorph;

    public bool MorphUsable2() => SampledPlayer && IsMorph;

    public bool MorphEnd() => Dead;

    // Wraith Stuff
    public CustomButton InvisButton { get; set; }
    public bool IsWraith => FormerRole?.Type == LayerEnum.Wraith;

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
    public List<Vent> Vents { get; set; }
    public bool IsMiner => FormerRole?.Type == LayerEnum.Miner;

    public void Mine()
    {
        RpcSpawnVent(this);
        MineButton.StartCooldown();
    }

    public bool MineCondition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or
        5)) && Player.moveable && !GetPlayerElevator(Player).IsInElevator && !Vents.Any(x => x.transform.position == Player.transform.position);

    public bool MinerUsable() => IsMiner;

    // Consigliere Stuff
    public List<byte> Investigated { get; set; }
    public CustomButton InvestigateButton { get; set; }
    public bool IsConsig => FormerRole?.Type == LayerEnum.Consigliere;

    public void Investigate()
    {
        var cooldown = Interact(Player, InvestigateButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

        InvestigateButton.StartCooldown(cooldown);
    }

    public bool ConsigException(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public bool ConsigUsable() => IsConsig;

    // Teleporter Stuff
    public CustomButton TeleportButton { get; set; }
    public Vector3 TeleportPoint { get; set; }
    public CustomButton MarkButton { get; set; }
    public bool IsTele => FormerRole?.Type == LayerEnum.Teleporter;

    public void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    public void Teleport()
    {
        Player.RpcCustomSnapTo(TeleportPoint);
        Utils.Flash(Color);
        TeleportButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    public bool MarkCondition()
    {
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)).ToArray();
        return hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != Player.transform.position;
    }

    public bool TeleUsable1() => IsTele;

    public bool TeleUsable2() => IsTele && TeleportPoint != Vector3.zero;

    public bool TeleCondition() => Player.transform.position != TeleportPoint;

    // Ambusher Stuff
    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }
    public bool IsAmb => FormerRole?.Type == LayerEnum.Ambusher;

    public void UnAmbush() => AmbushedPlayer = null;

    public void Ambush()
    {
        var cooldown = Interact(Player, AmbushButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            AmbushedPlayer = AmbushButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);
    }

    public bool AmbushException(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !CustomGameOptions.AmbushMates && Faction is Faction.Intruder or Faction.Syndicate)
        || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.AmbushMates);

    public bool AmbUsable() => IsAmb;

    public bool AmbEnd() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    // Consort Stuff
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomMenu BlockMenu { get; set; }
    public bool IsCons => FormerRole?.Type == LayerEnum.Consort;

    public bool ConsException(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

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
    public bool IsEnf => FormerRole?.Type == LayerEnum.Enforcer;

    public bool EnfException(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void BoomStart()
    {
        if (CustomPlayer.Local == BombedPlayer && !Dead)
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

    public void Bomb()
    {
        var cooldown = Interact(Player, BombButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BombedPlayer = BombButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else
            BombButton.StartCooldown(cooldown);
    }

    public bool EnfUsable() => IsEnf;

    public bool BoomEnd() => (BombedPlayer && BombedPlayer.HasDied()) || Dead || BombSuccessful;
}