using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Camouflager))
                return false;

            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(role.Player, RoleEnum.Consort, role.ClosestPlayer, __instance) || (__instance != role.KillButton && __instance != role.CamouflageButton))
                return false;

            if (__instance == role.CamouflageButton)
            {
                if (role.CamouflageTimer() != 0)
                    return false;

                Utils.Spread(role.Player, role.ClosestPlayer);

                if (Utils.CheckInteractionSesitive(role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller)))
                {
                    Utils.AlertKill(role.Player, role.ClosestPlayer, __instance == role.KillButton);

                    if (CustomGameOptions.ShieldBreaks && __instance == role.KillButton)
                        role.LastKill = DateTime.UtcNow;
                        
                    return false;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Camouflage);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                role.Camouflage();
                return false;
            }

            return true;
        }
    }
}