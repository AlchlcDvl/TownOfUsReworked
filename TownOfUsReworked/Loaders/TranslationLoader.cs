using static TownOfUsReworked.Localisation.TranslationManager;

namespace TownOfUsReworked.Loaders;

public class TranslationLoader : AssetLoader<Language>
{
    public override string Manifest => "Languages";
    public override string DirectoryInfo => TownOfUsReworked.Other;

    public static TranslationLoader Instance { get; set; }

    public override IEnumerator AfterLoading(object response)
    {
        var langs = (List<Language>)response;
        AllTranslations.AddRange(langs);
        LogMessage($"Found {AllTranslations.Count} translations");
        yield break;
    }
}