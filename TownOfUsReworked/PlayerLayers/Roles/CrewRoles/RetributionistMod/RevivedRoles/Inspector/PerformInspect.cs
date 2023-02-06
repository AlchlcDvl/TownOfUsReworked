using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.RevivedRoles.Inspector
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInspect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return false;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole?.RoleType != RoleEnum.Inspector)
                return false;

            if (__instance == role.InspectButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.InspectTimer() > 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    role.Inspected.Add(role.ClosestPlayer);
            
                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                }
                
                if (interact[0] == true)
                    role.LastInspected = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}
