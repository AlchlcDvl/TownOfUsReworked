using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInterrogate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff))
                return false;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (__instance == role.InterrogateButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.InterrogateTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
            
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                    role.Interrogated.Add(role.ClosestPlayer.PlayerId);
                
                if (interact[0] == true)
                    role.LastInterrogated = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
