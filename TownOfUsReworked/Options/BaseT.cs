namespace TownOfUsReworked.Options;

public abstract class OptionAttribute<T>(MultiMenu menu, CustomOptionType type) : OptionAttribute(menu, type)
{
    private static string LastChangedSetting = "";

    public T Value { get; set; }
    public T DefaultValue { get; set; }

    public T Get() => Value;

    public override void SetProperty(PropertyInfo property)
    {
        base.SetProperty(property);
        Value = DefaultValue = property.GetValue<T>(null);
    }

    public override string ToString() => $"{ID}:{(Value is Number num ? $"{num:0.###}" : $"{Value}")}";

    public void Set(T value, bool rpc = true, bool notify = true)
    {
        Value = value;
        Property?.SetValue(null, value);
        // OnChanged.Invoke(value);

        if (AmongUsClient.Instance.AmHost && rpc && !(ClientOnly || !ID.Contains("CustomOption") || Type is CustomOptionType.Header or CustomOptionType.Alignment))
            SendOptionRPC(this);

        if (Setting)
            SettingsPatches.OnValueChanged();

        if (ViewSetting)
            SettingsPatches.OnValueChangedView();

        var stringValue = Format();

        if (!notify || IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{SettingNotif()}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";

        if (LastChangedSetting == ID && HUD().Notifier.activeMessages.Count > 0)
            HUD().Notifier.activeMessages[^1].UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            var newMessage = UObject.Instantiate(HUD().Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, HUD().Notifier.transform);
            newMessage.transform.localPosition = new(0f, 0f, -2f);
            newMessage.SetUp(changed, HUD().Notifier.settingsChangeSprite, HUD().Notifier.settingsChangeColor, (Action)(() => HUD().Notifier.OnMessageDestroy(newMessage)));
            HUD().Notifier.ShiftMessages();
            HUD().Notifier.AddMessageToQueue(newMessage);
        }
    }
}