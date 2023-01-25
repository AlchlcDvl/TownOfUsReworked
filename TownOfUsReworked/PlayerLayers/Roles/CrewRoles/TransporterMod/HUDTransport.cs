using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTransport
    {
        private static Sprite Transport => TownOfUsReworked.TransportSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (role.TransportButton == null)
            {
                role.TransportButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TransportButton.graphic.enabled = true;
                role.TransportButton.gameObject.SetActive(false);
            }

            role.TransportButton.graphic.sprite = Transport;
            role.TransportButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer) && role.ButtonUsable);

            if (role.ButtonUsable)
                role.TransportButton.SetCoolDown(role.TransportTimer(), CustomGameOptions.TransportCooldown);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.TransportButton.cooldownTimerText, role.TransportButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";

            var renderer = role.TransportButton.graphic;
            
            if (!role.TransportButton.isCoolingDown && role.ButtonUsable)
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