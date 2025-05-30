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

    private static void DoTheThing(bool intro = false, bool meeting = false)
    {
        if (intro)
        {
            if (LocalPlayer?.Data?.Role is LayerHandler handler)
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