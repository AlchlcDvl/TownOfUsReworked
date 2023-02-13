using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShapeshift
    {
        public static Sprite Shapeshift => TownOfUsReworked.Placeholder;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shapeshifter))
                return;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (role.ShapeshiftButton == null)
            {
                role.ShapeshiftButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ShapeshiftButton.graphic.enabled = true;
                role.ShapeshiftButton.graphic.sprite = Shapeshift;
                role.ShapeshiftButton.gameObject.SetActive(false);
            }

            role.ShapeshiftButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.Enabled)
                role.ShapeshiftButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ShapeshiftDuration);
            else
                role.ShapeshiftButton.SetCoolDown(role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown);
            
            var renderer2 = role.ShapeshiftButton.graphic;

            if (!role.ShapeshiftButton.isCoolingDown && !role.Shapeshifted)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = Kill;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && Role.SyndicateHasChaosDrive);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown);
            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notSyndicate);
            var renderer = role.KillButton.graphic;
            
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