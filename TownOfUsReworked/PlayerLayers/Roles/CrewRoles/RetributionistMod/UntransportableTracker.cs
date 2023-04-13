using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UntransportableUpdate
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            foreach (var entry in role.UntransportablePlayers)
            {
                var player = Utils.PlayerById(entry.Key);

                if (player == null || player.Data?.IsDead != false || player.Data.Disconnected)
                    continue;

                if (role.UntransportablePlayers.ContainsKey(player.PlayerId) && player.moveable && role.UntransportablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) <
                    DateTime.UtcNow)
                {
                    role.UntransportablePlayers.Remove(player.PlayerId);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static class SaveLadderPlayer
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
        }
    }

    [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
    public static class SavePlatformPlayer
    {
        public static void Prefix()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.SetUntransportable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}