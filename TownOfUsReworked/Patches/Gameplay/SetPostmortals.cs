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
            if (vigi.KilledInno && Vigilante.HowDoesVigilanteDie == VigiOptions.PostMeeting)
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

    public static readonly HashSet<byte> WillBePhantoms = [];
    public static byte Phantoms;

    public static readonly HashSet<byte> WillBeBanshees = [];
    public static byte Banshees;

    public static readonly HashSet<byte> WillBeGhouls = [];
    public static byte Ghouls;

    public static void BeginPostmortals(PlayerControl player, bool ejection)
    {
        SetGhosties<Revealer>(player, ejection, WillBeRevealers, Revealers, player.Is<Crew>);
        SetGhosties<Phantom>(player, ejection, WillBePhantoms, Phantoms, () => player.Is<Outcast>() && !IsExcludedOutcast(player));
        SetGhosties<Banshee>(player, ejection, WillBeBanshees, Banshees, player.Is<Syndicate>);
        SetGhosties<Ghoul>(player, ejection, WillBeGhouls, Ghouls, player.Is<Intruder>);
    }

    private static void TryAddPostmortal(PlayerControl dead, HashSet<byte> set, byte count, Func<bool> condition)
    {
        if (dead.HasDied() && dead && !set.Contains(dead.PlayerId) && set.Count < count && condition())
            set.Add(dead.PlayerId);
    }

    private static void SetGhosties<T>(PlayerControl dead, bool ejection, HashSet<byte> set, byte count, Func<bool> condition) where T : Role, IGhosty
    {
        if (count == 0)
            return;

        TryAddPostmortal(dead, set, count, condition);
        var remove = new List<byte>();

        foreach (var id in set)
        {
            var dead2 = PlayerById(id);

            if (!dead2.HasDied() || !dead2)
            {
                remove.Add(id);
                continue;
            }

            if (!ejection)
                continue;

            if (!dead2.Is<T>(out var ghosty))
            {
                ghosty = Activator.CreateInstance<T>();
                var former = dead2.GetRole();
                ghosty.RoleUpdate(former);
                ghosty.OnStart();
            }

            dead2.GetComponent<PassiveButton>().OverrideOnClickListeners(dead2.OnClick);

            if (dead2.AmOwner && !ghosty.Caught)
                SetStartingPos(dead2);
        }

        set.RemoveAll(remove.Contains);
    }

    public static void RemoveFromPostmortals(PlayerControl player)
    {
        WillBeRevealers.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBePhantoms.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBeBanshees.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
        WillBeGhouls.RemoveAll(x => x == player.PlayerId || !PlayerById(x));
    }
}