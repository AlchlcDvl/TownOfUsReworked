namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class GameFilters
{
    private const string FilterText = "Reworked";

    [HarmonyPatch(typeof(FilterTagManager), nameof(FilterTagManager.RefreshTags))]
    public static void Postfix() => DataManager.Settings.Multiplayer.ValidGameFilterOptions.FilterTags.Add(FilterText);

    [HarmonyPatch(typeof(FilterTagsMenu), nameof(FilterTagsMenu.ChooseOption))]
    public static void Postfix(FilterTagsMenu __instance, ChatLanguageButton button, string filter)
    {
        if (__instance.targetOpts.FilterTags.Contains(FilterText))
        {
            if (filter == FilterText)
            {
                __instance.targetOpts.FilterTags = new();
                __instance.targetOpts.FilterTags.Add(FilterText);
                __instance.controllerSelectable.ForEach(x => x.GetComponent<ChatLanguageButton>().SetSelected(false));
                button.SetSelected(true);
            }
            else
            {
                __instance.targetOpts.FilterTags.Remove(FilterText);

                foreach (var btn in __instance.controllerSelectable)
                {
                    var langBtn = btn.GetComponent<ChatLanguageButton>();

                    if (langBtn.Text.text == FilterText)
                        langBtn.SetSelected(false);
                }
            }

            __instance.UpdateButtonText();
        }
    }
}