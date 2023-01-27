using System;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class PerformStake
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter, true))
                return false;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.StakeTimer() != 0f && __instance == role.StakeButton)
                return false;

            if (__instance == role.StakeButton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, null, role.ClosestPlayer.Is(SubFaction.Undead));

                if (interact[3] == true && interact[0] == true)
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
