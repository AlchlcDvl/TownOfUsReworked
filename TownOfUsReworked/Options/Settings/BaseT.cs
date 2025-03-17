namespace TownOfUsReworked.Options;

public abstract class Option<T>(CustomOptionType type) : Option(type)
{
    public T Value { get; set; }

    protected Type TargetType { get; } = typeof(T);

    public static implicit operator T(Option<T> opt) => opt.Value;

    public override void Set(MemberInfo member, BaseHeaderOption header, bool clientOnly)
    {
        base.Set(member, header, clientOnly);
        Value = member.GetValue<T>(null);
    }

    public override string ToString() => $"{ID}:{ValueString()}";

    protected virtual string ValueString() => $"{Value}";

    public void Set(T value, bool rpc = true, bool notify = true)
    {
        if (IsInGame() && !(ClientOnly || TownOfUsReworked.MciActive))
            return;

        if (Member == null)
            Value = value;
        else
        {
            Member.SetValue(null, value);
            Value = Member.GetValue<T>(null);
        }

        if (!CustomPlayer.Local)
            return;

        if (AmongUsClient.Instance.AmHost && rpc && !(ClientOnly || !ID.Contains("CustomOption") || this is BaseHeaderOption))
            SendOptionRPC(this);

        if (Setting)
            SettingsPatches.OnValueChanged();

        if (ViewSetting)
            SettingsPatches.OnValueChangedView();

        var stringValue = Format();

        if (!HudManager.InstanceExists || !notify || IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{SettingNotif()}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";
        var hud = HUD();

        if (LastChangedSetting == ID && hud.Notifier.activeMessages.Count > 0 && LastSettingNotif)
            LastSettingNotif.UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            LastSettingNotif = UObject.Instantiate(hud.Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, hud.Notifier.transform);
            LastSettingNotif.transform.localPosition = new(0f, 0f, -2f);
            LastSettingNotif.SetUp(changed, hud.Notifier.settingsChangeSprite, hud.Notifier.settingsChangeColor, (Action)(() => hud.Notifier.OnMessageDestroy(LastSettingNotif)));
            hud.Notifier.ShiftMessages();
            hud.Notifier.AddMessageToQueue(LastSettingNotif);
        }
    }
}