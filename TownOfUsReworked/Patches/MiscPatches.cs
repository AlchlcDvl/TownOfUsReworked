using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;
using TMPro;

namespace TownOfUsReworked.Patches
{
    public class MiscPatches
    {
        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
        public class EnableMapImps
        {
            private static void Prefix(ref GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
        public static class StopLoadingMainMenu
        {
            public static bool Prefix()
            {
                return !BepInExUpdater.UpdateRequired;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static class UpdatePatch
        {
            public static void Prefix(GameStartManager __instance)
            {
                __instance.MinPlayers = 1;
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        [HarmonyPriority(Priority.First)]
        public class ExileControllerPatch
        {
            public static ExileController lastExiled;
            
            public static void Prefix(ExileController __instance)
            {
                lastExiled = __instance;
            }
        }

        //Vent and kill shit
        //Yes thank you Discussions - AD
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
        class ToggleHighlightPatch
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
            {
                var player = PlayerControl.LocalPlayer;
                bool isActive = Utils.CanInteract(player);

                if (isActive)
                {
                    var role = Role.GetRole(player); 
                    var color = role != null ? role.Color : new Color32(255, 255, 255, 255);
                    ((Renderer)__instance.cosmetics.currentBodySprite.BodySprite).material.SetColor("_OutlineColor", color);
                }
            }
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
        class SetVentOutlinePatch
        {
            public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
            {
                var player = PlayerControl.LocalPlayer;
                bool active = player != null && !MeetingHud.Instance && Utils.CanVent(player, player.Data);

                if (active)
                {
                    var role = Role.GetRole(player); 
                    var color = role != null ? role.Color : new Color32(255, 255, 255, 255);
                    ((Renderer)__instance.myRend).material.SetColor("_OutlineColor", color);
                    ((Renderer)__instance.myRend).material.SetColor("_AddColor", mainTarget ? color : Color.clear);
                }
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
        class MapBehaviourPatch
        {
            static void Postfix(MapBehaviour __instance)
            {
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role != null)
                {
                    __instance.ColorControl.baseColor = role.Color;
                    __instance.ColorControl.SetColor(role.Color);
                }
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
        public class AmBanned
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }
    }
}