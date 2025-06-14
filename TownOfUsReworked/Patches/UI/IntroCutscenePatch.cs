namespace TownOfUsReworked.Patches.UI;

[HarmonyPatch]
public static class ShowTeamPatch
{
#if ANDROID
    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__37), "MoveNext")]
    public static void Postfix(IntroCutscene._ShowTeam_d__37 __instance)
#else
    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
    public static void Postfix(IntroCutscene._ShowTeam_d__38 __instance)
#endif
    {
        if (IsHnS())
            return;

        var role = LocalPlayer.GetRole();
        __instance.__4__this.TeamTitle.text = role.FactionName;
        __instance.__4__this.TeamTitle.color = role.FactionColor;
        __instance.__4__this.TeamTitle.outlineColor = UColor.black;
        __instance.__4__this.BackgroundBar.material.color = role.FactionColor;
        __instance.__4__this.ImpostorText.text = "";
    }
}

[HarmonyPatch]
public static class ShowRolePatch
{
#if ANDROID
    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__40), "MoveNext")]
    public static void Postfix(IntroCutscene._ShowRole_d__40 __instance)
#else
    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
    public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
#endif
    {
        if (IsHnS())
            return;

        var handler = LayerHandler.Handlers[LocalPlayer.PlayerId];

        var statusString = "";
        var status = "";

        if (!handler.CurrentModifier.Hidden)
            status += $" {handler.CurrentModifier.ColorString}{handler.CurrentModifier.Name}</color>";

        if (!handler.CurrentDisposition.Hidden)
            status += $" {handler.CurrentDisposition.ColorString}{handler.CurrentDisposition.Name}</color>";

        if (!handler.CurrentAbility.Hidden)
            status += $" {handler.CurrentAbility.ColorString}{handler.CurrentAbility.Name}</color>";

        if (status.Length != 0)
            statusString = $"\n<#{CustomColorManager.Status.ToHtmlStringRGBA()}>Status</color>:{status}";

        __instance.__4__this.RoleText.text = handler.CurrentRole.Name;
        __instance.__4__this.RoleText.color = __instance.__4__this.YouAreText.color = __instance.__4__this.RoleBlurbText.color = __instance.__4__this.BackgroundBar.material.color =
            handler.CurrentRole.Color;
        __instance.__4__this.RoleBlurbText.text = handler.CurrentRole.StartText + statusString;
#if ANDROID
        var pos = __instance.__4__this.BackgroundBar.transform.localPosition;
        __instance.__4__this.BackgroundBar.transform.localPosition = new(pos.x, pos.y, -15f);
#else
        __instance.__4__this.BackgroundBar.transform.SetLocalZ(-15f);
#endif
        __instance.__4__this.YouAreText.GetComponent<TextTranslatorTMP>()?.Destroy();
        __instance.__4__this.YouAreText.text = "You Are The";
    }
}

[HarmonyPatch(typeof(IntroCutscene))]
public static class IntroCutscenePatches
{
    [HarmonyPatch(nameof(IntroCutscene.CreatePlayer))]
    public static void Prefix(ref bool impostorPositioning)
    {
        if (!IsHnS())
            impostorPositioning = true;
    }

    [HarmonyPatch(nameof(IntroCutscene.CreatePlayer))]
    public static void Postfix(ref PoolablePlayer __result)
    {
        if (!IsHnS())
            __result.SetNameColor(LocalPlayer.GetRole().FactionColor);

        __result.transform.localScale *= LocalPlayer.GetSize();
    }

    [HarmonyPatch(nameof(IntroCutscene.SelectTeamToShow))]
    public static void Postfix(ref ISystem.List<PlayerControl> __result)
    {
        if (!IsHnS())
            __result = LocalPlayer.GetRole().Team().DistinctBy(x => x.PlayerId).ToIl2Cpp();
    }

    [HarmonyPatch(nameof(IntroCutscene.CoBegin))]
    public static void Prefix()
    {
        switch (MapPatches.CurrentMap)
        {
            case 0:
            {
                BetterSkeld.ApplyChanges();
                break;
            }
            case 1:
            {
                BetterMiraHq.ApplyChanges();
                break;
            }
            case 2:
            {
                BetterPolus.ApplyChanges();
                break;
            }
            case 4:
            {
                BetterAirship.ApplyChanges();
                break;
            }
        }
    }
}