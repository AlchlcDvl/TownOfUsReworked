using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static Sprite Guess => TownOfUsReworked.Placeholder;
        public static Sprite Hunt => TownOfUsReworked.WhisperSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter))
                return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (role.GuessButton == null)
            {
                role.GuessButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.GuessButton.graphic.enabled = true;
                role.GuessButton.graphic.sprite = Guess;
                role.GuessButton.gameObject.SetActive(false);
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";

            role.GuessButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.GuessButton.SetCoolDown(role.CheckTimer(), CustomGameOptions.BountyHunterCooldown);
            role.UsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.TargetFound);
            Utils.SetTarget(ref role.ClosestPlayer, role.GuessButton);
            var renderer = role.GuessButton.graphic;

            if (role.TargetFound || role.UsesLeft <= 0)
                role.GuessButton.graphic.sprite = Hunt;
            
            if (role.ClosestPlayer != null && !role.GuessButton.isCoolingDown)
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