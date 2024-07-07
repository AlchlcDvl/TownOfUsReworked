namespace TownOfUsReworked.Options2;

public class ToggleOptionAttribute(MultiMenu menu, string onChanged = null) : OptionAttribute(menu, CustomOptionType.Toggle)
{
    public Func<bool> OnClick { get; set; }
    private string OnClickName { get; } = onChanged;

    public bool Get() => (bool)Value;

    public void Toggle() => Set(OnClick == null ? !Get() : OnClick());

    public override string Format() => Get() ? "On" : "Off";

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();

        if (OnClickName != null)
            OnClick = (Func<bool>)AccessTools.GetDeclaredProperties(typeof(ToggleOptionAttribute)).Find(x => x.Name == OnClickName)?.GetValue(null);
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID);
        toggle.CheckMark.enabled = Get();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = "";
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(Get());
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(!Get());
    }

    private static bool LighterDarkerMethod()
    {
        TownOfUsReworked.LighterDarker.Value = !TownOfUsReworked.LighterDarker.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(Role.LocalRole.GenText);

        return TownOfUsReworked.LighterDarker.Value;
    }

    private static bool WhiteNameplatesMethod()
    {
        TownOfUsReworked.WhiteNameplates.Value = !TownOfUsReworked.WhiteNameplates.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(x => x.SetCosmetics(PlayerByVoteArea(x).Data));

        return TownOfUsReworked.WhiteNameplates.Value;
    }

    private static bool NoLevelsMethod()
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

    public static Func<bool> ToggleLighterDarker => LighterDarkerMethod;

    public static Func<bool> ToggleWhiteNameplates => WhiteNameplatesMethod;

    public static Func<bool> ToggleNoLevels => NoLevelsMethod;

    public static Func<bool> ToggleCrewColors => () => TownOfUsReworked.CustomCrewColors.Value = !TownOfUsReworked.CustomCrewColors.Value;

    public static Func<bool> ToggleNeutColors => () => TownOfUsReworked.CustomNeutColors.Value = !TownOfUsReworked.CustomNeutColors.Value;

    public static Func<bool> ToggleIntColors => () => TownOfUsReworked.CustomIntColors.Value = !TownOfUsReworked.CustomIntColors.Value;

    public static Func<bool> ToggleSynColors => () => TownOfUsReworked.CustomSynColors.Value = !TownOfUsReworked.CustomSynColors.Value;

    public static Func<bool> ToggleModColors => () => TownOfUsReworked.CustomModColors.Value = !TownOfUsReworked.CustomModColors.Value;

    public static Func<bool> ToggleAbColors => () => TownOfUsReworked.CustomAbColors.Value = !TownOfUsReworked.CustomAbColors.Value;

    public static Func<bool> ToggleObjColors => () => TownOfUsReworked.CustomObjColors.Value = !TownOfUsReworked.CustomObjColors.Value;

    public static Func<bool> ToggleCustomEjects => () => TownOfUsReworked.CustomEjects.Value = !TownOfUsReworked.CustomEjects.Value;

    public static Func<bool> ToggleOptimisationMode => () => TownOfUsReworked.OptimisationMode.Value = !TownOfUsReworked.OptimisationMode.Value;

    public static Func<bool> ToggleHideOtherGhosts => () => TownOfUsReworked.HideOtherGhosts.Value = !TownOfUsReworked.HideOtherGhosts.Value;
}