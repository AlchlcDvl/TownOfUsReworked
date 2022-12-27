using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Cannibal))
            {
                var cann = (Cannibal)role;
                cann.LastEaten = DateTime.UtcNow;
                cann.LastEaten = cann.LastEaten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
            }
        }
    }
}