using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers.CowardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class NoReport
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Coward))
                __instance.ReportButton.gameObject.SetActive(false);
        }
    }
}