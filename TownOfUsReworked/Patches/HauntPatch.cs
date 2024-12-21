namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HauntMenuMinigame))]
public static class HauntPatches
{
    [HarmonyPatch(nameof(HauntMenuMinigame.SetFilterText)), HarmonyPrefix]
    public static bool SetFilterTextPrefix(HauntMenuMinigame __instance)
    {
        if (IsHnS())
            return true;

        if (!DeadSeeEverything())
        {
            __instance.FilterText.SetText("");
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

        __instance.FilterText.SetText($"<size=75%>{objectiveString}</size>");
        return false;
    }

    [HarmonyPatch(nameof(HauntMenuMinigame.Start)), HarmonyPrefix]
    public static bool StartPrefix(HauntMenuMinigame __instance)
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

    [HarmonyPatch(nameof(HauntMenuMinigame.MatchesFilter))]
    public static bool Prefix(HauntMenuMinigame __instance, PlayerControl pc, ref bool __result)
    {
        __result = __instance.filterMode switch
        {
            HauntMenuMinigame.HauntFilters.Impostor => pc.GetFaction() is Faction.Intruder or Faction.Syndicate || pc.GetAlignment() is Alignment.NeutralApoc or Alignment.NeutralKill or
                Alignment.NeutralNeo || pc.Is<Hunter>(),
            HauntMenuMinigame.HauntFilters.Crewmate => pc.GetFaction() is Faction.Crew || pc.GetAlignment() is Alignment.NeutralBen or Alignment.NeutralEvil or
                Alignment.NeutralPros || pc.Is<Hunted>(),
            HauntMenuMinigame.HauntFilters.Ghost => pc.HasDied(),
            _ => true
        };
        return false;
    }
}