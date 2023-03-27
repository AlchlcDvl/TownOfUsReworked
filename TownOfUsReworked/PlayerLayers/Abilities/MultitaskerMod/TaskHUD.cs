using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities.MultitaskerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TaskHUD
    {
        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Multitasker))
                return;

            if (PlayerControl.LocalPlayer.Data.Disconnected)
                return;

            if (!Minigame.Instance)
                return;

            var Base = Minigame.Instance as MonoBehaviour;
            SpriteRenderer[] rends = Base.GetComponentsInChildren<SpriteRenderer>();
            var trans = CustomGameOptions.Transparancy / 100f;

            for (int i = 0; i < rends.Length; i++)
            {
                var oldColor1 = rends[i].color[0];
                var oldColor2 = rends[i].color[1];
                var oldColor3 = rends[i].color[2];
                rends[i].color = new Color(oldColor1, oldColor2, oldColor3, trans);
            }
        }
    }
}