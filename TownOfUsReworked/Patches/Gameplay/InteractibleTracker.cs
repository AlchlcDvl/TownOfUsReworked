namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch]
public static class InteractableTracker
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static void Prefix(PlayerPhysics __instance)
    {
        if (__instance.AmOwner)
            Handle(__instance.myPlayer);
    }

    [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
    public static void Prefix() => Handle(LocalPlayer);

    public static void Handle(PlayerControl player, float time = 6)
    {
        try
        {
            UninteractablePlayers.TryAdd(player.PlayerId, Time.time);
            UninteractablePlayers2.TryAdd(player.PlayerId, time);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, player, time, false);
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (player.Is<IPositioner>(out var ast))
            ast.LastPosition = player.transform.position;
    }
}