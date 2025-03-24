namespace TownOfUsReworked.Options;

public abstract class BaseMultiSelectOption<T>(CustomOptionType type, T? allValue, T? noneValue) : Option<MultiSelectValue<T>>(type), IMultiSelectOption where T : struct, Enum
{
    protected ValueMap<MissingBehaviour, T> Buttons { get; } = [];
    protected T? NoneValue { get; } = noneValue;
    protected T? AllValue { get; } = allValue;
    public IEnumerable<MissingBehaviour> Options => Buttons.Keys;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var multi = Setting.Cast<StringOption>();
        multi.TitleText.text = SettingNotif();
        multi.ValueText.transform.localPosition += new Vector3(1.05f, 0f, 0f);
        var rend = Setting.transform.GetChild(5).GetComponent<SpriteRenderer>();
        var button = Setting.transform.FindChild("Button").GetComponent<PassiveButton>();
        button.OverrideOnMouseOverListeners(() => rend.color = Buttons.Any() ? UColor.white : CustomColorManager.AcceptedTeal);
        button.OverrideOnMouseOutListeners(() => rend.color = Buttons.Any() ? CustomColorManager.AcceptedTeal : UColor.white);
        button.OverrideOnClickListeners(CreateButtons);
        Buttons.Clear();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(false);
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(false);
    }

    public override void ViewUpdate() => ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.text = Format();

    public override void Update() => Setting.Cast<StringOption>().ValueText.text = Format();

    protected MissingBehaviour CreateButton(T value, string name)
    {
        var behaviour = UObject.Instantiate(SettingsPatches.MultiOptionPrefab, Setting.transform.parent);
        var text = behaviour.GetComponentInChildren<TextMeshPro>();
        text.text = TranslationManager.Translate(name);
        text.color = TextColor(value);
        behaviour.name = name;
        var button = behaviour.GetComponentInChildren<PassiveButton>();
        var rend = behaviour.GetComponentInChildren<SpriteRenderer>();

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MciActive))
            button.enabled = false;
        else
        {
            button.OverrideOnClickListeners(() => SetValue(value));
            button.OverrideOnMouseOutListeners(() => rend.color = Value == value ? CustomColorManager.AcceptedTeal : UColor.white);
            button.OverrideOnMouseOverListeners(() => rend.color = Value == value ? UColor.white : CustomColorManager.AcceptedTeal);
        }

        behaviour.gameObject.SetActive(true);
        rend.color = Value == value ? CustomColorManager.AcceptedTeal : UColor.white;
        return behaviour;
    }

    private void SetValue(T value)
    {
        TrySetValue(value, out var newValue);
        Set(newValue);
        Buttons.ForEach((x, y) => x.GetComponentInChildren<SpriteRenderer>().color = Value == y ? CustomColorManager.AcceptedTeal : UColor.white);
    }

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadString(), false);

    protected override void ReadValueString(string value) => Set(value, false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(ValueString());

    protected virtual UColor TextColor(T value) => UColor.white;

    protected abstract void CreateButtons();

    protected abstract void TrySetValue(T value, out MultiSelectValue<T> newValue);
}

public interface IMultiSelectOption
{
    IEnumerable<MissingBehaviour> Options { get; }
}