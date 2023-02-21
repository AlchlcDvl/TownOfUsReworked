using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;

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
                role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SwoopButton.graphic.enabled = true;
                role.SwoopButton.graphic.sprite = Placeholder;
                role.SwoopButton.gameObject.SetActive(false);
            }

            role.SwoopButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.PrimaryButton = role.SwoopButton;

            if (role.IsSwooped)
                role.SwoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
            else
                role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCooldown);

            var renderer = role.SwoopButton.graphic;

            if (Utils.EnableAbilityButton(role.SwoopButton, role.Player, null, role.IsSwooped))
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