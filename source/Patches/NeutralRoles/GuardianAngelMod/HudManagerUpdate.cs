using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(protectButton.cooldownTimerText, protectButton.transform);
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
                protectButton.gameObject.SetActive(false);
            }
            else if (role.Protecting)
            {
                protectButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ProtectDuration);
                return;
            }
            else
            {
                protectButton.gameObject.SetActive(!MeetingHud.Instance);
                if (role.ButtonUsable)
                    protectButton.SetCoolDown(role.ProtectTimer(), CustomGameOptions.ProtectCd);
            }

            var renderer = protectButton.graphic;
            if (role.Protecting || (!protectButton.isCoolingDown && role.ButtonUsable))
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