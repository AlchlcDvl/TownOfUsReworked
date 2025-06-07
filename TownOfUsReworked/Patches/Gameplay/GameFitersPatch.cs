namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch]
public static class GameFilters
{
    private const string FilterText = "Reworked";

    [HarmonyPatch(typeof(FilterTagManager), nameof(FilterTagManager.RefreshTags))]
    public static void Postfix() => DataManager.Settings.Multiplayer.ValidGameFilterOptions.FilterTags.Add(FilterText);

    [HarmonyPatch(typeof(FilterTagsMenu))]
    public static class FilterTagsMenuPatches
    {
        [HarmonyPatch(nameof(FilterTagsMenu.Open))]
        public static bool Prefix(FilterTagsMenu __instance)
        {
            // A fix for the game inlining FilterTagsMenu.ChooseOption into the delegate
            __instance.Content.gameObject.SetActive(true);
            var validGameFilterOptions = DataManager.Settings.Multiplayer.ValidGameFilterOptions;
            var num = ((Mathf.CeilToInt(validGameFilterOptions.FilterTags.Count / 10f) / 2f) - 0.5f) * -2.5f;
            __instance.controllerSelectable = new();
            var num2 = 0;

            foreach (var entry in validGameFilterOptions.FilterTags)
            {
                var button = __instance.ButtonPool.Get<ChatLanguageButton>();
                button.transform.localPosition = new(num + (num2 / 10 * 2.5f), 2f - (num2 % 10 * 0.5f), 0f);
                button.Text.text = entry;
                button.Button.OverrideOnClickListeners(() => __instance.ChooseOption(button, entry)); // Set the new patched method in instead
                button.SetSelected(__instance.targetOpts.FilterTags.Contains(entry));
                __instance.controllerSelectable.Add(button.Button);
                num2++;
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.controllerSelectable[0], __instance.controllerSelectable);
            return false;
        }

        [HarmonyPatch(nameof(FilterTagsMenu.ChooseOption))]
        public static void Postfix(FilterTagsMenu __instance, ChatLanguageButton button, string filter)
        {
            if (!__instance.targetOpts.FilterTags.Contains(FilterText))
                return;

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