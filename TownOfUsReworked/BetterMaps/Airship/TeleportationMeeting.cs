using Reactor.Utilities;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class TeleportationMeeting
    {
        public static bool TeleportationStarted = false;

        public static void Prefix(PlayerControl __instance)
        {   
            if (LobbyBehaviour.Instance || __instance == null || __instance.Data == null || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (__instance?.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                return;

            if (CustomGameOptions.AddTeleporters)
            {
                if (!TeleportationStarted && Vector2.Distance(__instance.transform.position, new Vector2(17.331f, 15.236f)) < 0.5f && UnityEngine.Object.FindObjectOfType<AirshipStatus>() != null)
                    Coroutines.Start(CoTeleportPlayer(__instance));
            }
        }

        private static IEnumerator Fade(bool fadeAway, bool enableAfterFade)
        {
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;

            if (fadeAway)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0, 0, 0, i);
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0, 0, 0, i);
                    yield return null;
                }
            }

            if (enableAfterFade)
                HudManager.Instance.FullScreen.enabled = false;
        }

        private static IEnumerator CoTeleportPlayer(PlayerControl instance)
        {
            TeleportationStarted = true;
            yield return Fade(false, false);
            instance.NetTransform.RpcSnapTo(new Vector2(5.753f, -10.011f));
            yield return new WaitForSeconds(0.3f);
            yield return Fade(true, true);
            TeleportationStarted = false;

            yield break;
        }
    }
}
