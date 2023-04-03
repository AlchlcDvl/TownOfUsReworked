using System;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformInterrogate
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff))
                return true;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (__instance == role.InterrogateButton)
            {
                if (role.InterrogateTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                    role.Interrogated.Add(role.ClosestPlayer.PlayerId);

                if (interact[0])
                    role.LastInterrogated = DateTime.UtcNow;
                else if (interact[1])
                    role.LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}