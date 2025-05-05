using static TownOfUsReworked.Managers.TranslationManager;

namespace TownOfUsReworked.Loaders;

public sealed class TranslationLoader : AssetLoader<Language>
{
    protected override string Manifest => "Languages";
    protected override string DirectoryInfo => TownOfUsReworked.Other;
    protected override string Debug => "Translations";

    protected override void LoadAsset(Language item, int i)
    {
        if (item.ID != null)
            AllTranslations[item.ID.ToLower()] = item;

        item.IDs?.ForEach(id => AllTranslations[id.ToLower()] = item);
    }
}