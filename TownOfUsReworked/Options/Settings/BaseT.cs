namespace TownOfUsReworked.Options.Settings;

public abstract class Option<T>(CustomOptionType type, T defaultValue = default) : Option(type)
{
    private T innerValue = defaultValue;
    public T Value
    {
        get => innerValue;
        protected set
        {
            if (SelfMember)
            {
                if (ModifyValue is not null)
                    value = ModifyValue.Invoke(value, innerValue);

                if (Config is not null)
                    Config.Value = value;

                OnChanged?.Invoke(value);
            }
            else if (Member is not null)
            {
                Member.SetValue(null, value);
                value = Member.GetValue<T>(null);
            }

            innerValue = value;
        }
    }

    public ConfigEntry<T> Config
    {
        get;
        set
        {
            field = value;
            innerValue = value.Value;
        }
    }

    public Action<T> OnChanged { get; init; }
    public Func<T, T, T> ModifyValue { get; init; }

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

    public override void ReadValueRpc(RpcReader reader) => Set(reader.Read<T>(), false, receivedFromRpc: true);

    public override void WriteValueRpc(RpcWriter writer) => writer.Write(Value);

    protected virtual string ValueString() => $"{Value}";

    public void Set(T value, bool rpc = true, bool notify = true, bool receivedFromRpc = false)
    {
        if (IsInGame() && !(ClientOnly || TownOfUsReworked.MciActive))
            return;

        Value = value;

        if (!LocalPlayer)
            return;

        if (rpc && AmongUsClient.Instance.AmHost && !(ClientOnly || !ID.Contains("CustomOption") || this is BaseHeaderOption))
            SendOptionRPC(this);

        if (!receivedFromRpc)
        {
            if (Setting)
                SettingsPatches.OnValueChanged();

            if (ViewSetting)
                SettingsPatches.OnValueChangedView();
        }

        if (!HudManager.InstanceExists || !notify)
            return;

        var stringValue = FormatValue();

        if (IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{SettingNotif()}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";

        if (LastChangedSetting == ID && LastSettingNotif)
            LastSettingNotif.UpdateMessage(changed);
        else
        {
            var notifier = HUD().Notifier;
            LastChangedSetting = ID;
            LastSettingNotif = PopNotif(changed, notifier.settingsChangeColor, notifier.settingsChangeSprite);
        }
    }
}