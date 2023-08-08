namespace TownOfUsReworked.CustomOptions;

[HarmonyPatch]
public static class GameSettings
{
    public static int SettingsPage;

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
    public static class GameOptionsDataPatch
    {
        public static void Postfix(ref string __result)
        {
            if (IsHnS)
                return;

            __result = Settings();
        }
    }

    public static string Settings()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Currently Viewing Page ({SettingsPage + 1}/9)");
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
        else if (SettingsPage == 8)
            builder.AppendLine("\nRole Lists");

        var tobedisplayed = CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage && x.Active).ToList();

        foreach (var option in tobedisplayed)
        {
            if (option.Type == CustomOptionType.Button)
                continue;

            var index = tobedisplayed.IndexOf(option);
            var thing = option.Type != CustomOptionType.Header ? (index == tobedisplayed.Count - 1 || tobedisplayed[index + 1].Type == CustomOptionType.Header ? "┗ " : "┣ ") : "";

            if (option is CustomNestedOption nested)
                nested.InternalOptions.ForEach(x => builder.AppendLine($"{thing}{x}"));
            else
                builder.AppendLine($"{thing}{option}");
        }

        return $"<size=1.25>{builder}</size>";
    }

    public static void UpdatePageNumber()
    {
        if (HUD.Chat.freeChatField.textArea.hasFocus || HUD.Chat.quickChatMenu.IsOpen)
            return;
        
        var cached = SettingsPage;

        if (Input.GetKeyDown(KeyCode.Tab) && !GameSettingMenu.Instance)
            SettingsPage = CycleInt(8, 0, SettingsPage, true);

        if (Input.GetKeyDown(KeyCode.Backspace) && !GameSettingMenu.Instance)
            SettingsPage = CycleInt(8, 0, SettingsPage, false);

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

        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            SettingsPage = 8;
        
        SettingsPatches.Changed = cached != SettingsPage;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class LobbyBrowsing
    {
        public static void Postfix(HudManager __instance)
        {
            if (IsLobby)
                __instance.ReportButton.gameObject.SetActive(false);

            UpdatePageNumber();
        }
    }
}