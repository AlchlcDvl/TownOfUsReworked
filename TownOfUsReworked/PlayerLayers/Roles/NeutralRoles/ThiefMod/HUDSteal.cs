using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDSteal
    {
        public static Sprite Steal => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Thief))
                return;

            var role = Role.GetRole<Thief>(PlayerControl.LocalPlayer);

            if (role.StealButton == null)
            {
                role.StealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StealButton.graphic.enabled = true;
                role.StealButton.graphic.sprite = Steal;
                role.StealButton.gameObject.SetActive(false);
            }

            role.StealButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.StealButton.SetCoolDown(role.KillTimer(), CustomGameOptions.ThiefKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.StealButton);
            var renderer = role.StealButton.graphic;
            
            if (role.ClosestPlayer != null && !role.StealButton.isCoolingDown)
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