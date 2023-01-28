using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.IsDead)
                return;
            
            __instance.KillButton.gameObject.SetActive(false);
        }
    }
}