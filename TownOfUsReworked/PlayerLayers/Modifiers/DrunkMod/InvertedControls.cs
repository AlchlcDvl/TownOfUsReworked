using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.DrunkMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class InvertedControls
    {
        private static float _time;
        private static bool reversed;

        public static void Postfix(PlayerPhysics __instance)
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Drunk) && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove && __instance.AmOwner &&
                !MeetingHud.Instance)
            {
                _time += Time.deltaTime;

                if (CustomGameOptions.DrunkControlsSwap)
                {
                    if (_time > CustomGameOptions.DrunkInterval)
                    {
                        if (__instance.AmOwner && !reversed)
                            __instance.body.velocity *= -1;

                        _time -= CustomGameOptions.DrunkInterval;
                        reversed = !reversed;
                    }
                }
                else
                {
                    if (__instance.AmOwner)
                        __instance.body.velocity *= -1;
                }
            }
        }
    }
}