using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith) role;
                wraith.LastInvis = DateTime.UtcNow;
                wraith.LastInvis = wraith.LastInvis.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
            }
        }
    }
}