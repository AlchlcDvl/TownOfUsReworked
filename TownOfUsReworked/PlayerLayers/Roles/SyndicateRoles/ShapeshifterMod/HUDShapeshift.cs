using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShapeshift
    {
        public static Sprite Shapeshift => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Shapeshifter))
                return;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (role.ShapeshiftButton == null)
            {
                role.ShapeshiftButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ShapeshiftButton.graphic.enabled = true;
                role.ShapeshiftButton.graphic.sprite = Shapeshift;
                role.ShapeshiftButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.ShapeshiftButton.gameObject.SetActive(false);
            }
            
            role.ShapeshiftButton.GetComponent<AspectPosition>().Update();
            role.ShapeshiftButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.Enabled)
            {
                role.ShapeshiftButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            role.ShapeshiftButton.SetCoolDown(role.ShapeshiftTimer(), CustomGameOptions.CamouflagerCd);
            role.ShapeshiftButton.graphic.color = Palette.EnabledColor;
            role.ShapeshiftButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}