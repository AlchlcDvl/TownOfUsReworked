namespace TownOfUsReworked.Patches.UI;

[HarmonyPatch(typeof(HauntMenuMinigame))]
public static class HauntPatches
{
    [HarmonyPatch(nameof(HauntMenuMinigame.SetHauntTarget))]
    public static bool Prefix(HauntMenuMinigame __instance, PlayerControl target)
    {
        if (target)
        {
            __instance.HauntTarget = target;
            __instance.HauntingText.enabled = true;
            __instance.NameText.text = target.Data?.GetPlayerName(PlayerOutfitType.Default);
            __instance.SetFilterText();
        }
        else
        {
            __instance.HauntTarget = null;
            __instance.NameText.text = "";
            __instance.FilterText.text = "";
            __instance.HauntingText.enabled = false;
        }

        return false;
    }

    [HarmonyPatch(nameof(HauntMenuMinigame.SetFilterText)), HarmonyPrefix]
    public static bool SetFilterTextPrefix(HauntMenuMinigame __instance)
    {
        if (IsHnS())
            return true;

        if (!DeadSeeEverything())
        {
            __instance.FilterText.text = "";
            return false;
        }

        var handler = LayerHandler.Handlers[__instance.HauntTarget.PlayerId];
        var objectiveString = "";
        var otherString = "";

        if (handler.CurrentRole)
            objectiveString += handler.CurrentRole.Name;

        if (handler.CurrentDisposition && handler.CurrentDisposition.Type != Layer.NoneDisposition)
            objectiveString += $" {handler.CurrentDisposition.ColoredSymbol}";

        if (handler.CurrentModifier && handler.CurrentModifier.Type != Layer.NoneModifier)
            otherString += $" {handler.CurrentModifier.Name}";

        if (handler.CurrentAbility && handler.CurrentAbility.Type != Layer.NoneAbility)
            otherString += $" {handler.CurrentAbility.Name}";

        if (otherString.Length != 0)
            objectiveString += "\n" + otherString;

        __instance.FilterText.text = $"<size=75%>{objectiveString}</size>";
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
            if (!button.isActiveAndEnabled)
                continue;

            button.transform.SetLocalX(FloatRange.SpreadToEdges(-edgeDist, edgeDist, numActive, numButtons));
            numActive++;
        }

        return false;
    }
}

[HarmonyPatch(typeof(HauntMenuMinigame.__c__DisplayClass21_0), nameof(HauntMenuMinigame.__c__DisplayClass21_0._ChangePick_b__1))]
public static class FilterInlinePatch
{
    public static bool Prefix(HauntMenuMinigame.__c__DisplayClass21_0 __instance, PlayerControl pc, ref bool __result)
    {
        var role = pc.GetRole();
        __result = __instance.__4__this.filterMode switch
        {
            HauntMenuMinigame.HauntFilters.Impostor => role.Faction is not (Faction.Crew or Faction.Outcast or Faction.GameMode) || role is Hunter or OKilling or Neophyte or Betrayer,
            HauntMenuMinigame.HauntFilters.Crewmate => role.Faction is Faction.Crew || role.Alignment is Alignment.Benign or Alignment.Evil or Alignment.Proselyte || role is Hunted,
            HauntMenuMinigame.HauntFilters.Ghost => !role.Alive,
            _ => true
        };
        return false;
    }
}