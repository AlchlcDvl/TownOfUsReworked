using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using AmongUs.Data.Settings;
using Discord;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MapBehaviour))]
public static class MapBehaviourPatches
{
    [HarmonyPatch(nameof(MapBehaviour.FixedUpdate)), HarmonyPostfix]
    public static void FixedUpdatePostfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers().ForEach(x => x.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x.UpdateArrowBlip(__instance));

        if (LocalBlocked())
            __instance.Close();
    }

    [HarmonyPatch(nameof(MapBehaviour.Show))]
    public static bool Prefix(MapBehaviour __instance, MapOptions opts)
    {
        if (ClientHandler.Instance.SettingsActive)
            return false;

        ClientHandler.Instance.CloseMenus(SkipEnum.Map);

        if (PlayerLayer.LocalLayers().All(x => x.IsBlocked))
            return false;

        var notmodified = true;

        if (opts.Mode is MapOptions.Modes.Normal or MapOptions.Modes.Sabotage)
        {
            if (CustomPlayer.Local.CanSabotage() && !AllPlayers().Any(x => x.IsFlashed()))
                __instance.ShowSabotageMap();
            else
                __instance.ShowNormalMap();

            __instance.taskOverlay.gameObject.SetActive(!IsTaskRace() && !IsCustomHnS());
            notmodified = false;
        }

        PlayerLayer.LocalLayers().ForEach(x => x?.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x?.UpdateArrowBlip(__instance));
        CustomPlayer.Local.DisableButtons();
        return notmodified;
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
        PlayerLayer.LocalLayers().ForEach(x => x.UpdateMap(__instance));
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
            text.fontMaterial.SetFloat("_OutlineWidth", 0.1745f);
            text.fontMaterial.SetFloat("_FaceDilate", 0.151f);
        }

        text.transform.localPosition = new(0, 0, -20);
        text.SetText("");
        text.gameObject.SetActive(false);
    }
}

[HarmonyPatch]
public static class PlayerDataPatch
{
    [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
    [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.GetPrefsName))]
    [HarmonyPatch(typeof(SettingsData), nameof(SettingsData.FileName), MethodType.Getter)]
    public static void Postfix(ref string __result) => __result += "_ToU-Rew" + (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream ? "D" : "");
}

[HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
public static class SecurityLoggerPatch
{
    public static void Postfix(SecurityLogger __instance) => __instance.Timers = new float[127];
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class DisconnectHandler
{
    public static void Prefix(PlayerControl player)
    {
        CustomPlayer.AllCustomPlayers.RemoveAll(x => x.Player == player || !x.Player);

        if (player.AmOwner)
        {
            MCIUtils.RemoveAllPlayers();
            DebuggerBehaviour.Instance.ControllingFigure = 0;
        }

        if (IsLobby())
            return;

        SetPostmortals.RemoveFromPostmortals(player);
        OnGameEndPatches.AddSummaryInfo(player, true);
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

        __instance.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.color = new(x.color.r, x.color.g, x.color.b, Multitasker.Transparancy / 100f));
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
        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.SetPosition();

        var hud = HUD();
        hud.FullScreen.enabled = true;
        hud.FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);

        if (TownOfUsReworked.MCIActive)
        {
            foreach (var player in AllPlayers())
            {
                if (player.name.StartsWith("Bot"))
                    player.RpcCustomSnapTo(__instance.Locations.Random().Location);
            }
        }
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
        OptionAttribute.SaveSettings("LastUsed");
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

[HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.Initialize))]
public static class OverlayKillAnimationPatch
{
    private static int OutfitTypeCache;

    public static void Prefix(KillOverlayInitData initData)
    {
        var playerControl = AllPlayers().Find(x => x.GetCurrentOutfit() == initData.killerOutfit);
        OutfitTypeCache = (int)playerControl.CurrentOutfitType;

        if (!GameModifiers.AppearanceAnimation)
            playerControl.CurrentOutfitType = PlayerOutfitType.Default;
    }

    public static void Postfix(KillOverlayInitData initData) => AllPlayers().Find(x => x.GetCurrentOutfit() == initData.killerOutfit).CurrentOutfitType = (PlayerOutfitType)OutfitTypeCache;
}

[HarmonyPatch(typeof(HudManager))]
public static class HudPatches
{
    [HarmonyPatch(nameof(HudManager.Update))]
    public static void Postfix()
    {
        if (IsLobby() && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
        {
            var players = AllPlayers();

            while (players.Count() > GameSettings.LobbySize)
                AmongUsClient.Instance.SendLateRejection(AmongUsClient.Instance.GetClient(players.Last().OwnerId).Id, DisconnectReasons.GameFull);
        }
    }

    [HarmonyPatch(nameof(HudManager.Update))]
    public static bool Prefix() => CustomPlayer.Local;

    [HarmonyPatch(nameof(HudManager.Start)), HarmonyPrefix]
    public static bool StartPrefix(HudManager __instance) => __instance.playerListPrompt;

    [HarmonyPatch(nameof(HudManager.Start))]
    public static void Postfix(HudManager __instance) => ClientHandler.Instance.OnHudStart(__instance);

    [HarmonyPatch(nameof(HudManager.CoShowIntro)), HarmonyPrefix]
    public static void CoShowIntroPrefix(HudManager __instance) => __instance.GameLoadAnimation.SetActive(false);
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
        if ((!CustomPlayer.Local.CanButton(out var name) || CustomPlayer.Local.RemainingEmergencies == 0) && !CustomPlayer.Local.myTasks.Any(PlayerTask.TaskIsEmergency))
        {
            __instance.StatusText.SetText(name switch
            {
                "Shy" => "You are too shy to call a meeting",
                "GameMode" => "Don't call meetings",
                _ => $"{(CustomPlayer.Local.RemainingEmergencies == 0 ? "Y" : $"As the {name}, y")}ou cannot call any more meetings"
            });
            __instance.NumberText.SetText("");
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
        RoleGenManager.ResetEverything();
        StopAll();
        DefaultOutfitAll();
        var count = MCIUtils.Clients.Count;
        MCIUtils.Clients.Clear();
        MCIUtils.PlayerClientIDs.Clear();
        DebuggerBehaviour.Instance.TestWindow.Enabled = TownOfUsReworked.MCIActive && IsLocalGame();
        ClientHandler.Instance.OnLobbyStart();
        ClientHandler.Instance.Page = 0;
        ClientHandler.Instance.Buttons.Clear();
        ClientHandler.Instance.CloseMenus();
        FreeplayPatches.PreviouslySelected.Clear();
        var hud = HUD();
        hud.enabled = false;
        hud.enabled = true;

        if (count > 0 && TownOfUsReworked.Persistence.Value && !IsOnlineGame())
            MCIUtils.CreatePlayerInstances(count);

        CustomAchievementManager.QueuedAchievements.ForEach(x => x.ShowAchievement());
    }
}

[HarmonyPatch(typeof(HideAndSeekDeathPopup), nameof(HideAndSeekDeathPopup.Show))]
public static class DeathPopUpPatch
{
    public static void Prefix(HideAndSeekDeathPopup __instance)
    {
        if (IsCustomHnS() && !__instance.name.StartsWith("Achievement"))
        {
            __instance.text.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.text.SetText($"Was {(GameModeSettings.HnSMode == HnSMode.Infection ? "Converted" : "Killed")}");
        }
    }
}

[HarmonyPatch]
public static class RefreshPatch
{
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    [HarmonyPatch(typeof(GameData), nameof(GameData.DirtyAllData))]
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class MeetingCooldowns
{
    public static void Prefix(UObject obj)
    {
        if (!obj)
            return;

        if (Ejection() && obj == Ejection().gameObject)
        {
            ButtonUtils.Reset(CooldownType.Meeting);
            PlayerLayer.GetLayers<Retributionist>().ForEach(x => x.OnRoleSelected());
        }
        else if (ActiveTask() && obj == ActiveTask().gameObject)
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
            _selfDeath = UObject.Instantiate(HUD().KillOverlay.KillAnims[0], parent);

            _selfDeath.killerParts.gameObject.SetActive(false);
            _selfDeath.killerParts = null;
            _selfDeath.transform.Find("killstabknife").gameObject.SetActive(false);
            _selfDeath.transform.Find("killstabknifehand").gameObject.SetActive(false);

            _selfDeath.victimParts.transform.localPosition = new(-1.5f, 0, 0);
            _selfDeath.KillType = (KillAnimType)10;

            _selfDeath.AddComponent<CustomKillAnimationPlayer>();

            var array = HUD().KillOverlay.KillAnims.ToList();
            array.Add(_selfDeath);
            HUD().KillOverlay.KillAnims = array.ToArray();

            return _selfDeath;
        }
    }

    public static bool Prefix(KillOverlay __instance, NetworkedPlayerInfo killer, NetworkedPlayerInfo victim)
    {
        var rend = __instance.flameParent.transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (victim == killer || !GameModifiers.ShowKillerRoleColor)
            rend.color = CustomPlayer.Local.GetRole().Color;
        else
            rend.color = killer.Object.GetRole().Color;

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

        if (flag && customKillAnim)
            __result = customKillAnim.WaitForFinish().WrapToIl2Cpp();

        return !flag;
    }
}

[HarmonyPatch(typeof(FollowerCamera), nameof(FollowerCamera.Update))]
public static class FollowerCameraPatches
{
    public static void Postfix(FollowerCamera __instance)
    {
        if (!__instance.Target || __instance.Locked || !ClientOptions.LockCameraSway)
            return;

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
        __instance.hostLabel.SetText(TranslationController.Instance.GetString(StringNames.HostNounLabel));
        __instance.playerName.SetText($"{(string.IsNullOrEmpty(host.PlayerName) ? "..." : $"<#{text}>{host.PlayerName}</color>")} " + (AmongUsClient.Instance.AmHost ?
            $"<size=90%><b><font=\"Barlow-BoldItalic SDF\" material=\"Barlow-BoldItalic SDF Outline\">{TranslationController.Instance.GetString(StringNames.HostYouLabel)}" :
            $"({__instance.player.ColorBlindName})"));
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

[HarmonyPatch(typeof(NetworkedPlayerInfo))]
public static class PlayerDataPatches
{
    [HarmonyPatch(nameof(NetworkedPlayerInfo.ColorName), MethodType.Getter)]
    public static bool Prefix(NetworkedPlayerInfo __instance, ref string __result)
    {
        __result = __instance.GetPlayerColorString(PlayerOutfitType.Default);
        return false;
    }

    [HarmonyPatch(nameof(NetworkedPlayerInfo.GetPlayerColorString))]
    public static bool Prefix(NetworkedPlayerInfo __instance, PlayerOutfitType outfitType, ref string __result)
    {
        if (__instance.Outfits.TryGetValue(outfitType, out var outfit))
        {
            var translation = Palette.GetColorName(outfit.ColorId);
            __result = outfit.ColorId.IsDefault() ? (translation[0] + translation[1..].ToLower()) : translation;
        }
        else
            __result = "";

        return false;
    }

    [HarmonyPatch(nameof(NetworkedPlayerInfo.Serialize))]
    public static bool Prefix(NetworkedPlayerInfo __instance, MessageWriter writer, bool initialState, ref bool __result)
    {
        writer.Write(__instance.PlayerId);
        writer.WritePacked(__instance.ClientId);
        writer.Write((byte)__instance.Outfits.Count);

        foreach (var pair in __instance.Outfits)
        {
            writer.Write((byte)pair.Key);
            pair.Value.Serialize(writer);
        }

        writer.WritePacked(__instance.PlayerLevel);
        byte b = 0;

        if (__instance.Disconnected)
            b |= 1;

        if (__instance.IsDead)
            b |= 4;

        writer.Write(b);
        writer.Write((ushort)(__instance.Role?.Role ?? RoleTypes.Crewmate));
        var roleWhenAlive = false;

        try
        {
            roleWhenAlive = __instance.RoleWhenAlive != null && __instance.RoleWhenAlive.HasValue;
        } catch {}

        writer.Write(roleWhenAlive);

        if (roleWhenAlive)
            writer.Write((ushort)__instance.RoleWhenAlive.Value);

        if (__instance.Tasks != null)
        {
            writer.Write((byte)__instance.Tasks.Count);
            __instance.Tasks.ForEach(x => x.Serialize(writer));
        }
        else
            writer.Write(0);

        writer.Write(__instance.FriendCode ?? "");
        writer.Write(__instance.Puid ?? "");

        if (!initialState)
            __instance.ClearDirtyBits();

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
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
        if (GameModifiers.ParallelMedScans)
        {
            // Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = CustomPlayer.Local.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}