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

    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
    public static class HauntUpdatePatch
    {
        public static void Postfix()
        {
            if (!ConstantVariables.IsInGame)
                HudManager.Instance.AbilityButton.gameObject.SetActive(false);
            else if (ConstantVariables.IsHnS)
                HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
            else
            {
                var ghostRole = PlayerControl.LocalPlayer.IsPostmortal() && !PlayerControl.LocalPlayer.Caught();
                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead);
            }
        }
    }

    [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Start))]
    public static class AddNeutralHauntPatch
    {
        public static bool Prefix(HauntMenuMinigame __instance)
        {
            if (!ConstantVariables.IsNormal || Role.LocalRole.Zooming)
                return true;

            __instance.FilterButtons[0].gameObject.SetActive(true);
            var numActive = 0;
            var numButtons = __instance.FilterButtons.Count(x => x.isActiveAndEnabled);
            var edgeDist = 0.6f * numButtons;

            foreach (var button in __instance.FilterButtons)
            {
                if (button.isActiveAndEnabled)
                {
                    button.transform.SetLocalX(FloatRange.SpreadToEdges(-edgeDist, edgeDist, numActive, numButtons));
                    numActive++;
                }
            }

            return false;
        }
    }
}