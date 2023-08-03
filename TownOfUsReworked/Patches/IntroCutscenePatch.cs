namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__36), nameof(IntroCutscene._ShowTeam_d__36.MoveNext))]
    public static class ShowTeamPatch
    {
        public static void Prefix(IntroCutscene._ShowTeam_d__36 __instance) => Role.LocalRole.IntroPrefix(__instance);

        public static void Postfix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            __instance.__4__this.TeamTitle.text = Role.LocalRole.FactionName;
            __instance.__4__this.TeamTitle.color = Role.LocalRole.FactionColor;
            __instance.__4__this.TeamTitle.outlineColor = UColor.black;
            __instance.__4__this.BackgroundBar.material.color = Role.LocalRole.Color;
            __instance.__4__this.ImpostorText.text = " ";
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
    public static class ShowRolePatch
    {
        public static void Postfix(IntroCutscene._ShowRole_d__39 __instance)
        {
            var role = Role.LocalRole;
            var modifier = Modifier.LocalModifier;
            var objectifier = Objectifier.LocalObjectifier;
            var ability = Ability.LocalAbility;

            var statusString = "";
            var status = "";

            if (!modifier.Hidden)
                status += $" {modifier?.ColorString}{modifier?.Name}</color>";

            if (!objectifier.Hidden)
                status += $" {objectifier?.ColorString}{objectifier?.Name}</color>";

            if (!ability.Hidden)
                status += $" {ability?.ColorString}{ability?.Name}</color>";

            if (CustomPlayer.Local.IsRecruit())
                status += " <color=#575657FF>Recruited</color>";

            if (status.Length != 0)
                statusString = $"\n<color=#{Colors.Status.ToHtmlStringRGBA()}>Status</color>:{status}";

            __instance.__4__this.RoleText.text = role.Name;
            __instance.__4__this.RoleText.color = role.Color;
            __instance.__4__this.YouAreText.color = role.Color;
            __instance.__4__this.RoleBlurbText.text = role.StartText() + statusString;
            __instance.__4__this.RoleBlurbText.color = role.Color;
            __instance.__4__this.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.__4__this.YouAreText.text = "You Are The")));
        }
    }
}