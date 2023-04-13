using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class AddPrints
    {
        private static float _time;

        private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

        public static void Postfix()
        {
            if (!ConstantVariables.IsInGame || !PlayerControl.LocalPlayer.Is(RoleEnum.Detective) || LobbyBehaviour.Instance || MeetingHud.Instance)
                return;

            var investigator = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.FootprintInterval)
            {
                _time -= CustomGameOptions.FootprintInterval;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player?.Data.IsDead != false || player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        continue;

                    var canPlace = !investigator.AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.Player.PlayerId ==
                        player.PlayerId);

                    if (CustomGameOptions.VentFootprintVisible && ShipStatus.Instance?.AllVents.Any(vent => Vector2.Distance(vent.gameObject.transform.position, Position(player)) < 1f) ==
                        true)
                    {
                        canPlace = false;
                    }

                    if (canPlace)
                        _ = new Footprint(player, investigator);
                }

                for (var i = 0; i < investigator.AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = investigator.AllPrints[i];

                        if (footprint.Update())
                            i--;
                    } catch { /*Assume footprint value is null and allow the loop to continue*/ }
                }
            }
        }
    }
}