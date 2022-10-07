using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    internal class Start
    {
        private static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
            if (glitch != null)
            {
                ((Glitch)glitch).LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns + CustomGameOptions.MimicCooldown * -1);
                ((Glitch)glitch).LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns + CustomGameOptions.HackCooldown * -1);
                ((Glitch)glitch).LastKill = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns + CustomGameOptions.GlitchKillCooldown * -1);
            }
        }
    }
}