using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Abilities.RadarMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrow
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var ability in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Radar))
            {
                var radar = (Radar)ability;

                if (radar.Player.Data.IsDead)
                {
                    radar.RadarArrow.DestroyAll();
                    radar.RadarArrow.Clear();
                }

                foreach (var arrow in radar.RadarArrow)
                {
                    radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer, PlayerControl.AllPlayerControls.ToArray().ToList());
                    arrow.target = radar.ClosestPlayer.transform.position;
                }
            }
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            
            foreach (var player in AllPlayers)
            {
                if (player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled)
                    continue;

                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;

                if (!isClosest)
                    continue;

                var vector = playerPosition - refPosition;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }
    }
}