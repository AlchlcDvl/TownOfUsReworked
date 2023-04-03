using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using Hazel;
using Reactor.Utilities;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformTransport
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return true;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (__instance == role.SetTransportButton1)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                role.OpenMenu1();
                return false;
            }
            else if (__instance == role.SetTransportButton2)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                role.OpenMenu2();
                return false;
            }
            else if (__instance == role.TransportButton)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Transport);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(role.TransportPlayers());
                role.LastTransported = DateTime.UtcNow;
                role.UsesLeft--;
                return false;
            }

            return true;
        }
    }
}