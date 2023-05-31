using AmongUs.Data.Player;
using AmongUs.Data.Legacy;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
    public static class EnableMapImps
    {
        public static void Prefix(ref GameSettingMenu __instance) => __instance.HideForOnline = new(0);
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
            var active = PlayerControl.LocalPlayer && !MeetingHud.Instance && PlayerControl.LocalPlayer.CanVent();

            if (!Role.LocalRole || !active)
                return;

            __instance.myRend.material.SetColor("_OutlineColor", Role.LocalRole.Color);
            __instance.myRend.material.SetColor("_AddColor", mainTarget ? Role.LocalRole.Color : Color.clear);
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    public static class MapBehaviourPatch
    {
        public static void Postfix(MapBehaviour __instance)
        {
            foreach (var layer in PlayerLayer.LocalLayers)
                layer?.UpdateMap(__instance);

            foreach (var arrow in CustomArrow.AllArrows)
                arrow?.UpdateArrowBlip(__instance);
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

            text.transform.localPosition = new(0, 0, -20);
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

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
    public static class OpenMapMenuPatch
    {
        public static bool Prefix(MapBehaviour __instance, ref MapOptions opts)
        {
            var notmodified = true;
            var player = PlayerControl.LocalPlayer;

            if (opts.Mode is not MapOptions.Modes.None and not MapOptions.Modes.CountOverlay)
            {
                if (((player.Is(Faction.Syndicate) && CustomGameOptions.AltImps) || player.Is(Faction.Intruder)) && CustomGameOptions.IntrudersCanSabotage)
                    __instance.ShowSabotageMap();
                else
                    __instance.ShowNormalMap();

                __instance.taskOverlay.gameObject.SetActive(player.Is(Faction.Crew));
                notmodified = false;
            }

            foreach (var layer in PlayerLayer.LocalLayers)
                layer?.UpdateMap(__instance);

            return notmodified;
        }
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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static class CanMove
    {
        public static bool Prefix(PlayerControl __instance, ref bool __result)
        {
            __result = __instance.moveable && !Minigame.Instance && !__instance.shapeshifting && (!HudManager.InstanceExists || (!HudManager.Instance.Chat.IsOpen &&
                !HudManager.Instance.KillOverlay.IsOpen && !HudManager.Instance.GameMenu.IsOpen)) && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) &&
                !MeetingHud.Instance && !PlayerCustomizationMenu.Instance && !IntroCutscene.Instance;

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public static class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.IsPostmortal())
                amDead = __instance.myPlayer.Caught();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.IsPostmortal() || (__instance.IsPostmortal() && __instance.Caught()))
                return;

            value = !__instance.inVent;
        }
    }

    [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
    public static class MinigamePatch
    {
        public static void Postfix(Minigame __instance)
        {
            if (!__instance || !PlayerControl.LocalPlayer.Is(AbilityEnum.Multitasker))
                return;

            foreach (var rend in __instance.GetComponentsInChildren<SpriteRenderer>())
                rend.color = new(rend.color.r, rend.color.g, rend.color.b, CustomGameOptions.Transparancy / 100f);
        }
    }

    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class CustomMenuPatch
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            __instance.gameObject.AddComponent<ShapeShifterPagingBehaviour>().shapeshifterMinigame = __instance;
            var menu = CustomMenu.AllMenus.Find(x => x.Menu == __instance && x.Owner == PlayerControl.LocalPlayer);

            if (menu == null)
                return true;

            __instance.potentialVictims = new();
            var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

            for (var i = 0; i < menu.Targets.Count; i++)
            {
                var player = menu.Targets[i];
                var num = i % 3;
                var num2 = i / 3;
                var panel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                panel.SetPlayer(i, player.Data, (Action)(() => menu.Clicked(player)));
                panel.NameText.color = PlayerControl.LocalPlayer == player ? Role.GetRole(menu.Owner).Color : Color.white;
                __instance.potentialVictims.Add(panel);
                list2.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
            return false;
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
    public static class AirshipSpawnInPatch
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Astral) && Modifier.GetModifier<Astral>(PlayerControl.LocalPlayer).LastPosition != Vector3.zero)
                Modifier.GetModifier<Astral>(PlayerControl.LocalPlayer).SetPosition();
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class ExitGamePatch
    {
        public static void Prefix() => CustomOption.SaveSettings("LastUsedSettings");
    }

    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
    public static class GetPurchasePatch
    {
        public static bool Prefix(out bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.SetPurchased))]
    public static class SetPurchasedPatch
    {
        public static bool Prefix() => false;
    }
}