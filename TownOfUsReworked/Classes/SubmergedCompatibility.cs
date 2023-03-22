﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod;
using Hazel;

namespace TownOfUsReworked.Classes
{
    public static class SubmergedCompatibility
    {
        public const string SUBMERGED_GUID = "Submerged";
        public const string ElevatorMover = "ElevatorMover";
        public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)5;

        public static SemanticVersioning.Version Version { get; private set; }
        public static bool Loaded { get; private set; }
        public static BasePlugin Plugin { get; private set; }
        public static Assembly Assembly { get; private set; }
        public static Type[] Types { get; private set; }
        public static Dictionary<string, Type> InjectedTypes { get; private set; }

        private static MonoBehaviour _submarineStatus;

        public static MonoBehaviour SubmarineStatus
        {
            get
            {
                if (!Loaded)
                    return null;

                if (_submarineStatus is null || _submarineStatus.WasCollected || !_submarineStatus || _submarineStatus == null)
                {
                    if (ShipStatus.Instance is null || ShipStatus.Instance.WasCollected || !ShipStatus.Instance || ShipStatus.Instance == null)
                        return _submarineStatus = null;
                    else
                    {
                        if (ShipStatus.Instance.Type == SUBMERGED_MAP_TYPE)
                            return _submarineStatus = ShipStatus.Instance.GetComponent(Il2CppType.From(SubmarineStatusType))?.TryCast(SubmarineStatusType) as MonoBehaviour;
                        else
                            return _submarineStatus = null;
                    }
                }
                else
                    return _submarineStatus;
            }
        }

        public static bool DisableO2MaskCheckForEmergency
        {
            set
            {
                if (!Loaded)
                    return;

                DisableO2MaskCheckField.SetValue(null, value);
            }
        }

        private static Type SubmarineStatusType;
        private static MethodInfo CalculateLightRadiusMethod;

        private static Type TaskIsEmergencyPatchType;
        private static FieldInfo DisableO2MaskCheckField;

        private static MethodInfo RpcRequestChangeFloorMethod;
        private static Type FloorHandlerType;
        private static MethodInfo GetFloorHandlerMethod;

        private static Type Vent_MoveToVent_PatchType;
        private static FieldInfo InTransitionField;

        private static Type CustomTaskTypesType;
        private static FieldInfo RetrieveOxigenMaskField;
        public static TaskTypes RetrieveOxygenMask;
        private static Type SubmarineOxygenSystemType;
        private static FieldInfo SubmarineOxygenSystemInstanceField;
        private static MethodInfo RepairDamageMethod;

        private static Type SubmergedExileController;
        private static MethodInfo SubmergedExileWrapUpMethod;

        private static Type SubmarineElevator;
        private static MethodInfo GetInElevator;
        private static MethodInfo GetMovementStageFromTime;
        private static FieldInfo getSubElevatorSystem;

        private static Type SubmarineElevatorSystem;
        private static FieldInfo UpperDeckIsTargetFloor; 

        private static FieldInfo SubmergedInstance;
        private static FieldInfo SubmergedElevators;

        public static void Initialize()
        {
            Loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out PluginInfo plugin);

            if (!Loaded)
                return;

            Plugin = plugin!.Instance as BasePlugin;
            Version = plugin.Metadata.Version;

            Assembly = Plugin!.GetType().Assembly;
            Types = AccessTools.GetTypesFromAssembly(Assembly);

            InjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Types.FirstOrDefault(t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null,
                Array.Empty<object>());

            SubmarineStatusType = Types.First(t => t.Name == "SubmarineStatus");
            SubmergedInstance = AccessTools.Field(SubmarineStatusType, "Instance");
            SubmergedElevators = AccessTools.Field(SubmarineStatusType, "Elevators");

            CalculateLightRadiusMethod = AccessTools.Method(SubmarineStatusType, "CalculateLightRadius");

            TaskIsEmergencyPatchType = Types.First(t => t.Name == "PlayerTask_TaskIsEmergency_Patch");
            DisableO2MaskCheckField = AccessTools.Field(TaskIsEmergencyPatchType, "DisableO2MaskCheck");

            FloorHandlerType = Types.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", new Type[] { typeof(PlayerControl) });
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

            Vent_MoveToVent_PatchType = Types.First(t => t.Name == "Vent_MoveToVent_Patch");
            InTransitionField = AccessTools.Field(Vent_MoveToVent_PatchType, "InTransition");

            CustomTaskTypesType = Types.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxigenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            RetrieveOxygenMask = (TaskTypes)RetrieveOxigenMaskField.GetValue(null);

            SubmarineOxygenSystemType = Types.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceField = AccessTools.Field(SubmarineOxygenSystemType, "Instance");
            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");
            SubmergedExileController = Types.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileController, "WrapUpAndSpawn");

            SubmarineElevator = Types.First(t => t.Name == "SubmarineElevator");
            GetInElevator = AccessTools.Method(SubmarineElevator, "GetInElevator", new Type[] { typeof(PlayerControl) });
            GetMovementStageFromTime = AccessTools.Method(SubmarineElevator, "GetMovementStageFromTime");
            getSubElevatorSystem = AccessTools.Field(SubmarineElevator, "System");

            SubmarineElevatorSystem = Types.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloor = AccessTools.Field(SubmarineElevatorSystem, "UpperDeckIsTargetFloor");

            //I tried patching normally but it would never work
            Harmony _harmony = new Harmony("tou.submerged.patch");
            var exilerolechangePostfix = SymbolExtensions.GetMethodInfo(() => ExileRoleChangePostfix());
            _harmony.Patch(SubmergedExileWrapUpMethod, null, new HarmonyMethod(exilerolechangePostfix));
        }

        public static void CheckOutOfBoundsElevator(PlayerControl player)
        {
            if (!Loaded)
                return;

            if (!isSubmerged())
                return;

            var elevator = GetPlayerElevator(player);

            if (!elevator.Item1)
                return;

            var CurrentFloor = (bool)UpperDeckIsTargetFloor.GetValue(getSubElevatorSystem.GetValue(elevator.Item2)); //true is top, false is bottom
            var PlayerFloor = player.transform.position.y > -7f; //true is top, false is bottom

            if (CurrentFloor != PlayerFloor)
                ChangeFloor(CurrentFloor);
        }

        public static void MoveDeadPlayerElevator(PlayerControl player)
        {
            if (!isSubmerged())
                return;

            var elevator = GetPlayerElevator(player);

            if (!elevator.Item1)
                return;

            var MovementStage = (int)GetMovementStageFromTime.Invoke(elevator.Item2, null);

            if (MovementStage >= 5)
            {
                //Fade to clear
                var topfloortarget = (bool)UpperDeckIsTargetFloor.GetValue(getSubElevatorSystem.GetValue(elevator.Item2)); //true is top, false is bottom
                var topintendedtarget = player.transform.position.y > -7f; //true is top, false is bottom

                if (topfloortarget != topintendedtarget)
                    ChangeFloor(!topintendedtarget);
            }
        }

        public static Tuple<bool, object> GetPlayerElevator(PlayerControl player)
        {
            if (!isSubmerged())
                return Tuple.Create(false, (object)null);

            IList elevatorlist = Utils.CreateList(SubmarineElevator);
            elevatorlist = (IList)SubmergedElevators.GetValue(SubmergedInstance.GetValue(null));

            foreach (object elevator in elevatorlist)
            {
                if ((bool)GetInElevator.Invoke(elevator, new object[] { player }))
                    return Tuple.Create(true, elevator);
            }

            return Tuple.Create(false, (object)null);
        }

        public static void ExileRoleChangePostfix()
        {
            Coroutines.Start(waitMeeting(resetTimers));
            Coroutines.Start(waitMeeting(GhostRoleBegin));

            SetPhantom.ExileControllerPostfix(ExileController.Instance);
            SetRevealer.ExileControllerPostfix(ExileController.Instance);
            SetGhoul.ExileControllerPostfix(ExileController.Instance);
            SetBanshee.ExileControllerPostfix(ExileController.Instance);
        }

        public static IEnumerator waitStart(Action next)
        {
            while (HudManager.Instance.UICamera.transform.Find("SpawnInMinigame(Clone)") == null)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            while (HudManager.Instance.UICamera.transform.Find("SpawnInMinigame(Clone)") != null)
                yield return null;

            next();
        }

        public static IEnumerator waitMeeting(Action next)
        {
            while (!PlayerControl.LocalPlayer.moveable)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            while (HudManager.Instance.PlayerCam.transform.Find("SpawnInMinigame(Clone)") != null)
                yield return null;

            next();
        }

        public static void resetTimers()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            Utils.ResetCustomTimers();
        }

        public static void GhostRoleBegin()
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
            {
                if (!Role.GetRole<Revealer>(PlayerControl.LocalPlayer).Caught)
                {
                    var startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    while (startingVent == ShipStatus.Instance.AllVents[0] || startingVent == ShipStatus.Instance.AllVents[14])
                        startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    ChangeFloor(startingVent.transform.position.y > -7f);

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(startingVent.transform.position.x);
                    writer2.Write(startingVent.transform.position.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
            {
                if (!Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
                {
                    var startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    while (startingVent == ShipStatus.Instance.AllVents[0] || startingVent == ShipStatus.Instance.AllVents[14])
                        startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    ChangeFloor(startingVent.transform.position.y > -7f);

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(startingVent.transform.position.x);
                    writer2.Write(startingVent.transform.position.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
            {
                if (!Role.GetRole<Ghoul>(PlayerControl.LocalPlayer).Caught)
                {
                    var startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    while (startingVent == ShipStatus.Instance.AllVents[0] || startingVent == ShipStatus.Instance.AllVents[14])
                        startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    ChangeFloor(startingVent.transform.position.y > -7f);

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(startingVent.transform.position.x);
                    writer2.Write(startingVent.transform.position.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Banshee))
            {
                if (!Role.GetRole<Banshee>(PlayerControl.LocalPlayer).Caught)
                {
                    var startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    while (startingVent == ShipStatus.Instance.AllVents[0] || startingVent == ShipStatus.Instance.AllVents[14])
                        startingVent = ShipStatus.Instance.AllVents[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                    ChangeFloor(startingVent.transform.position.y > -7f);

                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(startingVent.transform.position.x);
                    writer2.Write(startingVent.transform.position.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                    PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
                }
            }
        }

        public static void Ghostrolefix(PlayerPhysics __instance)
        {
            if (Loaded && __instance.myPlayer.Data.IsDead)
            {
                var player = __instance.myPlayer;

                if (player.Is(RoleEnum.Phantom))
                {
                    if (!Role.GetRole<Phantom>(player).Caught)
                    {
                        if (player.AmOwner) 
                            MoveDeadPlayerElevator(player);
                        else
                            player.Collider.enabled = false;

                        var transform = __instance.transform;
                        var position = transform.position;
                        position.z = position.y/1000;

                        transform.position = position;
                        __instance.myPlayer.gameObject.layer = 8;
                    }
                }
                else if (player.Is(RoleEnum.Revealer))
                {
                    if (!Role.GetRole<Revealer>(player).Caught)
                    {
                        if (player.AmOwner)
                            MoveDeadPlayerElevator(player);
                        else
                            player.Collider.enabled = false;

                        var transform = __instance.transform;
                        var position = transform.position;
                        position.z = position.y / 1000;

                        transform.position = position;
                        __instance.myPlayer.gameObject.layer = 8;
                    }
                }
                else if (player.Is(RoleEnum.Banshee))
                {
                    if (!Role.GetRole<Banshee>(player).Caught)
                    {
                        if (player.AmOwner)
                            MoveDeadPlayerElevator(player);
                        else
                            player.Collider.enabled = false;

                        var transform = __instance.transform;
                        var position = transform.position;
                        position.z = position.y / 1000;

                        transform.position = position;
                        __instance.myPlayer.gameObject.layer = 8;
                    }
                }
                else if (player.Is(RoleEnum.Ghoul))
                {
                    if (!Role.GetRole<Ghoul>(player).Caught)
                    {
                        if (player.AmOwner)
                            MoveDeadPlayerElevator(player);
                        else
                            player.Collider.enabled = false;

                        var transform = __instance.transform;
                        var position = transform.position;
                        position.z = position.y / 1000;

                        transform.position = position;
                        __instance.myPlayer.gameObject.layer = 8;
                    }
                }
            }
        }
        public static MonoBehaviour AddSubmergedComponent(this GameObject obj, string typeName)
        {
            if (!Loaded)
                return obj.AddComponent<MissingSubmergedBehaviour>();

            var validType = InjectedTypes.TryGetValue(typeName, out Type type);
            return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingSubmergedBehaviour>();
        }

        public static float GetSubmergedNeutralLightRadius(bool isImpostor)
        {
            if (!Loaded)
                return 0;

            return (float)CalculateLightRadiusMethod.Invoke(SubmarineStatus, new object[] { null, true, isImpostor });
        }

        public static void ChangeFloor(bool toUpper)
        {
            if (!Loaded)
                return;

            var _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, new object[] { PlayerControl.LocalPlayer })).TryCast(FloorHandlerType) as MonoBehaviour;
            RpcRequestChangeFloorMethod.Invoke(_floorHandler, new object[] { toUpper });
        }

        public static bool getInTransition()
        {
            if (!Loaded)
                return false;

            return (bool)InTransitionField.GetValue(null);
        }


        public static void RepairOxygen()
        {
            if (!Loaded)
                return;

            try
            {
                ShipStatus.Instance.RpcRepairSystem((SystemTypes)130, 64);
                RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceField.GetValue(null), new object[] { PlayerControl.LocalPlayer, 64 });
            } catch (System.NullReferenceException) {}
        }

        public static bool isSubmerged() => Loaded && ShipStatus.Instance && ShipStatus.Instance.Type == SUBMERGED_MAP_TYPE;
    }

    public class MissingSubmergedBehaviour : MonoBehaviour
    {
        static MissingSubmergedBehaviour() => ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
        public MissingSubmergedBehaviour(IntPtr ptr) : base(ptr) { }
    }
}