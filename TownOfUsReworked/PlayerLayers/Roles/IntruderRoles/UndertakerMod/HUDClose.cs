using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Undertaker))
            {
                var role2 = (Undertaker) role;
                role2.DragDropButton.graphic.sprite = TownOfUsReworked.DragSprite;
                role2.CurrentlyDragging = null;
                role2.LastDragged = DateTime.UtcNow;
            }
        }
    }
}