using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformInteract
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Troll))
                return false;

            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (__instance == role.InteractButton)
            {
                if (role.InteractTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

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