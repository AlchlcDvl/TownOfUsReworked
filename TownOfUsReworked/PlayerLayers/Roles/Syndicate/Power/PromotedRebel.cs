namespace TownOfUsReworked.PlayerLayers.Roles;

public class PromotedRebel : Syndicate, IIntimidator
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicatePower;
        SpellCount = 0;
        Framed = [];
        StalkerArrows = [];
        Spelled = [];
        Bombs = [];
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
    }

    // Rebel Stuff
    public Syndicate FormerRole { get; set; }

    public override UColor Color
    {
        get
        {
            if (!ClientOptions.CustomSynColors)
                return CustomColorManager.Syndicate;
            else
                return FormerRole?.Color ?? CustomColorManager.Rebel;
        }
    }
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.PromotedRebel;
    public override Func<string> StartText => () => "Lead The <#008000FF>Syndicate</color>";
    public override Func<string> Description => () => "- You have succeeded the former <#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities" +
        (!FormerRole ? CommonAbilities : $"\n{FormerRole.ColorString}{FormerRole.Description()}</color>");
    public override bool RoleBlockImmune => FormerRole?.RoleBlockImmune ?? false;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButton("Delete"))
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
                else if (WarpPlayer2 && !Warping)
                    WarpPlayer2 = null;
                else if (WarpPlayer1 && !Warping)
                    WarpPlayer1 = null;
            }
            else if (PoisonedPlayer && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
                PoisonedPlayer = null;

            Message("Removed a target");
        }

        if (IsStalk)
        {
            if (Dead)
                return;

            foreach (var pair in StalkerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (!player || player.Data.Disconnected || (player.Data.IsDead && !body))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor(!HoldsDrive));
            }
        }
        else if (IsCol)
        {
            if (Dead)
                return;

            var shouldReset = false;

            if (GetDistance(Positive, Negative) <= Range)
            {
                if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                    RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

                if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                    RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

                Positive = null;
                Negative = null;
                shouldReset = true;
            }
            else if (GetDistance(Player, Negative) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (CanAttack(AttackEnum.Powerful, Negative.GetDefenseValue(Player)))
                    RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);

                Negative = null;
                shouldReset = true;
            }
            else if (GetDistance(Player, Positive) <= Range && HoldsDrive && ChargeButton.EffectActive)
            {
                if (CanAttack(AttackEnum.Powerful, Positive.GetDefenseValue(Player)))
                    RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);

                Positive = null;
                shouldReset = true;
            }

            if (Collider.CollideResetsCooldown && shouldReset)
            {
                PositiveButton.StartCooldown();
                NegativeButton.StartCooldown();
            }
        }
    }

    public void OnRoleSelected()
    {
        if (IsConc)
        {
            var wasnull = ConcealButton == null;
            ConcealButton ??= new(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConceal, new Cooldown(Concealer.ConcealCd),
                (LabelFunc)ConcLabel, new Duration(Concealer.ConcealDur), (EffectVoid)Conceal, (EffectEndVoid)UnConceal, (UsableFunc)ConcealUsable);

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
            FrameButton ??= new(this, new SpriteName("Frame"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)Frame, new Cooldown(Framer.FrameCd), "FRAME", (UsableFunc)FrameUsable1,
                (PlayerBodyExclusion)FrameException);
            RadialFrameButton ??= new(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)RadialFrame, (UsableFunc)FrameUsable2, "FRAME",
                new Cooldown(Framer.FrameCd));
        }
        else if (IsSS)
        {
            var wasnull = ShapeshiftButton == null;
            ShapeshiftButton ??= new(this, "Shapeshift", AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitShapeshift, new Cooldown(Shapeshifter.ShapeshiftCd),
                new Duration(Shapeshifter.ShapeshiftDur), (EffectVoid)Shift, (EffectEndVoid)UnShapeshift, (LabelFunc)SSLabel, (UsableFunc)SSUsable);

            if (wasnull && (ShapeshiftMenu1 == null || ShapeshiftMenu2 == null))
            {
                ShapeshiftMenu1 = new(Player, ShapeshiftClick1, SSException1);
                ShapeshiftMenu2 = new(Player, ShapeshiftClick2, SSException2);
            }
        }
        else if (IsSil)
        {
            SilenceButton ??= new(this, new SpriteName("Silence"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(Silencer.SilenceCd), (UsableFunc)SilUsable,
                "SILENCE", (PlayerBodyExclusion)SilenceException);
        }
        else if (IsTK)
        {
            TimeButton ??= new(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)TimeControl, new Cooldown(Timekeeper.TimeCd),
                (LabelFunc)TKLabel, new Duration(Timekeeper.TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart, (EffectEndVoid)UnControl, (UsableFunc)TKUsable);
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
            PositiveButton ??= new(this, new SpriteName("Positive"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)SetPositive, "SET POSITIVE", (UsableFunc)ColUsable1,
                new Cooldown(Collider.CollideCd), (PlayerBodyExclusion)PlusException);
            NegativeButton ??= new(this, new SpriteName("Negative"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)SetNegative, new Cooldown(Collider.CollideCd),
                (PlayerBodyExclusion)MinusException, "SET NEGATIVE", (UsableFunc)ColUsable1);
            ChargeButton ??= new(this, new SpriteName("Charge"), AbilityTypes.Targetless, KeybindType.Tertiary, (OnClickTargetless)Charge, new Cooldown(Collider.ChargeCd), "CHARGE",
                new Duration(Collider.ChargeDur), (UsableFunc)ColUsable2, (EndFunc)ChargeEnd);
        }
        else if (IsCrus)
        {
            CrusadeButton ??= new(this, new SpriteName("Crusade"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)Crusade, new Cooldown(Crusader.CrusadeCd), (EndFunc)CrusadeEnd,
                "CRUSADE", new Duration(Crusader.CrusadeDur), (EffectEndVoid)UnCrusade, (PlayerBodyExclusion)CrusadeException, (UsableFunc)CrusUsable);
        }
        else if (IsPois)
        {
            var wasnull = PoisonButton == null;
            PoisonButton ??= new(this, new SpriteName("Poison"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)HitPoison, new Cooldown(Poisoner.PoisonCd), (EndFunc)PoisonEnd,
                "POISON", new Duration(Poisoner.PoisonDur), (EffectEndVoid)UnPoison, (PlayerBodyExclusion)PoisonException, (UsableFunc)PoisUsable1);
            GlobalPoisonButton ??= new(this, new SpriteName("GlobalPoison"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitGlobalPoison, (LabelFunc)PoisLabel,
                new Cooldown(Poisoner.PoisonCd), new Duration(Poisoner.PoisonDur), (EffectEndVoid)UnPoison, (UsableFunc)PoisUsable2, (EndFunc)PoisonEnd);

            if (wasnull && PoisonMenu == null)
                PoisonMenu = new(Player, PoisonClick, PoisonException);
        }
        else if (IsSpell)
        {
            SpellButton ??= new(this, new SpriteName("Spellbind"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)Spell, new Cooldown(Spellslinger.SpellCd), (UsableFunc)SpellUsable,
                "SPELLBIND", (PlayerBodyExclusion)SpellException, (DifferenceFunc)SpellDifference);
        }
        else if (IsStalk)
        {
            StalkButton ??= new(this, new SpriteName("Stalk"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Stalk, new Cooldown(Stalker.StalkCd), "STALK",
                (PlayerBodyExclusion)StalkException, (UsableFunc)StalkUsable);
        }
        else if (IsWarp)
        {
            var wasnull = WarpButton == null;
            WarpButton ??= new(this, new SpriteName("Warp"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Warp, (LabelFunc)WarpLabel, new Cooldown(Warper.WarpCd),
                (UsableFunc)WarpUsable);

            if (wasnull && !WarpObj)
            {
                WarpObj = new("RebWarp") { layer = 5 };
                WarpObj.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
                AnimationPlaying = WarpObj.AddComponent<SpriteRenderer>();
                AnimationPlaying.sprite = PortalAnimation[0];
                AnimationPlaying.material = HatManager.Instance.PlayerMaterial;
                WarpObj.SetActive(true);
                WarpMenu1 = new(Player, WarpClick1, WarpException1);
                WarpMenu2 = new(Player, WarpClick2, WarpException2);

                if (IsSubmerged())
                    WarpObj.AddSubmergedComponent("ElevatorMover");
            }
        }

        Player.ResetButtons();
    }

    public override void Deinit()
    {
        base.Deinit();

        Bombs.ForEach(x => x?.gameObject?.Destroy());
        Bombs.Clear();

        StalkerArrows.Values.ToList().DestroyAll();
        StalkerArrows.Clear();

        WarpObj?.Destroy();

        ResetCharges();
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);
        Deinit();
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
            case RebActionsRPC.Warp:
            {
                WarpPlayer1 = reader.ReadPlayer();
                WarpPlayer2 = reader.ReadPlayer();
                Coroutines.Start(WarpPlayers());
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
                Error($"Received unknown RPC - {(int)rebAction}");
                break;
            }
        }
    }

    public override void OnDriveReceivedLocal()
    {
        if (IsStalk)
        {
            foreach (var player in AllPlayers())
            {
                if (!StalkerArrows.ContainsKey(player.PlayerId))
                    StalkerArrows.Add(player.PlayerId, new(Player, player.GetPlayerColor(false)));
            }
        }
    }

    public override void OnRevive() => OnRoleSelected();

    // Anarchist Stuff
    public bool IsAnarch => FormerRole is Anarchist;

    // Concealer Stuff
    public CustomButton ConcealButton { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomPlayerMenu ConcealMenu { get; set; }
    public bool IsConc => FormerRole is Concealer;

    public bool ConcealException(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !Concealer.ConcealMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Concealer.ConcealMates);

    public void Conceal()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
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
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConcealedPlayer = player;
        else
            ConcealButton.StartCooldown(cooldown);
    }

    public void HitConceal()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton, RebActionsRPC.Conceal);
            ConcealButton.Begin();
        }
        else if (!ConcealedPlayer)
            ConcealMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton, RebActionsRPC.Conceal, ConcealedPlayer);
            ConcealButton.Begin();
        }
    }

    public bool ConcealUsable() => IsConc;

    public string ConcLabel() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    public bool ConcealEnd() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead);

    // Framer Stuff
    public CustomButton FrameButton { get; set; }
    public List<byte> Framed { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public bool IsFram => FormerRole is Framer;

    public bool FrameException(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public void RpcFrame(PlayerControl player)
    {
        if (FrameException(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Frame, player.PlayerId);
    }

    public void Frame(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcFrame(target);

        FrameButton.StartCooldown(cooldown);
    }

    public void RadialFrame()
    {
        GetClosestPlayers(Player, Framer.ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    public bool FrameUsable1() => IsFram && !HoldsDrive;

    public bool FrameUsable2() => IsFram && HoldsDrive;

    // Poisoner Stuff
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public CustomPlayerMenu PoisonMenu { get; set; }
    public bool IsPois => FormerRole is Poisoner;

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
        var cooldown = Interact(Player, player, astral: true, delayed: true);

        if (cooldown != CooldownType.Fail)
            PoisonedPlayer = player;
        else
            GlobalPoisonButton.StartCooldown(cooldown);
    }

    public void HitPoison(PlayerControl target)
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

    public void HitGlobalPoison()
    {
        if (!PoisonedPlayer)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GlobalPoisonButton, RebActionsRPC.Poison, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
    }

    public bool PoisonEnd() => PoisonedPlayer.HasDied() || Dead;

    public string PoisLabel() => PoisonedPlayer ? "POISON" : "SET TARGET";

    public bool PoisUsable1() => !HoldsDrive && IsPois;

    public bool PoisUsable2() => HoldsDrive && IsPois;

    // Shapeshifter Stuff
    public CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; set; }
    public PlayerControl ShapeshiftPlayer2 { get; set; }
    public CustomPlayerMenu ShapeshiftMenu1 { get; set; }
    public CustomPlayerMenu ShapeshiftMenu2 { get; set; }
    public bool IsSS => FormerRole is Shapeshifter;

    public bool SSException1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) &&
        !Shapeshifter.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Shapeshifter.ShapeshiftMates);

    public bool SSException2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) &&
        !Shapeshifter.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&  !Shapeshifter.ShapeshiftMates);

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
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer1 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    public void ShapeshiftClick2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer2 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    public void HitShapeshift()
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

    public string SSLabel()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";
        else if (!ShapeshiftPlayer1)
            return "FIRST TARGET";
        else if (!ShapeshiftPlayer2)
            return "SECOND TARGET";
        else
            return "SHAPESHIFT";
    }

    public bool SSUsable() => IsSS;

    // Bomber Stuff
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }
    public bool IsBomb => FormerRole is Bomber;

    public void Place()
    {
        Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (Bomber.BombCooldownsLinked)
            DetonateButton.StartCooldown();

        if (Bomber.ShowBomb)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.DropBomb);
    }

    public void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (Bomber.BombCooldownsLinked)
            BombButton.StartCooldown();

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Explode);
        Play("Bomb");
    }

    public bool BombCondition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    public bool BombUsable1() => IsBomb;

    public bool BombUsable2() => IsBomb && Bombs.Any();

    // Warper Stuff
    public CustomButton WarpButton { get; set; }
    public PlayerControl WarpPlayer1 { get; set; }
    public PlayerControl WarpPlayer2 { get; set; }
    public CustomPlayerMenu WarpMenu1 { get; set; }
    public CustomPlayerMenu WarpMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying { get; set; }
    public GameObject WarpObj { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent { get; set; }
    public bool Warping { get; set; }
    public Vent Vent { get; set; }
    public bool IsWarp => FormerRole is Warper;

    public bool WarpException1(PlayerControl player) => (player == Player && !Warper.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
        (!BodyById(player.PlayerId) && player.Data.IsDead) || player.IsMoving();

    public bool WarpException2(PlayerControl player) => (player == Player && !Warper.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
        (!BodyById(player.PlayerId) && player.Data.IsDead) || player.IsMoving();

    public IEnumerator WarpPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;

        if (WarpPlayer1.Data.IsDead)
        {
            Player1Body = BodyById(WarpPlayer1.PlayerId);

            if (!Player1Body)
                yield break;
        }

        if (WarpPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(WarpPlayer2.PlayerId);

            if (!Player2Body)
                yield break;
        }

        if (WarpPlayer1.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            WarpPlayer1.MyPhysics.ExitAllVents();
        }

        if (WarpPlayer2.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            Vent = WarpPlayer2.GetClosestVent();
            WasInVent = true;
        }

        Warping = true;

        if (!WarpPlayer1.HasDied())
        {
            WarpPlayer1.moveable = false;
            WarpPlayer1.NetTransform.Halt();
        }

        if (WarpPlayer1.AmOwner)
            Flash(Color, Warper.WarpDur);

        if (!Player1Body && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < Warper.WarpDur)
                yield return EndFrame();
            else
                break;

            if (Meeting())
            {
                AnimationPlaying.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (!Player1Body && !Player2Body)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.CustomSnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged() && WarpPlayer1.AmOwner)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (WarpPlayer1.CanVent() && Vent && WasInVent)
                WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
        }
        else if (Player1Body && !Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = WarpPlayer2.GetTruePosition();

            if (IsSubmerged() && WarpPlayer2.AmOwner)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (!Player1Body && Player2Body)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.CustomSnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged() && WarpPlayer1.AmOwner)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body && Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = Player2Body.TruePosition;
        }

        if (WarpPlayer1.AmOwner)
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (MapBehaviourPatches.MapActive)
                Map().Close();
        }

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

        Coroutines.Start(PerformTimedAction(Warper.WarpDur, p =>
        {
            var index = (int)(p * PortalAnimation.Count);
            index = Mathf.Clamp(index, 0, PortalAnimation.Count - 1);
            AnimationPlaying.sprite = PortalAnimation[index];
            WarpPlayer1.SetPlayerMaterialColors(AnimationPlaying);

            if (p == 1)
                AnimationPlaying.sprite = PortalAnimation[0];
        }));
    }

    public void WarpClick1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            WarpPlayer1 = player;
        else
            WarpButton.StartCooldown(cooldown);
    }

    public void WarpClick2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            WarpPlayer2 = player;
        else
            WarpButton.StartCooldown(cooldown);
    }

    public void Warp()
    {
        if (HoldsDrive)
        {
            Warper.WarpAll();
            WarpButton.StartCooldown();
        }
        else if (!WarpPlayer1)
            WarpMenu1.Open();
        else if (!WarpPlayer2)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Warp, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    public bool WarpUsable() => IsWarp;

    public string WarpLabel()
    {
        if (HoldsDrive)
            return "WARP";
        else if (!WarpPlayer1)
            return "FIRST TARGET";
        else if (!WarpPlayer2)
            return "SECOND TARGET";
        else
            return "WARP";
    }

    // Crusader Stuff
    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }
    public bool IsCrus => FormerRole is Crusader;

    public bool CrusadeException(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !Crusader.CrusadeMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Crusader.CrusadeMates);

    public void UnCrusade() => CrusadedPlayer = null;

    public void Crusade(PlayerControl target)
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

    public bool CrusUsable() => IsCrus;

    public bool CrusadeEnd() => (CrusadedPlayer && CrusadedPlayer.HasDied()) || Dead;

    // Collider Stuff
    public CustomButton PositiveButton { get; set; }
    public CustomButton NegativeButton { get; set; }
    public CustomButton ChargeButton { get; set; }
    public PlayerControl Positive { get; set; }
    public PlayerControl Negative { get; set; }
    private float Range => Collider.CollideRange + (HoldsDrive ? Collider.CollideRangeIncrease : 0);
    public bool IsCol => FormerRole is Collider;

    public bool PlusException(PlayerControl player) => player == Negative || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction
        != SubFaction.None) || Player.IsLinkedTo(player);

    public bool MinusException(PlayerControl player) => player == Positive || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void Charge() => ChargeButton.Begin();

    public void SetPositive(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Positive = target;

        PositiveButton.StartCooldown(cooldown);

        if (Collider.ChargeCooldownsLinked)
            NegativeButton.StartCooldown(cooldown);
    }

    public void SetNegative(PlayerControl target)
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

    public bool ColUsable1() => IsCol;

    public bool ColUsable2() => IsCol && HoldsDrive;

    public bool ChargeEnd() => Dead;

    // Spellslinger Stuff
    public CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; set; }
    public int SpellCount { get; set; }
    public bool IsSpell => FormerRole is Spellslinger;

    public bool SpellException(PlayerControl player) => Spelled.Contains(player.PlayerId);

    public void Spell(PlayerControl target)
    {
        var cooldown = Interact(Player, target, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
        {
            Spelled.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Spellbind, target.PlayerId);

            if (HoldsDrive)
                SpellCount = 0;
            else
                SpellCount++;
        }

        SpellButton.StartCooldown(cooldown);
    }

    public bool SpellUsable() => IsSpell;

    public float SpellDifference() => SpellCount * Spellslinger.SpellCdIncrease;

    // Stalker Stuff
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public CustomButton StalkButton { get; set; }
    public bool IsStalk => FormerRole is Stalker;

    public bool StalkException(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public void DestroyArrow(byte targetPlayerId)
    {
        StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        StalkerArrows.Remove(targetPlayerId);
    }

    public void Stalk(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            StalkerArrows.Add(target.PlayerId, new(Player, target.GetPlayerColor(!HoldsDrive)));

        StalkButton.StartCooldown(cooldown);
    }

    public bool StalkUsable() => !HoldsDrive && IsStalk;

    // Drunkard Stuff
    public CustomButton ConfuseButton { get; set; }
    public float Modifier => ConfuseButton.EffectActive ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomPlayerMenu ConfuseMenu { get; set; }
    public bool IsDrunk => FormerRole is Drunkard;

    public bool DrunkException(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        !Drunkard.ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !Drunkard.ConfuseImmunity);

    public void StartConfusion()
    {
        if (ConfusedPlayer.AmOwner || HoldsDrive)
            Flash(CustomColorManager.Drunkard);
    }

    public void UnConfuse() => ConfusedPlayer = null;

    public void ConfuseClick(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConfusedPlayer = player;
        else
            ConfuseButton.StartCooldown(cooldown);
    }

    public void HitConfuse()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConfuseButton, RebActionsRPC.Confuse);
            ConfuseButton.Begin();
        }
        else if (!ConfusedPlayer)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConfuseButton, RebActionsRPC.Confuse, ConfusedPlayer);
            ConfuseButton.Begin();
        }
    }

    public string DrunkLabel() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    public bool ConfuseEnd() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public bool DrunkUsable() => IsDrunk;

    // Timekeeper Stuff
    public DateTime LastTimed { get; set; }
    public CustomButton TimeButton { get; set; }
    public bool IsTK => FormerRole is Timekeeper;

    public void ControlStart() => Flash(Color, Timekeeper.TimeDur);

    public void Control()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => x.GetRole().Rewinding = true);
    }

    public void UnControl() => AllPlayers().ForEach(x => x.GetRole().Rewinding = false);

    public void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    public string TKLabel() => HoldsDrive ? "REWIND" : "FREEZE";

    public bool TKUsable() => IsTK;

    // Silencer Stuff
    public CustomButton SilenceButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; set; }
    public PlayerControl SilencedPlayer
    {
        get => Target;
        set => Target = value;
    }
    public bool IsSil => FormerRole is Silencer;

    public bool SilenceException(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !Silencer.SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !Silencer.SilenceMates);

    public void Silence(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            SilencedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RebActionsRPC.Silence, SilencedPlayer);
        }

        SilenceButton.StartCooldown(cooldown);
    }

    public bool SilUsable() => IsSil;
}