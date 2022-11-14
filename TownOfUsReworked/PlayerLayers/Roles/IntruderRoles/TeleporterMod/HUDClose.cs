using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;
                
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Teleporter))
            {
                var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);
                role.TeleportButton.graphic.sprite = TownOfUsReworked.MarkSprite;
                role.LastTeleport = DateTime.UtcNow;
            }
        }
    }
}