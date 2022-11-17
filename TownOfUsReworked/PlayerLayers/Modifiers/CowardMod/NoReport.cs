using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

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
                    DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                }
            }
        }
    }
}