using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAssault
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Juggernaut))
                return true;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            if (__instance == role.AssaultButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true, false, role.JuggKills >= 4);

                if (interact[3])
                    role.JuggKills++;

                if (role.JuggKills == 4)
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));

                if (interact[0])
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1])
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}