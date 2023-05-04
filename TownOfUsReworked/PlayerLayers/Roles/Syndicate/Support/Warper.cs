using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using System.Collections;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using Hazel;
using Reactor.Utilities;
using System.Linq;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Warper : SyndicateRole
    {
        public CustomButton WarpButton;
        public DateTime LastWarped;
        public PlayerControl WarpPlayer1;
        public PlayerControl WarpPlayer2;
        public Dictionary<byte, DateTime> UnwarpablePlayers = new();
        public CustomMenu WarpMenu1;
        public CustomMenu WarpMenu2;

        public Warper(PlayerControl player) : base(player)
        {
            Name = "Warper";
            StartText = "Warp The Crew Away From Each Other";
            AbilitiesText = $"- You can warp all players, forcing them to be teleported to random locations\n- With the Chaos Drive, more locations are opened to you\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
            RoleType = RoleEnum.Warper;
            AlignmentName = SSu;
            WarpPlayer1 = null;
            WarpPlayer2 = null;
            UnwarpablePlayers = new();
            WarpMenu1 = new(Player, Click1);
            WarpMenu2 = new(Player, Click2);
            Type = LayerEnum.Warper;
            WarpButton = new(this, "Warp", AbilityTypes.Effect, "Secondary", Warp);
            InspectorResults = InspectorResults.MovesAround;
        }

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
            var WasInVent = false;
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

            if (PlayerControl.LocalPlayer == WarpPlayer1)
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

        public void Click1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                WarpPlayer1 = player;
            else if (interact[0])
                LastWarped = DateTime.UtcNow;
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
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
                writer.Write((byte)ActionsRPC.Warp);
                writer.Write(Player.PlayerId);
                writer.Write(WarpPlayer1.PlayerId);
                writer.Write(WarpPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(WarpPlayers());
                LastWarped = DateTime.UtcNow;
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag1 = WarpPlayer1 == null && !HoldsDrive;
            var flag2 = WarpPlayer2 == null && !HoldsDrive;
            WarpButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "WARP"), WarpTimer(), CustomGameOptions.WarpCooldown);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!HoldsDrive)
                {
                    if (WarpPlayer2 != null)
                        WarpPlayer2 = null;
                    else if (WarpPlayer1 != null)
                        WarpPlayer1 = null;
                }

                Utils.LogSomething("Removed a target");
            }

            foreach (var entry in UnwarpablePlayers)
            {
                var player = Utils.PlayerById(entry.Key);

                if (player?.Data.IsDead == true || player.Data.Disconnected)
                    continue;

                if (UnwarpablePlayers.ContainsKey(player.PlayerId) && player.moveable && UnwarpablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                    UnwarpablePlayers.Remove(player.PlayerId);
            }
        }
    }
}