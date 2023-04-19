using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Objects;
using System.Collections;
using TownOfUsReworked.Custom;
using System.Linq;
using Reactor.Utilities;

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
            AlignmentName = SSu;
            Framed = new();
            WarpPlayer1 = null;
            WarpPlayer2 = null;
            UnwarpablePlayers = new();
            WarpMenu1 = new(Player, WarpClick1);
            WarpMenu2 = new(Player, WarpClick2);
            ConcealMenu = new(Player, ConcealClick);
            PoisonMenu = new(Player, PoisonClick);
            ShapeshiftMenu1 = new(Player, ShapeshiftClick1);
            ShapeshiftMenu2 = new(Player, ShapeshiftClick2);
            ConcealedPlayer = null;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
            PoisonedPlayer = null;
            Type = LayerEnum.PromotedRebel;
            ConcealButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", HitConceal);
            ConfuseButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", Drunk);
            FrameButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", HitFrame);
            RadialFrameButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", RadialFrame);
            ShapeshiftButton = new(this, AssetManager.Shapeshift, AbilityTypes.Effect, "Secondary", HitShapeshift);
            BombButton = new(this, AssetManager.Plant, AbilityTypes.Effect, "Secondary", Place);
            DetonateButton = new(this, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary", Detonate);
            CrusadeButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", HitCrusade);
            PoisonButton = new(this, AssetManager.Poison, AbilityTypes.Direct, "ActionSecondary", HitPoison);
            GlobalPoisonButton = new(this, AssetManager.Poison, AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison);
            WarpButton = new(this, AssetManager.Warp, AbilityTypes.Effect, "Secondary", Warp);
        }

        //Rebel Stuff
        public Role FormerRole;
        public PlayerControl ClosestTarget;
        public bool Enabled;
        public float TimeRemaining;
        public bool OnEffect => TimeRemaining > 0f;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = ConcealedPlayer == null && !HoldsDrive;
            var flag1 = ConfusedPlayer == null && !HoldsDrive;
            var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
            var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != PoisonedPlayer).ToList();
            var flag2 = PoisonedPlayer == null && HoldsDrive;
            var flag3 = WarpPlayer1 == null && !HoldsDrive;
            var flag4 = WarpPlayer2 == null && !HoldsDrive;
            var flag5 = ShapeshiftPlayer1 == null && !HoldsDrive;
            var flag6 = ShapeshiftPlayer2 == null && !HoldsDrive;
            ShapeshiftButton.Update(flag5 ? "FIRST TARGET" : (flag6 ? "SECOND TARGET": "SHAPESHIFT"), ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, OnEffect, TimeRemaining,
                CustomGameOptions.ShapeshiftDuration, true, IsSS);
            WarpButton.Update(flag3 ? "FIRST TARGET" : (flag4 ? "SECOND TARGET" : "WARP"), WarpTimer(), CustomGameOptions.WarpCooldown, true, IsWarp);
            GlobalPoisonButton.Update(flag2 ? "SET TARGET" : "POISON", PoisonTimer(), CustomGameOptions.PoisonCd, OnEffect, TimeRemaining, CustomGameOptions.PoisonDuration, true, IsPois);
            PoisonButton.Update("POISON", PoisonTimer(), CustomGameOptions.PoisonCd, notSyn, OnEffect, TimeRemaining, CustomGameOptions.PoisonDuration, true, IsPois);
            FrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, notFramed, true, !HoldsDrive && IsFram);
            RadialFrameButton.Update("FRAME", FrameTimer(), CustomGameOptions.FrameCooldown, true, HoldsDrive && IsFram);
            ConfuseButton.Update(flag1 ? "SET TARGET" : "CONFUSE", DrunkTimer(), CustomGameOptions.ConfuseCooldown, OnEffect, TimeRemaining, CustomGameOptions.ConfuseDuration, true,
                IsDrunk);
            ConcealButton.Update(flag ? "SET TARGET" : "CONCEAL", ConcealTimer(), CustomGameOptions.ConcealCooldown, OnEffect, TimeRemaining, CustomGameOptions.ConcealDuration, true,
                IsConc);
            BombButton.Update("PLACE", BombTimer(), CustomGameOptions.BombCooldown, true, IsBomb);
            DetonateButton.Update("DETONATE", DetonateTimer(), CustomGameOptions.DetonateCooldown, true, Bombs.Count > 0 && IsBomb);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!HoldsDrive && !OnEffect)
                {
                    if (ShapeshiftPlayer2 != null)
                        ShapeshiftPlayer2 = null;
                    else if (ShapeshiftPlayer1 != null)
                        ShapeshiftPlayer1 = null;
                    else if (ConcealedPlayer != null)
                        ConcealedPlayer = null;
                    else if (ConfusedPlayer != null)
                        ConfusedPlayer = null;
                }
                else if (HoldsDrive && !OnEffect)
                {
                    if (PoisonedPlayer != null)
                        PoisonedPlayer = null;
                }

                Utils.LogSomething("Removed a target");
            }
        }

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
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConcealDuration;
                Conceal();
                Utils.Conceal();
            }
            else if (ConcealedPlayer == null)
                ConcealMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != Player && x != ConcealedPlayer).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Conceal);
                writer.Write(Player.PlayerId);
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
            writer.Write(Player.PlayerId);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void HitFrame()
        {
            if (FrameTimer() != 0f || Utils.IsTooFar(Player, ClosestTarget) || HoldsDrive)
                return;

            var interact = Utils.Interact(Player, ClosestTarget);

            if (interact[3])
                Frame(ClosestTarget);

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

            if (MeetingHud.Instance || Player.Data.IsDead || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected)
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
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
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

            if (interact[3])
                PoisonedPlayer = player;
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[1])
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void HitPoison()
        {
            if (PoisonTimer() != 0f || OnEffect || HoldsDrive || Utils.IsTooFar(Player, ClosestPlayer))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Poison);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                PoisonedPlayer = ClosestPlayer;
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
                PoisonMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != PoisonedPlayer).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Poison);
                writer.Write(Player.PlayerId);
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
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
                Utils.Shapeshift();
            }
            else if (ShapeshiftPlayer1 == null)
                ShapeshiftMenu1.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == ShapeshiftPlayer2 || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null))).ToList());
            else if (ShapeshiftPlayer2 == null)
                ShapeshiftMenu2.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == ShapeshiftPlayer1 || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null))).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Shapeshift);
                writer.Write(Player.PlayerId);
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

            Bombs.Add(BombExtensions.CreateBomb(Player.GetTruePosition(), HoldsDrive));
            LastPlaced = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastDetonated = DateTime.UtcNow;
        }

        public void Detonate()
        {
            if (DetonateTimer() != 0f || Bombs.Count == 0)
                return;

            Bombs.DetonateBombs();
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

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == WarpPlayer1.PlayerId)
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

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == WarpPlayer2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                WarpPlayer1.MyPhysics.ResetMoveState();
                WarpPlayer1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == WarpPlayer1.PlayerId)
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
                Utils.Flash(Colors.Warper, "You were warped to an unknown location!");

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
            {
                WarpMenu1.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == WarpPlayer2 || (x == Player && !CustomGameOptions.WarpSelf) || (x.Data.IsDead &&
                    Utils.BodyById(x.PlayerId) == null) || UnwarpablePlayers.ContainsKey(x.PlayerId))).ToList());
            }
            else if (WarpPlayer2 == null)
            {
                WarpMenu2.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == WarpPlayer1 || (x == Player && !CustomGameOptions.WarpSelf) || (x.Data.IsDead &&
                    Utils.BodyById(x.PlayerId) == null) || UnwarpablePlayers.ContainsKey(x.PlayerId))).ToList());
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Warp);
                writer.Write(Player.PlayerId);
                writer.Write(WarpPlayer1.PlayerId);
                writer.Write(WarpPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(WarpPlayers());
                LastWarped = DateTime.UtcNow;
            }
        }

        //Drunkard Stuff
        public CustomButton ConfuseButton;
        public DateTime LastConfused;
        public CustomMenu ConfuseMenu;
        public PlayerControl ConfusedPlayer;
        public bool IsDrunk => FormerRole?.RoleType == RoleEnum.Drunkard;

        public float DrunkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConfused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.FreezeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Confuse()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unconfuse()
        {
            Enabled = false;
            LastConfused = DateTime.UtcNow;

            if (HoldsDrive)
                Reverse.UnconfuseAll();
            else
                Reverse.UnconfuseSingle(ConfusedPlayer);
        }

        public void ConfuseClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConfusedPlayer = player;
            else if (interact[0])
                LastConfused = DateTime.UtcNow;
            else if (interact[1])
                LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Drunk()
        {
            if (DrunkTimer() != 0f || OnEffect)
                return;

            if (HoldsDrive)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Confuse);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                Reverse.ConfuseAll();
            }
            else if (ConfusedPlayer == null)
                ConfuseMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != ConfusedPlayer).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Confuse);
                writer.Write(Player.PlayerId);
                writer.Write(ConfusedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                Reverse.ConfuseSingle(ConfusedPlayer);
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

            if (Player.Data.IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || MeetingHud.Instance)
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
            if (CrusadeTimer() != 0f || Utils.IsTooFar(Player, ClosestTarget))
                return;

            var interact = Utils.Interact(Player, ClosestTarget);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RebelAction);
                writer.Write((byte)RebelActionsRPC.Crusade);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.CrusadeDuration;
                CrusadedPlayer = ClosestTarget;
                Crusade();
            }
            else if (interact[0])
                LastCrusaded = DateTime.UtcNow;
            else if (interact[1])
                LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Politician Stuff
        public List<byte> ExtraVotes = new();
        public int VoteBank;
        public bool VotedOnce;
        public bool CanVote => VoteBank > 0;
        public bool IsPol => FormerRole?.RoleType == RoleEnum.Politician;
    }
}