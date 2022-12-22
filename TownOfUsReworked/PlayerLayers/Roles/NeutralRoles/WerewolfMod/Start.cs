using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                ww.LastMauled = DateTime.UtcNow;
                ww.LastMauled = ww.LastMauled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
            }
        }
    }
}