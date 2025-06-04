namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
public static class PlayerControlOnClick
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (!LocalPlayer || !LocalPlayer.Data || !__instance.Data || Meeting() || Lobby() || LocalPlayer.IsBlocked())
            return false;

        if (IsHnS())
            return true;

        if (CustomButton.AllButtons.TryFinding(x => x.Owner.Local && x.Clickable() && ((__instance.AmOwner && x.Type.HasFlag(AbilityTypes.Targetless)) || x.Target == __instance), out var button))
        {
            button.Clicked();
            return false;
        }

        if (LocalPlayer.Data.Tasks is null || __instance.AmOwner || PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), __instance.GetTruePosition(),
            Constants.ShipAndObjectsMask, false))
        {
            return false;
        }

        CatchPostmortal(__instance, LocalPlayer);
        return false;
    }

    private static void CatchPostmortal(PlayerControl ghosty, PlayerControl clicker)
    {
        if (ghosty.Is<IGhosty>(out var role) && role.CanBeClicked(clicker))
            role.Catch(clicker);
    }
}

[HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
public static class DeadBodyOnClick
{
    public static bool Prefix(DeadBody __instance)
    {
        if (Meeting() || Lobby() || IsHnS() || PerformReport.ReportPressed || LocalPlayer.IsBlocked())
            return true;

        if (!CustomButton.AllButtons.TryFinding(x => x.Owner.Local && x.Target == __instance && x.Clickable(), out var button))
            return GameModeSettings.GameMode is not (Mode.HideAndSeek or Mode.TaskRace) && GetDistance(LocalPlayer, __instance) < LocalPlayer.lightSource.viewDistance;

        button?.Clicked();
        return false;
    }
}