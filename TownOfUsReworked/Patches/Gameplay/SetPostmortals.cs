namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch]
public static class SetPostmortals
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static void Postfix(ExileController __instance)
    {
        if (LocalPlayer.Data.Disconnected)
            return;

        foreach (var ghoul in PlayerLayer.GetLayers<Ghoul>())
        {
            if (ghoul.Caught)
                ghoul.MarkedPlayer = null;
            else if (ghoul.MarkedPlayer && !ghoul.MarkedPlayer.HasDied() && !ghoul.MarkedPlayer.Is(Alignment.Deity))
            {
                ghoul.MarkedPlayer.CustomDie(DeathReasonEnum.Marked, ghoul.Player);

                if (Lovers.BothLoversDie && ghoul.MarkedPlayer.Is<Lovers>(out var lover) && !lover.Other.Is(Alignment.Deity))
                    lover.Other.CustomDie(DeathReasonEnum.Marked, ghoul.Player);

                ghoul.MarkedPlayer = null;
            }
        }

        var exiled = __instance.initData?.networkedPlayer?.Object;

        if (exiled)
        {
            JesterWin(exiled);
            ExecutionerWin(exiled);

            if (Lovers.BothLoversDie && exiled.Is<Lovers>(out var lover) && !lover.Other.Is(Alignment.Deity))
                lover.Other.CustomDie(DeathReasonEnum.Suicide);
        }

        foreach (var dict in PlayerLayer.GetLayers<Dictator>())
        {
            if (!dict.Revealed || !dict.ToBeEjected)
                continue;

            dict.ToBeEjected.CustomDie(DeathReasonEnum.Dictated, dict.Player);

            if (dict.ToBeEjected.Is(Faction.Crew))
                dict.Player.CustomDie(DeathReasonEnum.Suicide);

            dict.ToBeEjected = null;
        }

        foreach (var vigi in PlayerLayer.GetLayers<Vigilante>())
        {
            if (vigi.PostMeetingDie)
                vigi.Player.CustomDie(DeathReasonEnum.Suicide);
        }

        if (OutcastSettings.AvoidOutcastKingmakers)
        {
            foreach (var ne in PlayerLayer.GetLayers<Evil>())
            {
                if (ne is { HasWon: true, Dead: false })
                    ne.Player.CustomDie(DeathReasonEnum.Escaped);
            }
        }

        if (LocalPlayer.Is<Astral>(out var ast) && !ast.Dead)
            ast.SetPosition();

        BeginPostmortals(exiled, true);

        foreach (var player in AllPlayers())
        {
            player.MyPhysics.ResetAnimState();
            player.MyPhysics.ResetMoveState();
        }

        AllBodies().Do(x => x?.gameObject?.Destroy());
        PlayerLayer.GetLayers<Retributionist>().Do(x => x.OnRoleSelected());
    }

    public static void BeginPostmortals(PlayerControl player, bool ejection)
    {
        SetRevealers(player, ejection);
        SetPhantoms(player, ejection);
        SetBanshees(player, ejection);
        SetGhouls(player, ejection);
    }

    private static void JesterWin(PlayerControl player)
    {
        if (player.Is<Jester>(out var jest))
            jest.VotedOut = true;
    }

    private static void ExecutionerWin(PlayerControl player)
    {
        foreach (var exe in PlayerLayer.GetLayers<Executioner>())
        {
            if ((Executioner.ExeCanWinBeyondDeath || !exe.Dead) && player == exe.TargetPlayer)
                exe.TargetVotedOut = true;
        }
    }

    private static void SetStartingPos(PlayerControl player)
    {
        if (!player.Data.IsDead || GameOptions.GhostSpawn == GhostSpawnType.AtMeeting || !player.Is<IGhosty>(out var ghost) || ghost.Caught)
            return;

        if (GameOptions.GhostSpawn == GhostSpawnType.PosBeforeMeeting)
        {
            player.RpcCustomSnapTo(ghost.LastPosition);
            return;
        }

        var ventsArray = AllMapVents().ToArray();
        var vents = ventsArray.ToList();

        if (Ship().Systems.TryGetValue(SystemTypes.Ventilation, out var systemType) && systemType.TryCast<VentilationSystem>(out var ventilationSystem))
            vents.RemoveAll(x => !ventilationSystem.PlayersCleaningVents.ContainsValue((byte)x.Id));

        if (IsSubmerged())
            vents.RemoveAll(x => ventsArray.IndexOf(x) is 0 or 14);

        vents.Shuffle();
        var startingVent = vents.Random();
        player.RpcCustomSnapTo(startingVent.transform.position);
    }

    public static readonly HashSet<byte> WillBeRevealers = [];
    public static byte Revealers;

    private static void SetRevealers(PlayerControl dead, bool ejection)
    {
        if (Revealers == 0)
            return;

        TryAddRevealer(dead);
        var remove = new List<byte>();

        foreach (var revId in WillBeRevealers)
        {
            var rev = PlayerById(revId);

            if (!rev.HasDied())
            {
                remove.Add(revId);
                continue;
            }

            if (!ejection)
                continue;

            if (!rev.Is<Revealer>(out var revealer))
            {
                var former = rev.GetRole();
                (revealer = new() { FormerRole = former }).RoleUpdate(former);
                ((IGhosty)revealer).OnStart();
            }

            rev.GetComponent<PassiveButton>().OverrideOnClickListeners(rev.OnClick);

            if (rev.AmOwner && !revealer.Caught)
                SetStartingPos(rev);
        }

        WillBeRevealers.RemoveAll(remove.Contains);
    }

    private static void TryAddRevealer(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeRevealers.Contains(dead.PlayerId) && WillBeRevealers.Count < Revealers && dead.Is<Crew>())
            WillBeRevealers.Add(dead.PlayerId);
    }

    public static readonly HashSet<byte> WillBePhantoms = [];
    public static byte Phantoms;

    private static void SetPhantoms(PlayerControl dead, bool ejection)
    {
        if (Phantoms == 0)
            return;

        TryAddPhantom(dead);
        var remove = new List<byte>();

        foreach (var phanId in WillBePhantoms)
        {
            var phan = PlayerById(phanId);

            if (!phan.HasDied())
            {
                remove.Add(phanId);
                continue;
            }

            if (!ejection)
                continue;

            if (!phan.Is<Phantom>(out var phantom))
            {
                (phantom = new()).RoleUpdate(phan.GetRole());
                ((IGhosty)phantom).OnStart();
            }

            phan.GetComponent<PassiveButton>().OverrideOnClickListeners(phan.OnClick);

            if (phan.AmOwner && !phantom.Caught)
                SetStartingPos(phan);
        }

        WillBePhantoms.RemoveAll(remove.Contains);
    }

    private static void TryAddPhantom(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBePhantoms.Contains(dead.PlayerId) && WillBePhantoms.Count < Phantoms && dead.Is<Outcast>() && !IsExcludedOutcast(dead))
            WillBePhantoms.Add(dead.PlayerId);
    }

    public static readonly HashSet<byte> WillBeBanshees = [];
    public static byte Banshees;

    private static void SetBanshees(PlayerControl dead, bool ejection)
    {
        if (Banshees == 0)
            return;

        TryAddBanshee(dead);
        var remove = new List<byte>();

        foreach (var banId in WillBeBanshees)
        {
            var ban = PlayerById(banId);

            if (!ban.HasDied())
            {
                remove.Add(banId);
                continue;
            }

            if (!ejection)
                continue;

            if (!ban.Is<Banshee>(out var banshee))
            {
                (banshee = new()).RoleUpdate(ban.GetRole());
                ((IGhosty)banshee).OnStart();
            }

            ban.GetComponent<PassiveButton>().OverrideOnClickListeners(ban.OnClick);

            if (ban.AmOwner && !banshee.Caught)
                SetStartingPos(ban);
        }

        WillBeBanshees.RemoveAll(remove.Contains);
    }

    private static void TryAddBanshee(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeBanshees.Contains(dead.PlayerId) && WillBeBanshees.Count < Banshees && dead.Is<Syndicate>())
            WillBeBanshees.Add(dead.PlayerId);
    }

    public static readonly HashSet<byte> WillBeGhouls = [];
    public static byte Ghouls;

    private static void SetGhouls(PlayerControl dead, bool ejection)
    {
        if (Ghouls == 0)
            return;

        TryAddGhoul(dead);
        var remove = new List<byte>();

        foreach (var ghoulId in WillBeGhouls)
        {
            var ghoul = PlayerById(ghoulId);

            if (!ghoul.HasDied())
            {
                remove.Add(ghoulId);
                continue;
            }

            if (!ejection)
                continue;

            if (!ghoul.Is<Ghoul>(out var gho))
            {
                (gho = new()).RoleUpdate(ghoul.GetRole());
                ((IGhosty)gho).OnStart();
            }

            ghoul.GetComponent<PassiveButton>().OverrideOnClickListeners(ghoul.OnClick);

            if (ghoul.AmOwner && !gho.Caught)
                SetStartingPos(ghoul);
        }

        WillBeGhouls.RemoveAll(remove.Contains);
    }

    private static void TryAddGhoul(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeGhouls.Contains(dead.PlayerId) && WillBeGhouls.Count < Ghouls && dead.Is<Intruder>())
            WillBeGhouls.Add(dead.PlayerId);
    }

    public static void RemoveFromPostmortals(PlayerControl player)
    {
        WillBeRevealers.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBePhantoms.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBeBanshees.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBeGhouls.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
    }
}