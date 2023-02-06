using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Freeze => TownOfUsReworked.FreezeSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.TimeMaster))
                return;

            var role = Role.GetRole<TimeMaster>(PlayerControl.LocalPlayer);

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.graphic.sprite = Freeze;
                role.FreezeButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.FreezeButton.gameObject.SetActive(false);
            }
            
            role.FreezeButton.GetComponent<AspectPosition>().Update();
            role.FreezeButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);

            if (role.Enabled)
            {
                role.FreezeButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.FreezeDuration);
                return;
            }

            role.FreezeButton.SetCoolDown(role.FreezeTimer(), CustomGameOptions.FreezeCooldown);
            role.FreezeButton.graphic.color = Palette.EnabledColor;
            role.FreezeButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}