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
        public static bool Prefix(ref bool __result) => __result = false;
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    public static class GameSettingMenuOnEnable
    {
        public static void Prefix(ref GameSettingMenu __instance) => __instance.HideForOnline = new(0);
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

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
    public static class OpenMapMenuPatch
    {
        public static bool Prefix(MapBehaviour __instance, ref MapOptions opts)
        {
            if (PlayerLayer.LocalLayers.All(x => x.IsBlocked))
                return false;

            var notmodified = true;
            var player = CustomPlayer.Local;

            if (opts.Mode is not MapOptions.Modes.None and not MapOptions.Modes.CountOverlay)
            {
                if (((player.Is(Faction.Syndicate) && CustomGameOptions.AltImps) || player.Is(Faction.Intruder)) && CustomGameOptions.IntrudersCanSabotage && (!player.Data.IsDead ||
                    (player.Data.IsDead && CustomGameOptions.GhostsCanSabotage)) && Role.GetRoles(player.GetFaction()).Any(x => !x.IsDead))
                {
                    __instance.ShowSabotageMap();
                }
                else
                    __instance.ShowNormalMap();

                __instance.taskOverlay.gameObject.SetActive(player.Is(Faction.Crew));
                notmodified = false;
            }

            foreach (var layer in PlayerLayer.LocalLayers)
                layer?.UpdateMap(__instance);

            foreach (var arrow in CustomArrow.AllArrows)
                arrow?.UpdateArrowBlip(__instance);

            return notmodified;
        }
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class DisconnectHandler
    {
        public static readonly List<PlayerControl> Disconnected = new();

        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player);

            if (IsLobby)
                return;

            ReassignPostmortals(player);
            Disconnected.Add(player);
            Summary.AddSummaryInfo(player, true);

            if (Meeting)
                MarkMeetingDead(player, player, false, true);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static class CanMove
    {
        public static bool Prefix(PlayerControl __instance, ref bool __result)
        {
            __result = __instance.moveable && !Minigame.Instance && !__instance.shapeshifting && (!HudManager.InstanceExists || (!HUD.Chat.IsOpenOrOpening &&
                !HUD.KillOverlay.IsOpen && !HUD.GameMenu.IsOpen)) && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) && !Meeting &&
                !IntroCutscene.Instance && !PlayerCustomizationMenu.Instance;
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

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public static class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.IsPostmortal())
                __instance.myPlayer.Collider.enabled = !__instance.myPlayer.Caught();
        }
    }

    [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
    public static class MinigamePatch
    {
        public static void Postfix(Minigame __instance)
        {
            if (!__instance || ! CustomPlayer.Local.Is(AbilityEnum.Multitasker))
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
            __instance.gameObject.AddComponent<MenuPagingBehaviour>().Menu = __instance;
            var menu = CustomMenu.AllMenus.Find(x => x.Menu == __instance && x.Owner == CustomPlayer.Local);

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
                panel.NameText.color = CustomPlayer.Local == player ? Role.GetRole(menu.Owner).Color : UColor.white;
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
            if (CustomPlayer.Local.Is(ModifierEnum.Astral) && Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition != Vector3.zero)
                Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();
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

    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    public static class UseButtonSetTargetPatch
    {
        public static bool Prefix(UseButton __instance)
        {
            InteractableBehaviour.ClosestTasks(CustomPlayer.Local);

            if (__instance.isActiveAndEnabled && CustomPlayer.LocalCustom && InteractableBehaviour.NearestTask != null && InteractableBehaviour.AllCustomPlateform.Count > 0)
            {
                __instance.graphic.color = new(1f, 1f, 1f, 1f);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.Initialize))]
    public static class OverlayKillAnimationPatch
    {
        private static int CurrentOutfitTypeCache;

        public static void Prefix(GameData.PlayerInfo kInfo)
        {
            var playerControl = CustomPlayer.AllPlayers.Find(p => p.PlayerId == kInfo.PlayerId);
            CurrentOutfitTypeCache = (int)playerControl.CurrentOutfitType;

            if (!CustomGameOptions.AppearanceAnimation)
                playerControl.CurrentOutfitType = PlayerOutfitType.Default;
        }

        public static void Postfix(GameData.PlayerInfo kInfo)
        {
            var playerControl = CustomPlayer.AllPlayers.Find(p => p.PlayerId == kInfo.PlayerId);
            playerControl.CurrentOutfitType = (PlayerOutfitType)CurrentOutfitTypeCache;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    public static class AddCustomPlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!CustomPlayer.AllCustomPlayers.Any(x => x.Player == __instance))
                _ = new CustomPlayer(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    public static class RemoveCustomPlayerPatch
    {
        public static void Prefix(PlayerControl __instance) => CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == __instance || x.Player == null);
    }

    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    [HarmonyPriority(Priority.First)]
    public static class RefreshPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class LobbySizePatch
    {
        public static void Postfix()
        {
            if (GameStartManager.Instance.LastPlayerCount > CustomGameOptions.LobbySize && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
            {
                while (CustomPlayer.AllPlayers.Count > CustomGameOptions.LobbySize)
                {
                    var player = CustomPlayer.AllPlayers[^1];
                    var client = AmongUsClient.Instance.GetClient(player.OwnerId);
                    AmongUsClient.Instance.KickPlayer(client.Id, false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SizePatch
    {
        public static void Postfix()
        {
            foreach (var player in CustomPlayer.AllPlayers.ToArray())
                player.transform.localScale = !(player.Data.IsDead || player.Data.Disconnected) ? CustomPlayer.Custom(player).SizeFactor : new(0.7f, 0.7f, 1f);

            foreach (var body in AllBodies)
                body.transform.localScale = CustomPlayer.Custom(PlayerByBody(body)).SizeFactor;
        }
    }

    [HarmonyPatch]
    public static class SpeedPatch
    {
        private static float _time;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                __instance.body.velocity *= CustomPlayer.Custom(__instance.myPlayer).SpeedFactor;

            if (__instance.myPlayer.Is(ModifierEnum.Flincher) && !__instance.myPlayer.Data.IsDead && __instance.myPlayer.CanMove && !Meeting)
            {
                _time += Time.deltaTime;

                if (_time >= CustomGameOptions.FlinchInterval && __instance.AmOwner)
                {
                    __instance.body.velocity *= -1;
                    _time -= CustomGameOptions.FlinchInterval;
                }
            }
        }

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && __instance.interpolateMovement != 0 && GameData.Instance)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= IsLobby ? CustomGameOptions.PlayerSpeed : CustomPlayer.Custom(player).SpeedFactor;
            }
        }
    }

    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    public static class BegoneModstamp
    {
        public static void Postfix(ModManager __instance)
        {
            if (__instance == null)
                return;

            __instance.ModStamp.gameObject.SetActive(NoPlayers || LobbyBehaviour.Instance);
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    public static class ConstantsPatch
    {
        public static void Postfix(ref int __result)
        {
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame)
                __result = Constants.GetVersion(2222, 0, 0, 0);
        }
    }
}