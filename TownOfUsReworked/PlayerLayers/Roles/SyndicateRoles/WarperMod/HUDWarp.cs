using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDWarp
    {
        public static Sprite Warp => TownOfUsReworked.WarpSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;

            if (isDead)
                return;

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (role.WarpButton == null)
            {
                role.WarpButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.WarpButton.graphic.enabled = true;
                role.WarpButton.graphic.sprite = Warp;
                role.WarpButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.WarpButton.gameObject.SetActive(false);
            }

            role.WarpButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.WarpButton.GetComponent<AspectPosition>().Update();

            role.WarpButton.SetCoolDown(role.WarpTimer(), CustomGameOptions.WarpCooldown);
            role.WarpButton.graphic.color = Palette.EnabledColor;
            role.WarpButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}