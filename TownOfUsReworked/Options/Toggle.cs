namespace TownOfUsReworked.Options;

public class CustomToggleOption : CustomOption
{
    public Func<bool> OnClick { get; }

    public CustomToggleOption(MultiMenu menu, string name, bool value, object parent = null) : this(menu, name, value, [parent], false) {}

    public CustomToggleOption(MultiMenu menu, string name, bool value, object[] parents, bool all = false) : base(menu, name, CustomOptionType.Toggle, value, parents, all) => Format = Blank;

    public CustomToggleOption(MultiMenu menu, string name, Func<bool> onClick, bool defaultValue, object parent = null, bool clientOnly = false) : base(menu, name, CustomOptionType.Toggle,
        defaultValue, parent: parent, clientOnly: clientOnly)
    {
        OnClick = onClick;
        Format = Blank;
    }

    public bool Get() => (bool)Value;

    public void Toggle() => Set(OnClick == null ? !Get() : OnClick());

    private static Func<object, string> Blank = val => (bool)val ? "On" : "Off";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = Name;
        toggle.CheckMark.enabled = Get();
    }

    public static implicit operator bool(CustomToggleOption option) => option?.Get() == true;

    public static bool LighterDarker()
    {
        TownOfUsReworked.LighterDarker.Value = !TownOfUsReworked.LighterDarker.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(Role.LocalRole.GenText);

        return TownOfUsReworked.LighterDarker.Value;
    }

    public static bool WhiteNameplates()
    {
        TownOfUsReworked.WhiteNameplates.Value = !TownOfUsReworked.WhiteNameplates.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(x => x.SetCosmetics(PlayerByVoteArea(x).Data));

        return TownOfUsReworked.WhiteNameplates.Value;
    }

    public static bool NoLevels()
    {
        TownOfUsReworked.NoLevels.Value = !TownOfUsReworked.NoLevels.Value;

        if (IsMeeting)
        {
            foreach (var voteArea in AllVoteAreas)
            {
                var rend = voteArea.LevelNumberText.GetComponentInParent<SpriteRenderer>();
                rend.enabled = TownOfUsReworked.NoLevels.Value;
                rend.gameObject.SetActive(TownOfUsReworked.NoLevels.Value);
            }
        }

        return TownOfUsReworked.NoLevels.Value;
    }
}