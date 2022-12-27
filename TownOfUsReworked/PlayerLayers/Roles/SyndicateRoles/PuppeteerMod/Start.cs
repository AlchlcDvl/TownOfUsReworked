using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Puppeteer))
            {
                var puppeteer = (Puppeteer) role;
                puppeteer.lastPossess = DateTime.UtcNow;
                puppeteer.lastPossess = puppeteer.lastPossess.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PossessCooldown);
            }
        }
    }
}