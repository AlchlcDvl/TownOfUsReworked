using HarmonyLib;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.WarperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static Sprite WarpButton => TownOfUsReworked.WarpSprite;

        public static void Postfix(HudManager __instance)
        {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance)
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

            var role = Role.GetRole<Warper>(PlayerControl.LocalPlayer);

            if (role.WarpButton == null)
            {
                role.WarpButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.WarpButton.GetComponentsInChildren<TextMeshPro>()[0].text = "";
                role.WarpButton.graphic.enabled = true;
                role.WarpButton.graphic.sprite = WarpButton;
            }

            role.WarpButton.graphic.sprite = WarpButton;

            role.WarpButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            role.WarpButton.SetCoolDown(0f, 1f);
            var renderer = role.WarpButton.graphic;

            var position1 = __instance.UseButton.transform.position;
            role.WarpButton.transform.position = TownOfUsReworked.BelowVentPosition;

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}