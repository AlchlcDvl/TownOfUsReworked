using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Patches;
using Hazel;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                return true;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (__instance != role.WarpButton)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!__instance.enabled)
                return false;

            if (__instance.isCoolingDown)
                return false;

            if (!__instance.isActiveAndEnabled)
                return false;

            if (role.WarpTimer() != 0)
                return false;

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Warp,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            role.Warp();
            role.LastWarped = DateTime.UtcNow;
            return false;
        }
    }
}