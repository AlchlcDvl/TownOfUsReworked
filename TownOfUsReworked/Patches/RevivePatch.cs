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
        KilledPlayers.RemoveAll(x => x.PlayerId == __instance.PlayerId || x == null);
        RecentlyKilled.RemoveAll(x => x == __instance.PlayerId || !PlayerById(x) || x == 255);
        Role.Cleaned.RemoveAll(x => x == __instance.PlayerId || x == 255 || !PlayerById(x));
        SetPostmortals.RemoveFromPostmortals(__instance);
        __instance.Data.SetImpostor(__instance.GetFaction() is Faction.Intruder or Faction.Syndicate);
        var body = BodyByPlayer(__instance);

        if (body != null)
        {
            __instance.NetTransform.RpcSnapTo(body.TruePosition);
            body.gameObject.Destroy();
        }

        if (IsSubmerged() && CustomPlayer.Local == __instance)
            ChangeFloor(__instance.transform.position.y > -7);

        if (__instance.Is(LayerEnum.Troll))
            Role.GetRole<Troll>(__instance).Killed = false;

        if (!__instance.AmOwner)
            return false;

        HUD.ShadowQuad.gameObject.SetActive(true);
        HUD.KillButton.gameObject.SetActive(false);
        HUD.AdminButton.gameObject.SetActive(__instance.IsImpostor() && IsHnS);
        HUD.SabotageButton.gameObject.SetActive(__instance.CanSabotage());
        HUD.ImpostorVentButton.gameObject.SetActive(__instance.CanVent());
        ButtonUtils.Reset();

        if (Chat.IsOpenOrOpening)
            Chat.ForceClosed();

        Chat.SetVisible(__instance.CanChat());
        return false;
    }
}