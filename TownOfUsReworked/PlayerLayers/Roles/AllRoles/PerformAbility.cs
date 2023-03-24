using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role == null)
                return true;

            if (!PlayerControl.LocalPlayer.Data.IsDead)
                return true;

            if (__instance == role.ZoomButton)
            {
                role.Zooming = !role.Zooming;
                var orthographicSize = !role.Zooming ? 3f : 12f;
                Camera.main.orthographicSize = orthographicSize;

                foreach (var cam in Camera.allCameras)
                {
                    if (cam?.gameObject.name == "UI Camera")
                        cam.orthographicSize = orthographicSize;
                }

                ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
                return false;
            }
            else if (__instance == role.SpectateButton)
            {
                return false;
            }

            return true;
        }
    }
}