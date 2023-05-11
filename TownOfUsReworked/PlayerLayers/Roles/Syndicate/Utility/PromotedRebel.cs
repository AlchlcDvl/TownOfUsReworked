using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Objects;
using System.Collections;
using TownOfUsReworked.Custom;
using System.Linq;
using Reactor.Utilities;
using TownOfUsReworked.Cosmetics;
using Reactor.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class PromotedRebel : SyndicateRole
    {
        public PromotedRebel(PlayerControl player) : base(player)
        {
            Name = "Rebel";
            RoleType = RoleEnum.PromotedRebel;
            StartText = "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
            AbilitiesText = "- You have succeeded the former <color=#FFFCCEFF>Rebel</color> and have a shorter cooldown on your former role's abilities";
            Color = CustomGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            Type = LayerEnum.PromotedRebel;
            AlignmentName = SSu;
            SpellCount = 0;
            Framed = new();
            UnwarpablePlayers = new();
            StalkerArrows = new();
            Spelled = new();
            ConcealedPlayer = null;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
            PoisonedPlayer = null;
            Positive = null;
            Negative = null;
            WarpPlayer1 = null;
            WarpPlayer2 = null;
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
            FrameButton = new(this, "Frame", AbilityTypes.Direct, "Secondary", HitFrame, Exception7);
            RadialFrameButton = new(this, "Frame", AbilityTypes.Effect, "Secondary", RadialFrame);
            ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Effect, "Secondary", HitShapeshift);
            BombButton = new(this, "Plant", AbilityTypes.Effect, "Secondary", Place);
            DetonateButton = new(this, "Detonate", AbilityTypes.Effect, "Tertiary", Detonate);
            CrusadeButton = new(this, "Crusade", AbilityTypes.Direct, "Secondary", HitCrusade);
            PoisonButton = new(this, "Poison", AbilityTypes.Direct, "ActionSecondary", HitPoison, Exception4);
            GlobalPoisonButton = new(this, "Poison", AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison);
            WarpButton = new(this, "Warp", AbilityTypes.Effect, "Secondary", Warp);
            InspectorResults = InspectorResults.LeadsTheGroup;
        }

        //Rebel Stuff
        public Role FormerRole;
        public bool Enabled;
        public float TimeRemaining;
        public bool OnEffect => TimeRemaining > 0f;

        public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates);

        public bool Exception2(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && Utils.BodyByPlayer(player) == null) || (player.Is(Faction)
            && !CustomGameOptions.ShapeshiftMates);

        public bool Exception3(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && Utils.BodyByPlayer(player) == null) || (player.Is(Faction)
            && !CustomGameOptions.ShapeshiftMates);

        public bool Exception4(PlayerControl player) => player == PoisonedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public bool Exception5(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead);

        public bool Exception6(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead);

        public bool Exception7(PlayerControl player) => Framed.Contains(player.PlayerId) || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public bool Exception8(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

        public bool Exception9(PlayerControl player) => Spelled.Contains(player.PlayerId) || player.Is(Faction);

        public bool Exception10(PlayerControl player) => player == Negative || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public bool Exception11(PlayerControl player) => player == Positive || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

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
            StalkButton.Update("STALK", StalkTimer(), CustomGameOptions.StalkCd, true, !HoldsDrive && IsStalk);
            SpellButton.Update("SPELL", SpellTimer(), CustomGameOptions.SpellCooldown + (SpellCount * CustomGameOptions.SpellCooldownIncrease), true, IsSpell);
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
            DetonateButton.Update("DETONATE", DetonateTimer(), CustomGameOptions.DetonateCooldown, true, Bombs.Count > 0 && IsBomb);

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
                    }
                    else if (PoisonedPlayer != null)
                        PoisonedPlayer = null;
                }

                Utils.LogSomething("Removed a target");
            }

            var range = CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);

            if (Utils.GetDistBetweenPlayers(Positive, Negative) <= range)
            {
                Utils.RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
                Utils.RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
            }

            if (IsDead)
                OnLobby();
            else
            {
                foreach (var arrow in StalkerArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);

                    #pragma warning disable
                    if (player == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        DestroyArrow(arrow.Key);
                        continue;
                    }
                    #pragma warning restore

                    if (DoUndo.IsCamoed && !HoldsDrive)
                        arrow.Value.image.color = Palette.PlayerColors[6];
                    else if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Rainbow;
                    else if (ColorUtils.IsChroma(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Chroma;
                    else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Monochrome;
                    else if (ColorUtils.IsMantle(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Mantle;
                    else if (ColorUtils.IsFire(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Fire;
                    else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Galaxy;
                    else
                        arrow.Value.image.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];

                    arrow.Value.target = player.transform.position;
                }

                if (HoldsDrive)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (!StalkerArrows.ContainsKey(player.PlayerId))
                        {
                            var gameObj = new GameObject("StalkArrow") { layer = 5 };
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = Player.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = AssetManager.GetSprite("Arrow");

                            if (DoUndo.IsCamoed && !HoldsDrive)
                                renderer.color = Palette.PlayerColors[6];
                            else if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Rainbow;
                            else if (ColorUtils.IsChroma(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Chroma;
                            else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Monochrome;
                            else if (ColorUtils.IsMantle(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Mantle;
                            else if (ColorUtils.IsFire(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Fire;
                            else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId))
                                renderer.color = ColorUtils.Galaxy;
                            else
                                renderer.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];

                            arrow.image = renderer;
                            arrow.target = player.transform.position;
                            StalkerArrows.Add(player.PlayerId, arrow);
                        }
                    }
                }
            }
        }

        public override void OnLobby()
        {
            base.OnLobby();

            Bomb.Clear(Bombs);
            Bombs.Clear();

            StalkerArrows.Values.DestroyAll();
            StalkerArrows.Clear();

            ClearPoints();
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            Positive = null;
            Negative = null;

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                Bomb.DetonateBombs(Bombs);
        }

        public override void UpdateMap(MapBehaviour __instance)
        {
            base.UpdateMap(__instance);

            if (IsDead || MeetingHud.Instance)
                return;

            foreach (var pair in StalkerArrows)
            {
                var player = Utils.PlayerById(pair.Key);

                if (!player.Data.IsDead)
                {
                    var v = pair.Value.target;
                    v /= ShipStatus.Instance.MapScale;
                    v.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                    v.z = -1f;

                    if (Points.ContainsKey(player.PlayerId))
                        Points[player.PlayerId].transform.localPosition = v;
                    else
                    {
                        var point = Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
                        point.transform.localPosition = v;
                        point.enabled = true;
                        player.SetPlayerMaterialColors(point);
                        Points.Add(player.PlayerId, point);
                    }
                }
            }
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

            if (MeetingHud.Instance || (ConcealedPlayer == null && !HoldsDrive))
                TimeRemaining = 0f;
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                Utils.DefaultOutfitAll();
            else
                Utils.DefaultOutfit(ConcealedPlayer);
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConcealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConcealCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void ConcealClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Conceal);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConcealDuration;
                Conceal();
                Utils.Conceal();
            }
            else if (ConcealedPlayer == null)
                ConcealMenu.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Conceal);
                writer.Write(PlayerId);
                writer.Write(ConcealedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConcealDuration;
                Conceal();
                Utils.Invis(ConcealedPlayer, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
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
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFramed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FrameCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Frame(PlayerControl player)
        {
            if (player.Is(Faction) || Framed.Contains(player.PlayerId))
                return;

            Framed.Add(player.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.RebelAction);
            writer.Write((byte)RebelActionsRPC.Frame);
            writer.Write(PlayerId);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HitFrame()
        {
            if (FrameTimer() != 0f || Utils.IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
                return;

            var interact = Utils.Interact(Player, FrameButton.TargetPlayer);

            if (interact[3])
                Frame(FrameButton.TargetPlayer);

            if (interact[0])
                LastFramed = DateTime.UtcNow;
            else if (interact[1])
                LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void RadialFrame()
        {
            if (FrameTimer() != 0f || !HoldsDrive)
                return;

            foreach (var player in Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                Frame(player);

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

            if (MeetingHud.Instance || IsDead || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void PoisonKill()
        {
            if (!PoisonedPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

                if (!PoisonedPlayer.Data.IsDead)
                {
                    try
                    {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false);
                    } catch {}
                }
            }

            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }

        public void PoisonClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

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
            if (PoisonTimer() != 0f || OnEffect || HoldsDrive || Utils.IsTooFar(Player, PoisonButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, PoisonButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Poison);
                writer.Write(PlayerId);
                writer.Write(PoisonButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                PoisonedPlayer = PoisonButton.TargetPlayer;
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[1])
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Poison);
                writer.Write(PlayerId);
                writer.Write(PoisonedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
        }

        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPoisoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PoisonCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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
                Utils.Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
                Utils.Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
            }

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            Enabled = false;
            LastShapeshifted = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                Utils.DefaultOutfitAll();
            else
            {
                Utils.DefaultOutfit(ShapeshiftPlayer1);
                Utils.DefaultOutfit(ShapeshiftPlayer2);
            }
        }

        public float ShapeshiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastShapeshifted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void ShapeshiftClick1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer1 = player;
            else if (interact[0])
                LastShapeshifted = DateTime.UtcNow;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void ShapeshiftClick2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Shapeshift);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
                Utils.Shapeshift();
            }
            else if (ShapeshiftPlayer1 == null)
                ShapeshiftMenu1.Open();
            else if (ShapeshiftPlayer2 == null)
                ShapeshiftMenu2.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Shapeshift);
                writer.Write(PlayerId);
                writer.Write(ShapeshiftPlayer1.PlayerId);
                writer.Write(ShapeshiftPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
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
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPlaced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BombCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DetonateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDetonated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DetonateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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
        public bool IsWarp => FormerRole?.RoleType == RoleEnum.Warper;

        public float WarpTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastWarped;
            var num = Player.GetModifiedCooldown(CustomGameOptions.WarpCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator WarpPlayers()
        {
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            bool WasInVent = false;
            Vent Vent = null;

            if (WarpPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(WarpPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (WarpPlayer1.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                WarpPlayer1.MyPhysics.ExitAllVents();
            }

            if (WarpPlayer2.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                Vent = WarpPlayer2.GetClosestVent();
                WasInVent = true;
            }

            if (Player1Body == null && Player2Body == null)
            {
                WarpPlayer1.MyPhysics.ResetMoveState();
                WarpPlayer1.NetTransform.SnapTo(new Vector2(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));
                WarpPlayer1.MyRend().flipX = WarpPlayer2.MyRend().flipX;

                if (SubmergedCompatibility.IsSubmerged && PlayerControl.LocalPlayer.PlayerId == WarpPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }

                if (WarpPlayer1.CanVent() && Vent != null && WasInVent)
                    WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = WarpPlayer2.GetTruePosition();

                if (SubmergedCompatibility.IsSubmerged && PlayerControl.LocalPlayer.PlayerId == WarpPlayer2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                WarpPlayer1.MyPhysics.ResetMoveState();
                WarpPlayer1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged && PlayerControl.LocalPlayer.PlayerId == WarpPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = Player2Body.TruePosition;
            }

            if (PlayerControl.LocalPlayer == WarpPlayer1 || PlayerControl.LocalPlayer == WarpPlayer2)
            {
                Utils.Flash(Colors.Warper);

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
        }

        public void WarpClick1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                WarpPlayer1 = player;
            else if (interact[0])
                LastWarped = DateTime.UtcNow;
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void WarpClick2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

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
                Utils.Warp();
                LastWarped = DateTime.UtcNow;
            }
            else if (WarpPlayer1 == null)
                WarpMenu1.Open();
            else if (WarpPlayer2 == null)
                WarpMenu2.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Warp);
                writer.Write(PlayerId);
                writer.Write(WarpPlayer1.PlayerId);
                writer.Write(WarpPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(WarpPlayers());
                LastWarped = DateTime.UtcNow;
            }
        }

        //Crusader Stuff
        public DateTime LastCrusaded;
        public PlayerControl CrusadedPlayer;
        public CustomButton CrusadeButton;
        public bool IsCrus => FormerRole?.RoleType == RoleEnum.Crusader;

        public float CrusadeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCrusaded;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CrusadeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Crusade()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || MeetingHud.Instance)
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
            if (CrusadeTimer() != 0f || Utils.IsTooFar(Player, CrusadeButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, CrusadeButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Crusade);
                writer.Write(PlayerId);
                writer.Write(CrusadeButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.CrusadeDuration;
                CrusadedPlayer = CrusadeButton.TargetPlayer;
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
        public DateTime LastPositive;
        public DateTime LastNegative;
        public PlayerControl Positive;
        public PlayerControl Negative;
        public bool IsCol => FormerRole?.RoleType == RoleEnum.Collider;

        public float PositiveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPositive;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float NegativeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastNegative;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void SetPositive()
        {
            if (HoldsDrive || Utils.IsTooFar(Player, PositiveButton.TargetPlayer) || PositiveTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, PositiveButton.TargetPlayer);

            if (interact[3])
                Positive = PositiveButton.TargetPlayer;

            if (interact[0])
                LastPositive = DateTime.UtcNow;
            else if (interact[1])
                LastPositive.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void SetNegative()
        {
            if (HoldsDrive || Utils.IsTooFar(Player, NegativeButton.TargetPlayer) || NegativeTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, NegativeButton.TargetPlayer);

            if (interact[3])
                Negative = NegativeButton.TargetPlayer;

            if (interact[0])
                LastNegative = DateTime.UtcNow;
            else if (interact[1])
                LastNegative.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Spellslinger Stuff
        public CustomButton SpellButton;
        public List<byte> Spelled = new();
        public DateTime LastSpelled;
        public int SpellCount;
        public bool IsSpell => FormerRole?.RoleType == RoleEnum.Spellslinger;

        public float SpellTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSpelled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SpellCooldown, SpellCount * CustomGameOptions.SpellCooldownIncrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Spell(PlayerControl player)
        {
            if (player.Is(Faction.Syndicate) || Spelled.Contains(player.PlayerId))
                return;

            Spelled.Add(player.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.RebelAction);
            writer.Write((byte)RebelActionsRPC.Spell);
            writer.Write(PlayerId);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HitSpell()
        {
            if (SpellTimer() != 0f || Utils.IsTooFar(Player, SpellButton.TargetPlayer))
                return;

            if (HoldsDrive)
            {
                Spell(SpellButton.TargetPlayer);
                LastSpelled = DateTime.UtcNow;
            }
            else
            {
                var interact = Utils.Interact(Player, SpellButton.TargetPlayer);

                if (interact[3])
                {
                    Spell(SpellButton.TargetPlayer);
                    SpellCount++;
                }

                if (interact[0])
                    LastSpelled = DateTime.UtcNow;
                else if (interact[1])
                    LastSpelled.AddSeconds(CustomGameOptions.ProtectKCReset);
            }

            if (Spelled.Count >= PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Syndicate)))
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
            }
        }

        //Stalker Stuff
        public Dictionary<byte, ArrowBehaviour> StalkerArrows = new();
        public DateTime LastStalked;
        public CustomButton StalkButton;
        public bool IsStalk => FormerRole?.RoleType == RoleEnum.Stalker;

        public float StalkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStalked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StalkCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            arrow.Value.gameObject?.Destroy();
            StalkerArrows.Remove(arrow.Key);
        }

        public void Stalk()
        {
            if (Utils.IsTooFar(Player, StalkButton.TargetPlayer) || StalkTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StalkButton.TargetPlayer);

            if (interact[3])
            {
                var target = StalkButton.TargetPlayer;
                var gameObj = new GameObject("StalkArrow");
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");

                if (DoUndo.IsCamoed && !HoldsDrive)
                    renderer.color = Palette.PlayerColors[6];
                else if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Rainbow;
                else if (ColorUtils.IsChroma(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Chroma;
                else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Monochrome;
                else if (ColorUtils.IsMantle(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Mantle;
                else if (ColorUtils.IsFire(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Fire;
                else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Galaxy;
                else
                    renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];

                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = target.transform.position;
                StalkerArrows.Add(target.PlayerId, arrow);
            }

            if (interact[0])
                LastStalked = DateTime.UtcNow;
            else if (interact[1])
                LastStalked.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }
}