using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using Discord;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TownOfUsReworked.Patches;

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
        PlayerLayer.LocalLayers.ForEach(x => x.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x.UpdateArrowBlip(__instance));

        if (LocalBlocked)
            __instance.Close();
    }
}

[HarmonyPatch(typeof(PooledMapIcon), nameof(PooledMapIcon.Reset))]
public static class PooledMapIconPatch
{
    public static void Postfix(PooledMapIcon __instance)
    {
        var sprite = __instance.GetComponent<SpriteRenderer>();

        if (sprite)
            PlayerMaterial.SetColors(new UColor(0.8793f, 1, 0, 1), sprite);

        var text = __instance.GetComponentInChildren<TextMeshPro>(true);

        if (!text)
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

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
public static class OpenMapMenuPatch
{
    public static bool Prefix(MapBehaviour __instance, ref MapOptions opts)
    {
        // if (ClientHandler.Instance.SettingsActive)
        //     return false;

        // ClientStuff.CloseMenus(SkipEnum.Map);

        if (PlayerLayer.LocalLayers.All(x => x.IsBlocked))
            return false;

        var notmodified = true;

        if (opts.Mode is not (MapOptions.Modes.None or MapOptions.Modes.CountOverlay))
        {
            if (CustomPlayer.Local.CanSabotage() && !CustomPlayer.AllPlayers.Any(x => x.IsFlashed()))
                __instance.ShowSabotageMap();
            else
                __instance.ShowNormalMap();

            __instance.taskOverlay.gameObject.SetActive(CustomPlayer.Local.CanDoTasks() && !IsTaskRace && !IsCustomHnS);
            notmodified = false;
        }

        PlayerLayer.LocalLayers.ForEach(x => x?.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x?.UpdateArrowBlip(__instance));
        CustomPlayer.Local.DisableButtons();
        return notmodified;
    }

    public static void Postfix() => MapPatch.MapActive = true;
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class MapPatch
{
    public static bool MapActive;

    public static void Postfix()
    {
        MapActive = false;
        CustomPlayer.Local.EnableButtons();
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class DisconnectHandler
{
    public static readonly List<byte> Disconnected = [];

    public static void Prefix(ref PlayerControl player)
    {
        var player2 = player;
        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player2 || !x.Player);

        if (player == CustomPlayer.Local)
        {
            MCIUtils.RemoveAllPlayers();
            DebuggerBehaviour.Instance.ControllingFigure = 0;
        }

        if (IsLobby)
            return;

        SetPostmortals.RemoveFromPostmortals(player);
        Disconnected.Add(player.PlayerId);
        OnGameEndPatches.AddSummaryInfo(player, true);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
public static class CanMove
{
    public static bool Prefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable && !ActiveTask && !__instance.shapeshifting && (!HudManager.InstanceExists || (!Chat.IsOpenOrOpening && !HUD.KillOverlay.IsOpen &&
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
        if (__instance.IsPostmortal() && !__instance.Caught())
            value = !__instance.inVent;
        else if (__instance.HasDied() && CustomPlayer.Local.HasDied() && __instance != CustomPlayer.Local)
            value = !ClientOptions.HideOtherGhosts;
        else if (((CustomPlayer.Local.TryGetLayer<Medium>(out var med) && med.MediatedPlayers.Contains(__instance.PlayerId)) || (CustomPlayer.Local.TryGetLayer<Retributionist>(out var
            ret) && ret.MediatedPlayers.Contains(__instance.PlayerId))) && __instance != CustomPlayer.Local)
        {
            value = true;
        }
        else
            value = __instance == CustomPlayer.Local || __instance?.Data?.IsDead == false;
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
        if (__instance is null or TaskAdderGame or HauntMenuMinigame or SpawnInMinigame || !CustomPlayer.Local.Is(LayerEnum.Multitasker))
            return;

        __instance.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.color = new(x.color.r, x.color.g, x.color.b, Multitasker.Transparancy / 100f));
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
            (panel.NameText.text, panel.NameText.color) = PlayerHandler.UpdateGameName(player);
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
        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast) && ast.LastPosition != Vector3.zero)
            ast.SetPosition();

        HUD.FullScreen.enabled = true;
        HUD.FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
public static class ExitGamePatch
{
    public static void Prefix()
    {
        var filePath = Path.Combine(TownOfUsReworked.Logs, "ReworkedLogs.log");
        var i = 1;

        while (File.Exists(filePath))
        {
            filePath = Path.Combine(TownOfUsReworked.Logs, $"ReworkedLogs{i}.log");
            i++;
        }

        SaveText($"{filePath.SanitisePath()}.log", SavedLogs, TownOfUsReworked.Logs);
        OptionAttribute.SaveSettings("Last Used");
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

    public static void Prefix(ref KillOverlayInitData initData)
    {
        var data = initData;
        var playerControl = CustomPlayer.AllPlayers.Find(x => x.GetCurrentOutfit() == data.killerOutfit);
        CurrentOutfitTypeCache = (int)playerControl.CurrentOutfitType;

        if (!GameModifiers.AppearanceAnimation)
            playerControl.CurrentOutfitType = PlayerOutfitType.Default;
    }

    public static void Postfix(ref KillOverlayInitData initData)
    {
        var data = initData;
        CustomPlayer.AllPlayers.Find(x => x.GetCurrentOutfit() == data.killerOutfit).CurrentOutfitType = (PlayerOutfitType)CurrentOutfitTypeCache;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
public static class AddCustomPlayerPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        CustomPlayer.Custom(__instance);
        AddAsset("Kill", __instance.KillSfx);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
public static class RemoveCustomPlayerPatch
{
    public static void Prefix(PlayerControl __instance) => CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == __instance || !x.Player);
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class LobbySizePatch
{
    public static void Postfix()
    {
        if (IsLobby)
        {
            while (CustomPlayer.AllPlayers.Count > GameSettings.LobbySize && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
                AmongUsClient.Instance.SendLateRejection(AmongUsClient.Instance.GetClient(CustomPlayer.AllPlayers.Last().OwnerId).Id, DisconnectReasons.GameFull);
        }
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
        if (!__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            __instance.body.velocity *= CustomPlayer.Custom(__instance.myPlayer).SpeedFactor;
    }
}

// [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
// public static class ConstantsPatch
// {
//     public static void Postfix(ref int __result)
//     {
//         if (IsOnlineGame && __result % 50 < 25)
//             __result += 25;
//     }
// }

[HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
public static class IsModdedPatch
{
    public static void Postfix(ref bool __result) => __result = true;
}

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public static class DiscordPatch
{
    public static void Prefix(ref Activity activity) => activity.Details += " Town Of Us Reworked";
}

[HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
public static class SetRoles
{
    public static void Postfix() => RoleGen.BeginRoleGen();
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class FixHudNullRef
{
    public static bool Prefix() => CustomPlayer.Local;
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
        RoleGen.ResetEverything();
        TownOfUsReworked.IsTest = IsLocalGame && (TownOfUsReworked.IsDev || (TownOfUsReworked.IsTest && TownOfUsReworked.MCIActive));
        StopAll();
        DefaultOutfitAll();
        var count = MCIUtils.Clients.Count;
        MCIUtils.Clients.Clear();
        MCIUtils.PlayerClientIDs.Clear();
        // ClientHandler.Instance.OnLobbyStart();
        DebuggerBehaviour.Instance.TestWindow.Enabled = TownOfUsReworked.MCIActive && IsLocalGame;
        DebuggerBehaviour.Instance.CooldownsWindow.Enabled = false;
        // ClientHandler.Instance.Page = 0;
        // ClientHandler.Instance.Buttons.Clear();
        // ClientStuff.CloseMenus();
        // FreeplayPatches.PreviouslySelected.Clear();

        if (count > 0 && TownOfUsReworked.Persistence && !IsOnlineGame)
            MCIUtils.CreatePlayerInstances(count);

        var lobbyTransform = References.Lobby.transform;
        var consolePrefab = lobbyTransform.FindChild("panel_Wardrobe").gameObject;
        var rewConsoleObj = UObject.Instantiate(consolePrefab, lobbyTransform);
        rewConsoleObj.name = "panel_Reworked";
        rewConsoleObj.transform.localPosition = new(0f, 2.86f, -9.898f);

        var consoleObj = rewConsoleObj.transform.GetChild(0).gameObject;
        consoleObj.GetComponent<OptionsConsole>().Destroy();
        consoleObj.AddComponent<LobbyConsole>().SetRenderer(rewConsoleObj.GetComponent<SpriteRenderer>());
        rewConsoleObj.GetComponentInChildren<BoxCollider2D>().size = new(0.01f, 0.01f);
    }
}

[HarmonyPatch(typeof(HideAndSeekDeathPopup), nameof(HideAndSeekDeathPopup.Show))]
public static class DeathPopUpPatch
{
    public static void Prefix(HideAndSeekDeathPopup __instance)
    {
        if (IsCustomHnS)
            Coroutines.Start(PerformTimedAction(0.01f, _ => __instance.text.text = $"Was {(GameModeSettings.HnSMode == HnSMode.Infection ? "Converted" : "Killed")}"));
    }
}

[HarmonyPatch(typeof(SkinLayer), nameof(SkinLayer.IsPlayingRunAnim))]
public static class FixLogSpam
{
    public static bool Prefix(SkinLayer __instance, ref bool __result)
    {
        try
        {
            var anim = __instance.animator.GetCurrentAnimation();
            __result = anim == __instance.skin.RunAnim || anim == __instance.skin.RunLeftAnim;
        }
        catch
        {
            __result = false;
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKinematic))]
public static class LadderFix
{
    public static bool Prefix(PlayerControl __instance, ref bool b)
    {
        if (__instance != CustomPlayer.Local || !__instance.onLadder || b || __instance.GetModifier() is not (Giant or Dwarf) || MapPatches.CurrentMap is not (5 or 4))
            return true;

        var ladder = UObject.FindObjectsOfType<Ladder>().OrderBy(x => Vector3.Distance(x.transform.position, __instance.transform.position)).ElementAt(0);

        if (!ladder.IsTop)
            return true; // Are we at the bottom?

        __instance.RpcCustomSnapTo(__instance.transform.position + new Vector3(0, 0.5f, 0f));
        return true;
    }
}

[HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
public static class RefreshPatch
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class MeetingCooldowns
{
    public static void Postfix(ref UObject obj)
    {
        if (!obj)
            return;

        if (Ejection && obj == Ejection.gameObject)
        {
            ButtonUtils.Reset(CooldownType.Meeting);
            PlayerLayer.GetLayers<Retributionist>().ForEach(x => x.OnRoleSelected());
        }
        else if (ActiveTask && obj == ActiveTask.gameObject)
            CustomPlayer.Local.EnableButtons();
    }
}

// Taken and adapted from Submerged recently going open source with their mod code
[HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation), typeof(NetworkedPlayerInfo), typeof(NetworkedPlayerInfo))]
public static class ShowCustomAnim
{
    private static OverlayKillAnimation _selfDeath;
    private static OverlayKillAnimation SelfDeath
    {
        get
        {
            // DO NOT TOUCH THIS GETTER
            // LITERALLY DO NOT TOUCH IT
            //
            // The objects used in this method are in some kind of ethereal state.
            // After very careful manipulation and a lot of time and patience,
            // I have managed to come up with a very meticulous recipe for modifying
            // the death animation. If you change this...you will pay with your blood!
            //
            // - Alex

            // SIR YES SIR o7
            // - AD

            if (_selfDeath)
                return _selfDeath;

            var parent = new GameObject("SelfKillObject").DontUnload().DontDestroy().transform;
            parent.gameObject.SetActive(false);
            _selfDeath = UObject.Instantiate(HudManager.Instance.KillOverlay.KillAnims[0], parent);

            _selfDeath.killerParts.gameObject.SetActive(false);
            _selfDeath.killerParts = null;
            _selfDeath.transform.Find("killstabknife").gameObject.SetActive(false);
            _selfDeath.transform.Find("killstabknifehand").gameObject.SetActive(false);

            _selfDeath.victimParts.transform.localPosition = new(-1.5f, 0, 0);
            _selfDeath.KillType = (KillAnimType)10;

            _selfDeath.gameObject.AddComponent<CustomKillAnimationPlayer>();
            return _selfDeath;
        }
    }

    public static bool Prefix(KillOverlay __instance, ref NetworkedPlayerInfo killer, ref NetworkedPlayerInfo victim)
    {
        if (killer.PlayerId != victim.PlayerId || AprilFoolsMode.ShouldHorseAround() || AprilFoolsMode.ShouldLongAround() || IsSubmerged())
            return true;

        __instance.ShowKillAnimation(SelfDeath, killer, victim);
        return false;
    }
}

[HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.WaitForFinish))]
public static class WaitForFinishPatch
{
    public static bool Prefix(OverlayKillAnimation __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        var flag = __instance.TryGetComponent<CustomKillAnimationPlayer>(out var customKillAnim);

        if (customKillAnim)
            __result = customKillAnim.WaitForFinish().WrapToIl2Cpp();

        return !flag;
    }
}

// Brought this back because the logs are being spammed....again
[HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
public static class BegoneModstamp
{
    public static bool Prefix(ModManager __instance)
    {
        try
        {
            // try catch my beloved <3
            __instance.localCamera = !HudManager.InstanceExists ? Camera.main : HUD.GetComponentInChildren<Camera>();
            __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.RightTop, new(0.6f, 0.6f, 0.1f +
                __instance.localCamera.nearClipPlane));
            __instance.ModStamp.gameObject.SetActive(IsEnded || NoLobby || IsLobby);
        } catch {}

        return false;
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.DirtyAllData))]
public static class DirtyAllDataPatch
{
    public static bool Prefix() => false;
}

/*[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class DebuggingClassForRandomStuff
{
    public static void Postfix()
    {
        // Some sick code here
        // Got too lazy to constantly remove it so it'll stay now, still commented tho because i love to reduce dll size :D
    }
}*/