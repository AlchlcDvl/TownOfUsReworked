using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.DrunkMod
{
    public class InvertedControls
    {
        public static float _time = 0f;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Drunk) && !__instance.myPlayer.Data.IsDead && __instance.myPlayer.CanMove &&
                    !MeetingHud.Instance)
                {
                    _time += Time.deltaTime;

                    if (CustomGameOptions.DrunkControlsSwap)
                    {
                        if (_time > CustomGameOptions.DrunkInterval)
                        {
                            if (__instance.AmOwner)
                                __instance.body.velocity *= -1;

                            _time -= CustomGameOptions.DrunkInterval;
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
}
