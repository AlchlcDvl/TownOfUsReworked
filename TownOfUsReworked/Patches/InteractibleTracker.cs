namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
public static class SaveLadderPlayer
{
    public static void Prefix(PlayerPhysics __instance)
    {
        try
        {
            UninteractiblePlayers.TryAdd(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local, 6, false);
        }
        catch (Exception e)
        {
            LogError(e);
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
            UninteractiblePlayers.TryAdd(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local, 6, false);
        }
        catch (Exception e)
        {
            LogError(e);
        }

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.LastPosition = CustomPlayer.LocalCustom.Position;
    }
}

[HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
public static class SaveZiplinePlayer
{
    public static void Prefix(ZiplineBehaviour __instance, ref PlayerControl player, ref bool fromTop)
    {
        try
        {
            UninteractiblePlayers.TryAdd(player.PlayerId, DateTime.UtcNow);
            UninteractiblePlayers2.TryAdd(player.PlayerId, fromTop ? __instance.upTravelTime : __instance.downTravelTime);
            CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, player, UninteractiblePlayers2[player.PlayerId], true);
            var hand = __instance.playerIdHands[player.PlayerId];

            if (player.GetCustomOutfitType() is CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly)
                hand.handRenderer.color.SetAlpha(player.MyRend().color.a);
            else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
                PlayerMaterial.SetColors(UColor.grey, hand.handRenderer);
            else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Colorblind)
                hand.handRenderer.color = UColor.grey;
            else if (player.IsMimicking(out var mimicked))
                hand.SetPlayerColor(mimicked.GetCurrentOutfit(), PlayerMaterial.MaskType.None, mimicked.cosmetics.GetPhantomRoleAlpha());
            else
                hand.SetPlayerColor(player.GetCurrentOutfit(), PlayerMaterial.MaskType.None, player.cosmetics.GetPhantomRoleAlpha());
        }
        catch (Exception e)
        {
            LogError(e);
        }

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.LastPosition = CustomPlayer.LocalCustom.Position;
    }
}