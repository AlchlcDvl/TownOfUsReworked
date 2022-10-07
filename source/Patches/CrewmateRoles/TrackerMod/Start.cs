using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker) role;
                tracker.LastTracked = DateTime.UtcNow;
                tracker.LastTracked = tracker.LastTracked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }
        }
    }
}