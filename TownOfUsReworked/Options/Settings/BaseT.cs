namespace TownOfUsReworked.Options;

public abstract class Option<T>(CustomOptionType type) : Option(type)
{
    private T innerValue;
    public T Value
    {
        get => innerValue;
        protected set
        {
            innerValue = value;

            if (Member is not null && !SelfMember)
                Member.SetValue(null, value);
        }
    }

    protected Type TargetType { get; } = typeof(T);

    public static implicit operator T(Option<T> opt) => opt.Value;

    public override void Set(MemberInfo member, BaseHeaderOption header, bool clientOnly, bool selfMember)
    {
        base.Set(member, header, clientOnly, selfMember);

        if (selfMember)
            return;

        try
        {
            innerValue = member.GetValue<T>(null);
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

        Value = value;

        if (!LocalPlayer)
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