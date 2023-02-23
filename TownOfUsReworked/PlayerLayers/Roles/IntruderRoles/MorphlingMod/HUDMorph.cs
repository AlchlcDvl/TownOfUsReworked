using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDMorph
    {
        public static Sprite SampleSprite => TownOfUsReworked.SampleSprite;
        public static Sprite MorphSprite => TownOfUsReworked.MorphSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Morphling))
                return;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (role.MorphButton == null)
            {
                role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = SampleSprite;
                role.MorphButton.gameObject.SetActive(false);
            }

            if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
                role.MorphButton.graphic.sprite = SampleSprite;

            role.MorphButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            
            if (role.MorphButton.graphic.sprite == SampleSprite)
            {
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
                var renderer2 = role.MorphButton.graphic;

                if (role.ClosestPlayer != null && !role.MorphButton.isCoolingDown)
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
            else
            {
                if (role.Morphed)
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                else 
                    role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);

                var renderer2 = role.MorphButton.graphic;
                role.MorphButton.SetTarget(null);

                if (!role.Morphed && !role.MorphButton.isCoolingDown)
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

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
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
