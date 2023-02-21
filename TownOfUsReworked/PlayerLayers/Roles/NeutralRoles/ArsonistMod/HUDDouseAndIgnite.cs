using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDDouseAndIgnite
    {
        public static Sprite IgniteSprite => TownOfUsReworked.IgniteSprite;
        public static Sprite DouseSprite => TownOfUsReworked.DouseSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Arsonist))
                return;

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;

                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;

                player.myRend().material.SetColor("_VisorColor", role.Color);
                player.nameText().color = Color.black;
            }

            if (role.DouseButton == null)
            {
                role.DouseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DouseButton.graphic.enabled = true;
                role.DouseButton.graphic.sprite = DouseSprite;
                role.DouseButton.gameObject.SetActive(false);
            }

            if (role.IgniteButton == null)
            {
                role.IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.IgniteButton.graphic.enabled = true;
                role.IgniteButton.graphic.sprite = IgniteSprite;
                role.IgniteButton.gameObject.SetActive(false);
            }

            role.IgniteButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.DousedAlive > 0);
            role.DouseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (!role.LastKiller && !CustomGameOptions.ArsoLastKillerBoost)
                role.IgniteButton.SetCoolDown(role.IgniteTimer(), CustomGameOptions.IgniteCd);
            else
                role.IgniteButton.SetCoolDown(0f, CustomGameOptions.IgniteCd);

            role.DouseButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            var doused = PlayerControl.AllPlayerControls.ToArray().Where(player => role.DousedPlayers.Contains(player.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayerDouse, role.DouseButton, notDoused);

            if (role.DousedAlive > 0)
                Utils.SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, doused);

            var renderer = role.DouseButton.graphic;

            if (!role.DouseButton.isCoolingDown && role.DouseButton.isActiveAndEnabled && role.ClosestPlayerDouse != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            var renderer2 = role.IgniteButton.graphic;
            
            if (!role.IgniteButton.isCoolingDown && role.IgniteButton.isActiveAndEnabled && role.ClosestPlayerIgnite != null)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
