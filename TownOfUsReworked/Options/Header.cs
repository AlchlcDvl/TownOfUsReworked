namespace TownOfUsReworked.Options;

public class CustomHeaderOption : CustomOption
{
    public HeaderType HeaderType { get; }

    public CustomHeaderOption(MultiMenu menu, string name, object parent = null, bool clientOnly = false) : this(menu, name, [parent], false, clientOnly) {}

    public CustomHeaderOption(MultiMenu menu, string name, object[] parents, bool all = false, bool clientOnly = false) : this(menu, name, HeaderType.General, parents, all, clientOnly) {}

    public CustomHeaderOption(MultiMenu menu, string name, HeaderType headerType, object parent = null, bool clientOnly = false) : this(menu, name, headerType, [parent], false, clientOnly) {}

    public CustomHeaderOption(MultiMenu menu, string name, HeaderType headerType, object[] parents, bool all = false, bool clientOnly = false) : base(menu, name, CustomOptionType.Header,
        null, parents, all, null, clientOnly)
    {
        HeaderType = headerType;
        Format = _ => "";
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{Name}</b>";

        // if (HeaderType == HeaderType.Layer)
        //     header.Background.color = SettingsPatches.GetSettingColor(SettingsPatches.SettingsPage).Shadow().Shadow();
    }
}