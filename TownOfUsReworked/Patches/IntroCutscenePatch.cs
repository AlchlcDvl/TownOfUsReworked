namespace TownOfUsReworked.Patches
{
    public static class IntroCutScenePatch
    {
        [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__36), nameof(IntroCutscene._ShowTeam_d__36.MoveNext))]
        public static class IntroCutscene_ShowTeam__d_21
        {
            public static void Prefix(IntroCutscene._ShowTeam_d__36 __instance) => Role.LocalRole.IntroPrefix(__instance);

            public static void Postfix(IntroCutscene._ShowTeam_d__36 __instance)
            {
                __instance.__4__this.TeamTitle.text = Role.LocalRole.FactionName;
                __instance.__4__this.TeamTitle.color = Role.LocalRole.FactionColor;
                __instance.__4__this.TeamTitle.outlineColor = Color.black;
                __instance.__4__this.BackgroundBar.material.color = Role.LocalRole.Color;
                __instance.__4__this.ImpostorText.text = " ";
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
        public static class IntroCutscene_ShowRole_d__24
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

                if (status.Length != 0)
                    statusString = $"\n<color=#{Colors.Status.ToHtmlStringRGBA()}>Status</color>:{status}";

                __instance.__4__this.RoleText.text = role.Name;
                __instance.__4__this.RoleText.color = role.Color;
                __instance.__4__this.YouAreText.color = role.Color;
                __instance.__4__this.YouAreText.text = "You Are The";
                __instance.__4__this.RoleBlurbText.text = role.StartText + statusString;
                __instance.__4__this.RoleBlurbText.color = role.Color;

                /*if (AssetManager.Sounds.Contains(role.IntroSound) && !role.IntroPlayed)
                {
                    SoundManager.Instance.StopSound(PlayerControl.LocalPlayer.Data.Role.IntroSound);
                    AssetManager.Play(role.IntroSound);
                    role.IntroPlayed = true;
                }*/

                var flag = !role.Base && ((CustomGameOptions.CustomCrewColors && PlayerControl.LocalPlayer.Is(Faction.Crew)) || (CustomGameOptions.CustomIntColors &&
                    PlayerControl.LocalPlayer.Is(Faction.Intruder)) || (CustomGameOptions.CustomSynColors && PlayerControl.LocalPlayer.Is(Faction.Syndicate)) ||
                    (CustomGameOptions.CustomNeutColors && PlayerControl.LocalPlayer.Is(Faction.Neutral)));

                if (flag)
                    __instance.__4__this.RoleText.outlineColor = role.FactionColor;
            }
        }
    }
}