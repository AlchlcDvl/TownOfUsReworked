using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDInteract
    {
        public static Sprite Interact => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Troll))
                return;

            var role = Role.GetRole<Troll>(PlayerControl.LocalPlayer);

            if (role.InteractButton == null)
            {
                role.InteractButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InteractButton.graphic.enabled = true;
                role.InteractButton.graphic.sprite = Interact;
                role.InteractButton.gameObject.SetActive(false);
            }

            role.InteractButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            Utils.SetTarget(ref role.ClosestPlayer, role.InteractButton);
            role.InteractButton.SetCoolDown(role.InteractTimer(), CustomGameOptions.InteractCooldown);
            var renderer = role.InteractButton.graphic;
            
            if (role.ClosestPlayer != null && !role.InteractButton.isCoolingDown)
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