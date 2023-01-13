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

            if (role.HackButton == null)
            {
                role.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HackButton.graphic.enabled = true;
                role.HackButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.HackButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            if (role.MimicButton == null)
            {
                role.MimicButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MimicButton.graphic.enabled = true;
                role.MimicButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.SabotagePosition;
                role.MimicButton.gameObject.SetActive(false);
            }

            role.MimicButton.GetComponent<AspectPosition>().Update();
            role.HackButton.GetComponent<AspectPosition>().Update();
            role.HackButton.graphic.sprite = HackSprite;
            role.MimicButton.graphic.sprite = MimicSprite;
            role.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.HackButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.MimicButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.GlitchKillCooldown);
            role.HackButton.SetCoolDown(role.HackTimer(), CustomGameOptions.HackCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
            Utils.SetTarget(ref role.ClosestPlayer, role.HackButton);
            
            if (role.IsUsingMimic)
            {
                role.MimicButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MimicDuration);
                return;
            }
            else
            {
                role.MimicButton.SetCoolDown(role.MimicTimer(), CustomGameOptions.MimicCooldown);
                role.MimicButton.graphic.color = Palette.EnabledColor;
                role.MimicButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
