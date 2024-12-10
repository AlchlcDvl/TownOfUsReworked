namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
public static class AbilityButtonPatch
{
    public static bool Prefix(AbilityButton __instance)
    {
        var result = !AllButtons.TryFinding(x => x.Base == __instance && !x.Disabled, out var button);

        if (!result)
            button.Update();

        return result;
    }
}

[HarmonyPatch(typeof(ActionButton))]
public static class ActionButtonPatches
{
    [HarmonyPatch(nameof(ActionButton.SetCoolDown)), HarmonyPrefix]
    public static void SetCoolDownPrefix(ActionButton __instance) => __instance.graphic.transform.localPosition = __instance.position;

    [HarmonyPatch(nameof(ActionButton.SetCoolDown))]
    public static void Postfix(ActionButton __instance, float timer, float maxTimer)
    {
        if (timer > Mathf.RoundToInt(maxTimer / 2f))
            __instance.cooldownTimerText.color = UColor.red;
        else if (timer >= 1f)
            __instance.cooldownTimerText.color = UColor.yellow;
        else
            __instance.cooldownTimerText.color = UColor.white;
    }

    [HarmonyPatch(nameof(ActionButton.SetFillUp))]
    public static bool Prefix(ActionButton __instance, float timer, float maxTimer)
    {
        var percentCool = Mathf.Clamp((maxTimer - timer) / maxTimer, 0f, 1f);
        __instance.isCoolingDown = percentCool > 0f;
        __instance.graphic.transform.localPosition = __instance.position + (Vector3)(URandom.insideUnitCircle * (timer < 3f ? URandom.Range(-0.05f, 0.051f) : 0f));
        __instance.cooldownTimerText.SetText($"{Mathf.CeilToInt(timer)}");
        __instance.cooldownTimerText.gameObject.SetActive(__instance.isCoolingDown);
        __instance.SetCooldownFill(percentCool);

        if (timer > Mathf.RoundToInt(maxTimer / 2f))
            __instance.cooldownTimerText.color = UColor.white;
        else if (timer > 3f)
            __instance.cooldownTimerText.color = UColor.yellow;
        else
            __instance.cooldownTimerText.color = UColor.red;

        return false;
    }
}