using HarmonyLib;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
    internal sealed class Hauntpatch
    {
        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                return true;

            var role = Role.GetRole(__instance.HauntTarget);
            var modifier = Modifier.GetModifier(__instance.HauntTarget);
            var ability = Ability.GetAbility(__instance.HauntTarget);
            var objectifier = Objectifier.GetObjectifier(__instance.HauntTarget);
            var objectiveString = "";
            var otherString = "";

            if (role != null)
                objectiveString += role.Name;

            if (objectifier != null)
                objectiveString += $" {objectifier.GetColoredSymbol()}";

            if (modifier != null)
                otherString += $" {modifier.Name}";

            if (ability != null)
                otherString += $" {ability.Name}";

            var String = objectiveString;

            if (otherString.Length != 0)
                String += "\n" + otherString;

            __instance.FilterText.text = "<size=75%>" + String + "</size>";
            return false;
        }
    }
}