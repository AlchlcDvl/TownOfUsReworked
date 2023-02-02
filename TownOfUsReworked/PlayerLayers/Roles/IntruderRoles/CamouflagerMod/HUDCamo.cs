using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDCamo
    {
        public static Sprite Camouflage => TownOfUsReworked.Camouflage;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Camouflager))
                return;

            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);

            if (role.CamouflageButton == null)
            {
                role.CamouflageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CamouflageButton.graphic.sprite = Camouflage;
                role.CamouflageButton.graphic.enabled = true;
                role.CamouflageButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();

            if (role.IsRecruit)
                notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(SubFaction.Cabal)).ToList();

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            role.CamouflageButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.Enabled)
                role.CamouflageButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
            else
                role.CamouflageButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);

            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);
            
            var renderer2 = role.CamouflageButton.graphic;
            var renderer = role.KillButton.graphic;

            if (!role.Camouflaged && !role.CamouflageButton.isCoolingDown)
            {
                role.CamouflageButton.graphic.color = Palette.EnabledColor;
                role.CamouflageButton.graphic.material.SetFloat("_Desat", 0f);
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