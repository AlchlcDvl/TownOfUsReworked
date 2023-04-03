using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformWarp
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Warper))
                return true;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (__instance == role.WarpButton)
            {
                if (role.WarpTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Warp);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.LastWarped = DateTime.UtcNow;
                Warper.Warp();
                return false;
            }

            return true;
        }
    }
}