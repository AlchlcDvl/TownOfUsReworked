using Reactor.Utilities;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class TeleportationMeeting
    {
        private static bool TeleportationStarted;

        public static void Prefix(PlayerControl __instance)
        {
            if (LobbyBehaviour.Instance || __instance == null || __instance.Data == null || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                return;

            if (CustomGameOptions.AddTeleporters && !TeleportationStarted && Vector2.Distance(__instance.transform.position, new Vector2(17.331f, 15.236f)) < 0.5f &&
                Object.FindObjectOfType<AirshipStatus>() != null)
            {
                Coroutines.Start(CoTeleportPlayer(__instance));
            }
        }

        private static IEnumerator CoTeleportPlayer(PlayerControl instance)
        {
            TeleportationStarted = true;
            Coroutines.Start(Utils.Fade(false, false));
            yield return new WaitForSeconds(0.25f);
            instance.NetTransform.RpcSnapTo(new Vector2(5.753f, -10.011f));
            yield return new WaitForSeconds(0.25f);
            Coroutines.Start(Utils.Fade(true, true));
            TeleportationStarted = false;
        }
    }
}