namespace TownOfUsReworked.Options.Settings;

public abstract class BaseMultiSelectOption<T>(CustomOptionType type, T? allValue, T? noneValue, MultiSelectValue<T> defaultValue = null) : Option<MultiSelectValue<T>>(type, defaultValue),
    IMultiSelectOption where T : struct, Enum
{
    protected ValueMap<BlankBehaviour, T> Buttons { get; } = [];
    private T? NoneValue { get; } = noneValue;
    private T? AllValue { get; } = allValue;
    public int LeastSelected { get; init; }
    public IEnumerable<BlankBehaviour> Options => Buttons.Keys;

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

    public override void ViewUpdate() => ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.text = FormatValue();

    public override void Update() => Setting.Cast<StringOption>().ValueText.text = FormatValue();

    protected BlankBehaviour CreateButton(T value, string name)
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
        TrySetValue(value);
        Set(Value);
        Buttons.ForEach((x, y) => x.GetComponentInChildren<SpriteRenderer>().color = Value == y ? CustomColorManager.AcceptedTeal : UColor.white);
    }

    public override void ReadValueRpc(NetData reader) => Set(reader.ReadMultiSelectValue<T>(), false);

    protected override void ReadValueString(string value) => Set(value, false);

    protected virtual UColor TextColor(T value) => UColor.white;

    protected abstract void CreateButtons();

    protected virtual void TrySetValue(T value)
    {
        if (AllValue.HasValue && value.Equals(AllValue))
        {
            var contained = Value == value;
            Value.Clear();
            Value.Add(NoneValue.HasValue ? (contained ? NoneValue : AllValue).Value : value);
        }
        else if (NoneValue.HasValue && value.Equals(NoneValue))
        {
            Value.Clear();
            Value.Add(NoneValue.Value);
        }
        else
        {
            if (Value == value && (Value.Count > LeastSelected))
                Value.Remove(value);
            else
                Value.Add(value);

            if (NoneValue.HasValue)
            {
                if (Value.Count == 0)
                    Value.Add(NoneValue.Value);
                else
                    Value.Remove(NoneValue.Value);
            }

            if (AllValue.HasValue)
                Value.Remove(AllValue.Value);
        }
    }

    public bool Contains(string value) => value.TrueSplit(',').Any(Value.Contains);
}

public interface IMultiSelectOption
{
    IEnumerable<BlankBehaviour> Options { get; }

    bool Contains(string value);
}