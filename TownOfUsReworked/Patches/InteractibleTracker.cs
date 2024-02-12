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
            CustomPlayer.Local.GetModifier<Astral>().LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
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
            CustomPlayer.Local.GetModifier<Astral>().LastPosition = CustomPlayer.LocalCustom.Position;
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
            CustomPlayer.Local.GetModifier<Astral>().LastPosition = CustomPlayer.LocalCustom.Position;
    }
}