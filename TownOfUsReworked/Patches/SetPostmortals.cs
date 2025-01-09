namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class SetPostmortals
{
    public static readonly List<byte> AssassinatedPlayers = [];
    public static readonly List<byte> EscapedPlayers = [];
    public static readonly List<byte> MarkedPlayers = [];
    public static readonly List<byte> MisfiredPlayers = [];

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static void ExileControllerPostfix(ExileController __instance)
    {
        if (CustomPlayer.LocalCustom.Disconnected)
            return;

        if (CustomPlayer.Local.TryGetLayer<Astral>(out var ast))
            ast.SetPosition();

        foreach (var id in AssassinatedPlayers)
        {
            var player = PlayerById(id);

            if (!player.HasDied())
                player.Exiled();
        }

        AssassinatedPlayers.Clear();
        EscapedPlayers.Clear();
        MarkedPlayers.Clear();
        MisfiredPlayers.Clear();

        foreach (var ghoul in PlayerLayer.GetLayers<Ghoul>())
        {
            if (ghoul.Caught)
                ghoul.MarkedPlayer = null;
            else if (ghoul.MarkedPlayer && !ghoul.MarkedPlayer.HasDied() && !ghoul.MarkedPlayer.Is(Alignment.NeutralApoc))
            {
                ghoul.MarkedPlayer.CustomDie(DeathReasonEnum.Marked, ghoul.Player);
                MarkedPlayers.Add(ghoul.MarkedPlayer.PlayerId);
                ghoul.MarkedPlayer = null;
            }
        }

        var exiled = __instance.initData?.networkedPlayer?.Object;

        if (exiled)
        {
            JesterWin(exiled);
            ExecutionerWin(exiled);

            if (Lovers.BothLoversDie && exiled.TryGetLayer<Lovers>(out var lover) && !lover.OtherLover.Is(Alignment.NeutralApoc))
                lover.OtherLover.CustomDie(DeathReasonEnum.Suicide);
        }

        foreach (var dict in PlayerLayer.GetLayers<Dictator>())
        {
            if (dict.Revealed && dict.ToBeEjected)
            {
                dict.ToBeEjected.CustomDie(DeathReasonEnum.Dictated, dict.Player);

                if (dict.ToBeEjected.Is(Faction.Crew) && dict.ToBeEjected.Is(SubFaction.None))
                {
                    dict.Player.CustomDie(DeathReasonEnum.Suicide);
                    MisfiredPlayers.Add(dict.Player.PlayerId);
                }

                dict.ToBeEjected = null;
            }
        }

        foreach (var vigi in PlayerLayer.GetLayers<Vigilante>())
        {
            if (vigi.PostMeetingDie)
            {
                vigi.Player.CustomDie(DeathReasonEnum.Suicide);
                MisfiredPlayers.Add(vigi.Player.PlayerId);
            }
        }

        if (NeutralSettings.AvoidNeutralKingmakers)
        {
            foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
            {
                if (bh.TargetKilled && !bh.Dead)
                {
                    bh.Player.CustomDie(DeathReasonEnum.Escaped);
                    EscapedPlayers.Add(bh.Player.PlayerId);
                }
            }

            foreach (var exe in PlayerLayer.GetLayers<Executioner>())
            {
                if (exe.TargetVotedOut && !exe.Dead)
                {
                    exe.Player.CustomDie(DeathReasonEnum.Escaped);
                    EscapedPlayers.Add(exe.Player.PlayerId);
                }
            }

            foreach (var guess in PlayerLayer.GetLayers<Guesser>())
            {
                if (guess.TargetGuessed && !guess.Dead)
                {
                    guess.Player.CustomDie(DeathReasonEnum.Escaped);
                    EscapedPlayers.Add(guess.Player.PlayerId);
                }
            }

            foreach (var cann in PlayerLayer.GetLayers<Cannibal>())
            {
                if (cann.Eaten && !cann.Dead)
                {
                    cann.Player.CustomDie(DeathReasonEnum.Escaped);
                    EscapedPlayers.Add(cann.Player.PlayerId);
                }
            }
        }

        BeginPostmortals(exiled, true);
        AllPlayers().ForEach(x => x?.MyPhysics?.ResetAnimState());
        AllBodies().ForEach(x => x?.gameObject?.Destroy());
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
        foreach (var jest in PlayerLayer.GetLayers<Jester>())
        {
            if (jest.Player == player)
            {
                jest.VotedOut = true;
                CallRpc(CustomRPC.WinLose, WinLose.JesterWins, jest);
            }
        }
    }

    private static void ExecutionerWin(PlayerControl player)
    {
        foreach (var exe in PlayerLayer.GetLayers<Executioner>())
        {
            if (!exe.TargetPlayer || (!Executioner.ExeCanWinBeyondDeath && exe.Dead))
                continue;

            if (player == exe.TargetPlayer)
            {
                exe.TargetVotedOut = true;
                CallRpc(CustomRPC.WinLose, WinLose.ExecutionerWins, exe);
            }
        }
    }

    private static void SetStartingVent(PlayerControl player)
    {
        if (!player.Data.IsDead || !player.IsPostmortal() || player.Caught())
            return;

        var ventsArray = AllMapVents();
        var vents = ventsArray.ToList();

        if (Ship().Systems.TryGetValue(SystemTypes.Ventilation, out var systemType) && systemType.TryCast<VentilationSystem>(out var ventilationSystem))
            vents.RemoveAll(x => !ventilationSystem.PlayersCleaningVents.ContainsValue((byte)x.Id));

        if (IsSubmerged())
            vents.RemoveAll(x => ventsArray.IndexOf(x) is 0 or 14);

        vents.Shuffle();
        var startingVent = vents.Random();
        player.RpcCustomSnapTo(startingVent.transform.position);
        player.MyPhysics.RpcEnterVent(startingVent.Id);
    }

    public static readonly List<byte> WillBeRevealers = [];
    public static byte Revealers;

    private static void SetRevealers(PlayerControl dead, bool ejection)
    {
        if (Revealers == 0)
            return;

        TryAddRevealer(dead);
        var remove = new List<byte>();

        foreach (var revid in WillBeRevealers)
        {
            var rev = PlayerById(revid);

            if (!rev.HasDied())
            {
                remove.Add(revid);
                continue;
            }

            if (!ejection)
                continue;

            if (!rev.Is<Revealer>())
            {
                var former = rev.GetRole();
                new Revealer() { FormerRole = former }.RoleUpdate(former);
                RemoveTasks(rev);
                rev.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            rev.GetComponent<PassiveButton>().OverrideOnClickListeners(rev.OnClick);

            if (rev.AmOwner)
            {
                if (!rev.GetLayer<Revealer>().Caught)
                    SetStartingVent(rev);
            }
        }

        WillBeRevealers.RemoveAll(remove.Contains);
    }

    private static void TryAddRevealer(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeRevealers.Contains(dead.PlayerId) && WillBeRevealers.Count < Revealers && dead.IsBase(Faction.Crew))
            WillBeRevealers.Add(dead.PlayerId);
    }

    public static readonly List<byte> WillBePhantoms = [];
    public static byte Phantoms;

    private static void SetPhantoms(PlayerControl dead, bool ejection)
    {
        if (Phantoms == 0)
            return;

        TryAddPhantom(dead);
        var remove = new List<byte>();

        foreach (var phanid in WillBePhantoms)
        {
            var phan = PlayerById(phanid);

            if (!phan.HasDied())
            {
                remove.Add(phanid);
                continue;
            }

            if (!ejection)
                continue;

            if (!phan.Is<Phantom>())
            {
                var former = phan.GetRole();
                new Phantom().RoleUpdate(former);
                RemoveTasks(phan);
                phan.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            phan.GetComponent<PassiveButton>().OverrideOnClickListeners(phan.OnClick);

            if (phan.AmOwner)
            {
                if (!phan.GetLayer<Phantom>().Caught)
                    SetStartingVent(phan);
            }
        }

        WillBePhantoms.RemoveAll(remove.Contains);
    }

    private static void TryAddPhantom(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBePhantoms.Contains(dead.PlayerId) && WillBePhantoms.Count < Phantoms && dead.IsBase(Faction.Neutral) && !IsExcludedNeutral(dead))
            WillBePhantoms.Add(dead.PlayerId);
    }

    public static readonly List<byte> WillBeBanshees = [];
    public static byte Banshees;

    private static void SetBanshees(PlayerControl dead, bool ejection)
    {
        if (Banshees == 0)
            return;

        TryAddBanshee(dead);
        var remove = new List<byte>();

        foreach (var banid in WillBeBanshees)
        {
            var ban = PlayerById(banid);

            if (!ban.HasDied())
            {
                remove.Add(banid);
                continue;
            }

            if (!ejection)
                continue;

            if (!ban.Is<Banshee>())
            {
                var former = ban.GetRole();
                new Banshee().RoleUpdate(former);
                ban.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            ban.GetComponent<PassiveButton>().OverrideOnClickListeners(ban.OnClick);

            if (ban.AmOwner)
            {
                if (!ban.GetLayer<Banshee>().Caught)
                    SetStartingVent(ban);
            }
        }

        WillBeBanshees.RemoveAll(remove.Contains);
    }

    private static void TryAddBanshee(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeBanshees.Contains(dead.PlayerId) && WillBeBanshees.Count < Banshees && dead.IsBase(Faction.Syndicate))
            WillBeBanshees.Add(dead.PlayerId);
    }

    public static readonly List<byte> WillBeGhouls = [];
    public static byte Ghouls;

    private static void SetGhouls(PlayerControl dead, bool ejection)
    {
        if (Ghouls == 0)
            return;

        TryAddGhoul(dead);
        var remove = new List<byte>();

        foreach (var ghoulid in WillBeGhouls)
        {
            var ghoul = PlayerById(ghoulid);

            if (!ghoul.HasDied())
            {
                remove.Add(ghoulid);
                continue;
            }

            if (!ejection)
                continue;

            if (!ghoul.Is<Ghoul>())
            {
                var former = ghoul.GetRole();
                new Ghoul().RoleUpdate(former);
                ghoul.gameObject.layer = LayerMask.NameToLayer("Players");
            }

            ghoul.GetComponent<PassiveButton>().OverrideOnClickListeners(ghoul.OnClick);

            if (ghoul.AmOwner)
            {
                if (ghoul.GetLayer<Ghoul>().Caught)
                    SetStartingVent(ghoul);
            }
        }

        WillBeGhouls.RemoveAll(remove.Contains);
    }

    private static void TryAddGhoul(PlayerControl dead)
    {
        if (dead.HasDied() && dead && !WillBeGhouls.Contains(dead.PlayerId) && WillBeGhouls.Count < Ghouls && dead.IsBase(Faction.Intruder))
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