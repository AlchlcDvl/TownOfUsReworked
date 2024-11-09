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
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId || !AllPlayers().Any(y => y.PlayerId == x.PlayerId));
        RecentlyKilled.RemoveAll(x => x == __instance.PlayerId || !PlayerById(x) || !AllPlayers().Any(y => y.PlayerId == x));
        Role.Cleaned.RemoveAll(x => x == __instance.PlayerId || !AllPlayers().Any(y => y.PlayerId == x) || !PlayerById(x));
        BodyLocations.Remove(__instance.PlayerId);
        SetPostmortals.RemoveFromPostmortals(__instance);
        var body = BodyByPlayer(__instance);

        if (body)
        {
            __instance.RpcCustomSnapTo(body.TruePosition);
            body.gameObject.Destroy();
        }

        if (IsSubmerged() && __instance.AmOwner)
            ChangeFloor(__instance.transform.position.y > -7);

        __instance.GetLayers().ForEach(x => x.OnRevive());

        if (!__instance.AmOwner)
            return false;

        HUD().ShadowQuad.gameObject.SetActive(true);
        HUD().KillButton.gameObject.SetActive(IsHnS());
        HUD().AdminButton.gameObject.SetActive(__instance.IsImpostor() && IsHnS());
        HUD().SabotageButton.gameObject.SetActive(__instance.CanSabotage());
        HUD().ImpostorVentButton.gameObject.SetActive(__instance.CanVent());
        ButtonUtils.Reset(player: __instance);

        if (Chat().IsOpenOrOpening)
            Chat().ForceClosed();

        Chat().SetVisible(__instance.CanChat());
        return false;
    }
}