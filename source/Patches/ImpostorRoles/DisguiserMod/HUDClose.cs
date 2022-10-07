using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.DisguiserMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Disguiser))
            {
                var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);
                role.DisguiseButton.graphic.sprite = TownOfUs.SampleSprite;
                role.MeasuredPlayer = null;
                role.LastDisguised = DateTime.UtcNow;
            }
        }
    }
}