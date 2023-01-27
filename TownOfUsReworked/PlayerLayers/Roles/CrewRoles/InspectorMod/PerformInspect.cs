using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInspect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Inspector, true))
                return false;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.InspectTimer() != 0f && __instance == role.InspectButton)
                return false;

            if (__instance == role.InspectButton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);

                if (interact[3] == true && interact[0] == true)
                {
                    role.Inspected.Add(role.ClosestPlayer);
                    role.LastInspected = DateTime.UtcNow;
            
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                    return false;
                }
                else if (interact[1] == true)
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}
