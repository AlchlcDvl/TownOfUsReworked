using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.RevivedRoles.Inspector
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDExamine
    {
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole?.RoleType != RoleEnum.Inspector)
                return;

            if (role.InspectButton == null)
            {
                role.InspectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InspectButton.graphic.enabled = true;
                role.InspectButton.graphic.sprite = Placeholder;
                role.InspectButton.gameObject.SetActive(false);
            }

            role.InspectButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.InspectButton.SetCoolDown(role.InspectTimer(), CustomGameOptions.InspectCooldown);
            var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Inspected.Contains(x)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.InspectButton, notinspected);
            var renderer = role.InspectButton.graphic;
            
            if (role.ClosestPlayer != null && !role.InspectButton.isCoolingDown)
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