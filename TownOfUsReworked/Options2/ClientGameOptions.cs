namespace TownOfUsReworked.Options2;

public static class ClientGameOptions2
{
    // Client Settings
    [HeaderOption(MultiMenu2.Client, [ "LighterDarker", "WhiteNameplates", "NoLevels", "CustomCrewColors", "CustomNeutColors", "CustomIntColors", "CustomSynColors", "CustomModColors", "CustomObjColors", "CustomAbColors", "CustomEjects", "OptimisationMode", "HideOtherGhosts" ], ClientOnly = true)]
    public static object ClientOptions { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleLighterDarker", ClientOnly = true)]
    public static bool LighterDarker { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleWhiteNameplates", ClientOnly = true)]
    public static bool WhiteNameplates { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleNoLevels", ClientOnly = true)]
    public static bool NoLevels { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleCrewColors", ClientOnly = true)]
    public static bool CustomCrewColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleNeutColors", ClientOnly = true)]
    public static bool CustomNeutColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleIntColors", ClientOnly = true)]
    public static bool CustomIntColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleSynColors", ClientOnly = true)]
    public static bool CustomSynColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleModColors", ClientOnly = true)]
    public static bool CustomModColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleObjColors", ClientOnly = true)]
    public static bool CustomObjColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleAbColors", ClientOnly = true)]
    public static bool CustomAbColors { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleCustomEjects", ClientOnly = true)]
    public static bool CustomEjects { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleOptimisationMode", ClientOnly = true)]
    public static bool OptimisationMode { get; set; }

    [ToggleOption(MultiMenu2.Client, "ToggleHideOtherGhosts", ClientOnly = true)]
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