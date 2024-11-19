namespace TownOfUsReworked.Monos;

public static class AllMonos
{
    private static bool ComponentsAdded;

    public static void RegisterMonos()
    {
        // So many monos...AM I THE NEXT SUBMERGED???? o_O

        // Handlers
        ClassInjector.RegisterTypeInIl2Cpp<HudHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ClientHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<DragHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<LayerHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<NameHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerControlHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<VoteAreaHandler>();

        // Paging
        ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MenuPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MeetingPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();

        // Misc
        ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MissingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<CustomKillAnimationPlayer>();
        ClassInjector.RegisterTypeInIl2Cpp<CameraEffect>();
        ClassInjector.RegisterTypeInIl2Cpp<Range>();
        ClassInjector.RegisterTypeInIl2Cpp<Bug>();
        ClassInjector.RegisterTypeInIl2Cpp<Bomb>();
    }

    public static void AddComponents()
    {
        if (ComponentsAdded)
            return;

        TownOfUsReworked.ModInstance.AddComponent<DebuggerBehaviour>();

        TownOfUsReworked.ModInstance.AddComponent<HudHandler>();
        TownOfUsReworked.ModInstance.AddComponent<ClientHandler>();
        TownOfUsReworked.ModInstance.AddComponent<DragHandler>();
        TownOfUsReworked.ModInstance.AddComponent<ColorHandler>();

        var prefab = (RoleBehaviour)new GameObject("LayerHandler").DontDestroy().AddComponent(Il2CppType.From(typeof(LayerHandler)));
        prefab.Role = (RoleTypes)100;
        prefab.TeamType = (RoleTeamTypes)5;
        prefab.CanBeKilled = true;
        prefab.CanUseKillButton = false;
        prefab.CanVent = false;
        prefab.NameColor = UColor.white;
        prefab.MaxCount = 127;
        prefab.DefaultGhostRole = (RoleTypes)100;
        prefab.AffectedByLightAffectors = true;
        prefab.IntroSound = null;

        var allRoles = RoleManager.Instance.AllRoles.ToArray().ToList();
        allRoles.Add(prefab);
        RoleManager.Instance.AllRoles = allRoles.ToArray();

        LayerHandler.Crewmate = RoleManager.Instance.GetRole(RoleTypes.Crewmate);
        LayerHandler.Impostor = RoleManager.Instance.GetRole(RoleTypes.Impostor);
        LayerHandler.CrewmateGhost = RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost);
        LayerHandler.ImpostorGhost = RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost);

        ComponentsAdded = true;
    }
}