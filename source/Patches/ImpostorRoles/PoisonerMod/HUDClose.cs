using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
            {
                var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
                role.PoisonButton.graphic.sprite = TownOfUs.PoisonSprite;
                role.LastPoisoned = DateTime.UtcNow;
            }
        }
    }
}