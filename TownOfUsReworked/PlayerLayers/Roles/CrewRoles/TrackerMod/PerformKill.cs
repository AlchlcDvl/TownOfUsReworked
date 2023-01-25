using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return false;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Tracker, role.ClosestPlayer, __instance) || __instance != role.TrackButton)
                return false;

            if (role.TrackerTimer() != 0f && __instance == role.TrackButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }

            var target = role.ClosestPlayer;
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;

            if (!CamouflageUnCamouflage.IsCamoed)
            {
                if (RainbowUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                    renderer.color = RainbowUtils.Rainbow;
                else
                    renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
            }
            else
                renderer.color = Color.gray;

            arrow.image = renderer;
            gameObj.layer = 5;
            arrow.target = target.transform.position;
            role.TrackerArrows.Add(target.PlayerId, arrow);
            role.UsesLeft--;
            role.LastTracked = DateTime.UtcNow;
            
            try
            {
                //SoundManager.Instance.PlaySound(TownOfUsReworked.TrackSound, false, 1f);
            } catch {}
            
            return false;
        }
    }
}
