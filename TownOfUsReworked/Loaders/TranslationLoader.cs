using static TownOfUsReworked.Managers.TranslationManager;

namespace TownOfUsReworked.Loaders;

public class TranslationLoader : AssetLoader<Language>
{
    public override string Manifest => "Languages";
    public override string DirectoryInfo => TownOfUsReworked.Other;

    public static TranslationLoader Instance { get; set; }

    public override IEnumerator AfterLoading(Language[] response)
    {
        foreach (var language in response)
        {
            if (language.ID != null)
                AllTranslations[language.ID] = language;

            language.IDs?.ForEach(id => AllTranslations[id] = language);
        }

        Message($"Found {AllTranslations.Count} translations");
        yield break;
    }
}