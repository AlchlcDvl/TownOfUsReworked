namespace TownOfUsReworked.Monos;

public static class AllMonos
{
    private static bool ComponentsAdded;

    public static void RegisterMonos()
    {
        var usableInterface = new RegisterTypeOptions() { Interfaces = new(new[] { typeof(IUsable) }) };

        // So many monos...AM I THE NEXT SUBMERGED???? o_O

        // Handlers
        ClassInjector.RegisterTypeInIl2Cpp<HudHandler>();
        // ClassInjector.RegisterTypeInIl2Cpp<ClientHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<MeetingHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<DragHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorHandler>();

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
        ClassInjector.RegisterTypeInIl2Cpp<LobbyConsole>(usableInterface);
    }

    public static void AddComponents()
    {
        if (ComponentsAdded)
            return;

        TownOfUsReworked.ModInstance.AddComponent<DebuggerBehaviour>();

        TownOfUsReworked.ModInstance.AddComponent<HudHandler>();
        // TownOfUsReworked.ModInstance.AddComponent<ClientHandler>();
        TownOfUsReworked.ModInstance.AddComponent<PlayerHandler>();
        TownOfUsReworked.ModInstance.AddComponent<MeetingHandler>();
        TownOfUsReworked.ModInstance.AddComponent<DragHandler>();
        TownOfUsReworked.ModInstance.AddComponent<ColorHandler>();

        ComponentsAdded = true;
    }
}