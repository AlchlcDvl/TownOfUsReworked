using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Objects;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PromotedRebelMod;
using System.Collections;
using TownOfUsReworked.Custom;

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
            WarpMenu1 = new CustomMenu(Player, new CustomMenu.Select(WarpClick1));
            WarpMenu2 = new CustomMenu(Player, new CustomMenu.Select(WarpClick2));
            ConcealMenu = new CustomMenu(Player, new CustomMenu.Select(ConcealClick));
            PoisonMenu = new CustomMenu(Player, new CustomMenu.Select(PoisonClick));
            ShapeshiftMenu1 = new CustomMenu(Player, new CustomMenu.Select(ShapeshiftClick1));
            ShapeshiftMenu2 = new CustomMenu(Player, new CustomMenu.Select(ShapeshiftClick2));
            ConcealedPlayer = null;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
        }

        //Rebel Stuff
        public Role FormerRole;
        public PlayerControl ClosestTarget;

        //Concealer Stuff
        public AbilityButton ConcealButton;
        public bool ConcealEnabled;
        public DateTime LastConcealed;
        public float ConcealTimeRemaining;
        public bool Concealed => ConcealTimeRemaining > 0f;
        public PlayerControl ConcealedPlayer;
        public CustomMenu ConcealMenu;

        public void Conceal()
        {
            ConcealEnabled = true;
            ConcealTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || (ConcealedPlayer == null && !SyndicateHasChaosDrive))
                ConcealTimeRemaining = 0f;
        }

        public void UnConceal()
        {
            ConcealEnabled = false;
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
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConcealCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.SidekickAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void ConcealClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConcealedPlayer = player;
            else if (interact[1])
                LastConcealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Framer Stuff
        public AbilityButton FrameButton;
        public List<byte> Framed = new();
        public DateTime LastFramed;

        public float FrameTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFramed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FrameCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.SidekickAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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

        //Poisoner Stuff
        public AbilityButton PoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public float PoisonTimeRemaining;
        public bool Poisoned => PoisonTimeRemaining > 0f;
        public bool PoisonEnabled;
        public CustomMenu PoisonMenu;

        public void Poison()
        {
            PoisonEnabled = true;
            PoisonTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected)
                PoisonTimeRemaining = 0f;
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
            PoisonEnabled = false;
            LastPoisoned = DateTime.UtcNow;
        }

        public void PoisonClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                PoisonedPlayer = player;
            else if (interact[1])
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPoisoned;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.PoisonCd, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.SidekickAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Shapeshifter Stuff
        public AbilityButton ShapeshiftButton;
        public bool ShapeshiftEnabled;
        public DateTime LastShapeshifted;
        public float ShapeshiftTimeRemaining;
        public bool Shapeshifted => ShapeshiftTimeRemaining > 0f;
        public PlayerControl ShapeshiftPlayer1;
        public PlayerControl ShapeshiftPlayer2;
        public CustomMenu ShapeshiftMenu1;
        public CustomMenu ShapeshiftMenu2;

        public void Shapeshift()
        {
            ShapeshiftEnabled = true;
            ShapeshiftTimeRemaining -= Time.deltaTime;

            if (!SyndicateHasChaosDrive)
            {
                Utils.Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
                Utils.Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
            }

            if (MeetingHud.Instance)
                ShapeshiftTimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            ShapeshiftEnabled = false;
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
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.SidekickAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void ShapeshiftClick1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer1 = player;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void ShapeshiftClick2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer2 = player;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Bomber Stuff
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public AbilityButton BombButton;
        public AbilityButton DetonateButton;
        public List<Bomb> Bombs;

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPlaced;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.BombCooldown, CustomButtons.GetUnderdogChange(Player), CustomGameOptions.SidekickAbilityCooldownDecrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DetonateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDetonated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DetonateCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        //Warper Stuff
        public AbilityButton WarpButton;
        public DateTime LastWarped;
        public PlayerControl WarpPlayer1;
        public PlayerControl WarpPlayer2;
        public CustomMenu WarpMenu1;
        public CustomMenu WarpMenu2;
        public Dictionary<byte, DateTime> UnwarpablePlayers = new();

        public float WarpTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastWarped;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.WarpCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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

                Vent = CustomButtons.GetClosestVent(WarpPlayer2);
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

                if (WarpPlayer1.CanVent(WarpPlayer1.Data) && Vent != null && WasInVent)
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
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void WarpClick2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                WarpPlayer2 = player;
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Drunkard Stuff
        public AbilityButton ConfuseButton;
        public bool ConfuseEnabled;
        public float ConfuseTimeRemaining;
        public DateTime LastConfused;
        public bool Confused => ConfuseTimeRemaining > 0f;
        public CustomMenu ConfuseMenu;
        public PlayerControl ConfusedPlayer;

        public float DrunkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConfused;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.FreezeCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Confuse()
        {
            ConfuseEnabled = true;
            ConfuseTimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                ConfuseTimeRemaining = 0f;
        }

        public void Unconfuse()
        {
            ConfuseEnabled = false;
            LastConfused = DateTime.UtcNow;
            Reverse.UnconfuseAll();
        }

        public void ConfuseClick(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConfusedPlayer = player;
            else if (interact[1])
                LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        //Crusader Stuff
        public bool CrusadeEnabled;
        public DateTime LastCrusaded;
        public float CrusadeTimeRemaining;
        public bool OnCrusade => CrusadeTimeRemaining > 0f;
        public PlayerControl CrusadedPlayer;
        public AbilityButton CrusadeButton;

        public float CrusadeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCrusaded;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AlertCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Crusade()
        {
            CrusadeEnabled = true;
            CrusadeTimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || MeetingHud.Instance)
                CrusadeTimeRemaining = 0f;
        }

        public void UnCrusade()
        {
            CrusadeEnabled = false;
            LastCrusaded = DateTime.UtcNow;
            CrusadedPlayer = null;
        }

        //Politician Stuff
        public List<byte> ExtraVotes = new();
        public int VoteBank;
        public bool VotedOnce;
        public bool CanVote => VoteBank > 0;
    }
}