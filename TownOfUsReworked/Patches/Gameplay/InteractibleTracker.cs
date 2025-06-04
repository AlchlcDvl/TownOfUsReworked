namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch]
public static class SavePlayer
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static void Prefix(PlayerPhysics __instance)
    {
        if (__instance.AmOwner)
            Handle(__instance.myPlayer);
    }

    [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
    public static void Prefix() => Handle(LocalPlayer);

    private static void Handle(PlayerControl player)
    {
        try
        {
            UninteractablePlayers.TryAdd(player.PlayerId, Time.time);
            UninteractablePlayers2.TryAdd(player.PlayerId, 6);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, player, 6, false);
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (player.Is<Astral>(out var ast))
            ast.LastPosition = player.transform.position;

        if (player.Is<IGhosty>(out var ghosty))
            ghosty.LastPosition = player.transform.position;
    }
}