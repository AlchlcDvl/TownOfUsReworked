using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using Discord;

namespace TownOfUsReworked.Patches;

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

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviourPatch
{
    public static void Postfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers.ForEach(x => x?.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x?.UpdateArrowBlip(__instance));
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
            text = new GameObject("Text") { layer = 5 }.AddComponent<TextMeshPro>();
            text.transform.SetParent(__instance.transform, false);
            text.fontSize = 1.5f;
            text.fontSizeMin = 1;
            text.fontSizeMax = 1.5f;
            text.enableAutoSizing = true;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
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

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class CloseMapMenuPatch
{
    public static void Postfix() => CustomPlayer.Local.EnableButtons();
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
public static class OpenMapMenuPatch
{
    public static bool Prefix(MapBehaviour __instance, ref MapOptions opts)
    {
        if (PlayerLayer.LocalLayers.All(x => x.IsBlocked))
            return false;

        var notmodified = true;

        if (opts.Mode is not (MapOptions.Modes.None or MapOptions.Modes.CountOverlay))
        {
            if (CustomPlayer.Local.CanSabotage())
                __instance.ShowSabotageMap();
            else
                __instance.ShowNormalMap();

            __instance.taskOverlay.gameObject.SetActive(CustomPlayer.Local.CanDoTasks() && !IsTaskRace && !IsCustomHnS);
            notmodified = false;
        }

        return notmodified;
    }

    public static void Postfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers.ForEach(x => x?.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x?.UpdateArrowBlip(__instance));
        CustomPlayer.Local.DisableButtons();
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class DisconnectHandler
{
    public static readonly List<PlayerControl> Disconnected = new();

    public static void Prefix(ref PlayerControl player)
    {
        var player2 = player;
        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player2 || x.Player == null);

        if (IsLobby)
            return;

        ReassignPostmortals(player);
        Disconnected.Add(player);
        OnGameEndPatch.AddSummaryInfo(player, true);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
public static class CanMove
{
    public static bool Prefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable && !ActiveTask && !__instance.shapeshifting && (!HudManager.InstanceExists || (!HUD.Chat.IsOpenOrOpening && !HUD.KillOverlay.IsOpen &&
            !HUD.GameMenu.IsOpen)) && (!Map || !Map.IsOpenStopped) && !Meeting && !IntroCutscene.Instance && !PlayerCustomizationMenu.Instance;
        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class HandleAnimation
{
    public static void Prefix(PlayerPhysics __instance, ref bool amDead)
    {
        if (__instance.myPlayer.IsPostmortal())
            amDead = __instance.myPlayer.Caught();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
public static class VisibleOverride
{
    public static void Prefix(PlayerControl __instance, ref bool value)
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
public static class MinigameBeginPatch
{
    public static void Postfix(Minigame __instance)
    {
        if (!__instance)
            return;

        CustomPlayer.Local.DisableButtons();

        if (!CustomPlayer.Local.Is(LayerEnum.Multitasker))
            return;

        __instance.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.color = new(x.color.r, x.color.g, x.color.b, CustomGameOptions.Transparancy / 100f));
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
        var list2 = new ISystem.List<UiElement>();

        for (var i = 0; i < menu.Targets.Count; i++)
        {
            var player = menu.Targets[i];
            var num = i % 3;
            var num2 = i / 3;
            var panel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
            panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
            panel.SetPlayer(i, player.Data, (Action)(() => menu.Clicked(player)));
            (panel.NameText.text, panel.NameText.color) = UpdateNames.UpdateGameName(player);
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
        if (CustomPlayer.Local.Is(LayerEnum.Astral) && Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition != Vector3.zero)
            Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
public static class ExitGamePatch
{
    public static void Prefix()
    {
        SaveText("ReworkedLogs.txt", SavedLogs);
        CustomOption.SaveSettings("LastUsedSettings");
    }
}

[HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
public static class GetPurchasePatch
{
    public static bool Prefix(ref bool __result)
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

[HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.Initialize))]
public static class OverlayKillAnimationPatch
{
    private static int CurrentOutfitTypeCache;

    public static void Prefix(ref GameData.PlayerInfo kInfo)
    {
        var playerControl = PlayerById(kInfo.PlayerId);
        CurrentOutfitTypeCache = (int)playerControl.CurrentOutfitType;

        if (!CustomGameOptions.AppearanceAnimation)
            playerControl.CurrentOutfitType = PlayerOutfitType.Default;
    }

    public static void Postfix(GameData.PlayerInfo kInfo) => PlayerById(kInfo.PlayerId).CurrentOutfitType = (PlayerOutfitType)CurrentOutfitTypeCache;
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
        CustomPlayer.AllPlayers.ForEach(x => x.transform.localScale = CustomPlayer.Custom(x).SizeFactor);
        AllBodies.ForEach(x => x.transform.localScale = CustomPlayer.Custom(PlayerByBody(x)).SizeFactor);
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
public static class SpeedPhysicsPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            __instance.body.velocity *= CustomPlayer.Custom(__instance.myPlayer).SpeedFactor;
    }
}

[HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
public static class SpeedNetworkPatch
{
    public static void Postfix(CustomNetworkTransform __instance)
    {
        if (!__instance.AmOwner && __instance.interpolateMovement != 0 && GameData.Instance)
            __instance.body.velocity *= CustomPlayer.Custom(__instance.gameObject.GetComponent<PlayerControl>()).SpeedFactor;
    }
}

[HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
public static class BegoneModstamp
{
    public static bool Prefix(ModManager __instance)
    {
        try
        {
            //try catch my beloved <3
            __instance.localCamera = !HudManager.InstanceExists ? Camera.main : HUD.GetComponentInChildren<Camera>();
            __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.RightTop, new(0.6f, 0.6f, 0.1f +
                __instance.localCamera.nearClipPlane));
            __instance.ModStamp.gameObject.SetActive(IsEnded || NoLobby || LobbyBehaviour.Instance);
        } catch {}

        return false;
    }
}

[HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
public static class ConstantsPatch
{
    public static void Postfix(ref int __result)
    {
        if (IsOnlineGame)
            __result = Constants.GetVersion(2222, 0, 0, 0);
    }
}

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public static class DiscordPatch
{
    public static void Prefix(ref Activity activity) => activity.Details += "Town Of Us Reworked";
}

[HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
public static class SetRoles
{
    public static void Postfix() => RoleGen.BeginRoleGen();
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class FixHudNullRef
{
    public static bool Prefix() => !IsEnded;
}

[HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
public static class EmergencyMinigameUpdatePatch
{
    public static void Postfix(EmergencyMinigame __instance)
    {
        if ((!CustomPlayer.Local.CanButton(out var name) || CustomPlayer.Local.RemainingEmergencies == 0) && !CustomPlayer.Local.myTasks.Any(PlayerTask.TaskIsEmergency))
        {
            var title = name switch
            {
                "Shy" => "You are too shy to call a meeting",
                "GameMode" => "Don't call meetings",
                _ => $"{(CustomPlayer.Local.RemainingEmergencies == 0 ? "Y" : $"As the {name}, y")}ou cannot call any more meetings"
            };
            __instance.StatusText.text = title;
            __instance.NumberText.text = string.Empty;
            __instance.ClosedLid.gameObject.SetActive(true);
            __instance.OpenLid.gameObject.SetActive(false);
            __instance.ButtonActive = false;
        }
    }
}

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class LobbyBehaviourPatch
{
    public static void Postfix()
    {
        SetFullScreenHUD();
        DataManager.Settings.Gameplay.ScreenShake = false;
        RoleGen.ResetEverything();
        TownOfUsReworked.IsTest = IsLocalGame && (TownOfUsReworked.IsDev || (TownOfUsReworked.IsTest && TownOfUsReworked.MCIActive));
        StopAll();
        DefaultOutfitAll();
        var count = MCIUtils.Clients.Count;
        MCIUtils.Clients.Clear();
        MCIUtils.PlayerIdClientId.Clear();
        DebuggerBehaviour.Instance.TestWindow.Enabled = TownOfUsReworked.MCIActive && IsLocalGame;
        DebuggerBehaviour.Instance.CooldownsWindow.Enabled = false;

        if (count > 0)
        {
            if (TownOfUsReworked.Persistence)
            {
                for (var i = 0; i < count; i++)
                    MCIUtils.CreatePlayerInstance();
            }
        }
    }
}

[HarmonyPatch(typeof(HideAndSeekDeathPopup), nameof(HideAndSeekDeathPopup.Show))]
public static class DeathPopUpPatch
{
    public static void Prefix(HideAndSeekDeathPopup __instance)
    {
        if (!IsCustomHnS)
            return;

        __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => __instance.text.text = $"Was {(CustomGameOptions.HnSMode == HnSMode.Infection ? "Converted" : "Killed")}")));
    }
}

[HarmonyPatch]
public static class RestartAppIfNecessaryPatch
{
    public const string TypeName = "Steamworks.SteamAPI, Assembly-CSharp-firstpass";
    public const string MethodName = "RestartAppIfNecessary";
    public const string FileName = "steam_appid.txt";

    public static bool Prepare() => Type.GetType(TypeName, false) != null;

    public static MethodBase TargetMethod() => AccessTools.Method($"{TypeName}:{MethodName}");

    public static bool Prefix(out bool __result)
    {
        if (!File.Exists(FileName))
            File.WriteAllText(FileName, "945360");

        return __result = false;
    }
}