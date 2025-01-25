namespace TownOfUsReworked.Managers;

public static class RoleGenManager
{
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

    public static readonly Enum[] CI = [ LayerEnum.Mystic, LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer, LayerEnum.Detective ];
    public static readonly Enum[] CSv = [ LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch ];
    public static readonly Enum[] CrP = [ LayerEnum.Altruist, LayerEnum.Medic, LayerEnum.Trapper, LayerEnum.Trickster ];
    public static readonly Enum[] CU = [ LayerEnum.Crewmate ];
    public static readonly Enum[] CK = [ LayerEnum.Vigilante, LayerEnum.Veteran, LayerEnum.Bastion ];
    public static readonly Enum[] CS = [ LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Shifter, LayerEnum.Chameleon, LayerEnum.Retributionist ];
    public static readonly Enum[][] Crew = [ CI, CSv, CrP, CK, CS, CU ];
    public static readonly Enum[][] RegCrew = [ CI, CrP, CS, CU ];
    public static readonly Enum[][] PowerCrew = [ CK, CSv ];
    public static readonly Enum[][][] NonCrew = [ Neutral, Intruders, Syndicate ];

    public static readonly Enum[] NB = [ LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief ];
    public static readonly Enum[] NE = [ LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Troll ];
    public static readonly Enum[] NN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer ];
    public static readonly Enum[] NH = [ LayerEnum.Plaguebearer ];
    public static readonly Enum[] NA = [ LayerEnum.Pestilence ];
    public static readonly Enum[] NK = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf ];
    public static readonly Enum[][] Neutral = [ NB, NE, NN, NH, NK ];
    public static readonly Enum[][] RegNeutral = [ NB, NE ];
    public static readonly Enum[][] HarmNeutral = [ NN, NH, NK ];
    public static readonly Enum[][][] NonNeutral = [ Crew, Intruders, Syndicate ];

    public static readonly Enum[] IC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    public static readonly Enum[] ID = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    public static readonly Enum[] IK = [ LayerEnum.Enforcer, LayerEnum.Ambusher ];
    public static readonly Enum[] IS = [ LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    public static readonly Enum[] IH = [ LayerEnum.Godfather ];
    public static readonly Enum[] IU = [ LayerEnum.Impostor ];
    public static readonly Enum[][] Intruders = [ IC, ID, IK, IS, IU, IH ];
    public static readonly Enum[][] RegIntruders = [ IC, ID, IS, IU ];
    public static readonly Enum[][] PowerIntruders = [ IK, IH ];
    public static readonly Enum[][][] NonIntruders = [ Neutral, Crew, Syndicate ];

    public static readonly Enum[] SSu = [ LayerEnum.Warper, LayerEnum.Stalker ];
    public static readonly Enum[] SD = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer] ;
    public static readonly Enum[] SP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    public static readonly Enum[] SyK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner ];
    public static readonly Enum[] SU = [ LayerEnum.Anarchist ];
    public static readonly Enum[][] Syndicate = [ SSu, SyK, SD, SP, SU ];
    public static readonly Enum[][] RegSyndicate = [ SSu, SD, SU ];
    public static readonly Enum[][] PowerSyndicate = [ SyK, SP ];
    public static readonly Enum[][][] NonSyndicate = [ Neutral, Intruders, Crew ];

    public static readonly Enum[] PS = [ LayerEnum.Warper, LayerEnum.Stalker, LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    public static readonly Enum[] PDi = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer];
    public static readonly Enum[] PP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    public static readonly Enum[] PH = [ LayerEnum.Godfather ];
    public static readonly Enum[] PK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner, LayerEnum.Enforcer, LayerEnum.Ambusher ];
    public static readonly Enum[] PC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    public static readonly Enum[] PDe = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    public static readonly Enum[] PU = [ LayerEnum.Anarchist, LayerEnum.Impostor ];
    public static readonly Enum[][] RegPandorica = [ PC, PDe, PDi, PS, PU ];
    public static readonly Enum[][] PowerPandorica = [ PK, PH, PP ];
    public static readonly Enum[][] Pandorica = [ PS, PDi, PP, PH, PK, PC, PDe, PU ];
    public static readonly Enum[][][] NonPandorica = [ HarmNeutral, Crew ];

    public static readonly Enum[][][] NonCompliance = [ RegNeutral, Intruders, Crew, Syndicate ];

    public static readonly Enum[] IlS = [ LayerEnum.Warper, LayerEnum.Stalker, LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    public static readonly Enum[] IlDi = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer];
    public static readonly Enum[] IP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    public static readonly Enum[] IlHe = [ LayerEnum.Godfather ];
    public static readonly Enum[] IlK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner, LayerEnum.Enforcer, LayerEnum.Ambusher, LayerEnum.Arsonist,
        LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf ];
    public static readonly Enum[] IlC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    public static readonly Enum[] IlDe = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    public static readonly Enum[] IlU = [ LayerEnum.Anarchist, LayerEnum.Impostor ];
    public static readonly Enum[] IN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer ];
    public static readonly Enum[] IlHa = [ LayerEnum.Plaguebearer ];
    public static readonly Enum[][] RegIlluminati = [ IlC, IlDe, IlDi, IlS, IlU ];
    public static readonly Enum[][] PowerIlluminati = [ IlK, IlHe, IP, IlHa, IN ];
    public static readonly Enum[][] Illuminati = [ IlS, IlDi, IP, IlHe, IlK, IlC, IlDe, IlU, IN, IlHa ];
    public static readonly Enum[][][] NonIlluminati = [ RegNeutral, Crew ];

    public static readonly Enum[][] Alignments = [ CI, CSv, CrP, CU, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, IH, IU, SU ];

    public static readonly List<byte> Spawns = [ 0, 1, 2, 3, 4, 5, 6 ];
    public static readonly List<byte> CustomSpawns = [ .. Spawns, 7, 8, 9 ];

    private static readonly TargetGen Targets = new();
    private static readonly ModifierGen Modifiers = new();
    private static readonly AbilityGen Abilities = new();
    private static readonly DispositionGen Dispositions = new();
    public static readonly Dictionary<GameMode, BaseRoleGen> RoleGen = new()
    {
        { GameMode.HideAndSeek, new HideAndSeekGen() },
        { GameMode.Classic, new ClassicGen() },
        { GameMode.RoleList, new RoleListGen() },
        { GameMode.Vanilla, new VanillaGen() },
        { GameMode.AllAny, new AllAnyGen() },
        { GameMode.TaskRace, new TaskRaceGen() }
    };

    public static readonly Dictionary<GameMode, BaseFilter> ModeFilters = new()
    {
        { GameMode.Classic, new CommonFilter() },
        { GameMode.AllAny, new AllAnyFilter() }
    };

    public static RoleOptionData GetSpawnItem(LayerEnum id) => id switch
    {
        LayerEnum.Revealer => CrewUtilityRoles.Revealer,
        LayerEnum.Ghoul => IntruderUtilityRoles.Ghoul,
        LayerEnum.Banshee => SyndicateUtilityRoles.Banshee,
        LayerEnum.Phantom => NeutralProselyteRoles.Phantom,
        LayerEnum.Pestilence => Options.NeutralHarbingerRoles.Plaguebearer,
        LayerEnum.Linked => Options.Dispositions.Linked,
        LayerEnum.Lovers => Options.Dispositions.Lovers,
        LayerEnum.Rivals => Options.Dispositions.Rivals,
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted or LayerEnum.CoolPotato or LayerEnum.BurningPotato or LayerEnum.CharredPotato => new(100, 15, false, false, id),
        _ => OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(x => x.Layer == id, out var result) ? result.Get() : new(0, 0, false, false, id)
    };

    public static bool IsValid(this LayerEnum layer, int? relatedCount = null) => layer switch
    {
        LayerEnum.Bastion => GameModifiers.WhoCanVent != WhoCanVentOptions.NoOne,
        LayerEnum.Mystic => new LayerEnum[] { LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Jackal, LayerEnum.Whisperer }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Seer => new LayerEnum[] { LayerEnum.BountyHunter, LayerEnum.Godfather, LayerEnum.Rebel, LayerEnum.Plaguebearer, LayerEnum.Mystic, LayerEnum.Traitor, LayerEnum.Amnesiac,
            LayerEnum.Thief, LayerEnum.Executioner, LayerEnum.GuardianAngel, LayerEnum.Guesser, LayerEnum.Shifter, LayerEnum.Fanatic }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Plaguebearer => !NeutralApocalypseSettings.DirectSpawn,
        LayerEnum.Pestilence => NeutralApocalypseSettings.DirectSpawn,
        LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief => !NeutralSettings.AvoidNeutralKingmakers,
        LayerEnum.Jackal => GameData.Instance.PlayerCount > 6,
        LayerEnum.Actor => new LayerEnum[] { LayerEnum.Bullseye, LayerEnum.Slayer, LayerEnum.Sniper, LayerEnum.Hitman }.Any(x => GetSpawnItem(x).IsActive()),
        LayerEnum.Miner => GameModifiers.WhoCanVent != WhoCanVentOptions.NoOne && (Miner.MinerSpawnOnMira || MapPatches.CurrentMap != 2),
        LayerEnum.Godfather or LayerEnum.Rebel => relatedCount >= 3,
        LayerEnum.Insider => GameModifiers.AnonymousVoting != AnonVotes.Disabled,
        LayerEnum.Tunneler => GameModifiers.WhoCanVent == WhoCanVentOptions.Default && CrewSettings.CrewVent == CrewVenting.Never,
        LayerEnum.Lovers => GameData.Instance.PlayerCount > 4,
        LayerEnum.Rivals => GameData.Instance.PlayerCount > 3,
        LayerEnum.Linked => Role.GetRoles(Faction.Neutral).Count() > 1 && GameData.Instance.PlayerCount > 4,
        _ => true
    };

    public static void Gen(PlayerControl player, LayerEnum id, PlayerLayerEnum rpc)
    {
        player.GetLayers().Find(x => x.LayerType == rpc)?.End();
        GetLayer(id, rpc).Start(player);

        if (!TownOfUsReworked.MCIActive)
            CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, rpc, player);
    }

    public static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, LayerEnum.None, rpc);

    public static PlayerLayer GetLayer(LayerEnum id, PlayerLayerEnum rpc)
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

        if (TownOfUsReworked.MCIActive)
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
            Message($"| {new string('-', maxName)} | {new string('-', maxRole)} | {new string('-', maxDisp)} | {new string('-', maxMod)} | {new string('-', maxAb)} |");

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
            allPlayers.ForEach(x => RoleManager.Instance.SetRole(x, (RoleTypes)100));
            CallRpc(CustomRPC.Misc, MiscRPC.EndRoleGen, SetPostmortals.Revealers, SetPostmortals.Phantoms, SetPostmortals.Banshees, SetPostmortals.Ghouls, Pure?.PlayerId ?? 255, Convertible,
                BetterAirship.SpawnPoints);
        }

        if (SyndicateSettings.AssignOnGameStart)
            AssignChaosDrive();

        Success("Gen Ended");
    }

    public static bool Check(int chance)
    {
        if (chance == 0)
            return false;

        if (chance == 100)
            return true;

        return URandom.RandomRangeInt(1, 100) <= chance;
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