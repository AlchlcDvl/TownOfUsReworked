namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedRebel : Syndicate
{
    public PromotedRebel(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicatePower;
        SpellCount = 0;
        Framed = new();
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
        ConfuseMenu = new(Player, ConfuseClick, DrunkException);
        WarpMenu1 = new(Player, WarpClick1, WarpException1);
        WarpMenu2 = new(Player, WarpClick2, WarpException2);
        ConcealMenu = new(Player, ConcealClick, ConcealException);
        PoisonMenu = new(Player, PoisonClick, PoisonException);
        ShapeshiftMenu1 = new(Player, ShapeshiftClick1, SSException1);
        ShapeshiftMenu2 = new(Player, ShapeshiftClick2, SSException2);
        SpellButton = new(this, "Spell", AbilityTypes.Target, "Secondary", HitSpell, CustomGameOptions.SpellCd, SpellException);
        StalkButton = new(this, "Stalk", AbilityTypes.Target, "ActionSecondary", Stalk, CustomGameOptions.StalkCd, StalkException);
        PositiveButton = new(this, "Positive", AbilityTypes.Target, "ActionSecondary", SetPositive, CustomGameOptions.CollideCd, PlusException);
        NegativeButton = new(this, "Negative", AbilityTypes.Target, "Secondary", SetNegative, CustomGameOptions.CollideCd, MinusException);
        ChargeButton = new(this, "Charge", AbilityTypes.Targetless, "Tertiary", CustomGameOptions.ChargeCd, CustomGameOptions.ChargeDur);
        ConcealButton = new(this, "Conceal", AbilityTypes.Targetless, "Secondary", HitConceal, CustomGameOptions.ConcealCd, CustomGameOptions.ConcealDur, (CustomButton.EffectVoid)Conceal,
            UnConceal);
        FrameButton = new(this, "Frame", AbilityTypes.Target, "Secondary", Frame, CustomGameOptions.FrameCd, FrameException);
        RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Targetless, "Secondary", RadialFrame, CustomGameOptions.FrameCd);
        ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Targetless, "Secondary", HitShapeshift, CustomGameOptions.ShapeshiftCd, CustomGameOptions.ShapeshiftDur,
            (CustomButton.EffectVoid)Shift, UnShapeshift);
        BombButton = new(this, "Plant", AbilityTypes.Targetless, "ActionSecondary", Place, CustomGameOptions.BombCd);
        DetonateButton = new(this, "Detonate", AbilityTypes.Targetless, "Secondary", Detonate, CustomGameOptions.DetonateCd);
        CrusadeButton = new(this, "Crusade", AbilityTypes.Target, "Secondary", Crusade, CustomGameOptions.CrusadeCd, CustomGameOptions.CrusadeDur, UnCrusade, CrusadeException);
        PoisonButton = new(this, "Poison", AbilityTypes.Target, "ActionSecondary", HitPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison, PoisonException);
        GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Targetless, "ActionSecondary", HitGlobalPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison);
        WarpButton = new(this, "Warp", AbilityTypes.Targetless, "ActionSecondary", Warp, CustomGameOptions.WarpCd);
        ConfuseButton = new(this, "Confuse", AbilityTypes.Targetless, "Secondary", HitConfuse, CustomGameOptions.ConfuseCd, CustomGameOptions.ConfuseDur,
            (CustomButton.EffectStartVoid)StartConfusion, UnConfuse);
        TimeButton = new(this, "Time", AbilityTypes.Targetless, "Secondary", TimeControl, CustomGameOptions.TimeCd, CustomGameOptions.TimeDur, Control, ControlStart, UnControl);
        SilenceButton = new(this, "Silence", AbilityTypes.Target, "Secondary", Silence, CustomGameOptions.SilenceCd, SilenceException);
    }

    //Rebel Stuff
    public Syndicate FormerRole { get; set; }

    public override Color Color
    {
        get
        {
            if (!ClientGameOptions.CustomSynColors)
                return Colors.Syndicate;
            else if (FormerRole != null)
                return FormerRole.Color;
            else
                return Colors.Rebel;
        }
    }
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.PromotedRebel;
    public override Func<string> StartText => () => "Lead The <color=#008000FF>Syndicate</color>";
    public override Func<string> Description => () => "- You have succeeded the former <color=#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities" +
        (FormerRole == null ? CommonAbilities : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");

    public override void TryEndEffect()
    {
        ConcealButton.Update3(!HoldsDrive && ConcealedPlayer != null && ConcealedPlayer.HasDied());
        ConfuseButton.Update3(!HoldsDrive && ConfusedPlayer != null && ConfusedPlayer.HasDied());
        ChargeButton.Update3(IsDead);
        CrusadeButton.Update3((CrusadedPlayer != null && CrusadedPlayer.HasDied()) || IsDead);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ConfuseButton.Update2(ConfusedPlayer == null && !HoldsDrive ? "SET TARGET" : "CONFUSE", IsDrunk);
        StalkButton.Update2("STALK", !HoldsDrive && IsStalk);
        SpellButton.Update2("SPELL", IsSpell, difference: SpellCount * CustomGameOptions.SpellCdIncrease * CustomGameOptions.RebPromotionCdDecrease);
        PositiveButton.Update2("SET POSITIVE", IsCol);
        NegativeButton.Update2("SET NEGATIVE", IsCol);
        ChargeButton.Update2("CHARGE", HoldsDrive && IsCol);
        ShapeshiftButton.Update2(SSLabel(), IsSS);
        WarpButton.Update2(WarpPlayer1 == null && !HoldsDrive ? "FIRST TARGET" : (WarpPlayer2 == null && !HoldsDrive ? "SECOND TARGET" : "WARP"), IsWarp);
        GlobalPoisonButton.Update2(PoisonedPlayer == null && HoldsDrive ? "SET TARGET" : "POISON", HoldsDrive && IsPois);
        PoisonButton.Update2("POISON", !HoldsDrive && IsPois);
        FrameButton.Update2("FRAME", !HoldsDrive && IsFram);
        RadialFrameButton.Update2("FRAME", HoldsDrive && IsFram);
        ConcealButton.Update2(ConcealedPlayer == null && !HoldsDrive ? "SET TARGET" : "CONCEAL", IsConc);
        BombButton.Update2("PLACE BOMB", IsBomb, !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2));
        DetonateButton.Update2("DETONATE", IsBomb && Bombs.Count > 0);
        TimeButton.Update2(HoldsDrive ? "REWIND" : "FREEZE", IsTK);
        SilenceButton.Update2("SILENCE", IsSil);
        CrusadeButton.Update2("CRUSADE", IsCrus);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive)
            {
                if (ShapeshiftPlayer2 != null && !ShapeshiftButton.EffectActive)
                    ShapeshiftPlayer2 = null;
                else if (ShapeshiftPlayer1 != null && !ShapeshiftButton.EffectActive)
                    ShapeshiftPlayer1 = null;
                else if (ConcealedPlayer != null && !ConcealButton.EffectActive)
                    ConcealedPlayer = null;
                else if (ConfusedPlayer != null && !ConfuseButton.EffectActive)
                    ConfusedPlayer = null;
                else if (WarpPlayer2 != null && !Warping)
                    WarpPlayer2 = null;
                else if (WarpPlayer1 != null && !Warping)
                    WarpPlayer1 = null;
            }
            else if (PoisonedPlayer != null && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
                PoisonedPlayer = null;

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
                    PositiveButton.StartCooldown();
                    NegativeButton.StartCooldown();
                }
            }
            else if (GetDistBetweenPlayers(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(LayerEnum.Pestilence) || Negative.IsOnAlert() ||
                    Negative.IsRetShielded() || Negative.IsVesting()))
                {
                    RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
                }

                Negative = null;

                if (CustomGameOptions.CollideResetsCooldown)
                {
                    PositiveButton.StartCooldown();
                    NegativeButton.StartCooldown();
                }
            }
            else if (GetDistBetweenPlayers(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(LayerEnum.Pestilence) || Positive.IsOnAlert() ||
                    Positive.IsRetShielded() || Positive.IsVesting()))
                {
                    RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
                }

                Positive = null;

                if (CustomGameOptions.CollideResetsCooldown)
                {
                    PositiveButton.StartCooldown();
                    NegativeButton.StartCooldown();
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

    public override void ReadRPC(MessageReader reader)
    {
        var rebAction = (RebActionsRPC)reader.ReadByte();

        switch (rebAction)
        {
            case RebActionsRPC.Poison:
                PoisonedPlayer = reader.ReadPlayer();
                break;

            case RebActionsRPC.Conceal:
                if (!HoldsDrive)
                    ConcealedPlayer = reader.ReadPlayer();

                break;

            case RebActionsRPC.Shapeshift:
                if (!HoldsDrive)
                {
                    ShapeshiftPlayer1 = reader.ReadPlayer();
                    ShapeshiftPlayer2 = reader.ReadPlayer();
                }

                break;

            case RebActionsRPC.Warp:
                WarpPlayer1 = reader.ReadPlayer();
                WarpPlayer2 = reader.ReadPlayer();
                Coroutines.Start(WarpPlayers());
                break;

            case RebActionsRPC.Crusade:
                CrusadedPlayer = reader.ReadPlayer();
                break;

            case RebActionsRPC.Spell:
                Spelled.Add(reader.ReadByte());
                break;

            case RebActionsRPC.Frame:
                Framed.Add(reader.ReadByte());
                break;

            case RebActionsRPC.Confuse:
                if (!HoldsDrive)
                    ConfusedPlayer = reader.ReadPlayer();

                break;

            case RebActionsRPC.Silence:
                SilencedPlayer = reader.ReadPlayer();
                break;

            default:
                LogError($"Received unknown RPC - {rebAction}");
                break;
        }
    }

    //Anarchist Stuff
    public bool IsAnarch => FormerRole?.Type == LayerEnum.Anarchist;

    //Concealer Stuff
    public CustomButton ConcealButton { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomMenu ConcealMenu { get; set; }
    public bool IsConc => FormerRole?.Type == LayerEnum.Concealer;

    public bool ConcealException(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates && Faction is
        Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConcealMates);

    public void Conceal()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));
    }

    public void UnConceal()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);

        ConcealedPlayer = null;
    }

    public void ConcealClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            ConcealedPlayer = player;
        else if (interact.Reset)
            ConcealButton.StartCooldown();
        else if (interact.Protected)
            ConcealButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void HitConceal()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton, RebActionsRPC.Conceal);
            ConcealButton.Begin();
        }
        else if (ConcealedPlayer == null)
            ConcealMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton, RebActionsRPC.Conceal, ConcealedPlayer);
            ConcealButton.Begin();
        }
    }

    //Framer Stuff
    public CustomButton FrameButton { get; set; }
    public List<byte> Framed { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public bool IsFram => FormerRole?.Type == LayerEnum.Framer;

    public bool FrameException(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public void RpcFrame(PlayerControl player)
    {
        if (FrameException(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RebActionsRPC.Frame, player.PlayerId);
    }

    public void Frame()
    {
        var interact = Interact(Player, FrameButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            RpcFrame(FrameButton.TargetPlayer);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        FrameButton.StartCooldown(cooldown);
    }

    public void RadialFrame()
    {
        GetClosestPlayers(Player.transform.position, CustomGameOptions.ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    //Poisoner Stuff
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public CustomMenu PoisonMenu { get; set; }
    public bool IsPois => FormerRole?.Type == LayerEnum.Poisoner;

    public bool PoisonException(PlayerControl player) => player == PoisonedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public void UnPoison()
    {
        if (!(PoisonedPlayer.HasDied() || PoisonedPlayer.Is(LayerEnum.Pestilence)))
            RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
    }

    public void PoisonClick(PlayerControl player)
    {
        var interact = Interact(Player, player, poisoning: true);

        if (interact.AbilityUsed)
            PoisonedPlayer = player;
        else if (interact.Reset)
            GlobalPoisonButton.StartCooldown();
        else if (interact.Protected)
            GlobalPoisonButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            GlobalPoisonButton.StartCooldown(CooldownType.Survivor);
    }

    public void HitPoison()
    {
        var interact = Interact(Player, PoisonButton.TargetPlayer, poisoning: true);

        if (interact.AbilityUsed)
        {
            PoisonedPlayer = PoisonButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, PoisonButton, RebActionsRPC.Poison, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else if (interact.Reset)
            PoisonButton.StartCooldown();
        else if (interact.Protected)
            PoisonButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            PoisonButton.StartCooldown(CooldownType.Survivor);
    }

    public void HitGlobalPoison()
    {
        if (PoisonedPlayer == null)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, PoisonButton, RebActionsRPC.Poison, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
    }

    //Shapeshifter Stuff
    public CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; set; }
    public PlayerControl ShapeshiftPlayer2 { get; set; }
    public CustomMenu ShapeshiftMenu1 { get; set; }
    public CustomMenu ShapeshiftMenu2 { get; set; }
    public bool IsSS => FormerRole?.Type == LayerEnum.Shapeshifter;

    public bool SSException1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction) &&
        !CustomGameOptions.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&
        !CustomGameOptions.ShapeshiftMates);

    public bool SSException2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction) &&
        !CustomGameOptions.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&
        !CustomGameOptions.ShapeshiftMates);

    public void Shift() => Shapeshifter.Shapeshift(ShapeshiftPlayer1, ShapeshiftPlayer2, HoldsDrive);

    public void UnShapeshift()
    {
        if (HoldsDrive)
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

        if (interact.AbilityUsed)
            ShapeshiftPlayer1 = player;
        else if (interact.Reset)
            ShapeshiftButton.StartCooldown();
        else if (interact.Protected)
            ShapeshiftButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void ShapeshiftClick2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            ShapeshiftPlayer2 = player;
        else if (interact.Reset)
            ShapeshiftButton.StartCooldown();
        else if (interact.Protected)
            ShapeshiftButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void HitShapeshift()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ShapeshiftButton, RebActionsRPC.Shapeshift);
            ShapeshiftButton.Begin();
        }
        else if (ShapeshiftPlayer1 == null)
            ShapeshiftMenu1.Open();
        else if (ShapeshiftPlayer2 == null)
            ShapeshiftMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ShapeshiftButton, RebActionsRPC.Shapeshift, ShapeshiftPlayer1, ShapeshiftPlayer2);
            ShapeshiftButton.Begin();
        }
    }

    public string SSLabel()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";
        else if (ShapeshiftPlayer1 == null)
            return "FIRST TARGET";
        else if (ShapeshiftPlayer2 == null)
            return "SECOND TARGET";
        else
            return "SHAPESHIFT";
    }

    //Bomber Stuff
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }
    public bool IsBomb => FormerRole?.Type == LayerEnum.Bomber;

    public void Place()
    {
        Bombs.Add(new(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (CustomGameOptions.BombCooldownsLinked)
            DetonateButton.StartCooldown();
    }

    public void Detonate()
    {
        Bomb.DetonateBombs(Bombs);
        DetonateButton.StartCooldown();

        if (CustomGameOptions.BombCooldownsLinked)
            BombButton.StartCooldown();
    }

    //Warper Stuff
    public CustomButton WarpButton { get; set; }
    public PlayerControl WarpPlayer1 { get; set; }
    public PlayerControl WarpPlayer2 { get; set; }
    public CustomMenu WarpMenu1 { get; set; }
    public CustomMenu WarpMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying { get; set; }
    public GameObject WarpObj { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent { get; set; }
    public bool Warping { get; set; }
    public Vent Vent { get; set; }
    public bool IsWarp => FormerRole?.Type == LayerEnum.Warper;

    public bool WarpException1(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public bool WarpException2(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public IEnumerator WarpPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;

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

        Warping = true;
        WarpPlayer1.moveable = false;
        WarpPlayer1.NetTransform.Halt();

        if (CustomPlayer.Local == WarpPlayer1)
            Flash(Color, CustomGameOptions.WarpDur);

        if (Player1Body == null && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.WarpDur)
                yield return null;
            else
                break;

            if (Meeting)
            {
                AnimationPlaying.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (Player1Body == null && Player2Body == null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
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

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer2)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body == null && Player2Body != null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
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
            if (ActiveTask)
                ActiveTask.Close();

            if (Map)
                Map.Close();
        }

        WarpPlayer1.moveable = true;
        WarpPlayer1.Collider.enabled = true;
        WarpPlayer1.NetTransform.enabled = true;
        WarpPlayer2.MyPhysics.ResetMoveState();
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        Warping = false;
        yield break;
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

        if (interact.AbilityUsed)
            WarpPlayer1 = player;
        else if (interact.Reset)
            WarpButton.StartCooldown();
        else if (interact.Protected)
            WarpButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void WarpClick2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            WarpPlayer2 = player;
        else if (interact.Reset)
            WarpButton.StartCooldown();
        else if (interact.Protected)
            WarpButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Warp()
    {
        if (HoldsDrive)
        {
            Utils.Warp();
            WarpButton.StartCooldown();
        }
        else if (WarpPlayer1 == null)
            WarpMenu1.Open();
        else if (WarpPlayer2 == null)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RebActionsRPC.Warp, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    //Crusader Stuff
    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }
    public bool IsCrus => FormerRole?.Type == LayerEnum.Crusader;

    public bool CrusadeException(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !CustomGameOptions.CrusadeMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.CrusadeMates);

    public void UnCrusade() => CrusadedPlayer = null;

    public void Crusade()
    {
        var interact = Interact(Player, CrusadeButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CrusadedPlayer = CrusadeButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, CrusadeButton, RebActionsRPC.Crusade, CrusadedPlayer);
            CrusadeButton.Begin();
        }
        else if (interact.Reset)
            CrusadeButton.StartCooldown();
        else if (interact.Protected)
            CrusadeButton.StartCooldown(CooldownType.GuardianAngel);
    }

    //Collider Stuff
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);
    public bool IsCol => FormerRole?.Type == LayerEnum.Collider;

    public bool PlusException(PlayerControl player) => player == Negative || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction
        != SubFaction.None) || Player.IsLinkedTo(player);

    public bool MinusException(PlayerControl player) => player == Positive || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void SetPositive()
    {
        var interact = Interact(Player, PositiveButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Positive = PositiveButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        PositiveButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    public void SetNegative()
    {
        var interact = Interact(Player, NegativeButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Negative = NegativeButton.TargetPlayer;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        NegativeButton.StartCooldown(cooldown);

        if (CustomGameOptions.ChargeCooldownsLinked)
            PositiveButton.StartCooldown(cooldown);
    }

    //Spellslinger Stuff
    public CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; set; }
    public int SpellCount { get; set; }
    public bool IsSpell => FormerRole?.Type == LayerEnum.Spellslinger;

    public bool SpellException(PlayerControl player) => Spelled.Contains(player.PlayerId);

    public void Spell(PlayerControl player)
    {
        if (player.Is(Faction) || Spelled.Contains(player.PlayerId))
            return;

        Spelled.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RebActionsRPC.Spell, player.PlayerId);

        if (!HoldsDrive)
            SpellCount++;
        else
            SpellCount = 0;
    }

    public void HitSpell()
    {
        if (HoldsDrive)
        {
            Spell(SpellButton.TargetPlayer);
            SpellButton.StartCooldown();
        }
        else
        {
            var interact = Interact(Player, SpellButton.TargetPlayer);

            if (interact.AbilityUsed)
                Spell(SpellButton.TargetPlayer);

            if (interact.Reset)
                SpellButton.StartCooldown();
            else if (interact.Protected)
                SpellButton.StartCooldown(CooldownType.GuardianAngel);
        }
    }

    //Stalker Stuff
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public CustomButton StalkButton { get; set; }
    public bool IsStalk => FormerRole?.Type == LayerEnum.Stalker;

    public bool StalkException(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public void DestroyArrow(byte targetPlayerId)
    {
        StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        StalkerArrows.Remove(targetPlayerId);
    }

    public void Stalk()
    {
        var interact = Interact(Player, StalkButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        StalkButton.StartCooldown(cooldown);
    }

    //Drunkard Stuff
    public CustomButton ConfuseButton { get; set; }
    public float Modifier => ConfuseButton.EffectActive ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomMenu ConfuseMenu { get; set; }
    public bool IsDrunk => FormerRole?.Type == LayerEnum.Drunkard;

    public bool DrunkException(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        !CustomGameOptions.ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConfuseImmunity);

    public void StartConfusion()
    {
        if (CustomPlayer.Local == ConfusedPlayer || HoldsDrive)
            Flash(Colors.Drunkard);
    }

    public void UnConfuse() => ConfusedPlayer = null;

    public void ConfuseClick(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            ConfusedPlayer = player;
        else if (interact.Reset)
            ConfuseButton.StartCooldown();
        else if (interact.Protected)
            ConfuseButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void HitConfuse()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConfuseButton, RebActionsRPC.Confuse);
            ConfuseButton.Begin();
        }
        else if (ConfusedPlayer == null)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConfuseButton, RebActionsRPC.Confuse, ConfusedPlayer);
            ConfuseButton.Begin();
        }
    }

    //Timekeeper Stuff
    public DateTime LastTimed { get; set; }
    public CustomButton TimeButton { get; set; }
    public bool IsTK => FormerRole?.Type == LayerEnum.Timekeeper;

    public void ControlStart() => Flash(Color, CustomGameOptions.TimeDur);

    public void Control()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = true);
    }

    public void UnControl() => CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = false);

    public void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, TimeButton);
        TimeButton.Begin();
    }

    //Silencer Stuff
    public CustomButton SilenceButton { get; set; }
    public PlayerControl SilencedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }
    public bool IsSil => FormerRole?.Type == LayerEnum.Silencer;

    public bool SilenceException(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        !CustomGameOptions.SilenceMates) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.SilenceMates);

    public void Silence()
    {
        var interact = Interact(Player, SilenceButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            SilencedPlayer = SilenceButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, RebActionsRPC.Silence, SilencedPlayer);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        SilenceButton.StartCooldown(cooldown);
    }
}