using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Cosmetics.CustomColors;
using UnityEngine;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class UpdateTrackerArrows
    {
        private static DateTime _time = DateTime.UnixEpoch;
        private static float Interval => CustomGameOptions.UpdateInterval;

        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Tracker))
                return;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TrackerArrows.Values.DestroyAll();
                role.TrackerArrows.Clear();
            }
            else
            {
                foreach (var arrow in role.TrackerArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);

                    if (player == null || player.Data?.IsDead != false || player.Data.Disconnected)
                    {
                        role.DestroyArrow(arrow.Key);
                        continue;
                    }

                    var Grey = DoUndo.IsCamoed;

                    if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Rainbow;
                    else if (ColorUtils.IsChroma(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Chroma;
                    else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Monochrome;
                    else if (ColorUtils.IsMantle(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Mantle;
                    else if (ColorUtils.IsFire(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Fire;
                    else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId) && !Grey)
                        arrow.Value.image.color = ColorUtils.Galaxy;
                    else if (Grey)
                        arrow.Value.image.color = Color.gray;
                    else
                        arrow.Value.image.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];

                    if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                        arrow.Value.target = player.transform.position;
                }

                if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                    _time = DateTime.UtcNow;
            }
        }
    }
}