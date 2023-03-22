using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities;
using System.Linq;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Beamer : SyndicateRole
    {
        public DateTime LastBeamed;
        public bool PressedButton;
        public bool MenuClick;
        public bool LastMouse;
        public ChatController BeamList;
        public PlayerControl BeamPlayer1;
        public PlayerControl BeamPlayer2;
        public Dictionary<byte, DateTime> UnbeamablePlayers = new Dictionary<byte, DateTime>();
        public AbilityButton BeamButton;

        public Beamer(PlayerControl player) : base(player)
        {
            Name = "Beamer";
            StartText = "Send A Player To Another";
            AbilitiesText = "- You can pick a player to be beamed to another player of your choice.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Beamer : Colors.Syndicate;
            LastBeamed = DateTime.UtcNow;
            RoleType = RoleEnum.Beamer;
            PressedButton = false;
            MenuClick = false;
            LastMouse = false;
            BeamList = null;
            BeamPlayer1 = null;
            BeamPlayer2 = null;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = SSu;
        }

        public float BeamTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBeamed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BeamCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public static IEnumerator BeamPlayers(byte player1, byte player2, bool die)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer) || PlayerControl.LocalPlayer == Utils.PlayerById(player1) || PlayerControl.LocalPlayer == Utils.PlayerById(player2))
            {
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                } catch {}
            }

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

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                    yield return null;

                TP1.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                var TempFacing = TP1.myRend().flipX;
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                TP1.myRend().flipX = TP2.myRend().flipX;

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = TP2.GetTruePosition();

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
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

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
                Player1Body.transform.position = Player2Body.TruePosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId || PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Beamer));

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TP1.moveable = true;
            TP1.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.MyPhysics.ResetMoveState();
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