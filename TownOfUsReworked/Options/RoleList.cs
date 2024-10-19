namespace TownOfUsReworked.Options;

public class RoleListEntryAttribute() : OptionAttribute<LayerEnum>(MultiMenu.Main, CustomOptionType.Entry)
{
    public bool IsBan { get; set; }
    private string Num { get; set; }
    private TextMeshPro ValueText { get; set; }

    public static List<PassiveButton> ChoiceButtons = [];

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("CustomOption.", "").Replace("Entry", "").Replace("Ban", "");
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = TranslationManager.Translate(ID).Replace("%num%", Num);
        ValueText = Setting.transform.GetChild(3).GetComponent<TextMeshPro>();
        ValueText.text = Format();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.titleText.text = TranslationManager.Translate(ID).Replace("%num%", Num);
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(false);
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(false);
    }

    public override void ModifyViewSetting()
    {
        base.ModifyViewSetting();
        ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.text = Format();
    }

    public override void ModifySetting()
    {
        base.ModifySetting();
        ValueText.text = Format();
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

    public static void ToDo()
    {
        SettingsPatches.SettingsPage = 4;
        SettingsPatches.CachedPage = 0;
        SettingsPatches.OnValueChanged();
    }

    public static bool IsBanned(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => x.IsBan && x.Get() == id) || (id == LayerEnum.Crewmate && RoleListBans.BanCrewmate) || (id == LayerEnum.Impostor &&
        RoleListBans.BanImpostor) || (id == LayerEnum.Anarchist && RoleListBans.BanAnarchist) || (id == LayerEnum.Murderer && RoleListBans.BanMurderer) || id is LayerEnum.Actor or LayerEnum.Revealer or LayerEnum.Ghoul or
        LayerEnum.Banshee or LayerEnum.Phantom or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Mafioso or LayerEnum.Sidekick || (int)id is (> 87 and < 138) or 178 or 179 or 180;

    public static bool IsAdded(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => !x.IsBan && x.Get() == id);

    private static bool IsUnique(LayerEnum id) => GetOptions<LayersOptionAttribute>().Any(x => x.Layer == id && x.Get().Unique);

    private static bool IsAvailable(LayerEnum id) => !IsBanned(id) && !(IsAdded(id) && IsUnique(id));

    public static IEnumerable<LayerEnum> GetLayers() => Enum.GetValues<LayerEnum>().Where(IsAvailable);
}