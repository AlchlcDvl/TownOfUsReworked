namespace TownOfUsReworked.Patches.Player;

[HarmonyPatch(typeof(PlayerControl))]
public static class PlayerControlPatches
{
    [HarmonyPatch(nameof(PlayerControl.CmdCheckColor)), HarmonyPrefix]
    public static bool CmdCheckColorPrefix(PlayerControl __instance, byte bodyColor)
    {
        CallRpc(ReworkedRpc.Vanilla, VanillaRpc.SetColor, __instance, bodyColor);
        __instance.SetColor(bodyColor);
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.SetPlayerMaterialColors))]
    public static bool Prefix(PlayerControl __instance, Renderer rend)
    {
        PlayerMaterial.SetColors(__instance.Data.DefaultOutfit.ColorId, rend);
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.StartMeeting))]
    public static void Prefix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        if (LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var handler))
            handler.BeforeMeeting();

        MeetingPatches.Reported = target;
        MeetingPatches.Reporter = __instance;

        if (!target || !__instance.AmOwner)
            return;

        var pc = target.Object;
        PlayerLayer.GetLayers<Plaguebearer>().Do(x => x.RpcSpreadInfection(pc, __instance));
        PlayerLayer.GetLayers<Arsonist>().Do(x => x.RpcSpreadDouse(pc, __instance));
        PlayerLayer.GetLayers<Cryomaniac>().Do(x => x.RpcSpreadDouse(pc, __instance));
    }

    [HarmonyPatch(nameof(PlayerControl.Revive)), HarmonyPrefix]
    public static bool RevivePrefix(PlayerControl __instance)
    {
        if (!__instance.Data.IsDead)
            return false;

        __instance.Data.IsDead = false;
        __instance.gameObject.layer = LayerMask.NameToLayer("Players");
        __instance.MyPhysics.ResetMoveState();
        __instance.clickKillCollider.enabled = true;
        __instance.Collider.enabled = true;
        __instance.cosmetics.SetPetSource(__instance);
        __instance.cosmetics.SetNameMask(true);
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId || AllPlayers().All(y => y.PlayerId != x.PlayerId));
        RecentlyKilled.RemoveAll(x => x == __instance.PlayerId || !PlayerById(x) || AllPlayers().All(y => y.PlayerId != x));
        Cleaned.RemoveAll(x => x == __instance.PlayerId || AllPlayers().All(y => y.PlayerId != x) || !PlayerById(x));
        BodyLocations.Remove(__instance.PlayerId);
        SetPostmortals.RemoveFromPostmortals(__instance);
        var body = BodyByPlayer(__instance);

        if (body)
        {
            __instance.RpcCustomSnapTo(body.TruePosition);
            body.gameObject.Destroy();
        }

        if (IsSubmerged() && __instance.AmOwner)
            ChangeFloor(__instance.transform.position.y > -7);

        if (LayerHandler.Handlers.TryGetValue(__instance.PlayerId, out var handler))
            handler.OnRevive();

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckPlayerWins();

        if (!__instance.AmOwner)
            return false;

        var hud = HUD();
        hud.ShadowQuad.gameObject.SetActive(true);
        hud.AdminButton.ToggleVisible(__instance.IsImpostor() && IsHnS());
        ButtonUtils.Reset();

        if (Chat().IsOpenOrOpening)
            Chat().ForceClosed();

        Chat().SetVisible(__instance.CanChat());
        CustomAchievementManager.UnlockAchievement("Revitalised");
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.ReportDeadBody))]
    [HarmonyPatch(nameof(PlayerControl.ReportClosest))]
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (LocalPlayer.Is<Coward>() || !PerformReport.ReportPressed)
            return false;

        var blocked = LocalBlocked();

        if (blocked)
            BlockExposed = true;

        return !blocked;
    }

    [HarmonyPatch(nameof(PlayerControl.CheckUseZipline)), HarmonyPrefix]
    public static void CheckUseZiplinePrefix(PlayerControl target, ref bool __state) => CanUsePatch.Prefix(target.Data, ref __state);

    [HarmonyPatch(nameof(PlayerControl.CheckUseZipline))]
    public static void Postfix(PlayerControl target, ref bool __state) => CanUsePatch.Postfix(target.Data, ref __state);

    [HarmonyPatch(nameof(PlayerControl.AdjustLighting)), HarmonyPrefix]
    public static bool AdjustLightingPrefix(PlayerControl __instance)
    {
        if (!__instance.AmOwner || !Ship())
            return true;

        var role = __instance.GetRole();

        if (!role)
            return true;

        var size = __instance.lightSource.viewDistance;
        var flashlights = role.Faction switch
        {
            Faction.Crew => CrewSettings.CrewFlashlight,
            Faction.Intruder => IntruderSettings.IntruderFlashlight,
            Faction.Syndicate => SyndicateSettings.SyndicateFlashlight,
            Faction.Apocalypse => ApocalypseSettings.ApocalypseFlashlight,
            Faction.Outcast => OutcastSettings.OutcastFlashlight,
            _ => role switch
            {
                Hunted => Hunted.HuntedFlashlight,
                Hunter => Hunter.HunterFlashlight,
                Runner => Runner.RunnerFlashlight,
                _ => false
            }
        } && __instance.Data.IsDead;

        if (flashlights)
            size /= Ship().MaxLightRadius;

        __instance.TargetFlashlight.gameObject.SetActive(flashlights);
        __instance.StartCoroutine(__instance.EnableRightJoystick(false));
        __instance.lightSource.SetupLightingForGameplay(flashlights, size, __instance.TargetFlashlight.transform);
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.SetKinematic))]
    public static bool Prefix(PlayerControl __instance, bool b)
    {
        if (!__instance.AmOwner || !__instance.onLadder || b || __instance.GetModifier() is not (Giant or Dwarf) || MapPatches.CurrentMap is not (5 or 4))
            return true;

        var ladder = UObject.FindObjectsOfType<Ladder>().OrderBy(x => Vector2.Distance(x.transform.position, __instance.transform.position)).FirstOrDefault();

        if (!ladder!.IsTop)
            return true; // Are we at the bottom?

        __instance.RpcCustomSnapTo(__instance.transform.position + new Vector3(0, 0.5f, 0f));
        return true;
    }

    [HarmonyPatch(nameof(PlayerControl.CompleteTask))]
    public static void Postfix(PlayerControl __instance, uint idx)
    {
        if (LayerHandler.Handlers.TryGetValue(__instance.PlayerId, out var handler))
            handler.UponTaskComplete(idx);

        if (AmongUsClient.Instance.AmHost)
        {
            if (CheckEndGame.TasksDone())
            {
                WinState = IsCustomHnS() ? WinLose.HuntedWin : WinLose.CrewWins;
                var winners = AllPlayers().Where(x => x.Is<Hunted>() || x.Is(Faction.Crew));
                winners.Do(x => LayerHandler.Handlers[x.PlayerId].Winner = true);
                CallRpc(ReworkedRpc.Misc, [ MiscRpc.WinLose, WinState, .. winners ]);
            }
            else
                CheckEndGame.CheckPlayerWins();
        }

        if (GameModifiers.EnableDispositions && GameModifiers.ExtendTimer == TimerExtension.Tasks)
            GameTimerHandler.Instance.ExtendTimer();

        if (!__instance.AmOwner)
            return;

        var hud = HUD();

        if (!hud.TaskPanel)
            return;

        string text;

        if (__instance.CanDoTasks())
        {
            var color = "FF00";
            var role = __instance.GetRole();

            if (role.TasksDone)
                color = "00FF";
            else if (role.TasksCompleted > 0)
                color = "FFFF";

            text = $"Tasks <#{color}00FF>({role.TasksCompleted}/{role.TotalTasks})</color>";
        }
        else
            text = "<#FF0000FF>Fake Tasks</color>";

        hud.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>().text = text;
    }

    [HarmonyPatch(nameof(PlayerControl.Awake)), HarmonyPostfix]
    public static void AwakePostfix(PlayerControl __instance)
    {
        __instance.AddComponent<PlayerControlHandler>();
        AddAsset("Kill", __instance.KillSfx);
    }

    [HarmonyPatch(nameof(PlayerControl.Start)), HarmonyPostfix]
    public static void StartPostfix(PlayerControl __instance)
    {
        if (MostRecentKiller == __instance.name && __instance.AmOwner)
            CustomAchievementManager.UnlockAchievement("LastBlood");
    }

    [HarmonyPatch(nameof(PlayerControl.Visible), MethodType.Setter), HarmonyPrefix]
    public static void VisiblePrefix(PlayerControl __instance, ref bool value)
    {
        if (!IsInGame())
            return;

        if (__instance.HasDied() && LocalPlayer.HasDied() && !__instance.AmOwner)
            value = !ClientOptions.HideOtherGhosts;
        else if (LocalPlayer.Is<IShaman>(out var med) && med.MediatedPlayers.Contains(__instance.PlayerId) && !__instance.AmOwner)
            value = true;
    }

    [HarmonyPatch(nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static void Postfix(ref bool __result) => __result &= !HUD().IsIntroDisplayed;

    [HarmonyPatch(nameof(PlayerControl.Exiled)), HarmonyPrefix]
    public static bool ExiledPrefix(PlayerControl __instance)
    {
        __instance.CustomDie(DeathReasonEnum.Ejected, reason: DeathReason.Exile);

        if (__instance.AmOwner)
            DataManager.Player.Stats.IncrementStat(StatID.TimesEjected);

        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.Die))]
    public static bool Prefix(PlayerControl __instance, DeathReason reason)
    {
        __instance.CustomDie(DeathReasonEnum.Killed, reason: reason);
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.RawSetName))]
    public static void Postfix(PlayerControl __instance, string name)
    {
        if (AllBodies().TryFinding(x => x.ParentId == __instance.PlayerId, out var body))
            body.name = name + "Body";
    }

    // Inlining fix + modded RPCs
    [HarmonyPatch(nameof(PlayerControl.HandleRpc))]
    public static bool Prefix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        switch (callId)
        {
            case 4:
            {
                __instance.Exiled();
                break;
            }
            case CustomRPCCallID:
            {
                var targetClientId = -1;

                if (reader.ReadBoolean())
                    targetClientId = reader.ReadPackedInt32();

                var data = new RpcReader(reader.ReadBytesAndSize());
                HandleRpc(data, targetClientId);
                break;
            }
            default:
                return true;
        }

        return false;
    }
}

[HarmonyPatch(typeof(NetworkedPlayerInfo))]
public static class PlayerInfoPatches
{
    [HarmonyPatch(nameof(NetworkedPlayerInfo.ColorName), MethodType.Getter)]
    public static bool Prefix(NetworkedPlayerInfo __instance, ref string __result)
    {
        __result = __instance.GetPlayerColorString();
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

    [HarmonyPatch(nameof(NetworkedPlayerInfo.Deserialize))]
    public static void Postfix(NetworkedPlayerInfo __instance) => __instance.StartCoroutine(CacheOutfits(__instance));

    private static IEnumerator CacheOutfits(NetworkedPlayerInfo __instance)
    {
        while (!__instance.Object || !PlayerById(__instance.PlayerId))
            yield return null;

        if (!AppearanceHandler.Handlers.TryGetValue(__instance.PlayerId, out var handler))
            handler = PlayerById(__instance.PlayerId).GetComponent<AppearanceHandler>();

        handler.Outfits.Clear();

        foreach (var (playerOutfitType, playerOutfit) in __instance.Outfits)
            handler.Outfits[playerOutfitType] = playerOutfit;
    }
}