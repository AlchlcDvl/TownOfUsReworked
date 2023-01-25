using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
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
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
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

            if (role.IgniteButton == null)
            {
                role.IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.IgniteButton.graphic.enabled = true;
                role.IgniteButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.IgniteButton.gameObject.SetActive(false);
            }
            
            role.IgniteButton.GetComponent<AspectPosition>().Update();
            role.IgniteButton.graphic.sprite = IgniteSprite;

            role.IgniteButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            
            if (!role.LastKiller && !CustomGameOptions.ArsoLastKillerBoost)
                role.IgniteButton.SetCoolDown(role.IgniteTimer(), CustomGameOptions.IgniteCd);
            else
                role.IgniteButton.SetCoolDown(0f, CustomGameOptions.IgniteCd);

            __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !role.DousedPlayers.Contains(player.PlayerId)).ToList();
            var doused = PlayerControl.AllPlayerControls.ToArray().Where(player => role.DousedPlayers.Contains(player.PlayerId)).ToList();

            Utils.SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, notDoused);

            if (role.DousedAlive > 0)
                Utils.SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, doused);

            return;
        }
    }
}
