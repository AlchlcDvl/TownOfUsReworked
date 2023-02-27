using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Modifiers.CowardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class NoReport
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Coward))
                __instance.ReportButton.gameObject.SetActive(false);
        }
    }
}