// ReSharper disable UnusedMember.Global

namespace TownOfUsReworked.Options;

[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    public static ToggleOption LighterDarker = new(true);

    public static ToggleOption WhiteNameplates = new(false) { OnChanged = ToggleNameplates };

    public static ToggleOption NoLevels = new() { OnChanged = ToggleLevelVisibility };

    public static ToggleOption CustomCrewColors = new(true);

    public static ToggleOption CustomNeutColors = new(true);

    public static ToggleOption CustomIntColors = new(true);

    public static ToggleOption CustomSynColors = new(true);

    public static ToggleOption CustomApocColors = new(true);

    public static ToggleOption CustomGmColors = new(true);

    public static ToggleOption CustomModColors = new(true);

    public static ToggleOption CustomDispColors = new(true);

    public static ToggleOption CustomAbColors = new(true);

    public static ToggleOption CustomEjects = new(true);

    public static ToggleOption HideOtherGhosts = new(true);

    public static ToggleOption OptimisationMode = new();

    public static ToggleOption LockCameraSway = new();

    public static ToggleOption ForceUseLocal = new();

    public static ToggleOption UseDarkTheme = new() { OnChanged = SetChatTheme };

    public static ToggleOption NoWelcome = new();

    public static ToggleOption AutoPlayAgain = new();

    public static ToggleOption DebugModeOn = new();

    private static void SetChatTheme(bool value) => ChatPatches.SetTheme(Chat(), value);

    private static void ToggleLevelVisibility(bool value)
    {
        if (Meeting())
            AllVoteAreas().Do(x => x.transform.GetChild(9).gameObject.SetActive(value));
    }

    private static void ToggleNameplates(bool value)
    {
        if (Meeting())
            AllVoteAreas().Do(x => x.Background.sprite = Ship().CosmeticsCache.GetNameplate(value ? "nameplate_NoPlate" : PlayerByVoteArea(x).CurrentOutfit.NamePlateId).Image);
    }
}