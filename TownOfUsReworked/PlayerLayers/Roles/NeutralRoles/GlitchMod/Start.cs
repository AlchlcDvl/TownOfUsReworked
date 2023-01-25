using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch) role;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastHack = glitch.LastHack.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = DateTime.UtcNow;
                glitch.LastMimic = glitch.LastMimic.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastKill = glitch.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
            }
        }
    }
}