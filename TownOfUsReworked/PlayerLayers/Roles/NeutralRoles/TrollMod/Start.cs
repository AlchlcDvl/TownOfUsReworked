using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                var troll = (Troll)role;
                troll.LastInteracted = DateTime.UtcNow;
                troll.LastInteracted = troll.LastInteracted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
            }
        }
    }
}