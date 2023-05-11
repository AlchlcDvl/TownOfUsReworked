using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
    public static class MinigamePatch
    {
        public static void Postfix(Minigame __instance)
        {
            if (!__instance || !PlayerControl.LocalPlayer.Is(AbilityEnum.Multitasker))
                return;

            foreach (var rend in Minigame.Instance?.GetComponentsInChildren<SpriteRenderer>())
            {
                var oldColor1 = rend.color[0];
                var oldColor2 = rend.color[1];
                var oldColor3 = rend.color[2];
                rend.color = new Color(oldColor1, oldColor2, oldColor3, CustomGameOptions.Transparancy / 100f);
            }
        }
    }
}