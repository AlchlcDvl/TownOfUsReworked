namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SpeedPatch
    {
        private static float _time;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                __instance.body.velocity *= __instance.myPlayer.GetAppearance().SpeedFactor;

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

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && __instance.interpolateMovement != 0 && GameData.Instance)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= player.GetAppearance().SpeedFactor;
            }
        }
    }
}