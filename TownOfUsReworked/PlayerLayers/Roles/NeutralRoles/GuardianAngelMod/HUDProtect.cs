using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDProtect
    {
        public static Sprite Protect => TownOfUsReworked.ProtectSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.GuardianAngel))
                return;

            var role = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (role.ProtectButton == null)
            {
                role.ProtectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ProtectButton.graphic.enabled = true;
                role.ProtectButton.graphic.sprite = Protect;
                role.ProtectButton.gameObject.SetActive(false);
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
            
            if (!role.Player.Data.IsDead)
                role.ProtectButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            else
                role.ProtectButton.gameObject.SetActive(CustomGameOptions.ProtectBeyondTheGrave && role.TargetAlive && !MeetingHud.Instance && GameStates.IsInGame);
            
            if (role.Protecting)
                role.ProtectButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ProtectDuration);
            else if (role.ButtonUsable)
                role.ProtectButton.SetCoolDown(role.ProtectTimer(), CustomGameOptions.ProtectCd);

            var renderer = role.ProtectButton.graphic;

            if (!role.Protecting && !role.ProtectButton.isCoolingDown && role.ButtonUsable)
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