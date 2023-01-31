using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAssault
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Juggernaut, true))
                return false;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.KillTimer() != 0f && __instance == role.AssaultButton)
                return false;

            if (__instance == role.AssaultButton)
            {
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                {
                    role.LastKilled = DateTime.UtcNow;
                    role.JuggKills += 1;
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