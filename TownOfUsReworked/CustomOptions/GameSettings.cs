using System.Text;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Data;
using System.Linq;
using UnityEngine;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class GameSettings
    {
        private static int SettingsPage;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        public static class GameOptionsDataPatch
        {
            public static void Postfix(ref string __result)
            {
                if (ConstantVariables.IsHnS)
                    return;

                var builder = new StringBuilder();
                builder.Append("Currently Viewing Page (").Append(SettingsPage + 1).AppendLine("/8)").AppendLine("Press Tab To Change Pages");

                if (SettingsPage == 0)
                    builder.AppendLine("\nGlobal Settings");
                else if (SettingsPage == 1)
                    builder.AppendLine("\n<color=#8BFDFDFF>Crew</color> Settings");
                else if (SettingsPage == 2)
                    builder.AppendLine("\n<color=#B3B3B3FF>Neutral</color> Settings");
                else if (SettingsPage == 3)
                    builder.AppendLine("\n<color=#FF0000FF>Intruder</color> Settings");
                else if (SettingsPage == 4)
                    builder.AppendLine("\n<color=#008000FF>Syndicate</color> Settings");
                else if (SettingsPage == 5)
                    builder.AppendLine("\n<color=#7F7F7FFF>Modifier</color> Settings");
                else if (SettingsPage == 6)
                    builder.AppendLine("\n<color=#DD585BFF>Objectifier</color> Settings");
                else if (SettingsPage == 7)
                    builder.AppendLine("\n<color=#FF9900FF>Ability</color> Settings");

                foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == (MultiMenu)SettingsPage))
                {
                    if (option.Type == CustomOptionType.Button || option.ID == -1)
                        continue;

                    if (option.Type == CustomOptionType.Header)
                    {
                        builder.Append('\n').AppendLine(option.Name);
                    }
                    else if (option.Type == CustomOptionType.Nested)
                    {
                        var nested = (CustomNestedOption)option;

                        foreach (var option2 in nested.InternalOptions)
                        {
                            if (option2.Type == CustomOptionType.Header)
                            {
                                builder.Append('\n').AppendLine(option2.Name);
                            }
                            else if (option2.Type != CustomOptionType.Header && option2.Type != CustomOptionType.Button)
                            {
                                builder.Append("    ").Append(option2.Name).Append(": ").Append(option2).AppendLine();
                            }
                        }
                    }
                    else
                        builder.Append("    ").Append(option.Name).Append(": ").Append(option).AppendLine();
                }

                __result = $"<size=1.25>{builder}</size>";
            }
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
                if (!ConstantVariables.IsLobby)
                    return;

                __instance.ReportButton.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (SettingsPage >= 7)
                        SettingsPage = 0;
                    else
                        SettingsPage++;
                }
            }
        }
    }
}