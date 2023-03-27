using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class AddPrints
    {
        private static float _time;
        private static float Interval => CustomGameOptions.FootprintInterval;
        private static bool Vent => CustomGameOptions.VentFootprintVisible;

        private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

        public static void Postfix()
        {
            if (!ConstantVariables.IsInGame || !PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist) || LobbyBehaviour.Instance || MeetingHud.Instance)
                return;

            var ret = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (ret.RevivedRole?.RoleType != RoleEnum.Detective)
                return;

            _time += Time.deltaTime;

            if (_time >= Interval)
            {
                _time -= Interval;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player?.Data.IsDead != false || player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        continue;

                    var canPlace = !ret.AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.Player.PlayerId ==
                        player.PlayerId);

                    if (Vent && ShipStatus.Instance?.AllVents.Any(vent => Vector2.Distance(vent.gameObject.transform.position, Position(player)) < 1f) == true)
                        canPlace = false;

                    if (canPlace)
                        _ = new Footprint(player, ret);
                }

                for (var i = 0; i < ret.AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = ret.AllPrints[i];

                        if (footprint.Update())
                            i--;
                    } catch { /*Assume footprint value is null and allow the loop to continue*/ }
                }
            }
        }
    }
}