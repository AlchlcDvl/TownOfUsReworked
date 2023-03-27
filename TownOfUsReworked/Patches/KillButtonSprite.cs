using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

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
                if (!ConstantVariables.IsInGame)
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                else if (ConstantVariables.IsHnS)
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
            }
        }
    }
}