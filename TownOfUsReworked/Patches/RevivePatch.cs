namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
public static class PlayerControlRevivePatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        CustomRevive(__instance);
        return false;
    }

    private static void CustomRevive(PlayerControl __instance)
    {
        if (!__instance.Data.IsDead)
            return;

        __instance.Data.IsDead = false;
        __instance.gameObject.layer = LayerMask.NameToLayer("Players");
        __instance.MyPhysics.ResetMoveState();
        __instance.clickKillCollider.enabled = true;
        __instance.Collider.enabled = true;
        __instance.cosmetics.SetPetSource(__instance);
        __instance.cosmetics.SetNameMask(true);
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId);
        RecentlyKilled.RemoveAll(x => x.PlayerId == __instance.PlayerId);
        Role.Cleaned.RemoveAll(x => x.PlayerId == __instance.PlayerId);
        ReassignPostmortals(__instance);
        __instance.Data.SetImpostor(__instance.Is(Faction.Intruder) || __instance.Is(Faction.Syndicate));
        var body = BodyByPlayer(__instance);

        if (body != null)
        {
            var position = body.TruePosition;
            __instance.NetTransform.RpcSnapTo(new(position.x, position.y + 0.3636f));
            body.gameObject.Destroy();
        }

        if (IsSubmerged && CustomPlayer.Local == __instance)
            ChangeFloor(__instance.transform.position.y > -7);

        if (__instance.Is(LayerEnum.Troll))
            Role.GetRole<Troll>(__instance).Killed = false;

        if (!__instance.AmOwner)
            return;

        HUD.ShadowQuad.gameObject.SetActive(true);
        HUD.KillButton.ToggleVisible(false);
        HUD.AdminButton.ToggleVisible(__instance.Data.IsImpostor() && IsHnS);
        HUD.SabotageButton.ToggleVisible(__instance.CanSabotage());
        HUD.ImpostorVentButton.ToggleVisible(__instance.CanVent());
        ButtonUtils.ResetCustomTimers();

        if (HUD.Chat.IsOpenOrOpening)
            HUD.Chat.ForceClosed();

        HUD.Chat.SetVisible(__instance.CanChat());
    }
}