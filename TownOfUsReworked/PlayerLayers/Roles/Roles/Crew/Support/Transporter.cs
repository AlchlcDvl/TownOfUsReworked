using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Object = UnityEngine.Object;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Transporter : CrewRole
    {
        public DateTime LastTransported;
        public PlayerControl TransportPlayer1;
        public PlayerControl TransportPlayer2;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public Dictionary<byte, DateTime> UntransportablePlayers = new();
        public AbilityButton TransportButton;
        public AbilityButton SetTransportButton1;
        public AbilityButton SetTransportButton2;
        public ShapeshifterMinigame TransportMenu1;
        public ShapeshifterMinigame TransportMenu2;

        public Transporter(PlayerControl player) : base(player)
        {
            Name = "Transporter";
            StartText = "Swap Locations Of Players For Maximun Confusion";
            AbilitiesText = "- You can swap the locations of 2 players of your choice\n- Transporting someone in a vent will make the other player teleport on top of that vent or into" +
                " the vent based on whether the target can vent or not";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Transporter : Colors.Crew;
            RoleType = RoleEnum.Transporter;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            UsesLeft = CustomGameOptions.TransportMaxUses;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.LikesToExplore;
            UntransportablePlayers = new();
            TransportMenu1 = null;
            TransportMenu2 = null;
        }

        public float TransportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTransported;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TransportCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator TransportPlayers()
        {
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            bool WasInVent1 = false;
            bool WasInVent2 = false;
            Vent Vent1 = null;
            Vent Vent2 = null;

            if (TransportPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(TransportPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (TransportPlayer2.Data.IsDead)
            {
                Player2Body = Utils.BodyById(TransportPlayer2.PlayerId);

                if (Player2Body == null)
                    yield break;
            }

            if (TransportPlayer1.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer1.MyPhysics.ExitAllVents();
                Vent1 = CustomButtons.GetClosestVent(TransportPlayer1);
                WasInVent1 = true;
            }

            if (TransportPlayer2.inVent)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer2.MyPhysics.ExitAllVents();
                Vent2 = CustomButtons.GetClosestVent(TransportPlayer2);
                WasInVent2 = true;
            }

            if (Player1Body == null && Player2Body == null)
            {
                TransportPlayer1.MyPhysics.ResetMoveState();
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                var TempFacing = TransportPlayer1.MyRend().flipX;
                TransportPlayer1.NetTransform.SnapTo(new Vector2(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
                TransportPlayer1.MyRend().flipX = TransportPlayer2.MyRend().flipX;
                TransportPlayer2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                TransportPlayer2.MyRend().flipX = TempFacing;

                if (SubmergedCompatibility.IsSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TransportPlayer1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                    if (PlayerControl.LocalPlayer.PlayerId == TransportPlayer2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }

                if (TransportPlayer1.CanVent(TransportPlayer1.Data) && Vent2 != null && WasInVent2)
                    TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

                if (TransportPlayer2.CanVent(TransportPlayer1.Data) && Vent1 != null && WasInVent1)
                    TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TransportPlayer2.GetTruePosition();
                TransportPlayer2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TransportPlayer2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                Utils.StopDragging(Player2Body.ParentId);
                TransportPlayer1.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TransportPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Utils.StopDragging(Player2Body.ParentId);
                (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
            }

            if (PlayerControl.LocalPlayer == TransportPlayer1 || PlayerControl.LocalPlayer == TransportPlayer2)
            {
                Utils.Flash(Colors.Transporter, "You were transported to a different location!");

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TransportPlayer1.moveable = true;
            TransportPlayer2.moveable = true;
            TransportPlayer1.Collider.enabled = true;
            TransportPlayer2.Collider.enabled = true;
            TransportPlayer1.NetTransform.enabled = true;
            TransportPlayer2.NetTransform.enabled = true;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
        }

        public void OpenMenu1()
        {
            if (TransportMenu1 == null)
            {
                if (Camera.main == null)
                    return;

                TransportMenu1 = Object.Instantiate(LayerExtentions.GetShapeshifterMenu(), Camera.main.transform, false);
            }

            TransportMenu1.transform.SetParent(Camera.main.transform, false);
            TransportMenu1.transform.localPosition = new Vector3(0f, 0f, -50f);
            TransportMenu1.Begin(null);
            Player.moveable = false;
            Player.NetTransform.Halt();
        }

        public void OpenMenu2()
        {
            if (TransportMenu2 == null)
            {
                if (Camera.main == null)
                    return;

                TransportMenu2 = Object.Instantiate(LayerExtentions.GetShapeshifterMenu(), Camera.main.transform, false);
            }

            TransportMenu2.transform.SetParent(Camera.main.transform, false);
            TransportMenu2.transform.localPosition = new Vector3(0f, 0f, -50f);
            TransportMenu2.Begin(null);
            Player.moveable = false;
            Player.NetTransform.Halt();
        }

        public void PanelClick(PlayerControl player, bool menu1)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)(menu1 ? ActionsRPC.SetTransport1 : ActionsRPC.SetTransport2));
            writer.Write(Player.PlayerId);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.Interact(Player, player);
            Utils.LogSomething($"Transporting {player.name}");

            if (TransportPlayer1 == null)
            {
                TransportPlayer1 = player;
                Utils.LogSomething($"1 - {TransportPlayer1.name}");
            }
            else if (TransportPlayer2 == null)
            {
                TransportPlayer2 = player;
                Utils.LogSomething($"2 - {TransportPlayer2.name}");
            }

            Player.moveable = true;
        }
    }
}