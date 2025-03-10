namespace TownOfUsReworked.Utils;

// FIXME: Submerged messes with the body reporting, causing the report button to be entirely unusable, might have to make a custom report button for that ngl
public static class ModCompatibility
{
    // public static MethodInfo TryCastMethod;

    public static void Initialise()
    {
        SubLoaded = InitializeSubmerged();
        LiLoaded = InitializeLevelImpostor();

        // TryCastMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.TryCast));
    }

    public const string SmGuid = "Submerged";
    private const ShipStatus.MapType SubmergedMapType = (ShipStatus.MapType)6;

    public static SemanticVersioning.Version SubVersion { get; private set; }
    public static bool SubLoaded { get; private set; }
    private static BasePlugin SubPlugin { get; set; }
    private static Assembly SubAssembly { get; set; }
    private static Dictionary<string, Type> SubTypes { get; set; }

    public static bool IsSubmerged() => SubLoaded && Ship() && Ship().Type == SubmergedMapType && MapPatches.CurrentMap == 6;

    private static MethodInfo RpcRequestChangeFloorMethod;
    private static MethodInfo RegisterFloorOverrideMethod;
    private static MethodInfo GetFloorHandlerMethod;

    private static PropertyInfo InTransitionProperty;

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

    private static bool InitializeSubmerged()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(SmGuid, out var subPlugin) || subPlugin == null)
            return false;

        Message("Submerged was detected");

        try
        {
            SubPlugin = subPlugin.Instance as BasePlugin;
            SubVersion = subPlugin.Metadata.Version;
            Message(SubVersion);

            SubAssembly = SubPlugin!.GetType().Assembly;
            SubTypes = AccessTools.GetTypesFromAssembly(SubAssembly).TryToDictionary(x => x.Name, x => x);

            var submarineStatusType = SubTypes["SubmarineStatus"];
            SubmergedInstanceField = AccessTools.Field(submarineStatusType, "instance");
            SubmergedElevatorsField = AccessTools.Field(submarineStatusType, "elevators");

            var floorHandlerType = SubTypes["FloorHandler"];
            GetFloorHandlerMethod = AccessTools.Method(floorHandlerType, "GetFloorHandler", [ typeof(PlayerControl) ]);
            RpcRequestChangeFloorMethod = AccessTools.Method(floorHandlerType, "RpcRequestChangeFloor");
            RegisterFloorOverrideMethod = AccessTools.Method(floorHandlerType, "RegisterFloorOverride");

            InTransitionProperty = AccessTools.Property(SubTypes["VentPatchData"], "InTransition");

            var customTaskTypesType = SubTypes["CustomTaskTypes"];
            var retrieveOxygenMaskField = AccessTools.Field(customTaskTypesType, "RetrieveOxygenMask");
            var retTaskTypeField = AccessTools.Field(customTaskTypesType, "taskType");
            RetrieveOxygenMask = retTaskTypeField.GetValue<TaskTypes>(retrieveOxygenMaskField.GetValue(null));

            var submarineOxygenSystemType = SubTypes["SubmarineOxygenSystem"];
            SubmarineOxygenSystemInstanceProperty = AccessTools.Property(submarineOxygenSystemType, "Instance");
            RepairDamageMethod = AccessTools.Method(submarineOxygenSystemType, "RepairDamage");

            var submergedExileWrapUpMethod = AccessTools.Method(SubTypes["SubmergedExileController"], "WrapUpAndSpawn");

            var submarineElevatorType = SubTypes["SubmarineElevator"];
            GetInElevatorMethod = AccessTools.Method(submarineElevatorType, "GetInElevator", [ typeof(PlayerControl) ]);
            GetMovementStageFromTimeMethod = AccessTools.Method(submarineElevatorType, "GetMovementStageFromTime");
            GetSubElevatorSystemField = AccessTools.Field(submarineElevatorType, "system");

            UpperDeckIsTargetFloorField = AccessTools.Field(SubTypes["SubmarineElevatorSystem"], "upperDeckIsTargetFloor");

            SpawnInStateType = SubTypes["SpawnInState"];

            var subSpawnSystemType = SubTypes["SubmarineSpawnInSystem"];
            var getReadyPlayerAmountMethod = AccessTools.Method(subSpawnSystemType, "GetReadyPlayerAmount");
            CurrentStateField = AccessTools.Field(subSpawnSystemType, "currentState");

            var canUseMethod = AccessTools.Method(SubTypes["ElevatorConsole"], "CanUse");

            var startMethod = AccessTools.Method(SubTypes["SubmarineSelectSpawn"], "Start");

            var hasMapGetter = AccessTools.PropertyGetter(SubTypes["CustomPlayerData"], "HasMap");

            // Why do the following patches even exist in submerged??
            var lightFlickerPatchesType = SubTypes["LightFlickerPatches"];
            var patch1 = AccessTools.Method(lightFlickerPatchesType, "CantClickDeadBodyPatch");
            var patch2 = AccessTools.Method(lightFlickerPatchesType, "CantClickReportButtonPatch");
            var patch3 = AccessTools.Method(lightFlickerPatchesType, "DontShowReportButtonPatch");

            var showReportButtonDisabledPatchesType = SubTypes["ShowReportButtonDisabledPatches"];
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

    private static void MoveDeadPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return;

        var (isInElevator, elevator) = GetPlayerElevator(player);

        if (!isInElevator || elevator == null)
            return;

        if ((int)GetMovementStageFromTimeMethod.Invoke(elevator, null)! < 5)
            return;

        // Fade to clear
        // True is top, false is bottom
        var topFloorTarget = UpperDeckIsTargetFloorField.GetValue<bool>(GetSubElevatorSystemField.GetValue(elevator));
        var topIntendedTarget = player.transform.position.y > -7f;

        if (topFloorTarget != topIntendedTarget)
            ChangeFloor(!topIntendedTarget);
    }

    public static (bool IsInElevator, object Elevator) GetPlayerElevator(PlayerControl player)
    {
        if (!IsSubmerged())
            return (false, null);

        foreach (var elevator in SubmergedElevatorsField.GetValue<IList>(SubmergedInstanceField.GetValue(null)))
        {
            if ((bool)GetInElevatorMethod.Invoke(elevator, [ player ])!)
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
        if (!IsSubmerged() || !__instance.myPlayer.Data.IsDead)
            return;

        var player = __instance.myPlayer;

        if (!player.IsPostmortal() || player.Caught())
            return;

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

    public static void AddSubmergedComponent(this GameObject obj, string typeName)
    {
        if (!IsSubmerged() || !SubTypes.TryGetValue(typeName, out var type) || type == null)
            obj.AddComponent<MissingBehaviour>();
        else
            obj.AddComponent(Il2CppType.From(type));
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!IsSubmerged())
            return;

        var floorHandler = GetFloorHandlerMethod.Invoke(null, [ CustomPlayer.Local ]);
        RpcRequestChangeFloorMethod.Invoke(floorHandler, [ toUpper ]);
        RegisterFloorOverrideMethod.Invoke(floorHandler, [ toUpper ]);
    }

    public static bool GetInTransition() => IsSubmerged() && InTransitionProperty.GetValue<bool>(null);

    public static void RepairOxygen()
    {
        try
        {
            RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceProperty.GetValue(null), [ CustomPlayer.Local, 64 ]);
        } catch {}
    }

    public static bool ReadyPlayerAmount(dynamic __instance, ref int __result)
    {
        if (!SubLoaded || !TownOfUsReworked.MciActive)
            return true;

        __result = __instance.GetTotalPlayerAmount();
        Enum.TryParse(SpawnInStateType, "Done", true, out var e);
        CurrentStateField.SetValue(__instance, e);
        return false;
    }

    public static bool SpawnPatch(dynamic __instance)
    {
        if ((!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && (!CustomPlayer.Local.TryGetLayer<Astral>(out var astral) || astral.LastPosition == Vector3.zero || astral.Dead))
            return true;

        HUD().FullScreen.color = HUD().FullScreen.color.SetAlpha(0f);
        __instance.Close();
        return false;
    }

    public static bool HasMapPatch(ref bool __result)
    {
        if (!TownOfUsReworked.MciActive)
            return true;

        __result = true;
        return false;
    }

    public const string LiGuid = "com.DigiWorm.LevelImposter";
    // private const ShipStatus.MapType LiMapType = (ShipStatus.MapType)7;

    private static SemanticVersioning.Version LiVersion { get;  set; }
    public static bool LiLoaded { get; private set; }
    private static BasePlugin LiPlugin { get; set; }
    private static Assembly LiAssembly { get; set; }
    private static Dictionary<string, Type> LiTypes { get; set; }

    // public static bool IsLevelImpostor() => LiLoaded && Ship() && Ship().Type == LiMapType && MapPatches.CurrentMap == 7;

    private static bool InitializeLevelImpostor()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(LiGuid, out var liPlugin) || liPlugin == null)
            return false;

        Message("LevelImpostor was detected");

        try
        {
            LiPlugin = liPlugin.Instance as BasePlugin;
            LiVersion = liPlugin.Metadata.Version;
            Message(LiVersion);

            LiAssembly = LiPlugin!.GetType().Assembly;
            LiTypes = AccessTools.GetTypesFromAssembly(LiAssembly).TryToDictionary(x => x.Name, x => x);

            var canUseMethod = AccessTools.Method(LiTypes["TriggerConsole"], "CanUse");

            var setMapMethod = AccessTools.Method(LiTypes["ShopManager"], "SelectMap");

            var compatType = typeof(ModCompatibility);

            TownOfUsReworked.ModInstance.Harmony.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(TriggerPrefix))), new(AccessTools.Method(compatType, nameof(TriggerPostfix))));
            TownOfUsReworked.ModInstance.Harmony.Patch(setMapMethod, null, new(AccessTools.Method(compatType, nameof(SetMapPostfix))));

            Success("LevelImposter compatibility finished");
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

    private static void SetMapPostfix() => MapSettings.Map = MapEnum.LevelImpostor;

    private static readonly string[] Unsupported = [ "AllTheRoles", "TownOfUs", "TheOtherRoles", "TownOfHost", "Lotus", "LasMonjas", "CrowdedMod", "MCI" ];

    public static bool CheckAbort(out string mod) => Unsupported.TryFinding(x => File.Exists(Path.Combine(TownOfUsReworked.ModsFolder, $"{x}.dll")), out mod);
}