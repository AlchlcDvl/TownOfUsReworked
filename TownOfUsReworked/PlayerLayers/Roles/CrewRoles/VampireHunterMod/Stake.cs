using System;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Stake
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter))
                return false;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter, role.ClosestPlayer, __instance) || __instance != role.StakeButton)
                return false;

            if (role.StakeTimer() != 0f && __instance == role.StakeButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastStaked = DateTime.UtcNow;
                return false;
            }
            
            if (role.ClosestPlayer.Is(SubFaction.Undead))
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);

            role.LastStaked = DateTime.UtcNow;
            return false;
        }
    }
}
