using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDConfuse
    {
        public static Sprite Confuse => TownOfUsReworked.Placeholder;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Drunkard))
                return;

            var role = Role.GetRole<Drunkard>(PlayerControl.LocalPlayer);

            if (role.ConfuseButton == null)
            {
                role.ConfuseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ConfuseButton.graphic.enabled = true;
                role.ConfuseButton.graphic.sprite = Confuse;
                role.ConfuseButton.gameObject.SetActive(false);
            }

            role.ConfuseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.Enabled)
                role.ConfuseButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ConfuseDuration);
            else
                role.ConfuseButton.SetCoolDown(role.DrunkTimer(), CustomGameOptions.ConfuseCooldown);

            var renderer = role.ConfuseButton.graphic;

            if (!role.Confused && !role.ConfuseButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
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
            var renderer2 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }
    }
}