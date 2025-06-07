namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (!LocalPlayer || !LocalPlayer.Data || !IsSubmerged())
            return;

        Coroutines.Start(WaitAction(() => ButtonUtils.Reset(CooldownType.Start)));
    }
}

[HarmonyPatch(typeof(PlayerPhysics)), HarmonyPriority(Priority.Low)]
public static class SubmergedPhysicsPatches
{
    [HarmonyPatch(nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPatch(nameof(PlayerPhysics.LateUpdate))]
    public static void Postfix(PlayerPhysics __instance)
    {
        if (!IsSubmerged() || !__instance.myPlayer.Data.IsDead)
            return;

        var player = __instance.myPlayer;

        if (!player.Is<IGhosty>(out var ghost) || ghost.Caught)
            return;

        if (player.AmOwner)
            MoveDeadPlayerElevator(player);
        else
            player.Collider.enabled = false;

        var transform = __instance.transform;
        var position = transform.position;
        position.z = position.y / 1000;

        transform.position = position;
        __instance.myPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
    }
}