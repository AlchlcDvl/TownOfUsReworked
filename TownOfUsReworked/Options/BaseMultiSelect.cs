namespace TownOfUsReworked.Options;

public abstract class BaseMultiSelectOptionAttribute<T>(CustomOptionType type, T allValue, T noneValue) : OptionAttribute<MultiSelectValue<T>>(type), IMultiSelectOption where T : struct, Enum
{
    protected ValueMap<MissingBehaviour, T> Buttons { get; } = [];
    private T NoneValue { get; } = noneValue;
    private T AllValue { get; } = allValue;
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
        behaviour.GetComponentInChildren<TextMeshPro>().text = TranslationManager.Translate(name);
        behaviour.name = name;
        var button = behaviour.GetComponentInChildren<PassiveButton>();
        var rend = behaviour.GetComponentInChildren<SpriteRenderer>();

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MciActive))
            button.enabled = false;
        else
        {
            button.OverrideOnClickListeners(() => SetValue(value));
            button.OverrideOnMouseOutListeners(() => rend.color = Value.Contains(value) ? CustomColorManager.AcceptedTeal : UColor.white);
            button.OverrideOnMouseOverListeners(() => rend.color = Value.Contains(value) ? UColor.white : CustomColorManager.AcceptedTeal);
        }

        behaviour.gameObject.SetActive(true);
        rend.color = Value.Contains(value) ? CustomColorManager.AcceptedTeal : UColor.white;
        return behaviour;
    }

    protected virtual void TrySetValue(T value, out MultiSelectValue<T> newValue)
    {
        newValue = Value;

        if (value.Equals(AllValue))
        {
            var contained = newValue.Contains(value);
            newValue.Clear();
            newValue.Add(contained ? NoneValue : AllValue);
        }
        else if (value.Equals(NoneValue))
        {
            newValue.Clear();
            newValue.Add(NoneValue);
        }
        else
        {
            if (newValue.Contains(value))
                newValue.Remove(value);
            else
                newValue.Add(value);

            if (newValue.Count == 0)
                newValue.Add(NoneValue);
            else
                newValue.Remove(NoneValue);

            newValue.Remove(AllValue);
        }
    }

    private void SetValue(T value)
    {
        TrySetValue(value, out var newValue);
        Set(newValue);
        Buttons.ForEach((x, y) => x.GetComponentInChildren<SpriteRenderer>().color = Value.Contains(y) ? CustomColorManager.AcceptedTeal : UColor.white);
    }

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadString(), false);

    protected override void ReadValueString(string value) => Set(value, false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(ValueString());

    protected abstract void CreateButtons();
}

public interface IMultiSelectOption
{
    IEnumerable<MissingBehaviour> Options { get; }
}