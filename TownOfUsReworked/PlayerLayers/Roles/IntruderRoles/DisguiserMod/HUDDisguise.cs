using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDDisguise
    {
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Disguiser))
                return;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (role.DisguiseButton == null)
            {
                role.DisguiseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DisguiseButton.graphic.enabled = true;
                role.DisguiseButton.graphic.sprite = MeasureSprite;
                role.DisguiseButton.gameObject.SetActive(false);

                if (role.DisguiseButton.graphic.sprite != MeasureSprite && role.DisguiseButton.graphic.sprite != DisguiseSprite)
                    role.DisguiseButton.graphic.sprite = MeasureSprite;
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            var targets = PlayerControl.AllPlayerControls.ToArray().ToList();

            if (CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)
                targets = targets.Where(x => x.Is(Faction.Intruder)).ToList();
            else if (CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders)
                targets = targets.Where(x => !x.Is(Faction.Intruder)).ToList();

            role.DisguiseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.DisguiseButton, targets);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);

            if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                role.DisguiseButton.SetCoolDown(0f, 1f);
            else
            {
                if (role.Disguised)
                    role.DisguiseButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DisguiseDuration);
                else
                    role.DisguiseButton.SetCoolDown(role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown);
            }

            var renderer = role.DisguiseButton.graphic;
            var renderer2 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.DisguiseButton.isCoolingDown && !role.Disguised)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

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
