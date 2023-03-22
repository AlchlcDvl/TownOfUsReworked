using System.Text;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class GameSettings
    {
        public static int SettingsPage = 0;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static class GameOptionsDataPatch
        {
            private static void Postfix(ref string __result)
            {
                if (GameStates.IsHnS)
                    return;

                var builder = new StringBuilder();
                builder.AppendLine($"Currently Viewing Page ({(SettingsPage + 1)}/8)");
                builder.AppendLine("Press Tab To Change Pages");

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
                        builder.AppendLine($"\n{option.Name}");
                    else if (option.Type == CustomOptionType.Nested)
                    {
                        var nested = (CustomNestedOption)option;
                        builder.AppendLine($"\n{option.Name}");

                        foreach (var option2 in nested.InternalOptions)
                        {
                            if (option2.Type != CustomOptionType.Header && option2.Type != CustomOptionType.Button)
                                builder.AppendLine($"    {option2.Name}: {option2}");
                        }
                    }
                    else
                        builder.AppendLine($"    {option.Name}: {option}");
                }

                __result = $"<size=1.25>{builder.ToString()}</size>";
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance) => __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (__instance.Children.Length - 6.5f) / 2;
        }
    }
}