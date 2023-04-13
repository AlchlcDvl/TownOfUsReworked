using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformHaunt
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jester))
                return true;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (__instance == role.HauntButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.HauntTimer() != 0f)
                    return false;

                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer, DeathReasonEnum.Killed, false);
                role.HasHaunted = true;
                role.MaxUses--;
                role.LastHaunted = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}