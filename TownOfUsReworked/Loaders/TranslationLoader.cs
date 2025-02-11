using static TownOfUsReworked.Managers.TranslationManager;

namespace TownOfUsReworked.Loaders;

public class TranslationLoader : AssetLoader<Language>
{
    protected override string Manifest => "Languages";
    protected override string DirectoryInfo => TownOfUsReworked.Other;

    protected override IEnumerator LoadAssets(Language[] response)
    {
        foreach (var language in response)
        {
            if (language.ID != null)
                AllTranslations[language.ID] = language;

            language.IDs?.ForEach(id => AllTranslations[id] = language);
            language.Values?.ForEach(lang => language.Modules[lang.Name] = lang.Value);
        }

        Message($"Found {AllTranslations.Count} translations");
        yield return EndFrame();
    }
}