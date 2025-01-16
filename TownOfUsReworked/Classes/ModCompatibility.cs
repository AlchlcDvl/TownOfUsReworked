namespace TownOfUsReworked.Classes;

public static class ModCompatibility
{
    // public static MethodInfo TryCastMethod;

    public static void Initialise()
    {
        SubLoaded = InitializeSubmerged();
        LILoaded = InitializeLevelImpostor();

        // TryCastMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.TryCast));
    }

    public const string SM_GUID = "Submerged";
    public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version SubVersion { get; private set; }
    public static bool SubLoaded { get; set; }
    private static BasePlugin SubPlugin { get; set; }
    private static Assembly SubAssembly { get; set; }
    private static Type[] SubTypes { get; set; }

    public static bool IsSubmerged() => SubLoaded && Ship() && Ship().Type == SUBMERGED_MAP_TYPE && MapPatches.CurrentMap == 6;

    private static MethodInfo RpcRequestChangeFloorMethod;
    private static MethodInfo RegisterFloorOverrideMethod;
    private static MethodInfo GetFloorHandlerMethod;

    private static PropertyInfo InTrasntionProperty;

    private static PropertyInfo SubmarineOxygenSystemInstanceProperty;
    private static MethodInfo RepairDamageMethod;
    public static TaskTypes RetrieveOxygenMask;

    private static MethodInfo GetInElevatorMethod;
    private static MethodInfo GetMovementStageFromTimeMethod;
    private static FieldInfo GetSubElevatorSystemField;

    private static FieldInfo UpperDeckIsTargetFloorField;

    private static FieldInfo SubmergedInstanceField;
    private static FieldInfo SubmergedElevatorsField;

    private static Type SpawnInStateType;
    private static FieldInfo CurrentStateField;

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

            var submarineStatusType = SubTypes.First(t => t.Name == "SubmarineStatus");
            SubmergedInstanceField = AccessTools.Field(submarineStatusType, "instance");
            SubmergedElevatorsField = AccessTools.Field(submarineStatusType, "elevators");

            var floorHandlerType = SubTypes.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(floorHandlerType, "GetFloorHandler", [ typeof(PlayerControl) ]);
            RpcRequestChangeFloorMethod = AccessTools.Method(floorHandlerType, "RpcRequestChangeFloor");
            RegisterFloorOverrideMethod = AccessTools.Method(floorHandlerType, "RegisterFloorOverride");

            var ventPatchDataType = SubTypes.First(t => t.Name == "VentPatchData");
            InTrasntionProperty = AccessTools.Property(ventPatchDataType, "InTransition");

            var customTaskTypesType = SubTypes.First(t => t.Name == "CustomTaskTypes");
            var retrieveOxygenMaskField = AccessTools.Field(customTaskTypesType, "RetrieveOxygenMask");
            var retTaskTypeField = AccessTools.Field(customTaskTypesType, "taskType");
            RetrieveOxygenMask = retTaskTypeField.GetValue<TaskTypes>(retrieveOxygenMaskField.GetValue(null));

            var submarineOxygenSystemType = SubTypes.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceProperty = AccessTools.Property(submarineOxygenSystemType, "Instance");
            RepairDamageMethod = AccessTools.Method(submarineOxygenSystemType, "RepairDamage");

            var submergedExileControllerType = SubTypes.First(t => t.Name == "SubmergedExileController");
            var submergedExileWrapUpMethod = AccessTools.Method(submergedExileControllerType, "WrapUpAndSpawn");

            var submarineElevatorType = SubTypes.First(t => t.Name == "SubmarineElevator");
            GetInElevatorMethod = AccessTools.Method(submarineElevatorType, "GetInElevator", [ typeof(PlayerControl) ]);
            GetMovementStageFromTimeMethod = AccessTools.Method(submarineElevatorType, "GetMovementStageFromTime");
            GetSubElevatorSystemField = AccessTools.Field(submarineElevatorType, "system");

            var submarineElevatorSystemType = SubTypes.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloorField = AccessTools.Field(submarineElevatorSystemType, "upperDeckIsTargetFloor");

            SpawnInStateType = SubTypes.First(t => t.Name == "SpawnInState");

            var subSpawnSystemType = SubTypes.First(t => t.Name == "SubmarineSpawnInSystem");
            var getReadyPlayerAmountMethod = AccessTools.Method(subSpawnSystemType, "GetReadyPlayerAmount");
            CurrentStateField = AccessTools.Field(subSpawnSystemType, "currentState");

            var elevatorConsoleType = SubTypes.First(t => t.Name == "ElevatorConsole");
            var canUseMethod = AccessTools.Method(elevatorConsoleType, "CanUse");

            var submarineSelectSpawnType = SubTypes.First(t => t.Name == "SubmarineSelectSpawn");
            var startMethod = AccessTools.Method(submarineSelectSpawnType, "Start");

            var customPlayerDataType = SubTypes.First(t => t.Name == "CustomPlayerData");
            var hasMapGetter = AccessTools.PropertyGetter(customPlayerDataType, "HasMap");

            // Why do the following patches even exist in submerged??
            var lightFlickerPatchesType = SubTypes.First(x => x.Name == "LightFlickerPatches");
            var patch1 = AccessTools.Method(lightFlickerPatchesType, "CantClickDeadBodyPatch");
            var patch2 = AccessTools.Method(lightFlickerPatchesType, "CantClickReportButtonPatch");
            var patch3 = AccessTools.Method(lightFlickerPatchesType, "DontShowReportButtonPatch");

            var showReportButtonDisabledPatchesType = SubTypes.First(x => x.Name == "ShowReportButtonDisabledPatches");
            var patch4 = AccessTools.Method(showReportButtonDisabledPatchesType, "DontShowReportButtonPatch");

            var compatType = typeof(ModCompatibility);

            TownOfUsReworked.ModInstance.Harmony.Patch(submergedExileWrapUpMethod, null, new(AccessTools.Method(compatType, nameof(ExileRoleChangePostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(getReadyPlayerAmountMethod, new(AccessTools.Method(compatType, nameof(ReadyPlayerAmount))));
            TownOfUsReworked.ModInstance.Harmony.Patch(startMethod, new(AccessTools.Method(compatType, nameof(SpawnPatch))));
            TownOfUsReworked.ModInstance.Harmony.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(ElevatorPrefix))), new(AccessTools.Method(compatType, nameof(ElevatorPostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(hasMapGetter, new(AccessTools.Method(compatType, nameof(HasMapPatch))));
            TownOfUsReworked.ModInstance.Harmony.Patch(patch1, new(AccessTools.Method(compatType, nameof(FixPrefix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(patch2, new(AccessTools.Method(compatType, nameof(FixPrefix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(patch3, new(AccessTools.Method(compatType, nameof(FixPrefix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(patch4, new(AccessTools.Method(compatType, nameof(FixPrefix))));

            Success("Submerged compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            Failure($"Could not load Submerged Compatibility\n{e}");
            return false;
        }
    }

    private static bool FixPrefix() => false;

    private static void ElevatorPrefix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Prefix(pc, ref __state);

    private static void ElevatorPostfix(NetworkedPlayerInfo pc, ref bool __state) => CanUsePatch.Postfix(pc, ref __state);

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

    public static void ExileRoleChangePostfix() => SetPostmortals.Postfix(Ejection());

    public static IEnumerator WaitAction(Action next)
    {
        while (!CustomPlayer.Local.moveable)
            yield return EndFrame();

        yield return Wait(0.5f);

        while (HUD().PlayerCam.transform.Find("SpawnInMinigame(Clone)"))
            yield return EndFrame();

        next();
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

    public static Component AddSubmergedComponent(this GameObject obj, string typeName)
    {
        if (!IsSubmerged())
            return obj.AddComponent<MissingBehaviour>();

        var type = SubTypes.Find(x => x.Name == typeName);
        return type == null ? obj.AddComponent<MissingBehaviour>() : obj.AddComponent(Il2CppType.From(type));
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!IsSubmerged())
            return;

        var _floorHandler = GetFloorHandlerMethod.Invoke(null, [ CustomPlayer.Local ]);
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

    public static bool SpawnPatch(dynamic __instance)
    {
        if ((CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) || (CustomPlayer.Local.TryGetLayer<Astral>(out var astral) && astral.LastPosition != Vector3.zero && !astral.Dead))
        {
            HUD().FullScreen.color = HUD().FullScreen.color.SetAlpha(0f);
            __instance.Close();
            return false;
        }

        return true;
    }

    public static bool HasMapPatch(ref bool __result)
    {
        if (TownOfUsReworked.MCIActive)
        {
            __result = true;
            return false;
        }

        return true;
    }

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

            var triggerConsoleType = LITypes.First(x => x.Name == "TriggerConsole");
            var canUseMethod = AccessTools.Method(triggerConsoleType, "CanUse");

            var compatType = typeof(ModCompatibility);

            TownOfUsReworked.ModInstance.Harmony.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(TriggerPrefix))), new(AccessTools.Method(compatType, nameof(TriggerPostfix))));

            Success("LevelImpostor compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            Failure($"Could not load LevelImposter Compatibility\n{e}");
            return false;
        }
    }

    private static void TriggerPrefix(NetworkedPlayerInfo playerInfo, ref bool __state) => CanUsePatch.Prefix(playerInfo, ref __state);

    private static void TriggerPostfix(NetworkedPlayerInfo playerInfo, ref bool __state) => CanUsePatch.Postfix(playerInfo, ref __state);

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