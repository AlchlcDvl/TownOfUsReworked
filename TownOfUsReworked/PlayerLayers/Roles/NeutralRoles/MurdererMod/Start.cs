using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.MurdererMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Murderer))
            {
                var murd = (Murderer)role;
                murd.LastKill = DateTime.UtcNow;
                murd.LastKill = murd.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
            }
        }
    }
}