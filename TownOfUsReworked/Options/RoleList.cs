namespace TownOfUsReworked.Options;

public class RoleListEntryAttribute() : OptionAttribute(MultiMenu.Main, CustomOptionType.Entry)
{
    public bool IsBan { get; set; }
    private string Num { get; set; }
    // public static readonly Dictionary<LayerEnum, string> Alignments = new()
    // {
    //     { LayerEnum.None, "None"},

    //     { LayerEnum.Any, "Any"},

    //     { LayerEnum.RandomCrew, "<color=#1D7CF2FF>Random</color> <color=#8CFFFFFF>Crew</color>"},
    //     { LayerEnum.RegularCrew, "<color=#1D7CF2FF>Regular</color> <color=#8CFFFFFF>Crew</color>"},

    //     { LayerEnum.RandomNeutral, "<color=#1D7CF2FF>Random</color> <color=#B3B3B3FF>Neutral</color>"},
    //     { LayerEnum.RegularNeutral, "<color=#1D7CF2FF>Regular</color> <color=#B3B3B3FF>Neutral</color>"},
    //     { LayerEnum.HarmfulNeutral, "<color=#1D7CF2FF>Harmful</color> <color=#B3B3B3FF>Neutral</color>"},

    //     { LayerEnum.RandomIntruder, "<color=#1D7CF2FF>Random</color> <color=#FF1919FF>Intruder</color>"},
    //     { LayerEnum.RegularIntruder, "<color=#1D7CF2FF>Regular</color> <color=#FF1919FF>Intruder</color>"},

    //     { LayerEnum.RandomSyndicate, "<color=#1D7CF2FF>Random</color> <color=#008000FF>Syndicate</color>"},
    //     { LayerEnum.RegularSyndicate, "<color=#1D7CF2FF>Regular</color> <color=#008000FF>Syndicate</color>"},
    // };

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("RoleList.Entry", "").Replace("RoleList.Ban", "");
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID).Replace("%entry%", Format()).Replace("%num%", Num);
        toggle.CheckMark.enabled = false;
    }

    public override string Format()
    {
        try
        {
            return TranslationManager.Translate($"RoleList.{Value}");
        }
        catch
        {
            Set(LayerEnum.None);
            return TranslationManager.Translate("RoleList.None");
        }
    }

    public LayerEnum Get() => (LayerEnum)Value;

    public void ToDo()
    {
        SettingsPatches.SettingsPage = 4;
        SettingsPatches.CachedPage = 0;
        SettingsPatches.OnValueChanged();
    }

    public override void ModifySetting(out string stringValue)
    {
        base.ModifySetting(out stringValue);
        var toggle = Setting.Cast<ToggleOption>();

        if (toggle.CheckMark)
            toggle.CheckMark.enabled = false;

        stringValue = Format();
        toggle.TitleText.text = TranslationManager.Translate(ID).Replace("%entry%", stringValue).Replace("%num%", Num);
    }

    public static bool IsBanned(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => x.IsBan && x.Get() == id) || id == LayerEnum.Actor || (id == LayerEnum.Crewmate &&
        RoleListBans.BanCrewmate) || (id == LayerEnum.Impostor && RoleListBans.BanImpostor) || (id == LayerEnum.Anarchist && RoleListBans.BanAnarchist) || (id == LayerEnum.Murderer &&
        RoleListBans.BanMurderer);

    private static bool IsAdded(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => !x.IsBan && x.Get() == id);

    private static bool IsUnique(LayerEnum id) => GetOptions<LayersOptionAttribute>().Any(x => x.Layer == id && x.Get().Unique);

    private static bool IsAvailable(LayerEnum id) => !IsBanned(id) && !(IsAdded(id) && IsUnique(id));
}