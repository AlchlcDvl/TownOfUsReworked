using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Inspector))
            {
                var inspector = (Inspector) role;
                inspector.LastExamined = DateTime.UtcNow;
                inspector.LastExamined = inspector.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            }
        }
    }
}