using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;
using TMPro;
using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using TownOfUsReworked.Crowded.Components;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.PlayerLayers;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public static class EnableMapImps
    {
        public static void Prefix(ref GameSettingMenu __instance) => __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
    }

    [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.IsAffectedByComms), MethodType.Getter)]
    public static class ButtonsPatch
    {
        public static void Postfix(ref bool __result) => __result = false;
    }

    //Vent and kill shit
    //Yes thank you Discussions - AD
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
    public static class SetVentOutlinePatch
    {
        public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
        {
            var active = PlayerControl.LocalPlayer != null && !MeetingHud.Instance && PlayerControl.LocalPlayer.CanVent();
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role == null || !active)
                return;

            __instance.myRend.material.SetColor("_OutlineColor", role.Color);
            __instance.myRend.material.SetColor("_AddColor", mainTarget ? role.Color : Color.clear);
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    public static class MapBehaviourPatch
    {
        public static void Postfix(MapBehaviour __instance)
        {
            foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                layer?.UpdateMap(__instance);
        }
    }

    [HarmonyPatch(typeof(PooledMapIcon), nameof(PooledMapIcon.Reset))]
    public static class PooledMapIconPatch
    {
        public static void Postfix(PooledMapIcon __instance)
        {
            var sprite = __instance.GetComponent<SpriteRenderer>();

            if (sprite != null)
                PlayerMaterial.SetColors(new Color(0.8793f, 1, 0, 1), sprite);

            var text = __instance.GetComponentInChildren<TextMeshPro>(true);

            if (text == null)
            {
                text = new GameObject("Text").AddComponent<TextMeshPro>();
                text.transform.SetParent(__instance.transform, false);
                text.fontSize = 1.5f;
                text.fontSizeMin = 1;
                text.fontSizeMax = 1.5f;
                text.enableAutoSizing = true;
                text.fontStyle = FontStyles.Bold;
                text.alignment = TextAlignmentOptions.Center;
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.gameObject.layer = 5;
                text.fontMaterial.EnableKeyword("OUTLINE_ON");
                text.fontMaterial.SetFloat("_OutlineWidth", 0.1745f);
                text.fontMaterial.SetFloat("_FaceDilate", 0.151f);
            }

            text.transform.localPosition = new Vector3(0, 0, -20);
            text.text = "";
            text.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static class AmBanned
    {
        public static void Postfix(out bool __result) => __result = false;
    }

    [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
    public static class SaveManagerPatch
    {
        public static void Postfix(ref string __result) => __result += "_ToU-Rew";
    }

    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.GetPrefsName))]
    public static class LegacySaveManagerPatch
    {
        public static void Postfix(ref string __result) => __result += "_ToU-Rew";
    }

    [HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
    public static class SecurityLoggerPatch
    {
        public static void Postfix(ref SecurityLogger __instance) => __instance.Timers = new float[127];
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public static class VitalsMinigameBeginPatch
    {
        public static void Postfix(VitalsMinigame __instance) => __instance.gameObject.AddComponent<VitalsPagingBehaviour>().vitalsMinigame = __instance;
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    public static class Sabotage
    {
        public static bool Prefix() => (PlayerControl.LocalPlayer.Is(Faction.Intruder) && CustomGameOptions.IntrudersCanSabotage) || (PlayerControl.LocalPlayer.Is(Faction.Syndicate) &&
            CustomGameOptions.AltImps);
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class DisconnectHandler
    {
        public readonly static List<PlayerControl> Disconnected = new();

        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            Utils.ReassignPostmortals(player);
            Disconnected.Add(player);
        }
    }

    [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
    public static class SyncToiletDoor
    {
        public static void Prefix(OpenDoorConsole __instance)
        {
            var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DoorSyncToilet, SendOption.Reliable);
            messageWriter.Write(__instance.MyDoor.Id);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }
}