using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.CorruptedMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var role in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
            {
                var corrupted = (Corrupted)role;
                corrupted.LastKilled = DateTime.UtcNow;
                corrupted.LastKilled = corrupted.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CorruptedKillCooldown);

                var role2 = Role.GetRole(corrupted.Player);

                if (corrupted.Player.Is(Faction.Crew))
                    role2.Faction = Faction.Neutral;
            }
        }
    }
}