using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Modifiers.CowardMod
{
    public class NoReport
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Coward))
                {
                    __instance.ReportButton.graphic.enabled = false;
                    __instance.ReportButton.gameObject.SetActive(false);
                }
            }
        }
    }
}