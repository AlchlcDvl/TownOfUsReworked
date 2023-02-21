using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDAlert
    {
        public static Sprite Alert => TownOfUsReworked.AlertSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Veteran))
                return;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (role.AlertButton == null)
            {
                role.AlertButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AlertButton.graphic.enabled = true;
                role.AlertButton.graphic.sprite = Alert;
                role.AlertButton.gameObject.SetActive(false);
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.AlertButton.cooldownTimerText, role.AlertButton.transform);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
                role.UsesText.gameObject.SetActive(false);
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";

            role.AlertButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.UsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.PrimaryButton = role.AlertButton;

            if (role.ButtonUsable)
            {
                if (role.Enabled)
                    role.AlertButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AlertDuration);
                else
                    role.AlertButton.SetCoolDown(role.AlertTimer(), CustomGameOptions.AlertCd);
            }

            var renderer = role.AlertButton.graphic;
            
            if (!role.AlertButton.isCoolingDown && role.ButtonUsable && !role.OnAlert)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}