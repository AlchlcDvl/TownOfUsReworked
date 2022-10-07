using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Dracula))
            {
                var drac = (Dracula) role;
                drac.LastBitten = DateTime.UtcNow;
                drac.LastBitten = drac.LastBitten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }
        }
    }
}