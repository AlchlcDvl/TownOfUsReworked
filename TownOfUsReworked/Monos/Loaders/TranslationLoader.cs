using static TownOfUsReworked.Localisation.TranslationManager;

namespace TownOfUsReworked.Monos;

public class TranslationLoader : AssetLoader
{
    public override string ManifestFileName => "Languages";
    public override string DirectoryInfo => TownOfUsReworked.Other;

    [HideFromIl2Cpp]
    public override Type JSONType => typeof(LanguageJSON);

    public static TranslationLoader Instance { get; private set; }

    public TranslationLoader(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    [HideFromIl2Cpp]
    public override IEnumerator AfterLoading(object response)
    {
        var langs = (LanguageJSON)response;
        AllTranslations.AddRange(langs.Languages);
        LogMessage($"Found {AllTranslations.Count} translations");
        yield break;
    }
}