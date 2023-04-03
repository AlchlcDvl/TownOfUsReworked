using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    public static class Sabotage
    {
        public static bool Prefix() => PlayerControl.LocalPlayer.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage;
    }
}