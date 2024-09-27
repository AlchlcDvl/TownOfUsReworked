namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
public static class HauntPatch
{
    public static bool Prefix(HauntMenuMinigame __instance)
    {
        if (IsHnS())
            return true;

        if (!DeadSeeEverything())
        {
            __instance.FilterText.text = " ";
            return false;
        }

        var role = __instance.HauntTarget.GetRole();
        var modifier = __instance.HauntTarget.GetModifier();
        var ability = __instance.HauntTarget.GetAbility();
        var disposition = __instance.HauntTarget.GetDisposition();
        var objectiveString = "";
        var otherString = "";

        if (role)
            objectiveString += role.Name;

        if (disposition && disposition.Type != LayerEnum.NoneDisposition)
            objectiveString += $" {disposition.ColoredSymbol}";

        if (modifier && modifier.Type != LayerEnum.NoneModifier)
            otherString += $" {modifier.Name}";

        if (ability && ability.Type != LayerEnum.NoneAbility)
            otherString += $" {ability.Name}";

        if (otherString.Length != 0)
            objectiveString += "\n" + otherString;

        __instance.FilterText.text = $"<size=75%>{objectiveString}</size>";
        return false;
    }
}

[HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Start))]
public static class AddNeutralHauntPatch
{
    public static bool Prefix(HauntMenuMinigame __instance)
    {
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