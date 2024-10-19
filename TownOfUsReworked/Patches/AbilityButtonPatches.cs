namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
public static class HauntUpdatePatch
{
    public static bool Prefix(AbilityButton __instance)
    {
        var result = !AllButtons.TryFinding(x => x.Base == __instance && !x.Disabled, out var button);
        button?.Update();
        return result;
    }
}

[HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetCoolDown))]
public static class AbilityButtonSetCooldown
{
    public static void Prefix(ActionButton __instance) => __instance.graphic.transform.localPosition = __instance.position;

    public static void Postfix(ActionButton __instance, float timer, float maxTimer)
    {
        if (timer > Mathf.RoundToInt(maxTimer / 2f))
            __instance.cooldownTimerText.color = UColor.red;
        else if (timer >= 1f)
            __instance.cooldownTimerText.color = UColor.yellow;
        else
            __instance.cooldownTimerText.color = UColor.white;
    }
}

[HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetFillUp))]
public static class AbilityButtonSetFillUp
{
    public static bool Prefix(ActionButton __instance, float timer, float maxTimer)
    {
        var percentCool = Mathf.Clamp((maxTimer - timer) / maxTimer, 0f, 1f);
        var rand = URandom.Range(-0.05f, 0.051f);
        __instance.isCoolingDown = percentCool > 0f;
        __instance.graphic.transform.localPosition = __instance.position + ((Vector3)URandom.insideUnitCircle * (__instance.isCoolingDown && timer < 3f ? rand : 0f));
        __instance.cooldownTimerText.text = Mathf.CeilToInt(timer).ToString();
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

[HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
public static class BlockDis
{
    public static bool Prefix(AbilityButton __instance) => !AllButtons.Any(x => x.Base == __instance);
}