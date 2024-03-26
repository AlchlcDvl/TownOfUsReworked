namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
public static class PlayerControlRevivePatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (!__instance.Data.IsDead)
            return false;

        __instance.Data.IsDead = false;
        __instance.gameObject.layer = LayerMask.NameToLayer("Players");
        __instance.MyPhysics.ResetMoveState();
        __instance.clickKillCollider.enabled = true;
        __instance.Collider.enabled = true;
        __instance.cosmetics.SetPetSource(__instance);
        __instance.cosmetics.SetNameMask(true);
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId || !CustomPlayer.AllPlayers.Any(y => y.PlayerId == x.PlayerId));
        RecentlyKilled.RemoveAll(x => x == __instance.PlayerId || !PlayerById(x) || !CustomPlayer.AllPlayers.Any(y => y.PlayerId == x));
        Role.Cleaned.RemoveAll(x => x == __instance.PlayerId || !CustomPlayer.AllPlayers.Any(y => y.PlayerId == x) || !PlayerById(x));
        SetPostmortals.RemoveFromPostmortals(__instance);
        __instance.SetImpostor(__instance.GetFaction() is Faction.Intruder or Faction.Syndicate);
        var body = BodyByPlayer(__instance);

        if (body != null)
        {
            __instance.RpcCustomSnapTo(body.TruePosition);
            body.gameObject.Destroy();
        }

        if (IsSubmerged() && CustomPlayer.Local == __instance)
            ChangeFloor(__instance.transform.position.y > -7);

        if (__instance.TryGetLayer<Troll>(LayerEnum.Troll, out var troll))
            troll.Killed = false;

        if (!__instance.AmOwner)
            return false;

        HUD.ShadowQuad.gameObject.SetActive(true);
        HUD.KillButton.gameObject.SetActive(false);
        HUD.AdminButton.gameObject.SetActive(__instance.IsImpostor() && IsHnS);
        HUD.SabotageButton.gameObject.SetActive(__instance.CanSabotage());
        HUD.ImpostorVentButton.gameObject.SetActive(__instance.CanVent());
        ButtonUtils.Reset(player: __instance);

        if (Chat.IsOpenOrOpening)
            Chat.ForceClosed();

        Chat.SetVisible(__instance.CanChat());
        return false;
    }
}