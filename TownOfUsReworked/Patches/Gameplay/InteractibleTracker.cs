namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch]
public static class InteractableTracker
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static void Prefix(PlayerPhysics __instance) => Handle(__instance.myPlayer);

    [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
    public static void Prefix() => Handle(LocalPlayer);

    public static void Handle(PlayerControl player, float time = 6, bool setRend = false)
    {
        if (!player.AmOwner)
            return;

        try
        {
            UninteractablePlayers.TryAdd(player.PlayerId, Time.time);
            UninteractablePlayers2.TryAdd(player.PlayerId, time);
            CallRpc(ReworkedRpc.Action, ActionsRpc.SetUninteractable, player, time, setRend);
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (player.Is<IPositioner>(out var ast))
            ast.LastPosition = player.transform.position;
    }
}