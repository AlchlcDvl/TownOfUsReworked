namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
public static class SaveLadderPlayer
{
    public static void Prefix(PlayerPhysics __instance)
    {
        try
        {
            UninteractiblePlayers.TryAdd(__instance.myPlayer.PlayerId, Time.time);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local, 6, false);
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
public static class SavePlatformPlayer
{
    public static void Prefix()
    {
        try
        {
            UninteractiblePlayers.TryAdd(CustomPlayer.Local.PlayerId, Time.time);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local, 6, false);
        }
        catch (Exception e)
        {
            Error(e);
        }

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public static class SaveZiplinePlayer
{
}