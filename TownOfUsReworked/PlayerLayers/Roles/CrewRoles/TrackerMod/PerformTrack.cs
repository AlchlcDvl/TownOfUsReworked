using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Cosmetics.CustomColors;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformTrack
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return true;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.TrackButton)
            {
                if (!Utils.ButtonUsable(role.TrackButton))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.TrackerTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    var target = role.ClosestPlayer;
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = TownOfUsReworked.Arrow;
                    var Grey = CamouflageUnCamouflage.IsCamoed;

                    if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Rainbow;
                    else if (ColorUtils.IsChroma(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Chroma;
                    else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Monochrome;
                    else if (ColorUtils.IsMantle(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Mantle;
                    else if (ColorUtils.IsFire(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Fire;
                    else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Galaxy;
                    else if (Grey)
                        renderer.color = Color.gray;
                    else
                        renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];

                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = target.transform.position;
                    role.TrackerArrows.Add(target.PlayerId, arrow);
                    role.UsesLeft--;
                }
                
                if (interact[0] == true)
                    role.LastTracked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return true;
        }
    }
}
