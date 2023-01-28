using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer, true))
                return false;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (__instance == role.FrameButton)
            {
                if (role.FrameTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    role.Framed.Add(role.ClosestPlayer.PlayerId);
                    role.LastFramed = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                    role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}