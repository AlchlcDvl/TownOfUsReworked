namespace TownOfUsReworked.Patches;

// The code is from The Other Roles: Community Edition with some modifications; link :- https://github.com/JustASysAdmin/TheOtherRoles2/blob/main/TheOtherRoles/Patches/IntroPatch.cs
public static class SpawnPatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    public static class IntroCutsceneOnDestroyPatch
    {
        public static void Prefix() => DoTheThing(true);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class BaseExileControllerPatch
    {
        public static void Postfix() => DoTheThing(meeting: true);
    }

    private static void DoTheThing(bool intro = false, bool meeting = false)
    {
        Chat()?.SetVisible(CustomPlayer.Local.CanChat());

        if (intro && CustomPlayer.Local.Data.Role is LayerHandler handler)
            handler.OnIntroEnd();

        AllPlayers().ForEach(x => x?.MyPhysics?.ResetAnimState());
        AllBodies().ForEach(x => x?.gameObject?.Destroy());
        ButtonUtils.Reset(CooldownType.Start);
        RandomSpawn(intro, meeting);
        HUD().FullScreen.enabled = true;
        Role.LocalRole.UpdateButtons();

        if (MapPatches.CurrentMap is not (4 or 6))
            HUD().FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);

        HUD().ImpostorVentButton.buttonLabelText.fontSharedMaterial = HUD().ReportButton.buttonLabelText.fontSharedMaterial = HUD().UseButton.buttonLabelText.fontSharedMaterial =
            HUD().PetButton.buttonLabelText.fontSharedMaterial = HUD().SabotageButton.buttonLabelText.fontSharedMaterial;

        if (HUD().TaskPanel)
        {
            var text = "";

            if (CustomPlayer.Local.CanDoTasks())
            {
                var color = "FF00";

                if (Role.LocalRole.TasksDone)
                    color = "00FF";
                else if (Role.LocalRole.TasksCompleted > 0)
                    color = "FFFF";

                text = $"Tasks <color=#{color}00FF>({Role.LocalRole.TasksCompleted}/{Role.LocalRole.TotalTasks})</color>";
            }
            else
                text = "<color=#FF0000FF>Fake Tasks</color>";

            HUD().TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>().SetText(text);
        }
    }

    private static void RandomSpawn(bool intro, bool meeting)
    {
        if (!AmongUsClient.Instance.AmHost || GameModifiers.RandomSpawns == RandomSpawning.Disabled || MapPatches.CurrentMap is 4 or 5 || (meeting && GameModifiers.RandomSpawns ==
            RandomSpawning.GameStart) || (intro && GameModifiers.RandomSpawns == RandomSpawning.PostMeeting))
        {
            return;
        }

        var allLocations = new List<Vector2>();
        allLocations.AddRanges(AllVents().Select(GetVentPosition), AllConsoles().Select(GetConsolePosition), AllSystemConsoles().Select(GetSystemConsolePosition));
        var tobeadded = MapPatches.CurrentMap switch
        {
            0 => SkeldSpawns,
            1 => MiraSpawns,
            2 => PolusSpawns,
            3 => dlekSSpawns,
            _ => null
        };

        if (tobeadded != null)
            allLocations.AddRange(tobeadded);

        foreach (var player in AllPlayers())
        {
            if (player.HasDied())
                continue;

            player.RpcCustomSnapTo(allLocations.Random());
            player.MyPhysics.ResetMoveState();

            if (IsSubmerged())
            {
                ChangeFloor(player.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
    }
}