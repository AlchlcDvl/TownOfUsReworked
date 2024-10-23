namespace TownOfUsReworked.Classes;

public static class RoleGen
{
    private static List<RoleOptionData> CrewAuditorRoles = [];
    private static List<RoleOptionData> CrewKillingRoles = [];
    private static List<RoleOptionData> CrewSupportRoles = [];
    private static List<RoleOptionData> CrewSovereignRoles = [];
    private static List<RoleOptionData> CrewProtectiveRoles = [];
    private static List<RoleOptionData> CrewInvestigativeRoles = [];
    private static List<RoleOptionData> CrewRoles = [];

    private static List<RoleOptionData> NeutralEvilRoles = [];
    private static List<RoleOptionData> NeutralBenignRoles = [];
    private static List<RoleOptionData> NeutralKillingRoles = [];
    private static List<RoleOptionData> NeutralNeophyteRoles = [];
    private static List<RoleOptionData> NeutralHarbingerRoles = [];
    private static List<RoleOptionData> NeutralRoles = [];

    private static List<RoleOptionData> IntruderHeadRoles = [];
    private static List<RoleOptionData> IntruderKillingRoles = [];
    private static List<RoleOptionData> IntruderSupportRoles = [];
    private static List<RoleOptionData> IntruderDeceptionRoles = [];
    private static List<RoleOptionData> IntruderConcealingRoles = [];
    private static List<RoleOptionData> IntruderRoles = [];

    private static List<RoleOptionData> SyndicatePowerRoles = [];
    private static List<RoleOptionData> SyndicateSupportRoles = [];
    private static List<RoleOptionData> SyndicateKillingRoles = [];
    private static List<RoleOptionData> SyndicateDisruptionRoles = [];
    private static List<RoleOptionData> SyndicateRoles = [];

    private static List<RoleOptionData> AllModifiers = [];
    private static List<RoleOptionData> AllAbilities = [];
    private static List<RoleOptionData> AllDispositions = [];
    private static List<RoleOptionData> AllRoles = [];

    public static PlayerControl PureCrew;
    public static int Convertible;

    private static readonly LayerEnum[] CA = [ LayerEnum.Mystic, LayerEnum.VampireHunter ];
    private static readonly LayerEnum[] CI = [ LayerEnum.Sheriff, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer, LayerEnum.Detective ];
    private static readonly LayerEnum[] CSv = [ LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch ];
    private static readonly LayerEnum[] CrP = [ LayerEnum.Altruist, LayerEnum.Medic, LayerEnum.Trapper ];
    private static readonly LayerEnum[] CU = [ LayerEnum.Crewmate ];
    private static readonly LayerEnum[] CK = [ LayerEnum.Vigilante, LayerEnum.Veteran, LayerEnum.Bastion ];
    private static readonly LayerEnum[] CS = [ LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Shifter, LayerEnum.Chameleon, LayerEnum.Retributionist ];
    private static readonly LayerEnum[][] Crew = [ CA, CI, CSv, CrP, CK, CS, CU ];
    private static readonly LayerEnum[][] RegCrew = [ CI, CrP, CK, CS ];
    private static readonly LayerEnum[][][] NonCrew = [ Neutral, Intruders, Syndicate ];

    private static readonly LayerEnum[] NB = [ LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief ];
    private static readonly LayerEnum[] NE = [ LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Troll ];
    private static readonly LayerEnum[] NN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer ];
    private static readonly LayerEnum[] NH = [ LayerEnum.Plaguebearer ];
    private static readonly LayerEnum[] NA = [ LayerEnum.Pestilence ];
    private static readonly LayerEnum[] NK = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller, LayerEnum.Werewolf ];
    private static readonly LayerEnum[][] Neutral = [ NB, NE, NN, NH, NK ];
    private static readonly LayerEnum[][] RegNeutral = [ NB, NE ];
    private static readonly LayerEnum[][] HarmNeutral = [ NN, NH, NK ];
    private static readonly LayerEnum[][][] NonNeutral = [ Crew, Intruders, Syndicate ];

    private static readonly LayerEnum[] IC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    private static readonly LayerEnum[] ID = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    private static readonly LayerEnum[] IK = [ LayerEnum.Enforcer, LayerEnum.Ambusher ];
    private static readonly LayerEnum[] IS = [ LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    private static readonly LayerEnum[] IH = [ LayerEnum.Godfather ];
    private static readonly LayerEnum[] IU = [ LayerEnum.Impostor ];
    private static readonly LayerEnum[][] Intruders = [ IC, ID, IK, IS, IU, IH ];
    private static readonly LayerEnum[][] RegIntruders = [ IC, ID, IK, IS ];
    private static readonly LayerEnum[][][] NonIntruders = [ Neutral, Crew, Syndicate ];

    private static readonly LayerEnum[] SSu = [ LayerEnum.Warper, LayerEnum.Stalker ];
    private static readonly LayerEnum[] SD = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer] ;
    private static readonly LayerEnum[] SP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    private static readonly LayerEnum[] SyK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner ];
    private static readonly LayerEnum[] SU = [ LayerEnum.Anarchist ];
    private static readonly LayerEnum[][] Syndicate = [ SSu, SyK, SD, SP, SU ];
    private static readonly LayerEnum[][] RegSyndicate = [ SSu, SyK, SD ];
    private static readonly LayerEnum[][][] NonSyndicate = [ Neutral, Intruders, Crew ];

    private static readonly LayerEnum[][][] FactionedEvils = [ Neutral, Crew ];

    private static readonly LayerEnum[] AlignmentEntries = [ LayerEnum.CrewSupport, LayerEnum.CrewInvest, LayerEnum.CrewSov, LayerEnum.CrewProt, LayerEnum.CrewKill, LayerEnum.CrewAudit, LayerEnum.IntruderSupport,
        LayerEnum.IntruderConceal, LayerEnum.IntruderDecep, LayerEnum.IntruderKill, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb, LayerEnum.NeutralBen, LayerEnum.NeutralEvil, LayerEnum.NeutralKill, LayerEnum.NeutralNeo,
        LayerEnum.SyndicateDisrup, LayerEnum.SyndicateKill, LayerEnum.SyndicatePower, LayerEnum.IntruderUtil, LayerEnum.CrewUtil, LayerEnum.SyndicateUtil, LayerEnum.IntruderHead ];
    private static readonly LayerEnum[] RandomEntries = [ LayerEnum.RandomCrew, LayerEnum.RandomIntruder, LayerEnum.RandomSyndicate, LayerEnum.RandomNeutral, LayerEnum.RegularCrew, LayerEnum.RegularIntruder,
        LayerEnum.RegularNeutral, LayerEnum.RegularSyndicate, LayerEnum.HarmfulNeutral, LayerEnum.NonCrew, LayerEnum.NonIntruder, LayerEnum.NonNeutral, LayerEnum.FactionedEvil, LayerEnum.NonSyndicate ];
    private static readonly LayerEnum[][] Alignments = [ CA, CI, CSv, CrP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA ];

    private static readonly LayerEnum[] GlobalMod = [ LayerEnum.Dwarf, LayerEnum.VIP, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral, LayerEnum.Indomitable, LayerEnum.Yeller,
        LayerEnum.Colorblind ];

    private static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    private static readonly LayerEnum[] CrewObj = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    private static readonly LayerEnum[] NeutralObj = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    private static readonly LayerEnum[] CrewAb = [ LayerEnum.Bullseye, LayerEnum.Swapper ];
    private static readonly LayerEnum[] Tasked = [ LayerEnum.Insider, LayerEnum.Multitasker ];
    private static readonly LayerEnum[] GlobalAb = [ LayerEnum.Radar, LayerEnum.Tiebreaker ];

    private static readonly List<byte> Spawns = [ 0, 1, 2, 3, 4, 5, 6 ];
    private static readonly List<byte> CustomSpawns = [ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 ];

    private static bool Check(RoleOptionData data, bool sorting = false)
    {
        if (IsAA())
            return URandom.RandomRangeInt(0, 2) == 0 && data.Active;
        else
        {
            if (data.Chance == 0)
                return false;

            if (data.Chance == 100)
                return !sorting;

            return URandom.RandomRangeInt(1, 100) <= data.Chance;
        }
    }

    private static List<RoleOptionData> Sort(List<RoleOptionData> items, int amount)
    {
        var newList = new List<RoleOptionData>();
        items.Shuffle();

        if (amount != AllPlayers().Count && IsAA())
            amount = AllPlayers().Count;
        else if (items.Count < amount)
            amount = items.Count;

        if (IsAA())
        {
            var rate = 0;

            while (newList.Count < amount && items.Any() && rate < 10000)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Unique)
                    items.Remove(items[0]);
                else
                    rate++;
            }
        }
        else
        {
            var guaranteed = items.Where(x => x.Chance == 100).ToList();
            guaranteed.Shuffle();
            var optionals = items.Where(x => Check(x, true)).ToList();
            optionals.Shuffle();
            newList.AddRanges(guaranteed, optionals);

            while (newList.Count < amount)
                newList.Add(items.Where(x => x.Chance < 100).Random(x => !newList.Contains(x)));
        }

        newList = [ .. newList.OrderByDescending(x => x.Chance) ];

        while (newList.Count > amount && newList.Count > 1)
            newList.Remove(newList.Last());

        newList.Shuffle();
        return newList;
    }

    private static void GetAdjustedFactions(out int impostors, out int syndicate, out int neutrals, out int crew)
    {
        var players = GameData.Instance.PlayerCount;
        impostors = IntruderSettings.IntruderCount;
        syndicate = SyndicateSettings.SyndicateCount;
        neutrals = IsKilling() ? GameModeSettings.NeutralsCount : URandom.RandomRangeInt(NeutralSettings.NeutralMin, NeutralSettings.NeutralMax + 1);

        if (impostors == 0 && syndicate == 0 && neutrals == 0)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0)
                impostors++;
            else if (random2 == 1)
                syndicate++;
            else if (random2 == 2)
                neutrals++;
        }

        while (neutrals >= players - impostors - syndicate)
            neutrals--;

        crew = players - impostors - syndicate - neutrals;

        while (crew == 0 && (impostors > 0 || syndicate > 0 || neutrals > 0) && players > 1)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0 && impostors > 0)
            {
                impostors--;
                crew++;
            }
            else if (random2 == 1 && syndicate > 0)
            {
                syndicate--;
                crew++;
            }
            else if (random2 == 2 && neutrals > 0)
            {
                neutrals--;
                crew++;
            }
        }

        if (TownOfUsReworked.IsTest)
            Info($"Crew = {crew}, Int = {impostors}, Syn = {syndicate}, Neut = {neutrals}");

        if (!IsAA())
            return;

        impostors = GetRandomCount();
        neutrals = GetRandomCount();
        syndicate = GetRandomCount();

        if (impostors == 0 && syndicate == 0 && neutrals == 0)
        {
            var random = URandom.RandomRangeInt(0, 3);

            if (random == 0)
                impostors++;
            else if (random == 1)
                syndicate++;
            else if (random == 2)
                neutrals++;
        }

        crew = players - impostors - syndicate - neutrals;

        while (crew == 0 && (impostors > 0 || syndicate > 0 || neutrals > 0) && players > 1)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0 && impostors > 0)
            {
                impostors--;
                crew++;
            }
            else if (random2 == 1 && syndicate > 0)
            {
                syndicate--;
                crew++;
            }
            else if (random2 == 2 && neutrals > 0)
            {
                neutrals--;
                crew++;
            }
        }

        if (TownOfUsReworked.IsTest)
            Info($"Crew = {crew}, Int = {impostors}, Syn = {syndicate}, Neut = {neutrals}");
    }

    private static int GetRandomCount()
    {
        var random = URandom.RandomRangeInt(0, 100);
        var players = GameData.Instance.PlayerCount;
        var result = 0;

        if (players <= 6)
        {
            if (random <= 5)
                result = 0;
            else
                result = 1;
        }
        else if (players == 7)
        {
            if (random < 5)
                result = 0;
            else if (random < 20)
                result = 2;
            else
                result = 1;
        }
        else if (players == 8)
        {
            if (random < 5)
                result = 0;
            else if (random < 40)
                result = 2;
            else
                result = 1;
        }
        else if (players == 9)
        {
            if (random < 5)
                result = 0;
            else if (random < 50)
                result = 2;
            else
                result = 1;
        }
        else if (players == 10)
        {
            if (random < 5)
                result = 0;
            else if (random < 10)
                result = 3;
            else if (random < 60)
                result = 2;
            else
                result = 1;
        }
        else if (players == 11)
        {
            if (random < 10)
                result = 0;
            else if (random < 60)
                result = 2;
            else if (random < 70)
                result = 3;
            else
                result = 1;
        }
        else if (players == 12)
        {
            if (random < 10)
                result = 0;
            else if (random < 60)
                result = 2;
            else if (random < 80)
                result = 3;
            else
                result = 1;
        }
        else if (players == 13)
        {
            if (random < 10)
                result = 0;
            else if (random < 60)
                result = 2;
            else if (random < 90)
                result = 3;
            else
                result = 1;
        }
        else if (players == 14)
        {
            if (random < 5)
                result = 0;
            else if (random < 25)
                result = 1;
            else if (random < 50)
                result = 3;
            else
                result = 2;
        }
        else if (random < 5)
            result = 0;
        else if (random < 20)
            result = 1;
        else if (random < 60)
            result = 3;
        else if (random < 90)
            result = 2;
        else
            result = 4;

        return result;
    }

    private static void GenVanilla()
    {
        while (AllRoles.Count < (SyndicateSettings.AltImps ? SyndicateSettings.SyndicateCount : IntruderSettings.IntruderCount))
            AllRoles.Add(GetSpawnItem(SyndicateSettings.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static void GenKilling()
    {
        GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);

        if (imps > 0)
        {
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Enforcer));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Morphling));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Blackmailer));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Miner));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Teleporter));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Wraith));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Consort));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Janitor));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Camouflager));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Grenadier));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Impostor));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Consigliere));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Disguiser));
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Ambusher));

            if (imps >= 3)
                IntruderRoles.Add(GetSpawnItem(LayerEnum.Godfather));

            IntruderRoles = Sort(IntruderRoles, imps);
        }

        if (syn > 0)
        {
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist));
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Bomber));
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Poisoner));
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Crusader));
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Collider));

            if (syn >= 3)
                SyndicateRoles.Add(GetSpawnItem(LayerEnum.Rebel));

            SyndicateRoles = Sort(SyndicateRoles, syn);
        }

        if (neut > 0)
        {
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Glitch));
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Werewolf));
            NeutralRoles.Add(GetSpawnItem(LayerEnum.SerialKiller));
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Juggernaut));
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Murderer));
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Thief));

            if (GameModeSettings.AddArsonist)
                NeutralRoles.Add(GetSpawnItem(LayerEnum.Arsonist));

            if (GameModeSettings.AddCryomaniac)
                NeutralRoles.Add(GetSpawnItem(LayerEnum.Cryomaniac));

            if (GameModeSettings.AddPlaguebearer)
                NeutralRoles.Add(GetSpawnItem(NeutralApocalypseSettings.DirectSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));

            NeutralRoles = Sort(NeutralRoles, neut);
        }

        if (crew > 0)
        {
            var vigis = crew / 3;
            var vets = crew / 3;
            var basts = crew / 3;

            while (vigis > 0 || vets > 0 || basts > 0)
            {
                if (vigis > 0)
                {
                    CrewRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add(GetSpawnItem(LayerEnum.Veteran));
                    vets--;
                }

                if (basts > 0)
                {
                    CrewRoles.Add(GetSpawnItem(LayerEnum.Bastion));
                    basts--;
                }
            }

            CrewRoles = Sort(CrewRoles, crew);
        }

        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
    }

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
            result = new List<LayerEnum>() { LayerEnum.VampireHunter, LayerEnum.BountyHunter, LayerEnum.Godfather, LayerEnum.Rebel, LayerEnum.Plaguebearer, LayerEnum.Mystic, LayerEnum.Traitor, LayerEnum.Amnesiac,
                LayerEnum.Thief, LayerEnum.Executioner, LayerEnum.GuardianAngel, LayerEnum.Guesser, LayerEnum.Shifter }.Any(x => GetSpawnItem(x).IsActive());
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
            result = Role.GetRoles(Faction.Neutral).Count > 1 && GameData.Instance.PlayerCount > 4;

        if (TownOfUsReworked.IsTest)
            result = true;

        return result;
    }

    private static void GenClassicCustomAA()
    {
        GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);

        if (crew > 0)
        {
            for (var i = 0; i < 26; i++)
            {
                var layer = (LayerEnum)i;
                var spawn = GetSpawnItem(layer);

                if (layer == LayerEnum.Revealer)
                    SetPostmortals.RevealerOn = Check(spawn);
                else if (spawn.IsActive())
                {
                    var num = spawn.Count;

                    while (num > 0)
                    {
                        if (layer is LayerEnum.Mayor or LayerEnum.Monarch or LayerEnum.Dictator)
                            CrewSovereignRoles.Add(spawn);
                        else if (layer is LayerEnum.Mystic or LayerEnum.VampireHunter)
                            CrewAuditorRoles.Add(spawn);
                        else if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker)
                            CrewInvestigativeRoles.Add(spawn);
                        else if (layer is LayerEnum.Bastion or LayerEnum.Vigilante or LayerEnum.Veteran)
                            CrewKillingRoles.Add(spawn);
                        else if (layer is LayerEnum.Altruist or LayerEnum.Medic or LayerEnum.Trapper)
                            CrewProtectiveRoles.Add(spawn);
                        else if (layer is LayerEnum.Chameleon or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Retributionist or LayerEnum.Shifter or LayerEnum.Transporter)
                            CrewSupportRoles.Add(spawn);
                        else if (layer == LayerEnum.Crewmate)
                            CrewRoles.Add(spawn);

                        num--;
                    }
                }

                Info($"{layer} Done");
            }
        }

        if (neut > 0)
        {
            for (var i = 26; i < 52; i++)
            {
                var layer = (LayerEnum)i;

                if (layer == LayerEnum.Betrayer)
                    continue;

                var spawn = GetSpawnItem(layer);

                if (layer == LayerEnum.Phantom)
                    SetPostmortals.PhantomOn = Check(spawn);
                else if (spawn.IsActive())
                {
                    var num = spawn.Count;

                    while (num > 0)
                    {
                        if (layer is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                            NeutralHarbingerRoles.Add(spawn);
                        else if (layer is LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief)
                            NeutralBenignRoles.Add(spawn);
                        else if (layer is LayerEnum.Actor or LayerEnum.BountyHunter or LayerEnum.Cannibal or LayerEnum.Executioner or LayerEnum.Guesser or LayerEnum.Jester or LayerEnum.Troll)
                            NeutralEvilRoles.Add(spawn);
                        else if (layer is LayerEnum.Arsonist or LayerEnum.Cryomaniac or LayerEnum.Glitch or LayerEnum.Juggernaut or LayerEnum.Murderer or LayerEnum.SerialKiller or LayerEnum.Werewolf)
                            NeutralKillingRoles.Add(spawn);
                        else if (layer is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer)
                            NeutralNeophyteRoles.Add(spawn);

                        num--;
                    }
                }

                Info($"{layer} Done");
            }
        }

        if (imps > 0)
        {
            for (var i = 52; i < 70; i++)
            {
                var layer = (LayerEnum)i;

                if (layer is LayerEnum.PromotedGodfather or LayerEnum.Mafioso)
                    continue;

                var spawn = GetSpawnItem(layer);

                if (layer == LayerEnum.Ghoul)
                    SetPostmortals.GhoulOn = Check(spawn);
                else if (spawn.IsActive(imps))
                {
                    var num = spawn.Count;

                    while (num > 0)
                    {
                        if (layer == LayerEnum.Godfather)
                            IntruderHeadRoles.Add(spawn);
                        else if (layer is LayerEnum.Blackmailer or LayerEnum.Camouflager or LayerEnum.Grenadier or LayerEnum.Janitor)
                            IntruderConcealingRoles.Add(spawn);
                        else if (layer is LayerEnum.Disguiser or LayerEnum.Morphling or LayerEnum.Wraith)
                            IntruderDeceptionRoles.Add(spawn);
                        else if (layer is LayerEnum.Ambusher or LayerEnum.Enforcer)
                            IntruderKillingRoles.Add(spawn);
                        else if (layer is LayerEnum.Consigliere or LayerEnum.Consort or LayerEnum.Miner or LayerEnum.Teleporter)
                            IntruderSupportRoles.Add(spawn);
                        else if (layer == LayerEnum.Impostor)
                            IntruderRoles.Add(spawn);

                        num--;
                    }
                }

                Info($"{layer} Done");
            }
        }

        if (syn > 0)
        {
            for (var i = 70; i < 88; i++)
            {
                var layer = (LayerEnum)i;

                if (layer is LayerEnum.PromotedRebel or LayerEnum.Sidekick)
                    continue;

                var spawn = GetSpawnItem(layer);

                if (layer == LayerEnum.Banshee)
                    SetPostmortals.BansheeOn = Check(spawn);
                else if (spawn.IsActive(syn))
                {
                    var num = spawn.Count;

                    while (num > 0)
                    {
                        if (layer is LayerEnum.Rebel or LayerEnum.Spellslinger)
                            SyndicatePowerRoles.Add(spawn);
                        else if (layer is LayerEnum.Concealer or LayerEnum.Drunkard or LayerEnum.Framer or LayerEnum.Shapeshifter or LayerEnum.Silencer or LayerEnum.Timekeeper)
                            SyndicateDisruptionRoles.Add(spawn);
                        else if (layer is LayerEnum.Bomber or LayerEnum.Collider or LayerEnum.Crusader or LayerEnum.Poisoner)
                            SyndicateKillingRoles.Add(spawn);
                        else if (layer is LayerEnum.Stalker or LayerEnum.Warper)
                            SyndicateSupportRoles.Add(spawn);
                        else if (layer == LayerEnum.Anarchist)
                            SyndicateRoles.Add(spawn);
                        else if (layer == LayerEnum.Banshee)
                            SetPostmortals.BansheeOn = Check(spawn);

                        num--;
                    }
                }

                Info($"{layer} Done");
            }
        }

        if (IsClassic() || IsCustom())
        {
            if (!SyndicateSettings.AltImps && imps > 0)
            {
                int maxIC = IntruderConcealingSettings.MaxIC;
                int maxID = IntruderDeceptionSettings.MaxID;
                int maxIK = IntruderKillingSettings.MaxIK;
                int maxIS = IntruderSupportSettings.MaxIS;
                int maxIH = IntruderHeadSettings.MaxIH;
                int minInt = IntruderSettings.IntruderMin;
                int maxInt = IntruderSettings.IntruderMax;

                if (minInt > maxInt)
                    (maxInt, minInt) = (minInt, maxInt);

                var maxIntSum = maxIC + maxID + maxIK + maxIS + maxIH;

                while (maxIntSum > maxInt)
                {
                    switch (URandom.RandomRangeInt(0, 5))
                    {
                        case 0:
                            if (maxIC > 0)
                                maxIC--;

                            break;

                        case 1:
                            if (maxID > 0)
                                maxID--;

                            break;

                        case 2:
                            if (maxIK > 0)
                                maxIK--;

                            break;

                        case 3:
                            if (maxIS > 0)
                                maxIS--;

                            break;

                        case 4:
                            if (maxIH > 0)
                                maxIH--;

                            break;
                    }

                    maxIntSum = maxIC + maxID + maxIK + maxIS + maxIH;
                }

                if (!GameModeSettings.IgnoreAlignmentCaps)
                {
                    IntruderConcealingRoles = Sort(IntruderConcealingRoles, maxIC);
                    IntruderDeceptionRoles = Sort(IntruderDeceptionRoles, maxID);
                    IntruderKillingRoles = Sort(IntruderKillingRoles, maxIK);
                    IntruderSupportRoles = Sort(IntruderSupportRoles, maxIS);
                    IntruderHeadRoles = Sort(IntruderHeadRoles, maxIH);
                }

                IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles, IntruderHeadRoles);

                while (maxInt > imps)
                    maxInt--;

                while (minInt > imps)
                    minInt--;

                IntruderRoles = Sort(IntruderRoles, GameModeSettings.IgnoreFactionCaps ? imps : URandom.RandomRangeInt(minInt, maxInt + 1));
            }

            while (IntruderRoles.Count < imps)
                IntruderRoles.Add(GetSpawnItem(LayerEnum.Impostor));

            if (syn > 0)
            {
                int maxSSu = SyndicateSupportSettings.MaxSSu;
                int maxSD = SyndicateDisruptionSettings.MaxSD;
                int maxSyK = SyndicateKillingSettings.MaxSyK;
                int maxSP = SyndicatePowerSettings.MaxSP;
                int minSyn = SyndicateSettings.SyndicateMin;
                int maxSyn = SyndicateSettings.SyndicateMax;

                if (minSyn > maxSyn)
                    (maxSyn, minSyn) = (minSyn, maxSyn);

                var maxSynSum = maxSSu + maxSD + maxSyK + maxSP;

                while (maxSynSum > maxSyn)
                {
                    switch (URandom.RandomRangeInt(0, 4))
                    {
                        case 0:
                            if (maxSSu > 0)
                                maxSSu--;

                            break;

                        case 1:
                            if (maxSD > 0)
                                maxSD--;

                            break;

                        case 2:
                            if (maxSyK > 0)
                                maxSyK--;

                            break;

                        case 3:
                            if (maxSP > 0)
                                maxSP--;

                            break;
                    }

                    maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
                }

                if (!GameModeSettings.IgnoreAlignmentCaps)
                {
                    SyndicateSupportRoles = Sort(SyndicateSupportRoles, maxSSu);
                    SyndicateDisruptionRoles = Sort(SyndicateDisruptionRoles, maxSD);
                    SyndicateKillingRoles = Sort(SyndicateKillingRoles, maxSyK);
                    SyndicatePowerRoles = Sort(SyndicatePowerRoles, maxSP);
                }

                SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);

                while (maxSyn > syn)
                    maxSyn--;

                while (minSyn > syn)
                    minSyn--;

                SyndicateRoles = Sort(SyndicateRoles, GameModeSettings.IgnoreFactionCaps ? syn : URandom.RandomRangeInt(minSyn, maxSyn + 1));
            }

            while (SyndicateRoles.Count < syn)
                SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist));

            if (neut > 0)
            {
                int maxNE = NeutralEvilSettings.MaxNE;
                int maxNB = NeutralBenignSettings.MaxNB;
                int maxNK = NeutralKillingSettings.MaxNK;
                int maxNN = NeutralNeophyteSettings.MaxNN;
                int maxNH = NeutralHarbingerSettings.MaxNH;
                int minNeut = NeutralSettings.NeutralMin;
                int maxNeut = NeutralSettings.NeutralMax;

                if (minNeut > maxNeut)
                    (maxNeut, minNeut) = (minNeut, maxNeut);

                var maxNeutSum = maxNE + maxNB + maxNK + maxNN + maxNH;

                while (maxNeutSum > maxNeut)
                {
                    switch (URandom.RandomRangeInt(0, 5))
                    {
                        case 0:
                            if (maxNE > 0)
                                maxNE--;

                            break;

                        case 1:
                            if (maxNB > 0)
                                maxNB--;

                            break;

                        case 2:
                            if (maxNK > 0)
                                maxNK--;

                            break;

                        case 3:
                            if (maxNN > 0)
                                maxNN--;

                            break;

                        case 4:
                            if (maxNH > 0)
                                maxNH--;

                            break;
                    }

                    maxNeutSum = maxNE + maxNB + maxNK + maxNN + maxNH;
                }

                if (!GameModeSettings.IgnoreAlignmentCaps)
                {
                    NeutralBenignRoles = Sort(NeutralBenignRoles, maxNB);
                    NeutralEvilRoles = Sort(NeutralEvilRoles, maxNE);
                    NeutralKillingRoles = Sort(NeutralKillingRoles, maxNK);
                    NeutralNeophyteRoles = Sort(NeutralNeophyteRoles, maxNN);
                    NeutralHarbingerRoles = Sort(NeutralHarbingerRoles, maxNH);
                }

                NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

                while (maxNeut > neut)
                    maxNeut--;

                while (minNeut > neut)
                    minNeut--;

                NeutralRoles = Sort(NeutralRoles, GameModeSettings.IgnoreFactionCaps ? neut : URandom.RandomRangeInt(minNeut, maxNeut + 1));
            }

            if (crew > 0)
            {
                int maxCI = CrewInvestigativeSettings.MaxCI;
                int maxCS = CrewSupportSettings.MaxCS;
                int maxCA = CrewAuditorSettings.MaxCA;
                int maxCK = CrewKillingSettings.MaxCK;
                int maxCrP = CrewProtectiveSettings.MaxCrP;
                int maxCSv = CrewSovereignSettings.MaxCSv;
                int minCrew = CrewSettings.CrewMin;
                int maxCrew = CrewSettings.CrewMax;

                if (minCrew > maxCrew)
                    (maxCrew, minCrew) = (minCrew, maxCrew);

                var maxCrewSum = maxCA + maxCI + maxCK + maxCrP + maxCS + maxCSv;

                while (maxCrewSum > maxCrew)
                {
                    switch (URandom.RandomRangeInt(0, 6))
                    {
                        case 0:
                            if (maxCA > 0)
                                maxCA--;

                            break;

                        case 1:
                            if (maxCI > 0)
                                maxCI--;

                            break;

                        case 2:
                            if (maxCK > 0)
                                maxCK--;

                            break;

                        case 3:
                            if (maxCS > 0)
                                maxCS--;

                            break;

                        case 4:
                            if (maxCSv > 0)
                                maxCSv--;

                            break;

                        case 5:
                            if (maxCrP > 0)
                                maxCrP--;

                            break;
                    }

                    maxCrewSum = maxCA + maxCI + maxCK + maxCrP + maxCS + maxCSv;
                }

                if (!GameModeSettings.IgnoreAlignmentCaps)
                {
                    CrewAuditorRoles = Sort(CrewAuditorRoles, maxCA);
                    CrewInvestigativeRoles = Sort(CrewInvestigativeRoles, maxCI);
                    CrewKillingRoles = Sort(CrewKillingRoles, maxCK);
                    CrewProtectiveRoles = Sort(CrewProtectiveRoles, maxCrP);
                    CrewSupportRoles = Sort(CrewSupportRoles, maxCS);
                    CrewSovereignRoles = Sort(CrewSovereignRoles, maxCSv);
                }

                CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);

                while (maxCrew > crew)
                    maxCrew--;

                while (minCrew > crew)
                    minCrew--;

                CrewRoles = Sort(CrewRoles, GameModeSettings.IgnoreFactionCaps ? crew : URandom.RandomRangeInt(minCrew, maxCrew + 1));
            }

            while (CrewRoles.Count < crew)
                CrewRoles.Add(GetSpawnItem(LayerEnum.Crewmate));

            Message("Classic/Custom Sorting Done");
        }
        else if (IsAA())
        {
            CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);
            IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles, IntruderHeadRoles);
            SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);
            NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

            IntruderRoles = Sort(IntruderRoles, imps);
            CrewRoles = Sort(CrewRoles, crew);
            NeutralRoles = Sort(NeutralRoles, neut);
            SyndicateRoles = Sort(SyndicateRoles, syn);

            Message("All Any Sorting Done");
        }

        CrewRoles.Shuffle();
        SyndicateRoles.Shuffle();
        IntruderRoles.Shuffle();
        NeutralRoles.Shuffle();

        AllRoles.AddRanges(CrewRoles, NeutralRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) || RoleListEntryAttribute.IsBanned(id);

    private static void GenRoleList()
    {
        var entries = OptionAttribute.GetOptions<RoleListEntryAttribute>().Where(x => x.Name.Contains("Entry"));
        var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get()));
        var randoms = entries.Where(x => RandomEntries.Contains(x.Get()));
        var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get())));
        var anies = entries.Where(x => x.Get() == LayerEnum.Any);
        // I have no idea what plural for any is lmao

        SetPostmortals.PhantomOn = RoleListEntries.EnablePhantom;
        SetPostmortals.RevealerOn = RoleListEntries.EnableRevealer;
        SetPostmortals.BansheeOn = RoleListEntries.EnableBanshee;
        SetPostmortals.GhoulOn = RoleListEntries.EnableGhoul;

        foreach (var entry in roles)
        {
            var ratelimit = 0;
            var id = entry.Get();
            var cachedCount = AllRoles.Count;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(id));
            }
        }

        foreach (var entry in alignments)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = id switch
                {
                    LayerEnum.CrewAudit => CA.Random(),
                    LayerEnum.CrewInvest => CI.Random(),
                    LayerEnum.CrewSov => CSv.Random(),
                    LayerEnum.CrewProt => CrP.Random(),
                    LayerEnum.CrewKill => CK.Random(),
                    LayerEnum.CrewSupport => CS.Random(),
                    LayerEnum.NeutralBen => NB.Random(),
                    LayerEnum.NeutralEvil => NE.Random(),
                    LayerEnum.NeutralNeo => NN.Random(),
                    LayerEnum.NeutralHarb => NH.Random(),
                    LayerEnum.NeutralApoc => NA.Random(),
                    LayerEnum.NeutralKill => NK.Random(),
                    LayerEnum.IntruderConceal => IC.Random(),
                    LayerEnum.IntruderDecep => ID.Random(),
                    LayerEnum.IntruderKill => IK.Random(),
                    LayerEnum.IntruderSupport => IS.Random(),
                    LayerEnum.SyndicateSupport => SSu.Random(),
                    LayerEnum.SyndicatePower => SP.Random(),
                    LayerEnum.SyndicateDisrup => SD.Random(),
                    LayerEnum.SyndicateKill => SyK.Random(),
                    LayerEnum.IntruderHead => IH.Random(),
                    _ => LayerEnum.None
                };

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        foreach (var entry in randoms)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = id switch
                {
                    LayerEnum.RandomCrew => Crew.Random().Random(),
                    LayerEnum.RandomNeutral => Neutral.Random().Random(),
                    LayerEnum.RandomIntruder => Intruders.Random().Random(),
                    LayerEnum.RandomSyndicate => Syndicate.Random().Random(),
                    LayerEnum.RegularCrew => RegCrew.Random().Random(),
                    LayerEnum.RegularIntruder => RegIntruders.Random().Random(),
                    LayerEnum.RegularNeutral => RegNeutral.Random().Random(),
                    LayerEnum.HarmfulNeutral => HarmNeutral.Random().Random(),
                    LayerEnum.RegularSyndicate => RegSyndicate.Random().Random(),
                    LayerEnum.NonIntruder => NonIntruders.Random().Random().Random(),
                    LayerEnum.NonCrew => NonCrew.Random().Random().Random(),
                    LayerEnum.NonNeutral => NonNeutral.Random().Random().Random(),
                    LayerEnum.NonSyndicate => NonSyndicate.Random().Random().Random(),
                    LayerEnum.FactionedEvil => FactionedEvils.Random().Random().Random(),
                    _ => LayerEnum.None
                };

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        foreach (var entry in anies)
        {
            var cachedCount = AllRoles.Count;
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = Alignments.Random().Random();

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        // In case if the ratelimits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static void GenTaskRace()
    {
        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Runner));
    }

    private static void GenHideAndSeek()
    {
        while (AllRoles.Count < GameModeSettings.HunterCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Hunter));

        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Hunted));
    }

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
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted => new(100, 15, false, false, id),
        _ => OptionAttribute.GetOptions<LayersOptionAttribute>().TryFinding(x => x.Layer == id, out var result) ? result.Get() : new(0, 0, false, false, id)
    };

    private static void GenAbilities()
    {
        for (var i = 120; i < 137; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                var num = spawn.Count;

                while (num > 0)
                {
                    AllAbilities.Add(spawn);
                    num--;
                }
            }

            Info($"{layer} Done");
        }

        int maxAb = AbilitiesSettings.MaxAbilities;
        int minAb = AbilitiesSettings.MinAbilities;

        while (maxAb > AllPlayers().Count)
            maxAb--;

        while (minAb > AllPlayers().Count)
            minAb--;

        AllAbilities = Sort(AllAbilities, GameModeSettings.IgnoreLayerCaps ? AllPlayers().Count : URandom.RandomRangeInt(minAb, maxAb + 1));

        var playerList = AllPlayers();
        playerList.Shuffle();

        AllAbilities = [ .. AllAbilities.OrderByDescending(x => x.Chance) ];

        while (AllAbilities.Count > AllPlayers().Count && AllAbilities.Count > 1)
            AllAbilities.Remove(AllAbilities.Last());

        AllAbilities.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllAbilities)
                ids += $" {spawn.ID}";

            Message("Abilities in the game: " + ids);
        }

        var invalid = new List<LayerEnum>();

        while (playerList.Any() && AllAbilities.Any())
        {
            var id = AllAbilities.TakeFirst().ID;
            PlayerControl assigned = null;

            if (id == LayerEnum.Snitch)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is(LayerEnum.Traitor) && !x.Is(LayerEnum.Fanatic));
            else if (id == LayerEnum.Sniper)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Syndicate));
            else if (CrewAb.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (id == LayerEnum.Slayer)
                assigned = playerList.FirstOrDefault(x => x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) || x.Is(Alignment.NeutralHarb));
            else if (id == LayerEnum.Hitman)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && !(x.Is(LayerEnum.Consigliere) && Consigliere.ConsigInfo == ConsigInfo.Role));
            else if (id == LayerEnum.Ninja)
            {
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) || x.Is(Alignment.CrewKill) ||
                    x.Is(LayerEnum.Corrupted));
            }
            else if (id == LayerEnum.Torch)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(Alignment.NeutralKill) && !NeutralKillingSettings.NKHasImpVision) || x.Is(Faction.Syndicate) || x.Is(Faction.Intruder) || (x.Is(Faction.Neutral) &&
                    !NeutralSettings.LightsAffectNeutrals) || (x.Is(Alignment.NeutralNeo) && !NeutralNeophyteSettings.NNHasImpVision) || (x.Is(Alignment.NeutralEvil) && !NeutralEvilSettings.NEHasImpVision) ||
                    (x.Is(Alignment.NeutralHarb) && !NeutralHarbingerSettings.NHHasImpVision)));
            }
            else if (id == LayerEnum.Underdog)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate));
            else if (Tasked.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.CanDoTasks());
            else if (id == LayerEnum.Tunneler)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is(LayerEnum.Engineer));
            else if (id == LayerEnum.ButtonBarry)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Actor) && !Actor.ActorButton) ||
                    (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton && x.Is(LayerEnum.Monarch)) ||
                    (!Dictator.DictatorButton && x.Is(LayerEnum.Dictator))));
            }
            else if (id == LayerEnum.Politician)
                assigned = playerList.FirstOrDefault(x => !(x.Is(Alignment.NeutralEvil) || x.Is(Alignment.NeutralBen) || x.Is(Alignment.NeutralNeo)));
            else if (GlobalAb.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Ruthless)
            {
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) && !x.Is(LayerEnum.Juggernaut)) ||
                    x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted));
            }

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllAbilities.Shuffle();

                if (!assigned.GetAbility())
                    Gen(assigned, id, PlayerLayerEnum.Ability);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Abilities in the game: " + ids);
        }

        Message("Abilities Done");
    }

    private static void GenDispositions()
    {
        for (var i = 107; i < 118; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                var num = spawn.Count * (layer is LayerEnum.Rivals or LayerEnum.Lovers or LayerEnum.Linked ? 2 : 1);

                while (num > 0)
                {
                    AllDispositions.Add(spawn);
                    num--;
                }
            }

            Info($"{spawn} Done");
        }

        int maxObj = DispositionsSettings.MaxDispositions;
        int minObj = DispositionsSettings.MinDispositions;

        while (maxObj > AllPlayers().Count)
            maxObj--;

        while (minObj > AllPlayers().Count)
            minObj--;

        AllDispositions = Sort(AllDispositions, GameModeSettings.IgnoreLayerCaps ? AllPlayers().Count : URandom.RandomRangeInt(minObj, maxObj + 1));

        var playerList = AllPlayers().Where(x => x != PureCrew).ToList();
        playerList.Shuffle();

        AllDispositions = [ .. AllDispositions.OrderByDescending(x => x.Chance) ];

        while (AllDispositions.Count > AllPlayers().Count && AllDispositions.Count > 1)
            AllDispositions.Remove(AllDispositions.Last());

        AllDispositions.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllDispositions)
                ids += $" {spawn.ID}";

            Message("Dispositions in the game: " + ids);
        }

        var invalid = new List<LayerEnum>();

        while (playerList.Any() && AllDispositions.Any())
        {
            var id = AllDispositions.TakeFirst().ID;
            PlayerControl assigned = null;

            if (LoverRival.Contains(id) && playerList.Count > 1)
                assigned = playerList.FirstOrDefault(x => x.GetRole().Type is not (LayerEnum.Altruist or LayerEnum.Troll or LayerEnum.Actor or LayerEnum.Jester or LayerEnum.Shifter));
            else if (CrewObj.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (NeutralObj.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Neutral));
            else if (id == LayerEnum.Allied)
                assigned = playerList.FirstOrDefault(x => x.Is(Alignment.NeutralKill));
            else if (id == LayerEnum.Mafia && playerList.Count > 1)
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Defector && playerList.Count > 1)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate));

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllDispositions.Shuffle();

                if (!assigned.GetDisposition())
                    Gen(assigned, id, PlayerLayerEnum.Disposition);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Dispositions in the game: " + ids);
        }

        Message("Dispositions Done");
    }

    private static void GenModifiers()
    {
        for (var i = 92; i < 106; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                var num = spawn.Count;

                while (num > 0)
                {
                    AllModifiers.Add(spawn);
                    num--;
                }
            }

            Info($"{layer} Done");
        }

        int maxMod = ModifiersSettings.MaxModifiers;
        int minMod = ModifiersSettings.MinModifiers;

        while (maxMod > AllPlayers().Count)
            maxMod--;

        while (minMod > AllPlayers().Count)
            minMod--;

        AllModifiers = Sort(AllModifiers, GameModeSettings.IgnoreLayerCaps ? AllPlayers().Count : URandom.RandomRangeInt(minMod, maxMod + 1));

        var playerList = AllPlayers();
        playerList.Shuffle();

        AllModifiers = [ .. AllModifiers.OrderByDescending(x => x.Chance) ];

        while (AllModifiers.Count > AllPlayers().Count && AllModifiers.Count > 1)
            AllModifiers.Remove(AllModifiers.Last());

        AllModifiers.Shuffle();

        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllModifiers)
                ids += $" {spawn.ID}";

            Message("Modifiers in the game: " + ids);
        }

        while (playerList.Any() && AllModifiers.Any())
        {
            var id = AllModifiers.TakeFirst().ID;
            PlayerControl assigned = null;

            if (id == LayerEnum.Bait)
            {
                assigned = playerList.FirstOrDefault(x => !(x.Is(LayerEnum.Vigilante) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Thief) || x.Is(LayerEnum.Altruist) ||
                    x.Is(LayerEnum.Troll)));
            }
            else if (id == LayerEnum.Diseased)
                assigned = playerList.FirstOrDefault(x => !(x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll)));
            else if (id == LayerEnum.Professional)
                assigned = playerList.FirstOrDefault(x => x.Is(LayerEnum.Bullseye) || x.Is(LayerEnum.Slayer) || x.Is(LayerEnum.Hitman) || x.Is(LayerEnum.Sniper));
            else if (GlobalMod.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Shy)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Swapper) && !Swapper.SwapperButton) ||
                    (x.Is(LayerEnum.Actor) && !Actor.ActorButton) || (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) && !Executioner.ExecutionerButton) || (x.Is(LayerEnum.Politician)
                    && !Politician.PoliticianButton) ||  x.Is(LayerEnum.ButtonBarry) || (!Dictator.DictatorButton && x.Is(LayerEnum.Dictator)) || (!Monarch.MonarchButton && x.Is(LayerEnum.Monarch))));
            }

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllModifiers.Shuffle();

                if (!assigned.GetModifier())
                    Gen(assigned, id, PlayerLayerEnum.Modifier);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Modifiers in the game: " + ids);
        }

        Message("Modifiers Done");
    }

    private static void SetTargets()
    {
        if (GetSpawnItem(LayerEnum.Allied).IsActive())
        {
            foreach (var ally in PlayerLayer.GetLayers<Allied>())
            {
                var alliedRole = ally.Player.GetRole();
                var factions = new byte[] { 1, 3, 0 };
                var faction = Allied.AlliedFaction == AlliedFaction.Random ? factions.Random() : factions[(int)Allied.AlliedFaction - 1];
                alliedRole.FactionColor = faction switch
                {
                    0 => CustomColorManager.Crew,
                    1 => CustomColorManager.Intruder,
                    3 => CustomColorManager.Syndicate,
                    _ => CustomColorManager.Neutral,
                };
                ally.Side = alliedRole.Faction = (Faction)faction;
                alliedRole.Alignment = alliedRole.Alignment.GetNewAlignment(ally.Side);
                ally.Player.SetImpostor(ally.Side is Faction.Intruder or Faction.Syndicate);
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ally, faction);
            }

            Message("Allied Faction Set Done");
        }

        if (GetSpawnItem(LayerEnum.Lovers).IsActive())
        {
            var lovers = PlayerLayer.GetLayers<Lovers>();
            lovers.Shuffle();

            for (var i = 0; i < lovers.Count - 1; i += 2)
            {
                var lover = lovers[i];

                if (lover.OtherLover)
                    continue;

                var other = lovers[i + 1];
                lover.OtherLover = other.Player;
                other.OtherLover = lover.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, lover, other);

                if (TownOfUsReworked.IsTest)
                    Message($"Lovers = {lover.PlayerName} & {other.PlayerName}");
            }

            foreach (var lover in lovers)
            {
                if (!lover.OtherLover)
                    NullLayer(lover.Player, PlayerLayerEnum.Disposition);
            }

            Message("Lovers Set");
        }

        if (GetSpawnItem(LayerEnum.Rivals).IsActive())
        {
            var rivals = PlayerLayer.GetLayers<Rivals>();
            rivals.Shuffle();

            for (var i = 0; i < rivals.Count - 1; i += 2)
            {
                var rival = rivals[i];

                if (rival.OtherRival)
                    continue;

                var other = rivals[i + 1];
                rival.OtherRival = other.Player;
                other.OtherRival = rival.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, rival, other);

                if (TownOfUsReworked.IsTest)
                    Message($"Rivals = {rival.PlayerName} & {other.PlayerName}");
            }

            foreach (var rival in rivals)
            {
                if (!rival.OtherRival)
                    NullLayer(rival.Player, PlayerLayerEnum.Disposition);
            }

            Message("Rivals Set");
        }

        if (GetSpawnItem(LayerEnum.Linked).IsActive())
        {
            var linked = PlayerLayer.GetLayers<Linked>();
            linked.Shuffle();

            for (var i = 0; i < linked.Count - 1; i += 2)
            {
                var link = linked[i];

                if (link.OtherLink)
                    continue;

                var other = linked[i + 1];
                link.OtherLink = other.Player;
                other.OtherLink = link.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, link, other);

                if (TownOfUsReworked.IsTest)
                    Message($"Linked = {link.PlayerName} & {other.PlayerName}");
            }

            foreach (var link in linked)
            {
                if (!link.OtherLink)
                    NullLayer(link.Player, PlayerLayerEnum.Disposition);
            }

            Message("Linked Set");
        }

        if (GetSpawnItem(LayerEnum.Mafia).IsActive())
        {
            if (PlayerLayer.GetLayers<Mafia>().Count == 1)
            {
                foreach (var player in AllPlayers().Where(x => x.Is(LayerEnum.Mafia)))
                    NullLayer(player, PlayerLayerEnum.Disposition);
            }

            Message("Mafia Set");
        }

        if (!Executioner.ExecutionerCanPickTargets && GetSpawnItem(LayerEnum.Executioner).IsActive())
        {
            foreach (var exe in PlayerLayer.GetLayers<Executioner>())
            {
                exe.TargetPlayer = AllPlayers().Random(x => x != exe.Player && !x.IsLinkedTo(exe.Player) && !x.Is(Alignment.CrewSov));

                if (exe.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, exe, exe.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        Message($"Exe Target = {exe.TargetPlayer?.name}");
                }
            }

            Message("Exe Targets Set");
        }

        if (!Guesser.GuesserCanPickTargets && GetSpawnItem(LayerEnum.Guesser).IsActive())
        {
            foreach (var guess in PlayerLayer.GetLayers<Guesser>())
            {
                guess.TargetPlayer = AllPlayers().Random(x => x != guess.Player && !x.IsLinkedTo(guess.Player) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.CrewInvest) && !x.Is(LayerEnum.Indomitable));

                if (guess.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, guess, guess.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        Message($"Guess Target = {guess.TargetPlayer?.name}");
                }
            }

            Message("Guess Targets Set");
        }

        if (!GuardianAngel.GuardianAngelCanPickTargets && GetSpawnItem(LayerEnum.GuardianAngel).IsActive())
        {
            foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
            {
                ga.TargetPlayer = AllPlayers().Random(x => x != ga.Player && !x.IsLinkedTo(ga.Player) && !x.Is(Alignment.NeutralEvil));

                if (ga.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ga, ga.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        Message($"GA Target = {ga.TargetPlayer?.name}");
                }
            }

            Message("GA Target Set");
        }

        if (!BountyHunter.BountyHunterCanPickTargets && GetSpawnItem(LayerEnum.BountyHunter).IsActive())
        {
            foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
            {
                bh.TargetPlayer = AllPlayers().Random(x => x != bh.Player && !bh.Player.IsLinkedTo(x));

                if (bh.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, bh, bh.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        Message($"BH Target = {bh.TargetPlayer?.name}");
                }
            }

            Message("BH Targets Set");
        }

        if (!Actor.ActorCanPickRole && GetSpawnItem(LayerEnum.Actor).IsActive())
        {
            foreach (var act in PlayerLayer.GetLayers<Actor>())
            {
                act.FillRoles(AllPlayers().Random(x => x != act.Player));
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, act, act.PretendRoles);

                if (TownOfUsReworked.IsTest)
                    Message($"Act Targets = {act.PretendListString()}");
            }

            Message("Act Variables Set");
        }

        if (GetSpawnItem(LayerEnum.Jackal).IsActive())
        {
            foreach (var jackal in PlayerLayer.GetLayers<Jackal>())
            {
                jackal.Recruit1 = AllPlayers().Random(x => !x.Is(Alignment.NeutralNeo) && x.Is(SubFaction.None) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.NeutralBen));

                if (jackal.Recruit1)
                {
                    jackal.Recruit2 = AllPlayers().Random(x => !x.Is(Alignment.NeutralNeo) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.NeutralBen) && x.Is(SubFaction.None) &&
                        jackal.Recruit1.GetFaction() != x.GetFaction());
                }

                if (jackal.Recruit1)
                    RpcConvert(jackal.Recruit1.PlayerId, jackal.PlayerId, SubFaction.Cabal);

                if (jackal.Recruit2)
                    RpcConvert(jackal.Recruit2.PlayerId, jackal.PlayerId, SubFaction.Cabal);

                if (TownOfUsReworked.IsTest)
                    Message($"Recruits = {jackal.Recruit1?.name} & {jackal.Recruit2?.name}");
            }

            Message("Jackal Recruits Set");
        }

        Message("Targets Set");
    }

    public static void ResetEverything()
    {
        WinState = WinLose.None;

        Role.SyndicateHasChaosDrive = false;
        Role.ChaosDriveMeetingTimerCount = 0;
        Role.DriveHolder = null;

        Role.Cleaned.Clear();

        MeetingPatches.MeetingCount = 0;

        KilledPlayers.Clear();

        PlayerHandler.Instance.PlayerNames.Clear();
        PlayerHandler.Instance.ColorNames.Clear();

        DragHandler.Instance.Dragging.Clear();

        Monos.Range.AllItems.Clear();

        PlayerLayer.DeleteAll();

        SetPostmortals.AssassinatedPlayers.Clear();
        SetPostmortals.MisfiredPlayers.Clear();
        SetPostmortals.MarkedPlayers.Clear();
        SetPostmortals.EscapedPlayers.Clear();

        AllRoles.Clear();
        AllModifiers.Clear();
        AllAbilities.Clear();
        AllDispositions.Clear();

        CrewAuditorRoles.Clear();
        CrewInvestigativeRoles.Clear();
        CrewKillingRoles.Clear();
        CrewProtectiveRoles.Clear();
        CrewSovereignRoles.Clear();
        CrewSupportRoles.Clear();
        CrewRoles.Clear();

        NeutralEvilRoles.Clear();
        NeutralBenignRoles.Clear();
        NeutralKillingRoles.Clear();
        NeutralNeophyteRoles.Clear();
        NeutralHarbingerRoles.Clear();
        NeutralRoles.Clear();

        IntruderHeadRoles.Clear();
        IntruderKillingRoles.Clear();
        IntruderSupportRoles.Clear();
        IntruderDeceptionRoles.Clear();
        IntruderConcealingRoles.Clear();
        IntruderRoles.Clear();

        SyndicateDisruptionRoles.Clear();
        SyndicateKillingRoles.Clear();
        SyndicateSupportRoles.Clear();
        SyndicatePowerRoles.Clear();
        SyndicateRoles.Clear();

        SetPostmortals.PhantomOn = false;
        SetPostmortals.RevealerOn = false;
        SetPostmortals.BansheeOn = false;
        SetPostmortals.GhoulOn = false;

        SetPostmortals.WillBeBanshees.Clear();
        SetPostmortals.WillBeGhouls.Clear();
        SetPostmortals.WillBeRevealers.Clear();
        SetPostmortals.WillBePhantoms.Clear();

        PureCrew = null;
        Convertible = 0;

        RecentlyKilled.Clear();

        DisconnectHandler.Disconnected.Clear();

        SettingsPatches.SettingsPage = 0;

        Assassin.RemainingKills = Assassin.AssassinKills == 0 ? 10000 : Assassin.AssassinKills;

        OnGameEndPatches.Disconnected.Clear();

        Footprint.OddEven.Clear();

        CachedFirstDead = FirstDead;
        FirstDead = null;

        // Role.IsLeft = false;

        CustomMeeting.DestroyAll();
        CustomArrow.DestroyAll();
        CustomMenu.DestroyAll();
        DestroyAll();

        Ash.DestroyAll();

        ClientHandler.CloseMenus();

        BodyLocations.Clear();

        CachedMorphs.Clear();

        TransitioningSize.Clear();

        TransitioningSpeed.Clear();

        UninteractiblePlayers.Clear();

        BetterAirship.SpawnPoints.Clear();
    }

    public static void BeginRoleGen()
    {
        if (IsHnS() || !AmongUsClient.Instance.AmHost)
            return;

        Message("RPC SET ROLE");
        Message($"Current Mode = {GameModeSettings.GameMode}");
        ResetEverything();
        CallRpc(CustomRPC.Misc, MiscRPC.Start);
        Message("Cleared Variables");
        Message("Role Gen Start");

        if (IsKilling())
            GenKilling();
        else if (IsVanilla())
            GenVanilla();
        else if (IsRoleList())
            GenRoleList();
        else if (IsTaskRace())
            GenTaskRace();
        else if (IsCustomHnS())
            GenHideAndSeek();
        else
            GenClassicCustomAA();

        var players = AllPlayers();

        if (!AllRoles.Any(x => x.ID == LayerEnum.Dracula))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.VampireHunter);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Mystic);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Seer));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.VampireHunter or LayerEnum.Amnesiac or LayerEnum.Thief or LayerEnum.Godfather or LayerEnum.Shifter or LayerEnum.Guesser or LayerEnum.Rebel
            or LayerEnum.Executioner or LayerEnum.GuardianAngel or LayerEnum.BountyHunter or LayerEnum.Mystic or LayerEnum.Actor))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Seer);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Sheriff));
        }

        if (AllRoles.Any(x => x.ID == LayerEnum.Cannibal) && AllRoles.Any(x => x.ID == LayerEnum.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var count = AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor));

            for (var i = 0; i < count; i++)
            {
                if (chance == 0)
                    AllRoles.Add(NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GetSpawnItem(LayerEnum.Amnesiac)));
                else
                    AllRoles.Add(IntruderRoles.Random(x => x.ID != LayerEnum.Janitor, GetSpawnItem(LayerEnum.Impostor)));
            }
        }

        if (players.Count <= 4 && AllRoles.Any(x => x.ID == LayerEnum.Amnesiac))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Amnesiac);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Thief));
        }

        AllRoles = [ .. AllRoles.OrderByDescending(x => x.Chance) ];

        while (AllRoles.Count > players.Count && AllRoles.Count > 1)
            AllRoles.Remove(AllRoles.Last());

        AllRoles.Shuffle();
        players.Shuffle();
        Message("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            Message("Roles in the game: " + ids);
        }

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        Message("Role Spawn Done");

        if (IsKilling())
        {
            Role.SyndicateHasChaosDrive = true;
            AssignChaosDrive();
            Message("Assigned Drive");
        }

        PureCrew = AllPlayers().Where(x => x.Is(Faction.Crew)).Random();
        CallRpc(CustomRPC.Misc, MiscRPC.SyncPureCrew, PureCrew);
        Message("Synced Pure Crew");

        if (!IsVanilla())
        {
            if (!IsRoleList() && !IsTaskRace() && !IsCustomHnS())
            {
                if (GameModifiers.EnableDispositions)
                    GenDispositions();

                if (GameModifiers.EnableAbilities)
                    GenAbilities();

                if (GameModifiers.EnableModifiers)
                    GenModifiers();
            }

            Convertible = AllPlayers().Count(x => x.Is(SubFaction.None) && x != PureCrew);
            CallRpc(CustomRPC.Misc, MiscRPC.SyncConvertible, Convertible);
            SetTargets();
        }

        if (MapPatches.CurrentMap == 4)
        {
            BetterAirship.SpawnPoints.AddRange((BetterAirship.EnableCustomSpawns ? CustomSpawns : Spawns).GetRandomRange(3));
            CallRpc(CustomRPC.Misc, MiscRPC.SetSpawnAirship, BetterAirship.SpawnPoints);
        }

        if (TownOfUsReworked.IsTest)
            Message("Name -> Role, Disposition, Modifier, Ability");

        CallRpc(CustomRPC.Misc, MiscRPC.EndRoleGen);

        foreach (var player in AllPlayers())
        {
            var role = player.GetRole() ?? new Roleless().Start<Role>(player);
            var mod = player.GetModifier() ?? new Modifierless().Start<Modifier>(player);
            var ab = player.GetAbility() ?? new Abilityless().Start<Ability>(player);
            var disp = player.GetDisposition() ?? new Dispositionless().Start<Disposition>(player);

            /*PlayerLayer.LayerLookup[player.PlayerId] = [ role, mod, ab, disp ];
            Role.RoleLookup[player.PlayerId] = role;
            Modifier.ModifierLookup[player.PlayerId] = mod;
            Disposition.DispositionLookup[player.PlayerId] = disp;
            Ability.AbilityLookup[player.PlayerId] = ab;*/

            if (TownOfUsReworked.IsTest)
                Message($"{player.name} -> {role}, {disp}, {mod}, {ab}");
        }

        Message("Gen Ended");
    }

    private static void Gen(PlayerControl player, LayerEnum id, PlayerLayerEnum rpc)
    {
        SetLayer(id, rpc).Start(player);
        CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, rpc, player);
    }

    private static void NullLayer(PlayerControl player, PlayerLayerEnum rpc)
    {
        player.GetLayers().Find(x => x.LayerType == rpc)?.Delete();
        Gen(player, LayerEnum.None, rpc);
    }

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

    public static void AssignChaosDrive()
    {
        var all = AllPlayers().Where(x => !x.HasDied() && x.Is(Faction.Syndicate) && x.IsBase(Faction.Syndicate)).ToList();

        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !all.Any())
            return;

        PlayerControl chosen = null;

        if (!Role.DriveHolder || Role.DriveHolder.HasDied())
        {
            if (!all.TryFinding(x => x.Is(LayerEnum.PromotedRebel), out chosen))
                chosen = all.Find(x => x.Is(Alignment.SyndicateDisrup));

            if (!chosen)
                chosen = all.Find(x => x.Is(Alignment.SyndicateSupport));

            if (!chosen)
                chosen = all.Find(x => x.Is(Alignment.SyndicatePower));

            if (!chosen)
                chosen = all.Find(x => x.Is(Alignment.SyndicateKill));

            if (!chosen)
                chosen = all.Find(x => x.GetRole() is Anarchist or Rebel or Sidekick);
        }

        if (chosen)
        {
            Role.DriveHolder = chosen;
            CallRpc(CustomRPC.Misc, MiscRPC.ChaosDrive, chosen);
        }
    }

    public static void Convert(byte target, byte convert, SubFaction sub, bool condition)
    {
        var converted = PlayerById(target);
        var converter = PlayerById(convert);

        if (condition || Convertible <= 0 || PureCrew == converted)
        {
            if (AmongUsClient.Instance.AmHost)
                Interact(converter, converted, true, true);

            return;
        }
        else
        {
            var role1 = converted.GetRole();
            var role2 = converter.GetRole();
            var converts = converted.Is(SubFaction.None);

            if (!converts && !converted.Is(sub))
                Interact(converter, converted, true, true);
            else
            {
                if (converter.Is(LayerEnum.Dracula))
                {
                    if (converts)
                        ((Dracula)role2).Converted.Add(target);
                    else if (converted.IsBitten())
                        ((Dracula)role2).Converted.Add(target);
                    else if (converted.Is(LayerEnum.Dracula))
                    {
                        ((Dracula)role2).Converted.AddRange(((Dracula)role1).Converted);
                        ((Dracula)role1).Converted.AddRange(((Dracula)role2).Converted);
                    }
                }
                else if (converter.Is(LayerEnum.Whisperer))
                {
                    if (converts)
                        ((Whisperer)role2).Persuaded.Add(target);
                    else if (converted.IsPersuaded())
                        ((Whisperer)role2).Persuaded.Add(target);
                    else if (converted.Is(LayerEnum.Whisperer))
                    {
                        ((Whisperer)role2).Persuaded.AddRange(((Whisperer)role1).Persuaded);
                        ((Whisperer)role1).Persuaded.AddRange(((Whisperer)role2).Persuaded);
                        ((Whisperer)role1).Persuaded.ForEach(x => ((Whisperer)role1).PlayerConversion.Remove(x));
                    }

                    ((Whisperer)role2).Persuaded.ForEach(x => ((Whisperer)role2).PlayerConversion.Remove(x));
                }
                else if (converter.Is(LayerEnum.Necromancer))
                {
                    if (converts)
                        ((Necromancer)role2).Resurrected.Add(target);
                    else if (converted.IsResurrected())
                        ((Necromancer)role2).Resurrected.Add(target);
                    else if (converted.Is(LayerEnum.Necromancer))
                    {
                        ((Necromancer)role2).Resurrected.AddRange(((Necromancer)role1).Resurrected);
                        ((Necromancer)role1).Resurrected.AddRange(((Necromancer)role2).Resurrected);
                    }
                }
                else if (converter.Is(LayerEnum.Jackal))
                {
                    if (converts)
                    {
                        var jackal = (Jackal)role2;
                        jackal.Recruited.Add(target);

                        if (!jackal.Recruit1)
                            jackal.Recruit1 = converted;
                        else if (!jackal.Recruit2)
                            jackal.Recruit2 = converted;
                        else if (!jackal.Recruit3)
                            jackal.Recruit3 = converted;
                    }
                    else if (converted.IsRecruit())
                        ((Jackal)role2).Recruited.Add(target);
                    else if (converted.Is(LayerEnum.Jackal))
                    {
                        ((Jackal)role2).Recruited.AddRange(((Jackal)role1).Recruited);
                        ((Jackal)role1).Recruited.AddRange(((Jackal)role2).Recruited);
                    }
                }

                var flash = sub switch
                {
                    SubFaction.Undead => CustomColorManager.Undead,
                    SubFaction.Cabal => CustomColorManager.Cabal,
                    SubFaction.Reanimated => CustomColorManager.Reanimated,
                    SubFaction.Sect => CustomColorManager.Sect,
                    _ => CustomColorManager.SubFaction
                };

                role1.SubFaction = sub;
                role1.SubFactionColor = flash;
                role1.Faction = Faction.Neutral;
                role1.Alignment = role1.Alignment.GetNewAlignment(Faction.Neutral);
                Convertible--;

                if (converted.AmOwner)
                    Flash(flash);
                else if (CustomPlayer.Local.Is(LayerEnum.Mystic))
                    Flash(CustomColorManager.Mystic);
            }
        }
    }

    public static void RpcConvert(byte target, byte convert, SubFaction sub, bool condition = false)
    {
        Convert(target, convert, sub, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, sub, condition);
    }
}