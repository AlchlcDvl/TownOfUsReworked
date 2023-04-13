using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch]
    public static class UnwarpableTracker
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class UnwarpableUpdate
        {
            public static void Postfix()
            {
                if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Warper))
                    return;

                var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

                foreach (var entry in role.UnwarpablePlayers)
                {
                    var player = Utils.PlayerById(entry.Key);

                    if (player == null || player.Data?.IsDead != false || player.Data.Disconnected)
                        continue;

                    if (role.UnwarpablePlayers.ContainsKey(player.PlayerId) && player.moveable && role.UnwarpablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                        role.UnwarpablePlayers.Remove(player.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static class SaveLadderPlayer
        {
            public static void Prefix(PlayerPhysics __instance)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                    Role.GetRole<Warper>(PlayerControl.LocalPlayer).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            }
        }

        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
        public static class SavePlatformPlayer
        {
            public static void Prefix()
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                    Role.GetRole<Warper>(PlayerControl.LocalPlayer).UnwarpablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetUnwarpable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}