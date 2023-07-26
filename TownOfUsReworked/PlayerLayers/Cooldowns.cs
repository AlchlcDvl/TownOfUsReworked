namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
    public static class HUDClose
    {
        public static void Postfix(UObject obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            ButtonUtils.ResetCustomTimers();
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__33), nameof(IntroCutscene._CoBegin_d__33.MoveNext))]
    public static class Start
    {
        public static void Postfix() => ButtonUtils.ResetCustomTimers(true);
    }
}