namespace TownOfUsReworked.Classes;

public static class ModCompatibility
{
    public const string SM_GUID = "Submerged";
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

            if (!_submarineStatus || _submarineStatus.WasCollected)
            {
                if (!ShipStatus.Instance || ShipStatus.Instance.WasCollected)
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

    public static bool IsSubmerged => SubLoaded && ShipStatus.Instance && ShipStatus.Instance.Type == SUBMERGED_MAP_TYPE;

    private static Type SubmarineStatusType;
    private static MethodInfo CalculateLightRadiusMethod;

    private static MethodInfo RpcRequestChangeFloorMethod;
    private static Type FloorHandlerType;
    private static MethodInfo GetFloorHandlerMethod;

    private static Type VentPatchDataType;
    private static PropertyInfo InTransitionField;

    private static Type CustomTaskTypesType;
    private static FieldInfo RetrieveOxygenMaskField;

    private static Type SubmarineOxygenSystemType;
    private static PropertyInfo SubmarineOxygenSystemInstanceField;
    private static MethodInfo RepairDamageMethod;
    private static FieldInfo RetTaskType;
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

    private static Type CustomPlayerData;
    private static FieldInfo HasMap;

    private static Type SpawnInState;
    private static FieldInfo CurrentState;

    private static Type SubSpawnSystem;
    private static MethodInfo GetReadyPlayerAmount;

    public static void InitializeSubmerged()
    {
        SubLoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SM_GUID, out var subPlugin);

        if (!SubLoaded)
            return;

        try
        {
            SubPlugin = subPlugin!.Instance as BasePlugin;
            SubVersion = subPlugin.Metadata.Version;
            LogInfo(SubVersion);

            SubAssembly = SubPlugin!.GetType().Assembly;
            SubTypes = AccessTools.GetTypesFromAssembly(SubAssembly);

            SubInjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(SubTypes, t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null,
                Array.Empty<object>());

            SubmarineStatusType = SubTypes.First(t => t.Name == "SubmarineStatus");
            SubmergedInstance = AccessTools.Field(SubmarineStatusType, "instance");
            SubmergedElevators = AccessTools.Field(SubmarineStatusType, "elevators");

            CalculateLightRadiusMethod = AccessTools.Method(SubmarineStatusType, "CalculateLightRadius");

            FloorHandlerType = SubTypes.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", new[] { typeof(PlayerControl) });
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

            VentPatchDataType = SubTypes.First(t => t.Name == "VentPatchData");
            InTransitionField = AccessTools.Property(VentPatchDataType, "InTransition");

            CustomTaskTypesType = SubTypes.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxygenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            RetTaskType = AccessTools.Field(CustomTaskTypesType, "taskType");
            RetrieveOxygenMask = (TaskTypes)RetTaskType.GetValue(RetrieveOxygenMaskField.GetValue(null));

            SubmarineOxygenSystemType = SubTypes.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceField = AccessTools.Property(SubmarineOxygenSystemType, "Instance");

            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");

            SubmergedExileController = SubTypes.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileController, "WrapUpAndSpawn");

            SubmarineElevator = SubTypes.First(t => t.Name == "SubmarineElevator");
            GetInElevator = AccessTools.Method(SubmarineElevator, "GetInElevator", new[] { typeof(PlayerControl) });
            GetMovementStageFromTime = AccessTools.Method(SubmarineElevator, "GetMovementStageFromTime");
            GetSubElevatorSystem = AccessTools.Field(SubmarineElevator, "system");

            SubmarineElevatorSystem = SubTypes.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloor = AccessTools.Field(SubmarineElevatorSystem, "upperDeckIsTargetFloor");

            CustomPlayerData = SubInjectedTypes.Where(t => t.Key == "CustomPlayerData").Select(x => x.Value).First();
            HasMap = AccessTools.Field(CustomPlayerData, "_hasMap");

            SpawnInState = SubTypes.First(t => t.Name == "SpawnInState");

            SubSpawnSystem = SubTypes.First(t => t.Name == "SubmarineSpawnInSystem");
            GetReadyPlayerAmount = AccessTools.Method(SubSpawnSystem, "GetReadyPlayerAmount");
            CurrentState = AccessTools.Field(SubSpawnSystem, "currentState");

            TownOfUsReworked.ModInstance.Harmony.Patch(SubmergedExileWrapUpMethod, null, new(AccessTools.Method(typeof(ModCompatibility), nameof(ExileRoleChangePostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(GetReadyPlayerAmount, new(AccessTools.Method(typeof(ModCompatibility), nameof(ReadyPlayerAmount))));
        }
        catch (Exception e)
        {
            LogError($"Could not load Submerged Compatibility\n{e}");
            SubLoaded = false;
        }
    }

    public static void CheckOutOfBoundsElevator(PlayerControl player)
    {
        if (!IsSubmerged)
            return;

        var (isInElevator, elevator) = GetPlayerElevator(player);

        if (!isInElevator)
            return;

        //True is top, false is bottom
        var currentFloor = (bool)UpperDeckIsTargetFloor.GetValue(GetSubElevatorSystem.GetValue(elevator));
        var playerFloor = player.transform.position.y > -7f;

        if (currentFloor != playerFloor)
            ChangeFloor(currentFloor);
    }

    public static void MoveDeadPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged)
            return;

        var (isInElevator, elevator) = GetPlayerElevator(player);

        if (!isInElevator)
            return;

        if ((int)GetMovementStageFromTime.Invoke(elevator, null) >= 5)
        {
            //Fade to clear
            //True is top, false is bottom
            var topfloortarget = (bool)UpperDeckIsTargetFloor.GetValue(GetSubElevatorSystem.GetValue(elevator));
            var topintendedtarget = player.transform.position.y > -7f;

            if (topfloortarget != topintendedtarget)
                ChangeFloor(!topintendedtarget);
        }
    }

    public static (bool IsInElevator, object Elevator) GetPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged)
            return (false, null);

        foreach (var elevator in (IList)SubmergedElevators.GetValue(SubmergedInstance.GetValue(null)))
        {
            if ((bool)GetInElevator.Invoke(elevator, new[] { player }))
                return (true, elevator);
        }

        return (false, null);
    }

    public static void ExileRoleChangePostfix()
    {
        Coroutines.Start(WaitMeeting(() => ButtonUtils.ResetCustomTimers(CooldownType.Meeting)));
        Coroutines.Start(WaitMeeting(GhostRoleBegin));
        SetPostmortals.ExileControllerPostfix(ExileController.Instance);
    }

    public static IEnumerator WaitStart(Action next)
    {
        while (HUD.UICamera.transform.Find("SpawnInMinigame(Clone)") == null)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        while (HUD.UICamera.transform.Find("SpawnInMinigame(Clone)") != null)
            yield return null;

        next();
    }

    public static IEnumerator WaitMeeting(Action next)
    {
        while (!CustomPlayer.Local.moveable)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        while (HUD.PlayerCam.transform.Find("SpawnInMinigame(Clone)") != null)
            yield return null;

        next();
    }

    public static void GhostRoleBegin()
    {
        if (!IsSubmerged || !CustomPlayer.LocalCustom.IsDead || !CustomPlayer.Local.IsPostmortal() || (CustomPlayer.Local.IsPostmortal() && CustomPlayer.Local.Caught()))
            return;

        var vents = ShipStatus.Instance.AllVents.ToList();
        var clean = PlayerControl.LocalPlayer.myTasks.ToArray().Where(x => x.TaskType == TaskTypes.VentCleaning).ToList();

        if (clean != null)
        {
            var ids = clean.Where(x => !x.IsComplete).ToList().ConvertAll(x => x.FindConsoles()[0].ConsoleId);
            vents = ShipStatus.Instance.AllVents.Where(x => !ids.Contains(x.Id)).ToList();
        }

        var startingVent = vents[URandom.RandomRangeInt(0, vents.Count)];

        while (ShipStatus.Instance.AllVents.IndexOf(startingVent) is 0 or 14)
            startingVent = vents[URandom.RandomRangeInt(0, vents.Count)];

        ChangeFloor(startingVent.transform.position.y > -7f);
        CustomPlayer.Local.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
        CustomPlayer.Local.MyPhysics.RpcEnterVent(startingVent.Id);
    }

    public static void Ghostrolefix(PlayerPhysics __instance)
    {
        if (IsSubmerged && __instance.myPlayer.Data.IsDead)
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
        if (!IsSubmerged)
            return obj.AddComponent<MissingBehaviour>();

        var validType = SubInjectedTypes.TryGetValue(typeName, out var type);
        return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingBehaviour>();
    }

    public static float GetSubmergedNeutralLightRadius(bool isImpostor)
    {
        if (!IsSubmerged)
            return 0;

        return (float)CalculateLightRadiusMethod.Invoke(SubmarineStatus, new object[] { null, true, isImpostor });
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!IsSubmerged)
            return;

        var _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, new[] { CustomPlayer.Local })).TryCast(FloorHandlerType) as MonoBehaviour;
        RpcRequestChangeFloorMethod.Invoke(_floorHandler, new object[] { toUpper });
    }

    public static bool GetInTransition()
    {
        if (!IsSubmerged)
            return false;

        return (bool)InTransitionField.GetValue(null);
    }

    public static void RepairOxygen()
    {
        if (!IsSubmerged)
            return;

        try
        {
            ShipStatus.Instance.RpcRepairSystem((SystemTypes)130, 64);
            RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceField.GetValue(null), new object[] { CustomPlayer.Local, 64 });
        } catch {}
    }

    public static bool ReadyPlayerAmount(dynamic __instance, ref int __result)
    {
        if (!SubLoaded)
            return true;

        if (TownOfUsReworked.MCIActive)
        {
            __result = __instance.GetTotalPlayerAmount();
            Enum.TryParse(SpawnInState, "Done", true, out var e);
            CurrentState.SetValue(__instance, e);
            return false;
        }

        return true;
    }

    public static void ImpartSub(PlayerControl bot)
    {
        var comp = bot?.gameObject?.AddComponent(Il2CppType.From(CustomPlayerData)).TryCast(CustomPlayerData);
        HasMap.SetValue(comp, true);
    }

    public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self,
        Array.Empty<object>());

    public const string LI_GUID = "com.DigiWorm.LevelImposter";
    public const ShipStatus.MapType LI_MAP_TYPE = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version LIVersion { get; private set; }
    public static bool LILoaded { get; private set; }
    public static BasePlugin LIPlugin { get; private set; }

    public static bool IsLIEnabled => LILoaded && ShipStatus.Instance && ShipStatus.Instance.Type == LI_MAP_TYPE;

    public static void InitializeLevelImpostor()
    {
        LILoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(LI_GUID, out var liPlugin);

        if (!LILoaded)
            return;

        try
        {
            LIPlugin = liPlugin!.Instance as BasePlugin;
            LIVersion = liPlugin.Metadata.Version;
            LogInfo(LIVersion);
        }
        catch (Exception e)
        {
            LogError($"Could not load LevelImposter Compatibility\n{e}");
            LILoaded = false;
        }
    }

    public static void Init()
    {
        try
        {
            InitializeLevelImpostor();
            InitializeSubmerged();
        }
        catch (Exception e)
        {
            LogError($"Couldn't load compatibilies:\n{e}");
        }
    }

    public static readonly List<string> Unsupported = new() { "Mini.RegionInstall", "AllTheRoles", "TownOfUs", "TheOtherRoles", "TownOfHost", "Lotus", "LasMonjas", "CrowdedMod" };

    public static void CheckAbort(out bool abort, out string mod)
    {
        abort = false;
        mod = "";

        foreach (var unsupp in Unsupported)
        {
            if (File.Exists($"{TownOfUsReworked.ModsFolder}{unsupp}.dll"))
            {
                abort = true;
                mod = unsupp;
                return;
            }
        }
    }
}