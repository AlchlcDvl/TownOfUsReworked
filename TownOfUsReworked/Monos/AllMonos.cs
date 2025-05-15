namespace TownOfUsReworked.Monos;

/// <summary>
/// Mono script manager that registers components from the mod.
/// </summary>
public static class AllMonos
{
    /// <summary>
    /// Flag to indicate whether or not important scripts have been attached/created.
    /// </summary>
    private static bool ComponentsAdded;

    /// <summary>
    /// Registers all of the scripts within the mod.
    /// </summary>
    public static void RegisterMonos()
    {
        // So many monos...AM I THE NEXT SUBMERGED???? o_O

        // Handlers
        ClassInjector.RegisterTypeInIl2Cpp<HudHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ClientHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<LayerHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<NameHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerControlHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<VoteAreaHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<DeadBodyHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<FootprintHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<StatsHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<StatusHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<CameraEffectHandler>();

        // Paging
        ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MenuPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MeetingPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();

        // Behaviours
        ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<BlankBehaviour>();

        // Objects
        ClassInjector.RegisterTypeInIl2Cpp<Range>();
        ClassInjector.RegisterTypeInIl2Cpp<Bug>();
        ClassInjector.RegisterTypeInIl2Cpp<Bomb>();
        ClassInjector.RegisterTypeInIl2Cpp<Footprint>();
        ClassInjector.RegisterTypeInIl2Cpp<Ash>();

        // Misc
        ClassInjector.RegisterTypeInIl2Cpp<CustomKillAnimationPlayer>();
    }

    /// <summary>
    /// Adds/creates important script object for use during the mod's session.
    /// </summary>
    public static void AddComponents()
    {
        if (ComponentsAdded)
            return;

        LayerHandler.Crewmate = RoleManager.Instance.GetRole(RoleTypes.Crewmate);
        LayerHandler.Impostor = RoleManager.Instance.GetRole(RoleTypes.Impostor);
        LayerHandler.CrewmateGhost = RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost);
        LayerHandler.ImpostorGhost = RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost);

        LayerHandler.HauntMenu = LayerHandler.CrewmateGhost.TryCast<CrewmateGhostRole>()?.HauntMenu;

        var prefab = new GameObject("LayerHandler").DontDestroy().AddComponent<LayerHandler>();
        prefab.Role = prefab.DefaultGhostRole = (RoleTypes)100;
        prefab.TeamType = (RoleTeamTypes)5;
        prefab.CanBeKilled = true;
        prefab.CanUseKillButton = false;
        prefab.CanVent = false;
        prefab.NameColor = UColor.white;
        prefab.MaxCount = 250;
        prefab.AffectedByLightAffectors = true;
        prefab.IntroSound = null;
        prefab.TasksCountTowardProgress = false;
        prefab.Ability = LayerHandler.CrewmateGhost.Ability;

        RoleManager.Instance.AllRoles = RoleManager.Instance.AllRoles.Concat([prefab]).ToArray();

        ComponentsAdded = true;
    }
}