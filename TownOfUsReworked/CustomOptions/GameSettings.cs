using System.Text;
using HarmonyLib;
using AmongUs.GameOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class GameSettings
    {
        public static bool AllOptions;

        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static class GameOptionsDataPatch
        {
            private static void Postfix(ref string __result)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    return;

                var builder = new StringBuilder(AllOptions ? __result : "");

                foreach (var option in CustomOption.AllOptions)
                {
                    if (option.Type == CustomOptionType.Button || option.ID == -1)
                        continue;
                    else if (option.Type == CustomOptionType.Header)
                        builder.AppendLine($"\n{option.Name}");
                    else if (option.Indent)
                        builder.AppendLine($"    {option.Name}: {option}");
                    else
                        builder.AppendLine($"{option.Name}: {option}");
                }

                __result = builder.ToString();
                __result = $"<size=1.25>{__result}</size>";
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (__instance.Children.Length - 6.5f) / 2;
            }
        }
    }
}