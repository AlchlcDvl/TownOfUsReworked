using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformInvestigate
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Consigliere))
                return true;

            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (__instance == role.InvestigateButton)
            {
                if (role.ConsigliereTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                    role.Investigated.Add(role.ClosestTarget.PlayerId);

                if (interact[0])
                    role.LastInvestigated = DateTime.UtcNow;
                else if (interact[1])
                    role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}