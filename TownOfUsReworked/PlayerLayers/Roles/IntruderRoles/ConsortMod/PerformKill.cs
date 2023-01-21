using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(ActionButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Consort))
                return false;

            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(role.Player, RoleEnum.Consort, role.ClosestPlayer, __instance))
                return false;

            if ((role.RoleblockTimer() != 0f && __instance == role.BlockButton) || (role.KillTimer() != 0f && __instance == role.KillButton))
                return false;

            Utils.Spread(role.Player, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller)))
            {
                Utils.AlertKill(role.Player, role.ClosestPlayer);

                if (CustomGameOptions.ShieldBreaks)
                {
                    if (__instance == role.KillButton)
                        role.LastKill = DateTime.UtcNow;
                    else if (__instance == role.BlockButton)
                        role.LastBlock = DateTime.UtcNow;
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Alert Done");
                return false;
            }
            
            if (__instance == role.BlockButton)
                role.PerformBlock();
            else if (__instance == role.KillButton)
                role.PerformKill();

            return false;
        }
    }
}