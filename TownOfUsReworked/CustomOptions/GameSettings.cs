namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class GameSettings
    {
        public static int SettingsPage;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        public static class GameOptionsDataPatch
        {
            public static void Postfix(ref string __result)
            {
                if (ConstantVariables.IsHnS)
                    return;

                __result = Settings();
            }
        }

        public static string Settings()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Currently Viewing Page ({SettingsPage + 1}/8)");
            builder.AppendLine("Press The Tab/Page Number To Change Pages");

            if (SettingsPage == 0)
                builder.AppendLine("\nGlobal");
            else if (SettingsPage == 1)
                builder.AppendLine("\n<color=#8CFFFFFF>Crew</color>");
            else if (SettingsPage == 2)
                builder.AppendLine("\n<color=#B3B3B3FF>Neutrals</color>");
            else if (SettingsPage == 3)
                builder.AppendLine("\n<color=#FF0000FF>Intruders</color>");
            else if (SettingsPage == 4)
                builder.AppendLine("\n<color=#008000FF>Syndicate</color>");
            else if (SettingsPage == 5)
                builder.AppendLine("\n<color=#7F7F7FFF>Modifiers</color>");
            else if (SettingsPage == 6)
                builder.AppendLine("\n<color=#DD585BFF>Objectifiers</color>");
            else if (SettingsPage == 7)
                builder.AppendLine("\n<color=#FF9900FF>Abilities</color>");

            foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage))
            {
                if (option.Type == CustomOptionType.Button)
                    continue;

                if (option.Type == CustomOptionType.Header)
                    builder.AppendLine($"\n{option.Name}");
                else if (option.Type == CustomOptionType.Nested)
                {
                    var nested = (CustomNestedOption)option;

                    foreach (var option2 in nested.InternalOptions)
                    {
                        if (option2.Type == CustomOptionType.Header)
                            builder.AppendLine($"\n{option2.Name}");
                        else if (option2.Type != CustomOptionType.Button)
                            builder.AppendLine($"    {option2.Name} - {option2}");
                    }
                }
                else
                    builder.AppendLine($"    {option.Name} - {option}");
            }

            return $"<size=1.25>{builder}</size>";
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance) => __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (__instance.Children.Length - 6.5f) / 2;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class LobbyBrowsing
        {
            public static void Postfix(HudManager __instance)
            {
                if (ConstantVariables.IsLobby)
                    __instance.ReportButton.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (SettingsPage >= 7)
                        SettingsPage = 0;
                    else
                        SettingsPage++;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    SettingsPage = 0;

                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    SettingsPage = 1;

                if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    SettingsPage = 2;

                if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                    SettingsPage = 3;

                if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                    SettingsPage = 4;

                if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                    SettingsPage = 5;

                if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                    SettingsPage = 6;

                if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                    SettingsPage = 7;

                if (!AssetManager.Sprites.ContainsKey("Use"))
                    AssetManager.Sprites.Add("Use", __instance.UseButton.graphic.sprite);
            }
        }
    }
}