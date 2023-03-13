using System;
using UnityEngine;
using Reactor.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Transporter : CrewRole
    {
        public DateTime LastTransported;
        public bool PressedButton;
        public bool MenuClick;
        public bool LastMouse;
        public ChatController TransportList;
        public PlayerControl TransportPlayer1;
        public PlayerControl TransportPlayer2;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public Dictionary<byte, DateTime> UntransportablePlayers;
        public AbilityButton TransportButton;
        
        public Transporter(PlayerControl player) : base(player)
        {
            Name = "Transporter";
            StartText = "Swap Locations Of Players For Maximun Confusion";
            AbilitiesText = "- You can swap the locations of 2 alive players of your choice.\n- Transporting someone in a vent will make the other player teleport on top of that vent." +
                $"\n- You have {UsesLeft} transports remaining.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Transporter : Colors.Crew;
            LastTransported = DateTime.UtcNow;
            RoleType = RoleEnum.Transporter;
            PressedButton = false;
            MenuClick = false;
            LastMouse = false;
            TransportList = null;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            UsesLeft = CustomGameOptions.TransportMaxUses;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            RoleDescription = "Your are a Transporter! You are an inventor who's created a machine that forcully warps the locations of 2 players! Use this to to whisk away" +
                "high priority Crew!";
            InspectorResults = InspectorResults.LikesToExplore;
            UntransportablePlayers = new Dictionary<byte, DateTime>();
        }

        public float TransportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTransported;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.TransportCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public static IEnumerator TransportPlayers(byte player1, byte player2, bool die)
        {
            var TP1 = Utils.PlayerById(player1);
            var TP2 = Utils.PlayerById(player2);
            var deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;

            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies)
                {
                    if (body.ParentId == TP1.PlayerId)
                        Player1Body = body;
                }
            }

            if (TP2.Data.IsDead)
            {
                foreach (var body in deadBodies)
                {
                    if (body.ParentId == TP2.PlayerId)
                        Player2Body = body;
                }
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                    yield return null;

                TP1.MyPhysics.ExitAllVents();
            }

            if (TP2.inVent && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                    yield return null;

                TP2.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                var TempFacing = TP1.myRend().flipX;
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                TP1.myRend().flipX = TP2.myRend().flipX;

                if (die)
                    Utils.MurderPlayer(TP1, TP2, !TP1.Is(AbilityEnum.Ninja));
                else
                {
                    TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                    TP2.myRend().flipX = TempFacing;
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TP2.GetTruePosition();
                TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = Player2Body.TruePosition;
                Player2Body.transform.position = TempPosition;
            }

            if (PlayerControl.LocalPlayer == TP1 || PlayerControl.LocalPlayer == TP2)
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Transporter));

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }

            TP1.moveable = true;
            TP2.moveable = true;
            TP1.Collider.enabled = true;
            TP2.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.NetTransform.enabled = true;
        }

        public static void StopDragging(byte PlayerId)
        {
            var undertakers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Undertaker && ((Undertaker)x).CurrentlyDragging != null && ((Undertaker)x).CurrentlyDragging.ParentId ==
                PlayerId);

            foreach (var undertaker in undertakers)
                ((Undertaker)undertaker).CurrentlyDragging = null;
        }
    }
}