using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
                role.DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                role.CurrentlyDragging = null;
                role.LastDragged = DateTime.UtcNow;
            }
        }
    }
}