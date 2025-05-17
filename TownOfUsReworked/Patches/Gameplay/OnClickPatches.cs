namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
public static class PlayerControlOnClick
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (!CustomPlayer.Local || !CustomPlayer.Local.Data || !__instance.Data || Meeting() || Lobby() || CustomPlayer.Local.IsBlocked())
            return false;

        if (IsHnS())
            return true;

        if (CustomButton.AllButtons.TryFinding(x => x.Owner.Local && x.Clickable() && ((__instance.AmOwner && x.Type.HasFlag(AbilityTypes.Targetless)) || x.Target == __instance), out var button))
        {
            button.Clicked();
            return false;
        }

        if (CustomPlayer.Local.Data.Tasks is null || __instance.AmOwner || PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), __instance.GetTruePosition(),
            Constants.ShipAndObjectsMask, false))
        {
            return false;
        }

        CallRpc(CustomRPC.Misc, MiscRPC.Catch, __instance, CustomPlayer.Local);
        CatchPostmortal(__instance, CustomPlayer.Local);
        return false;
    }

    public static void CatchPostmortal(PlayerControl ghosty, PlayerControl clicker)
    {
        if (!ghosty.Is<IGhosty>(out var role) || !role.CanBeClicked(clicker))
            return;

        role.Caught = true;
        ghosty.CustomDie(DeathReasonEnum.Caught, clicker);
    }
}

[HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
public static class DeadBodyOnClick
{
    public static bool Prefix(DeadBody __instance)
    {
        if (Meeting() || Lobby() || IsHnS() || PerformReport.ReportPressed || CustomPlayer.Local.IsBlocked())
            return true;

        if (!CustomButton.AllButtons.TryFinding(x => x.Owner.Local && x.Target == __instance && x.Clickable(), out var button))
            return GameModeSettings.GameMode is not (Mode.HideAndSeek or Mode.TaskRace) && GetDistance(CustomPlayer.Local, __instance) < CustomPlayer.Local.lightSource.viewDistance;

        button?.Clicked();
        return false;
    }
}