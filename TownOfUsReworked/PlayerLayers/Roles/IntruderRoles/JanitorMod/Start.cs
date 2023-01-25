using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Janitor))
            {
                var role2 = (Janitor) role;
                role2.LastCleaned = DateTime.UtcNow;
                role2.LastCleaned = role2.LastCleaned.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JanitorCleanCd);
            }
        }
    }
}