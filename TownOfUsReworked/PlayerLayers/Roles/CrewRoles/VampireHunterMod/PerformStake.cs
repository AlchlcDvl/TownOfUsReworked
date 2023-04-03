using System;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformStake
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter))
                return true;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (__instance == role.StakeButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.StakeTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, role.ClosestPlayer.Is(SubFaction.Undead));

                if (interact[3] || interact[0])
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[1])
                    role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastStaked.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}