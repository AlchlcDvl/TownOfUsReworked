using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformObliterate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Pestilence))
                return true;

            var role = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);

            if (__instance == role.ObliterateButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, null, true);

                if (interact[3] == true && interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return false;
        }
    }
}