﻿using System;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Lobby.CrowdedMod.Patches
{
    internal static class CreateGameOptionsPatches
    {
        [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
        public static class CreateOptionsPicker_Awake
        {
            public static void Postfix(CreateOptionsPicker __instance)
            {
                if (__instance.mode != SettingsMode.Host)
                    return;

                {
                    var firstButtonRenderer = __instance.MaxPlayerButtons[0];
                    firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";
                    firstButtonRenderer.enabled = false;

                    var firstButtonButton = firstButtonRenderer.GetComponent<PassiveButton>();
                    firstButtonButton.OnClick.RemoveAllListeners();
                    firstButtonButton.OnClick.AddListener((Action)(() =>
                    {
                        for (var i = 1; i < 11; i++)
                        {
                            var playerButton = __instance.MaxPlayerButtons[i];

                            var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                            var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name));
                            tmp.text = newValue.ToString();
                        }

                        __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
                    }));

                    firstButtonRenderer.Destroy();

                    var lastButtonRenderer = __instance.MaxPlayerButtons[__instance.MaxPlayerButtons.Count - 1];
                    lastButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";
                    lastButtonRenderer.enabled = false;

                    var lastButtonButton = lastButtonRenderer.GetComponent<PassiveButton>();
                    lastButtonButton.OnClick.RemoveAllListeners();
                    lastButtonButton.OnClick.AddListener((Action)(() =>
                    {
                        for (var i = 1; i < 11; i++)
                        {
                            var playerButton = __instance.MaxPlayerButtons[i];

                            var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                            var newValue = Mathf.Min(byte.Parse(tmp.text) + 10,
                                TownOfUsReworked.MaxPlayers - 14 + byte.Parse(playerButton.name));
                            tmp.text = newValue.ToString();
                        }

                        __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
                    }));

                    lastButtonRenderer.Destroy();

                    for (var i = 1; i < 11; i++)
                    {
                        var playerButton = __instance.MaxPlayerButtons[i].GetComponent<PassiveButton>();
                        var text = playerButton.GetComponentInChildren<TextMeshPro>();

                        playerButton.OnClick.RemoveAllListeners();
                        playerButton.OnClick.AddListener((Action)(() => __instance.SetMaxPlayersButtons(byte.Parse(text.text))));

                        for (var j = 0; j < __instance.MaxPlayerButtons.Count; j++)
                        {
                            __instance.MaxPlayerButtons[j].enabled = __instance.MaxPlayerButtons[j].GetComponentInChildren<TextMeshPro>().text ==
                                __instance.GetTargetOptions().MaxPlayers.ToString();
                        }
                    }
                }

                {
                    var secondButtonRenderer = __instance.ImpostorButtons[1];
                    var secondButton = secondButtonRenderer.gameObject;
                    secondButtonRenderer.enabled = false;
                    secondButton.transform.FindChild("ConsoleHighlight").gameObject.Destroy();
                    secondButton.GetComponent<PassiveButton>().Destroy();
                    secondButton.GetComponent<BoxCollider2D>().Destroy();

                    var secondButtonText = secondButton.GetComponentInChildren<TextMeshPro>();
                    secondButtonText.text = __instance.GetTargetOptions().NumImpostors.ToString();

                    var firstButtonRenderer = __instance.ImpostorButtons[0];
                    firstButtonRenderer.enabled = false;
                    firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";

                    var firstButton = firstButtonRenderer.GetComponent<PassiveButton>();
                    firstButton.OnClick.RemoveAllListeners();
                    firstButton.OnClick.AddListener((Action)(() =>
                    {
                        var newVal = Mathf.Max(byte.Parse(secondButtonText.text) - 1, 1);
                        __instance.SetImpostorButtons(newVal);
                        secondButtonText.text = newVal.ToString();
                    }));

                    var thirdButtonRenderer = __instance.ImpostorButtons[2];
                    thirdButtonRenderer.enabled = false;
                    thirdButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";

                    var thirdButton = thirdButtonRenderer.GetComponent<PassiveButton>();
                    thirdButton.OnClick.RemoveAllListeners();
                    thirdButton.OnClick.AddListener((Action)(() =>
                    {
                        var newVal = Mathf.Min(byte.Parse(secondButtonText.text) + 1, __instance.GetTargetOptions().MaxPlayers / 2);
                        __instance.SetImpostorButtons(newVal);
                        secondButtonText.text = newVal.ToString();
                    }));
                }
            }
        }

        [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
        public static class CreateOptionsPicker_UpdateMaxPlayersButtons
        {
            public static bool Prefix(CreateOptionsPicker __instance, [HarmonyArgument(0)] IGameOptions opts)
            {
                if (__instance.CrewArea)
                    __instance.CrewArea.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);

                var selectedAsString = opts.MaxPlayers.ToString();
                
                for (var i = 1; i < __instance.MaxPlayerButtons.Count - 1; i++)
                {
                    __instance.MaxPlayerButtons[i].enabled = __instance.MaxPlayerButtons[i].GetComponentInChildren<TextMeshPro>().text ==
                        selectedAsString;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
        public static class CreateOptionsPicker_UpdateImpostorsButtons
        {
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}