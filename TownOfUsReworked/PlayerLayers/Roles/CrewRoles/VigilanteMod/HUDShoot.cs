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
        public static Sprite Shoot => TownOfUsReworked.ShootSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return;
                
            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (role.ShootButton == null)
            {
                role.ShootButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ShootButton.graphic.enabled = true;
                role.ShootButton.graphic.sprite = Shoot;
                role.ShootButton.gameObject.SetActive(false);
            }

            role.ShootButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.KilledInno);
            role.ShootButton.SetCoolDown(role.KillTimer(), CustomGameOptions.VigiKillCd);
            Utils.SetTarget(ref role.ClosestPlayer, role.ShootButton);
            var renderer = role.ShootButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KilledInno && !role.ShootButton.isCoolingDown)
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
