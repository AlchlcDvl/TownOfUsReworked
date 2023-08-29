namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
public static class SaveLadderPlayer
{
    public static void Prefix(PlayerPhysics __instance)
    {
        try
        {
            if (CustomPlayer.Local.Is(LayerEnum.Transporter))
                ((Transporter)Role.LocalRole).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                ((Retributionist)Role.LocalRole).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.Warper))
                ((Warper)Role.LocalRole).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.PromotedRebel))
                ((PromotedRebel)Role.LocalRole).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else
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

[HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
public static class SavePlatformPlayer
{
    public static void Prefix()
    {
        try
        {
            if (CustomPlayer.Local.Is(LayerEnum.Transporter))
                ((Transporter)Role.LocalRole).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.Retributionist))
                ((Retributionist)Role.LocalRole).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.Warper))
                ((Warper)Role.LocalRole).UnwarpablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(LayerEnum.PromotedRebel))
                ((PromotedRebel)Role.LocalRole).UnwarpablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else
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