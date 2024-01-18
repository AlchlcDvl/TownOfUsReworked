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
        ClassInjector.RegisterTypeInIl2Cpp<AssetLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<HatsLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<NameplatesLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<VisorsLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorsLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<TranslationsLoader>();
    }

    public static void AddComponents()
    {
        TownOfUsReworked.ModInstance.AddComponent<DebuggerBehaviour>();
        TownOfUsReworked.ModInstance.AddComponent<TranslationsLoader>();
        TownOfUsReworked.ModInstance.AddComponent<HatsLoader>();
        TownOfUsReworked.ModInstance.AddComponent<NameplatesLoader>();
        TownOfUsReworked.ModInstance.AddComponent<VisorsLoader>();
        TownOfUsReworked.ModInstance.AddComponent<ColorsLoader>();
    }
}