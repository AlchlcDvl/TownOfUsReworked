using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SpeedPatch
    {
        private static float _time;
        private static bool reversed;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                __instance.body.velocity *= __instance.myPlayer.GetAppearance().SpeedFactor;

            if (__instance.myPlayer.Is(RoleEnum.Janitor))
            {
                var role = Role.GetRole<Janitor>(__instance.myPlayer);

                if (role.CurrentlyDragging != null && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    __instance.body.velocity *= CustomGameOptions.DragModifier;
            }
            else if (__instance.myPlayer.Is(RoleEnum.PromotedGodfather))
            {
                var role = Role.GetRole<PromotedGodfather>(__instance.myPlayer);

                if (role.CurrentlyDragging != null && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    __instance.body.velocity *= CustomGameOptions.DragModifier;
            }

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Drunk) && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove && __instance.AmOwner && !MeetingHud.Instance)
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
                else if (__instance.AmOwner)
                    __instance.body.velocity *= -1;
            }
            else if (__instance.myPlayer.Is(ModifierEnum.Flincher) && !__instance.myPlayer.Data.IsDead && __instance.myPlayer.CanMove && !MeetingHud.Instance)
            {
                _time += Time.deltaTime;

                if (_time >= CustomGameOptions.FlinchInterval && __instance.AmOwner)
                {
                    __instance.body.velocity *= -1;
                    _time -= CustomGameOptions.FlinchInterval;
                }
            }
        }

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && __instance.interpolateMovement != 0.0f && GameData.Instance)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= player.GetAppearance().SpeedFactor * (player.Data.IsDead && !player.Is(RoleEnum.Phantom) && !player.Is(RoleEnum.Revealer) &&
                    !player.Is(RoleEnum.Ghoul) && !player.Is(RoleEnum.Banshee) ? CustomGameOptions.GhostSpeed : 1);
            }
        }
    }
}