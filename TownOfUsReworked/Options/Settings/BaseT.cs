namespace TownOfUsReworked.Options;

public abstract class Option<T>(CustomOptionType type) : Option(type)
{
    public T Value { get; protected set; }

    protected Type TargetType { get; } = typeof(T);

    public static implicit operator T(Option<T> opt) => opt.Value;

    public override void Set(MemberInfo member, BaseHeaderOption header, bool clientOnly)
    {
        base.Set(member, header, clientOnly);

        try
        {
            Value = member.GetValue<T>(null);
        }
        catch
        {
            Fatal(Name);
        }
    }

    public override string ToString() => $"{ID}:{ValueString()}";

    public override void WriteValueRpc(NetData writer) => writer.Write(Value);

    protected virtual string ValueString() => $"{Value}";

    public void Set(T value, bool rpc = true, bool notify = true)
    {
        if (IsInGame() && !(ClientOnly || TownOfUsReworked.MciActive))
            return;

        if (Member is null)
            Value = value;
        else
        {
            Member.SetValue(null, value);
            Value = Member.GetValue<T>(null);
        }

        if (!CustomPlayer.Local)
            return;

        if (rpc && AmongUsClient.Instance.AmHost && !(ClientOnly || !ID.Contains("CustomOption") || this is BaseHeaderOption))
            SendOptionRPC(this);

        if (Setting)
            SettingsPatches.OnValueChanged();

        if (ViewSetting)
            SettingsPatches.OnValueChangedView();

        var stringValue = Format();

        if (!HudManager.InstanceExists || !notify || IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{SettingNotif()}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";
        var notifier = HUD().Notifier;

        if (LastChangedSetting == ID && LastSettingNotif)
            LastSettingNotif.UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            LastSettingNotif = PopNotif(changed, notifier.settingsChangeColor, notifier.settingsChangeSprite);
        }
    }
}