using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDHackAndMimic
    {
        public static Sprite MimicSprite => TownOfUsReworked.MimicSprite;
        public static Sprite HackSprite => TownOfUsReworked.HackSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (role.GlitchButton == null)
            {
                role.GlitchButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.GlitchButton.graphic.enabled = true;

                if (role.ClosestPlayer == null)
                    role.GlitchButton.graphic.sprite = MimicSprite;
                else
                    role.GlitchButton.graphic.sprite = HackSprite;

                role.GlitchButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.GlitchButton.gameObject.SetActive(false);
            }

            role.GlitchButton.GetComponent<AspectPosition>().Update();

            if (role.GlitchButton.graphic.sprite != HackSprite && role.GlitchButton.graphic.sprite != MimicSprite)
                role.GlitchButton.graphic.sprite = HackSprite;

            role.GlitchButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            
            if (role.GlitchButton.graphic.sprite == HackSprite)
            {
                role.GlitchButton.SetCoolDown(role.HackTimer(), CustomGameOptions.HackCooldown);
                Utils.SetTarget(ref role.ClosestPlayer, role.GlitchButton);
            }
            else
            {
                if (role.IsUsingMimic)
                {
                    role.GlitchButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.GlitchButton.SetCoolDown(role.MimicTimer(), CustomGameOptions.MimicCooldown);
                role.GlitchButton.graphic.color = Palette.EnabledColor;
                role.GlitchButton.graphic.material.SetFloat("_Desat", 0f);
            }

            role.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.GlitchKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
        }
    }
}
