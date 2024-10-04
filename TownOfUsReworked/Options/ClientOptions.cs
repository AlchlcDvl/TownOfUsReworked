namespace TownOfUsReworked.Options;

[HeaderOption(MultiMenu.Client, ClientOnly = true)]
public static class ClientOptions
{
    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool LighterDarker
    {
        get => TownOfUsReworked.LighterDarker.Value;
        set
        {
            if (value != TownOfUsReworked.LighterDarker.Value)
            {
                TownOfUsReworked.LighterDarker.Value = value;

                if (IsMeeting())
                    AllVoteAreas().ForEach(Role.LocalRole.GenText);
            }
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool WhiteNameplates
    {
        get => TownOfUsReworked.WhiteNameplates.Value;
        set
        {
            if (value != TownOfUsReworked.WhiteNameplates.Value)
            {
                TownOfUsReworked.WhiteNameplates.Value = value;

                if (IsMeeting())
                    AllVoteAreas().ForEach(x => SetNameplates(x, value));
            }
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
                TownOfUsReworked.NoLevels.Value = value;

                if (IsMeeting())
                    AllVoteAreas().ForEach(x => SetLevels(x, value));
            }
        }
    }

    private static void SetLevels(PlayerVoteArea voteArea, bool value) => voteArea.transform.GetChild(9).gameObject.SetActive(value);

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomCrewColors
    {
        get => TownOfUsReworked.CustomCrewColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomCrewColors.Value)
                TownOfUsReworked.CustomCrewColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomNeutColors
    {
        get => TownOfUsReworked.CustomNeutColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomNeutColors.Value)
                TownOfUsReworked.CustomNeutColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomIntColors
    {
        get => TownOfUsReworked.CustomIntColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomIntColors.Value)
                TownOfUsReworked.CustomIntColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomSynColors
    {
        get => TownOfUsReworked.CustomSynColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomSynColors.Value)
                TownOfUsReworked.CustomSynColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomModColors
    {
        get => TownOfUsReworked.CustomModColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomModColors.Value)
                TownOfUsReworked.CustomModColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomDispColors
    {
        get => TownOfUsReworked.CustomDispColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomDispColors.Value)
                TownOfUsReworked.CustomDispColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomAbColors
    {
        get => TownOfUsReworked.CustomAbColors.Value;
        set
        {
            if (value != TownOfUsReworked.CustomAbColors.Value)
                TownOfUsReworked.CustomAbColors.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool CustomEjects
    {
        get => TownOfUsReworked.CustomEjects.Value;
        set
        {
            if (value != TownOfUsReworked.CustomEjects.Value)
                TownOfUsReworked.CustomEjects.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool OptimisationMode
    {
        get => TownOfUsReworked.OptimisationMode.Value;
        set
        {
            if (value != TownOfUsReworked.OptimisationMode.Value)
                TownOfUsReworked.OptimisationMode.Value = value;
        }
    }

    [ToggleOption(MultiMenu.Client, ClientOnly = true)]
    public static bool HideOtherGhosts
    {
        get => TownOfUsReworked.HideOtherGhosts.Value;
        set
        {
            if (value != TownOfUsReworked.HideOtherGhosts.Value)
                TownOfUsReworked.HideOtherGhosts.Value = value;
        }
    }
}