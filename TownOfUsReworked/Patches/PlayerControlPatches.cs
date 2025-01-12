namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl))]
public static class PlayerControlPatches
{
    [HarmonyPatch(nameof(PlayerControl.CmdCheckColor))]
    public static bool Prefix(PlayerControl __instance, byte bodyColor)
    {
        CallRpc(CustomRPC.Vanilla, VanillaRPC.SetColor, __instance, bodyColor);
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
        if (CustomPlayer.Local?.Data?.Role is LayerHandler handler)
            handler.BeforeMeeting();

        MeetingPatches.Reported = target;
        MeetingPatches.Reporter = __instance;

        if (!target || !__instance.AmOwner)
            return;

        var pc = target.Object;
        PlayerLayer.GetLayers<Plaguebearer>().ForEach(x => x.RpcSpreadInfection(pc, __instance));
        PlayerLayer.GetLayers<Arsonist>().ForEach(x => x.RpcSpreadDouse(pc, __instance));
        PlayerLayer.GetLayers<Cryomaniac>().ForEach(x => x.RpcSpreadDouse(pc, __instance));
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
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId || !AllPlayers().Any(y => y.PlayerId == x.PlayerId));
        RecentlyKilled.RemoveAll(x => x == __instance.PlayerId || !PlayerById(x) || !AllPlayers().Any(y => y.PlayerId == x));
        Cleaned.RemoveAll(x => x == __instance.PlayerId || !AllPlayers().Any(y => y.PlayerId == x) || !PlayerById(x));
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

        if (__instance.Data.Role is LayerHandler layerHandler)
            layerHandler.OnRevive();

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckEnd();

        if (!__instance.AmOwner)
            return false;

        var hud = HUD();
        hud.ShadowQuad.gameObject.SetActive(true);
        hud.KillButton.ToggleVisible(IsHnS());
        hud.AdminButton.ToggleVisible(__instance.IsImpostor() && IsHnS());
        hud.SabotageButton.ToggleVisible(__instance.CanSabotage());
        hud.ImpostorVentButton.ToggleVisible(__instance.CanVent());
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

        if (CustomPlayer.Local.Is<Coward>() || !PerformReport.ReportPressed)
            return false;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }

    [HarmonyPatch(nameof(PlayerControl.CheckUseZipline))]
    public static void Prefix(PlayerControl target, ref bool __state) => CanUsePatch.Prefix(target.Data, ref __state);

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

        var size = Ship().CalculateLightRadius(__instance.Data);
        var flashlights = role.Faction switch
        {
            Faction.Crew => CrewSettings.CrewFlashlight,
            Faction.Intruder => IntruderSettings.IntruderFlashlight,
            Faction.Syndicate => SyndicateSettings.SyndicateFlashlight,
            Faction.Neutral => NeutralSettings.NeutralFlashlight,
            _ => role switch
            {
                Hunted => GameModeSettings.HuntedFlashlight,
                Hunter => GameModeSettings.HunterFlashlight,
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

        var ladder = UObject.FindObjectsOfType<Ladder>().OrderBy(x => Vector3.Distance(x.transform.position, __instance.transform.position)).ElementAt(0);

        if (!ladder.IsTop)
            return true; // Are we at the bottom?

        __instance.RpcCustomSnapTo(__instance.transform.position + new Vector3(0, 0.5f, 0f));
        return true;
    }

    [HarmonyPatch(nameof(PlayerControl.CompleteTask))]
    public static void Postfix(PlayerControl __instance, uint idx)
    {
        if (__instance.Data.Role is LayerHandler handler)
            handler.UponTaskComplete(idx);

        var hud = HUD();

        if (hud.TaskPanel)
        {
            var text = "";

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

            hud.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>().SetText(text);
        }
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
        if (__instance.IsPostmortal() && !__instance.Caught())
            value = !__instance.inVent;
        else if (__instance.HasDied() && CustomPlayer.Local.HasDied() && !__instance.AmOwner)
            value = !ClientOptions.HideOtherGhosts;
        else if (((CustomPlayer.Local.TryGetLayer<Medium>(out var med) && med.MediatedPlayers.Contains(__instance.PlayerId)) || (CustomPlayer.Local.TryGetLayer<Retributionist>(out var ret) &&
            ret.MediatedPlayers.Contains(__instance.PlayerId))) && !__instance.AmOwner)
        {
            value = true;
        }
    }

    [HarmonyPatch(nameof(PlayerControl.CanMove), MethodType.Getter), HarmonyPrefix]
    public static bool CanMovePrefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable && !ActiveTask() && !__instance.shapeshifting && (!HudManager.InstanceExists || (!Chat().IsOpenOrOpening && !HUD().KillOverlay.IsOpen && !Meeting() &&
            !HUD().GameMenu.IsOpen)) && (!Map() || !Map().IsOpenStopped) && !IntroCutscene.Instance && !PlayerCustomizationMenu.Instance;
        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.Exiled)), HarmonyPrefix]
    public static bool ExiledPrefix(PlayerControl __instance)
    {
        __instance.CustomDie(DeathReasonEnum.Ejected, reason: DeathReason.Exile);

        if (__instance.AmOwner)
            StatsManager.Instance.IncrementStat(StringNames.StatsTimesEjected);

        return false;
    }

    [HarmonyPatch(nameof(PlayerControl.Die)), HarmonyPrefix]
    public static bool DiePrefix(PlayerControl __instance, DeathReason reason)
    {
        __instance.CustomDie(DeathReasonEnum.Killed, reason: reason);
        return false;
    }
}