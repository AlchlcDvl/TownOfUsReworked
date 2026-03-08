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

    public static PlayerControl? Pure;
    public static byte Convertible;

    public static readonly Layer[] CI = [ Layer.Mystic, Layer.Sheriff, Layer.Tracker, Layer.Medium, Layer.Coroner, Layer.Operative, Layer.Seer, Layer.Detective ];
    public static readonly Layer[] CSv = [ Layer.Mayor, Layer.Dictator, Layer.Monarch, Layer.Democrat ];
    public static readonly Layer[] CrP = [ Layer.Altruist, Layer.Medic, Layer.Trapper ];
    public static readonly Layer[] CU = [ Layer.Crewmate ];
    public static readonly Layer[] CK = [ Layer.Vigilante, Layer.Veteran, Layer.Bastion ];
    public static readonly Layer[] CS = [ Layer.Engineer, Layer.Transporter, Layer.Escort, Layer.Chameleon, Layer.Retributionist ];
    public static readonly Layer[] Crew = [ .. CI, .. CSv, .. CrP, .. CK, .. CS, .. CU ];
    public static readonly Layer[] RegCrew = [ .. CI, .. CrP, .. CS, .. CU ];
    public static readonly Layer[] PowerCrew = [ .. CK, .. CSv ];

    public static readonly Layer[] NB = [ Layer.Amnesiac, Layer.GuardianAngel, Layer.Survivor, Layer.Thief ];
    public static readonly Layer[] NE = [ Layer.Jester, Layer.Actor, Layer.BountyHunter, Layer.Executioner, Layer.Guesser, Layer.Troll, Layer.Shifter ];
    public static readonly Layer[] NN = [ Layer.Jackal, Layer.Necromancer, Layer.Dracula, Layer.Whisperer, Layer.Zealot ];
    public static readonly Layer[] NK = [ Layer.Arsonist, Layer.Cryomaniac, Layer.Glitch, Layer.Juggernaut, Layer.Murderer, Layer.SerialKiller, Layer.Werewolf ];
    public static readonly Layer[] Outcast = [ .. NB, .. NE, .. NN, .. NK ];
    public static readonly Layer[] RegOutcast = [ .. NB, .. NE ];
    public static readonly Layer[] HarmOutcast = [ .. NN, .. NK ];

    public static readonly Layer[] AH = [ Layer.Plaguebearer, Layer.Cannibal, Layer.Cultist ];
    public static readonly Layer[] AD = [ Layer.Pestilence, Layer.Void, Layer.Gluttony ];
    private static readonly Layer[] Apocalypse = [ .. AH ];

    public static readonly Layer[] IC = [ Layer.Blackmailer, Layer.Camouflager, Layer.Grenadier, Layer.Janitor ];
    public static readonly Layer[] ID = [ Layer.Morphling, Layer.Disguiser, Layer.Wraith ];
    public static readonly Layer[] IK = [ Layer.Enforcer, Layer.Ambusher ];
    public static readonly Layer[] IS = [ Layer.Consigliere, Layer.Miner, Layer.Teleporter, Layer.Consort ];
    public static readonly Layer[] IH = [ Layer.Godfather ];
    public static readonly Layer[] IU = [ Layer.Impostor ];
    public static readonly Layer[] Intruders = [ .. IC, .. ID, .. IK, .. IS, .. IU, .. IH ];
    public static readonly Layer[] RegIntruders = [ .. IC, .. ID, .. IS, .. IU ];
    public static readonly Layer[] PowerIntruders = [ .. IK, .. IH ];

    public static readonly Layer[] SSu = [ Layer.Warper, Layer.Stalker ];
    public static readonly Layer[] SD = [ Layer.Timekeeper, Layer.Concealer, Layer.Drunkard, Layer.Framer, Layer.Shapeshifter, Layer.Silencer] ;
    public static readonly Layer[] SH = [ Layer.Rebel, Layer.Spellslinger ];
    public static readonly Layer[] SyK = [ Layer.Bomber, Layer.Collider, Layer.Crusader, Layer.Poisoner ];
    public static readonly Layer[] SU = [ Layer.Anarchist ];
    public static readonly Layer[] Syndicate = [ .. SSu, .. SyK, .. SD, .. SH, .. SU ];
    public static readonly Layer[] RegSyndicate = [ .. SSu, .. SD, .. SU ];
    public static readonly Layer[] PowerSyndicate = [ .. SyK, .. SH ];

    public static readonly Layer[] NonCrew = [ .. Outcast, .. Intruders, .. Syndicate, .. Apocalypse ];
    public static readonly Layer[] NonOutcast = [ .. Crew, .. Intruders, .. Syndicate, .. Apocalypse ];
    public static readonly Layer[] NonIntruders = [ .. Outcast, .. Crew, .. Syndicate, .. Apocalypse ];
    public static readonly Layer[] NonSyndicate = [ .. Outcast, .. Intruders, .. Crew, .. Apocalypse ];
    public static readonly Layer[] NonApocalypse = [ .. Outcast, .. Intruders, .. Crew, .. Syndicate ];

    public static readonly Layer[] Alignments = [ .. CI, .. CSv, .. CrP, .. CU, .. CK, .. CS, .. NB, .. NE, .. NN, .. NK, .. IC, .. ID, .. IS, .. SSu, .. SD, .. SH, .. SyK, .. IK, .. IH, .. IU,
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

    public static Layer[] NonPandorica()
    {
        var type = BadGuysSettings.PandoricaMembers;
        var result = new List<Layer>([ .. HarmOutcast, .. RegOutcast, .. Crew ]);

        if (type != PandoricaType.Intruders)
            result.AddRange(Intruders);

        if (type != PandoricaType.Syndicate)
            result.AddRange(Syndicate);

        if (type != PandoricaType.Apocalypse)
            result.AddRange(Apocalypse);

        return [.. result];
    }

    public static Layer[] NonCompliance()
    {
        var type = BadGuysSettings.ComplianceMembers;
        var result = new List<Layer>([ .. RegOutcast, .. Crew, .. Intruders, .. Syndicate, .. Apocalypse ]);

        if (type != ComplianceType.Killers)
            result.AddRange(NK);

        if (type != ComplianceType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static Layer[] NonCompOutcast()
    {
        var type = BadGuysSettings.ComplianceMembers;
        var result = new List<Layer>([ .. RegOutcast]);

        if (type != ComplianceType.Killers)
            result.AddRange(NK);

        if (type != ComplianceType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static Layer[] NonIllOutcast()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<Layer>([ .. RegOutcast]);

        if (type != IlluminatiType.Killers)
            result.AddRange(NK);

        if (type != IlluminatiType.Neophytes)
            result.AddRange(NN);

        return [.. result];
    }

    public static Layer[] Compliance()
    {
        var type = BadGuysSettings.ComplianceMembers;
        return [.. type == ComplianceType.Killers ? NK : [], .. type == ComplianceType.Neophytes ? NN : []];
    }

    public static Layer[] NonIlluminati()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<Layer>([ .. RegOutcast, .. Crew ]);

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

    public static Layer[] PS()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IS : [], .. type == PandoricaType.Syndicate ? SSu : []];
    }

    public static Layer[] PU()
    {
        var type = BadGuysSettings.PandoricaMembers;
        var result = new List<Layer>();

        if (type == PandoricaType.Intruders)
            result.Add(Layer.Impostor);

        if (type == PandoricaType.Syndicate)
            result.Add(Layer.Anarchist);

        return [.. result];
    }

    public static Layer[] PDi() => BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate ? SD : [];

    public static Layer[] PHe()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IH : [], .. type == PandoricaType.Syndicate ? SH : []];
    }

    public static Layer[] PC() => BadGuysSettings.PandoricaMembers == PandoricaType.Intruders ? IC : [];

    public static Layer[] PDe() => BadGuysSettings.PandoricaMembers == PandoricaType.Intruders ? ID : [];

    public static Layer[] PHa() => BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse ? AH : [];

    public static Layer[] PK()
    {
        var type = BadGuysSettings.PandoricaMembers;
        return [.. type == PandoricaType.Intruders ? IK : [], .. type == PandoricaType.Syndicate ? SyK : []];
    }

    public static Layer[] RegPandorica() => [ .. PC(), .. PDe(), .. PDi(), .. PS(), .. PU() ];

    public static Layer[] PowerPandorica() => [ .. PK(), .. PHa(), .. PHe() ];

    public static Layer[] Pandorica() => [ .. PS(), .. PDi(), .. PHa(), .. PK(), .. PC(), .. PDe(), .. PU(), .. PHe() ];

    public static Layer[] IlS()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IS : [], .. type == IlluminatiType.Syndicate ? SSu : []];
    }

    public static Layer[] IlU()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        var result = new List<Layer>();

        if (type == IlluminatiType.Intruders)
            result.Add(Layer.Impostor);

        if (type == IlluminatiType.Syndicate)
            result.Add(Layer.Anarchist);

        return [.. result];
    }

    public static Layer[] IlDi() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate ? SD : [];

    public static Layer[] IlHe()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IS : [], .. type == IlluminatiType.Syndicate ? SSu : []];
    }

    public static Layer[] IlC() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders ? IC : [];

    public static Layer[] IlDe() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders ? ID : [];

    public static Layer[] IN() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes ? NN : [];

    public static Layer[] IlHa() => BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse ? AH : [];

    public static Layer[] IlK()
    {
        var type = BadGuysSettings.IlluminatiMembers;
        return [.. type == IlluminatiType.Intruders ? IK : [], .. type == IlluminatiType.Syndicate ? SyK : [], .. type == IlluminatiType.Killers ? NK : []];
    }

    public static Layer[] RegIlluminati() => [ .. IlC(), .. IlDe(), .. IlDi(), .. IlS(), .. IlU() ];

    public static Layer[] PowerIlluminati() => [ .. IlK(), .. IlHe(), .. IlHa(), .. IN() ];

    public static Layer[] Illuminati() => [ .. IlS(), .. IlDi(), .. IlHe(), .. IlK(), .. IlC(), .. IlDe(), .. IlU(), .. IN(), .. IlHa() ];

    public static RoleOptionData GetSpawnItem(Layer id) => id switch
    {
        Layer.Pestilence => Options.ApocalypseHarbingerRoles.Plaguebearer,
        Layer.Void => Options.ApocalypseHarbingerRoles.Cultist,
        Layer.Gluttony => Options.ApocalypseHarbingerRoles.Cannibal,
        Layer.Mayor => Options.CrewSovereignRoles.Democrat,
        Layer.Runner or Layer.Hunter or Layer.Hunted => new(100, GameOptions.LobbySize.Value, false, false, id),
        _ => Option.GetOptions<LayerOption>().TryFinding(x => x.Layer == id, out var result) ? result.Value : new(0, 0, false, false, id)
    };

    public static bool IsValid(this Layer layer, int? relatedCount = null, bool forSettings = false) => layer switch
    {
        Layer.Bastion => VentingOptions.WhoCanVent != WhoCanVentOptions.NoOne,
        Layer.Mystic => new[] { Layer.Necromancer, Layer.Dracula, Layer.Jackal, Layer.Whisperer, Layer.Zealot }.Any(x => GetSpawnItem(x).IsActive()),
        Layer.Seer => new[] { Layer.BountyHunter, Layer.Godfather, Layer.Rebel, Layer.Plaguebearer, Layer.Mystic, Layer.Traitor, Layer.Amnesiac, Layer.Thief,
            Layer.Executioner, Layer.GuardianAngel, Layer.Guesser, Layer.Fanatic }.Any(x => GetSpawnItem(x).IsActive()),
        Layer.Amnesiac or Layer.GuardianAngel or Layer.Survivor or Layer.Thief => !OutcastSettings.AvoidOutcastKingmakers,
        Layer.Jackal => GameData.Instance.PlayerCount > 6,
        Layer.Actor => new[] { Layer.Bullseye, Layer.Slayer, Layer.Sniper, Layer.Hitman, Layer.Ranger, Layer.Marksman, Layer.Deadshot }.Any(x => GetSpawnItem(x).IsActive()),
        Layer.Miner => VentingOptions.WhoCanVent != WhoCanVentOptions.NoOne && (Miner.MinerSpawnOnMira || (forSettings ? MapSettings.Map != Map.MiraHq : MapPatches.CurrentMap != 2)),
        Layer.Godfather or Layer.Rebel => relatedCount is null or >= 3 || TownOfUsReworked.MciActive || forSettings,
        Layer.Insider => VotingOptions.AnonymousVoting != AnonVotes.Disabled,
        Layer.Tunneler => VentingOptions.WhoCanVent == WhoCanVentOptions.Default && CrewSettings.CrewVent == CrewVenting.Never,
        Layer.Lovers => GameData.Instance.PlayerCount > 4,
        Layer.Rivals => GameData.Instance.PlayerCount > 3,
        Layer.Linked => (forSettings || Role.GetBaseFactionRoles(Faction.Outcast).Count() > 1) && GameData.Instance.PlayerCount > 4,
        Layer.Democrat => !Mayor.MayorDirectSpawn || forSettings,
        Layer.Mayor => Mayor.MayorDirectSpawn || forSettings,
        Layer.Deadshot => BadGuysSettings.IlluminatiUnleashed,
        Layer.Marksman => !BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.OrderOfCompliance,
        Layer.Ranger => !BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.PandoricaOpens,
        Layer.Sniper or Layer.Hitman => !BadGuysSettings.IlluminatiUnleashed && !BadGuysSettings.PandoricaOpens,
        Layer.Slayer => !BadGuysSettings.IlluminatiUnleashed && !BadGuysSettings.OrderOfCompliance,
        Layer.Allied => (!BadGuysSettings.IlluminatiUnleashed && !BadGuysSettings.OrderOfCompliance) || (BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers !=
            IlluminatiType.Killers) || (!BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers != ComplianceType.Killers),
        Layer.Runner => GameModeSettings.GameMode == Mode.TaskRace,
        Layer.Hunter or Layer.Hunted => GameModeSettings.GameMode == Mode.HideAndSeek,
        _ when AH.Contains(layer) => !ApocalypseSettings.DirectSpawn || forSettings,
        _ when AD.Contains(layer) => ApocalypseSettings.DirectSpawn || forSettings,
        _ => true
    };

    public static void Gen(PlayerControl player, Layer id, PlayerLayerEnum rpc)
    {
        player.GetLayers().Find(x => x.LayerType == rpc)?.End();
        GetLayer(id, rpc).Start(player);

        if (!TownOfUsReworked.MciActive)
            CallRpc(MiscRpc.SetLayer, id, rpc, player);
    }

    public static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, Layer.None, rpc);

    public static PlayerLayer GetLayer(Layer id, PlayerLayerEnum rpc)
    {
        if (LayerDictionary.TryGetValue(id, out var dictEntry))
            return (PlayerLayer)Activator.CreateInstance(dictEntry.LayerType!)!;

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
        CallRpc(MiscRpc.Start);
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

        Retributionist.Exists = AllRoles.Any(x => x.ID == Layer.Retributionist);

        gen.Assign();

        var allPlayers = AllPlayers();

        if (GameModifiers.PurePlayers && allPlayers.Any(x => x.Is<Neophyte>()))
            Pure = allPlayers.Random(x => !x!.Is<Neophyte>());

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

        allPlayers.Do(x => RoleManager.Instance.SetRole(x, LayerHandler.Type));

        Convertible = (byte)allPlayers.Count(x => x.GetFaction().IsConvertible() && x != Pure);

        if (MapPatches.CurrentMap == 4)
            BetterAirship.SpawnPoints.AddRange(Spawns.GetRandomRange(3));

        if (BetterSabotages.OxySlow)
            ISpeedModifier.AllModifiers.Add(new OxySabSpeedModifier());

        ISpeedModifier.AllModifiers.Add(new BodyDraggingModifier());

        if (!TownOfUsReworked.MciActive)
        {
            CallRpc(MiscRpc.EndRoleGen, [SetPostmortals.Revealers, SetPostmortals.Phantoms, SetPostmortals.Banshees, SetPostmortals.Ghouls, Pure?.PlayerId ?? 255, Convertible, Retributionist.Exists,
                ..BetterAirship.SpawnPoints]);
        }

        Shifter.Originals.AddRange(allPlayers.Where(x => x.Is<Shifter>()));

        if (SyndicateSettings.AssignOnGameStart)
            AssignChaosDrive();

        if (TownOfUsReworked.MciActive)
        {
            var maxName = 1;
            var maxRole = 4;
            var maxDisp = 11;
            var maxMod = 8;
            var maxAb = 7;

            foreach (var player in allPlayers)
            {
                var role = player.GetRoleFromList();
                var mod = player.GetModifierFromList();
                var ab = player.GetAbilityFromList();
                var disp = player.GetDispositionFromList();
                var name = player.Data.PlayerName;
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

        Monos.Range.Clear();

        ISpeedModifier.AllModifiers.Clear();

        PlayerLayer.AllLayers.FullClear();

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

        CustomMeeting.AllCustomMeetings.FullClear();
        CustomArrow.AllArrows.FullClear();
        CustomMenu.AllMenus.FullClear();
        CustomButton.AllButtons.FullClear();

        CustomButton.ButtonLookup.Clear();

        Ash.Clear();

        Client.Instance.CloseMenus();

        BodyLocations.Clear();

        UninteractablePlayers.Clear();

        BetterAirship.SpawnPoints.Clear();

        // BaseStatus.AllStatuses.Clear();

        Mafia.Mafias.Clear();

        GameStartManagerPatches.PlayersReady.Clear();

        Shifter.Shifters.Clear();
        Shifter.Originals.Clear();

        Retributionist.Exists = false;
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

        foreach (var gen in RoleGen.Values)
            gen.Clear();

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