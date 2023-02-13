using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDSeer
    {
        public static Sprite Seer => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Seer))
                return;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            if (role.RevealButton == null)
            {
                role.RevealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RevealButton.graphic.enabled = true;
                role.RevealButton.graphic.sprite = Seer;
                role.RevealButton.gameObject.SetActive(false);
            }

            role.RevealButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.RevealButton.SetCoolDown(role.SeerTimer(), CustomGameOptions.SeerCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.RevealButton);
            var renderer = role.RevealButton.graphic;
            
            if (role.ClosestPlayer != null && !role.RevealButton.isCoolingDown)
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