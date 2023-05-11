using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
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
            else
            {
                var ghostRole = false;

                if (PlayerControl.LocalPlayer.IsPostmortal())
                    ghostRole = !PlayerControl.LocalPlayer.Caught();

                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead);
            }
        }
    }
}