using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDStabAndLust
    {
        public static Sprite LustSprite => TownOfUsReworked.Placeholder;
        public static Sprite Stab => TownOfUsReworked.StabSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
                return;

            var role = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            if (role.BloodlustButton == null)
            {
                role.BloodlustButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BloodlustButton.graphic.enabled = true;
                role.BloodlustButton.graphic.sprite = LustSprite;
                role.BloodlustButton.gameObject.SetActive(false);
            }

            if (role.StabButton == null)
            {
                role.StabButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StabButton.graphic.enabled = true;
                role.StabButton.graphic.sprite = Stab;
                role.StabButton.gameObject.SetActive(false);
            }

            role.BloodlustButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.StabButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.Lusted);

            if (role.Lusted)
            {
                role.BloodlustButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.BloodlustDuration);
                role.BloodlustButton.SetCoolDown(role.KillTimer(), CustomGameOptions.LustKillCd);
                Utils.SetTarget(ref role.ClosestPlayer, role.StabButton);
            }
            else
                role.BloodlustButton.SetCoolDown(role.LustTimer(), CustomGameOptions.BloodlustCd);
            
            var renderer = role.StabButton.graphic;
            var renderer2 = role.BloodlustButton.graphic;

            if (!role.Lusted && !role.BloodlustButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            if (role.Lusted && !role.StabButton.isCoolingDown && role.ClosestPlayer != null)
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
