using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShoot
    {
        private static Sprite Shoot => TownOfUsReworked.ShootSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return;
                
            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (role.ShootButton == null)
            {
                role.ShootButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ShootButton.graphic.enabled = true;
                role.ShootButton.gameObject.SetActive(false);
            }

            role.ShootButton.graphic.sprite = Shoot;
            role.ShootButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer) && !role.KilledInno);
            role.ShootButton.SetCoolDown(role.KillTimer(), CustomGameOptions.VigiKillCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ShootButton);
            var renderer = role.ShootButton.graphic;
            
            if (role.ClosestPlayer != null)
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
