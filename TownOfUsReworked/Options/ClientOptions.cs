namespace TownOfUsReworked.Options;

[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    [ToggleOption(MultiMenu.Client, "ToggleLighterDarker", ClientOnly = true)]
    public static bool LighterDarker { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleWhiteNameplates", ClientOnly = true)]
    public static bool WhiteNameplates { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleNoLevels", ClientOnly = true)]
    public static bool NoLevels { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleCrewColors", ClientOnly = true)]
    public static bool CustomCrewColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleNeutColors", ClientOnly = true)]
    public static bool CustomNeutColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleIntColors", ClientOnly = true)]
    public static bool CustomIntColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleSynColors", ClientOnly = true)]
    public static bool CustomSynColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleModColors", ClientOnly = true)]
    public static bool CustomModColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleObjColors", ClientOnly = true)]
    public static bool CustomObjColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleAbColors", ClientOnly = true)]
    public static bool CustomAbColors { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleCustomEjects", ClientOnly = true)]
    public static bool CustomEjects { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleOptimisationMode", ClientOnly = true)]
    public static bool OptimisationMode { get; set; }

    [ToggleOption(MultiMenu.Client, "ToggleHideOtherGhosts", ClientOnly = true)]
    public static bool HideOtherGhosts { get; set; }

    public static void SetDefaults()
    {
        // Weird thing I have to do to set their default values
        LighterDarker = TownOfUsReworked.LighterDarker.Value;
        WhiteNameplates = TownOfUsReworked.WhiteNameplates.Value;
        NoLevels = TownOfUsReworked.NoLevels.Value;
        CustomCrewColors = TownOfUsReworked.CustomCrewColors.Value;
        CustomNeutColors = TownOfUsReworked.CustomNeutColors.Value;
        CustomIntColors = TownOfUsReworked.CustomIntColors.Value;
        CustomSynColors = TownOfUsReworked.CustomSynColors.Value;
        CustomModColors = TownOfUsReworked.CustomModColors.Value;
        CustomObjColors = TownOfUsReworked.CustomObjColors.Value;
        CustomAbColors = TownOfUsReworked.CustomAbColors.Value;
        CustomEjects = TownOfUsReworked.CustomEjects.Value;
        OptimisationMode = TownOfUsReworked.OptimisationMode.Value;
        HideOtherGhosts = TownOfUsReworked.HideOtherGhosts.Value;
    }
}