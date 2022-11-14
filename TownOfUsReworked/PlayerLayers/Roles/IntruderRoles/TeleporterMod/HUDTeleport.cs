using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTeleport
    {
        public static Sprite MarkSprite => TownOfUsReworked.MarkSprite;
        public static Sprite TeleportSprite => TownOfUsReworked.TeleportSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Teleporter))
                return;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (role.TeleportButton == null)
            {
                role.TeleportButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TeleportButton.graphic.enabled = true;
                role.TeleportButton.graphic.sprite = MarkSprite;
                role.TeleportButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.TeleportButton.gameObject.SetActive(false);
            }

            role.TeleportButton.GetComponent<AspectPosition>().Update();

            if (role.TeleportButton.graphic.sprite != MarkSprite && role.TeleportButton.graphic.sprite != TeleportSprite)
                role.TeleportButton.graphic.sprite = MarkSprite;

            role.TeleportButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.TeleportButton.graphic.color = Palette.EnabledColor;
            role.TeleportButton.graphic.material.SetFloat("_Desat", 0f);

            if (role.TeleportButton.graphic.sprite == MarkSprite)
                role.TeleportButton.SetCoolDown(0f, 1f);
            else
                role.TeleportButton.SetCoolDown(role.TeleportTimer(), CustomGameOptions.TeleportCd);
        }
    }
}
