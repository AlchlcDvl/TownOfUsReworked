using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.WraithMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDInvis
    {
        public static Sprite InvisSprite => TownOfUsReworked.InvisSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Wraith))
                return;

            var role = Role.GetRole<Wraith>(PlayerControl.LocalPlayer);

            if (role.InvisButton == null)
            {
                role.InvisButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InvisButton.graphic.enabled = true;
                role.InvisButton.graphic.sprite = InvisSprite;
                role.InvisButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            role.InvisButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.IsInvis)
                role.InvisButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.InvisDuration);
            else
                role.InvisButton.SetCoolDown(role.InvisTimer(), CustomGameOptions.InvisCd);

            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);
            
            var renderer2 = role.InvisButton.graphic;
            var renderer = role.KillButton.graphic;

            if (!role.IsInvis && !role.InvisButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
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