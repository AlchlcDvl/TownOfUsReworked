namespace TownOfUsReworked.Options;

public class RoleListEntryAttribute() : OptionAttribute<LayerEnum>(MultiMenu.Main, CustomOptionType.Entry)
{
    public bool IsBan { get; set; }
    private string Num { get; set; }
    public static List<PassiveButton> ChoiceButtons = [];

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("RoleList.Entry", "").Replace("RoleList.Ban", "");
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID).Replace("%entry%", Format()).Replace("%num%", Num);
        toggle.CheckMark.enabled = false;
    }

    public override string Format()
    {
        try
        {
            return TranslationManager.Translate($"RoleList.{Value}");
        }
        catch
        {
            Set(LayerEnum.None);
            return TranslationManager.Translate("RoleList.None");
        }
    }

    public void ToDo()
    {
        SettingsPatches.SettingsPage = 4;
        SettingsPatches.CachedPage = 0;
        SettingsPatches.OnValueChanged();
    }

    public override void ModifySetting()
    {
        base.ModifySetting();
        var toggle = Setting.Cast<ToggleOption>();

        if (toggle.CheckMark)
            toggle.CheckMark.enabled = false;

        toggle.TitleText.text = TranslationManager.Translate(ID).Replace("%entry%", Format()).Replace("%num%", Num);
    }

    public static bool IsBanned(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => x.IsBan && x.Get() == id) || (id == LayerEnum.Crewmate && RoleListBans.BanCrewmate) || (id == LayerEnum.Impostor &&
        RoleListBans.BanImpostor) || (id == LayerEnum.Anarchist && RoleListBans.BanAnarchist) || (id == LayerEnum.Murderer && RoleListBans.BanMurderer) || id is LayerEnum.Actor or LayerEnum.Revealer or LayerEnum.Ghoul or
        LayerEnum.Banshee or LayerEnum.Phantom or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Mafioso or LayerEnum.Sidekick;

    private static bool IsAdded(LayerEnum id) => GetOptions<RoleListEntryAttribute>().Any(x => !x.IsBan && x.Get() == id);

    private static bool IsUnique(LayerEnum id) => GetOptions<LayersOptionAttribute>().Any(x => x.Layer == id && x.Get().Unique);

    private static bool IsAvailable(LayerEnum id) => !IsBanned(id) && !(IsAdded(id) && IsUnique(id));

    public static IEnumerable<LayerEnum> GetLayers() => Enum.GetValues<LayerEnum>().Where(IsAvailable);
}