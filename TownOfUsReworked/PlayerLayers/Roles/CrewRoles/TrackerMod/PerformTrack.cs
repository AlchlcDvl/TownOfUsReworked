using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformTrack
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker, true))
                return false;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.TrackerTimer() != 0f && __instance == role.TrackButton)
                return false;

            if (__instance == role.TrackButton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);

                if (interact[3] == true && interact[0] == true)
                {
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
                else if (interact[1] == true)
                    role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}
