using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Camouflager))
            {
                var camouflager = (Camouflager)role;
                camouflager.LastCamouflaged = DateTime.UtcNow;
                camouflager.LastCamouflaged = camouflager.LastCamouflaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
            }
        }
    }
}