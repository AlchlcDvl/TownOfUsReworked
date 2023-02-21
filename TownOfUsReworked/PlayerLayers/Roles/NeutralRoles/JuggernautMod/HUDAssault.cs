using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDAssault
    {
        public static Sprite Assault => TownOfUsReworked.AssaultSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Juggernaut))
                return;

            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            if (role.AssaultButton == null)
            {
                role.AssaultButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AssaultButton.graphic.enabled = true;
                role.AssaultButton.graphic.sprite = Assault;
                role.AssaultButton.gameObject.SetActive(false);
            }

            role.AssaultButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.AssaultButton.SetCoolDown(role.KillTimer(), CustomGameOptions.JuggKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.AssaultButton);
            var renderer = role.AssaultButton.graphic;
            
            if (role.ClosestPlayer != null && !role.AssaultButton.isCoolingDown)
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