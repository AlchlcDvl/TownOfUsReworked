namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class ModCompatibility
    {
        public const string SUBMERGED_GUID = "Submerged";
        public const string ElevatorMover = "ElevatorMover";
        public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)5;

        public static SemanticVersioning.Version SubVersion { get; private set; }
        public static bool SubLoaded { get; private set; }
        public static BasePlugin SubPlugin { get; private set; }
        public static Assembly SubAssembly { get; private set; }
        public static Type[] SubTypes { get; private set; }
        public static Dictionary<string, Type> SubInjectedTypes { get; private set; }

        private static MonoBehaviour _submarineStatus;

        public static MonoBehaviour SubmarineStatus
        {
            get
            {
                if (!SubLoaded)
                    return null;

                if (_submarineStatus?.WasCollected == true || !_submarineStatus || _submarineStatus == null)
                {
                    if (ShipStatus.Instance?.WasCollected == true || !ShipStatus.Instance || ShipStatus.Instance == null)
                        return _submarineStatus = null;
                    else if (ShipStatus.Instance.Type == SUBMERGED_MAP_TYPE)
                        return _submarineStatus = ShipStatus.Instance.GetComponent(Il2CppType.From(SubmarineStatusType))?.TryCast(SubmarineStatusType) as MonoBehaviour;
                    else
                        return _submarineStatus = null;
                }
                else
                    return _submarineStatus;
            }
        }

        public static bool DisableO2MaskCheckForEmergency
        {
            set
            {
                if (!SubLoaded)
                    return;

                DisableO2MaskCheckField.SetValue(null, value);
            }
        }

        public static bool IsSubmerged => SubLoaded && ShipStatus.Instance && ShipStatus.Instance.Type == SUBMERGED_MAP_TYPE;

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

        private static Type SubmarineOxygenSystemType;
        private static FieldInfo SubmarineOxygenSystemInstanceField;
        private static MethodInfo RepairDamageMethod;
        public static TaskTypes RetrieveOxygenMask;

        private static Type SubmergedExileController;
        private static MethodInfo SubmergedExileWrapUpMethod;

        private static Type SubmarineElevator;
        private static MethodInfo GetInElevator;
        private static MethodInfo GetMovementStageFromTime;
        private static FieldInfo GetSubElevatorSystem;

        private static Type SubmarineElevatorSystem;
        private static FieldInfo UpperDeckIsTargetFloor;

        private static FieldInfo SubmergedInstance;
        private static FieldInfo SubmergedElevators;

        private readonly static Harmony SubHarmony = new("tourew.subcompat.patch");

        public static void InitializeSubmerged()
        {
            SubLoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out var subPlugin);

            if (!SubLoaded)
                return;

            SubPlugin = subPlugin!.Instance as BasePlugin;
            SubVersion = subPlugin.Metadata.Version;
            Utils.LogSomething(SubVersion);

            SubAssembly = SubPlugin!.GetType().Assembly;
            SubTypes = AccessTools.GetTypesFromAssembly(SubAssembly);

            SubInjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(SubTypes, t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null,
                Array.Empty<object>());

            SubmarineStatusType = SubTypes.First(t => t.Name == "SubmarineStatus");
            SubmergedInstance = AccessTools.Field(SubmarineStatusType, "Instance");
            SubmergedElevators = AccessTools.Field(SubmarineStatusType, "Elevators");

            CalculateLightRadiusMethod = AccessTools.Method(SubmarineStatusType, "CalculateLightRadius");

            TaskIsEmergencyPatchType = SubTypes.First(t => t.Name == "PlayerTask_TaskIsEmergency_Patch");
            DisableO2MaskCheckField = AccessTools.Field(TaskIsEmergencyPatchType, "DisableO2MaskCheck");

            FloorHandlerType = SubTypes.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", new[] { typeof(PlayerControl) });
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

            Vent_MoveToVent_PatchType = SubTypes.First(t => t.Name == "Vent_MoveToVent_Patch");
            InTransitionField = AccessTools.Field(Vent_MoveToVent_PatchType, "InTransition");

            CustomTaskTypesType = SubTypes.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxigenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            RetrieveOxygenMask = (TaskTypes)RetrieveOxigenMaskField.GetValue(null);

            SubmarineOxygenSystemType = SubTypes.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceField = AccessTools.Field(SubmarineOxygenSystemType, "Instance");

            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");

            SubmergedExileController = SubTypes.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileController, "WrapUpAndSpawn");

            SubmarineElevator = SubTypes.First(t => t.Name == "SubmarineElevator");
            GetInElevator = AccessTools.Method(SubmarineElevator, "GetInElevator", new[] { typeof(PlayerControl) });
            GetMovementStageFromTime = AccessTools.Method(SubmarineElevator, "GetMovementStageFromTime");
            GetSubElevatorSystem = AccessTools.Field(SubmarineElevator, "System");

            SubmarineElevatorSystem = SubTypes.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloor = AccessTools.Field(SubmarineElevatorSystem, "UpperDeckIsTargetFloor");

            //I tried patching normally but it would never work
            SubHarmony.Patch(SubmergedExileWrapUpMethod, null, new(SymbolExtensions.GetMethodInfo(() => ExileRoleChangePostfix())));
        }

        public static void CheckOutOfBoundsElevator(PlayerControl player)
        {
            if (!SubLoaded || IsSubmerged)
                return;

            var elevator = GetPlayerElevator(player);

            if (!elevator.Item1)
                return;

            //True is top, false is bottom
            var currentFloor = (bool)UpperDeckIsTargetFloor.GetValue(GetSubElevatorSystem.GetValue(elevator.Item2));
            var playerFloor = player.transform.position.y > -7f;

            if (currentFloor != playerFloor)
                ChangeFloor(currentFloor);
        }

        public static void MoveDeadPlayerElevator(PlayerControl player)
        {
            if (!IsSubmerged)
                return;

            var elevator = GetPlayerElevator(player);

            if (!elevator.Item1)
                return;

            if ((int)GetMovementStageFromTime.Invoke(elevator.Item2, null) >= 5)
            {
                //Fade to clear
                //True is top, false is bottom
                var topfloortarget = (bool)UpperDeckIsTargetFloor.GetValue(GetSubElevatorSystem.GetValue(elevator.Item2));
                var topintendedtarget = player.transform.position.y > -7f;

                if (topfloortarget != topintendedtarget)
                    ChangeFloor(!topintendedtarget);
            }
        }

        public static Tuple<bool, object> GetPlayerElevator(PlayerControl player)
        {
            if (!IsSubmerged)
                return Tuple.Create(false, (object)null);

            foreach (var elevator in (IList)SubmergedElevators.GetValue(SubmergedInstance.GetValue(null)))
            {
                if ((bool)GetInElevator.Invoke(elevator, new[] { player }))
                    return Tuple.Create(true, elevator);
            }

            return Tuple.Create(false, (object)null);
        }

        public static void ExileRoleChangePostfix()
        {
            Coroutines.Start(WaitMeeting(() => ButtonUtils.ResetCustomTimers(false)));
            Coroutines.Start(WaitMeeting(GhostRoleBegin));
            SetPostmortals.ExileControllerPostfix(ExileController.Instance);
        }

        public static IEnumerator WaitStart(Action next)
        {
            while (HudManager.Instance.UICamera.transform.Find("SpawnInMinigame(Clone)") == null)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            while (HudManager.Instance.UICamera.transform.Find("SpawnInMinigame(Clone)") != null)
                yield return null;

            next();
        }

        public static IEnumerator WaitMeeting(Action next)
        {
            while (!PlayerControl.LocalPlayer.moveable)
                yield return null;

            yield return new WaitForSeconds(0.5f);

            while (HudManager.Instance.PlayerCam.transform.Find("SpawnInMinigame(Clone)") != null)
                yield return null;

            next();
        }

        public static void GhostRoleBegin()
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (PlayerControl.LocalPlayer.IsPostmortal() && !PlayerControl.LocalPlayer.Caught())
            {
                var startingVent = ShipStatus.Instance.AllVents[URandom.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                while (startingVent == ShipStatus.Instance.AllVents[0] || startingVent == ShipStatus.Instance.AllVents[14])
                    startingVent = ShipStatus.Instance.AllVents[URandom.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

                ChangeFloor(startingVent.transform.position.y > -7f);
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
                writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                writer2.Write(startingVent.transform.position);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
            }
        }

        public static void Ghostrolefix(PlayerPhysics __instance)
        {
            if (SubLoaded && __instance.myPlayer.Data.IsDead)
            {
                var player = __instance.myPlayer;

                if (player.IsPostmortal() && !player.Caught())
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

        public static MonoBehaviour AddSubmergedComponent(this GameObject obj, string typeName)
        {
            if (!SubLoaded)
                return obj.AddComponent<MissingSubmergedBehaviour>();

            var validType = SubInjectedTypes.TryGetValue(typeName, out Type type);
            return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingSubmergedBehaviour>();
        }

        public static float GetSubmergedNeutralLightRadius(bool isImpostor)
        {
            if (!SubLoaded)
                return 0;

            return (float)CalculateLightRadiusMethod.Invoke(SubmarineStatus, new object[] { null, true, isImpostor });
        }

        public static void ChangeFloor(bool toUpper)
        {
            if (!SubLoaded)
                return;

            var _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, new[] { PlayerControl.LocalPlayer })).TryCast(FloorHandlerType) as MonoBehaviour;
            RpcRequestChangeFloorMethod.Invoke(_floorHandler, new object[] { toUpper });
        }

        public static bool GetInTransition()
        {
            if (!SubLoaded)
                return false;

            return (bool)InTransitionField.GetValue(null);
        }

        public static void RepairOxygen()
        {
            if (!SubLoaded)
                return;

            try
            {
                ShipStatus.Instance.RpcRepairSystem((SystemTypes)130, 64);
                RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceField.GetValue(null), new object[] { PlayerControl.LocalPlayer, 64 });
            } catch (NullReferenceException) {}
        }

        public const string LI_GUID = "com.DigiWorm.LevelImposter";
        public const ShipStatus.MapType LI_MAP_TYPE = (ShipStatus.MapType)6;

        public static SemanticVersioning.Version LIVersion { get; private set; }
        public static bool LILoaded { get; private set; }
        public static BasePlugin LIPlugin { get; private set; }
        /*public static Assembly LIAssembly { get; private set; }
        public static Type[] LITypes { get; private set; }
        public static Dictionary<string, Type> LIInjectedTypes { get; private set; }*/

        public static bool IsLIEnabled => LILoaded && ShipStatus.Instance && ShipStatus.Instance.Type == LI_MAP_TYPE;

        /*private static MonoBehaviour _liStatus;

        public static MonoBehaviour LevelImpostorStatus
        {
            get
            {
                if (!LILoaded)
                    return null;

                if (_liStatus?.WasCollected == true || !_liStatus || _liStatus == null)
                {
                    if (ShipStatus.Instance?.WasCollected == true || !ShipStatus.Instance || ShipStatus.Instance == null)
                        return _liStatus = null;
                    else if (ShipStatus.Instance.Type == LI_MAP_TYPE)
                        return _liStatus = ShipStatus.Instance.GetComponent(Il2CppType.From(LIStatusType))?.TryCast(LIStatusType) as MonoBehaviour;
                    else
                        return _liStatus = null;
                }
                else
                    return _liStatus;
            }
        }

        private static Type LIStatusType;

        private static Type ChangeMapsType;
        private static MethodInfo ChangeMapsMethod;

        private readonly static Harmony LIHarmony = new("tourew.licompat.patch");*/

        public static void InitializeLevelImpostor()
        {
            LILoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(LI_GUID, out var liPlugin);

            if (!LILoaded)
                return;

            LIPlugin = liPlugin!.Instance as BasePlugin;
            LIVersion = liPlugin.Metadata.Version;
            Utils.LogSomething(LIVersion);

            /*LIAssembly = SubPlugin!.GetType().Assembly;
            LITypes = AccessTools.GetTypesFromAssembly(LIAssembly);

            LIInjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(LITypes, t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null,
                Array.Empty<object>());

            LIStatusType = LITypes.First(t => t.Name == "LIShipStatus");

            ChangeMapsType = LITypes.First(t => t.Name == "MapNameValuePatch");
            ChangeMapsMethod = AccessTools.Method(ChangeMapsType, "IncrementFix");

            //I have no idea what I'm doing lmao
            //LIHarmony.Patch(ChangeMapsMethod, null, new(SymbolExtensions.GetMethodInfo(() => SetLIMap())));*/
        }

        /*public static MonoBehaviour AddLevelImpostorComponent(this GameObject obj, string typeName)
        {
            if (!LILoaded)
                return obj.AddComponent<MissingLIBehaviour>();

            var validType = LIInjectedTypes.TryGetValue(typeName, out Type type);
            return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingLIBehaviour>();
        }*/

        public static void SetLIMap() => Generate.Map.Set(Generate.Map.Values.Length - 1);

        public const string RD_GUID = "gg.reactor.debugger";
    }
}