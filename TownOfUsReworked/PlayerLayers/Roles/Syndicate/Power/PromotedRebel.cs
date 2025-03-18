namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class PromotedRebel : Syndicate, ISilencer, IHexer, IMover, ICrusader, IFramer, IShaper, ITimeLord, IDrunkard
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Power;
        Framed.Clear();
        StalkerArrows.Clear();
        Spelled.Clear();
        Bombs.Clear();
        ConcealedPlayer = null;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        SilencedPlayer = null;
        PoisonedPlayer = null;
        Positive = null;
        Negative = null;
        ConfusedPlayer = null;
    }

    // Rebel Stuff
    public Syndicate FormerRole { get; init; }

    public override UColor MainColor => FormerRole?.Color ?? CustomColorManager.Rebel;
    public override LayerEnum Type => LayerEnum.PromotedRebel;
    public override Func<string> StartText => () => "Lead The <#008000FF>Syndicate</color>";
    public override Func<string> Description => () => "- You have succeeded the former <#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities" +
        (!FormerRole ? CommonAbilities : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");
    public override bool RoleBlockImmune => FormerRole?.RoleBlockImmune ?? false;

    public override void Reset(bool meeting, bool start)
    {
        ShapeshiftPlayer1 = ShapeshiftPlayer2 = PoisonedPlayer = ConcealedPlayer = Positive = Negative = SilencedPlayer = CrusadedPlayer = null;

        if (Bomber.BombsRemoveOnNewRound && meeting)
        {
            Bombs.ForEach(x => x?.gameObject?.Destroy());
            Bombs.Clear();
        }
    }

    protected override void OnTrueDeath() => Framed.Clear();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButtonDown("Delete"))
        {
            if (!HoldsDrive)
            {
                if (ShapeshiftPlayer2 && !ShapeshiftButton.EffectActive)
                    ShapeshiftPlayer2 = null;
                else if (ShapeshiftPlayer1 && !ShapeshiftButton.EffectActive)
                    ShapeshiftPlayer1 = null;
                else if (ConcealedPlayer && !ConcealButton.EffectActive)
                    ConcealedPlayer = null;
                else if (ConfusedPlayer && !ConfuseButton.EffectActive)
                    ConfusedPlayer = null;
                else if (IsWarp && !Moving && WarpMenu.Selected.Count > 0)
                    WarpMenu.Selected.TakeLast();
            }
            else if (PoisonedPlayer && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
                PoisonedPlayer = null;

            Message("Removed a target");
        }

        if (IsCol)
        {
            if (Dead)
                return;

            var shouldReset = false;

            if (GetDistance(Positive, Negative) <= Range)
            {
                if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                    Player.RpcMurderPlayer(Negative, DeathReasonEnum.Collided, false);

                if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                    Player.RpcMurderPlayer(Positive, DeathReasonEnum.Collided, false);

                Positive = null;
                Negative = null;
                shouldReset = true;
            }
            else if (GetDistance(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                    Player.RpcMurderPlayer(Negative, DeathReasonEnum.Collided, false);

                Negative = null;
                shouldReset = true;
            }
            else if (GetDistance(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                    Player.RpcMurderPlayer(Positive, DeathReasonEnum.Collided, false);

                Positive = null;
                shouldReset = true;
            }

            if (!Collider.CollideResetsCooldown || !shouldReset)
                return;

            PositiveButton.StartCooldown();
            NegativeButton.StartCooldown();
        }
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (IsSil && SilencedPlayer == player)
            name += " <#AAB43EFF>乂</color>";
        else if (IsCrus && CrusadedPlayer == player)
            name += " <#DF7AE8FF>τ</color>";
        else if (IsCol)
        {
            if (Positive == player)
                name += " <#B345FFFF>+</color>";

            if (Negative == player)
                name += " <#B345FFFF>-</color>";
        }
    }

    public void OnRoleSelected()
    {
        if (IsConc)
        {
            var wasnull = ConcealButton == null;
            ConcealButton ??= new(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConceal, new Cooldown(Concealer.ConcealCd),
                (LabelFunc)ConcLabel, new Duration(Concealer.ConcealDur), (EffectVoid)Conceal, (EffectEndVoid)UnConceal, (UsableFunc)ConcealUsable, (EndFunc)ConcealEnd);

            if (wasnull && ConfuseMenu == null)
                ConcealMenu = new(Player, ConcealClick, ConcealException);
        }
        else if (IsDrunk)
        {
            var wasnull = ConfuseButton == null;
            ConfuseButton ??= new(this, new SpriteName("Confuse"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConfuse, new Cooldown(Drunkard.ConfuseCd),
                new Duration(Drunkard.ConfuseDur), (EffectStartVoid)StartConfusion, (EffectEndVoid)UnConfuse, (LabelFunc)DrunkLabel, (EndFunc)ConfuseEnd, (UsableFunc)DrunkUsable);

            if (wasnull && ConfuseMenu == null)
                ConfuseMenu = new(Player, ConfuseClick, DrunkException);
        }
        else if (IsFram)
        {
            FrameButton ??= new(this, new SpriteName("Frame"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Frame, new Cooldown(Framer.FrameCd), "FRAME", (UsableFunc)FrameUsable1,
                (PlayerBodyExclusion)FrameException);
            RadialFrameButton ??= new(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)RadialFrame, (UsableFunc)FrameUsable2, "FRAME",
                new Cooldown(Framer.FrameCd));
        }
        else if (IsSS)
        {
            var wasnull = ShapeshiftButton == null;
            ShapeshiftButton ??= new(this, new SpriteName("Shapeshift"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitShapeshift, (EffectEndVoid)UnShapeshift,
                new Cooldown(Shapeshifter.ShapeshiftCd), new Duration(Shapeshifter.ShapeshiftDur), (EffectVoid)Shift, (LabelFunc)SSLabel, (UsableFunc)SSUsable);

            if (wasnull && (ShapeshiftMenu1 == null || ShapeshiftMenu2 == null))
            {
                ShapeshiftMenu1 = new(Player, ShapeshiftClick1, SSException1);
                ShapeshiftMenu2 = new(Player, ShapeshiftClick2, SSException2);
            }
        }
        else if (IsSil)
        {
            SilenceButton ??= new(this, new SpriteName("Silence"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(Silencer.SilenceCd), (UsableFunc)SilUsable,
                "SILENCE", (PlayerBodyExclusion)SilenceException);
        }
        else if (IsTk)
        {
            TimeButton ??= new(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(Timekeeper.TimeCd),
                (LabelFunc)TkLabel, new Duration(Timekeeper.TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart, (EffectEndVoid)Timekeeper.UnControl, (UsableFunc)TkUsable);
        }
        else if (IsBomb)
        {
            BombButton ??= new(this, new SpriteName("Plant"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Place, new Cooldown(Bomber.BombCd),
                "PLACE BOMB", (ConditionFunc)BombCondition, (UsableFunc)BombUsable1);
            DetonateButton ??= new(this, new SpriteName("Detonate"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Detonate, new Cooldown(Bomber.DetonateCd),
                "DETONATE", (UsableFunc)BombUsable2);
        }
        else if (IsCol)
        {
            PositiveButton ??= new(this, new SpriteName("Positive"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SetPositive, "SET POSITIVE", (UsableFunc)ColUsable1,
                new Cooldown(Collider.CollideCd), (PlayerBodyExclusion)PlusException);
            NegativeButton ??= new(this, new SpriteName("Negative"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)SetNegative, new Cooldown(Collider.CollideCd),
                (PlayerBodyExclusion)MinusException, "SET NEGATIVE", (UsableFunc)ColUsable1);
            ChargeButton ??= new(this, new SpriteName("Charge"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Charge, new Cooldown(Collider.ChargeCd), "CHARGE",
                new Duration(Collider.ChargeDur), (UsableFunc)ColUsable2, (EndFunc)ChargeEnd);
        }
        else if (IsCrus)
        {
            CrusadeButton ??= new(this, new SpriteName("Crusade"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Crusade, new Cooldown(Crusader.CrusadeCd), (EndFunc)CrusadeEnd,
                "CRUSADE", new Duration(Crusader.CrusadeDur), (EffectEndVoid)UnCrusade, (PlayerBodyExclusion)CrusadeException, (UsableFunc)CrusUsable);
        }
        else if (IsPois)
        {
            var wasnull = PoisonButton == null;
            PoisonButton ??= new(this, new SpriteName("Poison"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)HitPoison, new Cooldown(Poisoner.PoisonCd), (EndFunc)PoisonEnd,
                "POISON", new Duration(Poisoner.PoisonDur), (EffectEndVoid)UnPoison, (PlayerBodyExclusion)PoisonException, (UsableFunc)PoisUsable1);
            GlobalPoisonButton ??= new(this, new SpriteName("GlobalPoison"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitGlobalPoison, (LabelFunc)PoisLabel,
                new Cooldown(Poisoner.PoisonCd), new Duration(Poisoner.PoisonDur), (EffectEndVoid)UnPoison, (UsableFunc)PoisUsable2, (EndFunc)PoisonEnd);

            if (wasnull && PoisonMenu == null)
                PoisonMenu = new(Player, PoisonClick, PoisonException);
        }
        else if (IsSpell)
        {
            SpellButton ??= new(this, new SpriteName("Spellbind"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Spell, new Cooldown(Spellslinger.SpellCd), (UsableFunc)SpellUsable,
                "SPELLBIND", (PlayerBodyExclusion)SpellException, (DifferenceFunc)SpellDifference);
        }
        else if (IsStalk)
        {
            StalkButton ??= new(this, new SpriteName("Stalk"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Stalk, new Cooldown(Stalker.StalkCd), "STALK",
                (PlayerBodyExclusion)StalkException, (UsableFunc)StalkUsable);
        }
        else if (IsWarp)
        {
            var wasnull = WarpButton == null;
            WarpButton ??= new(this, new SpriteName("Warp"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Warp, (LabelFunc)WarpLabel, new Cooldown(Warper.WarpCd),
                (UsableFunc)WarpUsable);

            if (wasnull && WarpMenu == null)
                WarpMenu = new(Player, WarpClick, WarpException);
        }

        Player.ResetButtons();
    }

    protected override void Deinit()
    {
        base.Deinit();

        Bombs.ForEach(x => x?.gameObject?.Destroy());
        Bombs.Clear();

        ResetCharges();
    }

    public override void ClearArrows()
    {
        base.ClearArrows();

        StalkerArrows.Values.DestroyAll();
        StalkerArrows.Clear();
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);
        ClearArrows();
    }

    public override void BeforeMeeting()
    {
        if (Bomber.BombsDetonateOnMeetingStart && IsBomb)
        {
            Bombs.ForEach(x => x.Detonate());
            Bombs.Clear();
        }
        else if (IsCol)
            ResetCharges();
    }

    public override void ReadRPC(MessageReader reader)
    {
        var rebAction = reader.ReadEnum<RebActionsRPC>();

        switch (rebAction)
        {
            case RebActionsRPC.Poison:
            {
                PoisonedPlayer = reader.ReadPlayer();
                break;
            }
            case RebActionsRPC.Conceal:
            {
                if (!HoldsDrive)
                    ConcealedPlayer = reader.ReadPlayer();

                break;
            }
            case RebActionsRPC.Shapeshift:
            {
                if (!HoldsDrive)
                {
                    ShapeshiftPlayer1 = reader.ReadPlayer();
                    ShapeshiftPlayer2 = reader.ReadPlayer();
                }

                break;
            }
            case RebActionsRPC.WarpAll:
            {
                var coords = new Dictionary<byte, Vector2>();
                var num = reader.ReadByte();

                while (num-- > 0)
                    coords[reader.ReadByte()] = reader.ReadVector2();

                Coroutines.Start(Warper.WarpAll(coords, this));
                break;
            }
            case RebActionsRPC.WarpSingle:
            {
                Coroutines.Start(Warper.WarpPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));
                break;
            }
            case RebActionsRPC.Crusade:
            {
                CrusadedPlayer = reader.ReadPlayer();
                break;
            }
            case RebActionsRPC.Spellbind:
            {
                Spelled.Add(reader.ReadByte());

                if (AmongUsClient.Instance.AmHost)
                    CheckEndGame.CheckSpellWin(this);

                break;
            }
            case RebActionsRPC.Frame:
            {
                Framed.Add(reader.ReadByte());
                break;
            }
            case RebActionsRPC.Confuse:
            {
                if (!HoldsDrive)
                    ConfusedPlayer = reader.ReadPlayer();

                break;
            }
            case RebActionsRPC.Silence:
            {
                SilencedPlayer = reader.ReadPlayer();
                break;
            }
            case RebActionsRPC.Explode:
            {
                if (Bomber.BombAlert)
                    Play("Bomb");

                Bombs.ForEach(x => x.gameObject.Destroy());
                Bombs.Clear();
                break;
            }
            case RebActionsRPC.DropBomb:
            {
                Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {rebAction}");
                break;
            }
        }
    }

    protected override void OnDriveReceivedLocal()
    {
        if (IsStalk)
        {
            foreach (var player in AllPlayers())
            {
                if (!StalkerArrows.ContainsKey(player.PlayerId))
                    StalkerArrows.Add(player.PlayerId, new(Player, player, player.GetPlayerColor(false)));
            }
        }
    }

    public override void OnRevive() => OnRoleSelected();

    // Anarchist Stuff
    // public bool IsAnarch => FormerRole is Anarchist;

    // Concealer Stuff
    private CustomButton ConcealButton { get; set; }
    private PlayerControl ConcealedPlayer { get; set; }
    private CustomPlayerMenu ConcealMenu { get; set; }
    private bool IsConc => FormerRole is Concealer;

    private bool ConcealException(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !Concealer.ConcealMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Concealer.ConcealMates);

    private void Conceal()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));
    }

    private void UnConceal()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);

        ConcealedPlayer = null;
    }

    private void ConcealClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConcealedPlayer = player;
        else
            ConcealButton.StartCooldown(cooldown);
    }

    private void HitConceal()
    {
        if (HoldsDrive || ConcealedPlayer)
        {
            var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.ButtonAction, RebActionsRPC.Conceal, ConcealButton);

            if (ConcealedPlayer)
                writer.Write(ConcealedPlayer.PlayerId);

            writer.CloseRpc();
            ConcealButton.Begin();
        }
        else
            ConcealMenu.Open();
    }

    private bool ConcealUsable() => IsConc;

    private string ConcLabel() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    private bool ConcealEnd() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead);

    // Framer Stuff
    private CustomButton FrameButton { get; set; }
    public List<byte> Framed { get; } = [];
    private CustomButton RadialFrameButton { get; set; }
    private bool IsFram => FormerRole is Framer;

    private bool FrameException(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    private void RpcFrame(PlayerControl player)
    {
        if (FrameException(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Frame, player.PlayerId);
    }

    private void Frame(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcFrame(target);

        FrameButton.StartCooldown(cooldown);
    }

    private void RadialFrame()
    {
        GetClosestPlayers(Player, Framer.ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    private bool FrameUsable1() => IsFram && !HoldsDrive;

    private bool FrameUsable2() => IsFram && HoldsDrive;

    // Poisoner Stuff
    private CustomButton PoisonButton { get; set; }
    private CustomButton GlobalPoisonButton { get; set; }
    private PlayerControl PoisonedPlayer { get; set; }
    private CustomPlayerMenu PoisonMenu { get; set; }
    private bool IsPois => FormerRole is Poisoner;

    private bool PoisonException(PlayerControl player) => player == PoisonedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    private void UnPoison()
    {
        if (!PoisonedPlayer.HasDied() && CanAttack(AttackEnum.Basic, PoisonedPlayer.GetDefenseValue(Player)))
            Player.MurderPlayer(PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
    }

    private void PoisonClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player, astral: true, delayed: true);

        if (cooldown != CooldownType.Fail)
            PoisonedPlayer = player;
        else
            GlobalPoisonButton.StartCooldown(cooldown);
    }

    private void HitPoison(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true, delayed: true);

        if (cooldown != CooldownType.Fail)
        {
            PoisonedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, PoisonButton, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else
            PoisonButton.StartCooldown(cooldown);
    }

    private void HitGlobalPoison()
    {
        if (PoisonedPlayer)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GlobalPoisonButton, RebActionsRPC.Poison, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
        else
            PoisonMenu.Open();
    }

    private bool PoisonEnd() => PoisonedPlayer.HasDied() || Dead;

    private string PoisLabel() => PoisonedPlayer ? "POISON" : "SET TARGET";

    private bool PoisUsable1() => !HoldsDrive && IsPois;

    private bool PoisUsable2() => HoldsDrive && IsPois;

    // Shapeshifter Stuff
    private CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; private set; }
    public PlayerControl ShapeshiftPlayer2 { get; private set; }
    private CustomPlayerMenu ShapeshiftMenu1 { get; set; }
    private CustomPlayerMenu ShapeshiftMenu2 { get; set; }
    private bool IsSS => FormerRole is Shapeshifter;

    private bool SSException1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) &&
        !Shapeshifter.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Shapeshifter.ShapeshiftMates);

    private bool SSException2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) &&
        !Shapeshifter.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&  !Shapeshifter.ShapeshiftMates);

    private void Shift() => Shapeshifter.Shapeshift(ShapeshiftPlayer1, ShapeshiftPlayer2, HoldsDrive);

    private void UnShapeshift()
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

    private void ShapeshiftClick1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer1 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    private void ShapeshiftClick2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer2 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    private void HitShapeshift()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ShapeshiftButton, RebActionsRPC.Shapeshift);
            ShapeshiftButton.Begin();
        }
        else if (!ShapeshiftPlayer1)
            ShapeshiftMenu1.Open();
        else if (!ShapeshiftPlayer2)
            ShapeshiftMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ShapeshiftButton, RebActionsRPC.Shapeshift, ShapeshiftPlayer1, ShapeshiftPlayer2);
            ShapeshiftButton.Begin();
        }
    }

    private string SSLabel()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";

        if (!ShapeshiftPlayer1)
            return "FIRST TARGET";

        return !ShapeshiftPlayer2 ? "SECOND TARGET" : "SHAPESHIFT";
    }

    private bool SSUsable() => IsSS;

    // Bomber Stuff
    private CustomButton BombButton { get; set; }
    private CustomButton DetonateButton { get; set; }
    private List<Bomb> Bombs { get; } = [];
    private bool IsBomb => FormerRole is Bomber;

    private void Place()
    {
        Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (Bomber.BombCooldownsLinked)
            DetonateButton.StartCooldown();

        if (Bomber.ShowBomb)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.DropBomb);
    }

    private void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (Bomber.BombCooldownsLinked)
            BombButton.StartCooldown();

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Explode);
        Play("Bomb");
    }

    private bool BombCondition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    private bool BombUsable1() => IsBomb;

    private bool BombUsable2() => IsBomb && Bombs.Any();

    // Warper Stuff
    private CustomButton WarpButton { get; set; }
    private CustomPlayerMenu WarpMenu { get; set; }
    public bool Moving { get; set; }
    private bool IsWarp => FormerRole is Warper;

    private bool WarpException(PlayerControl player) => (player == Player && !Warper.WarpSelf) || UninteractablePlayers.ContainsKey(player.PlayerId) || (!BodyById(player.PlayerId) &&
        player.Data.IsDead) || player.IsMoving();

    private bool WarpClick(PlayerControl player, out bool shouldClose)
    {
        shouldClose = false;

        if (player.IsMoving())
            return false;

        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            return true;

        WarpButton.StartCooldown(cooldown);
        shouldClose = true;
        return false;
    }

    private void Warp()
    {
        if (HoldsDrive)
        {
            var coords = GenerateWarpCoordinates();
            var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.WarpAll);

            if (writer != null)
            {
                writer.Write((byte)coords.Count);

                foreach (var (id, pos) in coords)
                {
                    writer.Write(id);
                    writer.Write(pos);
                }

                writer.CloseRpc();
            }

            Coroutines.Start(Warper.WarpAll(coords, this));
            WarpButton.StartCooldown();
        }
        else if (WarpMenu.Selected.Count < 2)
            WarpMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.WarpSingle, WarpMenu.Selected[0], WarpMenu.Selected[1]);
            Coroutines.Start(Warper.WarpPlayers(PlayerById(WarpMenu.Selected[0]), PlayerById(WarpMenu.Selected[1]), this));
            WarpMenu.Selected.Clear();
            WarpButton.StartCooldown();
        }
    }

    private bool WarpUsable() => IsWarp;

    private string WarpLabel()
    {
        if (HoldsDrive)
            return "WARP";

        return WarpMenu.Selected.Count switch
        {
            0 => "FIRST TARGET",
            1 => "SECOND TARGET",
            _ =>  "WARP"
        };
    }

    // Crusader Stuff
    public PlayerControl CrusadedPlayer { get; private set; }
    public CustomButton CrusadeButton { get; private set; }
    private bool IsCrus => FormerRole is Crusader;

    private bool CrusadeException(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !Crusader.CrusadeMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Crusader.CrusadeMates);

    private void UnCrusade() => CrusadedPlayer = null;

    private void Crusade(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CrusadedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CrusadeButton, RebActionsRPC.Crusade, CrusadedPlayer);
            CrusadeButton.Begin();
        }
        else
            CrusadeButton.StartCooldown(cooldown);
    }

    private bool CrusUsable() => IsCrus;

    private bool CrusadeEnd() => (CrusadedPlayer && CrusadedPlayer.HasDied()) || Dead;

    // Collider Stuff
    private CustomButton PositiveButton { get; set; }
    private CustomButton NegativeButton { get; set; }
    private CustomButton ChargeButton { get; set; }
    private PlayerControl Positive { get; set; }
    private PlayerControl Negative { get; set; }
    private float Range => Collider.CollideRange + (HoldsDrive ? Collider.CollideRangeIncrease : 0);
    private bool IsCol => FormerRole is Collider;

    private bool PlusException(PlayerControl player) => player == Negative || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction
        != SubFaction.None) || Player.IsLinkedTo(player);

    private bool MinusException(PlayerControl player) => player == Positive || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    private void Charge() => ChargeButton.Begin();

    private void SetPositive(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Positive = target;

        PositiveButton.StartCooldown(cooldown);

        if (Collider.ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    private void SetNegative(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Negative = target;

        NegativeButton.StartCooldown(cooldown);

        if (Collider.ChargeCooldownsLinked)
            PositiveButton.StartCooldown(cooldown);
    }

    private void ResetCharges()
    {
        Positive = null;
        Negative = null;
    }

    private bool ColUsable1() => IsCol;

    private bool ColUsable2() => IsCol && HoldsDrive;

    private bool ChargeEnd() => Dead;

    // Spellslinger Stuff
    private CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; } = [];
    private bool IsSpell => FormerRole is Spellslinger;

    private bool SpellException(PlayerControl player) => Spelled.Contains(player.PlayerId);

    private void Spell(PlayerControl target)
    {
        var cooldown = Interact(Player, target, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
        {
            Spelled.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Spellbind, target.PlayerId);

            if (AmongUsClient.Instance.AmHost)
                CheckEndGame.CheckSpellWin(this);
        }

        SpellButton.StartCooldown(cooldown);
    }

    private bool SpellUsable() => IsSpell;

    private float SpellDifference() => HoldsDrive ? 0 : (Spelled.Count * Spellslinger.SpellCdIncrease);

    // Stalker Stuff
    private Dictionary<byte, PlayerArrow> StalkerArrows { get; } = [];
    private CustomButton StalkButton { get; set; }
    private bool IsStalk => FormerRole is Stalker;

    private bool StalkException(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    private void Stalk(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            StalkerArrows.Add(target.PlayerId, new(Player, target, target.GetPlayerColor(!HoldsDrive)));

        StalkButton.StartCooldown(cooldown);
    }

    private bool StalkUsable() => !HoldsDrive && IsStalk;

    // Drunkard Stuff
    public CustomButton ConfuseButton { get; private set; }
    public PlayerControl ConfusedPlayer { get; private set; }
    private CustomPlayerMenu ConfuseMenu { get; set; }
    private bool IsDrunk => FormerRole is Drunkard;

    private bool DrunkException(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        !Drunkard.ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Drunkard.ConfuseImmunity);

    private void StartConfusion()
    {
        if (ConfusedPlayer.AmOwner || HoldsDrive)
            Flash(CustomColorManager.Drunkard);
    }

    private void UnConfuse() => ConfusedPlayer = null;

    private void ConfuseClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConfusedPlayer = player;
        else
            ConfuseButton.StartCooldown(cooldown);
    }

    private void HitConfuse()
    {
        if (HoldsDrive || ConfusedPlayer)
        {
            var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.ButtonAction, RebActionsRPC.Confuse, ConfusedPlayer);

            if (ConfusedPlayer)
                writer.Write(ConfusedPlayer.PlayerId);

            writer.CloseRpc();
            ConfuseButton.Begin();
        }
        else
            ConfuseMenu.Open();
    }

    private string DrunkLabel() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    private bool ConfuseEnd() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    private bool DrunkUsable() => IsDrunk;

    // Timekeeper Stuff
    public CustomButton TimeButton { get; private set; }
    private bool IsTk => FormerRole is Timekeeper;

    private void ControlStart() => Flash(Color, Timekeeper.TimeDur);

    private void Control()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => x.GetRole().Rewinding = true);
    }

    private void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    private string TkLabel() => HoldsDrive ? "REWIND" : "FREEZE";

    private bool TkUsable() => IsTk;

    // Silencer Stuff
    private CustomButton SilenceButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target => SilencedPlayer;
    public PlayerControl SilencedPlayer { get; private set; }
    private bool IsSil => FormerRole is Silencer;

    private bool SilenceException(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !Silencer.SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Silencer.SilenceMates);

    private void Silence(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            SilencedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Silence, SilencedPlayer);

            if (target.IsBlackmailed())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        SilenceButton.StartCooldown(cooldown);
    }

    private bool SilUsable() => IsSil;
}