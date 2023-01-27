using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using System;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch]
    public class UntransportableTracker
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public class UntransportableUpdate
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1)
                    return;

                if (PlayerControl.LocalPlayer == null)
                    return;

                if (PlayerControl.LocalPlayer.Data == null)
                    return;

                if (PlayerControl.LocalPlayer.Data.IsDead)
                    return;

                if (!GameData.Instance)
                    return;

                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    return;

                var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

                foreach (var entry in role.UntransportablePlayers)
                {
                    var player = Utils.PlayerById(entry.Key);
                    
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                        continue;

                    if (role.UntransportablePlayers.ContainsKey(player.PlayerId) && player.moveable == true &&
                        role.UntransportablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                        role.UntransportablePlayers.Remove(player.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public class SaveLadderPlayer
        {
            public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] Ladder source, [HarmonyArgument(1)] byte climbLadderSid)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            }
        }

        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
        public class SavePlatformPlayer
        {
            public static void Prefix(MovingPlatformBehaviour __instance)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.SetUntransportable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}