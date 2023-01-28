using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTrack
    {
        public static Sprite Track => TownOfUsReworked.TrackSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.TrackButton == null)
            {
                role.TrackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TrackButton.graphic.enabled = true;
                role.TrackButton.graphic.sprite = Track;
                role.TrackButton.gameObject.SetActive(false);
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(role.TrackButton.cooldownTimerText, role.TrackButton.transform);
                role.UsesText.transform.localPosition = new Vector3(role.UsesText.transform.localPosition.x + 0.26f, role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
                role.UsesText.gameObject.SetActive(false);
            }

            if (role.UsesText != null)
                role.UsesText.text = $"{role.UsesLeft}";

            role.TrackButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.UsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ButtonUsable);
            role.TrackButton.SetCoolDown(role.TrackerTimer(), CustomGameOptions.TrackCd);
            var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.IsTracking(x)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.TrackButton, notTracked);
            var renderer = role.TrackButton.graphic;
            
            if (role.ClosestPlayer != null && role.ButtonUsable && !role.TrackButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}