using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers.FlincherMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class Flinch
    {
        private static float _time;

        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(ModifierEnum.Flincher) && !__instance.myPlayer.Data.IsDead && __instance.myPlayer.CanMove && !MeetingHud.Instance)
            {
                _time += Time.deltaTime;

                if (_time >= CustomGameOptions.FlinchInterval && __instance.AmOwner)
                {
                    __instance.body.velocity *= -1;
                    _time -= CustomGameOptions.FlinchInterval;
                }
            }
        }
    }
}