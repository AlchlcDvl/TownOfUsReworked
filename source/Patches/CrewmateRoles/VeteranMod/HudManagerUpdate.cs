using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateAlertButton(__instance);
        }

        public static void UpdateAlertButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Veteran)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(alertButton.cooldownTimerText, alertButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            if (isDead)
            {
                alertButton.gameObject.SetActive(false);
                // alertButton.isActive = false;
            }
            else if (role.OnAlert)
            {
                alertButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AlertDuration);
            }
            else
            {
                alertButton.gameObject.SetActive(!MeetingHud.Instance);
                // alertButton.isActive = !MeetingHud.Instance;
                if (role.ButtonUsable)
                    alertButton.SetCoolDown(role.AlertTimer(), CustomGameOptions.AlertCd);
            }

            var renderer = alertButton.graphic;
            if (role.OnAlert || (!alertButton.isCoolingDown && role.ButtonUsable))
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