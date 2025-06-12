// ReSharper disable InconsistentNaming

namespace TownOfUsReworked.Managers;

public static class RoleGenManager
{
    public static readonly List<RoleOptionData> CrewKillingRoles = [];
    public static readonly List<RoleOptionData> CrewSupportRoles = [];
    public static readonly List<RoleOptionData> CrewSovereignRoles = [];
    public static readonly List<RoleOptionData> CrewProtectiveRoles = [];
    public static readonly List<RoleOptionData> CrewInvestigativeRoles = [];
    public static readonly List<RoleOptionData> CrewRoles = [];

    public static readonly List<RoleOptionData> OutcastEvilRoles = [];
    public static readonly List<RoleOptionData> OutcastBenignRoles = [];
    public static readonly List<RoleOptionData> OutcastKillingRoles = [];
    public static readonly List<RoleOptionData> OutcastNeophyteRoles = [];
    public static readonly List<RoleOptionData> OutcastRoles = [];

    public static readonly List<RoleOptionData> IntruderHeadRoles = [];
    public static readonly List<RoleOptionData> IntruderKillingRoles = [];
    public static readonly List<RoleOptionData> IntruderSupportRoles = [];
    public static readonly List<RoleOptionData> IntruderDeceptionRoles = [];
    public static readonly List<RoleOptionData> IntruderConcealingRoles = [];
    public static readonly List<RoleOptionData> IntruderRoles = [];

    public static readonly List<RoleOptionData> SyndicateHeadRoles = [];
    public static readonly List<RoleOptionData> SyndicateSupportRoles = [];
    public static readonly List<RoleOptionData> SyndicateKillingRoles = [];
    public static readonly List<RoleOptionData> SyndicateDisruptionRoles = [];
    public static readonly List<RoleOptionData> SyndicateRoles = [];

    public static readonly List<RoleOptionData> ApocalypseHarbingerRoles = [];
    public static readonly List<RoleOptionData> ApocalypseRoles = [];

    public static readonly List<RoleOptionData> PandoricaRoles = [];
    public static readonly List<RoleOptionData> ComplianceRoles = [];
    public static readonly List<RoleOptionData> IlluminatiRoles = [];

    public static readonly List<RoleOptionData> AllModifiers = [];
    public static readonly List<RoleOptionData> AllAbilities = [];
    public static readonly List<RoleOptionData> AllDispositions = [];
    public static readonly List<RoleOptionData> AllRoles = [];

    public static PlayerControl Pure;
    public static byte Convertible;

    public static readonly LayerEnum[] CI = [ LayerEnum.Mystic, LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer, LayerEnum.Detective ];
    public static readonly LayerEnum[] CSv = [ LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch, LayerEnum.Democrat ];
    public static readonly LayerEnum[] CrP = [ LayerEnum.Altruist, LayerEnum.Medic, LayerEnum.Trapper ];
    public static readonly LayerEnum[] CU = [ LayerEnum.Crewmate ];
    public static readonly LayerEnum[] CK = [ LayerEnum.Vigilante, LayerEnum.Veteran, LayerEnum.Bastion ];
    public static readonly LayerEnum[] CS = [ LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Chameleon, LayerEnum.Retributionist ];
    public static readonly LayerEnum[] Crew = [ .. CI, .. CSv, .. CrP, .. CK, .. CS, .. CU ];
    public static readonly LayerEnum[] RegCrew = [ .. CI, .. CrP, .. CS, .. CU ];
    public static readonly LayerEnum[] PowerCrew = [ .. CK, .. CSv ];

    public static readonly LayerEnum[] NB = [ LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief ];
    public static readonly LayerEnum[] NE = [ LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Troll, LayerEnum.Shifter ];
    public static readonly LayerEnum[] NN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer, LayerEnum.Zealot ];
    public static readonly LayerEnum[] NK = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf ];
    public static readonly LayerEnum[] Outcast = [ .. NB, .. NE, .. NN, .. NK ];
    public static readonly LayerEnum[] RegOutcast = [ .. NB, .. NE ];
    public static readonly LayerEnum[] HarmOutcast = [ .. NN, .. NK ];

    public static readonly LayerEnum[] AH = [ LayerEnum.Plaguebearer, LayerEnum.Cannibal, LayerEnum.Cultist ];
    public static readonly LayerEnum[] AD = [ LayerEnum.Pestilence, LayerEnum.Void, LayerEnum.Gluttony ];
    public static readonly LayerEnum[] Apocalypse = [ .. AH ];

    public static readonly LayerEnum[] IC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    public static readonly LayerEnum[] ID = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    public static readonly LayerEnum[] IK = [ LayerEnum.Enforcer, LayerEnum.Ambusher ];
    public static readonly LayerEnum[] IS = [ LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    public static readonly LayerEnum[] IH = [ LayerEnum.Godfather ];
    public static readonly LayerEnum[] IU = [ LayerEnum.Impostor ];
    public static readonly LayerEnum[] Intruders = [ .. IC, .. ID, .. IK, .. IS, .. IU, .. IH ];
    public static readonly LayerEnum[] RegIntruders = [ .. IC, .. ID, .. IS, .. IU ];
    public static readonly LayerEnum[] PowerIntruders = [ .. IK, .. IH ];

    public static readonly LayerEnum[] SSu = [ LayerEnum.Warper, LayerEnum.Stalker ];
    public static readonly LayerEnum[] SD = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer] ;
    public static readonly LayerEnum[] SH = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    public static readonly LayerEnum[] SyK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner ];
    public static readonly LayerEnum[] SU = [ LayerEnum.Anarchist ];
    public static readonly LayerEnum[] Syndicate = [ .. SSu, .. SyK, .. SD, .. SH, .. SU ];
    public static readonly LayerEnum[] RegSyndicate = [ .. SSu, .. SD, .. SU ];
    public static readonly LayerEnum[] PowerSyndicate = [ .. SyK, .. SH ];

    public static readonly LayerEnum[] NonCrew = [ .. Outcast, .. Intruders, .. Syndicate, .. Apocalypse ];
    public static readonly LayerEnum[] NonOutcast = [ .. Crew, .. Intruders, .. Syndicate, .. Apocalypse ];
    public static readonly LayerEnum[] NonIntruders = [ .. Outcast, .. Crew, .. Syndicate, .. Apocalypse ];
    public static readonly LayerEnum[] NonSyndicate = [ .. Outcast, .. Intruders, .. Crew, .. Apocalypse ];
    public static readonly LayerEnum[] NonApocalypse = [ .. Outcast, .. Intruders, .. Crew, .. Syndicate ];

    public static readonly LayerEnum[] Alignments = [ .. CI, .. CSv, .. CrP, .. CU, .. CK, .. CS, .. NB, .. NE, .. NN, .. NK, .. IC, .. ID, .. IS, .. SSu, .. SD, .. SH, .. SyK, .. IK, .. IH, .. IU,
        .. SU, .. AH ];

    private static readonly byte[] Spawns = [ 0, 1, 2, 3, 4, 5, 6, 7, 8 ];

    private static readonly TargetGen Targets = new();
    private static readonly ModifierGen Modifiers = new();
    private static readonly AbilityGen Abilities = new();
    private static readonly DispositionGen Dispositions = new();
    private static readonly Dictionary<Mode, BaseRoleGen> RoleGen = new()
    {
        { Mode.HideAndSeek, new HideAndSeekGen() },
        { Mode.Classic, new ClassicGen() },
        { Mode.List, new ListGen() },
        { Mode.Vanilla, new VanillaGen() },
        { Mode.AllAny, new AllAnyGen() },
        { Mode.TaskRace, new TaskRaceGen() }
    };

    public static readonly Dictionary<Mode, BaseFilter> ModeFilters = new()
    {
        { Mode.Classic, new CommonFilter() },
        { Mode.AllAny, new AllAnyFilter() }
    };

    public static LayerEnum[] NonPandorica()
    {
        var type = BadGuysSettings.PandoricaMembers;
        var result = new List<LayerEnum>([ .. HarmOutcast, .. RegOutcast, .. Crew ]);

        if (type != PandoricaType.Intruders)
            result.AddRange(Intruders);

        if (type != PandoricaType.Syndicate)
            result.AddRange(Syndicate);

        if (type != PandoricaType.Apocalypse)
            result.AddRange(Apocalypse);

        return [.. result];
    }

    public static LayerEnum[] NonCompliance()
    {
        var type = BadGuysSettings.ComplianceMembers;
        var result = new List<LayerEnum>([ .. RegOutcast, .. Crew, .. Intruders, .. Syndicate, .. Apocalypse ]);

        if (type != ComplianceType.Killers)
            result.AddRange(NK);

        if (type != ComplianceType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static LayerEnum[] NonCompOutcast()
    {
        var type = BadGuysSettings.ComplianceMembers;
        var result = new List<LayerEnum>([ .. RegOutcast]);

        if (type != ComplianceType.Killers)
            result.AddRange(NK);

        if (type != ComplianceType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static LayerEnum[] NonIllOutcast()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<LayerEnum>([ .. RegOutcast]);

        if (type != IlluminatiType.Killers)
            result.AddRange(NK);

        if (type != IlluminatiType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static LayerEnum[] Compliance()
    {
        var type = BadGuysSettings.ComplianceMembers;
        return [.. type == ComplianceType.Killers ? NK : [], .. type == ComplianceType.Neophytes ? NN : []];
    }

    public static LayerEnum[] NonIlluminati()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<LayerEnum>([ .. RegOutcast, .. Crew ]);

        if (type != IlluminatiType.Intruders)
            result.AddRange(Intruders);

        if (type != IlluminatiType.Syndicate)
            result.AddRange(Syndicate);

        if (type != IlluminatiType.Apocalypse)
            result.AddRange(Apocalypse);

        if (type != IlluminatiType.Killers)
            result.AddRange(NK);

        if (type != IlluminatiType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static LayerEnum[] PS()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IS : [], .. type == PandoricaType.Syndicate ? SSu : []];
    }

    public static LayerEnum[] PU()
    {
        var type = BadGuysSettings.PandoricaMembers;
        var result = new List<LayerEnum>();

        if (type == PandoricaType.Intruders)
            result.Add(LayerEnum.Impostor);

        if (type == PandoricaType.Syndicate)
            result.Add(LayerEnum.Anarchist);

        return [.. result];
    }

    public static LayerEnum[] PDi() => BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate ? SD : [];

    public static LayerEnum[] PHe()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IH : [], .. type == PandoricaType.Syndicate ? SH : []];
    }

    public static LayerEnum[] PC() => BadGuysSettings.PandoricaMembers == PandoricaType.Intruders ? IC : [];

    public static LayerEnum[] PDe() => BadGuysSettings.PandoricaMembers == PandoricaType.Intruders ? ID : [];

    public static LayerEnum[] PHa() => BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse ? AH : [];

    public static LayerEnum[] PK()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IK : [], .. type == PandoricaType.Syndicate ? SyK : []];
    }

    public static LayerEnum[] RegPandorica() => [ .. PC(), .. PDe(), .. PDi(), .. PS(), .. PU() ];

    public static LayerEnum[] PowerPandorica() => [ .. PK(), .. PHa(), .. PHe() ];

    public static LayerEnum[] Pandorica() => [ .. PS(), .. PDi(), .. PHa(), .. PK(), .. PC(), .. PDe(), .. PU(), .. PHe() ];

    public static LayerEnum[] IlS()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IS : [], .. type == IlluminatiType.Syndicate ? SSu : []];
    }

    public static LayerEnum[] IlU()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<LayerEnum>();

        if (type == IlluminatiType.Intruders)
            result.Add(LayerEnum.Impostor);

        if (type == IlluminatiType.Syndicate)
            result.Add(LayerEnum.Anarchist);

        return [.. result];
    }

    public static LayerEnum[] IlDi() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate ? SD : [];

    public static LayerEnum[] IlHe()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IS : [], .. type == IlluminatiType.Syndicate ? SSu : []];
    }

    public static LayerEnum[] IlC() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders ? IC : [];

    public static LayerEnum[] IlDe() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders ? ID : [];

    public static LayerEnum[] IN() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes ? NN : [];

    public static LayerEnum[] IlHa() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse ? AH : [];

    public static LayerEnum[] IlK()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IK : [], .. type == IlluminatiType.Syndicate ? SyK : [], .. type == IlluminatiType.Killers ? NK : []];
    }

    public static LayerEnum[] RegIlluminati() => [ .. IlC(), .. IlDe(), .. IlDi(), .. IlS(), .. IlU() ];

    public static LayerEnum[] PowerIlluminati() => [ .. IlK(), .. IlHe(), .. IlHa(), .. IN() ];

    public static LayerEnum[] Illuminati() => [ .. IlS(), .. IlDi(), .. IlHe(), .. IlK(), .. IlC(), .. IlDe(), .. IlU(), .. IN(), .. IlHa() ];

    public static RoleOptionData GetSpawnItem(LayerEnum id) => id switch
    {
        LayerEnum.Pestilence => Options.ApocalypseHarbingerRoles.Plaguebearer,
        LayerEnum.Void => Options.ApocalypseHarbingerRoles.Cultist,
        LayerEnum.Gluttony => Options.ApocalypseHarbingerRoles.Cannibal,
        LayerEnum.Mayor => Options.CrewSovereignRoles.Democrat,
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted => new(100, GameOptions.LobbySize.Value, false, false, id),
        _ => Option.GetOptions<LayerOption>().TryFinding(x => x.Layer == id, out var result) ? result.Value : new(0, 0, false, false, id)
    };

    public static bool IsValid(this LayerEnum layer, int? relatedCount = null) => layer switch
    {
        LayerEnum.Bastion => VentingOptions.WhoCanVent != WhoCanVentOptions.NoOne,
        LayerEnum.Mystic => new[] { LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Jackal, LayerEnum.Whisperer }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Seer => new[] { LayerEnum.BountyHunter, LayerEnum.Godfather, LayerEnum.Rebel, LayerEnum.Plaguebearer, LayerEnum.Mystic, LayerEnum.Traitor, LayerEnum.Amnesiac, LayerEnum.Thief,
            LayerEnum.Executioner, LayerEnum.GuardianAngel, LayerEnum.Guesser, LayerEnum.Fanatic }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief => !OutcastSettings.AvoidOutcastKingmakers,
        LayerEnum.Jackal => GameData.Instance.PlayerCount > 6,
        LayerEnum.Actor => new[] { LayerEnum.Bullseye, LayerEnum.Slayer, LayerEnum.Sniper, LayerEnum.Hitman }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Miner => VentingOptions.WhoCanVent != WhoCanVentOptions.NoOne && (Miner.MinerSpawnOnMira || MapPatches.CurrentMap != 2),
        LayerEnum.Godfather or LayerEnum.Rebel => relatedCount >= 3 || TownOfUsReworked.MciActive,
        LayerEnum.Insider => VotingOptions.AnonymousVoting != AnonVotes.Disabled,
        LayerEnum.Tunneler => VentingOptions.WhoCanVent == WhoCanVentOptions.Default && CrewSettings.CrewVent == CrewVenting.Never,
        LayerEnum.Lovers => GameData.Instance.PlayerCount > 4,
        LayerEnum.Rivals => GameData.Instance.PlayerCount > 3,
        LayerEnum.Linked => Role.GetRoles(Faction.Outcast).Count() > 1 && GameData.Instance.PlayerCount > 4,
        LayerEnum.Democrat => !Mayor.MayorDirectSpawn,
        LayerEnum.Mayor => Mayor.MayorDirectSpawn,
        LayerEnum.Allied => !BadGuysSettings.IlluminatiUnleashed && !BadGuysSettings.OrderOfCompliance,
        _ when AH.Contains(layer) => !ApocalypseSettings.DirectSpawn,
        _ when AD.Contains(layer) => ApocalypseSettings.DirectSpawn,
        _ => true
    };

    public static void Gen(PlayerControl player, LayerEnum id, PlayerLayerEnum rpc)
    {
        player.GetLayers().Find(x => x.LayerType == rpc)?.End();
        GetLayer(id, rpc).Start(player);

        if (!TownOfUsReworked.MciActive)
            CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, rpc, player);
    }

    public static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, LayerEnum.None, rpc);

    public static PlayerLayer GetLayer(LayerEnum id, PlayerLayerEnum rpc)
    {
        if (LayerDictionary.TryGetValue(id, out var dictEntry))
            return (PlayerLayer)Activator.CreateInstance(dictEntry.LayerType);

        return rpc switch
        {
            PlayerLayerEnum.Role => new Roleless(),
            PlayerLayerEnum.Modifier => new Modifierless(),
            PlayerLayerEnum.Disposition => new Dispositionless(),
            PlayerLayerEnum.Ability => new Abilityless(),
            _ => throw new ArgumentOutOfRangeException($"{id}:{rpc}")
        };
    }

    public static void BeginRoleGen()
    {
        if (IsHnS() || !AmongUsClient.Instance.AmHost)
            return;

        Message("Role Gen Start");
        Message($"Current Mode = {GameModeSettings.GameMode}");
        ResetEverything();
        CallRpc(CustomRPC.Misc, MiscRPC.Start);
        Message("Cleared Variables");
        var gen = RoleGen[GameModeSettings.GameMode];

        gen.InitList();
        gen.InitSynList();
        gen.InitIntList();
        gen.InitNeutList();
        gen.InitApocList();
        gen.InitCrewList();
        gen.PreFilter();
        gen.Filter();
        gen.Assign();

        var allPlayers = AllPlayers();

        if (GameModifiers.PurePlayers && allPlayers.Any(x => x.Is<Neophyte>()))
            Pure = allPlayers.Random(x => !x.Is<Neophyte>());

        if (gen.AllowNonRoles)
        {
            if (GameModifiers.EnableDispositions)
            {
                Dispositions.InitList();
                Dispositions.Assign();
            }

            if (GameModifiers.EnableAbilities)
            {
                Abilities.InitList();
                Abilities.Assign();
            }

            if (GameModifiers.EnableModifiers)
            {
                Modifiers.InitList();
                Modifiers.Assign();
            }
        }

        if (gen.HasTargets)
            Targets.Assign();

        gen.PostAssignment();

        Convertible = (byte)allPlayers.Count(x => x.GetFaction().IsConvertible() && x != Pure);

        if (MapPatches.CurrentMap == 4)
            BetterAirship.SpawnPoints.AddRange(Spawns.GetRandomRange(3));

        if (TownOfUsReworked.MciActive)
        {
            var maxName = 1;
            var maxRole = 4;
            var maxDisp = 11;
            var maxMod = 8;
            var maxAb = 7;

            foreach (var player in allPlayers)
            {
                RoleManager.Instance.SetRole(player, (RoleTypes)100);

                var role = player.GetRoleFromList();
                var mod = player.GetModifierFromList();
                var ab = player.GetAbilityFromList();
                var disp = player.GetDispositionFromList();
                var name = player.name;
                var roleStr = role.ToString();
                var dispStr = disp.ToString();
                var modStr = mod.ToString();
                var abStr = ab.ToString();

                if (name.Length > maxName)
                    maxName = name.Length;

                if (roleStr.Length > maxRole)
                    maxRole = roleStr.Length;

                if (dispStr.Length > maxDisp)
                    maxDisp = dispStr.Length;

                if (modStr.Length > maxMod)
                    maxMod = modStr.Length;

                if (abStr.Length > maxAb)
                    maxAb = abStr.Length;
            }

            Message($"| {"Name".PadCenter(maxName)} | {"Role".PadCenter(maxRole)} | {"Disposition".PadCenter(maxDisp)} | {"Modifier".PadCenter(maxMod)} | {"Ability".PadCenter(maxAb)} |");
            Message($"| {new('-', maxName)} | {new('-', maxRole)} | {new('-', maxDisp)} | {new('-', maxMod)} | {new('-', maxAb)} |");

            foreach (var player in allPlayers)
            {
                var role = player.GetRole();
                var mod = player.GetModifier();
                var ab = player.GetAbility();
                var disp = player.GetDisposition();

                var name = player.name;
                var roleStr = role.ToString();
                var dispStr = disp.ToString();
                var modStr = mod.ToString();
                var abStr = ab.ToString();

                Message($"| {name.PadCenter(maxName)} | {roleStr.PadCenter(maxRole)} | {dispStr.PadCenter(maxDisp)} | {modStr.PadCenter(maxMod)} | {abStr.PadCenter(maxAb)} |");
            }
        }
        else
        {
            allPlayers.Do(x => RoleManager.Instance.SetRole(x, (RoleTypes)100));
            CallRpc(CustomRPC.Misc, MiscRPC.EndRoleGen, SetPostmortals.Revealers, SetPostmortals.Phantoms, SetPostmortals.Banshees, SetPostmortals.Ghouls, Pure?.PlayerId ?? 255, Convertible,
                BetterAirship.SpawnPoints);
        }

        if (SyndicateSettings.AssignOnGameStart)
            AssignChaosDrive();

        ClearGens();
        Success("Gen Ended");
    }

    public static bool Check(int chance) => chance switch
    {
        0 => false,
        100 => true,
        _ => URandom.RandomRangeInt(1, 100) <= chance
    };

    private static void Clear()
    {
        WinState = WinLose.None;

        CameraEffectHandler.Initialize();
        CameraEffectHandler.ClearEffects();

        MeetingPatches.MeetingCount = 0;

        PlayerLayers.Roles.Syndicate.SyndicateHasChaosDrive = false;
        PlayerLayers.Roles.Syndicate.DriveHolder = null;

        Cleaned.Clear();

        MeetingPatches.MeetingCount = 0;

        KilledPlayers.Clear();

        DeadBodyHandler.Dragging.Clear();

        Monos.Range.AllItems.Clear();

        PlayerLayer.AllLayers.Clear();

        SetPostmortals.Phantoms = 0;
        SetPostmortals.Revealers = 0;
        SetPostmortals.Banshees = 0;
        SetPostmortals.Ghouls = 0;

        SetPostmortals.WillBeBanshees.Clear();
        SetPostmortals.WillBeGhouls.Clear();
        SetPostmortals.WillBeRevealers.Clear();
        SetPostmortals.WillBePhantoms.Clear();

        ChatPatches.ChatHistory.Clear();

        Pure = null;
        Convertible = 0;

        RecentlyKilled.Clear();

        Assassin.RemainingKills = Assassin.AssassinKills == 0 ? 10000 : Assassin.AssassinKills;

        CachedFirstDead = FirstDead;
        FirstDead = null;

        BlockExposed = false;

        // Role.IsLeft = false;

        CustomMeeting.AllCustomMeetings.Clear();
        CustomArrow.AllArrows.Clear();
        CustomMenu.AllMenus.Clear();
        CustomButton.AllButtons.Clear();

        Ash.AllPiles.ForEach(x => x?.gameObject?.Destroy());
        Ash.AllPiles.Clear();

        Client.Instance.CloseMenus();

        BodyLocations.Clear();

        UninteractablePlayers.Clear();

        BetterAirship.SpawnPoints.Clear();

        // BaseStatus.AllStatuses.Clear();
    }

    public static void ResetEverything()
    {
        Clear();
        ClearGens();
    }

    private static void ClearGens()
    {
        Modifiers.Clear();
        Abilities.Clear();
        Dispositions.Clear();
        Targets.Clear();
        RoleGen.Values.Do(x => x.Clear());

        AllModifiers.Clear();
        AllDispositions.Clear();
        AllAbilities.Clear();
        AllRoles.Clear();

        CrewInvestigativeRoles.Clear();
        CrewSupportRoles.Clear();
        CrewProtectiveRoles.Clear();
        CrewSovereignRoles.Clear();
        CrewKillingRoles.Clear();

        IntruderConcealingRoles.Clear();
        IntruderDeceptionRoles.Clear();
        IntruderKillingRoles.Clear();
        IntruderSupportRoles.Clear();
        IntruderHeadRoles.Clear();

        SyndicateSupportRoles.Clear();
        SyndicateKillingRoles.Clear();
        SyndicateHeadRoles.Clear();
        SyndicateDisruptionRoles.Clear();

        OutcastBenignRoles.Clear();
        OutcastEvilRoles.Clear();
        OutcastKillingRoles.Clear();
        OutcastNeophyteRoles.Clear();

        ApocalypseHarbingerRoles.Clear();

        SyndicateRoles.Clear();
        CrewRoles.Clear();
        IntruderRoles.Clear();
        ApocalypseRoles.Clear();
        ComplianceRoles.Clear();
        PandoricaRoles.Clear();
        IlluminatiRoles.Clear();
    }
}