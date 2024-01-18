using static TownOfUsReworked.Localisation.TranslationManager;

namespace TownOfUsReworked.Monos;

public class TranslationsLoader : AssetLoader
{
    public override string ManifestFileName => "Languages";

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(LanguageJSON);

    public static TranslationsLoader Instance { get; private set; }

    public TranslationsLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance != null)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override void AfterLoading(object response)
    {
        var langs = (LanguageJSON)response;
        AllTranslations.AddRange(langs.Languages);
        LogMessage($"Found {AllTranslations.Count} translations");
    }
}