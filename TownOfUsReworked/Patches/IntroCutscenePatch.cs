namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
public static class ShowTeamPatch
{
    public static void Postfix(IntroCutscene._ShowTeam_d__38 __instance)
    {
        if (IsHnS())
            return;

        __instance.__4__this.TeamTitle.text = Role.LocalRole.FactionName;
        __instance.__4__this.TeamTitle.color = Role.LocalRole.FactionColor;
        __instance.__4__this.TeamTitle.outlineColor = UColor.black;
        __instance.__4__this.BackgroundBar.material.color = Role.LocalRole.FactionColor;
        __instance.__4__this.ImpostorText.text = " ";
    }
}

[HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
public static class ShowRolePatch
{
    public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
    {
        if (IsHnS())
            return;

        var role = Role.LocalRole;
        var modifier = Modifier.LocalModifier;
        var disposition = Disposition.LocalDisposition;
        var ability = Ability.LocalAbility;

        var statusString = "";
        var status = "";

        if (!modifier.Hidden)
            status += $" {modifier?.ColorString}{modifier?.Name}</color>";

        if (!disposition.Hidden)
            status += $" {disposition?.ColorString}{disposition?.Name}</color>";

        if (!ability.Hidden)
            status += $" {ability?.ColorString}{ability?.Name}</color>";

        if (CustomPlayer.Local.IsRecruit())
            status += " <color=#575657FF>Recruited</color>";

        if (status.Length != 0)
            statusString = $"\n<color=#{CustomColorManager.Status.ToHtmlStringRGBA()}>Status</color>:{status}";

        __instance.__4__this.RoleText.text = role.Name;
        __instance.__4__this.RoleText.color = role.Color;
        __instance.__4__this.YouAreText.color = role.Color;
        __instance.__4__this.RoleBlurbText.text = role.StartText() + statusString;
        __instance.__4__this.RoleBlurbText.color = role.Color;
        Coroutines.Start(PerformTimedAction(0.01f, _ => __instance.__4__this.YouAreText.text = "You Are The"));
    }
}

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
public static class CreatePlayerPatch
{
    public static void Prefix(ref bool impostorPositioning)
    {
        if (!IsHnS())
            impostorPositioning = true;
    }

    public static void Postfix(ref PoolablePlayer __result)
    {
        if (!IsHnS())
            __result.SetNameColor(Role.LocalRole.FactionColor);
    }
}

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.SelectTeamToShow))]
public static class OverrideShowTeam
{
    public static void Postfix(ref ISystem.List<PlayerControl> __result)
    {
        if (!IsHnS())
            __result = Role.LocalRole.Team().ToIl2Cpp();
    }
}