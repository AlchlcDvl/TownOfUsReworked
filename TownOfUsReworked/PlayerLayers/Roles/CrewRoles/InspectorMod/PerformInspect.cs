using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformInspect
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Inspector))
                return true;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (__instance == role.InspectButton)
            {
                if (!Utils.ButtonUsable(role.InspectButton))
                    return false;

                if (role.InspectTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.Inspected.Contains(role.ClosestPlayer.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                    role.Inspected.Add(role.ClosestPlayer.PlayerId);
                
                if (interact[0] == true)
                    role.LastInspected = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return true;
        }
    }
}
