namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
public static class PlayerControlOnClick
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (CustomPlayer.Local == null || CustomPlayer.Local.Data == null || __instance == null || __instance.Data == null || Meeting || Lobby)
            return false;

        if (IsHnS)
            return true;

        var button = CustomButton.AllButtons.Find(x => x.Owner.Local && x.TargetPlayer == __instance && x.Clickable);

        if (button == null)
            button = CustomButton.AllButtons.Find(x => x.Owner.Local && x.Clickable && __instance == CustomPlayer.Local && x.Type == AbilityTypes.Targetless);

        if (button != null)
        {
            button.Clicked();
            return false;
        }

        if (CustomPlayer.Local.Data.Tasks == null || __instance == CustomPlayer.Local)
            return false;

        CatchPostmortal(__instance, CustomPlayer.Local);
        CallRpc(CustomRPC.Misc, MiscRPC.Catch, __instance, CustomPlayer.Local);
        return false;
    }

    public static void CatchPostmortal(PlayerControl player, PlayerControl clicker)
    {
        var tasksLeft = player.Data.Tasks.Count(x => !x.Complete);
        var role = Role.GetRole(player);

        if (role is Phantom phantom)
        {
            if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                phantom.Caught = true;
            else
                return;
        }
        else if (role is Revealer revealer)
        {
            if ((CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate)) ||
                (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew && CustomPlayer.Local.Is(Faction.Crew)))
            {
                return;
            }

            if (tasksLeft <= CustomGameOptions.RevealerTasksRemainingClicked)
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
        if (__instance == null || Meeting || Lobby || IsHnS)
            return true;

        var button = CustomButton.AllButtons.Find(x => x.Owner.Local && x.TargetBody == __instance && x.Clickable && !x.Blocked);
        button?.Clicked();
        return button == null && !IsTaskRace && !IsCustomHnS;
    }
}