using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShift
    {
        private static Sprite Shift => TownOfUsReworked.Shift;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shifter))
                return;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (role.ShiftButton == null)
            {
                role.ShiftButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ShiftButton.graphic.enabled = true;
                role.ShiftButton.graphic.sprite = Shift;
                role.ShiftButton.gameObject.SetActive(false);
            }

            role.ShiftButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer, __instance));
            role.ShiftButton.SetCoolDown(role.ShiftTimer(), CustomGameOptions.ShifterCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ShiftButton);
            var renderer = role.ShiftButton.graphic;
            
            if (role.ClosestPlayer != null && !role.ShiftButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
