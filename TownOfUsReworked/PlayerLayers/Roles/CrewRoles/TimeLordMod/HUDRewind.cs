using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRewind
    {
        private static Sprite Rewind => TownOfUsReworked.Rewind;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.TimeLord))
                return;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);

            if (role.RewindButton == null)
            {
                role.RewindButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RewindButton.graphic.enabled = true;
                role.RewindButton.gameObject.SetActive(false);
            }

            role.RewindButton.graphic.sprite = Rewind;
            role.RewindButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer) && role.ButtonUsable);

            if (role.ButtonUsable)
                role.RewindButton.SetCoolDown(role.TimeLordRewindTimer(), role.GetCooldown());

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.RewindButton.cooldownTimerText, role.RewindButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.UsesText != null)
                role.UsesText.text = role.UsesLeft + "";

            var renderer = role.RewindButton.graphic;
            
            if (!role.RewindButton.isCoolingDown && !RecordRewind.rewinding && role.ButtonUsable)
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
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
        }
    }
}