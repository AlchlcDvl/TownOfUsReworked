using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    public static class IntroCutScenePatch
    {
        [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__36), nameof(IntroCutscene._ShowTeam_d__36.MoveNext))]
        public static class IntroCutscene_ShowTeam__d_21
        {
            public static void Prefix(IntroCutscene._ShowTeam_d__36 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);
                role?.IntroPrefix(__instance);
            }

            public static void Postfix(IntroCutscene._ShowTeam_d__36 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);
                __instance.__4__this.TeamTitle.text = role.FactionName;
                __instance.__4__this.TeamTitle.color = role.FactionColor;
                __instance.__4__this.TeamTitle.outlineColor = Color.black;
                __instance.__4__this.BackgroundBar.material.color = role.Color;
                __instance.__4__this.ImpostorText.text = " ";
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
        public static class IntroCutscene_ShowRole_d__24
        {
            public static void Postfix(IntroCutscene._ShowRole_d__39 __instance)
            {
                var player = PlayerControl.LocalPlayer;
                var role = Role.GetRole(player);
                var modifier = Modifier.GetModifier(player);
                var objectifier = Objectifier.GetObjectifier(player);
                var ability = Ability.GetAbility(player);

                var statusString = "";
                var status = "";

                if (!modifier.Hidden)
                    status += $" {modifier?.ColorString}{modifier?.Name}</color>";

                if (!objectifier.Hidden)
                    status += $" {objectifier?.ColorString}{objectifier?.Name}</color>";

                if (!ability.Hidden)
                    status += $" {ability?.ColorString}{ability?.Name}</color>";

                if (status.Length != 0)
                    statusString = $"\n<size=4><color=#{Colors.Status.ToHtmlStringRGBA()}>Status</color>:{status}</size>";

                __instance.__4__this.RoleText.text = role.Name;
                __instance.__4__this.RoleText.color = role.Color;
                __instance.__4__this.YouAreText.color = role.Color;
                __instance.__4__this.YouAreText.text = "You Are The";
                __instance.__4__this.RoleBlurbText.text = role.StartText + statusString;
                __instance.__4__this.RoleBlurbText.color = role.Color;

                if (AssetManager.Sounds.Contains(role.IntroSound) && !role.IntroPlayed)
                {
                    SoundManager.Instance.StopSound(PlayerControl.LocalPlayer.Data.Role.IntroSound);
                    AssetManager.Play(role.IntroSound);
                    role.IntroPlayed = true;
                }

                var flag = !role.Base && ((CustomGameOptions.CustomCrewColors && PlayerControl.LocalPlayer.Is(Faction.Crew)) || (CustomGameOptions.CustomIntColors &&
                    PlayerControl.LocalPlayer.Is(Faction.Intruder)) || (CustomGameOptions.CustomSynColors && PlayerControl.LocalPlayer.Is(Faction.Syndicate)) ||
                    (CustomGameOptions.CustomNeutColors && PlayerControl.LocalPlayer.Is(Faction.Neutral)));

                if (flag)
                    __instance.__4__this.RoleText.outlineColor = role.FactionColor;
            }
        }
    }
}