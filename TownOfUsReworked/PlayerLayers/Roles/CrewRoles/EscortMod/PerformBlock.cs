using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformBlock
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Escort, true))
                return false;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.RoleblockTimer() != 0f && __instance == role.BlockButton)
                return false;

            if (__instance == role.BlockButton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller));

                if (interact[3] == true && interact[0] == true)
                {
                    role.RPCSetBlocked(role.ClosestPlayer);
                    return false;
                }
                else if (interact[1] == true)
                    role.LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}
