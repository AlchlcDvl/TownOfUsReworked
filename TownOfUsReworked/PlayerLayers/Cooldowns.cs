using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            ButtonUtils.ResetCustomTimers(false);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__33), nameof(IntroCutscene._CoBegin_d__33.MoveNext))]
    public static class Start
    {
        public static void Postfix() => ButtonUtils.ResetCustomTimers(true);
    }
}