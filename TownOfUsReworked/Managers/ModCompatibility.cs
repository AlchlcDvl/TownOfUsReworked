#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.Managers;

// FIXME: Submerged messes with the body reporting, causing the report button to be entirely unusable, might have to make a custom report button for that ngl
    // Delayed for now because Submerged is not yet updated to the latest version

/// <summary>
/// Master class to help with mod compatibility with Submerged and LevelImpostor.
/// </summary>
public static class ModCompatibilityManager
{
    // public static MethodInfo TryCastMethod;

    /// <summary>
    /// Initialises the necessary patches, methods and members for compatibility usage.
    /// </summary>
    public static void Initialise()
    {
        var injector = new EnumInjector<ShipStatus.MapType>(false, true);

        SubmergedMapType = injector.InjectAndReturn("Submerged", 6);
        // LiMapType = Injector.InjectAndReturn("LevelImpostor", 7);

        SubLoaded = InitializeSubmerged();
        LiLoaded = InitializeLevelImpostor();

        // TryCastMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.TryCast));
    }

    private const string SmGuid = "Submerged";
    private static ShipStatus.MapType SubmergedMapType;

    public static SemVer SubVersion { get; private set; }
    public static bool SubLoaded { get; private set; }
    private static Dictionary<string, Type> SubTypes { get; set; }

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

    private static readonly float[] Segments =
    [
        0.2f, // Off
        0.3f, 0.15f, // Off
        0.35f, 0.2f, // Off
        0.8f, 0.1f, // Off,
        0.2f, 0.1f, 0.1f
    ];
    private static readonly float SegmentSum = Segments.Sum();
    private static float LightTimer;
    private static AudioSource PowerDownSound;
    private static AudioSource PowerUpSound;

    public static bool IsSubmerged() => SubLoaded && Ship() && Ship().Type == SubmergedMapType && MapPatches.CurrentMap == 6;

    private static bool InitializeSubmerged()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(SmGuid, out var subPlugin) || subPlugin is null)
            return false;

        Message("Submerged was detected");

        try
        {
            SubVersion = subPlugin.Metadata.Version;
            Message(SubVersion);

            SubTypes = AccessTools.GetTypesFromAssembly(subPlugin.Instance.GetType().Assembly).TryToDictionary(x => x.Name, x => x);

            var submarineStatusType = SubTypes["SubmarineStatus"];
            SubmergedInstanceField = AccessTools.Field(submarineStatusType, "instance");
            SubmergedElevatorsField = AccessTools.Field(submarineStatusType, "elevators");

            var floorHandlerType = SubTypes["FloorHandler"];
            GetFloorHandlerMethod = AccessTools.Method(floorHandlerType, "GetFloorHandler", [typeof(PlayerControl)]);
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
            GetInElevatorMethod = AccessTools.Method(submarineElevatorType, "GetInElevator", [typeof(PlayerControl)]);
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

            var lightPatch = AccessTools.Method(SubTypes["SubmarineStatusPatches"], "CalculateLightRadiusPatch");

            var awakePatch = AccessTools.Method(SubTypes["SubmarineStatus"], "Awake");

            var compatType = typeof(ModCompatibilityManager);

            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(submergedExileWrapUpMethod, null, new(AccessTools.Method(compatType, nameof(SetPostmortals.Postfix))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(getReadyPlayerAmountMethod, new(AccessTools.Method(compatType, nameof(ReadyPlayerAmount))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(startMethod, new(AccessTools.Method(compatType, nameof(SpawnPatch))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(CanUsePatch.Prefix))), new(AccessTools.Method(compatType,
                nameof(CanUsePatch.Postfix))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(hasMapGetter, new(AccessTools.Method(compatType, nameof(HasMapPatch))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(lightPatch, new(AccessTools.Method(compatType, nameof(SubmergedLightsPatch))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(awakePatch, null, new(AccessTools.Method(compatType, nameof(AwakePatch))));

            Success("Submerged compatibility finished");
            return true;
        }
        catch (Exception e)
        {
            Failure($"Could not load Submerged Compatibility\n{e}");
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

        if (!isInElevator || elevator is null)
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
            if ((bool)GetInElevatorMethod.Invoke(elevator, [player])!)
                return (true, elevator);
        }

        return (false, null);
    }

    public static IEnumerator WaitAction(Action next)
    {
        while (!LocalPlayer.moveable)
            yield return null;

        yield return Wait(0.5f);

        while (HUD().PlayerCam.transform.Find("SpawnInMinigame(Clone)"))
            yield return null;

        next();
    }

    public static void AddSubmergedComponent(this GameObject obj, string typeName)
    {
        if (!IsSubmerged() || !SubTypes.TryGetValue(typeName, out var type) || type is null)
            obj.AddComponent<BlankBehaviour>();
        else
            obj.AddComponent(Il2CppType.From(type));
    }

    public static void ChangeFloor(bool toUpper)
    {
        if (!IsSubmerged())
            return;

        var floorHandler = GetFloorHandlerMethod.Invoke(null, [LocalPlayer]);
        RpcRequestChangeFloorMethod.Invoke(floorHandler, [toUpper]);
        RegisterFloorOverrideMethod.Invoke(floorHandler, [toUpper]);
    }

    public static bool GetInTransition() => IsSubmerged() && InTransitionProperty.GetValue<bool>(null);

    public static void RepairOxygen()
    {
        try
        {
            RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceProperty.GetValue(null), [LocalPlayer, 64]);
        }
        catch { }
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
        if ((GameOptions.GhostSpawn != GhostSpawnType.AtMeeting || !LocalPlayer.Is<IGhosty>(out var ghost) || ghost.Caught) && (!LocalPlayer.Is<Astral>(out var astral) || astral.LastPosition ==
            Vector3.zero || astral.Dead))
        {
            return true;
        }

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

    private static bool SubmergedLightsPatch() => false;

    private static void AwakePatch(dynamic __instance)
    {
        AddAsset("PowerUp", __instance.minigameProperties.audioClips[2]);
        AddAsset("PowerDown", __instance.minigameProperties.audioClips[1]);
    }

    // TODO: Test this when Submerged updates
    public static float GetLightModifier()
    {
        if (LightTimer > SegmentSum - 0.4f && !PowerDownSound)
            PowerDownSound = Play("PowerDown");

        if (LightTimer < SegmentSum)
        {
            var lightsOn = true;
            var currentSum = 0f;

            foreach (var time in Segments)
            {
                if (LightTimer > currentSum)
                {
                    lightsOn = !lightsOn;
                    currentSum += time;
                }
                else
                    return lightsOn ? 1 : 0;
            }
        }

        if (LightTimer < SegmentSum + 2.50f)
            return 0;

        if (!PowerUpSound)
            PowerUpSound = Play("PowerUp");

        if (LightTimer < SegmentSum + 3.75f)
            return 0;

        var adjustedAmount = LightTimer - SegmentSum - 3.75f;
        return Mathf.Lerp(0, 1f, Mathf.Clamp01(adjustedAmount));
    }

    public const string LiGuid = "com.DigiWorm.LevelImposter";
    // private static ShipStatus.MapType LiMapType;

    private static SemVer LiVersion { get; set; }
    public static bool LiLoaded { get; private set; }
    private static Dictionary<string, Type> LiTypes { get; set; }

    // public static bool IsLevelImpostor() => LiLoaded && Ship() && Ship().Type == LiMapType && MapPatches.CurrentMap == 7;

    private static bool InitializeLevelImpostor()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(LiGuid, out var liPlugin) || liPlugin is null)
            return false;

        Message("LevelImpostor was detected");

        try
        {
            LiVersion = liPlugin.Metadata.Version;
            Message(LiVersion);

            LiTypes = AccessTools.GetTypesFromAssembly(liPlugin.Instance.GetType().Assembly).TryToDictionary(x => x.Name, x => x);

            var canUseMethod = AccessTools.Method(LiTypes["TriggerConsole"], "CanUse");

            var setMapMethod = AccessTools.Method(LiTypes["ShopManager"], "SelectMap");

            var compatType = typeof(ModCompatibilityManager);

            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(canUseMethod, new(AccessTools.Method(compatType, nameof(TriggerPrefix))), new(AccessTools.Method(compatType, nameof(TriggerPostfix))));
            TownOfUsReworked.ModInstance.HarmonyInstance.Patch(setMapMethod, null, new(AccessTools.Method(compatType, nameof(SetMapPostfix))));

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

    private static void SetMapPostfix() => MapSettings.Map = Data.Enums.Map.LevelImpostor;

    private static readonly string[] Unsupported = ["AllTheRoles", "TownOfUs", "TheOtherRoles", "TownOfHost", "Lotus", "LasMonjas", "CrowdedMod", "MCI"];
    private static readonly string[] DevOnly = ["sinai-dev-UnityExplorer"];

    public static bool CheckAbort(out string mod) => Unsupported.TryFinding(ModExists, out mod) || (!TownOfUsReworked.IsDev && DevOnly.TryFinding(ModExists, out mod));

    private static bool ModExists(string modName)
    {
        var path = Path.Combine(TownOfUsReworked.ModsFolder, modName);
        return File.Exists(path + ".dll") || Directory.Exists(path);
    }
}