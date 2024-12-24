namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AbilityButton))]
public static class AbilityButtonPatch
{
    [HarmonyPatch(nameof(AbilityButton.Update)), HarmonyPrefix]
    public static bool UpdatePrefix(AbilityButton __instance)
    {
        var result = !AllButtons.TryFinding(x => x.Base == __instance && !x.Disabled, out var button);

        if (!result)
            button.Update();

        return result;
    }

    [HarmonyPatch(nameof(AbilityButton.DoClick)), HarmonyPrefix]
    public static bool DoClickPrefix(AbilityButton __instance) => !AllButtons.Any(x => x.Base == __instance);
}

[HarmonyPatch(typeof(ActionButton))]
public static class ActionButtonPatches
{
    [HarmonyPatch(nameof(ActionButton.SetCoolDown)), HarmonyPrefix]
    public static bool SetCoolDownPrefix(ActionButton __instance, float timer, float maxTimer)
    {
        var percentCool = Mathf.Clamp(timer / maxTimer, 0f, 1f);
        __instance.isCoolingDown = percentCool > 0f;
        __instance.graphic.transform.localPosition = __instance.position;
        __instance.cooldownTimerText.SetText($"{Mathf.CeilToInt(timer)}");
        __instance.cooldownTimerText.gameObject.SetActive(__instance.isCoolingDown);
        __instance.SetCooldownFill(percentCool);
        __instance.cooldownTimerText.color = percentCool switch
        {
            > 0.5f => UColor.red,
            > 0.1f => UColor.yellow,
            _ => UColor.white
        };
        return false;
    }

    [HarmonyPatch(nameof(ActionButton.SetFillUp)), HarmonyPrefix]
    public static bool SetFillUpPrefix(ActionButton __instance, float timer, float maxTimer)
    {
        var percentCool = Mathf.Clamp((maxTimer - timer) / maxTimer, 0f, 1f);
        __instance.isCoolingDown = percentCool > 0f;
        __instance.graphic.transform.localPosition = __instance.position + (Vector3)(URandom.insideUnitCircle * (percentCool > 0.9f ? URandom.Range(-0.05f, 0.051f) : 0f));
        __instance.cooldownTimerText.SetText($"{Mathf.CeilToInt(timer)}");
        __instance.cooldownTimerText.gameObject.SetActive(__instance.isCoolingDown);
        __instance.SetCooldownFill(percentCool);
        __instance.cooldownTimerText.color = percentCool switch
        {
            > 0.9f => UColor.red,
            > 0.5f => UColor.yellow,
            _ => UColor.white
        };
        return false;
    }
}