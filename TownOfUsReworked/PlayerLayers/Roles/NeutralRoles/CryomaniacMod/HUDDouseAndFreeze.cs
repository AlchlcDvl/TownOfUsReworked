using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndFreeze
    {
        public static Sprite Freeze => TownOfUsReworked.Placeholder;
        public static Sprite Douse => TownOfUsReworked.DouseSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Cryomaniac))
                return;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;

                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;

                player.myRend().material.SetColor("_VisorColor", role.Color);
                player.nameText().color = Color.black;
            }

            if (role.FreezeButton == null)
            {
                role.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FreezeButton.graphic.enabled = true;
                role.FreezeButton.graphic.sprite = Freeze;
                role.FreezeButton.gameObject.SetActive(false);
            }

            if (role.DouseButton == null)
            {
                role.DouseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DouseButton.graphic.enabled = true;
                role.DouseButton.graphic.sprite = Douse;
                role.DouseButton.gameObject.SetActive(false);
            }

            role.FreezeButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.FreezeUsed && role.DousedAlive > 0);
            role.DouseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.FreezeButton.SetCoolDown(0f, 1f);
            role.DouseButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.CryoDouseCooldown);
            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.DouseButton, notDoused);
            var renderer2 = role.FreezeButton.graphic;

            if (!role.FreezeButton.isCoolingDown && role.FreezeButton.isActiveAndEnabled && !role.FreezeUsed && role.DousedAlive > 0)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            var renderer = role.DouseButton.graphic;

            if (!role.DouseButton.isCoolingDown && role.DouseButton.isActiveAndEnabled && role.ClosestPlayer != null)
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
