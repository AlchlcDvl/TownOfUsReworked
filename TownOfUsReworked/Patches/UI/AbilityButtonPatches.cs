namespace TownOfUsReworked.Patches.UI;

[HarmonyPatch(typeof(AbilityButton))]
public static class AbilityButtonPatch
{
    [HarmonyPatch(nameof(AbilityButton.Update)), HarmonyPrefix]
    public static bool UpdatePrefix(AbilityButton __instance)
    {
        if (CustomButton.AllButtons.TryFinding(x => x.Base == __instance && !x.Disabled, out var button))
            button.Update();

        return false;
    }

    [HarmonyPatch(nameof(AbilityButton.DoClick)), HarmonyPrefix]
    public static bool DoClickPrefix(AbilityButton __instance) => CustomButton.AllButtons.All(x => x.Base != __instance); // This event is overridden, but for the sake of it, I've patched this too
}

[HarmonyPatch(typeof(ActionButton))]
public static class ActionButtonPatches
{
    [HarmonyPatch(nameof(ActionButton.SetCoolDown)), HarmonyPrefix]
    public static bool SetCoolDownPrefix(ActionButton __instance, float timer, float maxTimer)
    {
        var percentCool = Mathf.Clamp(timer / maxTimer, 0f, 1f);
        __instance.graphic.transform.localPosition = __instance.position;
        __instance.SetFill(percentCool, timer);
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
        __instance.graphic.transform.localPosition = __instance.position + (Vector3)(URandom.insideUnitCircle * (percentCool > 0.9f ? URandom.Range(-0.05f, 0.051f) : 0f));
        __instance.SetFill(percentCool, timer);
        __instance.cooldownTimerText.color = percentCool switch
        {
            > 0.9f => UColor.red,
            > 0.5f => UColor.yellow,
            _ => UColor.white
        };
        return false;
    }

    private static void SetFill(this ActionButton __instance, float percentCool, float timer)
    {
        __instance.isCoolingDown = percentCool > 0f;
        __instance.cooldownTimerText.text = $"{Mathf.CeilToInt(timer)}";
        __instance.cooldownTimerText.gameObject.SetActive(__instance.isCoolingDown);
        __instance.SetCooldownFill(percentCool);
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
public static class ToggleVisibility
{
    public static bool Visible = true;

    public static void Postfix(bool isActive) => Visible = isActive;
}