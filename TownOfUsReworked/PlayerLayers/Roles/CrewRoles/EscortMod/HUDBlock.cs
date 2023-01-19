using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        private static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Escort))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);
            var roleblockButton = role.BlockButton;

            if (roleblockButton == null)
            {
                roleblockButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                roleblockButton.graphic.enabled = true;
                roleblockButton.gameObject.SetActive(false);
            }

            roleblockButton.graphic.sprite = Placeholder;
            roleblockButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance && !isDead);
            Utils.SetTarget(ref role.ClosestPlayer, roleblockButton);

            if (role.Enabled)
                roleblockButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.EscRoleblockDuration);
            else
                roleblockButton.SetCoolDown(role.RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown);

            var renderer = roleblockButton.graphic;
            
            if (role.ClosestPlayer != null)
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