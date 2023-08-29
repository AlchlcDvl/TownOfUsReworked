namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedGodfather : Intruder
{
    public PromotedGodfather(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.IntruderSupport;
        BlockMenu = new(Player, ConsClick, Exception1);
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
        BlockButton = new(this, "ConsortRoleblock", AbilityTypes.Effect, "Secondary", Roleblock);
        BombButton = new(this, "Enforce", AbilityTypes.Direct, "Secondary", Bomb, Exception7);
        BlackmailButton = new(this, "Blackmail", AbilityTypes.Direct, "Secondary", Blackmail, Exception2);
        CamouflageButton = new(this, "Camouflage", AbilityTypes.Effect, "Secondary", HitCamouflage);
        FlashButton = new(this, "Flash", AbilityTypes.Effect, "Secondary", HitFlash);
        CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean);
        DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag);
        DropButton = new(this, "Drop", AbilityTypes.Effect, "Tertiary", Drop);
        DisguiseButton = new(this, "Disguise", AbilityTypes.Direct, "Secondary", HitDisguise, Exception3);
        MeasureButton = new(this, "Measure", AbilityTypes.Direct, "Tertiary", Measure, Exception4);
        MorphButton = new(this, "Morph", AbilityTypes.Effect, "Secondary", HitMorph);
        SampleButton = new(this, "Sample", AbilityTypes.Direct, "Tertiary", Sample, Exception5);
        InvisButton = new(this, "Invis", AbilityTypes.Effect, "Secondary", HitInvis);
        AmbushButton = new(this, "Ambush", AbilityTypes.Direct, "Secondary", HitAmbush, Exception6);
        InvestigateButton = new(this, "Investigate", AbilityTypes.Direct, "Secondary", Investigate, Exception8);
        MineButton = new(this, "Mine", AbilityTypes.Effect, "Secondary", Mine);
        MarkButton = new(this, "Mark", AbilityTypes.Effect, "Secondary", Mark);
        TeleportButton = new(this, "Teleport", AbilityTypes.Effect, "Secondary", Teleport);
    }

    //PromotedGodfather Stuff
    public Role FormerRole { get; set; }
    public float TimeRemaining { get; set; }
    public bool OnEffect => TimeRemaining > 0f;
    public float TimeRemaining2 { get; set; }
    public bool DelayActive => TimeRemaining2 > 0f;
    public bool Enabled { get; set; }

    public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.PromotedGodfather;
    public override Func<string> StartText => () => "Lead The <color=#FF0000FF>Intruders</color>";
    public override Func<string> Description => () => "- You have succeeded the former <color=#404C08FF>Godfather</color> and have a shorter cooldown on your former role's abilities"
        + (FormerRole == null ? "" : $"\n{FormerRole.Description()}");
    public override InspectorResults InspectorResults => FormerRole == null ? InspectorResults.LeadsTheGroup : FormerRole.InspectorResults;

    public bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public bool Exception2(PlayerControl player) => player == BlackmailedPlayer || ((player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None)) &&
        CustomGameOptions.BlackmailMates);

    public bool Exception3(PlayerControl player) => (player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction) &&
        CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders);

    public bool Exception4(PlayerControl player) => player == MeasuredPlayer;

    public bool Exception5(PlayerControl player) => player == SampledPlayer;

    public bool Exception6(PlayerControl player) => player == AmbushedPlayer;

    public bool Exception7(PlayerControl player) => player == BombedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public bool Exception8(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system.dummy.IsActive;
        var sabActive = system.specials.Any(s => s.IsActive);
        var condition = !dummyActive && !sabActive;
        var flag = BlockTarget == null;
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not 8 and not 5).ToArray();
        CanPlace = hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator;
        CanMark = CanPlace && TeleportPoint != Player.transform.position;
        MarkButton.Update("MARK", MarkTimer, CustomGameOptions.TeleMarkCd, CanMark, IsTele);
        TeleportButton.Update("TELEPORT", TeleportTimer, CustomGameOptions.TeleportCd, true, TeleportPoint != Vector3.zero && IsTele && TeleportPoint != Player.transform.position);
        MineButton.Update("MINE", MineTimer, CustomGameOptions.MineCd, CanPlace, IsMiner);
        BlockButton.Update(flag ? "SET TARGET" : "ROLEBLOCK", RoleblockTimer, CustomGameOptions.ConsortCd, OnEffect, TimeRemaining, CustomGameOptions.ConsortDur,
            true, IsCons);
        InvestigateButton.Update("INVESTIGATE", ConsigliereTimer, CustomGameOptions.InvestigateCd, true, IsConsig);
        BombButton.Update("BOMB", BombTimer, CustomGameOptions.EnforceCd, DelayActive || OnEffect, DelayActive ? TimeRemaining2 : TimeRemaining, DelayActive ?
            CustomGameOptions.EnforceDelay : CustomGameOptions.EnforceDur, true, IsEnf);
        AmbushButton.Update("AMBUSH", AmbushTimer, CustomGameOptions.AmbushDur, OnEffect, TimeRemaining, CustomGameOptions.AmbushDur, true, IsAmb);
        MorphButton.Update("MORPH", MorphTimer, CustomGameOptions.MorphCd, OnEffect, TimeRemaining, CustomGameOptions.MorphDur, true, SampledPlayer != null && IsMorph);
        SampleButton.Update("SAMPLE", SampleTimer, CustomGameOptions.SampleCd, true, IsMorph);
        FlashButton.Update("FLASH", FlashTimer, CustomGameOptions.FlashCd, OnEffect, TimeRemaining, CustomGameOptions.FlashDur, condition, IsGren);
        DisguiseButton.Update("DISGUISE", DisguiseTimer, CustomGameOptions.DisguiseCd, DelayActive || OnEffect, DelayActive ? TimeRemaining2 : TimeRemaining, DelayActive ?
            CustomGameOptions.DisguiseDelay : CustomGameOptions.DisguiseDur, true, MeasuredPlayer != null && IsDisg);
        MeasureButton.Update("MEASURE", MeasureTimer, CustomGameOptions.MeasureCd, true, IsDisg);
        BlackmailButton.Update("BLACKMAIL", BlackmailTimer, CustomGameOptions.BlackmailCd, true, IsBM);
        CamouflageButton.Update("CAMOUFLAGE", CamouflageTimer, CustomGameOptions.CamouflagerCd, OnEffect, TimeRemaining, CustomGameOptions.CamouflageDur, !DoUndo.IsCamoed,
            IsCamo);
        CleanButton.Update("CLEAN", CleanTimer, CustomGameOptions.CleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0, true,
            CurrentlyDragging == null && IsJani);
        DragButton.Update("DRAG", DragTimer, CustomGameOptions.DragCd, true, CurrentlyDragging == null && IsJani);
        DropButton.Update("DROP", true, CurrentlyDragging != null && IsJani);
        InvisButton.Update("INVIS", InvisTimer, CustomGameOptions.InvisCd, OnEffect, TimeRemaining, CustomGameOptions.InvisDur, true, IsWraith);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (BlockTarget != null && !OnEffect && IsCons)
                BlockTarget = null;

            LogInfo("Removed a target");
        }
    }

    //Impostor Stuff
    public bool IsImp => FormerRole?.Type == LayerEnum.Impostor;

    //Blackmailer Stuff
    public CustomButton BlackmailButton { get; set; }
    public PlayerControl BlackmailedPlayer { get; set; }
    public DateTime LastBlackmailed { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }
    public bool IsBM => FormerRole?.Type == LayerEnum.Blackmailer;
    public float BlackmailTimer => ButtonUtils.Timer(Player, LastBlackmailed, CustomGameOptions.BlackmailCd);

    public void Blackmail()
    {
        if (BlackmailTimer != 0f || IsTooFar(Player, BlackmailButton.TargetPlayer) || BlackmailButton.TargetPlayer == BlackmailedPlayer)
            return;

        var interact = Interact(Player, BlackmailButton.TargetPlayer);

        if (interact[3])
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Blackmail, this, BlackmailedPlayer);
        }

        if (interact[0])
            LastBlackmailed = DateTime.UtcNow;
        else if (interact[1])
            LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Camouflager Stuff
    public CustomButton CamouflageButton { get; set; }
    public DateTime LastCamouflaged { get; set; }
    public bool IsCamo => FormerRole?.Type == LayerEnum.Camouflager;
    public float CamouflageTimer => ButtonUtils.Timer(Player, LastCamouflaged, CustomGameOptions.CamouflagerCd);

    public void Camouflage()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        Utils.Camouflage();

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void UnCamouflage()
    {
        Enabled = false;
        LastCamouflaged = DateTime.UtcNow;
        DefaultOutfitAll();
    }

    public void HitCamouflage()
    {
        if (CamouflageTimer != 0f || DoUndo.IsCamoed)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Camouflage, this);
        TimeRemaining = CustomGameOptions.CamouflageDur;
        Camouflage();
    }

    //Grenadier Stuff
    public CustomButton FlashButton { get; set; }
    public DateTime LastFlashed { get; set; }
    private static Color32 NormalVision => new(212, 212, 212, 0);
    private static Color32 DimVision => new(212, 212, 212, 51);
    private static Color32 BlindVision => new(212, 212, 212, 255);
    public List<PlayerControl> FlashedPlayers { get; set; }
    public bool IsGren => FormerRole?.Type == LayerEnum.Grenadier;
    public float FlashTimer => ButtonUtils.Timer(Player, LastFlashed, CustomGameOptions.FlashCd);

    public void Flash()
    {
        if (!Enabled)
            FlashedPlayers = GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting)
            TimeRemaining = 0f;

        //To stop the scenario where the flash and sabotage are called at the same time.
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system.dummy.IsActive;
        var sabActive = system.specials.Any(s => s.IsActive);

        if (sabActive || dummyActive)
            return;

        foreach (var player in FlashedPlayers)
        {
            if (CustomPlayer.Local == player)
            {
                if (TimeRemaining > CustomGameOptions.FlashDur - 0.5f)
                {
                    var fade = (TimeRemaining - CustomGameOptions.FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (TimeRemaining <= (CustomGameOptions.FlashDur - 0.5f) && TimeRemaining >= 0.5f)
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = DimVision;
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (TimeRemaining < 0.5f)
                {
                    var fade2 = (TimeRemaining * -2.0f) + 1.0f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else
                {
                    HUD.FullScreen.color = NormalVision;
                    TimeRemaining = 0f;
                }

                if (Map)
                    Map.Close();

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }
        }
    }

    private static bool ShouldPlayerBeDimmed(PlayerControl player) => (player.Is(Faction.Intruder) || player.Data.IsDead) && !Meeting;

    private static bool ShouldPlayerBeBlinded(PlayerControl player) => !(player.Is(Faction.Intruder) || player.Data.IsDead || Meeting);

    public void UnFlash()
    {
        Enabled = false;
        LastFlashed = DateTime.UtcNow;
        FlashedPlayers.Clear();
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system.dummy.IsActive;
        var sabActive = system.specials.Any(s => s.IsActive);

        if (sabActive || dummyActive || FlashTimer != 0f)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.FlashGrenade, this);
        TimeRemaining = CustomGameOptions.FlashDur;
        Flash();
    }

    //Janitor Stuff
    public CustomButton CleanButton { get; set; }
    public DateTime LastCleaned { get; set; }
    public DateTime LastDragged { get; set; }
    public CustomButton DragButton { get; set; }
    public CustomButton DropButton { get; set; }
    public DeadBody CurrentlyDragging { get; set; }
    public bool IsJani => FormerRole?.Type == LayerEnum.Janitor;
    public float CleanTimer => ButtonUtils.Timer(Player, LastCleaned, CustomGameOptions.CleanCd, LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus :
        0);
    public float DragTimer => ButtonUtils.Timer(Player, LastDragged, CustomGameOptions.DragCd);

    public void Clean()
    {
        if (CleanTimer != 0f || IsTooFar(Player, CleanButton.TargetBody))
            return;

        Spread(Player, PlayerByBody(CleanButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, this, CleanButton.TargetBody);
        Coroutines.Start(FadeBody(CleanButton.TargetBody));
        LastCleaned = DateTime.UtcNow;

        if (CustomGameOptions.JaniCooldownsLinked)
            LastKilled = DateTime.UtcNow;
    }

    public void Drag()
    {
        if (IsTooFar(Player, DragButton.TargetBody) || CurrentlyDragging || DragTimer != 0f)
            return;

        CurrentlyDragging = DragButton.TargetBody;
        Spread(Player, PlayerByBody(CurrentlyDragging));
        CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Drag, this, CurrentlyDragging);
        var drag = CurrentlyDragging.gameObject.AddComponent<DragBehaviour>();
        drag.Source = Player;
        drag.Dragged = CurrentlyDragging;
    }

    public void Drop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.Drop, CurrentlyDragging);
        CurrentlyDragging.gameObject.GetComponent<DragBehaviour>().Destroy();
        CurrentlyDragging = null;
        LastDragged = DateTime.UtcNow;
    }

    //Disguiser Stuff
    public CustomButton DisguiseButton { get; set; }
    public DateTime LastDisguised { get; set; }
    public PlayerControl MeasuredPlayer { get; set; }
    public PlayerControl DisguisedPlayer { get; set; }
    public PlayerControl CopiedPlayer { get; set; }
    public CustomButton MeasureButton { get; set; }
    public DateTime LastMeasured { get; set; }
    public bool IsDisg => FormerRole?.Type == LayerEnum.Disguiser;
    public float DisguiseTimer => ButtonUtils.Timer(Player, LastDisguised, CustomGameOptions.DisguiseCd);
    public float MeasureTimer => ButtonUtils.Timer(Player, LastMeasured, CustomGameOptions.MeasureCd);

    public void Disguise()
    {
        TimeRemaining -= Time.deltaTime;
        Utils.Morph(DisguisedPlayer, CopiedPlayer);

        if (IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected || Meeting)
            TimeRemaining = 0f;
    }

    public void DisgDelay()
    {
        TimeRemaining2 -= Time.deltaTime;

        if (IsDead || DisguisedPlayer.Data.IsDead || DisguisedPlayer.Data.Disconnected || Meeting)
            TimeRemaining2 = 0f;
    }

    public void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        LastDisguised = DateTime.UtcNow;

        if (CustomGameOptions.DisgCooldownsLinked)
            LastMeasured = DateTime.UtcNow;
    }

    public void HitDisguise()
    {
        if (DisguiseTimer != 0f || IsTooFar(Player, DisguiseButton.TargetPlayer) || DisguiseButton.TargetPlayer == MeasuredPlayer || OnEffect || DelayActive)
            return;

        var interact = Interact(Player, DisguiseButton.TargetPlayer);

        if (interact[3])
        {
            TimeRemaining = CustomGameOptions.DisguiseDur;
            TimeRemaining2 = CustomGameOptions.DisguiseDelay;
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = DisguiseButton.TargetPlayer;
            DisgDelay();
            CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Disguise, this, CopiedPlayer, DisguisedPlayer);

            if (CustomGameOptions.DisgCooldownsLinked)
                LastMeasured = DateTime.UtcNow;
        }
        else if (interact[0])
        {
            LastDisguised = DateTime.UtcNow;

            if (CustomGameOptions.DisgCooldownsLinked)
                LastMeasured = DateTime.UtcNow;
        }
        else if (interact[1])
        {
            LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

            if (CustomGameOptions.DisgCooldownsLinked)
                LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    public void Measure()
    {
        if (MeasureTimer != 0f || IsTooFar(Player, MeasureButton.TargetPlayer) || MeasureButton.TargetPlayer == MeasuredPlayer)
            return;

        var interact = Interact(Player, MeasureButton.TargetPlayer);

        if (interact[3])
            MeasuredPlayer = MeasureButton.TargetPlayer;

        if (interact[0])
        {
            LastMeasured = DateTime.UtcNow;

            if (CustomGameOptions.DisgCooldownsLinked)
                LastDisguised = DateTime.UtcNow;
        }
        else if (interact[1])
        {
            LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);

            if (CustomGameOptions.DisgCooldownsLinked)
                LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    //Morphling Stuff
    public CustomButton MorphButton { get; set; }
    public DateTime LastMorphed { get; set; }
    public PlayerControl MorphedPlayer { get; set; }
    public PlayerControl SampledPlayer { get; set; }
    public DateTime LastSampled { get; set; }
    public CustomButton SampleButton { get; set; }
    public bool IsMorph => FormerRole?.Type == LayerEnum.Morphling;
    public float MorphTimer => ButtonUtils.Timer(Player, LastMorphed, CustomGameOptions.MorphCd);
    public float SampleTimer => ButtonUtils.Timer(Player, LastSampled, CustomGameOptions.SampleCd);

    public void Morph()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        Utils.Morph(Player, MorphedPlayer);

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void Unmorph()
    {
        Enabled = false;
        MorphedPlayer = null;
        DefaultOutfit(Player);
        LastMorphed = DateTime.UtcNow;

        if (CustomGameOptions.MorphCooldownsLinked)
            LastSampled = DateTime.UtcNow;
    }

    public void HitMorph()
    {
        if (MorphTimer != 0f || SampledPlayer == null || OnEffect)
            return;

        TimeRemaining = CustomGameOptions.MorphDur;
        MorphedPlayer = SampledPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Morph, this, MorphedPlayer);
        Morph();
    }

    public void Sample()
    {
        if (SampleTimer != 0f || IsTooFar(Player, SampleButton.TargetPlayer) || SampledPlayer == SampleButton.TargetPlayer)
            return;

        var interact = Interact(Player, SampleButton.TargetPlayer);

        if (interact[3])
            SampledPlayer = SampleButton.TargetPlayer;

        if (interact[0])
        {
            LastSampled = DateTime.UtcNow;

            if (CustomGameOptions.MorphCooldownsLinked)
                LastMorphed = DateTime.UtcNow;
        }
        else if (interact[1])
        {
            LastSampled.AddSeconds(CustomGameOptions.ProtectKCReset);

            if (CustomGameOptions.MorphCooldownsLinked)
                LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    //Wraith Stuff
    public CustomButton InvisButton { get; set; }
    public DateTime LastInvis { get; set; }
    public bool IsWraith => FormerRole?.Type == LayerEnum.Wraith;
    public float InvisTimer => ButtonUtils.Timer(Player, LastInvis, CustomGameOptions.InvisCd);

    public void Invis()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void Uninvis()
    {
        Enabled = false;
        LastInvis = DateTime.UtcNow;
        DefaultOutfit(Player);
    }

    public void HitInvis()
    {
        if (InvisTimer != 0f || OnEffect)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Invis, this);
        TimeRemaining = CustomGameOptions.InvisDur;
        Invis();
    }

    //Consigliere Stuff
    public List<byte> Investigated { get; set; }
    public CustomButton InvestigateButton { get; set; }
    public DateTime LastInvestigated { get; set; }
    public bool IsConsig => FormerRole?.Type == LayerEnum.Consigliere;
    public float ConsigliereTimer => ButtonUtils.Timer(Player, LastInvestigated, CustomGameOptions.InvestigateCd);

    public void Investigate()
    {
        if (ConsigliereTimer != 0f || IsTooFar(Player, InvestigateButton.TargetPlayer) || Investigated.Contains(InvestigateButton.TargetPlayer.PlayerId))
            return;

        var interact = Interact(Player, InvestigateButton.TargetPlayer);

        if (interact[3])
            Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

        if (interact[0])
            LastInvestigated = DateTime.UtcNow;
        else if (interact[1])
            LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Miner Stuff
    public CustomButton MineButton { get; set; }
    public DateTime LastMined { get; set; }
    public bool CanPlace { get; set; }
    public List<Vent> Vents { get; set; }
    public bool IsMiner => FormerRole?.Type == LayerEnum.Miner;
    public float MineTimer => ButtonUtils.Timer(Player, LastMined, CustomGameOptions.MineCd);

    public void Mine()
    {
        if (!CanPlace || MineTimer != 0f)
            return;

        RpcSpawnVent(this);
        LastMined = DateTime.UtcNow;
    }

    //Teleporter Stuff
    public CustomButton TeleportButton { get; set; }
    public DateTime LastTeleported { get; set; }
    public Vector3 TeleportPoint { get; set; }
    public DateTime LastMarked { get; set; }
    public CustomButton MarkButton { get; set; }
    public bool CanMark { get; set; }
    public bool IsTele => FormerRole?.Type == LayerEnum.Teleporter;
    public float MarkTimer => ButtonUtils.Timer(Player, LastMarked, CustomGameOptions.TeleMarkCd);
    public float TeleportTimer => ButtonUtils.Timer(Player, LastTeleported, CustomGameOptions.TeleportCd);

    public void Mark()
    {
        if (!CanMark || MarkTimer != 0f || TeleportPoint == Player.transform.position)
            return;

        TeleportPoint = Player.transform.position;
        LastMarked = DateTime.UtcNow;

        if (CustomGameOptions.TeleCooldownsLinked)
            LastTeleported = DateTime.UtcNow;
    }

    public void Teleport()
    {
        if (TeleportTimer != 0f || TeleportPoint == Player.transform.position || TeleportPoint == Vector3.zero)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Teleport, this, TeleportPoint);
        LastTeleported = DateTime.UtcNow;
        Utils.Teleport(Player, TeleportPoint);

        if (CustomGameOptions.TeleCooldownsLinked)
            LastMarked = DateTime.UtcNow;
    }

    //Ambusher Stuff
    public DateTime LastAmbushed { get; set; }
    public PlayerControl AmbushedPlayer { get; set; }
    public CustomButton AmbushButton { get; set; }
    public bool IsAmb => FormerRole?.Type == LayerEnum.Ambusher;
    public float AmbushTimer => ButtonUtils.Timer(Player, LastAmbushed, CustomGameOptions.AmbushCd);

    public void Ambush()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || Meeting)
            TimeRemaining = 0f;
    }

    public void UnAmbush()
    {
        Enabled = false;
        LastAmbushed = DateTime.UtcNow;
        AmbushedPlayer = null;
    }

    public void HitAmbush()
    {
        if (AmbushTimer != 0f || IsTooFar(Player, AmbushButton.TargetPlayer) || AmbushButton.TargetPlayer == AmbushedPlayer)
            return;

        var interact = Interact(Player, AmbushButton.TargetPlayer);

        if (interact[3])
        {
            TimeRemaining = CustomGameOptions.AmbushDur;
            AmbushedPlayer = AmbushButton.TargetPlayer;
            Ambush();
            CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.Ambush, this, AmbushedPlayer);
        }
        else if (interact[0])
            LastAmbushed = DateTime.UtcNow;
        else if (interact[1])
            LastAmbushed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Consort Stuff
    public DateTime LastBlocked { get; set; }
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomMenu BlockMenu { get; set; }
    public bool IsCons => FormerRole?.Type == LayerEnum.Consort;
    public float RoleblockTimer => ButtonUtils.Timer(Player, LastBlocked, CustomGameOptions.ConsortCd);

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

        if (IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || Meeting || !BlockTarget.IsBlocked())
            TimeRemaining = 0f;
    }

    public void ConsClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            BlockTarget = player;
        else if (interact[0])
            LastBlocked = DateTime.UtcNow;
        else if (interact[1])
            LastBlocked.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void Roleblock()
    {
        if (RoleblockTimer != 0f)
            return;

        if (BlockTarget == null)
            BlockMenu.Open();
        else
        {
            TimeRemaining = CustomGameOptions.ConsortDur;
            Block();
            CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.ConsRoleblock, this, BlockTarget);
        }
    }

    //Enforcer Stuff
    public CustomButton BombButton { get; set; }
    public PlayerControl BombedPlayer { get; set; }
    public DateTime LastBombed { get; set; }
    public bool BombSuccessful { get; set; }
    public bool IsEnf => FormerRole?.Type == LayerEnum.Enforcer;
    public float BombTimer => ButtonUtils.Timer(Player, LastBombed, CustomGameOptions.EnforceCd);

    public void Boom()
    {
        if (!Enabled && CustomPlayer.Local == BombedPlayer)
        {
            Utils.Flash(Color);
            GetRole(BombedPlayer).Bombed = true;
        }

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || Meeting || BombSuccessful)
            TimeRemaining = 0f;
    }

    public void BombDelay()
    {
        TimeRemaining2 -= Time.deltaTime;

        if (IsDead || Meeting)
            TimeRemaining2 = 0f;
    }

    public void Unboom()
    {
        Enabled = false;
        LastBombed = DateTime.UtcNow;
        GetRole(BombedPlayer).Bombed = false;

        if (!BombSuccessful)
            Explode();

        BombedPlayer = null;
    }

    private void Explode()
    {
        foreach (var player in GetClosestPlayers(BombedPlayer.GetTruePosition(), CustomGameOptions.EnforceRadius))
        {
            Spread(BombedPlayer, player);

            if (player.IsVesting() || player.IsProtected() || player.IsOnAlert() || player.IsShielded() || player.IsRetShielded())
                continue;

            if (!player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(BombedPlayer, player, DeathReasonEnum.Bombed, false);
        }
    }

    public void Bomb()
    {
        if (BombTimer != 0f || IsTooFar(Player, BombButton.TargetPlayer) || BombedPlayer == BombButton.TargetPlayer)
            return;

        var interact = Interact(Player, BombButton.TargetPlayer);

        if (interact[3])
        {
            TimeRemaining = CustomGameOptions.EnforceDur;
            TimeRemaining2 = CustomGameOptions.EnforceDelay;
            BombedPlayer = BombButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.GodfatherAction, GodfatherActionsRPC.SetBomb, this, BombedPlayer);
            BombDelay();
        }
        else if (interact[0])
            LastBombed = DateTime.UtcNow;
        else if (interact[1])
            LastBombed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }
}