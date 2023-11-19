namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
public static class SaveLadderPlayer
{
    public static void Prefix(PlayerPhysics __instance)
    {
        try
        {
            UninteractiblePlayers.TryAdd(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local);
        }
        catch (Exception e)
        {
            LogError(e);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Astral))
            Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), typeof(PlayerControl))]
public static class SavePlatformPlayer
{
    public static void Prefix()
    {
        try
        {
            UninteractiblePlayers.TryAdd(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local);
        }
        catch (Exception e)
        {
            LogError(e);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Astral))
            Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public static class SaveZiplinePlayer
{
    public static void Prefix()
    {
        try
        {
            UninteractiblePlayers.TryAdd(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local);
        }
        catch (Exception e)
        {
            LogError(e);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Astral))
            Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
    }
}