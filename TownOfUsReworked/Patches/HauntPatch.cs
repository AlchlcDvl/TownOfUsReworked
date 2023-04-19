using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
    public static class HauntPatch
    {
        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (ConstantVariables.IsHnS || !CustomGameOptions.DeadSeeEverything)
                return true;

            var role = Role.GetRole(__instance.HauntTarget);
            var modifier = Modifier.GetModifier(__instance.HauntTarget);
            var ability = Ability.GetAbility(__instance.HauntTarget);
            var objectifier = Objectifier.GetObjectifier(__instance.HauntTarget);
            var objectiveString = "";
            var otherString = "";

            if (role != null)
                objectiveString += role.Name;

            if (objectifier != null && objectifier.ObjectifierType != ObjectifierEnum.None)
                objectiveString += $" {objectifier.ColoredSymbol}";

            if (modifier != null && modifier.ModifierType != ModifierEnum.None)
                otherString += $" {modifier.Name}";

            if (ability != null && ability.AbilityType != AbilityEnum.None)
                otherString += $" {ability.Name}";

            var String = objectiveString;

            if (otherString.Length != 0)
                String += "\n" + otherString;

            __instance.FilterText.text = $"<size=75%>{String}</size>";
            return false;
        }
    }
}