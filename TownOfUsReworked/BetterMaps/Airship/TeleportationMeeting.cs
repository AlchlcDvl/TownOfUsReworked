namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TeleportationMeeting
    {
        private static bool TeleportationStarted;

        public static void Prefix()
        {
            if (CustomPlayer.Local == null || CustomPlayer.LocalCustom.Data == null)
                return;

            if (CustomGameOptions.AddTeleporters && !TeleportationStarted && Vector2.Distance(CustomPlayer.LocalCustom.Position, new(17.331f, 15.236f)) < 0.5f &&
                UObject.FindObjectOfType<AirshipStatus>() != null)
            {
                Coroutines.Start(CoTeleportPlayer(CustomPlayer.Local));
            }
        }

        private static IEnumerator CoTeleportPlayer(PlayerControl instance)
        {
            TeleportationStarted = true;
            Coroutines.Start(Fade(false, false));
            yield return new WaitForSeconds(0.25f);
            instance.NetTransform.RpcSnapTo(new(5.753f, -10.011f));
            yield return new WaitForSeconds(0.25f);
            Coroutines.Start(Fade(true, true));
            TeleportationStarted = false;
        }
    }
}