using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class KillButtonSprite
    {
        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        public static class AbilityButtonUpdatePatch
        {
            public static void Postfix()
            {
                if (!GameStates.IsInGame)
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                else if (GameStates.IsHnS)
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
            }
        }
    }
}