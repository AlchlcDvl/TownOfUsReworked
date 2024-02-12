namespace TownOfUsReworked.Modules;

public static class AllMonos
{
    public static void RegisterMonos()
    {
        var usableInterface = new RegisterTypeOptions() { Interfaces = new(new[] { typeof(IUsable) }) };

        //So many monos...AM I THE NEXT SUBMERGED???? o_O
        ClassInjector.RegisterTypeInIl2Cpp<MissingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MenuPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<MeetingPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<DragBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<AssetLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<HatLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<NameplateLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<VisorLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<ColorLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<TranslationLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<PresetLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<ImageLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<PortalLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<SoundLoader>();
        ClassInjector.RegisterTypeInIl2Cpp<HudHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<LobbyConsole>(usableInterface);
    }

    public static void AddComponents()
    {
        TownOfUsReworked.ModInstance.AddComponent<DebuggerBehaviour>();
        TownOfUsReworked.ModInstance.AddComponent<TranslationLoader>();
        TownOfUsReworked.ModInstance.AddComponent<HatLoader>();
        TownOfUsReworked.ModInstance.AddComponent<NameplateLoader>();
        TownOfUsReworked.ModInstance.AddComponent<VisorLoader>();
        TownOfUsReworked.ModInstance.AddComponent<ColorLoader>();
        TownOfUsReworked.ModInstance.AddComponent<PresetLoader>();
        TownOfUsReworked.ModInstance.AddComponent<ImageLoader>();
        TownOfUsReworked.ModInstance.AddComponent<PortalLoader>();
        TownOfUsReworked.ModInstance.AddComponent<SoundLoader>();
    }

    public static void LateAddComponents()
    {
        TownOfUsReworked.ModInstance.AddComponent<HudHandler>();
    }
}