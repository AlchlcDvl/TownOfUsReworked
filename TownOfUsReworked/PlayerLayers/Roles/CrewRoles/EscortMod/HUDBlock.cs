using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (role.BlockButton == null)
            {
                role.BlockButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlockButton.graphic.enabled = true;
                role.BlockButton.graphic.sprite = Placeholder;
                role.BlockButton.gameObject.SetActive(false);
            }

            role.BlockButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            Utils.SetTarget(ref role.ClosestPlayer, role.BlockButton);

            if (role.Enabled)
                role.BlockButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.EscRoleblockDuration);
            else
                role.BlockButton.SetCoolDown(role.RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown);

            var renderer = role.BlockButton.graphic;
            
            if (Utils.EnableAbilityButton(role.BlockButton, role.Player, role.ClosestPlayer))
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