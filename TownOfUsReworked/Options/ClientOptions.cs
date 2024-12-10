namespace TownOfUsReworked.Options;

[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool LighterDarker
    {
        get => TownOfUsReworked.LighterDarker.Value;
        set => TownOfUsReworked.LighterDarker.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool WhiteNameplates
    {
        get => TownOfUsReworked.WhiteNameplates.Value;
        set
        {
            if (value != TownOfUsReworked.WhiteNameplates.Value)
            {
                if (IsMeeting())
                    AllVoteAreas().ForEach(x => SetNameplates(x, value));
            }

            TownOfUsReworked.WhiteNameplates.Value = value;
        }
    }

    private static void SetNameplates(PlayerVoteArea voteArea, bool value) => voteArea.Background.sprite = Ship().CosmeticsCache.GetNameplate(value ? "nameplate_NoPlate" :
        PlayerByVoteArea(voteArea).CurrentOutfit.NamePlateId).Image;

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool NoLevels
    {
        get => TownOfUsReworked.NoLevels.Value;
        set
        {
            if (value != TownOfUsReworked.NoLevels.Value)
            {
                if (IsMeeting())
                    AllVoteAreas().ForEach(x => SetLevels(x, value));
            }

            TownOfUsReworked.NoLevels.Value = value;
        }
    }

    private static void SetLevels(PlayerVoteArea voteArea, bool value) => voteArea.transform.GetChild(9).gameObject.SetActive(value);

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomCrewColors
    {
        get => TownOfUsReworked.CustomCrewColors.Value;
        set => TownOfUsReworked.CustomCrewColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomNeutColors
    {
        get => TownOfUsReworked.CustomNeutColors.Value;
        set => TownOfUsReworked.CustomNeutColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomIntColors
    {
        get => TownOfUsReworked.CustomIntColors.Value;
        set => TownOfUsReworked.CustomIntColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomSynColors
    {
        get => TownOfUsReworked.CustomSynColors.Value;
        set => TownOfUsReworked.CustomSynColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomModColors
    {
        get => TownOfUsReworked.CustomModColors.Value;
        set => TownOfUsReworked.CustomModColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomDispColors
    {
        get => TownOfUsReworked.CustomDispColors.Value;
        set => TownOfUsReworked.CustomDispColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomAbColors
    {
        get => TownOfUsReworked.CustomAbColors.Value;
        set => TownOfUsReworked.CustomAbColors.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomEjects
    {
        get => TownOfUsReworked.CustomEjects.Value;
        set => TownOfUsReworked.CustomEjects.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool OptimisationMode
    {
        get => TownOfUsReworked.OptimisationMode.Value;
        set => TownOfUsReworked.OptimisationMode.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool HideOtherGhosts
    {
        get => TownOfUsReworked.HideOtherGhosts.Value;
        set => TownOfUsReworked.HideOtherGhosts.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool LockCameraSway
    {
        get => TownOfUsReworked.LockCameraSway.Value;
        set => TownOfUsReworked.LockCameraSway.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool ForceUseLocal
    {
        get => TownOfUsReworked.ForceUseLocal.Value;
        set => TownOfUsReworked.ForceUseLocal.Value = value;
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
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
}