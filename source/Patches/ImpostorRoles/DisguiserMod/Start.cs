using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.DisguiserMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var disguiser = (Disguiser) role;
                disguiser.LastDisguised = DateTime.UtcNow;
                disguiser.LastDisguised = disguiser.LastDisguised.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
            }
        }
    }
}