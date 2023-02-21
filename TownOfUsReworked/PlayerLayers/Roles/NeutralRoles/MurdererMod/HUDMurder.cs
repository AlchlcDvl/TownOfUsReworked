using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.MurdererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMurder
    {
        public static Sprite Murder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Murderer))
                return;

            var role = Role.GetRole<Murderer>(PlayerControl.LocalPlayer);

            if (role.MurderButton == null)
            {
                role.MurderButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MurderButton.graphic.enabled = true;
                role.MurderButton.graphic.sprite = Murder;
                role.MurderButton.gameObject.SetActive(false);
            }

            role.MurderButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.MurderButton.SetCoolDown(role.KillTimer(), CustomGameOptions.MurdKCD);
            Utils.SetTarget(ref role.ClosestPlayer, role.MurderButton);
            var renderer = role.MurderButton.graphic;
            
            if (role.ClosestPlayer != null && !role.MurderButton.isCoolingDown)
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