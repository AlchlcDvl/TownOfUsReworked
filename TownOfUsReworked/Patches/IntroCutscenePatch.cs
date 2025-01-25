namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
public static class ShowTeamPatch
{
    public static void Postfix(IntroCutscene._ShowTeam_d__38 __instance)
    {
        if (IsHnS())
            return;

        ShowRolePatch.Starting = true;
        var role = CustomPlayer.Local.GetRole();
        __instance.__4__this.TeamTitle.text = role.FactionName;
        __instance.__4__this.TeamTitle.color = role.FactionColor;
        __instance.__4__this.TeamTitle.outlineColor = UColor.black;
        __instance.__4__this.BackgroundBar.material.color = role.FactionColor;
        __instance.__4__this.ImpostorText.text = "";
    }
}

[HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
public static class ShowRolePatch
{
    public static bool Starting;

    public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
    {
        if (IsHnS())
            return;

        Starting = true;
        var role = CustomPlayer.Local.GetRole();
        var modifier = CustomPlayer.Local.GetModifier();
        var disposition = CustomPlayer.Local.GetDisposition();
        var ability = CustomPlayer.Local.GetAbility();

        var statusString = "";
        var status = "";

        if (!modifier.Hidden)
            status += $" {modifier.ColorString}{modifier.Name}</color>";

        if (!disposition.Hidden)
            status += $" {disposition.ColorString}{disposition.Name}</color>";

        if (!ability.Hidden)
            status += $" {ability.ColorString}{ability.Name}</color>";

        if (role.IsRecruit && role.Alignment != Alignment.Neophyte)
            status += " <#575657FF>Recruited</color>";

        if (status.Length != 0)
            statusString = $"\n<#{CustomColorManager.Status.ToHtmlStringRGBA()}>Status</color>:{status}";

        __instance.__4__this.RoleText.text = role.Name;
        __instance.__4__this.RoleText.color = __instance.__4__this.YouAreText.color = __instance.__4__this.RoleBlurbText.color = __instance.__4__this.BackgroundBar.material.color = role.Color;
        __instance.__4__this.RoleBlurbText.text = role.StartText() + statusString;
        __instance.__4__this.BackgroundBar.transform.SetLocalZ(-15f);
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
            __result.SetNameColor(CustomPlayer.Local.GetRole().FactionColor);
    }

    [HarmonyPatch(nameof(IntroCutscene.SelectTeamToShow))]
    public static void Postfix(ref ISystem.List<PlayerControl> __result)
    {
        if (!IsHnS())
        {
            var result = CustomPlayer.Local.GetRole().Team();
            var copy = new List<PlayerControl>();

            foreach (var player in result)
            {
                if (result.Count(x => x == player) > 1 && !copy.Contains(player))
                    copy.Add(player);
            }

            result.RemoveRange(copy); // Removes all copies of players that appeared more than once
            result.AddRange(copy); // Adds only one instance of each copied player back
            __result = result.ToIl2Cpp();
        }
    }

    [HarmonyPatch(nameof(IntroCutscene.CoBegin))]
    public static void Prefix()
    {
        if (MapPatches.CurrentMap == 0)
            BetterSkeld.ApplyChanges();
        else if (MapPatches.CurrentMap == 1)
            BetterMiraHQ.ApplyChanges();
        else if (MapPatches.CurrentMap == 2)
            BetterPolus.ApplyChanges();
    }
}