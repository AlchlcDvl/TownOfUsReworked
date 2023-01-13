using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal class Sabotage
    {
        private static bool Prefix(MapBehaviour __instance)
        {
            return !PlayerControl.LocalPlayer.Is(Faction.Intruder) || (PlayerControl.LocalPlayer.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage);
        }
    }
}