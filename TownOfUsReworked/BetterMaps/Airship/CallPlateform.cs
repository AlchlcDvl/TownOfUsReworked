using HarmonyLib;
using Hazel;
using System.Collections;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    public static class CallPlateform
    {
        public static bool PlateformIsUsed = false;
        public static PlatformConsole PlateformConsole = Object.FindObjectOfType<PlatformConsole>();

        public static void Postfix(AirshipStatus __instance)
        {
            Tasks.AllCustomPlateform.Clear();
            Tasks.NearestTask = null;

            if (CustomGameOptions.CallPlatform)
            {
                Tasks.CreateThisTask(new Vector3(5.531f, 9.788f, 1f), new Vector3(0f, 0f, 0f), () =>
                {
                    var Plateform = Object.FindObjectOfType<MovingPlatformBehaviour>();

                    if (!Plateform.IsLeft && !PlateformIsUsed)
                        UsePlateforRpc(Plateform, false);
                });

                Tasks.CreateThisTask(new Vector3(10.148f, 9.806f, 1f), new Vector3(0f, 180f, 0f), () =>
                {
                    var Plateform = Object.FindObjectOfType<MovingPlatformBehaviour>();

                    if (Plateform.IsLeft && !PlateformIsUsed)
                        UsePlateforRpc(Plateform, true);
                });
            }
        }

        public static void SyncPlateform(bool isLeft)
        {
            var Plateform = Object.FindObjectOfType<MovingPlatformBehaviour>();
            Coroutines.Start(UsePlatform(Plateform, isLeft));
        }

        private static void UsePlateforRpc(MovingPlatformBehaviour Plateform, bool isLeft)
        {
            var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncPlateform, SendOption.None);
            messageWriter.Write(isLeft);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            Coroutines.Start(UsePlatform(Plateform, isLeft));
        }

        private static IEnumerator UsePlatform(MovingPlatformBehaviour Plateform, bool isLeft)
        {
            PlateformIsUsed = true;
            Plateform.IsLeft = isLeft;
            Plateform.transform.localPosition = (Plateform.IsLeft ? Plateform.LeftPosition : Plateform.RightPosition);
            Plateform.IsDirty = true;

            var sourcePos = Plateform.IsLeft ? Plateform.LeftPosition : Plateform.RightPosition;
            var targetPos = (!Plateform.IsLeft) ? Plateform.LeftPosition : Plateform.RightPosition;
            yield return Effects.Wait(0.1f);

            yield return Effects.Slide3D(Plateform.transform, sourcePos, targetPos, PlayerControl.LocalPlayer.MyPhysics.Speed);

            Plateform.IsLeft = !Plateform.IsLeft;
            yield return Effects.Wait(0.1f);
            PlateformIsUsed = false;

            yield break;
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
            couldUse = (!CallPlateform.PlateformIsUsed && !pc.IsDead && @object.CanMove && !__instance.Platform.InUse && Vector2.Distance(__instance.Platform.transform.position,
                __instance.transform.position) < 2f);
            canUse = couldUse;

            if (canUse)
            {
                var truePosition = @object.GetTruePosition();
                var position = __instance.transform.position;
                num = Vector2.Distance(truePosition, position);
                canUse &= (num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false));
            }

            __result = num;
            return false;
        }
    }
}