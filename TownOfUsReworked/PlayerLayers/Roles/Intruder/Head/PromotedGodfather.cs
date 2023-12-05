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
        BombButton = new(this, "Enforce", AbilityTypes.Target, "Secondary", Bomb, CustomGameOptions.EnforceCd, CustomGameOptions.EnforceDur, BoomStart, UnBoom, CustomGameOptions.EnforceDelay,
            EnfException);
        BlackmailButton = new(this, "Blackmail", AbilityTypes.Target, "Secondary", Blackmail, CustomGameOptions.BlackmailCd, BMException);
        CamouflageButton = new(this, "Camouflage", AbilityTypes.Targetless, "Secondary", HitCamouflage, CustomGameOptions.CamouflagerCd, CustomGameOptions.CamouflageDur,
            (CustomButton.EffectVoid)Camouflage, UnCamouflage);
        FlashButton = new(this, "Flash", AbilityTypes.Targetless, "Secondary", HitFlash, CustomGameOptions.FlashCd, CustomGameOptions.FlashDur, Flash, StartFlash, UnFlash);
        DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag, CustomGameOptions.DragCd);
        DropButton = new(this, "Drop", AbilityTypes.Targetless, "Tertiary", Drop);
        CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean, CustomGameOptions.CleanCd);
        DisguiseButton = new(this, "Disguise", AbilityTypes.Target, "Secondary", HitDisguise, CustomGameOptions.DisguiseCd, CustomGameOptions.DisguiseDur, Disguise, UnDisguise,
            CustomGameOptions.DisguiseDelay, DisgException);
        MeasureButton = new(this, "Measure", AbilityTypes.Target, "Tertiary", Measure, CustomGameOptions.MeasureCd, MeasureException);
        MorphButton = new(this, "Morph", AbilityTypes.Targetless, "Secondary", HitMorph, CustomGameOptions.MorphCd, CustomGameOptions.MorphDur, (CustomButton.EffectVoid)Morph, UnMorph);
        SampleButton = new(this, "Sample", AbilityTypes.Target, "Tertiary", Sample, CustomGameOptions.SampleCd, MorphException);
        InvisButton = new(this, "Invis", AbilityTypes.Targetless, "Secondary", HitInvis, CustomGameOptions.InvisCd, CustomGameOptions.InvisDur, (CustomButton.EffectVoid)Invis, UnInvis);
        AmbushButton = new(this, "Ambush", AbilityTypes.Target, "Secondary", Ambush, CustomGameOptions.AmbushCd, CustomGameOptions.AmbushDur, UnAmbush, AmbushException);
        InvestigateButton = new(this, "Investigate", AbilityTypes.Target, "Secondary", Investigate, CustomGameOptions.InvestigateCd, ConsigException);
        MineButton = new(this, Miner.SpriteName, AbilityTypes.Targetless, "Secondary", Mine, CustomGameOptions.MineCd);
        MarkButton = new(this, "Mark", AbilityTypes.Targetless, "Secondary", Mark, CustomGameOptions.TeleMarkCd);
        TeleportButton = new(this, "Teleport", AbilityTypes.Targetless, "Secondary", Teleport, CustomGameOptions.TeleportCd);
    }

    //PromotedGodfather Stuff
    public Role FormerRole { get; set; }

    public override Color Color
    {
        get
        {
            if (!ClientGameOptions.CustomIntColors)
                return Colors.Intruder;
            else if (FormerRole != null)
                return FormerRole.Color;
            else
                return Colors.Godfather;
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
        FlashButton.Update2("FLASH", IsGren, FlashCondition());
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

            LogInfo("Removed a target");
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
                var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
                drag.Source = Player;
                drag.Dragged = CurrentlyDragging;
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
    public Color PrevColor { get; set; }
    public bool IsBM => FormerRole?.Type == LayerEnum.Blackmailer;

    public void Blackmail()
    {
        var interact = Interact(Player, BlackmailButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, GFActionsRPC.Blackmail, BlackmailedPlayer);
        }

        if (interact.Reset)
            BlackmailButton.StartCooldown();
        else if (interact.Protected)
            BlackmailButton.StartCooldown(CooldownType.GuardianAngel);
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
    private static Color32 NormalVision => new(212, 212, 212, 0);
    private static Color32 DimVision => new(212, 212, 212, 51);
    private static Color32 BlindVision => new(212, 212, 212, 255);
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
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, CustomGameOptions.FlashDur - 0.5f))
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = DimVision;
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2.0f) + 1.0f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else
                {
                    SetFullScreenHUD();
                    FlashButton.EffectTime = 0f;
                }

                if (MapPatch.MapActive)
                    Map.Close();

                if (ActiveTask)
                    ActiveTask.Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && Meeting;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead || Meeting);

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

    public bool FlashCondition() => Ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>()?.AnyActive == true;

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
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, this, CleanButton.TargetBody);
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
        var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
        drag.Source = Player;
        drag.Dragged = CurrentlyDragging;
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
        var interact = Interact(Player, DisguiseButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = DisguiseButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, DisguiseButton, CopiedPlayer, DisguisedPlayer);
            DisguiseButton.Begin();
        }
        else if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        if (!interact.AbilityUsed)
            MeasureButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public bool DisgException(PlayerControl player) => MeasureException(player) || (((player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) ||
        (!player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    public bool MeasureException(PlayerControl player) => player == MeasuredPlayer;

    public void Measure()
    {
        var interact = Interact(Player, MeasureButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            MeasuredPlayer = MeasureButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

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
        var interact = Interact(Player, SampleButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            SampledPlayer = SampleButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

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
        var interact = Interact(Player, InvestigateButton.TargetPlayer);

        if (interact.AbilityUsed)
            Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

        if (interact.Reset)
            InvestigateButton.StartCooldown();
        else if (interact.Protected)
            InvestigateButton.StartCooldown(CooldownType.GuardianAngel);
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
        var interact = Interact(Player, AmbushButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            AmbushedPlayer = AmbushButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else if (interact.Reset)
            AmbushButton.StartCooldown();
        else if (interact.Protected)
            AmbushButton.StartCooldown(CooldownType.GuardianAngel);
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
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            BlockTarget = player;
        else if (interact.Reset)
            BlockButton.StartCooldown();
        else if (interact.Protected)
            BlockButton.StartCooldown(CooldownType.GuardianAngel);
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
            Explode();

        GetRole(BombedPlayer).Bombed = false;
        BombedPlayer = null;
        BombSuccessful = false;
    }

    private void Explode()
    {
        foreach (var player in GetClosestPlayers(BombedPlayer.transform.position, CustomGameOptions.EnforceRadius))
        {
            Spread(BombedPlayer, player);

            if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsProtectedMonarch() || player.Is(LayerEnum.Pestilence))
                continue;

            RpcMurderPlayer(Player, player, DeathReasonEnum.Bombed, false);
        }
    }

    public void Bomb()
    {
        var interact = Interact(Player, BombButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            BombedPlayer = BombButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BombButton, BombedPlayer);
            BombButton.Begin();
        }
        else if (interact.Reset)
            BombButton.StartCooldown();
        else if (interact.Protected)
            BombButton.StartCooldown(CooldownType.GuardianAngel);
    }
}