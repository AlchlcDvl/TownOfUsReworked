using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformObliterate
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Pestilence))
                return true;

            var role = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);

            if (__instance == role.ObliterateButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3] == true || interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}