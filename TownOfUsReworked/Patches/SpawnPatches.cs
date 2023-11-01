namespace TownOfUsReworked.Patches;

//The code is from The Other Roles: Community Edition with modifications; link :- https://github.com/JustASysAdmin/TheOtherRoles2/blob/main/TheOtherRoles/Patches/IntroPatch.cs
public static class SpawnPatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    public static class IntroCutsceneOnDestroyPatch
    {
        public static void Prefix() => DoTheThing();
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class BaseExileControllerPatch
    {
        public static void Postfix() => DoTheThing();
    }

    private static void DoTheThing()
    {
        HUD?.GameSettings?.gameObject?.SetActive(false);
        DataManager.Settings.Gameplay.ScreenShake = true;
        HUD?.Chat?.SetVisible(CustomPlayer.Local.CanChat());
        RandomSpawn();
        PlayerLayer.LocalLayers.ForEach(x => x?.OnIntroEnd());
    }

    private static void RandomSpawn()
    {
        if (!AmongUsClient.Instance.AmHost || !CustomGameOptions.RandomSpawns || MapPatches.CurrentMap is 4 or 5 or 6)
            return;

        var allLocations = new List<Vector2>();
        AllVents.ForEach(x => allLocations.Add(GetVentPosition(x)));
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

            player.NetTransform.RpcSnapTo(allLocations.Random());
        }
    }
}