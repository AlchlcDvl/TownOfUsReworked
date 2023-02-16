using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDHackAndMimic
    {
        public static Sprite MimicSprite => TownOfUsReworked.MimicSprite;
        public static Sprite HackSprite => TownOfUsReworked.HackSprite;
        public static Sprite EraseData => TownOfUsReworked.EraseDataSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (role.HackButton == null)
            {
                role.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HackButton.graphic.enabled = true;
                role.HackButton.graphic.sprite = HackSprite;
                role.HackButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = EraseData;
                role.KillButton.gameObject.SetActive(false);
            }

            if (role.MimicButton == null)
            {
                role.MimicButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MimicButton.graphic.enabled = true;
                role.MimicButton.graphic.sprite = MimicSprite;
                role.MimicButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.HackButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.MimicButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.GlitchKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
            Utils.SetTarget(ref role.ClosestPlayer, role.HackButton);
            
            if (role.IsUsingMimic)
                role.MimicButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MimicDuration);
            else
                role.MimicButton.SetCoolDown(role.MimicTimer(), CustomGameOptions.MimicCooldown);
            
            if (role.IsUsingHack)
                role.HackButton.SetCoolDown(role.TimeRemaining2, CustomGameOptions.HackDuration);
            else
                role.HackButton.SetCoolDown(role.HackTimer(), CustomGameOptions.HackCooldown);

            var renderer = role.MimicButton.graphic;
            var renderer2 = role.HackButton.graphic;
            var renderer3 = role.KillButton.graphic;

            if (!role.MimicButton.isCoolingDown && !role.IsUsingMimic)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (!role.KillButton.isCoolingDown && role.ClosestPlayer != null)
            {
                renderer3.color = Palette.EnabledColor;
                renderer3.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer3.color = Palette.DisabledClear;
                renderer3.material.SetFloat("_Desat", 1f);
            }

            if (!role.HackButton.isCoolingDown && !role.IsUsingHack && role.ClosestPlayer != null)
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
