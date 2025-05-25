namespace TownOfUsReworked.Patches.Core.Player;

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

        if (__instance.Data.Role is LayerHandler layerHandler)
            layerHandler.OnRevive();

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

        if (CustomPlayer.Local.Is<Coward>() || !PerformReport.ReportPressed)
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
            Faction.Neutral => NeutralSettings.NeutralFlashlight,
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
        if (__instance.Data.Role is LayerHandler handler)
            handler.UponTaskComplete(idx);

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
        if (__instance.IsPostmortal() && !__instance.Caught())
            value = !__instance.inVent;
        else if (__instance.HasDied() && CustomPlayer.Local.HasDied() && !__instance.AmOwner)
            value = !ClientOptions.HideOtherGhosts;
        else if (CustomPlayer.Local.Is<IShaman>(out var med) && med.MediatedPlayers.Contains(__instance.PlayerId) && !__instance.AmOwner)
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

    [HarmonyPatch(nameof(NetworkedPlayerInfo.Init))]
    public static void Postfix(NetworkedPlayerInfo __instance)
    {
        var outfit = __instance.Outfits[PlayerOutfitType.Default];
        __instance.Outfits[PlayerOutfitType.Default] = new CustomOutfit(outfit);
    }

    [HarmonyPatch(nameof(NetworkedPlayerInfo.DefaultOutfit), MethodType.Getter)]
    public static bool Prefix(NetworkedPlayerInfo __instance, ref PlayerOutfit  __result)
    {
        if (IsInGame() && __instance.Outfits.TryGetValue((PlayerOutfitType)CustomPlayerOutfitType.GameDefault, out var outfit))
            __result = outfit;
        else
            __result = __instance.Outfits[PlayerOutfitType.Default];

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
    public static bool Prefix(NetworkedPlayerInfo __instance, MessageReader reader, bool initialState)
    {
        __instance.PlayerId = reader.ReadByte();
        __instance.ClientId = reader.ReadPackedInt32();
        var b = reader.ReadByte();
        __instance.Outfits.Clear();

        for (var i = 0; i < b; i++)
        {
            var playerOutfitType = (PlayerOutfitType)reader.ReadByte();
            var playerOutfit = new CustomOutfit();
            playerOutfit.Deserialize(reader);
            __instance.Outfits[playerOutfitType] = playerOutfit;
        }

        __instance.PlayerLevel = reader.ReadPackedUInt32();
        var b2 = reader.ReadByte();
        __instance.Disconnected = (b2 & 1) > 0;
        __instance.IsDead = (b2 & 4) > 0;
        __instance.RoleType = (RoleTypes)reader.ReadUInt16();

        if (reader.ReadBoolean())
            __instance.RoleWhenAlive = new((RoleTypes)reader.ReadUInt16());

        var b3 = reader.ReadByte();
        __instance.Tasks.Clear();

        for (var j = 0; j < b3; j++)
        {
            var taskInfo = new NetworkedPlayerInfo.TaskInfo();
            taskInfo.Deserialize(reader);
            __instance.Tasks.Add(taskInfo);
        }

        __instance.FriendCode = reader.ReadString();
        __instance.Puid = reader.ReadString();

        switch (initialState)
        {
            case true when !GameData.Instance.GetPlayerById(__instance.PlayerId) && !GameData.Instance.IsProcessingInfo(__instance):
            {
                GameData.Instance.AddPlayerInfo(__instance);
                break;
            }
            case false when __instance.Object:
            {
                __instance.Object.MyPhysics.ResetAnimState();
                break;
            }
        }

        GameData.Instance.RecomputeTaskCounts();
        return false;
    }
}