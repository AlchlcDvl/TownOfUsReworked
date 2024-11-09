using AmongUs.Data.Player;
using AmongUs.Data.Legacy;
using AmongUs.Data.Settings;
using Discord;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviourPatch
{
    public static void Postfix(MapBehaviour __instance)
    {
        PlayerLayer.LocalLayers().ForEach(x => x.UpdateMap(__instance));
        CustomArrow.AllArrows.ForEach(x => x.UpdateArrowBlip(__instance));

        if (LocalBlocked())
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
public static class PlayerDataPatch
{
    public static void Postfix(ref string __result) => __result += "_ToU-Rew" + (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream ? "D" : "");
}

[HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.GetPrefsName))]
public static class LegacySaveManagerPatch
{
    public static void Postfix(ref string __result) => __result += "_ToU-Rew" + (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream ? "D" : "");
}

[HarmonyPatch(typeof(SettingsData), nameof(SettingsData.FileName), MethodType.Getter)]
public static class SettingsDataPatch
{
    public static void Postfix(ref string __result) => __result += "_ToU-Rew" + (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream ? "D" : "");
}

[HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
public static class SecurityLoggerPatch
{
    public static void Postfix(SecurityLogger __instance) => __instance.Timers = new float[127];
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
public static class OpenMapMenuPatch
{
    public static bool Prefix(MapBehaviour __instance, MapOptions opts)
    {
        if (ClientHandler.Instance.SettingsActive)
            return false;

        ClientHandler.Instance.CloseMenus(SkipEnum.Map);

        if (PlayerLayer.LocalLayers().All(x => x.IsBlocked))
            return false;

        var notmodified = true;

        if (opts.Mode is not (MapOptions.Modes.None or MapOptions.Modes.CountOverlay))
        {
            if (CustomPlayer.Local.CanSabotage() && !AllPlayers().Any(x => x.IsFlashed()))
                __instance.ShowSabotageMap();
            else
                __instance.ShowNormalMap();

            __instance.taskOverlay.gameObject.SetActive(CustomPlayer.Local.CanDoTasks() && !IsTaskRace() && !IsCustomHnS());
            notmodified = false;
        }

        PlayerLayer.LocalLayers().ForEach(x => x?.UpdateMap(__instance));
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
        Disconnected.Add(player.PlayerId);
        OnGameEndPatches.AddSummaryInfo(player, true);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
public static class CanMove
{
    public static bool Prefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable && !ActiveTask() && !__instance.shapeshifting && (!HudManager.InstanceExists || (!Chat().IsOpenOrOpening && !HUD().KillOverlay.IsOpen && !Meeting() &&
            !HUD().GameMenu.IsOpen)) && (!Map() || !Map().IsOpenStopped) && !IntroCutscene.Instance && !PlayerCustomizationMenu.Instance;
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
        else if (((CustomPlayer.Local.TryGetLayer<Medium>(out var med) && med.MediatedPlayers.Contains(__instance.PlayerId)) || (CustomPlayer.Local.TryGetLayer<Retributionist>(out var ret) &&
            ret.MediatedPlayers.Contains(__instance.PlayerId))) && __instance != CustomPlayer.Local)
        {
            value = true;
        }
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
        if (__instance is TaskAdderGame or HauntMenuMinigame or SpawnInMinigame or ShapeshifterMinigame || !CustomPlayer.Local.Is(LayerEnum.Multitasker))
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
        var result = CustomMenu.AllMenus.TryFinding(x => x.Menu == __instance && x.Owner.AmOwner, out var menu);

        if (result)
            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, menu.CreateMenu(__instance));

        return !result;
    }
}

[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
public static class AirshipSpawnInPatch
{
    public static void Postfix()
    {
        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast) && ast.LastPosition != Vector3.zero)
            ast.SetPosition();

        HUD().FullScreen.enabled = true;
        HUD().FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);
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
        if (TownOfUsReworked.IsDev || TownOfUsReworked.IsStream)
            __result = true;

        return !(TownOfUsReworked.IsDev || TownOfUsReworked.IsStream);
    }
}

[HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.SetPurchased))]
public static class SetPurchasedPatch
{
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
        if (IsLobby())
        {
            while (AllPlayers().Count > GameSettings.LobbySize && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
                AmongUsClient.Instance.SendLateRejection(AmongUsClient.Instance.GetClient(AllPlayers().Last().OwnerId).Id, DisconnectReasons.GameFull);
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
    public static void Postfix(LobbyBehaviour __instance)
    {
        SetFullScreenHUD();
        RoleGen.ResetEverything();
        TownOfUsReworked.IsTest = IsLocalGame() && (TownOfUsReworked.IsDev || (TownOfUsReworked.IsTest && TownOfUsReworked.MCIActive));
        StopAll();
        DefaultOutfitAll();
        var count = MCIUtils.Clients.Count;
        MCIUtils.Clients.Clear();
        MCIUtils.PlayerClientIDs.Clear();
        DebuggerBehaviour.Instance.TestWindow.Enabled = TownOfUsReworked.MCIActive && IsLocalGame();
        ClientHandler.Instance.OnLobbyStart(__instance);
        ClientHandler.Instance.Page = 0;
        ClientHandler.Instance.Buttons.Clear();
        ClientHandler.Instance.CloseMenus();
        FreeplayPatches.PreviouslySelected.Clear();

        if (count > 0 && TownOfUsReworked.Persistence.Value && !IsOnlineGame())
            MCIUtils.CreatePlayerInstances(count);
    }
}

[HarmonyPatch(typeof(HideAndSeekDeathPopup), nameof(HideAndSeekDeathPopup.Show))]
public static class DeathPopUpPatch
{
    public static void Prefix(HideAndSeekDeathPopup __instance)
    {
        if (IsCustomHnS())
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
    public static bool Prefix(PlayerControl __instance, bool b)
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
    public static void Postfix(UObject obj)
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

    public static bool Prefix(KillOverlay __instance, NetworkedPlayerInfo killer, NetworkedPlayerInfo victim)
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
            __instance.localCamera = !HudManager.InstanceExists ? Camera.main : HUD().GetComponentInChildren<Camera>();
            __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.RightTop, new(0.6f, 0.6f, 0.1f +
                __instance.localCamera.nearClipPlane));
            __instance.ModStamp.gameObject.SetActive(IsEnded() || NoLobby() || IsLobby());
        } catch {}

        return false;
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.DirtyAllData))]
public static class DirtyAllDataPatch
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation), typeof(NetworkedPlayerInfo), typeof(NetworkedPlayerInfo))]
public static class OverrideFlameColorPatch
{
    public static void Prefix(KillOverlay __instance, NetworkedPlayerInfo killer, NetworkedPlayerInfo victim)
    {
        var rend = __instance.flameParent.transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (victim == killer || !GameModifiers.ShowKillerRoleColor)
            rend.color = Role.LocalRole.Color;
        else
            rend.color = killer.Object.GetRole().Color;
    }
}

[HarmonyPatch(typeof(NetworkedPlayerInfo), nameof(NetworkedPlayerInfo.Serialize))]
public static class FixNullRef
{
    public static bool Prefix(NetworkedPlayerInfo __instance, MessageWriter writer, bool initialState, ref bool __result)
    {
        writer.Write(__instance.PlayerId);
        writer.WritePacked(__instance.ClientId);
        writer.Write((byte)__instance.Outfits.Count);

        foreach (var keyValuePair in __instance.Outfits)
        {
            writer.Write((byte)keyValuePair.Key);
            keyValuePair.Value.Serialize(writer);
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
            roleWhenAlive = __instance.RoleWhenAlive != null;
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

        writer.Write(__instance.FriendCode ?? string.Empty);
        writer.Write(__instance.Puid ?? string.Empty);

        if (!initialState)
            __instance.ClearDirtyBits();

        __result = true;
        return false;
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

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class FixedPlayerPatch
{
    public static void Postfix()
    {
        if (CustomPlayer.LocalCustom?.Data?.Role is LayerHandler handler)
            handler.FixedPlayerUpdate();
    }
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