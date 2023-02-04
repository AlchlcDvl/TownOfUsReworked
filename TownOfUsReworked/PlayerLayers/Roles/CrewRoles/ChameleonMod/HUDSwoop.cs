using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDSwoop
    {
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon))
                return;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (role.SwoopButton == null)
            {
                role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform);
                role.SwoopButton.graphic.enabled = true;
                role.SwoopButton.graphic.sprite = Placeholder;
                role.SwoopButton.gameObject.SetActive(false);
            }

            role.SwoopButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.IsSwooped)
                role.SwoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
            else
                role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCooldown);

            var renderer = role.SwoopButton.graphic;

            if (role.IsSwooped || !role.SwoopButton.isCoolingDown)
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