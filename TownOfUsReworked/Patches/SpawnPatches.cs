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
        HUD?.GameSettings?.gameObject?.SetActive(false);
        HUD?.Chat?.SetVisible(CustomPlayer.Local.CanChat());
        PlayerLayer.LocalLayers.ForEach(x => x?.OnIntroEnd());
        CustomPlayer.AllPlayers.ForEach(x => x?.MyPhysics?.ResetAnimState());
        CustomPlayer.Local.EnableButtons();
        AllBodies.ForEach(x => x?.gameObject?.Destroy());
        ButtonUtils.Reset(CooldownType.Start);
        RandomSpawn(intro, meeting);
        HUD.FullScreen.enabled = true;

        if (MapPatches.CurrentMap is not (4 or 6))
            HUD.FullScreen.color = new(0.6f, 0.6f, 0.6f, 0f);

        Sprites.TryAdd("DefaultVent", HUD.ImpostorVentButton.graphic.sprite);
        Sprites.TryAdd("DefaultSabotage", HUD.SabotageButton.graphic.sprite);
        var sab = GetSprite("DefaultSabotage");
        var vent = GetSprite("DefaultVent");

        if (CustomPlayer.Local.Is(Faction.Syndicate))
            sab = GetSprite("SyndicateSabotage");
        else if (CustomPlayer.Local.Is(Faction.Intruder))
            sab = GetSprite("IntruderSabotage");

        if (CustomPlayer.Local.Is(Faction.Intruder))
            vent = GetSprite("IntruderVent");
        else if (CustomPlayer.Local.Is(Faction.Syndicate))
            vent = GetSprite("SyndicateVent");
        else if (CustomPlayer.Local.Is(Faction.Crew))
            vent = GetSprite("CrewVent");
        else if (CustomPlayer.Local.Is(Faction.Neutral))
            vent = GetSprite("NeutralVent");

        HUD.SabotageButton.graphic.sprite = sab;
        HUD.SabotageButton.graphic.SetCooldownNormalizedUvs();
        HUD.ImpostorVentButton.graphic.sprite = vent;
        HUD.ImpostorVentButton.graphic.SetCooldownNormalizedUvs();
        HUD.ImpostorVentButton.buttonLabelText.fontSharedMaterial = HUD.ReportButton.buttonLabelText.fontSharedMaterial = HUD.UseButton.buttonLabelText.fontSharedMaterial =
            HUD.PetButton.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;
    }

    private static void RandomSpawn(bool intro, bool meeting)
    {
        if (!AmongUsClient.Instance.AmHost || CustomGameOptions.RandomSpawns == RandomSpawning.Disabled || MapPatches.CurrentMap is 4 or 5 || (meeting && CustomGameOptions.RandomSpawns ==
            RandomSpawning.GameStart) || (intro && CustomGameOptions.RandomSpawns == RandomSpawning.PostMeeting))
        {
            return;
        }

        var allLocations = new List<Vector2>();
        AllMapVents.ForEach(x => allLocations.Add(GetVentPosition(x)));
        AllConsoles.ForEach(x => allLocations.Add(GetConsolePosition(x)));
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

        foreach (var player in CustomPlayer.AllPlayers)
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