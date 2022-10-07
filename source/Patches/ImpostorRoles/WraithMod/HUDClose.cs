using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Wraith))
            {
                var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);
                role.LastInvis = DateTime.UtcNow;
            }
        }
    }
}