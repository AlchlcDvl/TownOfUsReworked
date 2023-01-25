using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDExamine
    {
        private static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Inspector))
                return;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (role.InspectButton == null)
            {
                role.InspectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InspectButton.graphic.enabled = true;
                role.InspectButton.gameObject.SetActive(false);
            }

            role.InspectButton.graphic.sprite = Placeholder;
            role.InspectButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer));
            role.InspectButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.InspectCooldown);
            var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Inspected.Contains(x)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.InspectButton, notinspected);

            var renderer = role.InspectButton.graphic;
            
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