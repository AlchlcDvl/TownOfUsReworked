namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedGodfather : Intruder
{
    public PromotedGodfather(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderHead;
        BlockMenu = new(Player, ConsClick, ConsException);
        TeleportPoint = Vector3.zero;
        Investigated = new();
        FlashedPlayers = new();
        Vents = new();
        CopiedPlayer = null;
        DisguisedPlayer = null;
        MorphedPlayer = null;
        SampledPlayer = null;
        MeasuredPlayer = null;
        AmbushedPlayer = null;
        BombedPlayer = null;
        BlockTarget = null;
        BlackmailedPlayer = null;
        BlockButton = new(this, "ConsortRoleblock", AbilityTypes.Targetless, "Secondary", Roleblock, CustomGameOptions.ConsortCd, CustomGameOptions.ConsortDur, (CustomButton.EffectVoid)Block,
            UnBlock);
        BombButton = new(this, "Enforce", AbilityTypes.Alive, "Secondary", Bomb, CustomGameOptions.EnforceCd, CustomGameOptions.EnforceDur, BoomStart, UnBoom, CustomGameOptions.EnforceDelay,
            EnfException);
        BlackmailButton = new(this, "Blackmail", AbilityTypes.Alive, "Secondary", Blackmail, CustomGameOptions.BlackmailCd, BMException);
        CamouflageButton = new(this, "Camouflage", AbilityTypes.Targetless, "Secondary", HitCamouflage, CustomGameOptions.CamouflagerCd, CustomGameOptions.CamouflageDur,
            (CustomButton.EffectVoid)Camouflage, UnCamouflage);
        FlashButton = new(this, "Flash", AbilityTypes.Targetless, "Secondary", HitFlash, CustomGameOptions.FlashCd, CustomGameOptions.FlashDur, Flash, StartFlash, UnFlash);
        DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag, CustomGameOptions.DragCd);
        DropButton = new(this, "Drop", AbilityTypes.Targetless, "Tertiary", Drop);
        CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean, CustomGameOptions.CleanCd);
        DisguiseButton = new(this, "Disguise", AbilityTypes.Alive, "Secondary", HitDisguise, CustomGameOptions.DisguiseCd, CustomGameOptions.DisguiseDur, Disguise, UnDisguise,
            CustomGameOptions.DisguiseDelay, DisgException);
        MeasureButton = new(this, "Measure", AbilityTypes.Alive, "Tertiary", Measure, CustomGameOptions.MeasureCd, MeasureException);
        MorphButton = new(this, "Morph", AbilityTypes.Targetless, "Secondary", HitMorph, CustomGameOptions.MorphCd, CustomGameOptions.MorphDur, (CustomButton.EffectVoid)Morph, UnMorph);
        SampleButton = new(this, "Sample", AbilityTypes.Alive, "Tertiary", Sample, CustomGameOptions.SampleCd, MorphException);
        InvisButton = new(this, "Invis", AbilityTypes.Targetless, "Secondary", HitInvis, CustomGameOptions.InvisCd, CustomGameOptions.InvisDur, (CustomButton.EffectVoid)Invis, UnInvis);
        AmbushButton = new(this, "Ambush", AbilityTypes.Alive, "Secondary", Ambush, CustomGameOptions.AmbushCd, CustomGameOptions.AmbushDur, UnAmbush, AmbushException);
        InvestigateButton = new(this, "Investigate", AbilityTypes.Alive, "Secondary", Investigate, CustomGameOptions.InvestigateCd, ConsigException);
        MineButton = new(this, Miner.SpriteName, AbilityTypes.Targetless, "Secondary", Mine, CustomGameOptions.MineCd);
        MarkButton = new(this, "Mark", AbilityTypes.Targetless, "Secondary", Mark, CustomGameOptions.TeleMarkCd);
        TeleportButton = new(this, "Teleport", AbilityTypes.Targetless, "Secondary", Teleport, CustomGameOptions.TeleportCd);
    }

    //PromotedGodfather Stuff
    public Role FormerRole { get; set; }

    public override UColor Color
    {
        get
        {
            if (!ClientGameOptions.CustomIntColors)
                return CustomColorManager.Intruder;
            else if (FormerRole != null)
                return FormerRole.Color;
            else
                return CustomColorManager.Godfather;
        }
    }
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.PromotedGodfather;
    public override Func<string> StartText => () => "Lead The <color=#FF1919FF>Intruders</color>";
    public override Func<string> Description => () => "- You have succeeded the former <color=#404C08FF>Godfather</color> and have a shorter cooldown on your former role's abilities"
        + (FormerRole == null ? "" : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");

    public override void TryEndEffect()
    {
        BlockButton.Update3((BlockTarget != null && BlockTarget.HasDied()) || IsDead);
        AmbushButton.Update3(IsDead || (AmbushedPlayer != null && AmbushedPlayer.HasDied()));
        BombButton.Update3((BombedPlayer != null && BombedPlayer.HasDied()) || IsDead || BombSuccessful);
        MorphButton.Update3(IsDead);
        DisguiseButton.Update3((DisguisedPlayer != null && DisguisedPlayer.HasDied()) || IsDead);
        InvisButton.Update3(IsDead);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MarkButton.Update2("MARK SPOT", IsTele, MarkCondition());
        TeleportButton.Update2("TELEPORT", TeleportPoint != Vector3.zero, Player.transform.position != TeleportPoint);
        MineButton.Update2("MINE VENT", IsMiner, MineCondition());
        BlockButton.Update2(BlockTarget == null ? "SET TARGET" : "ROLEBLOCK");
        InvestigateButton.Update2("INVESTIGATE", IsConsig);
        BombButton.Update2("SET BOMB", IsEnf);
        AmbushButton.Update2("AMBUSH", IsAmb);
        SampleButton.Update2("SAMPLE", IsMorph);
        MorphButton.Update2("MORPH", SampledPlayer != null && IsMorph);
        FlashButton.Update2("FLASH", IsGren, !Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !CustomGameOptions.SaboFlash);
        MeasureButton.Update2("MEASURE", IsDisg);
        DisguiseButton.Update2("DISGUISE", MeasuredPlayer != null && IsDisg);
        BlackmailButton.Update2("BLACKMAIL", IsBM);
        CamouflageButton.Update2("CAMOUFLAGE", IsCamo, !HudUpdate.IsCamoed);
        CleanButton.Update2("CLEAN BODY", CurrentlyDragging == null && IsJani, difference: LastImp && CustomGameOptions.SoloBoost && !IsDead ? -CustomGameOptions.UnderdogKillBonus : 0);
        DragButton.Update2("DRAG BODY", CurrentlyDragging == null && IsJani);
        DropButton.Update2("DROP BODY", CurrentlyDragging != null && IsJani);
        InvisButton.Update2("INVISIBILITY", IsWraith);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (BlockTarget != null && !BlockButton.EffectActive && IsCons)
                BlockTarget = null;

            LogMessage("Removed a target");
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
                CurrentlyDragging.gameObject.AddComponent<DragBehaviour>().SetDrag(Player, CurrentlyDragging);
                break;

            case GFActionsRPC.Ambush:
                AmbushedPlayer = reader.ReadPlayer();
                break;

            default:
                LogError($"Received unknown RPC - {gfAction}");
                break;
        }
    }

    //Impostor Stuff
    public bool IsImp => FormerRole?.Type == LayerEnum.Impostor;

    //Blackmailer Stuff
    public CustomButton BlackmailButton { get; set; }
    public PlayerControl BlackmailedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public UColor PrevColor { get; set; }
    public bool IsBM => FormerRole?.Type == LayerEnum.Blackmailer;

    public void Blackmail()
    {
        var cooldown = Interact(Player, BlackmailButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, GFActionsRPC.Blackmail, BlackmailedPlayer);
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    public bool BMException(PlayerControl player) => player == BlackmailedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        CustomGameOptions.BlackmailMates) || (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.BlackmailMates);

    //Camouflager Stuff
    public CustomButton CamouflageButton { get; set; }
    public bool IsCamo => FormerRole?.Type == LayerEnum.Camouflager;

    public void Camouflage()
    {
        HudUpdate.GodfatherEnabled = true;
        Utils.Camouflage();
    }

    public void UnCamouflage()
    {
        HudUpdate.GodfatherEnabled = false;
        DefaultOutfitAll();
    }

    public void HitCamouflage()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, CamouflageButton);
        CamouflageButton.Begin();
    }

    //Grenadier Stuff
    public CustomButton FlashButton { get; set; }
    public List<PlayerControl> FlashedPlayers { get; set; }
    public bool IsGren => FormerRole?.Type == LayerEnum.Grenadier;

    public void Flash()
    {
        foreach (var player in FlashedPlayers)
        {
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
                else if (FlashButton.EffectTime.IsInRange(0.5f, CustomGameOptions.FlashDur - 0.5f))
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

    private bool ShouldPlayerBeDimmed(PlayerControl player) => ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && !Meeting;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player) && !Meeting;

    public void UnFlash()
    {
        FlashedPlayers.Clear();
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, FlashButton);
        FlashButton.Begin();
    }

    public void StartFlash() => FlashedPlayers = GetClosestPlayers(Player.transform.position, CustomGameOptions.FlashRadius);

    //Janitor Stuff
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, GFActionsRPC.Drag, CurrentlyDragging);
        CurrentlyDragging.gameObject.AddComponent<DragBehaviour>().SetDrag(Player, CurrentlyDragging);
    }

    public void Drop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging);
        CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
        CurrentlyDragging = null;
        DragButton.StartCooldown();
    }

    //Disguiser Stuff
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, DisguiseButton, CopiedPlayer, DisguisedPlayer);
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

    //Morphling Stuff
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, MorphButton, MorphedPlayer);
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

    //Wraith Stuff
    public CustomButton InvisButton { get; set; }
    public bool IsWraith => FormerRole?.Type == LayerEnum.Wraith;

    public void Invis() => Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    public void UnInvis() => DefaultOutfit(Player);

    public void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, InvisButton);
        InvisButton.Begin();
    }

    //Miner Stuff
    public CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; set; }
    public bool IsMiner => FormerRole?.Type == LayerEnum.Miner;

    public void Mine()
    {
        RpcSpawnVent(this);
        MineButton.StartCooldown();
    }

    public bool MineCondition()
    {
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)).ToArray();
        return hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && !Vents.Any(x => x.transform.position == Player.transform.position);
    }

    //Consigliere Stuff
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
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    //Teleporter Stuff
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
        Player.NetTransform.RpcSnapTo(TeleportPoint);
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

    //Ambusher Stuff
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);
    }

    public bool AmbushException(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !CustomGameOptions.AmbushMates && Faction is Faction.Intruder or Faction.Syndicate)
        || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.AmbushMates);

    //Consort Stuff
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomMenu BlockMenu { get; set; }
    public bool IsCons => FormerRole?.Type == LayerEnum.Consort;

    public bool ConsException(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public void UnBlock()
    {
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

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
        if (BlockTarget == null)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    //Enforcer Stuff
    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public bool BombSuccessful { get; set; }
    public bool IsEnf => FormerRole?.Type == LayerEnum.Enforcer;

    public bool EnfException(PlayerControl player) => player == BombedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void BoomStart()
    {
        if (CustomPlayer.Local == BombedPlayer && !IsDead)
        {
            Utils.Flash(Color);
            GetRole(BombedPlayer).Bombed = true;
        }
    }

    public void UnBoom()
    {
        if (!BombSuccessful)
            Enforcer.Explode(BombedPlayer, Player);

        GetRole(BombedPlayer).Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    public void Bomb()
    {
        var cooldown = Interact(Player, BombButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BombedPlayer = BombButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else
            BombButton.StartCooldown(cooldown);
    }
}