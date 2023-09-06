namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
public static class AirshipExile
{
    public static void Postfix(AirshipExileController __instance) => SetPostmortals.ExileControllerPostfix(__instance);
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), new Type[] { typeof(GameObject) })]
public static class SubmergedExile
{
    public static void Prefix(GameObject obj)
    {
        if (!SubLoaded || TownOfUsReworked.NormalOptions?.MapId != 5)
            return;

        if (obj.name?.Contains("ExileCutscene") == true)
            SetPostmortals.ExileControllerPostfix(ConfirmEjects.LastExiled);
        else if (obj.name.Contains("SpawnInMinigame"))
        {
            if (CustomPlayer.Local.Is(LayerEnum.Astral) && !CustomPlayer.LocalCustom.IsDead)
                Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();
        }
    }
}

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class SetPostmortals
{
    public static readonly List<PlayerControl> AssassinatedPlayers = new();
    public static readonly List<PlayerControl> EscapedPlayers = new();

    public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

    public static void ExileControllerPostfix(ExileController __instance)
    {
        if (CustomPlayer.LocalCustom.Data.Disconnected)
            return;

        if (CustomPlayer.Local.Is(LayerEnum.Astral) && !CustomPlayer.LocalCustom.IsDead)
            Modifier.GetModifier<Astral>(CustomPlayer.Local).SetPosition();

        foreach (var player in AssassinatedPlayers)
        {
            if (!player.Data.Disconnected)
                player.Exiled();
        }

        AssassinatedPlayers.Clear();

        foreach (var ghoul in Role.GetRoles<Ghoul>(LayerEnum.Ghoul))
        {
            if (ghoul.Caught)
                ghoul.MarkedPlayer = null;
            else if (ghoul.MarkedPlayer != null && !ghoul.MarkedPlayer.HasDied())
                ghoul.MarkedPlayer.Exiled();
        }

        var exiled = __instance.exiled?.Object;

        if (exiled != null)
        {
            JesterWin(exiled);
            ExecutionerWin(exiled);

            if (exiled.Is(LayerEnum.Lovers))
            {
                var lover = exiled.GetOtherLover();

                if (!lover.Is(LayerEnum.Pestilence) && CustomGameOptions.BothLoversDie)
                    lover?.Exiled();
            }
        }

        foreach (var dict in Role.GetRoles<Dictator>(LayerEnum.Dictator))
        {
            if (dict.Revealed && dict.ToBeEjected.Count > 0 && !dict.ToBeEjected.Any(x => x == 255))
            {
                foreach (var exiled1 in dict.ToBeEjected)
                {
                    var player = PlayerById(exiled1);

                    if (player == null)
                        continue;

                    player.Exiled();
                    var role = Role.GetRole(player);
                    role.KilledBy = " By " + dict.PlayerName;
                    role.DeathReason = DeathReasonEnum.Dictated;
                }

                if (dict.ToDie)
                {
                    dict.Player.Exiled();
                    dict.DeathReason = DeathReasonEnum.Suicide;
                }

                dict.Ejected = true;
                dict.ToBeEjected.Clear();
            }
        }

        foreach (var bh in Role.GetRoles<BountyHunter>(LayerEnum.BountyHunter))
        {
            if (bh.TargetKilled && !bh.IsDead)
            {
                bh.Player.Exiled();
                bh.DeathReason = DeathReasonEnum.Escaped;
                EscapedPlayers.Add(bh.Player);
            }
        }

        foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
        {
            if (exe.TargetVotedOut && !exe.IsDead)
            {
                exe.Player.Exiled();
                exe.DeathReason = DeathReasonEnum.Escaped;
                EscapedPlayers.Add(exe.Player);
            }
        }

        foreach (var guess in Role.GetRoles<Guesser>(LayerEnum.Guesser))
        {
            if (guess.TargetGuessed && !guess.IsDead)
            {
                guess.Player.Exiled();
                guess.DeathReason = DeathReasonEnum.Escaped;
                EscapedPlayers.Add(guess.Player);
            }
        }

        foreach (var cann in Role.GetRoles<Cannibal>(LayerEnum.Cannibal))
        {
            if (cann.Eaten && !cann.IsDead)
            {
                cann.Player.Exiled();
                cann.DeathReason = DeathReasonEnum.Escaped;
                EscapedPlayers.Add(cann.Player);
            }
        }

        foreach (var vigi in Role.GetRoles<Vigilante>(LayerEnum.Vigilante))
        {
            if (vigi.PostMeetingDie)
            {
                vigi.Player.Exiled();
                vigi.DeathReason = DeathReasonEnum.Suicide;
                RecentlyKilled.Add(vigi.Player);
            }
        }

        BeginPostmortals(exiled);
    }

    private static void BeginPostmortals(PlayerControl player)
    {
        SetRevealers(player);
        SetPhantoms(player);
        SetBanshees(player);
        SetGhouls(player);
    }

    private static void JesterWin(PlayerControl player)
    {
        foreach (var jest in Role.GetRoles<Jester>(LayerEnum.Jester))
        {
            if (jest.Player == player)
            {
                jest.VotedOut = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.JesterWin, jest);
            }
        }
    }

    private static void ExecutionerWin(PlayerControl player)
    {
        foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
        {
            if (exe.TargetPlayer == null || (!CustomGameOptions.ExeCanWinBeyondDeath && exe.IsDead))
                continue;

            if (player == exe.TargetPlayer)
            {
                exe.TargetVotedOut = true;
                CallRpc(CustomRPC.WinLose, WinLoseRPC.ExecutionerWin, exe);
            }
        }
    }

    private static void SetStartingVent(PlayerControl player)
    {
        var vents = ShipStatus.Instance.AllVents.ToList();
        var clean = PlayerControl.LocalPlayer.myTasks.ToArray().Where(x => x.TaskType == TaskTypes.VentCleaning).ToList();

        if (clean != null)
        {
            var ids = clean.Where(x => !x.IsComplete).ToList().ConvertAll(x => x.FindConsoles()[0].ConsoleId);
            vents = ShipStatus.Instance.AllVents.Where(x => !ids.Contains(x.Id)).ToList();
        }

        var startingVent = vents[URandom.RandomRangeInt(0, vents.Count)];
        player.NetTransform.RpcSnapTo(new(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
        player.MyPhysics.RpcEnterVent(startingVent.Id);
    }

    public static List<PlayerControl> WillBeRevealers = new();
    public static bool RevealerOn;

    private static void SetRevealers(PlayerControl exiled)
    {
        if (!RevealerOn)
            return;

        if (!WillBeRevealers.Contains(exiled) && WillBeRevealers.Count < CustomGameOptions.RevealerCount && exiled.Is(Faction.Crew))
            WillBeRevealers.Add(exiled);

        foreach (var rev in WillBeRevealers)
        {
            if (!rev.Data.IsDead)
                continue;

            if (!rev.Is(LayerEnum.Revealer))
            {
                var former = Role.GetRole(rev);
                var role = new Revealer(rev) { FormerRole = former };
                role.RoleUpdate(former);
                RemoveTasks(CustomPlayer.Local);
                rev.gameObject.layer = LayerMask.NameToLayer("Players");

                if (CustomPlayer.Local != rev)
                    CustomPlayer.Local.MyPhysics.ResetMoveState();
            }

            if (rev == CustomPlayer.Local)
            {
                if (Role.GetRole<Revealer>(rev).Caught)
                    continue;

                SetStartingVent(rev);
            }
        }
    }

    public static List<PlayerControl> WillBePhantoms = new();
    public static bool PhantomOn;

    private static void SetPhantoms(PlayerControl exiled)
    {
        if (!PhantomOn)
            return;

        if (!WillBePhantoms.Contains(exiled) && WillBePhantoms.Count < CustomGameOptions.PhantomCount && exiled.Is(Faction.Neutral) &&
            !NeutralHasUnfinishedBusiness(exiled))
        {
            WillBePhantoms.Add(exiled);
        }

        foreach (var phan in WillBePhantoms)
        {
            if (!phan.Data.IsDead)
                continue;

            if (!phan.Is(LayerEnum.Phantom))
            {
                var former = Role.GetRole(phan);
                var role = new Phantom(phan);
                role.RoleUpdate(former);
                RemoveTasks(phan);
                phan.gameObject.layer = LayerMask.NameToLayer("Players");

                if (CustomPlayer.Local != phan)
                    CustomPlayer.Local.MyPhysics.ResetMoveState();
            }

            if (phan == CustomPlayer.Local)
            {
                if (Role.GetRole<Phantom>(phan).Caught)
                    continue;

                SetStartingVent(phan);
            }
        }
    }

    public static List<PlayerControl> WillBeBanshees = new();
    public static bool BansheeOn;

    private static void SetBanshees(PlayerControl exiled)
    {
        if (!BansheeOn)
            return;

        if (!WillBeBanshees.Contains(exiled) && WillBeBanshees.Count < CustomGameOptions.BansheeCount && exiled.Is(Faction.Syndicate))
            WillBeBanshees.Add(exiled);

        foreach (var ban in WillBeBanshees)
        {
            if (!ban.Data.IsDead)
                continue;

            if (!ban.Is(LayerEnum.Banshee))
            {
                var former = Role.GetRole(ban);
                var role = new Banshee(ban);
                role.RoleUpdate(former);
                ban.gameObject.layer = LayerMask.NameToLayer("Players");

                if (CustomPlayer.Local != ban)
                    CustomPlayer.Local.MyPhysics.ResetMoveState();
            }

            if (ban == CustomPlayer.Local)
            {
                if (Role.GetRole<Banshee>(ban).Caught)
                    continue;

                SetStartingVent(ban);
            }
        }
    }

    public static List<PlayerControl> WillBeGhouls = new();
    public static bool GhoulOn;

    private static void SetGhouls(PlayerControl exiled)
    {
        if (!GhoulOn)
            return;

        if (!WillBeGhouls.Contains(exiled) && WillBeGhouls.Count < CustomGameOptions.GhoulCount && exiled.Is(Faction.Intruder))
            WillBeGhouls.Add(exiled);

        foreach (var ghoul in WillBeGhouls)
        {
            if (!ghoul.Data.IsDead)
                continue;

            if (!ghoul.Is(LayerEnum.Ghoul))
            {
                var former = Role.GetRole(ghoul);
                var role = new Ghoul(ghoul);
                role.RoleUpdate(former);
                ghoul.gameObject.layer = LayerMask.NameToLayer("Players");

                if (CustomPlayer.Local != ghoul)
                    CustomPlayer.Local.MyPhysics.ResetMoveState();
            }

            if (ghoul == CustomPlayer.Local)
            {
                if (Role.GetRole<Ghoul>(ghoul).Caught)
                    continue;

                SetStartingVent(ghoul);
            }
        }
    }
}