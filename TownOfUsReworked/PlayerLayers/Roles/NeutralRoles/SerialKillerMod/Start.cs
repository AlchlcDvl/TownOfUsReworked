using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller) role;
                sk.LastLusted = DateTime.UtcNow;
                sk.LastKilled = DateTime.UtcNow;
                sk.LastLusted = sk.LastLusted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                sk.LastKilled = sk.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
            }
        }
    }
}