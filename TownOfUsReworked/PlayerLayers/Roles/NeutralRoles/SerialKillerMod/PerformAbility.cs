using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.SerialKiller))
                return false;

            var role = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            if (__instance == role.BloodlustButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.LustTimer() != 0)
                    return false;

                role.TimeRemaining = CustomGameOptions.BloodlustDuration;
                role.Bloodlust();
                return false;
            }
            else if (__instance == role.StabButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;
                
                if (!role.Lusted)
                    return false;
                
                if (role.KillTimer() != 0f)
                    return false;
                
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

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
