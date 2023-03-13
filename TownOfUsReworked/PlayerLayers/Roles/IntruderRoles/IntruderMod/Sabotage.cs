using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal class Sabotage
    {
        private static bool Prefix(MapBehaviour __instance)
        {
            return PlayerControl.LocalPlayer.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage;
        }
    }
}