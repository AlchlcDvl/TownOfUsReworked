using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBug
    {
        public static Sprite Bug => TownOfUsReworked.BugSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative))
                return;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (role.BugButton == null)
            {
                role.BugButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BugButton.graphic.enabled = true;
                role.BugButton.graphic.sprite = Bug;
                role.BugButton.gameObject.SetActive(false);
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.BugButton.cooldownTimerText, role.BugButton.transform);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
                role.UsesText.gameObject.SetActive(false);
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";

            role.BugButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.UsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.PrimaryButton = role.BugButton;

            if (role.ButtonUsable)
                role.BugButton.SetCoolDown(role.BugTimer(), CustomGameOptions.BugCooldown);

            var renderer = role.BugButton.graphic;
            
            if (Utils.EnableAbilityButton(role.BugButton, role.Player, null, false, role.ButtonUsable))
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
