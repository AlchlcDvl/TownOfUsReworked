namespace TownOfUsReworked.Patches.Core.Player;

// The code is from The Other Roles: Community Edition with some modifications; link :- https://github.com/JustASysAdmin/TheOtherRoles2/blob/main/TheOtherRoles/Patches/IntroPatch.cs
[HarmonyPatch]
public static class SpawnPatches
{
#if PC
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    public static void Prefix() => DoTheThing(true);
#endif

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static void Postfix() => DoTheThing(meeting: true);

    // Inlining fix
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.Animate))]
    public static bool Prefix(AirshipExileController __instance, ref IIEnumerator __result)
    {
        __result = Original(__instance).WrapToIl2Cpp();
        return false;
    }

    private static IEnumerator Original(AirshipExileController __instance)
    {
        if (__instance.initData != null && __instance.initData.outfit != null)
            PlayerMaterial.SetColors(__instance.initData.outfit.ColorId, __instance.HandSlot);

        var num = __instance.Duration + 3.2f;
        __instance.StartCoroutine(Effects.All(
            __instance.SlowMoSlide2D(__instance.ForegroundCloud.transform, new(-1.4f, -2.41f), new(-0.4f, -2.41f), num),
            __instance.SlowMoSlide2D(__instance.BackgroundCloud.transform, new(-0.97f, -1.043f), new(0.25f, -1.043f), num),
            __instance.SlowMoSlide2D(__instance.Cloud1.transform, new(3f, 0.25f), new(6.5f, 0.25f), num),
            __instance.SlowMoSlide2D(__instance.Cloud2.transform, new(-6f, 3f), new(5f, 3f), num),
            __instance.SlowMoSlide2D(__instance.Cloud3.transform, new(-4f, -2.2f), new(4f, -2.2f), num)
        ));

        if (HudManager.InstanceExists)
            yield return HUD().CoFadeFullScreen(UColor.black, UColor.clear, 0.2f, false);
        else
            yield return Effects.Wait(0.2f);

        yield return Effects.Wait(0.5f);
        yield return Effects.All(__instance.PlayerFall(), __instance.HandleText(1.75f, __instance.Duration * 0.5f));
        yield return Effects.Wait(0.5f);

        if (HudManager.InstanceExists)
            yield return HUD().CoFadeFullScreen(UColor.clear, UColor.black, 0.2f, false);
        else
            yield return Effects.Wait(0.2f);

        yield return __instance.WrapUpAndSpawn();
    }

    private static void DoTheThing(bool intro = false, bool meeting = false)
    {
        if (intro)
        {
            if (LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var handler))
                handler.OnIntroEnd();

            KillCounts.Clear();
            MostRecentKiller = null;
            Debugging.Instance.SelectedTab = Debugging.Instance.Tabs[1];
        }

        Chat()?.SetVisible(LocalPlayer.CanChat());

        foreach (var player in AllPlayers())
        {
            player.MyPhysics.ResetAnimState();
            player.MyPhysics.ResetMoveState();
        }

        AllBodies().Do(x => x.gameObject.Destroy());
        ButtonUtils.Reset(intro ? CooldownType.Start : CooldownType.Meeting);
        RandomSpawn(intro, meeting);
        var role = LocalPlayer.GetRole();
        role.UpdateButtons();
        LocalPlayer.RegenTask();
        var hud = HUD();

        if (MapPatches.CurrentMap is not (4 or 6))
            hud.FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);

        hud.ImpostorVentButton.buttonLabelText.fontSharedMaterial = hud.ReportButton.buttonLabelText.fontSharedMaterial = hud.UseButton.buttonLabelText.fontSharedMaterial =
            hud.PetButton.buttonLabelText.fontSharedMaterial = hud.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (!hud.TaskPanel)
            return;

        string text;

        if (LocalPlayer.CanDoTasks())
        {
            var color = "FF00";

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

    private static void RandomSpawn(bool intro, bool meeting)
    {
        if (!AmongUsClient.Instance.AmHost || MapPatches.CurrentMap is 4 or 5 || (meeting && GameModifiers.RandomSpawns == RandomSpawning.GameStart) || (intro && GameModifiers.RandomSpawns ==
            RandomSpawning.PostMeeting) || GameModifiers.RandomSpawns != [ RandomSpawning.GameStart, RandomSpawning.GameStart ])
        {
            return;
        }

        var allLocations = new List<Vector2>();
        allLocations.AddRange(AllVents().Select(x => (Vector2)x.transform.position));
        var toAdd = MapPatches.CurrentMap switch
        {
            0 => SkeldSpawns,
            1 => MiraSpawns,
            2 => PolusSpawns,
            3 => dlekSSpawns,
            _ => null
        };

        if (toAdd is not null)
            allLocations.AddRange(toAdd);

        foreach (var player in AllPlayers())
        {
            if (player.HasDied())
                continue;

            player.RpcCustomSnapTo(allLocations.Random());
            player.MyPhysics.ResetMoveState();
            player.MyPhysics.ResetAnimState();

            if (!IsSubmerged())
                continue;

            ChangeFloor(player.GetTruePosition().y > -7);
            CheckOutOfBoundsElevator(LocalPlayer);
        }
    }
}