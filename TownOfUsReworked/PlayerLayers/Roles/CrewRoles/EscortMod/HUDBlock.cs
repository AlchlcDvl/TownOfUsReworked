using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        private static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
            {
                role.BlockButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlockButton.graphic.enabled = true;
                role.BlockButton.gameObject.SetActive(false);
            }

            role.BlockButton.GetComponent<AspectPosition>().Update();
            role.BlockButton.graphic.sprite = Placeholder;
            role.BlockButton.gameObject.SetActive(Utils.SetActive(role.Player));
            Utils.SetTarget(ref role.ClosestPlayer, role.BlockButton);

            if (role.Enabled)
                role.BlockButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.EscRoleblockDuration);
            else
                role.BlockButton.SetCoolDown(role.RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown);

            var renderer = role.BlockButton.graphic;
            
            if (role.ClosestPlayer != null)
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