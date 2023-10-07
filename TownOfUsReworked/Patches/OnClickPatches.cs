namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
[HarmonyPriority(Priority.First)]
public static class PlayerControlOnClick
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (__instance == null || Meeting || CustomPlayer.LocalCustom.IsDead || LobbyBehaviour.Instance)
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

        if (CustomPlayer.Local == null || CustomPlayer.Local.Data == null || CustomPlayer.Local.Data.Tasks == null || __instance == CustomPlayer.Local)
            return false;

        var tasksLeft = __instance.Data.Tasks.Count(x => !x.Complete);

        if (__instance.Is(LayerEnum.Phantom))
        {
            if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
            {
                Role.GetRole<Phantom>(__instance).Caught = true;
                __instance.Exiled();
                CallRpc(CustomRPC.Misc, MiscRPC.CatchPhantom, __instance);
            }
        }
        else if (__instance.Is(LayerEnum.Revealer))
        {
            if ((CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.EvilsOnly && !(CustomPlayer.Local.Is(Faction.Intruder) || CustomPlayer.Local.Is(Faction.Syndicate))) ||
                (CustomGameOptions.RevealerCanBeClickedBy == RevealerCanBeClickedBy.NonCrew && CustomPlayer.Local.Is(Faction.Crew)))
            {
                return false;
            }

            if (tasksLeft <= CustomGameOptions.RevealerTasksRemainingClicked)
            {
                var role = Role.GetRole<Revealer>(__instance);
                role.Caught = true;
                role.CompletedTasks = false;
                __instance.Exiled();
                CallRpc(CustomRPC.Misc, MiscRPC.CatchRevealer, __instance);
            }
        }
        else if (__instance.Is(LayerEnum.Banshee))
        {
            if (!CustomPlayer.Local.Is(Faction.Syndicate))
            {
                Role.GetRole<Banshee>(__instance).Caught = true;
                __instance.Exiled();
                CallRpc(CustomRPC.Misc, MiscRPC.CatchBanshee, __instance);
            }
        }
        else if (__instance.Is(LayerEnum.Ghoul))
        {
            if (!CustomPlayer.Local.Is(Faction.Intruder))
            {
                Role.GetRole<Ghoul>(__instance).Caught = true;
                __instance.Exiled();
                CallRpc(CustomRPC.Misc, MiscRPC.CatchGhoul, __instance);
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
[HarmonyPriority(Priority.First)]
public static class DeadBodyOnClick
{
    public static bool Prefix(DeadBody __instance)
    {
        if (__instance == null || Meeting || LobbyBehaviour.Instance || IsHnS)
            return true;

        var button = CustomButton.AllButtons.Find(x => x.Owner.Local && x.TargetBody == __instance && x.Clickable&& !x.Blocked);
        button?.Clicked();
        return button == null && !IsTaskRace && !IsCustomHnS;
    }
}