using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consigliere))
                return false;

            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (__instance == role.InvestigateButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.ConsigliereTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                    role.Investigated.Add(role.ClosestPlayer.PlayerId);
                
                if (interact[0] == true)
                    role.LastInvestigated = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (!__instance.isActiveAndEnabled)
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

//WHY THE FUCK DOES CONSIG INSIST ON KILLING?!