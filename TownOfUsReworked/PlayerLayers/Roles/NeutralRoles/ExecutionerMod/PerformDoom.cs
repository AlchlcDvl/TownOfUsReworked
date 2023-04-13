using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformHaunt
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Executioner))
                return true;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (__instance == role.DoomButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.DoomTimer() != 0f)
                    return false;

                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer, DeathReasonEnum.Killed, false);
                role.HasDoomed = true;
                role.MaxUses--;
                role.LastDoomed = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}