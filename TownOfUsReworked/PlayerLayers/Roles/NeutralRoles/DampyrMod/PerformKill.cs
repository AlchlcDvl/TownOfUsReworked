using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DampyrMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dampyr))
                return false;

            var role = Role.GetRole<Dampyr>(PlayerControl.LocalPlayer);

            if (__instance == role.KillButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true, false, Role.GetRoleValue(RoleEnum.VampireHunter));

                if (interact[3] == true && interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;

                    foreach (var drac in Role.GetRoles(RoleEnum.Dracula))
                    {
                        var dracula = (Dracula)drac;
                        dracula.LastBitten = DateTime.UtcNow;
                    }
                }
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