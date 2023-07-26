namespace TownOfUsReworked.PlayerLayers.Roles
{
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
        public Role FormerRole;
        public bool Enabled;
        public float TimeRemaining;
        public bool OnEffect => TimeRemaining > 0f;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
        public override string Name => "Rebel";
        public override LayerEnum Type => LayerEnum.PromotedRebel;
        public override RoleEnum RoleType => RoleEnum.PromotedRebel;
        public override Func<string> StartText => () => "Lead The <color=#008000FF>Syndicate</color>";
        public override Func<string> AbilitiesText => () => "- You have succeeded the former <color=#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities\n" +
            (FormerRole == null ? "" : $"\n{FormerRole.AbilitiesText()}");
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
            ConfuseButton.Update(flag7 ? "SET TARGET" : "CONFUSE", ConfuseTimer(), CustomGameOptions.ConfuseCooldown, OnEffect, TimeRemaining, CustomGameOptions.ConfuseDuration, true,
                IsDrunk);
            StalkButton.Update("STALK", StalkTimer(), CustomGameOptions.StalkCd, true, !HoldsDrive && IsStalk);
            SpellButton.Update("SPELL", SpellTimer(), CustomGameOptions.SpellCooldown, SpellCount * CustomGameOptions.SpellCooldownIncrease, true, IsSpell);
            PositiveButton.Update("SET POSITIVE", PositiveTimer(), CustomGameOptions.CollideCooldown, true, IsCol);
            NegativeButton.Update("SET NEGATIVE", NegativeTimer(), CustomGameOptions.CollideCooldown, true, IsCol);
            ShapeshiftButton.Update(flag5 ? "FIRST TARGET" : (flag6 ? "SECOND TARGET": "SHAPESHIFT"), ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, OnEffect, TimeRemaining,
                CustomGameOptions.ShapeshiftDuration, true, IsSS);
            WarpButton.Update(flag3 ? "FIRST TARGET" : (flag4 ? "SECOND TARGET" : "WARP"), WarpTimer(), CustomGameOptions.WarpCooldown, true, IsWarp);
            PoisonButton.Update("POISON", PoisonTimer(), CustomGameOptions.PoisonCd, OnEffect, TimeRemaining, CustomGameOptions.PoisonDuration, true, !HoldsDrive && IsPois);
            GlobalPoisonButton.Update(flag1 ? "SET TARGET" : "POISON", PoisonTimer(), CustomGameOptions.PoisonCd, OnEffect, TimeRemaining, CustomGameOptions.PoisonDuration, true,
                HoldsDrive && IsPois);
            FrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, true, !HoldsDrive && IsFram);
            RadialFrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, true, HoldsDrive && IsFram);
            ConcealButton.Update(flag ? "SET TARGET" : "CONCEAL", ConcealTimer(), CustomGameOptions.ConcealCooldown, OnEffect, TimeRemaining, CustomGameOptions.ConcealDuration, true,
                IsConc);
            BombButton.Update("PLACE", BombTimer(), CustomGameOptions.BombCooldown, true, IsBomb);
            DetonateButton.Update("DETONATE", DetonateTimer(), CustomGameOptions.DetonateCooldown, true, Bombs?.Count > 0 && IsBomb);
            TimeButton.Update(HoldsDrive ? "REWIND" : "FREEZE", TimeTimer(), CustomGameOptions.TimeControlCooldown, OnEffect, TimeRemaining, CustomGameOptions.TimeControlDuration, true,
                IsTK);
            SilenceButton.Update("SILENCE", SilenceTimer(), CustomGameOptions.SilenceCooldown, true, IsSil);
            ChargeButton.Update("CHARGE", ChargeTimer(), CustomGameOptions.ChargeCooldown, OnEffect, TimeRemaining, CustomGameOptions.ChargeDuration, true, HoldsDrive && IsCol);

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

                LogSomething("Removed a target");
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
                    if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(RoleEnum.Pestilence) || Negative.IsOnAlert() ||
                        Negative.IsRetShielded() || Negative.IsVesting()))
                    {
                        RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
                    }

                    if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(RoleEnum.Pestilence) || Positive.IsOnAlert() ||
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
                    if (!(Negative.IsShielded() || Negative.IsProtected() || Negative.IsProtectedMonarch() || Negative.Is(RoleEnum.Pestilence) || Negative.IsOnAlert() ||
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
                    if (!(Positive.IsShielded() || Positive.IsProtected() || Positive.IsProtectedMonarch() || Positive.Is(RoleEnum.Pestilence) || Positive.IsOnAlert() ||
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
        public bool IsAnarch => FormerRole?.RoleType == RoleEnum.Anarchist;

        //Concealer Stuff
        public CustomButton ConcealButton;
        public DateTime LastConcealed;
        public PlayerControl ConcealedPlayer;
        public CustomMenu ConcealMenu;
        public bool IsConc => FormerRole?.RoleType == RoleEnum.Concealer;

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

        public float ConcealTimer()
        {
            var timespan = DateTime.UtcNow - LastConcealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConcealCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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
            if (ConcealTimer() != 0f || OnEffect)
                return;

            if (HoldsDrive)
            {
                TimeRemaining = CustomGameOptions.ConcealDuration;
                Conceal();
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Conceal, this);
            }
            else if (ConcealedPlayer == null)
                ConcealMenu.Open();
            else
            {
                TimeRemaining = CustomGameOptions.ConcealDuration;
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Conceal, this, ConcealedPlayer);
                Conceal();
            }
        }

        //Framer Stuff
        public CustomButton FrameButton;
        public List<byte> Framed = new();
        public DateTime LastFramed;
        public CustomButton RadialFrameButton;
        public bool IsFram => FormerRole?.RoleType == RoleEnum.Framer;

        public float FrameTimer()
        {
            var timespan = DateTime.UtcNow - LastFramed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FrameCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void RpcFrame(PlayerControl player)
        {
            if ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || Framed.Contains(player.PlayerId))
                return;

            Framed.Add(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Frame, this, player);
        }

        public void Frame()
        {
            if (FrameTimer() != 0f || IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
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
            if (FrameTimer() != 0f || !HoldsDrive)
                return;

            foreach (var player in GetClosestPlayers(CustomPlayer.Local.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                RpcFrame(player);

            LastFramed = DateTime.UtcNow;
        }

        //Poisoner Stuff
        public CustomButton PoisonButton;
        public CustomButton GlobalPoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public CustomMenu PoisonMenu;
        public bool IsPois => FormerRole?.RoleType == RoleEnum.Poisoner;

        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || IsDead || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void PoisonKill()
        {
            if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(RoleEnum.Pestilence)))
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
            if (PoisonTimer() != 0f || OnEffect || HoldsDrive || IsTooFar(Player, PoisonButton.TargetPlayer))
                return;

            var interact = Interact(Player, PoisonButton.TargetPlayer);

            if (interact[3] && !PoisonButton.TargetPlayer.IsProtected() && !PoisonButton.TargetPlayer.IsVesting() && !PoisonButton.TargetPlayer.IsProtectedMonarch())
            {
                PoisonedPlayer = PoisonButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Poison, this, PoisonedPlayer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
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
            if (PoisonTimer() != 0f || OnEffect || !HoldsDrive)
                return;

            if (PoisonedPlayer == null)
                PoisonMenu.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Poison, this, PoisonedPlayer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
        }

        public float PoisonTimer()
        {
            var timespan = DateTime.UtcNow - LastPoisoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PoisonCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        //Shapeshifter Stuff
        public CustomButton ShapeshiftButton;
        public DateTime LastShapeshifted;
        public PlayerControl ShapeshiftPlayer1;
        public PlayerControl ShapeshiftPlayer2;
        public CustomMenu ShapeshiftMenu1;
        public CustomMenu ShapeshiftMenu2;
        public bool IsSS => FormerRole?.RoleType == RoleEnum.Shapeshifter;

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

        public float ShapeshiftTimer()
        {
            var timespan = DateTime.UtcNow - LastShapeshifted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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
            if (ShapeshiftTimer() != 0f)
                return;

            if (HoldsDrive)
            {
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
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
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
            }
        }

        //Bomber Stuff
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public CustomButton BombButton;
        public CustomButton DetonateButton;
        public List<Bomb> Bombs;
        public bool IsBomb => FormerRole?.RoleType == RoleEnum.Bomber;

        public float BombTimer()
        {
            var timespan = DateTime.UtcNow - LastPlaced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BombCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float DetonateTimer()
        {
            var timespan = DateTime.UtcNow - LastDetonated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DetonateCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Place()
        {
            if (BombTimer() != 0f)
                return;

            Bombs.Add(new Bomb(Player.GetTruePosition(), HoldsDrive, Player));
            LastPlaced = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastDetonated = DateTime.UtcNow;
        }

        public void Detonate()
        {
            if (DetonateTimer() != 0f || Bombs.Count == 0)
                return;

            Bomb.DetonateBombs(Bombs);
            LastDetonated = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastPlaced = DateTime.UtcNow;
        }

        //Warper Stuff
        public CustomButton WarpButton;
        public DateTime LastWarped;
        public PlayerControl WarpPlayer1;
        public PlayerControl WarpPlayer2;
        public CustomMenu WarpMenu1;
        public CustomMenu WarpMenu2;
        public Dictionary<byte, DateTime> UnwarpablePlayers = new();
        public SpriteRenderer AnimationPlaying;
        public GameObject WarpObj;
        public DeadBody Player1Body;
        public DeadBody Player2Body;
        public bool WasInVent;
        public Vent Vent;
        public bool IsWarp => FormerRole?.RoleType == RoleEnum.Warper;

        public float WarpTimer()
        {
            var timespan = DateTime.UtcNow - LastWarped;
            var num = Player.GetModifiedCooldown(CustomGameOptions.WarpCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public IEnumerator WarpPlayers()
        {
            Player1Body = null;
            Player2Body = null;
            WasInVent = false;
            Vent = null;
            TimeRemaining = CustomGameOptions.WarpDuration;

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
                Flash(Color, CustomGameOptions.WarpDuration);

            if (Player1Body == null && !WasInVent)
                AnimateWarp();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.WarpDuration)
                {
                    TimeRemaining -= Time.deltaTime;
                    yield return null;
                }
                else
                    break;

                if (Meeting)
                    yield break;
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

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
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

            HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.WarpDuration, new Action<float>(p =>
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
            if (WarpTimer() != 0f)
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
        public DateTime LastCrusaded;
        public PlayerControl CrusadedPlayer;
        public CustomButton CrusadeButton;
        public bool IsCrus => FormerRole?.RoleType == RoleEnum.Crusader;

        public float CrusadeTimer()
        {
            var timespan = DateTime.UtcNow - LastCrusaded;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CrusadeCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

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
            if (CrusadeTimer() != 0f || IsTooFar(Player, CrusadeButton.TargetPlayer))
                return;

            var interact = Interact(Player, CrusadeButton.TargetPlayer);

            if (interact[3])
            {
                CrusadedPlayer = CrusadeButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Crusade, this, CrusadedPlayer);
                TimeRemaining = CustomGameOptions.CrusadeDuration;
                Crusade();
            }
            else if (interact[0])
                LastCrusaded = DateTime.UtcNow;
            else if (interact[1])
                LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Collider Stuff
        public CustomButton PositiveButton;
        public CustomButton NegativeButton;
        public CustomButton ChargeButton;
        public DateTime LastPositive;
        public DateTime LastNegative;
        public DateTime LastCharged;
        public PlayerControl Positive;
        public PlayerControl Negative;
        private float Range => CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);
        public bool IsCol => FormerRole?.RoleType == RoleEnum.Collider;

        public float PositiveTimer()
        {
            var timespan = DateTime.UtcNow - LastPositive;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float NegativeTimer()
        {
            var timespan = DateTime.UtcNow - LastNegative;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float ChargeTimer()
        {
            var timespan = DateTime.UtcNow - LastCharged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ChargeCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void SetPositive()
        {
            if (HoldsDrive || IsTooFar(Player, PositiveButton.TargetPlayer) || PositiveTimer() != 0f)
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
            if (HoldsDrive || IsTooFar(Player, NegativeButton.TargetPlayer) || NegativeTimer() != 0f)
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
            if (!HoldsDrive || OnEffect || ChargeTimer() != 0f)
                return;

            TimeRemaining = CustomGameOptions.ChargeDuration;
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
        public CustomButton SpellButton;
        public List<byte> Spelled = new();
        public DateTime LastSpelled;
        public int SpellCount;
        public bool IsSpell => FormerRole?.RoleType == RoleEnum.Spellslinger;

        public float SpellTimer()
        {
            var timespan = DateTime.UtcNow - LastSpelled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SpellCooldown, SpellCount * CustomGameOptions.SpellCooldownIncrease) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

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
            if (SpellTimer() != 0f || IsTooFar(Player, SpellButton.TargetPlayer))
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
        public Dictionary<byte, CustomArrow> StalkerArrows = new();
        public DateTime LastStalked;
        public CustomButton StalkButton;
        public bool IsStalk => FormerRole?.RoleType == RoleEnum.Stalker;

        public float StalkTimer()
        {
            var timespan = DateTime.UtcNow - LastStalked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StalkCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
            StalkerArrows.Remove(targetPlayerId);
        }

        public void Stalk()
        {
            if (IsTooFar(Player, StalkButton.TargetPlayer) || StalkTimer() != 0f)
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
        public CustomButton ConfuseButton;
        public DateTime LastConfused;
        public float Modifier => OnEffect ? -1 : 1;
        public PlayerControl ConfusedPlayer;
        public CustomMenu ConfuseMenu;
        public bool IsDrunk => FormerRole?.RoleType == RoleEnum.Drunkard;

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

        public float ConfuseTimer()
        {
            var timespan = DateTime.UtcNow - LastConfused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConfuseCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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
            if (ConfuseTimer() != 0f || OnEffect)
                return;

            if (HoldsDrive)
            {
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Confuse, this);
            }
            else if (ConfusedPlayer == null)
                ConfuseMenu.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.Confuse, this, ConfusedPlayer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
            }
        }

        //Time Keeper Stuff
        public DateTime LastTimed;
        public CustomButton TimeButton;
        public bool IsTK => FormerRole?.RoleType == RoleEnum.TimeKeeper;

        public float TimeTimer()
        {
            var timespan = DateTime.UtcNow - LastTimed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TimeControlCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Control()
        {
            if (!Enabled)
                Flash(Color, CustomGameOptions.TimeControlDuration);

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (HoldsDrive)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                    GetRole(player).Rewinding = true;
            }

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnControl()
        {
            Enabled = false;
            LastTimed = DateTime.UtcNow;

            foreach (var player in CustomPlayer.AllPlayers)
                GetRole(player).Rewinding = false;
        }

        public void TimeControl()
        {
            TimeRemaining = CustomGameOptions.TimeControlDuration;
            Control();
            CallRpc(CustomRPC.Action, ActionsRPC.RebelAction, RebelActionsRPC.TimeControl, this);
        }

        //Silencer Stuff
        public CustomButton SilenceButton;
        public PlayerControl SilencedPlayer;
        public DateTime LastSilenced;
        public bool ShookAlready;
        public Sprite PrevOverlay;
        public Color PrevColor;
        public bool IsSil => FormerRole?.RoleType == RoleEnum.Silencer;

        public float SilenceTimer()
        {
            var timespan = DateTime.UtcNow - LastSilenced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SilenceCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Silence()
        {
            if (SilenceTimer() != 0f || IsTooFar(Player, SilenceButton.TargetPlayer) || SilenceButton.TargetPlayer == SilencedPlayer)
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
}