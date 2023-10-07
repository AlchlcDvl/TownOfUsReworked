namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
public static class HauntUpdatePatch
{
    public static bool Prefix(AbilityButton __instance)
    {
        var button = CustomButton.AllButtons.Find(x => x.Base == __instance);

        if (button != null)
        {
            button.Update1();
            return false;
        }

        return true;
    }

    public static void Postfix(AbilityButton __instance)
    {
        if (__instance != HUD.AbilityButton)
            return;

        if (!IsInGame)
            __instance.gameObject.SetActive(false);
        else if (IsHnS)
            __instance.gameObject.SetActive(!CustomPlayer.Local.IsImpostor());
        else
            __instance.gameObject.SetActive(!(CustomPlayer.Local.IsPostmortal() && !CustomPlayer.Local.Caught()) && !Meeting && CustomPlayer.LocalCustom.IsDead);
    }
}

[HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetCoolDown))]
public static class AbilityButtonSetCooldown
{
    public static void Prefix(ActionButton __instance) => __instance.graphic.transform.localPosition = __instance.position;

    public static void Postfix(ActionButton __instance, ref float timer, ref float maxTimer)
    {
        if (timer > Mathf.FloorToInt(maxTimer / 2f))
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
    public static bool Prefix(ActionButton __instance, ref float timer, ref float maxTimer)
    {
        var percentCool = Mathf.Clamp((maxTimer - timer) / maxTimer, 0f, 1f);
        __instance.isCoolingDown = percentCool > 0f;
        __instance.graphic.transform.localPosition = __instance.position + ((Vector3)URandom.insideUnitCircle * (__instance.isCoolingDown && timer < 3f ? 0.05f : 0f));
        __instance.cooldownTimerText.text = Mathf.CeilToInt(timer).ToString();
        __instance.cooldownTimerText.gameObject.SetActive(__instance.isCoolingDown);
        __instance.SetCooldownFill(percentCool);

        if (timer > Mathf.FloorToInt(maxTimer / 2f))
            __instance.cooldownTimerText.color = UColor.white;
        else if (timer > 3f)
            __instance.cooldownTimerText.color = UColor.yellow;
        else
            __instance.cooldownTimerText.color = UColor.red;

        return false;
    }
}