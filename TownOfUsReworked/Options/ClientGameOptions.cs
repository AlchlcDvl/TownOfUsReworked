namespace TownOfUsReworked.Options;

public static class ClientGameOptions
{
    public static bool LighterDarker => Generate.LighterDarker;
    public static bool WhiteNameplates => Generate.WhiteNameplates;
    public static bool NoLevels => Generate.NoLevels;
    public static bool CustomCrewColors => Generate.CustomCrewColors;
    public static bool CustomNeutColors => Generate.CustomNeutColors;
    public static bool CustomIntColors => Generate.CustomIntColors;
    public static bool CustomSynColors => Generate.CustomSynColors;
    public static bool CustomModColors => Generate.CustomModColors;
    public static bool CustomObjColors => Generate.CustomObjColors;
    public static bool CustomAbColors => Generate.CustomAbColors;
    public static bool CustomEjects => Generate.CustomEjects;
    public static bool OptimisationMode => Generate.OptimisationMode;
}