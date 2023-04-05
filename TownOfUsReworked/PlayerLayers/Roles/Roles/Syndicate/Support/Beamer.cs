using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Hazel;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Beamer : SyndicateRole
    {
        public DateTime LastBeamed;
        public PlayerControl BeamPlayer1;
        public PlayerControl BeamPlayer2;
        public Dictionary<byte, DateTime> UnbeamablePlayers = new();
        public AbilityButton BeamButton;
        public AbilityButton SetBeamButton1;
        public AbilityButton SetBeamButton2;
        public ShapeshifterMinigame BeamMenu1;
        public ShapeshifterMinigame BeamMenu2;

        public Beamer(PlayerControl player) : base(player)
        {
            Name = "Beamer";
            StartText = "Send A Player To Another";
            AbilitiesText = $"- You can pick a player to be beamed to another player of your choice\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Beamer : Colors.Syndicate;
            RoleType = RoleEnum.Beamer;
            BeamPlayer1 = null;
            BeamPlayer2 = null;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = SSu;
            UnbeamablePlayers = new();
            BeamMenu1 = null;
            BeamMenu2 = null;
        }

        public float BeamTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBeamed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.BeamCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator BeamPlayers()
        {
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;

            if (BeamPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(BeamPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (BeamPlayer1.inVent && PlayerControl.LocalPlayer.PlayerId == BeamPlayer1.PlayerId)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                BeamPlayer1.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                BeamPlayer1.MyPhysics.ResetMoveState();
                BeamPlayer1.NetTransform.SnapTo(new Vector2(BeamPlayer2.GetTruePosition().x, BeamPlayer2.GetTruePosition().y + 0.3636f));
                BeamPlayer1.MyRend().flipX = BeamPlayer2.MyRend().flipX;

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == BeamPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(BeamPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = BeamPlayer2.GetTruePosition();

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == BeamPlayer2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(BeamPlayer2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                BeamPlayer1.MyPhysics.ResetMoveState();
                BeamPlayer1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == BeamPlayer1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(BeamPlayer1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = Player2Body.TruePosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == BeamPlayer1.PlayerId || PlayerControl.LocalPlayer.PlayerId == BeamPlayer2.PlayerId)
            {
                Utils.Flash(Colors.Beamer, "You were beamed to an unknown location!");

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            BeamPlayer1.moveable = true;
            BeamPlayer1.Collider.enabled = true;
            BeamPlayer1.NetTransform.enabled = true;
            BeamPlayer2.MyPhysics.ResetMoveState();
            BeamPlayer1 = null;
            BeamPlayer2 = null;
        }

        public void OpenMenu1()
        {
            if (BeamMenu1 == null)
            {
                if (Camera.main == null)
                    return;

                BeamMenu1 = Object.Instantiate(LayerExtentions.GetShapeshifterMenu(), Camera.main.transform, false);
            }

            BeamMenu1.transform.SetParent(Camera.main.transform, false);
            BeamMenu1.transform.localPosition = new Vector3(0f, 0f, -50f);
            BeamMenu1.Begin(null);
            Player.moveable = false;
            Player.NetTransform.Halt();
        }

        public void OpenMenu2()
        {
            if (BeamMenu2 == null)
            {
                if (Camera.main == null)
                    return;

                BeamMenu2 = Object.Instantiate(LayerExtentions.GetShapeshifterMenu(), Camera.main.transform, false);
            }

            BeamMenu2.transform.SetParent(Camera.main.transform, false);
            BeamMenu2.transform.localPosition = new Vector3(0f, 0f, -50f);
            BeamMenu2.Begin(null);
            Player.moveable = false;
            Player.NetTransform.Halt();
        }

        public void PanelClick(PlayerControl player, bool menu1)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)(menu1 ? ActionsRPC.SetBeam1 : ActionsRPC.SetBeam2));
            writer.Write(Player.PlayerId);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.Interact(Player, player);
            Utils.LogSomething($"Beaming {player.name}");

            if (BeamPlayer1 == null)
            {
                BeamPlayer1 = player;
                Utils.LogSomething($"1 - {BeamPlayer1.name}");
            }
            else if (BeamPlayer2 == null)
            {
                BeamPlayer2 = player;
                Utils.LogSomething($"2 - {BeamPlayer2.name}");
            }

            Player.moveable = true;
        }
    }
}