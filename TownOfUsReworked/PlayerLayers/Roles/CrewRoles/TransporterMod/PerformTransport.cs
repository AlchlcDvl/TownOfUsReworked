using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using Hazel;
using Reactor.Utilities;
using System;
using System.Linq;
using TownOfUsReworked.CustomOptions;

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

            if (__instance == role.TransportButton)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                var list = PlayerControl.AllPlayerControls.ToArray().Where(x => !((x == role.Player && !CustomGameOptions.TransSelf) || role.UntransportablePlayers.ContainsKey(x.PlayerId)
                    || (Utils.BodyById(x.PlayerId) == null && x.Data.IsDead) || x == role.TransportPlayer1 || x == role.TransportPlayer2)).ToList();

                if (role.TransportPlayer1 == null)
                    role.TransportMenu1.Open(list);
                else if (role.TransportPlayer2 == null)
                    role.TransportMenu2.Open(list);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Transport);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.TransportPlayer1.PlayerId);
                    writer.Write(role.TransportPlayer2.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Coroutines.Start(role.TransportPlayers());
                    role.LastTransported = DateTime.UtcNow;
                    role.UsesLeft--;
                }

                return false;
            }

            return true;
        }
    }
}