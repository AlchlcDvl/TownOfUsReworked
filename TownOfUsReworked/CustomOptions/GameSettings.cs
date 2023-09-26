namespace TownOfUsReworked.CustomOptions;

public static class GameSettings
{
    public static int SettingsPage;
    public static int CurrentPage = 1;

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
    public static class GameOptionsDataPatch
    {
        public static bool Prefix(ref string __result)
        {
            if (IsHnS)
                return true;

            __result = Settings();
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class LobbyBrowsing
    {
        public static void Postfix(HudManager __instance)
        {
            if (IsLobby)
                __instance?.ReportButton?.gameObject?.SetActive(false);

            UpdatePageNumber();
        }
    }

    public static string Settings()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"<b><size=160%>{Translate($"GameSettings.Page{SettingsPage + 1}")}</size></b>");

        var tobedisplayed = CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage && x.Active).ToList();

        foreach (var option in tobedisplayed)
        {
            if (option.Type == CustomOptionType.Button)
                continue;

            var index = tobedisplayed.IndexOf(option);
            var thing = option is not CustomHeaderOption ? (index == tobedisplayed.Count - 1 || tobedisplayed[index + 1].Type == CustomOptionType.Header ? "┗ " : "┣ " ) : "";
            builder.AppendLine($"{thing}{option}");
        }

        builder.AppendLine();
        builder.AppendLine(Translate("GameSettings.CurrentPage").Replace("%page1%", $"{CurrentPage}").Replace("%page2%", $"{MaxPages()}"));
        builder.AppendLine(Translate("GameSettings.Instructions"));
        return $"<size=1.25>{builder}</size>";
    }

    private static int MaxPages()
    {
        var result = 9;

        if (!CustomGameOptions.EnableAbilities && !IsRoleList)
            result--;

        if (!CustomGameOptions.EnableModifiers && !IsRoleList)
            result--;

        if (!CustomGameOptions.EnableObjectifiers && !IsRoleList)
            result--;

        if (!IsRoleList)
            result--;
        else
            result -= 3;

        return result;
    }

    public static void UpdatePageNumber()
    {
        if (HUD.Chat && HUD.Chat.IsOpenOrOpening)
            return;

        var cached = SettingsPage;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CurrentPage = CycleInt(MaxPages(), 1, CurrentPage, true);
            SettingsPage++;

            if (SettingsPage == 5 && (!CustomGameOptions.EnableModifiers || IsRoleList))
                SettingsPage = 6;

            if (SettingsPage == 6 && (!CustomGameOptions.EnableObjectifiers || IsRoleList))
                SettingsPage = 7;

            if (SettingsPage == 7 && (!CustomGameOptions.EnableAbilities || IsRoleList))
                SettingsPage = 8;

            if (SettingsPage == 8 && !IsRoleList)
                SettingsPage = 0;

            if (SettingsPage > 8)
                SettingsPage = 0;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            CurrentPage = CycleInt(MaxPages(), 1, CurrentPage, false);
            SettingsPage--;

            if (SettingsPage < 0)
                SettingsPage = 8;

            if (SettingsPage == 8 && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage == 7 && (!CustomGameOptions.EnableAbilities || IsRoleList))
                SettingsPage = 6;

            if (SettingsPage == 6 && (!CustomGameOptions.EnableObjectifiers || IsRoleList))
                SettingsPage = 5;

            if (SettingsPage == 5 && (!CustomGameOptions.EnableModifiers || IsRoleList))
                SettingsPage = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            CurrentPage = 1;
            SettingsPage = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            CurrentPage = 2;
            SettingsPage = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            CurrentPage = 3;
            SettingsPage = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            CurrentPage = 4;
            SettingsPage = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            CurrentPage = 5;
            SettingsPage = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (IsRoleList)
                SettingsPage = 8;
            else if (CustomGameOptions.EnableModifiers)
                SettingsPage = 5;
            else if (CustomGameOptions.EnableObjectifiers)
                SettingsPage = 6;
            else if (CustomGameOptions.EnableAbilities)
                SettingsPage = 7;

            if (SettingsPage is 5 or 6 or 7 or 8)
                CurrentPage = 6;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            if (CustomGameOptions.EnableObjectifiers && !IsRoleList)
                SettingsPage = 6;
            else if (CustomGameOptions.EnableAbilities && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage is 6 or 7)
                CurrentPage = 7;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (CustomGameOptions.EnableAbilities && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage == 7)
                CurrentPage = 8;
        }

        SettingsPatches.Changed = cached != SettingsPage;
    }
}