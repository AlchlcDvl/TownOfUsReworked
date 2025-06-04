// ReSharper disable UnusedMember.Global

namespace TownOfUsReworked.Options;

/// <summary>
/// All of the options that are client sided and not networked in any way.
/// </summary>
[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    public static ReworkedToggleOption LighterDarker = new(true);

    public static ReworkedToggleOption WhiteNameplates = new() { OnChanged = ToggleNameplates };

    public static ReworkedToggleOption NoLevels = new() { OnChanged = ToggleLevelVisibility };

    public static ReworkedToggleOption CustomCrewColors = new(true);

    public static ReworkedToggleOption CustomNeutColors = new(true);

    public static ReworkedToggleOption CustomIntColors = new(true);

    public static ReworkedToggleOption CustomSynColors = new(true);

    public static ReworkedToggleOption CustomApocColors = new(true);

    public static ReworkedToggleOption CustomGmColors = new(true);

    public static ReworkedToggleOption CustomModColors = new(true);

    public static ReworkedToggleOption CustomDispColors = new(true);

    public static ReworkedToggleOption CustomAbColors = new(true);

    public static ReworkedToggleOption CustomEjects = new(true);

    public static ReworkedToggleOption HideOtherGhosts = new(true);

    public static ReworkedToggleOption OptimisationMode = new();

    public static ReworkedToggleOption LockCameraSway = new();

    public static ReworkedToggleOption ForceUseLocal = new();

    public static ReworkedToggleOption UseDarkTheme = new() { OnChanged = SetChatTheme };

    public static ReworkedToggleOption NoWelcome = new();

    public static ReworkedToggleOption AutoPlayAgain = new();

    public static ReworkedToggleOption DebugModeOn = new();

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