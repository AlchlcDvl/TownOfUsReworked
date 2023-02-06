using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return false;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (__instance == role.KillButton || __instance == role.HackButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                if (__instance == role.HackButton)
                {
                    if (role.HackTimer() > 0f)
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller), false, false, Role.GetRoleValue(RoleEnum.Pestilence));

                    if (interact[3] == true && interact[0] == true)
                        role.RPCSetHacked(role.ClosestPlayer);

                    if (interact[3] == true)
                        role.LastHack = DateTime.UtcNow;
                    else if (interact[1] == true)
                        role.LastHack.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else if (__instance == role.KillButton)
                {
                    if (role.KillTimer() > 0f)
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
                }
            }
            else if (__instance == role.MimicButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                role.MimicList = null;
                role.OpenMimicList();
                return false;
            }

            return false;
        }
    }
}