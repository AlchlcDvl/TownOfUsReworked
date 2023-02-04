using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInteract
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Troll))
                return false;

            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (__instance == role.InteractButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.InteractTimer() > 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true || interact[0] == true)
                    role.LastInteracted = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastInteracted = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInteracted.AddSeconds(CustomGameOptions.ProtectKCReset);
                    
                return false;
            }

            return false;
        }
    }
}