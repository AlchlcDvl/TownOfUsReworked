namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedRebel : Syndicate
{
    public PromotedRebel(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateSupport;
        SpellCount = 0;
        Framed = new();
        UnwarpablePlayers = new();
        StalkerArrows = new();
        Spelled = new();
        Bombs = new();
        ConcealedPlayer = null;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        SilencedPlayer = null;
        PoisonedPlayer = null;
        Positive = null;
        Negative = null;
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        ConfusedPlayer = null;
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;
        WarpObj = new("Warp") { layer = 5 };
        WarpObj.AddSubmergedComponent("ElevatorMover");
        WarpObj.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying = WarpObj.AddComponent<SpriteRenderer>();
        AnimationPlaying.sprite = PortalAnimation[0];
        AnimationPlaying.material = HatManager.Instance.PlayerMaterial;
        WarpObj.SetActive(true);
        ConfuseMenu = new(Player, ConfuseClick, Exception12);
        WarpMenu1 = new(Player, WarpClick1, Exception5);
        WarpMenu2 = new(Player, WarpClick2, Exception6);
        ConcealMenu = new(Player, ConcealClick, Exception1);
        PoisonMenu = new(Player, PoisonClick, Exception4);
        ShapeshiftMenu1 = new(Player, ShapeshiftClick1, Exception2);
        ShapeshiftMenu2 = new(Player, ShapeshiftClick2, Exception3);
        SpellButton = new(this, "Spell", AbilityTypes.Direct, "Secondary", HitSpell, Exception9);
        StalkButton = new(this, "Stalk", AbilityTypes.Direct, "ActionSecondary", Stalk, Exception8);
        PositiveButton = new(this, "Positive", AbilityTypes.Direct, "ActionSecondary", SetPositive, Exception10);
        NegativeButton = new(this, "Negative", AbilityTypes.Direct, "Secondary", SetNegative, Exception11);
        ConcealButton = new(this, "Conceal", AbilityTypes.Effect, "Secondary", HitConceal);
        FrameButton = new(this, "Frame", AbilityTypes.Direct, "Secondary", Frame, Exception7);
        RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Effect, "Secondary", RadialFrame);
        ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Effect, "Secondary", HitShapeshift);
        BombButton = new(this, "Plant", AbilityTypes.Effect, "Secondary", Place);
        DetonateButton = new(this, "Detonate", AbilityTypes.Effect, "Tertiary", Detonate);
        CrusadeButton = new(this, "Crusade", AbilityTypes.Direct, "Secondary", HitCrusade);
        PoisonButton = new(this, "Poison", AbilityTypes.Direct, "ActionSecondary", HitPoison, Exception4);
        GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison, Exception14);
        WarpButton = new(this, "Warp", AbilityTypes.Effect, "Secondary", Warp);
        ConfuseButton = new(this, "Confuse", AbilityTypes.Effect, "Secondary", HitConfuse);
        TimeButton = new(this, "Time", AbilityTypes.Effect, "Secondary", TimeControl);
        SilenceButton = new(this, "Silence", AbilityTypes.Direct, "Secondary", Silence, Exception13);
        ChargeButton = new(this, "Charge", AbilityTypes.Effect, "Tertiary", Charge);
    }

    //Rebel Stuff
    public Role FormerRole { get; set; }
    public bool Enabled { get; set; }
    public float TimeRemaining { get; set; }
    public bool OnEffect => TimeRemaining > 0f;

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.PromotedRebel;
    public override Func<string> StartText => () => "Lead The <color=#008000FF>Syndicate</color>";
    public override Func<string> Description => () => "- You have succeeded the former <color=#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities" +
        (FormerRole == null ? "" : $"\n{FormerRole.Description()}");
    public override InspectorResults InspectorResults => FormerRole == null ? InspectorResults.LeadsTheGroup : FormerRole.InspectorResults;

    public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates);

    public bool Exception2(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction)
        && !CustomGameOptions.ShapeshiftMates);

    public bool Exception3(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction)
        && !CustomGameOptions.ShapeshiftMates);

    public bool Exception4(PlayerControl player) => player == PoisonedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public bool Exception5(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public bool Exception6(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public bool Exception7(PlayerControl player) => Framed.Contains(player.PlayerId) || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public bool Exception8(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public bool Exception9(PlayerControl player) => Spelled.Contains(player.PlayerId) || player.Is(Faction);

    public bool Exception10(PlayerControl player) => player == Negative || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public bool Exception11(PlayerControl player) => player == Positive || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public bool Exception12(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConfuseImmunity);

    public bool Exception13(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction != Faction.Crew && !CustomGameOptions.SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.SilenceMates);

    public bool Exception14(PlayerControl player) => player == PoisonedPlayer || (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var flag = ConcealedPlayer == null && !HoldsDrive;
        var flag1 = PoisonedPlayer == null && HoldsDrive;
        var flag2 = PoisonedPlayer == null && HoldsDrive;
        var flag3 = WarpPlayer1 == null && !HoldsDrive;
        var flag4 = WarpPlayer2 == null && !HoldsDrive;
        var flag5 = ShapeshiftPlayer1 == null && !HoldsDrive;
        var flag6 = ShapeshiftPlayer2 == null && !HoldsDrive;
        var flag7 = ConfusedPlayer == null && !HoldsDrive;
        ConfuseButton.Update(flag7 ? "SET TARGET" : "CONFUSE", ConfuseTimer, CustomGameOptions.ConfuseCd, OnEffect, TimeRemaining, CustomGameOptions.ConfuseDur, true, IsDrunk);
        StalkButton.Update("STALK", StalkTimer, CustomGameOptions.StalkCd, true, !HoldsDrive && IsStalk);
        SpellButton.Update("SPELL", SpellTimer, CustomGameOptions.SpellCd, SpellCount * CustomGameOptions.SpellCdIncrease, true, IsSpell);
        PositiveButton.Update("SET POSITIVE", PositiveTimer, CustomGameOptions.CollideCd, true, IsCol);
        NegativeButton.Update("SET NEGATIVE", NegativeTimer, CustomGameOptions.CollideCd, true, IsCol);
        ShapeshiftButton.Update(flag5 ? "FIRST TARGET" : (flag6 ? "SECOND TARGET": "SHAPESHIFT"), ShapeshiftTimer, CustomGameOptions.ShapeshiftCd, OnEffect, TimeRemaining,
            CustomGameOptions.ShapeshiftDur, true, IsSS);
        WarpButton.Update(flag3 ? "FIRST TARGET" : (flag4 ? "SECOND TARGET" : "WARP"), WarpTimer, CustomGameOptions.WarpCd, true, IsWarp);
        PoisonButton.Update("POISON", PoisonTimer, CustomGameOptions.PoisonCd, OnEffect, TimeRemaining, CustomGameOptions.PoisonDur, true, !HoldsDrive && IsPois);
        GlobalPoisonButton.Update(flag1 ? "SET TARGET" : "POISON", PoisonTimer, CustomGameOptions.PoisonCd, OnEffect, TimeRemaining, CustomGameOptions.PoisonDur, true, HoldsDrive &&
            IsPois);
        FrameButton.Update("FRAME", FrameTimer, CustomGameOptions.FrameCd, true, !HoldsDrive && IsFram);
        RadialFrameButton.Update("FRAME", FrameTimer, CustomGameOptions.FrameCd, true, HoldsDrive && IsFram);
        ConcealButton.Update(flag ? "SET TARGET" : "CONCEAL", ConcealTimer, CustomGameOptions.ConcealCd, OnEffect, TimeRemaining, CustomGameOptions.ConcealDur, true, IsConc);
        BombButton.Update("PLACE", BombTimer, CustomGameOptions.BombCd, true, IsBomb);
        DetonateButton.Update("DETONATE", DetonateTimer, CustomGameOptions.DetonateCd, true, Bombs?.Count > 0 && IsBomb);
        TimeButton.Update(HoldsDrive ? "REWIND" : "FREEZE", TimeTimer, CustomGameOptions.TimeCd, OnEffect, TimeRemaining, CustomGameOptions.TimeDur, true, IsTK);
        SilenceButton.Update("SILENCE", SilenceTimer, CustomGameOptions.SilenceCd, true, IsSil);
        ChargeButton.Update("CHARGE", ChargeTimer, CustomGameOptions.ChargeCd, OnEffect, TimeRemaining, CustomGameOptions.ChargeDur, true, HoldsDrive && IsCol);
        CrusadeButton.Update("CRUSADE", CrusadeTimer, CustomGameOptions.CrusadeCd, OnEffect, TimeRemaining, CustomGameOptions.CrusadeDur, true, IsCrus);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!OnEffect)
            {
                if (!HoldsDrive)
                {
                    if (ShapeshiftPlayer2 != null)
                        ShapeshiftPlayer2 = null;
                    else if (ShapeshiftPlayer1 != null)
                        ShapeshiftPlayer1 = null;
                    else if (ConcealedPlayer != null)
                        ConcealedPlayer = null;
                    else if (ConfusedPlayer != null)
                        ConfusedPlayer = null;
                    else if (WarpPlayer2 != null)
                        WarpPlayer2 = null;
                    else if (WarpPlayer1 != null)
                        WarpPlayer1 = null;
                }
                else if (PoisonedPlayer != null)
                    PoisonedPlayer = null;
            }

            LogInfo("Removed a target");
        }

        if (IsDead)
            OnLobby();
        else if (IsStalk)
        {
            foreach (var pair in StalkerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (player == null || player.Data.Disconnected || (player.Data.IsDead && !body))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor(!HoldsDrive));
            }

            if (HoldsDrive)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (!StalkerArrows.ContainsKey(player.PlayerId))
                        StalkerArrows.Add(player.PlayerId, new(Player, player.GetPlayerColor(false)));
                }
            }
        }
        else if (IsCol)
        {
            if (GetDistBetweenPlayers(Positive, Negative) <= Range)
            {
                if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() ||
                    Negative.IsRetShielded() || Negative.IsVesting()))
                {
                    RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
                }

                if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() ||
                    Positive.IsRetShielded() || Positive.IsVesting()))
                {
                    RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
                }

                Positive = null;
                Negative = null;

                if (CustomGameOptions.CollideResetsCooldown)
                {
                    LastPositive = DateTime.UtcNow;
                    LastNegative = DateTime.UtcNow;
                }
            }
            else if (GetDistBetweenPlayers(Player, Negative) <= Range && HoldsDrive && OnEffect)
            {
                if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() ||
                    Negative.IsRetShielded() || Negative.IsVesting()))
                {
                    RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
                }

                Negative = null;

                if (CustomGameOptions.CollideResetsCooldown)
                {
                    LastPositive = DateTime.UtcNow;
                    LastNegative = DateTime.UtcNow;
                }
            }
            else if (GetDistBetweenPlayers(Player, Positive) <= Range && HoldsDrive && OnEffect)
            {
                if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() ||
                    Positive.IsRetShielded() || Positive.IsVesting()))
                {
                    RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
                }

                Positive = null;

                if (CustomGameOptions.CollideResetsCooldown)
                {
                    LastPositive = DateTime.UtcNow;
                    LastNegative = DateTime.UtcNow;
                }
            }
        }
    }

    public override void OnLobby()
    {
        base.OnLobby();

        Bomb.Clear(Bombs);
        Bombs.Clear();

        StalkerArrows.Values.ToList().DestroyAll();
        StalkerArrows.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        Positive = null;
        Negative = null;

        if (CustomGameOptions.BombsDetonateOnMeetingStart)
            Bomb.DetonateBombs(Bombs);
    }

    //Anarchist Stuff
    public bool IsAnarch => FormerRole?.Type == LayerEnum.Anarchist;

    //Concealer Stuff
    public CustomButton ConcealButton { get; set; }
    public DateTime LastConcealed { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomMenu ConcealMenu { get; set; }
    public bool IsConc => FormerRole?.Type == LayerEnum.Concealer;
    public float ConcealTimer => ButtonUtils.Timer(Player, LastConcealed, CustomGameOptions.ConcealCd);

    public void Conceal()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (HoldsDrive)
            Conceal();
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));

        if (Meeting || (ConcealedPlayer == null && !HoldsDrive))
            TimeRemaining = 0f;
    }

    public void UnConceal()
    {
        Enabled = false;
        LastConcealed = DateTime.UtcNow;

        if (SyndicateHasChaosDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);
    }

    public void ConcealClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            ConcealedPlayer = player;
        else if (interact[0])
            LastConcealed = DateTime.UtcNow;
        else if (interact[1])
            LastConcealed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void HitConceal()
    {
        if (ConcealTimer != 0f || OnEffect)
            return;

        if (HoldsDrive)
        {
            TimeRemaining = CustomGameOptions.ConcealDur;
            Conceal();
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Conceal, this);
        }
        else if (ConcealedPlayer == null)
            ConcealMenu.Open();
        else
        {
            TimeRemaining = CustomGameOptions.ConcealDur;
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Conceal, this, ConcealedPlayer);
            Conceal();
        }
    }

    //Framer Stuff
    public CustomButton FrameButton { get; set; }
    public List<byte> Framed { get; set; }
    public DateTime LastFramed { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public bool IsFram => FormerRole?.Type == LayerEnum.Framer;
    public float FrameTimer => ButtonUtils.Timer(Player, LastFramed, CustomGameOptions.FrameCd);

    public void RpcFrame(PlayerControl player)
    {
        if ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || Framed.Contains(player.PlayerId))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Frame, this, player);
    }

    public void Frame()
    {
        if (FrameTimer != 0f || IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
            return;

        var interact = Interact(Player, FrameButton.TargetPlayer);

        if (interact[3])
            RpcFrame(FrameButton.TargetPlayer);

        if (interact[0])
            LastFramed = DateTime.UtcNow;
        else if (interact[1])
            LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void RadialFrame()
    {
        if (FrameTimer != 0f || !HoldsDrive)
            return;

        GetClosestPlayers(CustomPlayer.Local.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius).ForEach(RpcFrame);
        LastFramed = DateTime.UtcNow;
    }

    //Poisoner Stuff
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public DateTime LastPoisoned { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public CustomMenu PoisonMenu { get; set; }
    public bool IsPois => FormerRole?.Type == LayerEnum.Poisoner;
    public float PoisonTimer => ButtonUtils.Timer(Player, LastPoisoned, CustomGameOptions.PoisonCd);

    public void Poison()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || IsDead || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected)
            TimeRemaining = 0f;
    }

    public void PoisonKill()
    {
        if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(LayerEnum.Pestilence)))
            RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
        Enabled = false;
        LastPoisoned = DateTime.UtcNow;
    }

    public void PoisonClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3] && !player.IsProtected() && !player.IsVesting())
            PoisonedPlayer = player;
        else if (interact[0])
            LastPoisoned = DateTime.UtcNow;
        else if (interact[1] || player.IsProtected())
            LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (player.IsVesting())
            LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void HitPoison()
    {
        if (PoisonTimer != 0f || OnEffect || HoldsDrive || IsTooFar(Player, PoisonButton.TargetPlayer))
            return;

        var interact = Interact(Player, PoisonButton.TargetPlayer);

        if (interact[3] && !PoisonButton.TargetPlayer.IsProtected() && !PoisonButton.TargetPlayer.IsVesting() && !PoisonButton.TargetPlayer.IsProtectedMonarch())
        {
            PoisonedPlayer = PoisonButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Poison, this, PoisonedPlayer);
            TimeRemaining = CustomGameOptions.PoisonDur;
            Poison();
        }
        else if (interact[1] || PoisonButton.TargetPlayer.IsProtected())
            LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[0])
            LastPoisoned = DateTime.UtcNow;
        else if (interact[2])
            LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void HitGlobalPoison()
    {
        if (PoisonTimer != 0f || OnEffect || !HoldsDrive)
            return;

        if (PoisonedPlayer == null)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Poison, this, PoisonedPlayer);
            TimeRemaining = CustomGameOptions.PoisonDur;
            Poison();
        }
    }

    //Shapeshifter Stuff
    public CustomButton ShapeshiftButton { get; set; }
    public DateTime LastShapeshifted { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; set; }
    public PlayerControl ShapeshiftPlayer2 { get; set; }
    public CustomMenu ShapeshiftMenu1 { get; set; }
    public CustomMenu ShapeshiftMenu2 { get; set; }
    public bool IsSS => FormerRole?.Type == LayerEnum.Shapeshifter;
    public float ShapeshiftTimer => ButtonUtils.Timer(Player, LastShapeshifted, CustomGameOptions.ShapeshiftCd);

    public void Shapeshift()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (!SyndicateHasChaosDrive)
        {
            Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
            Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
        }
        else
            Utils.Shapeshift();

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void UnShapeshift()
    {
        Enabled = false;
        LastShapeshifted = DateTime.UtcNow;

        if (SyndicateHasChaosDrive)
            DefaultOutfitAll();
        else
        {
            DefaultOutfit(ShapeshiftPlayer1);
            DefaultOutfit(ShapeshiftPlayer2);
        }

        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
    }

    public void ShapeshiftClick1(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            ShapeshiftPlayer1 = player;
        else if (interact[0])
            LastShapeshifted = DateTime.UtcNow;
        else if (interact[1])
            LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void ShapeshiftClick2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            ShapeshiftPlayer2 = player;
        else if (interact[0])
            LastShapeshifted = DateTime.UtcNow;
        else if (interact[1])
            LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void HitShapeshift()
    {
        if (ShapeshiftTimer != 0f || OnEffect)
            return;

        if (HoldsDrive)
        {
            TimeRemaining = CustomGameOptions.ShapeshiftDur;
            Shapeshift();
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Shapeshift, this);
        }
        else if (ShapeshiftPlayer1 == null)
            ShapeshiftMenu1.Open();
        else if (ShapeshiftPlayer2 == null)
            ShapeshiftMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Shapeshift, this, ShapeshiftPlayer1, ShapeshiftPlayer2);
            TimeRemaining = CustomGameOptions.ShapeshiftDur;
            Shapeshift();
        }
    }

    //Bomber Stuff
    public DateTime LastPlaced { get; set; }
    public DateTime LastDetonated { get; set; }
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }
    public bool IsBomb => FormerRole?.Type == LayerEnum.Bomber;
    public float BombTimer => ButtonUtils.Timer(Player, LastPlaced, CustomGameOptions.BombCd);
    public float DetonateTimer => ButtonUtils.Timer(Player, LastDetonated, CustomGameOptions.DetonateCd);

    public void Place()
    {
        if (BombTimer != 0f)
            return;

        Bombs.Add(new Bomb(Player, HoldsDrive));
        LastPlaced = DateTime.UtcNow;

        if (CustomGameOptions.BombCooldownsLinked)
            LastDetonated = DateTime.UtcNow;
    }

    public void Detonate()
    {
        if (DetonateTimer != 0f || Bombs.Count == 0)
            return;

        Bomb.DetonateBombs(Bombs);
        LastDetonated = DateTime.UtcNow;

        if (CustomGameOptions.BombCooldownsLinked)
            LastPlaced = DateTime.UtcNow;
    }

    //Warper Stuff
    public CustomButton WarpButton { get; set; }
    public DateTime LastWarped { get; set; }
    public PlayerControl WarpPlayer1 { get; set; }
    public PlayerControl WarpPlayer2 { get; set; }
    public CustomMenu WarpMenu1 { get; set; }
    public CustomMenu WarpMenu2 { get; set; }
    public Dictionary<byte, DateTime> UnwarpablePlayers { get; set; }
    public SpriteRenderer AnimationPlaying { get; set; }
    public GameObject WarpObj { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent { get; set; }
    public Vent Vent { get; set; }
    public bool IsWarp => FormerRole?.Type == LayerEnum.Warper;
    public float WarpTimer => ButtonUtils.Timer(Player, LastWarped, CustomGameOptions.WarpCd);

    public IEnumerator WarpPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;
        TimeRemaining = CustomGameOptions.WarpDur;

        if (WarpPlayer1.Data.IsDead)
        {
            Player1Body = BodyById(WarpPlayer1.PlayerId);

            if (Player1Body == null)
                yield break;
        }

        if (WarpPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(WarpPlayer2.PlayerId);

            if (Player2Body == null)
                yield break;
        }

        if (WarpPlayer1.inVent)
        {
            while (GetInTransition())
                yield return null;

            WarpPlayer1.MyPhysics.ExitAllVents();
        }

        if (WarpPlayer2.inVent)
        {
            while (GetInTransition())
                yield return null;

            Vent = WarpPlayer2.GetClosestVent();
            WasInVent = true;
        }

        WarpPlayer1.moveable = false;
        WarpPlayer1.NetTransform.Halt();

        if (CustomPlayer.Local == WarpPlayer1)
            Flash(Color, CustomGameOptions.WarpDur);

        if (Player1Body == null && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var now = DateTime.UtcNow;
            var seconds = (now - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.WarpDur)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }
            else
                break;

            if (Meeting)
            {
                TimeRemaining = 0;
                yield break;
            }
        }

        if (Player1Body == null && Player2Body == null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (WarpPlayer1.CanVent() && Vent != null && WasInVent)
                WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
        }
        else if (Player1Body != null && Player2Body == null)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = WarpPlayer2.GetTruePosition();

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer2)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body == null && Player2Body != null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body != null && Player2Body != null)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = Player2Body.TruePosition;
        }

        if (CustomPlayer.Local == WarpPlayer1)
        {
            if (Minigame.Instance)
                Minigame.Instance.Close();

            if (Map)
                Map.Close();
        }

        WarpPlayer1.moveable = true;
        WarpPlayer1.Collider.enabled = true;
        WarpPlayer1.NetTransform.enabled = true;
        WarpPlayer2.MyPhysics.ResetMoveState();
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        TimeRemaining = 0; //Insurance
        LastWarped = DateTime.UtcNow;
    }

    public void AnimateWarp()
    {
        WarpObj.transform.position = new(WarpPlayer1.GetTruePosition().x, WarpPlayer1.GetTruePosition().y + 0.35f, (WarpPlayer1.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying.flipX = WarpPlayer1.MyRend().flipX;
        AnimationPlaying.transform.localScale *= 0.9f * WarpPlayer1.GetModifiedSize();

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.WarpDur, new Action<float>(p =>
        {
            var index = (int)(p * PortalAnimation.Length);
            index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
            AnimationPlaying.sprite = PortalAnimation[index];
            WarpPlayer1.SetPlayerMaterialColors(AnimationPlaying);

            if (p == 1)
                AnimationPlaying.sprite = null;
        })));
    }

    public void WarpClick1(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            WarpPlayer1 = player;
        else if (interact[0])
            LastWarped = DateTime.UtcNow;
        else if (interact[1])
            LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void WarpClick2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            WarpPlayer2 = player;
        else if (interact[0])
            LastWarped = DateTime.UtcNow;
        else if (interact[1])
            LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void Warp()
    {
        if (WarpTimer != 0f)
            return;

        if (HoldsDrive)
        {
            Warp();
            LastWarped = DateTime.UtcNow;
        }
        else if (WarpPlayer1 == null)
            WarpMenu1.Open();
        else if (WarpPlayer2 == null)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Warp, this, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    //Crusader Stuff
    public DateTime LastCrusaded { get; set; }
    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }
    public bool IsCrus => FormerRole?.Type == LayerEnum.Crusader;
    public float CrusadeTimer => ButtonUtils.Timer(Player, LastCrusaded, CustomGameOptions.CrusadeCd);

    public void Crusade()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || Meeting)
            TimeRemaining = 0f;
    }

    public void UnCrusade()
    {
        Enabled = false;
        LastCrusaded = DateTime.UtcNow;
        CrusadedPlayer = null;
    }

    public void HitCrusade()
    {
        if (CrusadeTimer != 0f || IsTooFar(Player, CrusadeButton.TargetPlayer))
            return;

        var interact = Interact(Player, CrusadeButton.TargetPlayer);

        if (interact[3])
        {
            CrusadedPlayer = CrusadeButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Crusade, this, CrusadedPlayer);
            TimeRemaining = CustomGameOptions.CrusadeDur;
            Crusade();
        }
        else if (interact[0])
            LastCrusaded = DateTime.UtcNow;
        else if (interact[1])
            LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Collider Stuff
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public DateTime LastPositive { get; set; }
    public DateTime LastNegative { get; set; }
    public DateTime LastCharged { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);
    public bool IsCol => FormerRole?.Type == LayerEnum.Collider;
    public float PositiveTimer => ButtonUtils.Timer(Player, LastPositive, CustomGameOptions.CollideCd);
    public float NegativeTimer => ButtonUtils.Timer(Player, LastNegative, CustomGameOptions.CollideCd);
    public float ChargeTimer => ButtonUtils.Timer(Player, LastCharged, CustomGameOptions.ChargeCd);

    public void SetPositive()
    {
        if (HoldsDrive || IsTooFar(Player, PositiveButton.TargetPlayer) || PositiveTimer != 0f)
            return;

        var interact = Interact(Player, PositiveButton.TargetPlayer);

        if (interact[3])
            Positive = PositiveButton.TargetPlayer;

        if (interact[0])
            LastPositive = DateTime.UtcNow;
        else if (interact[1])
            LastPositive.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void SetNegative()
    {
        if (HoldsDrive || IsTooFar(Player, NegativeButton.TargetPlayer) || NegativeTimer != 0f)
            return;

        var interact = Interact(Player, NegativeButton.TargetPlayer);

        if (interact[3])
            Negative = NegativeButton.TargetPlayer;

        if (interact[0])
            LastNegative = DateTime.UtcNow;
        else if (interact[1])
            LastNegative.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void Charge()
    {
        if (!HoldsDrive || OnEffect || ChargeTimer != 0f)
            return;

        TimeRemaining = CustomGameOptions.ChargeDur;
        ChargeSelf();
    }

    public void ChargeSelf()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void DischargeSelf()
    {
        Enabled = false;
        LastCharged = DateTime.UtcNow;
    }

    //Spellslinger Stuff
    public CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; set; }
    public DateTime LastSpelled { get; set; }
    public int SpellCount { get; set; }
    public bool IsSpell => FormerRole?.Type == LayerEnum.Spellslinger;
    public float SpellTimer => ButtonUtils.Timer(Player, LastSpelled, CustomGameOptions.SpellCd, SpellCount * CustomGameOptions.SpellCdIncrease);

    public void Spell(PlayerControl player)
    {
        if (player.Is(Faction.Syndicate) || Spelled.Contains(player.PlayerId))
            return;

        Spelled.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Spell, this, player);

        if (!HoldsDrive)
            SpellCount++;
        else
            SpellCount = 0;
    }

    public void HitSpell()
    {
        if (SpellTimer != 0f || IsTooFar(Player, SpellButton.TargetPlayer))
            return;

        if (HoldsDrive)
        {
            Spell(SpellButton.TargetPlayer);
            LastSpelled = DateTime.UtcNow;
        }
        else
        {
            var interact = Interact(Player, SpellButton.TargetPlayer);

            if (interact[3])
                Spell(SpellButton.TargetPlayer);

            if (interact[0])
                LastSpelled = DateTime.UtcNow;
            else if (interact[1])
                LastSpelled.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }

    //Stalker Stuff
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public DateTime LastStalked { get; set; }
    public CustomButton StalkButton { get; set; }
    public bool IsStalk => FormerRole?.Type == LayerEnum.Stalker;
    public float StalkTimer => ButtonUtils.Timer(Player, LastStalked, CustomGameOptions.StalkCd);

    public void DestroyArrow(byte targetPlayerId)
    {
        StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        StalkerArrows.Remove(targetPlayerId);
    }

    public void Stalk()
    {
        if (IsTooFar(Player, StalkButton.TargetPlayer) || StalkTimer != 0f)
            return;

        var interact = Interact(Player, StalkButton.TargetPlayer);

        if (interact[3])
            StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

        if (interact[0])
            LastStalked = DateTime.UtcNow;
        else if (interact[1])
            LastStalked.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    //Drunkard Stuff
    public CustomButton ConfuseButton { get; set; }
    public DateTime LastConfused { get; set; }
    public float Modifier => OnEffect ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomMenu ConfuseMenu { get; set; }
    public bool IsDrunk => FormerRole?.Type == LayerEnum.Drunkard;
    public float ConfuseTimer => ButtonUtils.Timer(Player, LastConfused, CustomGameOptions.ConfuseCd);

    public void Confuse()
    {
        if (!Enabled && (CustomPlayer.Local == ConfusedPlayer || HoldsDrive))
            Flash(Color);

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || (ConfusedPlayer == null && !HoldsDrive))
            TimeRemaining = 0f;
    }

    public void UnConfuse()
    {
        Enabled = false;
        LastConfused = DateTime.UtcNow;
        ConfusedPlayer = null;
    }

    public void ConfuseClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            ConfusedPlayer = player;
        else if (interact[0])
            LastConfused = DateTime.UtcNow;
        else if (interact[1])
            LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void HitConfuse()
    {
        if (ConfuseTimer != 0f || OnEffect)
            return;

        if (HoldsDrive)
        {
            TimeRemaining = CustomGameOptions.ConfuseDur;
            Confuse();
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Confuse, this);
        }
        else if (ConfusedPlayer == null)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Confuse, this, ConfusedPlayer);
            TimeRemaining = CustomGameOptions.ConfuseDur;
            Confuse();
        }
    }

    //Time Keeper Stuff
    public DateTime LastTimed { get; set; }
    public CustomButton TimeButton { get; set; }
    public bool IsTK => FormerRole?.Type == LayerEnum.TimeKeeper;
    public float TimeTimer => ButtonUtils.Timer(Player, LastTimed, CustomGameOptions.TimeCd);

    public void Control()
    {
        if (!Enabled)
            Flash(Color, CustomGameOptions.TimeDur);

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = true);

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void UnControl()
    {
        Enabled = false;
        LastTimed = DateTime.UtcNow;
        CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = false);
    }

    public void TimeControl()
    {
        if (TimeTimer != 0f || OnEffect)
            return;

        TimeRemaining = CustomGameOptions.TimeDur;
        Control();
        CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.TimeControl, this);
    }

    //Silencer Stuff
    public CustomButton SilenceButton { get; set; }
    public PlayerControl SilencedPlayer { get; set; }
    public DateTime LastSilenced { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }
    public bool IsSil => FormerRole?.Type == LayerEnum.Silencer;
    public float SilenceTimer => ButtonUtils.Timer(Player, LastSilenced, CustomGameOptions.SilenceCd);

    public void Silence()
    {
        if (SilenceTimer != 0f || IsTooFar(Player, SilenceButton.TargetPlayer) || SilenceButton.TargetPlayer == SilencedPlayer)
            return;

        var interact = Interact(Player, SilenceButton.TargetPlayer);

        if (interact[3])
        {
            SilencedPlayer = SilenceButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Silence, this, SilencedPlayer);
        }

        if (interact[0])
            LastSilenced = DateTime.UtcNow;
        else if (interact[1])
            LastSilenced.AddSeconds(CustomGameOptions.ProtectKCReset);
    }
}