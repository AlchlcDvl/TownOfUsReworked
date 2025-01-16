namespace TownOfUsReworked.Options;

public class RoleListEntryAttribute() : OptionAttribute<LayerEnum>(MultiMenu.Main, CustomOptionType.Entry)
{
    public bool IsBan { get; set; }
    private string Num { get; set; }
    private TextMeshPro ValueText { get; set; }

    public static readonly List<PassiveButton> ChoiceButtons = [];
    public static string SelectedEntry;

    public override void PostLoadSetup()
    {
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("CustomOption.", "").Replace("Entry", "").Replace("Ban", "");
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var entry = Setting.Cast<ToggleOption>();
        entry.TitleText.text = SettingNotif();
        ValueText = Setting.transform.GetChild(3).GetComponent<TextMeshPro>();

        if (!AmongUsClient.Instance.AmHost || (IsInGame() && !TownOfUsReworked.MCIActive))
            entry.CheckMark.transform.parent.gameObject.SetActive(false);
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(false);
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(false);
    }

    public override void ViewUpdate()
    {
        var valueText = ViewSetting.Cast<ViewSettingsInfoPanel>().settingText;
        valueText.text = Format();

        if (LayerDictionary.TryGetValue(Value, out var entry))
            valueText.color = entry.Color;
    }

    public override void Update()
    {
        ValueText.text = Format();

        if (LayerDictionary.TryGetValue(Value, out var entry))
            ValueText.color = entry.Color;
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

    public override string SettingNotif() => base.SettingNotif().Replace("%num%", Num);

    public void ToDo()
    {
        SettingsPatches.SettingsPage = 4;
        SettingsPatches.CachedPage = 0;
        SelectedEntry = ID;
        var scrollbar = GameSettingMenu.Instance.GameSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();

        foreach (var option in AllOptions)
        {
            if (option.Setting)
                option.Setting.gameObject.SetActive(false);
        }

        SettingsPatches.OnValueChanged();
    }

    public static bool IsBanned(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => x.IsBan && x.Get() == id) || (id == LayerEnum.Crewmate && RoleListBans.BanCrewmate) || (id ==
        LayerEnum.Impostor && RoleListBans.BanImpostor) || (id == LayerEnum.Anarchist && RoleListBans.BanAnarchist) || (id == LayerEnum.Murderer && RoleListBans.BanMurderer) || id is
        LayerEnum.Actor or LayerEnum.Revealer or LayerEnum.Ghoul or LayerEnum.Banshee or LayerEnum.Phantom or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Mafioso or
        LayerEnum.Sidekick or LayerEnum.Betrayer || (int)id is (> 87 and < 142) or 182 or 183 or 184;

    public static bool IsAdded(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => !x.IsBan && x.Get() == id);

    private static bool IsUnique(LayerEnum id) => GetOptions<LayerOptionAttribute>().Any(x => x.Layer == id && x.Get().Unique);

    private static bool IsAvailable(LayerEnum id) => !IsBanned(id) && !(IsAdded(id) && IsUnique(id));

    public static IEnumerable<LayerEnum> GetLayers() => Enum.GetValues<LayerEnum>().Where(IsAvailable);
}