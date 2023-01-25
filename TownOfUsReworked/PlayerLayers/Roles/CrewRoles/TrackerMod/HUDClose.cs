using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker)role;
                tracker.LastTracked = DateTime.UtcNow;
                
                if (CustomGameOptions.ResetOnNewRound)
                {
                    tracker.UsesLeft = CustomGameOptions.MaxTracks;
                    tracker.TrackerArrows.Values.DestroyAll();
                    tracker.TrackerArrows.Clear();
                }
            }
        }
    }
}