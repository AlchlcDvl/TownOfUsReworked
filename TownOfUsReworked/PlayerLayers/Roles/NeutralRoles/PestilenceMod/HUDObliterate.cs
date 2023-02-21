using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDObliterate
    {
        public static Sprite Obliterate => TownOfUsReworked.ObliterateSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Pestilence))
                return;

            var role = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);

            if (role.ObliterateButton == null)
            {
                role.ObliterateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ObliterateButton.graphic.enabled = true;
                role.ObliterateButton.graphic.sprite = Obliterate;
                role.ObliterateButton.gameObject.SetActive(false);
            }

            role.ObliterateButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.ObliterateButton.SetCoolDown(role.KillTimer(), CustomGameOptions.PestKillCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ObliterateButton);
            var renderer = role.ObliterateButton.graphic;
            
            if (role.ClosestPlayer != null && !role.ObliterateButton.isCoolingDown)
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