namespace TownOfUsReworked.Options.Settings;

public sealed class LayerHeaderOption(Layer layer) : SpecialHeader(MultiMenu.LayerSubOptions, CustomOptionType.LayerHeader)
{
    public readonly Layer Layer = layer;
    private LayerOption LinkedOption;

    public override void OptionCreated()
    {
        base.OptionCreated();
        Desc.GetComponentInChildren<TextMeshPro>().text = TranslationManager.Translate($"ShortDesc.{Layer}");
        Setting.transform.FindChild("Title").GetComponent<TextMeshPro>().color = (Label.color = LayerDictionary[Layer].Color).Alternate(0.3f);
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        LinkedOption = GetOptions<LayerOption>().FirstOrDefault(x => x.Layer == Layer);
    }

    protected override bool Visible() => LinkedOption?.Header?.PartiallyActive() == true && LinkedOption?.PartiallyActive() == true;

    public override void Debug()
    {
        base.Debug();
        TranslationManager.DebugId($"ShortDesc.{Layer}");
    }
}