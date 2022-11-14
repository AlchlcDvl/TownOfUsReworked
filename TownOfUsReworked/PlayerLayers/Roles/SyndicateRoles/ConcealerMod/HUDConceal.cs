using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDConceal
    {
        public static Sprite Conceal => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Concealer))
                return;

            var role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);

            if (role.ConcealButton == null)
            {
                role.ConcealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ConcealButton.graphic.enabled = true;
                role.ConcealButton.graphic.sprite = Conceal;
                role.ConcealButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.ConcealButton.gameObject.SetActive(false);
            }
            
            role.ConcealButton.GetComponent<AspectPosition>().Update();
            role.ConcealButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.Enabled)
            {
                role.ConcealButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            role.ConcealButton.SetCoolDown(role.ConcealTimer(), CustomGameOptions.CamouflagerCd);
            role.ConcealButton.graphic.color = Palette.EnabledColor;
            role.ConcealButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}