namespace TownOfUsReworked.Classes;

public static class ModCompatibility
{
    public const string SM_GUID = "Submerged";
    public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version SubVersion { get; private set; }
    public static bool SubLoaded { get; private set; }
    public static BasePlugin SubPlugin { get; private set; }
    public static Assembly SubAssembly { get; private set; }
    public static Type[] SubTypes { get; private set; }
    public static Dictionary<string, Type> SubInjectedTypes { get; private set; }

    public static bool IsSubmerged() => SubLoaded && Ship && Ship.Type == SUBMERGED_MAP_TYPE;

    private static Type SubmarineStatusType;

    private static MethodInfo RpcRequestChangeFloorMethod;
    private static Type FloorHandlerType;
    private static MethodInfo GetFloorHandlerMethod;

    private static Type VentPatchDataType;
    private static PropertyInfo InTrasntionProperty;

    private static Type CustomTaskTypesType;
    private static FieldInfo RetrieveOxygenMaskField;

    private static Type SubmarineOxygenSystemType;
    private static PropertyInfo SubmarineOxygenSystemInstanceProperty;
    private static MethodInfo RepairDamageMethod;
    private static FieldInfo RetTaskTypeField;
    public static TaskTypes RetrieveOxygenMask;

    private static Type SubmergedExileControllerType;
    private static MethodInfo SubmergedExileWrapUpMethod;

    private static Type SubmarineElevatorType;
    private static MethodInfo GetInElevatorMethod;
    private static MethodInfo GetMovementStageFromTimeMethod;
    private static FieldInfo GetSubElevatorSystemField;

    private static Type SubmarineElevatorSystemType;
    private static FieldInfo UpperDeckIsTargetFloorField;

    private static FieldInfo SubmergedInstanceField;
    private static FieldInfo SubmergedElevatorsField;

    private static Type CustomPlayerDataType;
    private static FieldInfo HasMapField;

    private static Type SpawnInStateType;
    private static FieldInfo CurrentStateField;

    private static Type SubSpawnSystemType;
    private static MethodInfo GetReadyPlayerAmountMethod;

    public static bool InitializeSubmerged()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(SM_GUID, out var subPlugin) || subPlugin == null)
            return false;

        LogMessage("Submerged was detected");

        try
        {
            SubPlugin = subPlugin!.Instance as BasePlugin;
            SubVersion = subPlugin.Metadata.Version;
            LogInfo(SubVersion);

            SubAssembly = SubPlugin!.GetType().Assembly;
            SubTypes = AccessTools.GetTypesFromAssembly(SubAssembly);

            SubInjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(SubTypes, t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null, null);

            SubmarineStatusType = SubTypes.First(t => t.Name == "SubmarineStatus");
            SubmergedInstanceField = AccessTools.Field(SubmarineStatusType, "instance");
            SubmergedElevatorsField = AccessTools.Field(SubmarineStatusType, "elevators");

            FloorHandlerType = SubTypes.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", new[] { typeof(PlayerControl) });
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

            VentPatchDataType = SubTypes.First(t => t.Name == "VentPatchData");
            InTrasntionProperty = AccessTools.Property(VentPatchDataType, "InTransition");

            CustomTaskTypesType = SubTypes.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxygenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            RetTaskTypeField = AccessTools.Field(CustomTaskTypesType, "taskType");
            RetrieveOxygenMask = (TaskTypes)RetTaskTypeField.GetValue(RetrieveOxygenMaskField.GetValue(null));

            SubmarineOxygenSystemType = SubTypes.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceProperty = AccessTools.Property(SubmarineOxygenSystemType, "Instance");

            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");

            SubmergedExileControllerType = SubTypes.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileControllerType, "WrapUpAndSpawn");

            SubmarineElevatorType = SubTypes.First(t => t.Name == "SubmarineElevator");
            GetInElevatorMethod = AccessTools.Method(SubmarineElevatorType, "GetInElevator", new[] { typeof(PlayerControl) });
            GetMovementStageFromTimeMethod = AccessTools.Method(SubmarineElevatorType, "GetMovementStageFromTime");
            GetSubElevatorSystemField = AccessTools.Field(SubmarineElevatorType, "system");

            SubmarineElevatorSystemType = SubTypes.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloorField = AccessTools.Field(SubmarineElevatorSystemType, "upperDeckIsTargetFloor");

            CustomPlayerDataType = SubInjectedTypes.Where(t => t.Key == "CustomPlayerData").Select(x => x.Value).First();
            HasMapField = AccessTools.Field(CustomPlayerDataType, "_hasMap");

            SpawnInStateType = SubTypes.First(t => t.Name == "SpawnInState");

            SubSpawnSystemType = SubTypes.First(t => t.Name == "SubmarineSpawnInSystem");
            GetReadyPlayerAmountMethod = AccessTools.Method(SubSpawnSystemType, "GetReadyPlayerAmount");
            CurrentStateField = AccessTools.Field(SubSpawnSystemType, "currentState");

            TownOfUsReworked.ModInstance.Harmony.Patch(SubmergedExileWrapUpMethod, null, new(AccessTools.Method(typeof(ModCompatibility), nameof(ExileRoleChangePostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(GetReadyPlayerAmountMethod, new(AccessTools.Method(typeof(ModCompatibility), nameof(ReadyPlayerAmount))));

            LogMessage("Submerged compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            LogError($"Could not load Submerged Compatibility\n{e}");
            return false;
        }
    }

    public static void CheckOutOfBoundsElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return;

        var (isInElevator, elevator) = GetPlayerElevator(player);

        if (!isInElevator)
            return;

        //True is top, false is bottom
        var currentFloor = (bool)UpperDeckIsTargetFloorField.GetValue(GetSubElevatorSystemField.GetValue(elevator));
        var playerFloor = player.transform.position.y > -7f;

        if (currentFloor != playerFloor)
            ChangeFloor(currentFloor);
    }

    public static void MoveDeadPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return;

        var (isInElevator, elevator) = GetPlayerElevator(player);

        if (!isInElevator || elevator == null)
            return;

        if ((int)GetMovementStageFromTimeMethod.Invoke(elevator, null) >= 5)
        {
            //Fade to clear
            //True is top, false is bottom
            var topfloortarget = (bool)UpperDeckIsTargetFloorField.GetValue(GetSubElevatorSystemField.GetValue(elevator));
            var topintendedtarget = player.transform.position.y > -7f;

            if (topfloortarget != topintendedtarget)
                ChangeFloor(!topintendedtarget);
        }
    }

    public static (bool IsInElevator, object Elevator) GetPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return (false, null);

        foreach (var elevator in (IList)SubmergedElevatorsField.GetValue(SubmergedInstanceField.GetValue(null)))
        {
            if ((bool)GetInElevatorMethod.Invoke(elevator, new[] { player }))
                return (true, elevator);
        }

        return (false, null);
    }

    public static void ExileRoleChangePostfix()
    {
        Coroutines.Start(WaitMeeting(() => ButtonUtils.Reset(CooldownType.Meeting)));
        Coroutines.Start(WaitMeeting(GhostRoleBegin));
        SetPostmortals.ExileControllerPostfix(Ejection);
    }

    public static IEnumerator WaitStart(Action next)
    {
        while (HUD.UICamera.transform.Find("SpawnInMinigame(Clone)") == null)
            yield return null;

        yield return Wait(0.5f);

        while (HUD.UICamera.transform.Find("SpawnInMinigame(Clone)") != null)
            yield return null;

        next();
        yield break;
    }

    public static IEnumerator WaitMeeting(Action next)
    {
        while (!CustomPlayer.Local.moveable)
            yield return null;

        yield return Wait(0.5f);

        while (HUD.PlayerCam.transform.Find("SpawnInMinigame(Clone)") != null)
            yield return null;

        next();
        yield break;
    }

    public static void GhostRoleBegin()
    {
        if (!IsSubmerged() || !CustomPlayer.LocalCustom.IsDead || !CustomPlayer.Local.IsPostmortal() || (CustomPlayer.Local.IsPostmortal() && CustomPlayer.Local.Caught()))
            return;

        var vents = AllMapVents;

        if (Ship.Systems.TryGetValue(SystemTypes.Ventilation, out var systemType))
        {
            var ventilationSystem = systemType.Cast<VentilationSystem>();
            vents = AllMapVents.Where(x => ventilationSystem.PlayersCleaningVents.ContainsValue((byte)x.Id)).ToList();
        }

        var startingVent = vents[URandom.RandomRangeInt(0, vents.Count)];

        while (AllMapVents.IndexOf(startingVent) is 0 or 14)
            startingVent = vents[URandom.RandomRangeInt(0, vents.Count)];

        ChangeFloor(startingVent.transform.position.y > -7f);
        CustomPlayer.Local.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
        CustomPlayer.Local.MyPhysics.RpcEnterVent(startingVent.Id);
    }

    public static void Ghostrolefix(PlayerPhysics __instance)
    {
        if (IsSubmerged() && __instance.myPlayer.Data.IsDead)
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
        if (!IsSubmerged())
            return obj.AddComponent<MissingBehaviour>();

        var validType = SubInjectedTypes.TryGetValue(typeName, out var type);
        return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingBehaviour>();
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!IsSubmerged())
            return;

        var _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, new[] { CustomPlayer.Local })).TryCast(FloorHandlerType) as MonoBehaviour;
        RpcRequestChangeFloorMethod.Invoke(_floorHandler, new object[] { toUpper });
    }

    public static bool GetInTransition()
    {
        if (!IsSubmerged())
            return false;

        return (bool)InTrasntionProperty.GetValue(null);
    }

    public static void RepairOxygen()
    {
        if (!IsSubmerged())
            return;

        try
        {
            Ship.RpcUpdateSystem((SystemTypes)130, 64);
            RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceProperty.GetValue(null), new object[] { CustomPlayer.Local, 64 });
        } catch {}
    }

    public static bool ReadyPlayerAmount(dynamic __instance, ref int __result)
    {
        if (!SubLoaded)
            return true;

        if (TownOfUsReworked.MCIActive)
        {
            __result = __instance.GetTotalPlayerAmount();
            Enum.TryParse(SpawnInStateType, "Done", true, out var e);
            CurrentStateField.SetValue(__instance, e);
            return false;
        }

        return true;
    }

    public static void ImpartSub(PlayerControl bot)
    {
        var comp = bot?.gameObject?.AddComponent(Il2CppType.From(CustomPlayerDataType))?.TryCast(CustomPlayerDataType);
        HasMapField.SetValue(comp, true);
    }

    public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, null);

    public const string LI_GUID = "com.DigiWorm.LevelImposter";
    public const ShipStatus.MapType LI_MAP_TYPE = (ShipStatus.MapType)7;

    public static SemanticVersioning.Version LIVersion { get; private set; }
    public static bool LILoaded { get; private set; }
    public static BasePlugin LIPlugin { get; private set; }
    public static Assembly LIAssembly { get; private set; }
    public static Type[] LITypes { get; private set; }

    public static bool IsLevelImpostor() => LILoaded && Ship && Ship.Type == LI_MAP_TYPE;

    private static Type LIShipStatusType;

    private static Type MapLoaderType;
    private static Type LIMapType;
    private static MethodInfo LoadMapMethod;
    private static MethodInfo UnloadMapMethod;

    public static string CurrentLIMap;

    public static bool InitializeLevelImpostor()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(LI_GUID, out var liPlugin) || liPlugin == null)
            return false;

        LogMessage("LevelImpostor was detected");

        try
        {
            LIPlugin = liPlugin!.Instance as BasePlugin;
            LIVersion = liPlugin.Metadata.Version;
            LogInfo(LIVersion);

            LIAssembly = LIPlugin!.GetType().Assembly;
            LITypes = AccessTools.GetTypesFromAssembly(LIAssembly);

            LIShipStatusType = LITypes.First(t => t.Name == "LIShipStatus");

            MapLoaderType = LITypes.First(t => t.Name == "MapLoader");
            LIMapType = LITypes.First(t => t.Name == "LIMap");
            LoadMapMethod = AccessTools.Method(MapLoaderType, "LoadMap", new[] { LIMapType, typeof(bool) });
            UnloadMapMethod = AccessTools.Method(MapLoaderType, "UnloadMap");

            TownOfUsReworked.ModInstance.Harmony.Patch(LoadMapMethod, null, new(AccessTools.Method(typeof(ModCompatibility), nameof(SetCurrentMap))));
            TownOfUsReworked.ModInstance.Harmony.Patch(UnloadMapMethod, null, new(AccessTools.Method(typeof(ModCompatibility), nameof(UnloadMap))));

            LogMessage("LevelImpostor compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            LogError($"Could not load LevelImposter Compatibility\n{e}");
            return false;
        }
    }

    private static void SetCurrentMap(ref dynamic map)
    {
        if (!LILoaded || map == null)
        {
            CurrentLIMap = "LevelImpostor";
            return;
        }

        CurrentLIMap = $"{map.name} <size=0.9>By {map.authorName}</size>";
    }

    private static void UnloadMap() => CurrentLIMap = "LevelImpostor";

    public static void Init()
    {
        try
        {
            LILoaded = InitializeLevelImpostor();
            SubLoaded = InitializeSubmerged();
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