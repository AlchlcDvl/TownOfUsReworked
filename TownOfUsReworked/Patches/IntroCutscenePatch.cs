using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    public static class IntroCutScenePatch
    {
        private static TextMeshPro StatusText;

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        public static class IntroCutscene_BeginCrewmate
        {
            public static void Postfix(IntroCutscene __instance)
            {
                var player = PlayerControl.LocalPlayer;
                var modifier = Modifier.GetModifier(player);
                var objectifier = Objectifier.GetObjectifier(player);
                var ability = Ability.GetAbility(player);
                var flag = modifier == null && ability == null && objectifier == null;

                StatusText = !flag ? Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false) : null;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        public static class IntroCutscene_BeginImpostor
        {
            public static void Postfix(IntroCutscene __instance)
            {
                var player = PlayerControl.LocalPlayer;
                var modifier = Modifier.GetModifier(player);
                var objectifier = Objectifier.GetObjectifier(player);
                var ability = Ability.GetAbility(player);
                var flag = modifier == null && ability == null && objectifier == null;

                StatusText = !flag ? Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false) : null;
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
        public static class IntroCutscene_CoBegin_d__29
        {
            public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role != null)
                {
                    __instance.__4__this.TeamTitle.text = role.FactionName;
                    __instance.__4__this.TeamTitle.color = role.FactionColor;
                    __instance.__4__this.TeamTitle.outlineColor = Color.black;
                    __instance.__4__this.RoleText.text = role.Name;
                    __instance.__4__this.RoleText.color = role.Color;
                    __instance.__4__this.YouAreText.color = role.Color;
                    __instance.__4__this.YouAreText.text = "You Are The";
                    __instance.__4__this.BackgroundBar.material.color = role.Color;
                    __instance.__4__this.ImpostorText.text = " ";
                    __instance.__4__this.RoleBlurbText.color = role.Color;
                    __instance.__4__this.RoleBlurbText.text = role.StartText;

                    var flag = !role.Base && ((CustomGameOptions.CustomCrewColors && PlayerControl.LocalPlayer.Is(Faction.Crew)) || (CustomGameOptions.CustomIntColors &&
                        PlayerControl.LocalPlayer.Is(Faction.Intruder)) || (CustomGameOptions.CustomSynColors && PlayerControl.LocalPlayer.Is(Faction.Syndicate)) ||
                        (CustomGameOptions.CustomNeutColors && PlayerControl.LocalPlayer.Is(Faction.Neutral)));

                    if (flag)
                        __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                }

                if (StatusText != null)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);

                    var statusString = "";
                    var status = "";

                    if (modifier?.Hidden == false)
                        status += $" {modifier.ColorString}{modifier.Name}</color>";

                    if (objectifier?.Hidden == false)
                        status += $" {objectifier.ColorString}{objectifier.Name}</color>";

                    if (ability?.Hidden == false)
                        status += $" {ability.ColorString}{ability.Name}</color>";

                    if (status.Length != 0)
                        statusString = "<size=4><color=#" + Colors.Status.ToHtmlStringRGBA() + ">Status</color>:" + statusString + status + "</size>";

                    StatusText.text = statusString;
                    StatusText.outlineColor = Colors.Status;

                    StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                    StatusText.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__32), nameof(IntroCutscene._ShowTeam_d__32.MoveNext))]
        public static class IntroCutscene_ShowTeam__d_21
        {
            public static void Prefix(IntroCutscene._ShowTeam_d__32 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);
                role?.IntroPrefix(__instance);
            }

            public static void Postfix(IntroCutscene._ShowTeam_d__32 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role != null)
                {
                    __instance.__4__this.TeamTitle.text = role.FactionName;
                    __instance.__4__this.TeamTitle.color = role.FactionColor;
                    __instance.__4__this.TeamTitle.outlineColor = Color.black;
                    __instance.__4__this.BackgroundBar.material.color = role.Color;
                    __instance.__4__this.ImpostorText.text = " ";
                }

                if (StatusText != null)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);

                    var statusString = "";
                    var status = "";

                    if (modifier?.Hidden == false)
                        status += $" {modifier.ColorString}{modifier.Name}</color>";

                    if (objectifier?.Hidden == false)
                        status += $" {objectifier.ColorString}{objectifier.Name}</color>";

                    if (ability?.Hidden == false)
                        status += $" {ability.ColorString}{ability.Name}</color>";

                    if (status.Length != 0)
                        statusString = "<size=4><color=#" + Colors.Status.ToHtmlStringRGBA() + ">Status</color>:" + statusString + status + "</size>";

                    StatusText.text = statusString;
                    StatusText.outlineColor = Colors.Status;

                    StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                    StatusText.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__35), nameof(IntroCutscene._ShowRole_d__35.MoveNext))]
        public static class IntroCutscene_ShowRole_d__24
        {
            public static void Postfix(IntroCutscene._ShowRole_d__35 __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role != null)
                {
                    __instance.__4__this.RoleText.text = role.Name;
                    __instance.__4__this.RoleText.color = role.Color;
                    __instance.__4__this.YouAreText.color = role.Color;
                    __instance.__4__this.YouAreText.text = "You Are The";
                    __instance.__4__this.RoleBlurbText.text = role.StartText;
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
}