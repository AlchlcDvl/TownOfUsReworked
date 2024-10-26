namespace TownOfUsReworked.Classes;

public static class ModCompatibility
{
    public const string SM_GUID = "Submerged";
    public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version SubVersion { get; private set; }
    public static bool SubLoaded { get; set; }
    private static BasePlugin SubPlugin { get; set; }
    private static Assembly SubAssembly { get; set; }
    private static Type[] SubTypes { get; set; }
    private static Dictionary<string, Type> SubInjectedTypes { get; set; }

    public static bool IsSubmerged() => SubLoaded && Ship() && Ship().Type == SUBMERGED_MAP_TYPE && MapPatches.CurrentMap == 6;

    private static Type SubmarineStatusType;

    private static MethodInfo RpcRequestChangeFloorMethod;
    private static MethodInfo RegisterFloorOverrideMethod;
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

        Message("Submerged was detected");

        try
        {
            SubPlugin = subPlugin.Instance as BasePlugin;
            SubVersion = subPlugin.Metadata.Version;
            Message(SubVersion);

            SubAssembly = SubPlugin.GetType().Assembly;
            SubTypes = AccessTools.GetTypesFromAssembly(SubAssembly);

            SubInjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(SubTypes, t => t.Name == "ComponentExtensions"), "RegisteredTypes").Invoke(null, null);

            SubmarineStatusType = SubTypes.First(t => t.Name == "SubmarineStatus");
            SubmergedInstanceField = AccessTools.Field(SubmarineStatusType, "instance");
            SubmergedElevatorsField = AccessTools.Field(SubmarineStatusType, "elevators");

            FloorHandlerType = SubTypes.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", [ typeof(PlayerControl) ]);
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");
            RegisterFloorOverrideMethod = AccessTools.Method(FloorHandlerType, "RegisterFloorOverride");

            VentPatchDataType = SubTypes.First(t => t.Name == "VentPatchData");
            InTrasntionProperty = AccessTools.Property(VentPatchDataType, "InTransition");

            CustomTaskTypesType = SubTypes.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxygenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            RetTaskTypeField = AccessTools.Field(CustomTaskTypesType, "taskType");
            RetrieveOxygenMask = RetTaskTypeField.GetValue<TaskTypes>(RetrieveOxygenMaskField.GetValue(null));

            SubmarineOxygenSystemType = SubTypes.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceProperty = AccessTools.Property(SubmarineOxygenSystemType, "Instance");

            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");

            SubmergedExileControllerType = SubTypes.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileControllerType, "WrapUpAndSpawn");

            SubmarineElevatorType = SubTypes.First(t => t.Name == "SubmarineElevator");
            GetInElevatorMethod = AccessTools.Method(SubmarineElevatorType, "GetInElevator", [ typeof(PlayerControl) ]);
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

            var compatType = typeof(ModCompatibility);

            TownOfUsReworked.ModInstance.Harmony.Patch(SubmergedExileWrapUpMethod, null, new(AccessTools.Method(compatType, nameof(ExileRoleChangePostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(GetReadyPlayerAmountMethod, new(AccessTools.Method(compatType, nameof(ReadyPlayerAmount))));

            Message("Submerged compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            Error($"Could not load Submerged Compatibility\n{e}");
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

        // True is top, false is bottom
        var currentFloor = UpperDeckIsTargetFloorField.GetValue<bool>(GetSubElevatorSystemField.GetValue(elevator));
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
            // Fade to clear
            // True is top, false is bottom
            var topfloortarget = UpperDeckIsTargetFloorField.GetValue<bool>(GetSubElevatorSystemField.GetValue(elevator));
            var topintendedtarget = player.transform.position.y > -7f;

            if (topfloortarget != topintendedtarget)
                ChangeFloor(!topintendedtarget);
        }
    }

    public static (bool IsInElevator, object Elevator) GetPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return (false, null);

        foreach (var elevator in SubmergedElevatorsField.GetValue<IList>(SubmergedInstanceField.GetValue(null)))
        {
            if ((bool)GetInElevatorMethod.Invoke(elevator, [ player ]))
                return (true, elevator);
        }

        return (false, null);
    }

    public static void ExileRoleChangePostfix()
    {
        Coroutines.Start(WaitMeeting(() => ButtonUtils.Reset(CooldownType.Meeting)));
        SetPostmortals.ExileControllerPostfix(Ejection());
    }

    public static IEnumerator WaitStart(Action next)
    {
        while (!HUD().UICamera.transform.Find("SpawnInMinigame(Clone)"))
            yield return EndFrame();

        yield return Wait(0.5f);

        while (HUD().UICamera.transform.Find("SpawnInMinigame(Clone)"))
            yield return EndFrame();

        next();
        yield break;
    }

    public static IEnumerator WaitMeeting(Action next)
    {
        while (!CustomPlayer.Local.moveable)
            yield return EndFrame();

        yield return Wait(0.5f);

        while (HUD().PlayerCam.transform.Find("SpawnInMinigame(Clone)"))
            yield return EndFrame();

        next();
        yield break;
    }

    public static void GhostRoleFix(PlayerPhysics __instance)
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

        var _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, [ CustomPlayer.Local ])).TryCast(FloorHandlerType) as MonoBehaviour;
        RpcRequestChangeFloorMethod.Invoke(_floorHandler, [ toUpper ]);
        RegisterFloorOverrideMethod.Invoke(_floorHandler, [ toUpper ]);
    }

    public static bool GetInTransition()
    {
        if (!IsSubmerged())
            return false;

        return InTrasntionProperty.GetValue<bool>(null);
    }

    public static void RepairOxygen()
    {
        try
        {
            RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceProperty.GetValue(null), [ CustomPlayer.Local, 64 ]);
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
    public static bool LILoaded { get; set; }
    private static BasePlugin LIPlugin { get; set; }
    private static Assembly LIAssembly { get; set; }
    private static Type[] LITypes { get; set; }

    public static bool IsLevelImpostor() => LILoaded && Ship() && Ship().Type == LI_MAP_TYPE && MapPatches.CurrentMap == 7;

    public static bool InitializeLevelImpostor()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(LI_GUID, out var liPlugin) || liPlugin == null)
            return false;

        Message("LevelImpostor was detected");

        try
        {
            LIPlugin = liPlugin.Instance as BasePlugin;
            LIVersion = liPlugin.Metadata.Version;
            Message(LIVersion);

            LIAssembly = LIPlugin.GetType().Assembly;
            LITypes = AccessTools.GetTypesFromAssembly(LIAssembly);

            Message("LevelImpostor compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            Error($"Could not load LevelImposter Compatibility\n{e}");
            return false;
        }
    }

    private static readonly string[] Unsupported = [ "AllTheRoles", "TownOfUs", "TheOtherRoles", "TownOfHost", "Lotus", "LasMonjas", "CrowdedMod", "MCI" ];

    public static bool CheckAbort(out string mod)
    {
        mod = "";

        foreach (var unsupp in Unsupported)
        {
            if (File.Exists(Path.Combine(TownOfUsReworked.ModsFolder, $"{unsupp}.dll")))
            {
                mod = unsupp;
                return true;
            }
        }

        return false;
    }
}