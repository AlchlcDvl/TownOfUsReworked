namespace TownOfUsReworked.Options2;

public class HeaderOptionAttribute(MultiMenu menu, string[] groupMemberStrings, HeaderType type = HeaderType.General) : OptionAttribute(menu, CustomOptionType.Header)
{
    public HeaderType HeaderType { get; } = type;
    public string[] GroupMemberStrings { get; } = groupMemberStrings;
    public OptionAttribute[] GroupMembers { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        Created(Setting);
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Created(ViewSetting);
    }

    private void Created(MonoBehaviour setting)
    {
        var header = setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";

        if (HeaderType == HeaderType.Layer)
            header.Background.color = SettingsPatches.GetSettingColor(SettingsPatches.SettingsPage).Shadow().Shadow();
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        GroupMembers = AllOptions.Where(x => GroupMemberStrings.Contains(x.Property.Name)).ToArray();
    }
}