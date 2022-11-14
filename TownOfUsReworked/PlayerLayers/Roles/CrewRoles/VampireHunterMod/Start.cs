using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Vigilante))
            {
                var vampireHunter = (VampireHunter) role;
                vampireHunter.LastStaked = DateTime.UtcNow;
                vampireHunter.LastStaked = vampireHunter.LastStaked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
            }
        }
    }
}