using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateVestButton(__instance);
        }

        public static void UpdateVestButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Survivor)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var vestButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(vestButton.cooldownTimerText, vestButton.transform);
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
                vestButton.gameObject.SetActive(false);
            }
            else if (role.Vesting)
            {
                vestButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.VestDuration);
                return;
            }
            else
            {
                vestButton.gameObject.SetActive(!MeetingHud.Instance);
                if (role.ButtonUsable)
                    vestButton.SetCoolDown(role.VestTimer(), CustomGameOptions.VestCd);
            }

            var renderer = vestButton.graphic;
            if (role.Vesting || (!vestButton.isCoolingDown && role.ButtonUsable))
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