using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDWhisper
    {
        public static Sprite WhisperSprite => TownOfUsReworked.WhisperSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (role.WhisperButton == null)
            {
                role.WhisperButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.WhisperButton.graphic.enabled = true;
                role.WhisperButton.graphic.sprite = WhisperSprite;
                role.WhisperButton.gameObject.SetActive(false);
            }

            role.WhisperButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer, __instance));

            role.WhisperButton.SetCoolDown(role.WhisperTimer(), CustomGameOptions.WhisperCooldown + (CustomGameOptions.WhisperCooldownIncrease * role.WhisperCount));

            var renderer = role.WhisperButton.graphic;

            if (!role.WhisperButton.isCoolingDown && role.WhisperButton.gameObject.active)
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