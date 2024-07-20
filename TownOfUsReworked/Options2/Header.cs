namespace TownOfUsReworked.Options2;

public class HeaderOptionAttribute(MultiMenu2 menu, string[] groupMemberStrings, HeaderType type = HeaderType.General) : OptionAttribute(menu, CustomOptionType.Header)
{
    public HeaderType HeaderType { get; } = type;
    public string[] GroupMemberStrings { get; } = groupMemberStrings;
    public OptionAttribute[] GroupMembers { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Single { get; set; }
    private GameObject Collapse { get; set; }
    private GameMode SavedMode { get; set; } = GameMode.None;
    private static Vector3 Left { get; set; }
    private static Vector3 Right { get; set; }
    private static Vector3 Diff { get; set; }

    public bool Get() => (bool)Value;

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
        {
            header.Background.color = SettingsPatches.GetSettingColor(SettingsPatches.SettingsPage).Shadow().Shadow();
            var quota = header.transform.GetChild(2);
            Count = quota.GetChild(1).gameObject;
            Chance = quota.GetChild(4).gameObject;
            Single = quota.GetChild(5).gameObject;
            Active1 = quota.GetChild(6).gameObject;
            Unique = quota.GetChild(7).gameObject;

            if (Left == default)
                Left = Count.transform.localPosition;

            if (Right == default)
                Right = Chance.transform.localPosition;

            if (Diff == default)
                Diff = (Left - Right) / 2;

            UpdateParts();
        }

        Collapse = header.transform.FindChild("Collapse").gameObject;
        Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        Collapse.GetComponentInChildren<TextMeshPro>().text = Get() ? "-" : "+";
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        GroupMembers = AllOptions.Where(x => GroupMemberStrings.Contains(x.Property.Name)).ToArray();
        OptionParents1.Add((GroupMemberStrings, [ Property.Name ]));
    }

    public void Toggle()
    {
        Value = !Get();
        Collapse.GetComponentInChildren<TextMeshPro>().text = (bool)Value ? "-" : "+";
        SettingsPatches.OnValueChanged(GameSettingMenu.Instance);
    }

    public void UpdateParts()
    {
        if (SavedMode == CustomGameOptions2.GameMode || HeaderType == HeaderType.General)
            return;

        SavedMode = CustomGameOptions2.GameMode;
        Chance.SetActive(SavedMode is GameMode.Classic or GameMode.Custom or GameMode.KillingOnly);
        Count.SetActive(SavedMode == GameMode.Custom);
        Unique.SetActive(SavedMode is GameMode.AllAny or GameMode.RoleList);
        Active1.SetActive(SavedMode == GameMode.AllAny);
        Single.SetActive(SavedMode is not (GameMode.Custom or GameMode.AllAny));

        switch (SavedMode)
        {
            case GameMode.Classic:
                Chance.transform.localPosition = Right + Diff;
                break;

            case GameMode.Custom:
                Chance.transform.localPosition = Right;
                Count.transform.localPosition = Left;
                break;

            case GameMode.AllAny:
                Unique.transform.localPosition = Right;
                Active1.transform.localPosition = Left;
                break;

            case GameMode.RoleList:
                Unique.transform.localPosition = Right + Diff;
                break;

            case GameMode.KillingOnly:
                Chance.transform.localPosition = Right + Diff;
                break;
        }
    }
}