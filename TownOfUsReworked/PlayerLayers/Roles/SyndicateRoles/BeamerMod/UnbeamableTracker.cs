using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch]
    public class UnbeamableTracker
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public class UnbeamableUpdate
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                    return;

                var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

                foreach (var entry in role.UnbeamablePlayers)
                {
                    var player = Utils.PlayerById(entry.Key);
                    
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                        continue;

                    if (role.UnbeamablePlayers.ContainsKey(player.PlayerId) && player.moveable == true && role.UnbeamablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) <
                        DateTime.UtcNow)
                        role.UnbeamablePlayers.Remove(player.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public class SaveLadderPlayer
        {
            public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] Ladder source, [HarmonyArgument(1)] byte climbLadderSid)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer))
                    Role.GetRole<Beamer>(PlayerControl.LocalPlayer).UnbeamablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            }
        }

        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
        public class SavePlatformPlayer
        {
            public static void Prefix(MovingPlatformBehaviour __instance)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer))
                    Role.GetRole<Beamer>(PlayerControl.LocalPlayer).UnbeamablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetUnbeamable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}