using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.OperativeMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HudBug
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateBugButton(__instance);
        }

        public static void UpdateBugButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Operative)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var bugButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(bugButton.cooldownTimerText, bugButton.transform);
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
                bugButton.gameObject.SetActive(false);
            }
            else
            {
                bugButton.gameObject.SetActive(!MeetingHud.Instance);
                if (role.ButtonUsable)
                {
                    bugButton.SetCoolDown(role.BugTimer(), CustomGameOptions.BugCooldown);
                }
                
            }

            var renderer = bugButton.graphic;
            if (!bugButton.isCoolingDown && bugButton.gameObject.active && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}
