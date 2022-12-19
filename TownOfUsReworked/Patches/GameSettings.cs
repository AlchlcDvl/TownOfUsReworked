using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Patches
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
                var builder = new StringBuilder(AllOptions ? __result : "");

                foreach (var option in CustomOption.AllOptions)
                {
                    if (option.Name == "<color=#8BFDFDFF>Crew</color> <color=#1D7CF2FF>Investigative</color> <color=#FFD700FF>Roles</color>")
                        builder.Append("(Scroll for all settings)");

                    if (option.Type == CustomOptionType.Button | option.ID == -1)
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

            public static IEnumerable<MethodBase> TargetMethods()
            {
                return typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
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