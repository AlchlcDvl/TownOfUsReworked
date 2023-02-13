using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RecordRewind
    {
        public static bool rewinding = false;
        public static Role whoIsRewinding;
        public static List<PointInTime> points = new List<PointInTime>();
        private static float deadTime;
        private static bool isDead;
        private static float recordTime => CustomGameOptions.RewindDuration;

        public static void Record()
        {
            if (points.Count > Mathf.Round(recordTime / Time.deltaTime))
                points.RemoveAt(points.Count - 1);

            if (PlayerControl.LocalPlayer == null)
                return;

            Vector3 position;
            Vector2 velocity;

            if (!PlayerControl.LocalPlayer.moveable && points.Count > 0)
            {
                position = points[0].position;
                velocity = Vector2.zero;
            }
            else
            {
                position = PlayerControl.LocalPlayer.transform.position;
                velocity = PlayerControl.LocalPlayer.gameObject.GetComponent<Rigidbody2D>().velocity;
            }

            points.Insert(0, new PointInTime(position, velocity, Time.time));

            if (PlayerControl.LocalPlayer.Data.IsDead && !isDead)
            {
                isDead = true;
                deadTime = TempData.LastDeathReason == DeathReason.Exile || PlayerControl.LocalPlayer.Is(RoleEnum.Altruist) ? 0 : Time.time;
            }
            else if (!PlayerControl.LocalPlayer.Data.IsDead && isDead)
            {
                isDead = false;
                deadTime = 0;
            }
        }

        public static void Rewind()
        {
            if (Minigame.Instance)
                Minigame.Instance.Close();

            if (points.Count > 2)
            {
                points.RemoveAt(0);
                points.RemoveAt(0);

                if (PlayerControl.LocalPlayer.Data.IsDead || (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord) && CustomGameOptions.TLImmunity) || PlayerControl.LocalPlayer.inMovingPlat ||
                    (PlayerControl.LocalPlayer.Is(RoleEnum.TimeMaster) && CustomGameOptions.TMImmunity))
                    return;

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
                
                if (!PlayerControl.LocalPlayer.inVent)
                {
                    if (!PlayerControl.LocalPlayer.Collider.enabled)
                    {
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        PlayerControl.LocalPlayer.Collider.enabled = true;
                        PlayerControl.LocalPlayer.NetTransform.enabled = true;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FixAnimation, SendOption.Reliable, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    var currentPoint = points[0];

                    PlayerControl.LocalPlayer.transform.position = currentPoint.position;

                    if (SubmergedCompatibility.isSubmerged())
                        SubmergedCompatibility.ChangeFloor(currentPoint.position.y > -7);

                    PlayerControl.LocalPlayer.gameObject.GetComponent<Rigidbody2D>().velocity = currentPoint.velocity * 3;

                    if (isDead && currentPoint.unix < deadTime && PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.RewindRevive)
                    {
                        var player = PlayerControl.LocalPlayer;
                        ReviveBody(player);
                        player.myTasks.RemoveAt(0);
                        deadTime = 0;
                        isDead = false;

                        var write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
                        write.Write((byte)ActionsRPC.RewindRevive);
                        write.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(write);
                    }
                }

                points.RemoveAt(0);
            }
            else
                StartStop.StopRewind(whoIsRewinding);
        }

        public static void ReviveBody(PlayerControl player)
        {
            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;

                if (poisonerRole.PoisonedPlayer == player)
                    poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }

            player.Revive();

            Murder.KilledPlayers.Remove(Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == player.PlayerId);

            if (body != null)
                Object.Destroy(body.gameObject);
        }
    }
}