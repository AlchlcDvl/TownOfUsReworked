namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
public static class PlayerControlOnClick
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (!CustomPlayer.Local ||! CustomPlayer.Local.Data || !__instance.Data || Meeting() || Lobby())
            return false;

        if (IsHnS())
            return true;

        if (AllButtons.TryFinding(x => x.Owner.Local && x.Clickable() && ((__instance.AmOwner && x.Type == AbilityType.Targetless) || x.Target == __instance), out var button))
        {
            button.Clicked();
            return false;
        }

        if (CustomPlayer.Local.Data.Tasks == null || __instance.AmOwner)
            return false;

        CallRpc(CustomRPC.Misc, MiscRPC.Catch, __instance, CustomPlayer.Local);
        CatchPostmortal(__instance, CustomPlayer.Local);
        return false;
    }

    public static void CatchPostmortal(PlayerControl player, PlayerControl clicker)
    {
        var role = player.GetRole();

        if (role is Phantom phantom)
        {
            if (role.TasksLeft <= Phantom.PhantomTasksRemaining)
                phantom.Caught = true;
            else
                return;
        }
        else if (role is Revealer revealer)
        {
            if ((Revealer.RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate)) ||
                (Revealer.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew && CustomPlayer.Local.Is(Faction.Crew)))
            {
                return;
            }

            if (role.TasksLeft <= Revealer.RevealerTasksRemainingClicked)
                revealer.Caught = true;
            else
                return;
        }
        else if (role is Banshee banshee)
        {
            if (!CustomPlayer.Local.Is(Faction.Syndicate))
                banshee.Caught = true;
            else
                return;
        }
        else if (role is Ghoul ghoul)
        {
            if (!CustomPlayer.Local.Is(Faction.Intruder))
                ghoul.Caught = true;
            else
                return;
        }
        else
            return;

        player.Exiled();
        role.KilledBy = " By " + clicker.name;
        role.DeathReason = DeathReasonEnum.Clicked;
    }
}

[HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
public static class DeadBodyOnClick
{
    public static bool Prefix(DeadBody __instance)
    {
        if (Meeting() || Lobby() || IsHnS() || PerformReport.ReportPressed)
            return true;

        var result = AllButtons.TryFinding(x => x.Owner.Local && x.Target == __instance && x.Clickable() && !x.Owner.IsBlocked, out var button);
        button?.Clicked();
        return !result && !IsTaskRace() && !IsCustomHnS();
    }
}