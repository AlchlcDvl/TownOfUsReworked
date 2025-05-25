using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using AmongUs.Data.Settings;
using Discord;

namespace TownOfUsReworked.Patches.Technical;

[HarmonyPatch(typeof(MapBehaviour))]
public static class MapBehaviourPatches
{
    [HarmonyPatch(nameof(MapBehaviour.FixedUpdate)), HarmonyPostfix]
    public static void FixedUpdatePostfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers().Do(x => x.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x.UpdateArrowBlip(__instance));

        if (LocalBlocked())
            __instance.Close();
    }

    [HarmonyPatch(nameof(MapBehaviour.Show))]
    public static bool Prefix(MapBehaviour __instance, MapOptions opts)
    {
        if (Client.Instance.SettingsActive)
            return false;

        Client.Instance.CloseMenus(SkipEnum.Map);

        if (CustomPlayer.Local.IsBlocked())
            return false;

        var notModified = true;

        if (opts.Mode is MapOptions.Modes.Normal or MapOptions.Modes.Sabotage)
        {
            if (CustomPlayer.Local.CanSabotage() && !AllPlayers().Any(x => x.IsFlashed()))
                __instance.ShowSabotageMap();
            else
                __instance.ShowNormalMap();

            __instance.taskOverlay.gameObject.SetActive(!IsTaskRace() && !IsCustomHnS());
            notModified = false;
        }

        PlayerLayer.LocalLayers().Do(x => x?.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x?.UpdateArrowBlip(__instance));
        CustomPlayer.Local.DisableButtons();
        return notModified;
    }

    [HarmonyPatch(nameof(MapBehaviour.Show)), HarmonyPostfix]
    public static void ShowPostfix() => MapActive = true;

    public static bool MapActive;

    [HarmonyPatch(nameof(MapBehaviour.Close)), HarmonyPostfix]
    public static void ClosePostfix()
    {
        MapActive = false;
        CustomPlayer.Local.EnableButtons();
    }

    [HarmonyPatch(nameof(MapBehaviour.Awake)), HarmonyPostfix]
    public static void AwakePostfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers().Do(x => x.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x.UpdateArrowBlip(__instance));
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
            text.fontMaterial.SetFloat(OutlineWidth, 0.1745f);
            text.fontMaterial.SetFloat(FaceDilate, 0.151f);
        }

        text.transform.localPosition = new(0, 0, -20);
        text.text = "";
        text.gameObject.SetActive(false);
    }
}

[HarmonyPatch]
public static class PlayerDataPatch
{
    [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.GetPrefsName))]
    [HarmonyPatch(typeof(SettingsData), nameof(SettingsData.FileName), MethodType.Getter)]
    // ReSharper disable once HeuristicUnreachableCode
    public static void Postfix(ref string __result) => __result += "_ToU-Rew" + (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream ? "D" : "");
}

[HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
public static class SecurityLoggerPatch
{
    public static void Postfix(SecurityLogger __instance) => __instance.Timers = new(250);
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class DisconnectHandler
{
    public static void Prefix(PlayerControl player)
    {
        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player || !x.Player);

        if (player.AmOwner)
        {
            MciUtils.RemoveAllPlayers();
            Debugging.Instance.ControllingFigure = 0;
        }

        if (IsLobby())
            return;

        SetPostmortals.RemoveFromPostmortals(player);
        OnGameEndPatches.AddSummaryInfo(player, true);
        CheckEndGame.CheckPlayerWins();
    }
}

[HarmonyPatch(typeof(PlayerPhysics))]
public static class HandleAnimation
{
    [HarmonyPatch(nameof(PlayerPhysics.HandleAnimation))]
    public static void Prefix(PlayerPhysics __instance, ref bool amDead)
    {
        if (__instance.myPlayer.IsPostmortal())
            amDead = __instance.myPlayer.Caught();
    }

    [HarmonyPatch(nameof(PlayerPhysics.ResetMoveState)), HarmonyPostfix]
    public static void ResetMoveStatePostfix(PlayerPhysics __instance)
    {
        if (__instance.myPlayer.IsPostmortal())
            __instance.myPlayer.Collider.enabled = !__instance.myPlayer.Caught();
    }

    [HarmonyPatch(nameof(PlayerPhysics.FixedUpdate)), HarmonyPostfix]
    public static void FixedUpdatePostfix(PlayerPhysics __instance)
    {
        if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            __instance.body.velocity *= CustomPlayer.Custom(__instance.myPlayer).SpeedFactor;
    }
}

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class MinigameBeginPatch
{
    public static void Postfix(Minigame __instance)
    {
        if (!CustomPlayer.Local.Is<Multitasker>() || __instance.TryCast<TaskAdderGame>() || __instance.TryCast<HauntMenuMinigame>() || __instance.TryCast<SpawnInMinigame>() ||
            __instance.TryCast<ShapeshifterMinigame>())
        {
            return;
        }

        __instance.GetComponentsInChildren<SpriteRenderer>().Do(x => x.color = new(x.color.r, x.color.g, x.color.b, Multitasker.Transparency / 100f));
    }
}

[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class CustomMenuPatch
{
    public static bool Prefix(ShapeshifterMinigame __instance)
    {
        __instance.AddComponent<MenuPagingBehaviour>().Menu = __instance;
        var result = CustomMenu.AllMenus.TryFinding(x => x.Menu == __instance && x.Owner.AmOwner, out var menu);

        if (result)
            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, menu.CreateMenu(__instance));

        return !result;
    }
}

[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
public static class AirshipSpawnInPatch
{
    public static void Postfix(SpawnInMinigame __instance)
    {
        if (CustomPlayer.Local.Is<Astral>(out var ast))
            ast.SetPosition();

        HUD().FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);

        if (!TownOfUsReworked.MciActive)
            return;

        foreach (var player in AllPlayers())
        {
            if (player.name.StartsWith("Bot"))
                player.RpcCustomSnapTo(__instance.Locations.Random().Location);
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient))]
public static class AmongUsClientPatches
{
    [HarmonyPatch(nameof(AmongUsClient.ExitGame))]
    public static void Prefix()
    {
        var filePath = Path.Combine(TownOfUsReworked.Logs, "ReworkedLogs.log");

        for (var i = 1; File.Exists(filePath); i++)
            filePath = Path.Combine(TownOfUsReworked.Logs, $"ReworkedLogs{i}.log");

        SaveText($"{filePath.SanitisePath()}.log", SavedLogs, TownOfUsReworked.Logs);
        Option.SaveSettings("LastUsed");
    }

    [HarmonyPatch(nameof(AmongUsClient.CreatePlayer))]
    public static bool Prefix(AmongUsClient __instance, ClientData clientData, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = BetterCreatePlayer(__instance, clientData).WrapToIl2Cpp();
        return false;
    }

    private static IEnumerator BetterCreatePlayer(AmongUsClient __instance, ClientData clientData)
    {
        if (clientData.IsBeingCreated || clientData.Character)
            yield break;

        if (!__instance.AmHost)
        {
            __instance.logger.Debug("Waiting for host to make my player");
            yield break;
        }

        clientData.IsBeingCreated = true;
        var isOwnerOfPlayerData = __instance.NetworkMode is NetworkModes.LocalGame or NetworkModes.FreePlay || __instance.AmModdedHost;
        int b;

        if (isOwnerOfPlayerData)
        {
            b = GameData.Instance.HasPlayer(clientData) ? GameData.Instance.GetPlayerIdFromClient(clientData) : MciUtils.GetAvailableId(false);

            if (b == -1)
            {
                __instance.SendLateRejection(clientData.Id, DisconnectReasons.GameFull);
                __instance.logger.Info("Overfilled room.");
                clientData.IsBeingCreated = false;
                yield break;
            }
        }
        else
        {
            yield return WaitUntil(() => GameData.Instance.HasPlayer(clientData));
            b = GameData.Instance.GetPlayerIdFromClient(clientData);
        }

        var zero = Vector2.zero;

        if (DestroyableSingleton<TutorialManager>.InstanceExists)
            zero = new(-1.9f, 3.25f);

        var pc = UObject.Instantiate(__instance.PlayerPrefab, zero, Quaternion.identity);
        pc.PlayerId = (byte)b;
        pc.FriendCode = clientData.FriendCode;
        pc.Puid = clientData.ProductUserId;
        clientData.Character = pc;
        __instance.UpdateCachedClients(clientData, clientData.Character);
        var ship = Ship();

        if (ship)
            ship.SpawnPlayer(pc, Palette.PlayerColors.Length, false);

        if (isOwnerOfPlayerData)
            __instance.Spawn(GameData.Instance.AddPlayer(pc, clientData));
        else
            yield return WaitUntil(() => GameData.Instance.GetPlayerByClient(clientData));

        __instance.Spawn(pc, clientData.Id, SpawnFlags.IsClientCharacter);

        if (isOwnerOfPlayerData)
            GameData.Instance.DirtyAllData();

        if (GameManager.Instance.LogicOptions.IsDefaults)
            GameManager.Instance.LogicOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.NetworkMode);

        clientData.IsBeingCreated = false;
    }
}

[HarmonyPatch(typeof(PlayerPurchasesData))]
public static class GetPurchasePatch
{
    [HarmonyPatch(nameof(PlayerPurchasesData.GetPurchase))]
    public static bool Prefix(ref bool __result) => !(__result = TownOfUsReworked.IsDev || TownOfUsReworked.IsStream);

    [HarmonyPatch(nameof(PlayerPurchasesData.SetPurchased))]
    public static bool Prefix() => !(TownOfUsReworked.IsDev || TownOfUsReworked.IsStream);
}

[HarmonyPatch(typeof(HudManager))]
public static class HudPatches
{
    [HarmonyPatch(nameof(HudManager.Update))]
    public static void Postfix()
    {
        if (!IsLobby() || !AmongUsClient.Instance.AmHost || !AmongUsClient.Instance.CanBan() || IsInGame())
            return;

        var players = AllPlayers();

        while (players.Count() > GameSettings.LobbySize)
            AmongUsClient.Instance.SendLateRejection(AmongUsClient.Instance.GetClient(players.Last().OwnerId).Id, DisconnectReasons.GameFull);
    }

    [HarmonyPatch(nameof(HudManager.CoShowIntro))]
    public static void Prefix(HudManager __instance) => __instance.GameLoadAnimation.SetActive(false);
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

[HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
public static class IsModdedPatch
{
    public static void Postfix(ref bool __result) => __result = true;
}

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public static class DiscordPatch
{
    public static void Prefix(Activity activity) => activity.Details += " Town Of Us Reworked";
}

[HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
public static class EmergencyMinigameUpdatePatch
{
    public static void Postfix(EmergencyMinigame __instance)
    {
        if ((CustomPlayer.Local.CanButton(out var name) && CustomPlayer.Local.RemainingEmergencies != 0) || CustomPlayer.Local.myTasks.Any(PlayerTask.TaskIsEmergency))
            return;
        __instance.StatusText.text = name switch
        {
            "Shy" => "You are too shy to call a meeting",
            "GameMode" => "Don't call meetings",
            _ => $"{(CustomPlayer.Local.RemainingEmergencies == 0 ? "Y" : $"As the {name}, y")}ou cannot call any more meetings"
        };
        __instance.NumberText.text = "";
        __instance.ClosedLid.gameObject.SetActive(true);
        __instance.OpenLid.gameObject.SetActive(false);
        __instance.ButtonActive = false;
    }
}

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class LobbyBehaviourPatch
{
    public static void Postfix()
    {
        SetFullScreenHUD();
        RoleGenManager.ResetEverything();
        StopAll();
        DefaultOutfitAll();
        ClientHandler.OnLobbyStart();
        Client.Instance.Page = 0;
        Client.Instance.Buttons.Clear();
        Client.Instance.CloseMenus();
        FreeplayPatches.PreviouslySelected.Clear();
        var count = MciUtils.Clients.Count;
        Debugging.Instance.TestWindow.Enabled = TownOfUsReworked.MciActive && IsLocalGame();
        MciUtils.Clients.Clear();
        MciUtils.PlayerClientIDs.Clear();
        Debugging.Instance.SelectedTab = Debugging.Instance.Tabs[0];

        if (count > 0 && TownOfUsReworked.Persistence.Value && !IsOnlineGame())
            MciUtils.CreatePlayerInstances(count);

        CustomAchievementManager.QueuedAchievements.ForEach(x => x.ShowAchievement());
    }
}

[HarmonyPatch(typeof(HideAndSeekDeathPopup), nameof(HideAndSeekDeathPopup.Show))]
public static class DeathPopUpPatch
{
    public static void Prefix(HideAndSeekDeathPopup __instance)
    {
        if (!IsCustomHnS() || __instance.name.StartsWith("Achievement"))
            return;

        __instance.text.GetComponent<TextTranslatorTMP>().Destroy();
        __instance.text.text = $"Was {(GameModeSettings.HnSMode == HnSMode.Infection ? "Converted" : "Killed")}";
    }
}

[HarmonyPatch]
public static class RefreshPatch
{
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    [HarmonyPatch(typeof(GameData), nameof(GameData.DirtyAllData))]
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.FixedUpdate))]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.FixedUpdate))]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.UpdateValuesAndText))]
    [HarmonyPatch(typeof(NotificationPopper), nameof(NotificationPopper.AddSettingsChangeMessage))]
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class MeetingCooldowns
{
    public static void Prefix(UObject obj)
    {
        if (ActiveTask() && obj == ActiveTask().gameObject)
            CustomPlayer.Local.EnableButtons();
    }
}

// Taken and adapted from Submerged recently going open source with their mod code
[HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation), typeof(NetworkedPlayerInfo), typeof(NetworkedPlayerInfo))]
public static class ShowCustomAnim
{
    private static OverlayKillAnimation selfDeath;
    private static OverlayKillAnimation SelfDeath
    {
        get
        {
            // DO NOT TOUCH THIS GETTER
            // LITERALLY DO NOT TOUCH IT
            //
            // The objects used in here are in some kind of ethereal state.
            // After very careful manipulation and a lot of time and patience,
            // I have managed to come up with a very meticulous recipe for modifying
            // the death animation. If you change it...you will pay with your blood!
            //
            // - Alex

            // SIR YES SIR o7
            // - AD

            if (!selfDeath)
            {
                var parent = new GameObject("SelfKillObject").DontDestroy().transform;
                parent.gameObject.SetActive(false);

                selfDeath = UObject.Instantiate(HUD().KillOverlay.KillAnims[0], parent);

                selfDeath.killerParts.gameObject.SetActive(false);
                selfDeath.killerParts = null;
                selfDeath.transform.Find("killstabknife").gameObject.SetActive(false);
                selfDeath.transform.Find("killstabknifehand").gameObject.SetActive(false);

                selfDeath.victimParts.transform.localPosition = new(-1.5f, 0, 0);
                selfDeath.KillType = (KillAnimType)10;

                selfDeath.AddComponent<CustomKillAnimationPlayer>();

                var array = HUD().KillOverlay.KillAnims.ToList();
                array.Add(selfDeath);
                HUD().KillOverlay.KillAnims = array.ToArray();
            }

            return selfDeath;
        }
    }

    public static bool Prefix(KillOverlay __instance, NetworkedPlayerInfo killer, NetworkedPlayerInfo victim)
    {
        __instance.flameParent.transform.GetChild(0).GetComponent<SpriteRenderer>().color =
        (
            victim == killer || !GameModifiers.ShowKillerRoleColor
                ? CustomPlayer.Local
                : killer.Object
        ).GetRole().Color;

        if (killer.PlayerId != victim.PlayerId || AprilFoolsMode.ShouldHorseAround() || AprilFoolsMode.ShouldLongAround() || IsSubmerged())
            return true;

        __instance.ShowKillAnimation(SelfDeath, killer, victim);
        return false;
    }
}

[HarmonyPatch(typeof(OverlayKillAnimation))]
public static class OverlayKillAnimationPatches
{
    [HarmonyPatch(nameof(OverlayKillAnimation.WaitForFinish))]
    public static bool Prefix(OverlayKillAnimation __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        var flag = __instance.TryGetComponent<CustomKillAnimationPlayer>(out var customKillAnim);

        if (flag && customKillAnim)
            __result = customKillAnim.WaitForFinish().WrapToIl2Cpp();

        return !flag;
    }

    private static int OutfitTypeCache;

    [HarmonyPatch(nameof(OverlayKillAnimation.Initialize))]
    public static void Prefix(KillOverlayInitData initData)
    {
        var playerControl = AllPlayers().Find(x => x.CurrentOutfit() == initData.killerOutfit);
        OutfitTypeCache = (int)playerControl.CurrentOutfitType;

        if (!GameModifiers.AppearanceAnimation)
            playerControl.CurrentOutfitType = PlayerOutfitType.Default;
    }

    [HarmonyPatch(nameof(OverlayKillAnimation.Initialize))]
    public static void Postfix(KillOverlayInitData initData) => AllPlayers().Find(x => x.CurrentOutfit() == initData.killerOutfit).CurrentOutfitType = (PlayerOutfitType)OutfitTypeCache;
}

#if ANDROID
[HarmonyPatch(typeof(OverlayKillAnimation._CoShow_d__18), "MoveNext")]
#else
[HarmonyPatch(typeof(OverlayKillAnimation._CoShow_d__18), nameof(OverlayKillAnimation._CoShow_d__18.MoveNext))]
#endif
public static class FixMeetingKills
{
    public static void Postfix(bool __result)
    {
        if (Meeting())
            AllVoteAreas().Do(x => x.gameObject.SetActive(!__result));
    }
}

[HarmonyPatch(typeof(FollowerCamera), nameof(FollowerCamera.Update))]
public static class FollowerCameraPatches
{
    public static bool Prefix(FollowerCamera __instance)
    {
        if (!ClientOptions.LockCameraSway)
            return true;

        if (!__instance.Target || __instance.Locked)
            return false;

        var v = (Vector2)__instance.Target.transform.position + __instance.Offset;

        if (__instance.shakeAmount > 0f && DataManager.Settings.Gameplay.ScreenShake && __instance.OverrideScreenShakeEnabled)
        {
            var num = Time.fixedTime * __instance.shakePeriod;
            var num2 = (Mathf.PerlinNoise(0.5f, num) * 2f) - 1f;
            var num3 = (Mathf.PerlinNoise(num, 0.5f) * 2f) - 1f;
            v.x += num2 * __instance.shakeAmount;
            v.y += num3 * __instance.shakeAmount;
        }

        __instance.transform.position = v;
        return false;
    }
}

[HarmonyPatch(typeof(HostInfoPanel), nameof(HostInfoPanel.SetUp))]
public static class HostInfoPanelPatch
{
    public static bool Prefix(HostInfoPanel __instance)
    {
        var host = GameData.Instance.GetHost();
        __instance.hostWidth = __instance.hostLabel.GetRenderedValues().x;

        if (!host || host.IsIncomplete)
            return false;

        __instance.content.SetActive(true);

        if (!__instance.firstUpdate)
        {
            __instance.firstUpdate = true;
            __instance.StartCoroutine(__instance.SetCosmetics(host));
        }

        var text = ColorUtility.ToHtmlStringRGB(__instance.player.ColorId.GetColor(false));
        __instance.hostLabel.text = TranslationController.Instance.GetString(StringNames.HostNounLabel);
        __instance.playerName.text = $"{(string.IsNullOrEmpty(host.PlayerName) ? "..." : $"<#{text}>{host.PlayerName}</color>")} " + (AmongUsClient.Instance.AmHost ?
            $"<size=90%><b><font=\"Barlow-BoldItalic SDF\" material=\"Barlow-BoldItalic SDF Outline\">{TranslationController.Instance.GetString(StringNames.HostYouLabel)}" :
            $"({__instance.player.ColorBlindName})");
        var x = __instance.playerName.GetRenderedValues().x;
        var num = __instance.hostWidth + 0.48f;
        __instance.content.transform.localPosition = new((-0.43f + num) / 2f, __instance.content.transform.localPosition.y, __instance.content.transform.localPosition.z);
        __instance.playerHolder.transform.localPosition = new(((__instance.playerName.transform.localPosition.x - x) / 2f) - 0.215f, __instance.playerHolder.transform.localPosition.y,
            __instance.playerHolder.transform.localPosition.z);
        __instance.hostLabel.transform.localPosition = new(__instance.playerHolder.transform.localPosition.x - 0.22f, __instance.hostLabel.transform.localPosition.y,
            __instance.hostLabel.transform.localPosition.z);
        return false;
    }
}

[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
public static class OverrideKillAnim
{
    public static bool Prefix(KillAnimation __instance, PlayerControl source, PlayerControl target, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = CoPerformKill(__instance, source, target, DeathReasonEnum.Killed, true).WrapToIl2Cpp();
        return false;
    }
}

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<IObject>))]
public static class PatchColours
{
    public static bool Prefix(StringNames id, ref string __result)
    {
        var result = TranslationManager.Translate(id, out var customString);

        if (result)
            __result = customString;

        return !result;
    }

    public static void Postfix(StringNames id, ref string __result)
    {
        if (__result.StartsWith("STRMISS") && !__result.Contains('('))
            __result += $" ({id})";
    }
}

[HarmonyPatch(typeof(MedScanMinigame))]
public static class MedScanMinigamePatch
{
    [HarmonyPatch(nameof(MedScanMinigame.Begin))]
    public static void Postfix(MedScanMinigame __instance)
    {
        var newHeightFeet = 0;
        var newHeightInch = Mathf.RoundToInt(((3f * 12f) + 6f) * CustomPlayer.Local.GetModifiedSize());
        var newWeight = Mathf.RoundToInt(92f * CustomPlayer.Local.GetModifiedSize());

        while (newHeightInch >= 12)
        {
            newHeightFeet++;
            newHeightInch -= 12;
        }

        __instance.completeString = __instance.completeString.Replace("3' 6", $"{newHeightFeet}' {newHeightInch}").Replace("92lb", $"{newWeight}lb");
    }

    [HarmonyPatch(nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (!GameModifiers.ParallelMedScans)
            return;

        // Allows multiple medbay scans at once
        __instance.medscan.CurrentUser = CustomPlayer.Local.PlayerId;
        __instance.medscan.UsersList.Clear();
    }
}

[HarmonyPatch]
public static class FuckOffModStampIWillMurderYouIfYouErrorAgain
{
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))] // I have a hate-only relationship with ModManager
    [HarmonyPatch(typeof(NotificationPopper), nameof(NotificationPopper.ShiftMessages))]
    public static Exception Finalizer() => null; // My first use of a finalizer ong
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RPCHandling
{
    public static void Postfix(byte callId, MessageReader reader)
    {
        if (callId != CustomRPCCallID)
            return;

        using var data = new NetData(reader.ReadBytes(reader.ReadUInt16()));
        HandleRpc(data);
    }
}

[HarmonyPatch(typeof(Enum))]
public static class EnumPatches
{
    [HarmonyPatch(nameof(Enum.GetValues), typeof(Type))]
    public static bool Prefix(Type enumType, ref Array __result)
    {
        if (!CustomEnumInjector.Injectors.TryGetValue(enumType, out var injector))
            return true;

        __result = injector.Values();
        return false;
    }

    [HarmonyPatch(nameof(Enum.ToString), [])]
    public static bool Prefix(Enum __instance, ref string __result)
    {
        if (!CustomEnumInjector.Injectors.TryGetValue(__instance.GetType(), out var injector))
            return true;

        __result = injector.ToString(__instance);
        return false;
    }

    [HarmonyPatch(nameof(Enum.ToString), []), HarmonyReversePatch]
    public static string OriginalToString(Enum instance) => throw new NotSupportedException();
}