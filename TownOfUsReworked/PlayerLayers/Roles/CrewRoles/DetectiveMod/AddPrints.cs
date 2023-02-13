using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class AddPrints
    {
        private static float _time;
        private static float Interval => CustomGameOptions.FootprintInterval * 10;
        private static bool Vent => CustomGameOptions.VentFootprintVisible;

        private static Vector2 Position(PlayerControl player)
        {
            return player.GetTruePosition() + new Vector2(0, 0.366667f);
        }

        public static void Postfix(PlayerControl __instance)
        {
            if (!GameStates.IsInGame || !PlayerControl.LocalPlayer.Is(RoleEnum.Detective) || LobbyBehaviour.Instance || MeetingHud.Instance)
                return;
            
            var investigator = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
            _time += Time.deltaTime;

            if (_time >= Interval)
            {
                _time -= Interval;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data.IsDead || player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        continue;

                    var canPlace = !investigator.AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.Player.PlayerId ==
                        player.PlayerId);

                    if (Vent && ShipStatus.Instance != null)
                    {
                        if (ShipStatus.Instance.AllVents.Any(vent => Vector2.Distance(vent.gameObject.transform.position, Position(player)) < 1f))
                            canPlace = false;
                    }

                    if (canPlace)
                        new Footprint(player, investigator);
                }

                for (var i = 0; i < investigator.AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = investigator.AllPrints[i];

                        if (footprint.Update())
                            i--;
                    }
                    catch
                    {
                        //Assume footprint value is null and allow the loop to continue
                        continue;
                    }
                    
                }
            }
        }
    }
}