namespace TownOfUsReworked.Modules;

public static class AllMonos
{
    public static void RegisterMonos()
    {
        ClassInjector.RegisterTypeInIl2Cpp<MissingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MenuPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MeetingHudPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<DragBehaviour>();
    }

    public static void AddComponents() => TownOfUsReworked.ModInstance.AddComponent<DebuggerBehaviour>();
}