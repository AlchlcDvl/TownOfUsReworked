namespace TownOfUsReworked.Managers;

public static class RoleGenManager
{
    public static readonly List<RoleOptionData> CrewAuditorRoles = [];
    public static readonly List<RoleOptionData> CrewKillingRoles = [];
    public static readonly List<RoleOptionData> CrewSupportRoles = [];
    public static readonly List<RoleOptionData> CrewSovereignRoles = [];
    public static readonly List<RoleOptionData> CrewProtectiveRoles = [];
    public static readonly List<RoleOptionData> CrewInvestigativeRoles = [];
    public static readonly List<RoleOptionData> CrewRoles = [];

    public static readonly List<RoleOptionData> NeutralEvilRoles = [];
    public static readonly List<RoleOptionData> NeutralBenignRoles = [];
    public static readonly List<RoleOptionData> NeutralKillingRoles = [];
    public static readonly List<RoleOptionData> NeutralNeophyteRoles = [];
    public static readonly List<RoleOptionData> NeutralHarbingerRoles = [];
    public static readonly List<RoleOptionData> NeutralRoles = [];

    public static readonly List<RoleOptionData> IntruderHeadRoles = [];
    public static readonly List<RoleOptionData> IntruderKillingRoles = [];
    public static readonly List<RoleOptionData> IntruderSupportRoles = [];
    public static readonly List<RoleOptionData> IntruderDeceptionRoles = [];
    public static readonly List<RoleOptionData> IntruderConcealingRoles = [];
    public static readonly List<RoleOptionData> IntruderRoles = [];

    public static readonly List<RoleOptionData> SyndicatePowerRoles = [];
    public static readonly List<RoleOptionData> SyndicateSupportRoles = [];
    public static readonly List<RoleOptionData> SyndicateKillingRoles = [];
    public static readonly List<RoleOptionData> SyndicateDisruptionRoles = [];
    public static readonly List<RoleOptionData> SyndicateRoles = [];

    public static readonly List<RoleOptionData> AllModifiers = [];
    public static readonly List<RoleOptionData> AllAbilities = [];
    public static readonly List<RoleOptionData> AllDispositions = [];
    public static readonly List<RoleOptionData> AllRoles = [];

    public static PlayerControl Pure;
    public static byte Convertible;

    public static readonly LayerEnum[] CA = [ LayerEnum.Mystic, LayerEnum.VampireHunter ];
    public static readonly LayerEnum[] CI = [ LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer, LayerEnum.Detective ];
    public static readonly LayerEnum[] CSv = [ LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch ];
    public static readonly LayerEnum[] CrP = [ LayerEnum.Altruist, LayerEnum.Medic, LayerEnum.Trapper ];
    public static readonly LayerEnum[] CU = [ LayerEnum.Crewmate ];
    public static readonly LayerEnum[] CK = [ LayerEnum.Vigilante, LayerEnum.Veteran, LayerEnum.Bastion ];
    public static readonly LayerEnum[] CS = [ LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Shifter, LayerEnum.Chameleon, LayerEnum.Retributionist ];
    public static readonly LayerEnum[][] Crew = [ CA, CI, CSv, CrP, CK, CS, CU ];
    public static readonly LayerEnum[][] RegCrew = [ CI, CrP, CK, CS ];
    public static readonly LayerEnum[][][] NonCrew = [ Neutral, Intruders, Syndicate ];

    public static readonly LayerEnum[] NB = [ LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief ];
    public static readonly LayerEnum[] NE = [ LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Troll ];
    public static readonly LayerEnum[] NN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer ];
    public static readonly LayerEnum[] NH = [ LayerEnum.Plaguebearer ];
    public static readonly LayerEnum[] NA = [ LayerEnum.Pestilence ];
    public static readonly LayerEnum[] NK = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf ];
    public static readonly LayerEnum[][] Neutral = [ NB, NE, NN, NH, NK ];
    public static readonly LayerEnum[][] RegNeutral = [ NB, NE ];
    public static readonly LayerEnum[][] HarmNeutral = [ NN, NH, NK ];
    public static readonly LayerEnum[][][] NonNeutral = [ Crew, Intruders, Syndicate ];

    public static readonly LayerEnum[] IC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    public static readonly LayerEnum[] ID = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    public static readonly LayerEnum[] IK = [ LayerEnum.Enforcer, LayerEnum.Ambusher ];
    public static readonly LayerEnum[] IS = [ LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    public static readonly LayerEnum[] IH = [ LayerEnum.Godfather ];
    public static readonly LayerEnum[] IU = [ LayerEnum.Impostor ];
    public static readonly LayerEnum[][] Intruders = [ IC, ID, IK, IS, IU, IH ];
    public static readonly LayerEnum[][] RegIntruders = [ IC, ID, IK, IS ];
    public static readonly LayerEnum[][][] NonIntruders = [ Neutral, Crew, Syndicate ];

    public static readonly LayerEnum[] SSu = [ LayerEnum.Warper, LayerEnum.Stalker ];
    public static readonly LayerEnum[] SD = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer] ;
    public static readonly LayerEnum[] SP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    public static readonly LayerEnum[] SyK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner ];
    public static readonly LayerEnum[] SU = [ LayerEnum.Anarchist ];
    public static readonly LayerEnum[][] Syndicate = [ SSu, SyK, SD, SP, SU ];
    public static readonly LayerEnum[][] RegSyndicate = [ SSu, SyK, SD ];
    public static readonly LayerEnum[][][] NonSyndicate = [ Neutral, Intruders, Crew ];

    public static readonly LayerEnum[][][] FactionedEvils = [ Neutral, Crew ];

    public static readonly LayerEnum[] AlignmentEntries = [ LayerEnum.CrewSupport, LayerEnum.CrewInvest, LayerEnum.CrewSov, LayerEnum.CrewProt, LayerEnum.CrewKill, LayerEnum.CrewAudit,
        LayerEnum.IntruderSupport, LayerEnum.IntruderConceal, LayerEnum.IntruderDecep, LayerEnum.IntruderKill, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb, LayerEnum.NeutralBen,
        LayerEnum.NeutralEvil, LayerEnum.NeutralKill, LayerEnum.NeutralNeo, LayerEnum.SyndicateDisrup, LayerEnum.SyndicateKill, LayerEnum.SyndicatePower, LayerEnum.IntruderUtil,
        LayerEnum.CrewUtil, LayerEnum.SyndicateUtil, LayerEnum.IntruderHead ];
    public static readonly LayerEnum[] RandomEntries = [ LayerEnum.RandomCrew, LayerEnum.RandomIntruder, LayerEnum.RandomSyndicate, LayerEnum.RandomNeutral, LayerEnum.RegularCrew,
        LayerEnum.RegularIntruder, LayerEnum.RegularNeutral, LayerEnum.RegularSyndicate, LayerEnum.HarmfulNeutral, LayerEnum.NonCrew, LayerEnum.NonIntruder, LayerEnum.NonNeutral,
        LayerEnum.FactionedEvil, LayerEnum.NonSyndicate ];
    public static readonly LayerEnum[][] Alignments = [ CA, CI, CSv, CrP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA ];

    public static readonly LayerEnum[] GlobalMod = [ LayerEnum.Dwarf, LayerEnum.VIP, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral,
        LayerEnum.Indomitable, LayerEnum.Yeller, LayerEnum.Colorblind ];

    public static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    public static readonly LayerEnum[] CrewDisp = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    public static readonly LayerEnum[] NeutralDisp = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    public static readonly LayerEnum[] CrewAb = [ LayerEnum.Bullseye, LayerEnum.Swapper ];
    public static readonly LayerEnum[] Tasked = [ LayerEnum.Insider, LayerEnum.Multitasker ];
    public static readonly LayerEnum[] GlobalAb = [ LayerEnum.Radar, LayerEnum.Tiebreaker ];

    public static readonly List<byte> Spawns = [ 0, 1, 2, 3, 4, 5, 6 ];
    public static readonly List<byte> CustomSpawns = [ .. Spawns, 7, 8, 9 ];

    private static readonly TargetGen Targets = new();
    private static readonly ModifierGen Modifiers = new();
    private static readonly AbilityGen Abilities = new();
    private static readonly DispositionGen Dispositions = new();
    public static readonly Dictionary<GameMode, BaseRoleGen> RoleGen = new()
    {
        { GameMode.HideAndSeek, new HideAndSeekGen() },
        { GameMode.Classic, new ClassicCustomGen() },
        { GameMode.RoleList, new RoleListGen() },
        { GameMode.Vanilla, new VanillaGen() },
        { GameMode.AllAny, new AllAnyGen() },
        { GameMode.Custom, new ClassicCustomGen() },
        { GameMode.KillingOnly, new KillingOnlyGen() },
        { GameMode.TaskRace, new TaskRaceGen() }
    };

    public static readonly Dictionary<GameMode, BaseFilter> ModeFilters = new()
    {
        { GameMode.Classic, new CommonFilter() },
        { GameMode.Custom, new CommonFilter() },
        { GameMode.AllAny, new AllAnyFilter() }
    };

    public static RoleOptionData GetSpawnItem(LayerEnum id) => id switch
    {
        LayerEnum.Anarchist => SyndicateUtilityRoles.Anarchist,
        LayerEnum.Impostor => IntruderUtilityRoles.Impostor,
        LayerEnum.Murderer => Options.NeutralKillingRoles.Murderer,
        LayerEnum.Vigilante => Options.CrewKillingRoles.Vigilante,
        LayerEnum.Veteran => Options.CrewKillingRoles.Veteran,
        LayerEnum.Bastion => Options.CrewKillingRoles.Bastion,
        LayerEnum.Crewmate => CrewUtilityRoles.Crewmate,
        LayerEnum.Revealer => CrewUtilityRoles.Revealer,
        LayerEnum.Ghoul => IntruderUtilityRoles.Ghoul,
        LayerEnum.Banshee => SyndicateUtilityRoles.Banshee,
        LayerEnum.Phantom => NeutralProselyteRoles.Phantom,
        LayerEnum.Pestilence => Options.NeutralHarbingerRoles.Plaguebearer,
        LayerEnum.Linked => Options.Dispositions.Linked,
        LayerEnum.Lovers => Options.Dispositions.Lovers,
        LayerEnum.Rivals => Options.Dispositions.Rivals,
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted => new(100, 15, false, false, id),
        _ => OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(x => x.Layer == id, out var result) ? result.Get() : new(0, 0, false, false, id)
    };

    public static bool IsValid(this LayerEnum layer, int? relatedCount = null)
    {
        var result = true;

        if (layer == LayerEnum.Bastion)
            result = GameModifiers.WhoCanVent != WhoCanVentOptions.NoOne;
        else if (layer is LayerEnum.Crewmate or LayerEnum.Impostor or LayerEnum.Anarchist)
            result = IsCustom();
        else if (layer == LayerEnum.VampireHunter)
            result = GetSpawnItem(LayerEnum.Dracula).IsActive();
        else if (layer == LayerEnum.Mystic)
            result = new List<LayerEnum>() { LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Jackal, LayerEnum.Whisperer }.Any(x => GetSpawnItem(x).IsActive());
        else if (layer == LayerEnum.Seer)
        {
            result = new List<LayerEnum>() { LayerEnum.VampireHunter, LayerEnum.BountyHunter, LayerEnum.Godfather, LayerEnum.Rebel, LayerEnum.Plaguebearer, LayerEnum.Mystic, LayerEnum.Traitor,
                LayerEnum.Amnesiac, LayerEnum.Thief, LayerEnum.Executioner, LayerEnum.GuardianAngel, LayerEnum.Guesser, LayerEnum.Shifter, LayerEnum.Fanatic }.Any(x =>
                    GetSpawnItem(x).IsActive());
        }
        else if (layer == LayerEnum.Plaguebearer)
            result = !NeutralApocalypseSettings.DirectSpawn;
        else if (layer == LayerEnum.Pestilence)
            result = NeutralApocalypseSettings.DirectSpawn;
        else if (layer is LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief)
            result = !NeutralSettings.AvoidNeutralKingmakers;
        else if (layer == LayerEnum.Jackal)
            result = GameData.Instance.PlayerCount > 6;
        else if (layer == LayerEnum.Actor)
            result = new List<LayerEnum>() { LayerEnum.Bullseye, LayerEnum.Slayer, LayerEnum.Sniper, LayerEnum.Hitman }.Any(x => GetSpawnItem(x).IsActive());
        else if (layer == LayerEnum.Miner)
            result = GameModifiers.WhoCanVent != WhoCanVentOptions.NoOne && !(!Miner.MinerSpawnOnMira && MapPatches.CurrentMap == 2);
        else if (layer is LayerEnum.Godfather or LayerEnum.Rebel)
            result = relatedCount > 3;
        else if (layer == LayerEnum.Insider)
            result = GameModifiers.AnonymousVoting != AnonVotes.Disabled;
        else if (layer == LayerEnum.Tunneler)
            result = GameModifiers.WhoCanVent == WhoCanVentOptions.Default && CrewSettings.CrewVent == CrewVenting.Never;
        else if (layer == LayerEnum.Lovers)
            result = GameData.Instance.PlayerCount > 4;
        else if (layer == LayerEnum.Rivals)
            result = GameData.Instance.PlayerCount > 3;
        else if (layer == LayerEnum.Linked)
            result = Role.GetRoles(Faction.Neutral).Count() > 1 && GameData.Instance.PlayerCount > 4;

        return result;
    }

    public static int GetRandomCount()
    {
        var random = URandom.RandomRangeInt(0, 100);
        return GameData.Instance.PlayerCount switch
        {
            <= 6 => random <= 5 ? 0 : 1,
            7 => random switch
            {
                < 5 => 0,
                < 20 => 2,
                _ => 1
            },
            8 => random switch
            {
                < 5 => 0,
                < 40 => 2,
                _ => 1
            },
            9 => random switch
            {
                < 5 => 0,
                < 50 => 2,
                _ => 1
            },
            10 => random switch
            {
                < 5 => 0,
                < 10 => 3,
                < 60 => 2,
                _ => 1
            },
            11 => random switch
            {
                < 5 => 0,
                < 40 => 3,
                < 70 => 2,
                _ => 1
            },
            12 => random switch
            {
                < 5 => 0,
                < 60 => 3,
                < 80 => 2,
                _ => 1
            },
            13 => random switch
            {
                < 5 => 0,
                < 60 => 3,
                < 90 => 2,
                _ => 1
            },
            14 => random switch
            {
                < 5 => 0,
                < 25 => 1,
                < 60 => 3,
                _ => 2
            },
            _ => random switch
            {
                < 5 => 0,
                < 20 => 1,
                < 60 => 3,
                < 90 => 2,
                _ => 4
            }
        };
    }

    public static void Gen(PlayerControl player, LayerEnum id, PlayerLayerEnum rpc)
    {
        player.GetLayers().Find(x => x.LayerType == rpc)?.End();
        SetLayer(id, rpc).Start(player);
        CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, rpc, player);
    }

    public static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, LayerEnum.None, rpc);

    public static PlayerLayer SetLayer(LayerEnum id, PlayerLayerEnum rpc)
    {
        if (LayerDictionary.TryGetValue(id, out var dictEntry))
            return (PlayerLayer)Activator.CreateInstance(dictEntry.LayerType);
        else
        {
            return rpc switch
            {
                PlayerLayerEnum.Role => new Roleless(),
                PlayerLayerEnum.Modifier => new Modifierless(),
                PlayerLayerEnum.Disposition => new Dispositionless(),
                PlayerLayerEnum.Ability => new Abilityless(),
                _ => throw new NotImplementedException($"{id}:{rpc}")
            };
        }
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
        gen.InitCrewList();
        gen.Filter();
        gen.Assign();

        var allPlayers = AllPlayers();

        if (GameModifiers.PurePlayers && allPlayers.Any(x => x.Is<Neophyte>()))
            Pure = allPlayers.Random();

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

        Convertible = (byte)allPlayers.Count(x => x.Is(SubFaction.None) && x != Pure);

        if (MapPatches.CurrentMap == 4)
            BetterAirship.SpawnPoints.AddRange((BetterAirship.EnableCustomSpawns ? CustomSpawns : Spawns).GetRandomRange(3));

        CallRpc(CustomRPC.Misc, MiscRPC.EndRoleGen, SetPostmortals.Revealers, SetPostmortals.Phantoms, SetPostmortals.Banshees, SetPostmortals.Ghouls, Pure?.PlayerId ?? 255, Convertible,
            BetterAirship.SpawnPoints);

        if (TownOfUsReworked.MCIActive)
        {
            var maxName = 4;
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

            Message($"| {"Name".PadCenter(maxName)} -> {"Role".PadCenter(maxRole)} | {"Disposition".PadCenter(maxDisp)} | {"Modifier".PadCenter(maxMod)} | {"Ability".PadCenter(maxAb)} |");
            Message($"| {new string('-', maxName)} -> {new string('-', maxRole)} | {new string('-', maxDisp)} | {new string('-', maxMod)} | {new string('-', maxAb)} |");

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

                Message($"| {name.PadCenter(maxName)} -> {roleStr.PadCenter(maxRole)} | {dispStr.PadCenter(maxDisp)} | {modStr.PadCenter(maxMod)} | {abStr.PadCenter(maxAb)} |");
            }
        }
        else
            allPlayers.ForEach(x => RoleManager.Instance.SetRole(x, (RoleTypes)100));

        Success("Gen Ended");
    }

    public static bool Check(RoleOptionData data)
    {
        if (data.Chance == 0)
            return false;

        if (data.Chance == 100)
            return true;

        return URandom.RandomRangeInt(1, 100) <= data.Chance;
    }

    public static void Clear()
    {
        WinState = WinLose.None;

        MeetingPatches.MeetingCount = 0;

        PlayerLayers.Roles.Syndicate.SyndicateHasChaosDrive = false;
        PlayerLayers.Roles.Syndicate.DriveHolder = null;

        Cleaned.Clear();

        MeetingPatches.MeetingCount = 0;

        KilledPlayers.Clear();

        NameHandler.PlayerNames.Clear();
        NameHandler.ColorNames.Clear();

        DragHandler.Instance.Dragging.Clear();

        Monos.Range.AllItems.Clear();

        PlayerLayer.AllLayers.Clear();

        AllRoles.Clear();
        AllModifiers.Clear();
        AllAbilities.Clear();
        AllDispositions.Clear();

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

        SettingsPatches.SettingsPage = 0;

        Assassin.RemainingKills = Assassin.AssassinKills == 0 ? 10000 : Assassin.AssassinKills;

        OnGameEndPatches.Disconnected.Clear();

        CachedFirstDead = FirstDead;
        FirstDead = null;

        Blocked.BlockExposed = false;

        // Role.IsLeft = false;

        CustomMeeting.DestroyAll();
        CustomArrow.DestroyAll();
        CustomMenu.DestroyAll();
        CustomButton.DestroyAll();

        Ash.DestroyAll();

        ClientHandler.Instance.CloseMenus();

        BodyLocations.Clear();

        CachedMorphs.Clear();

        TransitioningSize.Clear();

        TransitioningSpeed.Clear();

        UninteractiblePlayers.Clear();

        BetterAirship.SpawnPoints.Clear();
    }

    public static void ResetEverything()
    {
        Clear();
        Modifiers.Clear();
        Abilities.Clear();
        Dispositions.Clear();
        Targets.Clear();
        RoleGen.Values.ForEach(x => x.Clear());
    }
}