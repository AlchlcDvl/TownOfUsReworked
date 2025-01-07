namespace TownOfUsReworked.Options;

public abstract class OptionAttribute<T>(MultiMenu menu, CustomOptionType type, int priority = -1) : OptionAttribute(menu, type, priority)
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
        if (IsInGame() && !ClientOnly)
            return;

        Property?.SetValue(null, value);
        Value = Property.GetValue<T>(null);

        if (!CustomPlayer.Local)
            return;

        // OnChanged(value);

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
        var hud = HUD();

        if (LastChangedSetting == ID && hud.Notifier.activeMessages.Count > 0)
            hud.Notifier.activeMessages[^1].UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            var newMessage = UObject.Instantiate(hud.Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, hud.Notifier.transform);
            newMessage.transform.localPosition = new(0f, 0f, -2f);
            newMessage.SetUp(changed, hud.Notifier.settingsChangeSprite, hud.Notifier.settingsChangeColor, (Action)(() => hud.Notifier.OnMessageDestroy(newMessage)));
            hud.Notifier.ShiftMessages();
            hud.Notifier.AddMessageToQueue(newMessage);
        }
    }
}