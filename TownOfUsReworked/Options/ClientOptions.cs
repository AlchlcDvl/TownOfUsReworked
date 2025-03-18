// ReSharper disable UnusedMember.Global
namespace TownOfUsReworked.Options;

[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    [ToggleOption]
    public static bool LighterDarker
    {
        get => TownOfUsReworked.LighterDarker.Value;
        set => TownOfUsReworked.LighterDarker.Value = value;
    }

    [ToggleOption]
    public static bool WhiteNameplates
    {
        get => TownOfUsReworked.WhiteNameplates.Value;
        set
        {
            if (value != TownOfUsReworked.WhiteNameplates.Value && Meeting())
                AllVoteAreas().ForEach(x => x.Background.sprite = Ship().CosmeticsCache.GetNameplate(value ? "nameplate_NoPlate" : PlayerByVoteArea(x).CurrentOutfit.NamePlateId).Image);

            TownOfUsReworked.WhiteNameplates.Value = value;
        }
    }

    [ToggleOption]
    public static bool NoLevels
    {
        get => TownOfUsReworked.NoLevels.Value;
        set
        {
            if (value != TownOfUsReworked.NoLevels.Value && Meeting())
                AllVoteAreas().ForEach(x => x.transform.GetChild(9).gameObject.SetActive(value));

            TownOfUsReworked.NoLevels.Value = value;
        }
    }

    [ToggleOption]
    public static bool CustomCrewColors
    {
        get => TownOfUsReworked.CustomCrewColors.Value;
        set => TownOfUsReworked.CustomCrewColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomNeutColors
    {
        get => TownOfUsReworked.CustomNeutColors.Value;
        set => TownOfUsReworked.CustomNeutColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomIntColors
    {
        get => TownOfUsReworked.CustomIntColors.Value;
        set => TownOfUsReworked.CustomIntColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomSynColors
    {
        get => TownOfUsReworked.CustomSynColors.Value;
        set => TownOfUsReworked.CustomSynColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomGmColors
    {
        get => TownOfUsReworked.CustomGmColors.Value;
        set => TownOfUsReworked.CustomGmColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomModColors
    {
        get => TownOfUsReworked.CustomModColors.Value;
        set => TownOfUsReworked.CustomModColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomDispColors
    {
        get => TownOfUsReworked.CustomDispColors.Value;
        set => TownOfUsReworked.CustomDispColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomAbColors
    {
        get => TownOfUsReworked.CustomAbColors.Value;
        set => TownOfUsReworked.CustomAbColors.Value = value;
    }

    [ToggleOption]
    public static bool CustomEjects
    {
        get => TownOfUsReworked.CustomEjects.Value;
        set => TownOfUsReworked.CustomEjects.Value = value;
    }

    [ToggleOption]
    public static bool OptimisationMode
    {
        get => TownOfUsReworked.OptimisationMode.Value;
        set => TownOfUsReworked.OptimisationMode.Value = value;
    }

    [ToggleOption]
    public static bool HideOtherGhosts
    {
        get => TownOfUsReworked.HideOtherGhosts.Value;
        set => TownOfUsReworked.HideOtherGhosts.Value = value;
    }

    [ToggleOption]
    public static bool LockCameraSway
    {
        get => TownOfUsReworked.LockCameraSway.Value;
        set => TownOfUsReworked.LockCameraSway.Value = value;
    }

    [ToggleOption]
    public static bool ForceUseLocal
    {
        get => TownOfUsReworked.ForceUseLocal.Value;
        set => TownOfUsReworked.ForceUseLocal.Value = value;
    }

    [ToggleOption]
    public static bool UseDarkTheme
    {
        get => TownOfUsReworked.UseDarkTheme.Value;
        set
        {
            if (value != TownOfUsReworked.UseDarkTheme.Value)
                ChatPatches.SetTheme(Chat());

            TownOfUsReworked.UseDarkTheme.Value = value;
        }
    }

    [ToggleOption]
    public static bool NoWelcome
    {
        get => TownOfUsReworked.NoWelcome.Value;
        set => TownOfUsReworked.NoWelcome.Value = value;
    }

    [ToggleOption]
    public static bool AutoPlayAgain
    {
        get => TownOfUsReworked.AutoPlayAgain.Value;
        set => TownOfUsReworked.AutoPlayAgain.Value = value;
    }
}