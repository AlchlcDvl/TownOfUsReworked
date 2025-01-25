namespace TownOfUsReworked.Options;

public abstract class BaseMultiSelectOptionAttribute<T>(CustomOptionType type, T allValue, T noneValue) : OptionAttribute<List<T>>(type), IMultiSelectOption
{
    public ValueMap<ToggleOption, T> Buttons { get; } = [];
    public T NoneValue { get; } = noneValue;
    public T AllValue { get; } = allValue;
    public IEnumerable<ToggleOption> Options => Buttons.Keys;

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

    public ToggleOption CreateButton(T value, string name)
    {
        var behaviour = UObject.Instantiate(SettingsPatches.MultiOptionPrefab, Setting.transform.parent);
        behaviour.TitleText.text = TranslationManager.Translate(name);
        behaviour.name = name;
        var button = behaviour.GetComponentInChildren<PassiveButton>();

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MCIActive))
            button.enabled = false;
        else
            button.OverrideOnClickListeners(() => SetValue(value));

        behaviour.gameObject.SetActive(true);
        behaviour.CheckMark.enabled = Value.Contains(value);
        return behaviour;
    }

    public void SetValue(T value)
    {
        if (value.Equals(AllValue))
        {
            var contained = Value.Contains(value);
            Value.Clear();
            Value.Add(contained ? NoneValue : AllValue);
        }
        else if (value.Equals(NoneValue))
        {
            Value.Clear();
            Value.Add(NoneValue);
        }
        else
        {
            if (Value.Contains(value))
                Value.Remove(value);
            else
                Value.Add(value);

            if (Value.Count == 0)
                Value.Add(NoneValue);
            else
                Value.Remove(NoneValue);

            Value.Remove(AllValue);
        }

        Set(Value);
        Buttons.ForEach((x, y) => x.CheckMark.enabled = Value.Contains(y));
    }

    public override string ValueString() => string.Join(",", Value);

    public override void ReadValueRpc(MessageReader reader) => Set(Parse(reader.ReadString()), false);

    public override void ReadValueString(string value) => Set(Parse(value), false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(ValueString());

    public abstract List<T> Parse(string value);

    public abstract void CreateButtons();
}

public interface IMultiSelectOption
{
    IEnumerable<ToggleOption> Options { get; }
}