namespace TownOfUsReworked.Options;

public abstract class OptionAttribute<T>(CustomOptionType type) : OptionAttribute(type)
{
    private static string LastChangedSetting = "";

    public T Value { get; set; }
    public Type TargetType { get; } = typeof(T);

    public static implicit operator T(OptionAttribute<T> opt) => opt.Value;

    public T Get() => Value;

    public override void SetProperty(PropertyInfo property)
    {
        base.SetProperty(property);
        Value = property.GetValue<T>(null);
    }

    public override void SetField(FieldInfo field)
    {
        base.SetField(field);
        Value = field.GetValue<T>(null);
    }

    public override string ToString() => $"{ID}:{ValueString()}";

    public virtual string ValueString() => $"{Value}";

    public override void PostLoadSetup() {}

    public void Set(T value, bool rpc = true, bool notify = true)
    {
        if (IsInGame() && !(ClientOnly || TownOfUsReworked.MCIActive))
            return;

        if (IsProperty)
        {
            Property.SetValue(null, value);
            Value = Property.GetValue<T>(null);
        }
        else if (IsField)
        {
            Field.SetValue(null, value);
            Value = Field.GetValue<T>(null);
        }
        else
            Value = value;

        if (!CustomPlayer.Local)
            return;

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