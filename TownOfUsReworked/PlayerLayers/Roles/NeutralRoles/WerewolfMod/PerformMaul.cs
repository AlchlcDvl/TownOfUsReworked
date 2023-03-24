using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformMaul
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Werewolf))
                return true;

            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);

            if (__instance == role.MaulButton)
            {
                if (role.MaulTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);
                role.Maul(role.Player);

                if (interact[0])
                    role.LastMauled = DateTime.UtcNow;
                else if (interact[1])
                    role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastMauled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}