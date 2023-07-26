namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    public static class CallPlateform
    {
        public static bool PlateformIsUsed;

        public static void Postfix()
        {
            InteractableBehaviour.AllCustomPlateform.Clear();
            InteractableBehaviour.NearestTask = null;

            if (CustomGameOptions.CallPlatform)
            {
                InteractableBehaviour.CreateThisTask(new(5.531f, 9.788f, 1f), new(0f, 0f, 0f), () => UsePlatform(false));
                InteractableBehaviour.CreateThisTask(new(10.148f, 9.806f, 1f), new(0f, 180f, 0f), () => UsePlatform(true));
            }
        }

        private static void UsePlatform(bool isLeft)
        {
            var platform = UObject.FindObjectOfType<MovingPlatformBehaviour>();

            if (platform.IsLeft && !PlateformIsUsed)
                UsePlateforRpc(platform, isLeft);
        }

        public static void SyncPlateform(bool isLeft) => Coroutines.Start(UsePlatform(UObject.FindObjectOfType<MovingPlatformBehaviour>(), isLeft));

        private static void UsePlateforRpc(MovingPlatformBehaviour platform, bool isLeft)
        {
            Coroutines.Start(UsePlatform(platform, isLeft));
            CallRpc(CustomRPC.Misc, MiscRPC.SyncPlateform, isLeft);
        }

        private static IEnumerator UsePlatform(MovingPlatformBehaviour platform, bool isLeft)
        {
            PlateformIsUsed = true;
            platform.IsLeft = isLeft;
            platform.transform.localPosition = platform.IsLeft ? platform.LeftPosition : platform.RightPosition;
            platform.IsDirty = true;

            var sourcePos = platform.IsLeft ? platform.LeftPosition : platform.RightPosition;
            var targetPos = !platform.IsLeft ? platform.LeftPosition : platform.RightPosition;
            yield return Effects.Wait(0.1f);

            yield return Effects.Slide3D(platform.transform, sourcePos, targetPos, CustomPlayer.Local.MyPhysics.Speed);

            platform.IsLeft = !platform.IsLeft;
            yield return Effects.Wait(0.1f);
            PlateformIsUsed = false;
        }
    }

    [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.CanUse))]
    public static class UsePlateformPatch
    {
        public static bool Prefix(ref float __result, PlatformConsole __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)]
            out bool couldUse)
        {
            var num = float.MaxValue;
            var @object = pc.Object;
            couldUse = !pc.IsDead && @object.CanMove && !__instance.Platform.InUse && Vector2.Distance(__instance.Platform.transform.position, __instance.transform.position) < 2f;
            canUse = couldUse;

            if (canUse)
            {
                var truePosition = @object.GetTruePosition();
                var position = __instance.transform.position;
                num = Vector2.Distance(truePosition, position);
                canUse &= num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false);
            }

            __result = num;
            return false;
        }
    }
}