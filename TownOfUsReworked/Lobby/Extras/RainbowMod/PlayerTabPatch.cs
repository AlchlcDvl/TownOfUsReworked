using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.Lobby.Extras.RainbowMod
{
    [HarmonyPatch(typeof(PlayerTab))]
    public static class PlayerTabPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.OnEnable))]
        public static void OnEnablePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                var colorChip = __instance.ColorChips[i];
                colorChip.transform.localScale *= 0.6f;
                var x = __instance.XRange.Lerp((i % 6) / 6f) + 0.166f;
                var y = __instance.YStart - (i / 6) * 0.5f;
                colorChip.transform.localPosition = new Vector3(x, y, 2f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.Update))]
        public static void UpdatePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                if (RainbowUtils.IsRainbow(i))
                {
                    __instance.ColorChips[i].Inner.SpriteColor = RainbowUtils.Rainbow;
                    break;
                }
            }

        }
    }
}
