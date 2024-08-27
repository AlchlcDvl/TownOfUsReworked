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
    private static List<RoleOptionData> AllObjectifiers = [];
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

    private static readonly LayerEnum[] NB = [ LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief ];
    private static readonly LayerEnum[] NE = [ LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.Troll ];
    private static readonly LayerEnum[] NN = [ LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer ];
    private static readonly LayerEnum[] NH = [ LayerEnum.Plaguebearer ];
    private static readonly LayerEnum[] NA = [ LayerEnum.Pestilence ];
    private static readonly LayerEnum[] NK = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller,
        LayerEnum.Werewolf ];
    private static readonly LayerEnum[][] Neutral = [ NB, NE, NN, NH, NK ];
    private static readonly LayerEnum[][] RegNeutral = [ NB, NE ];
    private static readonly LayerEnum[][] HarmNeutral = [ NN, NH, NK ];

    private static readonly LayerEnum[] IC = [ LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor ];
    private static readonly LayerEnum[] ID = [ LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith ];
    private static readonly LayerEnum[] IK = [ LayerEnum.Enforcer, LayerEnum.Ambusher ];
    private static readonly LayerEnum[] IS = [ LayerEnum.Consigliere, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort ];
    private static readonly LayerEnum[] IH = [ LayerEnum.Godfather ];
    private static readonly LayerEnum[] IU = [ LayerEnum.Impostor ];
    private static readonly LayerEnum[][] Intruders = [ IC, ID, IK, IS, IU, IH ];
    private static readonly LayerEnum[][] RegIntruders = [ IC, ID, IK, IS ];

    private static readonly LayerEnum[] SSu = [ LayerEnum.Warper, LayerEnum.Stalker ];
    private static readonly LayerEnum[] SD = [ LayerEnum.Timekeeper, LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer] ;
    private static readonly LayerEnum[] SP = [ LayerEnum.Rebel, LayerEnum.Spellslinger ];
    private static readonly LayerEnum[] SyK = [ LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner ];
    private static readonly LayerEnum[] SU = [ LayerEnum.Anarchist ];
    private static readonly LayerEnum[][] Syndicate = [ SSu, SyK, SD, SP, SU ];
    private static readonly LayerEnum[][] RegSyndicate = [ SSu, SyK, SD ];

    private static readonly LayerEnum[] AlignmentEntries = [ LayerEnum.CrewSupport, LayerEnum.CrewInvest, LayerEnum.CrewSov, LayerEnum.CrewProt, LayerEnum.CrewKill, LayerEnum.CrewAudit,
        LayerEnum.IntruderSupport, LayerEnum.IntruderConceal, LayerEnum.IntruderDecep, LayerEnum.IntruderKill, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb, LayerEnum.NeutralBen,
        LayerEnum.NeutralEvil, LayerEnum.NeutralKill, LayerEnum.NeutralNeo, LayerEnum.SyndicateDisrup, LayerEnum.SyndicateKill, LayerEnum.SyndicatePower, LayerEnum.IntruderUtil,
        LayerEnum.CrewUtil, LayerEnum.SyndicateUtil, LayerEnum.IntruderHead ];
    private static readonly LayerEnum[] RandomEntries = [ LayerEnum.RandomCrew, LayerEnum.RandomIntruder, LayerEnum.RandomSyndicate, LayerEnum.RandomNeutral, LayerEnum.RegularCrew,
        LayerEnum.RegularIntruder, LayerEnum.RegularNeutral, LayerEnum.RegularSyndicate, LayerEnum.HarmfulNeutral ];
    private static readonly LayerEnum[][] Alignments = [ CA, CI, CSv, CrP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA ];

    private static readonly LayerEnum[] GlobalMod = [ LayerEnum.Dwarf, LayerEnum.VIP, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral,
        LayerEnum.Indomitable, LayerEnum.Yeller, LayerEnum.Colorblind ];

    private static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    private static readonly LayerEnum[] CrewObj = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    private static readonly LayerEnum[] NeutralObj = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    private static readonly LayerEnum[] CrewAb = [ LayerEnum.Bullseye, LayerEnum.Swapper ];
    private static readonly LayerEnum[] Tasked = [ LayerEnum.Insider, LayerEnum.Multitasker ];
    private static readonly LayerEnum[] GlobalAb = [ LayerEnum.Radar, LayerEnum.Tiebreaker ];

    private static readonly List<byte> Spawns = [ 0, 1, 2, 3, 4, 5, 6 ];

    private static bool Check(RoleOptionData data, bool sorting = false)
    {
        if (IsAA)
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

        if (amount != CustomPlayer.AllPlayers.Count && IsAA)
            amount = CustomPlayer.AllPlayers.Count;
        else if (items.Count < amount)
            amount = items.Count;

        if (IsAA)
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

        newList = [ .. newList.OrderBy(x => 100 - x.Chance) ];

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
        neutrals = IsKilling ? GameModeSettings.NeutralsCount : URandom.RandomRangeInt(NeutralSettings.NeutralMin, NeutralSettings.NeutralMax + 1);

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
            LogInfo($"Crew = {crew}, Int = {impostors}, Syn = {syndicate}, Neut = {neutrals}");

        if (!IsAA)
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
            LogInfo($"Crew = {crew}, Int = {impostors}, Syn = {syndicate}, Neut = {neutrals}");
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

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
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
                NeutralRoles.Add(GetSpawnItem(Pestilence.PestSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));

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

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
    }

    public static bool IsValid(this LayerEnum layer, int? relatedCount = null)
    {
        var result = true;

        if (layer == LayerEnum.Bastion)
            result = GameModifiers.WhoCanVent != WhoCanVentOptions.NoOne;
        else if (layer is LayerEnum.Crewmate or LayerEnum.Impostor or LayerEnum.Anarchist)
            result = IsCustom;
        else if (layer == LayerEnum.VampireHunter)
            result = GetSpawnItem(LayerEnum.Dracula).IsActive();
        else if (layer == LayerEnum.Mystic)
            result = new List<LayerEnum>() { LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Jackal, LayerEnum.Whisperer }.Any(x => GetSpawnItem(x).IsActive());
        else if (layer == LayerEnum.Seer)
        {
            result = new List<LayerEnum>() { LayerEnum.VampireHunter, LayerEnum.BountyHunter, LayerEnum.Godfather, LayerEnum.Rebel, LayerEnum.Plaguebearer, LayerEnum.Mystic,
                LayerEnum.Traitor, LayerEnum.Amnesiac, LayerEnum.Thief, LayerEnum.Executioner, LayerEnum.GuardianAngel, LayerEnum.Guesser, LayerEnum.Shifter }.Any(x =>
                    GetSpawnItem(x).IsActive());
        }
        else if (layer == LayerEnum.Plaguebearer)
            result = !Pestilence.PestSpawn;
        else if (layer == LayerEnum.Pestilence)
            result = Pestilence.PestSpawn;
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
        var num = 0;

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
                    num = spawn.Count;

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

                LogInfo($"{layer} Done");
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
                    num = spawn.Count;

                    while (num > 0)
                    {
                        if (layer is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                            NeutralHarbingerRoles.Add(spawn);
                        else if (layer is LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief)
                            NeutralBenignRoles.Add(spawn);
                        else if (layer is LayerEnum.Actor or LayerEnum.BountyHunter or LayerEnum.Cannibal or LayerEnum.Executioner or LayerEnum.Guesser or LayerEnum.Jester or
                            LayerEnum.Troll)
                        {
                            NeutralEvilRoles.Add(spawn);
                        }
                        else if (layer is LayerEnum.Arsonist or LayerEnum.Cryomaniac or LayerEnum.Glitch or LayerEnum.Juggernaut or LayerEnum.Murderer or LayerEnum.SerialKiller or
                            LayerEnum.Werewolf)
                        {
                            NeutralKillingRoles.Add(spawn);
                        }
                        else if (layer is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer)
                            NeutralNeophyteRoles.Add(spawn);

                        num--;
                    }
                }

                LogInfo($"{layer} Done");
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
                    num = spawn.Count;

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

                LogInfo($"{layer} Done");
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
                    num = spawn.Count;

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

                LogInfo($"{layer} Done");
            }
        }

        if (IsClassic || IsCustom)
        {
            if (!SyndicateSettings.AltImps && imps > 0)
            {
                var maxIC = IntruderConcealingSettings.MaxIC;
                var maxID = IntruderDeceptionSettings.MaxID;
                var maxIK = IntruderKillingSettings.MaxIK;
                var maxIS = IntruderSupportSettings.MaxIS;
                var maxIH = IntruderHeadSettings.MaxIH;
                var minInt = IntruderSettings.IntruderMin;
                var maxInt = IntruderSettings.IntruderMax;

                if (minInt > maxInt)
                    (maxInt, minInt) = (minInt, maxInt);

                var maxIntSum = maxIC + maxID + maxIK + maxIS + maxIH;

                while (maxIntSum > maxInt)
                {
                    switch (URandom.RandomRangeInt(0, 5))
                    {
                        case 0:
                            if (maxIC > 0) maxIC--;
                            break;

                        case 1:
                            if (maxID > 0) maxID--;
                            break;

                        case 2:
                            if (maxIK > 0) maxIK--;
                            break;

                        case 3:
                            if (maxIS > 0) maxIS--;
                            break;

                        case 4:
                            if (maxIH > 0) maxIH--;
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
                var maxSSu = SyndicateSupportSettings.MaxSSu;
                var maxSD = SyndicateDisruptionSettings.MaxSD;
                var maxSyK = SyndicateKillingSettings.MaxSyK;
                var maxSP = SyndicatePowerSettings.MaxSP;
                var minSyn = SyndicateSettings.SyndicateMin;
                var maxSyn = SyndicateSettings.SyndicateMax;

                if (minSyn > maxSyn)
                    (maxSyn, minSyn) = (minSyn, maxSyn);

                var maxSynSum = maxSSu + maxSD + maxSyK + maxSP;

                while (maxSynSum > maxSyn)
                {
                    switch (URandom.RandomRangeInt(0, 4))
                    {
                        case 0:
                            if (maxSSu > 0) maxSSu--;
                            break;

                        case 1:
                            if (maxSD > 0) maxSD--;
                            break;

                        case 2:
                            if (maxSyK > 0) maxSyK--;
                            break;

                        case 3:
                            if (maxSP > 0) maxSP--;
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
                var maxNE = NeutralEvilSettings.MaxNE;
                var maxNB = NeutralBenignSettings.MaxNB;
                var maxNK = NeutralKillingSettings.MaxNK;
                var maxNN = NeutralNeophyteSettings.MaxNN;
                var maxNH = NeutralHarbingerSettings.MaxNE;
                var minNeut = NeutralSettings.NeutralMin;
                var maxNeut = NeutralSettings.NeutralMax;

                if (minNeut > maxNeut)
                    (maxNeut, minNeut) = (minNeut, maxNeut);

                var maxNeutSum = maxNE + maxNB + maxNK + maxNN + maxNH;

                while (maxNeutSum > maxNeut)
                {
                    switch (URandom.RandomRangeInt(0, 5))
                    {
                        case 0:
                            if (maxNE > 0) maxNE--;
                            break;

                        case 1:
                            if (maxNB > 0) maxNB--;
                            break;

                        case 2:
                            if (maxNK > 0) maxNK--;
                            break;

                        case 3:
                            if (maxNN > 0) maxNN--;
                            break;

                        case 4:
                            if (maxNH > 0) maxNH--;
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
                var maxCI = CrewInvestigativeSettings.MaxCI;
                var maxCS = CrewSupportSettings.MaxCS;
                var maxCA = CrewAuditorSettings.MaxCA;
                var maxCK = CrewKillingSettings.MaxCK;
                var maxCrP = CrewProtectiveSettings.MaxCrP;
                var maxCSv = CrewSovereignSettings.MaxCsV;
                var minCrew = CrewSettings.CrewMin;
                var maxCrew = CrewSettings.CrewMax;

                if (minCrew > maxCrew)
                    (maxCrew, minCrew) = (minCrew, maxCrew);

                var maxCrewSum = maxCA + maxCI + maxCK + maxCrP + maxCS + maxCSv;

                while (maxCrewSum > maxCrew)
                {
                    switch (URandom.RandomRangeInt(0, 6))
                    {
                        case 0:
                            if (maxCA > 0) maxCA--;
                            break;

                        case 1:
                            if (maxCI > 0) maxCI--;
                            break;

                        case 2:
                            if (maxCK > 0) maxCK--;
                            break;

                        case 3:
                            if (maxCS > 0) maxCS--;
                            break;

                        case 4:
                            if (maxCSv > 0) maxCSv--;
                            break;

                        case 5:
                            if (maxCrP > 0) maxCrP--;
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

            LogMessage("Classic/Custom Sorting Done");
        }
        else if (IsAA)
        {
            CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);
            IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles, IntruderHeadRoles);
            SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);
            NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

            IntruderRoles = Sort(IntruderRoles, imps);
            CrewRoles = Sort(CrewRoles, crew);
            NeutralRoles = Sort(NeutralRoles, neut);
            SyndicateRoles = Sort(SyndicateRoles, syn);

            LogMessage("All Any Sorting Done");
        }

        CrewRoles.Shuffle();
        SyndicateRoles.Shuffle();
        IntruderRoles.Shuffle();
        NeutralRoles.Shuffle();

        AllRoles.AddRanges(CrewRoles, NeutralRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) /*|| RoleListEntryAttribute.IsBanned(id)*/ || id == LayerEnum.Actor || (id == LayerEnum.Crewmate &&
        GameModeSettings.BanCrewmate) || (id == LayerEnum.Impostor && GameModeSettings.BanImpostor) || (id == LayerEnum.Anarchist && GameModeSettings.BanAnarchist) || (id ==
        LayerEnum.Murderer && GameModeSettings.BanMurderer);

    private static void GenRoleList()
    {
        // var entries = CustomOption.GetOptions<RoleListEntryOption>().Where(x => x.Name.Contains("Entry"));
        // var bans = CustomOption.GetOptions<RoleListEntryOption>().Where(x => x.IsBan);
        // var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get()));
        // var randoms = entries.Where(x => RandomEntries.Contains(x.Get()));
        // var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get())));
        // var anies = entries.Where(x => x.Get() == LayerEnum.Any);
        // // I have no idea what plural for any is lmao

        // SetPostmortals.PhantomOn = GameModeSettings.EnablePhantom;
        // SetPostmortals.RevealerOn = GameModeSettings.EnableRevealer;
        // SetPostmortals.BansheeOn = GameModeSettings.EnableBanshee;
        // SetPostmortals.GhoulOn = GameModeSettings.EnableGhoul;

        // foreach (var entry in roles)
        // {
        //     var ratelimit = 0;
        //     var id = entry.Get();
        //     var cachedCount = AllRoles.Count;

        //     while (cachedCount == AllRoles.Count && ratelimit < 10000)
        //     {
        //         if (CannotAdd(id))
        //             ratelimit++;
        //         else
        //             AllRoles.Add(GetSpawnItem(id));
        //     }
        // }

        // foreach (var entry in alignments)
        // {
        //     var cachedCount = AllRoles.Count;
        //     var id = entry.Get();
        //     var ratelimit = 0;
        //     var random = LayerEnum.None;

        //     while (cachedCount == AllRoles.Count && ratelimit < 10000)
        //     {
        //         if (id == LayerEnum.CrewAudit)
        //             random = CA.Random();
        //         else if (id == LayerEnum.CrewInvest)
        //             random = CI.Random();
        //         else if (id == LayerEnum.CrewSov)
        //             random = CSv.Random();
        //         else if (id == LayerEnum.CrewProt)
        //             random = CrP.Random();
        //         else if (id == LayerEnum.CrewKill)
        //             random = CK.Random();
        //         else if (id == LayerEnum.CrewSupport)
        //             random = CS.Random();
        //         else if (id == LayerEnum.NeutralBen)
        //             random = NB.Random();
        //         else if (id == LayerEnum.NeutralEvil)
        //             random = NE.Random();
        //         else if (id == LayerEnum.NeutralNeo)
        //             random = NN.Random();
        //         else if (id == LayerEnum.NeutralHarb)
        //             random = NH.Random();
        //         else if (id == LayerEnum.NeutralApoc)
        //             random = NA.Random();
        //         else if (id == LayerEnum.NeutralKill)
        //             random = NK.Random();
        //         else if (id == LayerEnum.IntruderConceal)
        //             random = IC.Random();
        //         else if (id == LayerEnum.IntruderDecep)
        //             random = ID.Random();
        //         else if (id == LayerEnum.IntruderKill)
        //             random = IK.Random();
        //         else if (id == LayerEnum.IntruderSupport)
        //             random = IS.Random();
        //         else if (id == LayerEnum.SyndicateSupport)
        //             random = SSu.Random();
        //         else if (id == LayerEnum.SyndicatePower)
        //             random = SP.Random();
        //         else if (id == LayerEnum.SyndicateDisrup)
        //             random = SD.Random();
        //         else if (id == LayerEnum.SyndicateKill)
        //             random = SyK.Random();
        //         else if (id == LayerEnum.CrewUtil)
        //             random = CU.Random();
        //         else if (id == LayerEnum.IntruderUtil)
        //             random = IU.Random();
        //         else if (id == LayerEnum.SyndicateUtil)
        //             random = SU.Random();
        //         else if (id == LayerEnum.IntruderHead)
        //             random = IH.Random();

        //         if (CannotAdd(id))
        //             ratelimit++;
        //         else
        //             AllRoles.Add(GetSpawnItem(random));
        //     }
        // }

        // foreach (var entry in randoms)
        // {
        //     var cachedCount = AllRoles.Count;
        //     var id = entry.Get();
        //     var ratelimit = 0;
        //     var random = LayerEnum.None;

        //     while (cachedCount == AllRoles.Count && ratelimit < 10000)
        //     {
        //         if (id == LayerEnum.RandomCrew)
        //             random = Crew.Random().Random();
        //         else if (id == LayerEnum.RandomNeutral)
        //             random = Neutral.Random().Random();
        //         else if (id == LayerEnum.RandomIntruder)
        //             random = Intruders.Random().Random();
        //         else if (id == LayerEnum.RandomSyndicate)
        //             random = Syndicate.Random().Random();
        //         else if (id == LayerEnum.RegularCrew)
        //             random = RegCrew.Random().Random();
        //         else if (id == LayerEnum.RegularIntruder)
        //             random = RegIntruders.Random().Random();
        //         else if (id == LayerEnum.RegularNeutral)
        //             random = RegNeutral.Random().Random();
        //         else if (id == LayerEnum.HarmfulNeutral)
        //             random = HarmNeutral.Random().Random();
        //         else if (id == LayerEnum.RegularSyndicate)
        //             random = RegSyndicate.Random().Random();

        //         if (CannotAdd(id))
        //             ratelimit++;
        //         else
        //             AllRoles.Add(GetSpawnItem(random));
        //     }
        // }

        // foreach (var entry in anies)
        // {
        //     var cachedCount = AllRoles.Count;
        //     var ratelimit = 0;

        //     while (cachedCount == AllRoles.Count && ratelimit < 10000)
        //     {
        //         var random = Alignments.Random().Random();

        //         if (CannotAdd(random))
        //             ratelimit++;
        //         else
        //             AllRoles.Add(GetSpawnItem(random));
        //     }
        // }

        // // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        // // In case if the ratelimits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        // while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
        //     AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static void GenTaskRace()
    {
        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Runner));
    }

    private static void GenHideAndSeek()
    {
        while (AllRoles.Count < GameModeSettings.HunterCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Hunter));

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
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
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted => new(100, 100, false, false, id),
        _ => OptionAttribute.GetOptions<LayersOptionAttribute>().Find(x => x.Layer == id)?.Get() ?? new(0, 0, false, false, id)
    };

    private static void GenAbilities()
    {
        var num = 0;

        for (var i = 120; i < 137; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                num = spawn.Count;

                while (num > 0)
                {
                    AllAbilities.Add(spawn);
                    num--;
                }
            }

            LogInfo($"{layer} Done");
        }

        var maxAb = AbilitiesSettings.MaxAbilities;
        var minAb = AbilitiesSettings.MinAbilities;

        while (maxAb > CustomPlayer.AllPlayers.Count)
            maxAb--;

        while (minAb > CustomPlayer.AllPlayers.Count)
            minAb--;

        AllAbilities = Sort(AllAbilities, GameModeSettings.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minAb, maxAb + 1));

        var canHaveIntruderAbility = CustomPlayer.AllPlayers;
        var canHaveNeutralAbility = CustomPlayer.AllPlayers;
        var canHaveCrewAbility = CustomPlayer.AllPlayers;
        var canHaveSyndicateAbility = CustomPlayer.AllPlayers;
        var canHaveTunnelerAbility = CustomPlayer.AllPlayers;
        var canHaveSnitch = CustomPlayer.AllPlayers;
        var canHaveTaskedAbility = CustomPlayer.AllPlayers;
        var canHaveTorch = CustomPlayer.AllPlayers;
        var canHaveEvilAbility = CustomPlayer.AllPlayers;
        var canHaveKillingAbility = CustomPlayer.AllPlayers;
        var canHaveRuthless = CustomPlayer.AllPlayers;
        var canHaveAbility = CustomPlayer.AllPlayers;
        var canHaveBB = CustomPlayer.AllPlayers;
        var canHavePolitician = CustomPlayer.AllPlayers;

        canHaveIntruderAbility.RemoveAll(x => !x.Is(Faction.Intruder) || (x.Is(LayerEnum.Consigliere) && Consigliere.ConsigInfo == ConsigInfo.Role));
        canHaveIntruderAbility.Shuffle();

        canHaveNeutralAbility.RemoveAll(x => !(x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) || x.Is(Alignment.NeutralHarb)));
        canHaveNeutralAbility.Shuffle();

        canHaveCrewAbility.RemoveAll(x => !x.Is(Faction.Crew));
        canHaveCrewAbility.Shuffle();

        canHaveSyndicateAbility.RemoveAll(x => !x.Is(Faction.Syndicate));
        canHaveSyndicateAbility.Shuffle();

        canHaveTunnelerAbility.RemoveAll(x => !x.Is(Faction.Crew) || x.Is(LayerEnum.Engineer));
        canHaveTunnelerAbility.Shuffle();

        canHaveSnitch.RemoveAll(x => !x.Is(Faction.Crew) || x.Is(LayerEnum.Traitor));
        canHaveSnitch.Shuffle();

        canHaveTaskedAbility.RemoveAll(x => !x.CanDoTasks());
        canHaveTaskedAbility.Shuffle();

        canHaveTorch.RemoveAll(x => (x.Is(Alignment.NeutralKill) && !NeutralKillingSettings.HasImpVision) || x.Is(Faction.Syndicate) || x.Is(Faction.Intruder) || (x.Is(Faction.Neutral) &&
            !NeutralSettings.LightsAffectNeutrals));
        canHaveTorch.Shuffle();

        canHaveEvilAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
        canHaveEvilAbility.Shuffle();

        canHaveKillingAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) ||
            x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted)));
        canHaveKillingAbility.Shuffle();

        canHaveRuthless.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) ||
            x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted)) || x.Is(LayerEnum.Juggernaut));
        canHaveRuthless.Shuffle();

        canHaveBB.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Actor) && !Actor.ActorButton) ||
            (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton && x.Is(LayerEnum.Monarch))
            || (!Dictator.DictatorButton && x.Is(LayerEnum.Dictator)));
        canHaveBB.Shuffle();

        canHavePolitician.RemoveAll(x => x.Is(Alignment.NeutralEvil) || x.Is(Alignment.NeutralBen) || x.Is(Alignment.NeutralNeo));
        canHavePolitician.Shuffle();

        AllAbilities = [ .. AllAbilities.OrderBy(x => 100 - x.Chance) ];

        while (AllAbilities.Count > CustomPlayer.AllPlayers.Count && AllAbilities.Count > 1)
            AllAbilities.Remove(AllAbilities.Last());

        AllAbilities.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllAbilities)
                ids += $" {spawn.ID}";

            LogMessage("Abilities in the game: " + ids);
        }

        while (canHaveSnitch.Any() || (GameModifiers.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Any()) || canHaveIntruderAbility.Any() || canHaveTorch.Any() ||
            canHaveNeutralAbility.Any() || canHaveCrewAbility.Any() || canHaveSyndicateAbility.Any() || canHaveAbility.Any() || canHaveEvilAbility.Any() || canHaveKillingAbility.Any() ||
            canHaveTaskedAbility.Any())
        {
            if (AllAbilities.Count == 0)
                break;

            var id = AllAbilities.TakeFirst().ID;
            PlayerControl assigned = null;

            if (canHaveSnitch.Any() && id == LayerEnum.Snitch)
                assigned = canHaveSnitch.TakeFirst();
            else if (canHaveSyndicateAbility.Any() && id == LayerEnum.Sniper)
                assigned = canHaveSyndicateAbility.TakeFirst();
            else if (canHaveCrewAbility.Any() && CrewAb.Contains(id))
                assigned = canHaveCrewAbility.TakeFirst();
            else if (canHaveNeutralAbility.Any() && id == LayerEnum.Slayer)
                assigned = canHaveNeutralAbility.TakeFirst();
            else if (canHaveIntruderAbility.Any() && id == LayerEnum.Hitman)
                assigned = canHaveIntruderAbility.TakeFirst();
            else if (canHaveKillingAbility.Any() && id == LayerEnum.Ninja)
                assigned = canHaveKillingAbility.TakeFirst();
            else if (canHaveTorch.Any() && id == LayerEnum.Torch)
                assigned = canHaveTorch.TakeFirst();
            else if (canHaveEvilAbility.Any() && id == LayerEnum.Underdog)
                assigned = canHaveEvilAbility.TakeFirst();
            else if (canHaveTaskedAbility.Any() && Tasked.Contains(id))
                assigned = canHaveTaskedAbility.TakeFirst();
            else if (canHaveTunnelerAbility.Any() && id == LayerEnum.Tunneler)
                assigned = canHaveTunnelerAbility.TakeFirst();
            else if (canHaveBB.Any() && id == LayerEnum.ButtonBarry)
                assigned = canHaveBB.TakeFirst();
            else if (canHavePolitician.Any() && id == LayerEnum.Politician)
                assigned = canHavePolitician.TakeFirst();
            else if (canHaveAbility.Any() && GlobalAb.Contains(id))
                assigned = canHaveAbility.TakeFirst();
            else if (canHaveRuthless.Any() && id == LayerEnum.Ruthless)
                assigned = canHaveRuthless.TakeFirst();

            if (assigned)
            {
                canHaveSnitch.Remove(assigned);
                canHaveSyndicateAbility.Remove(assigned);
                canHaveCrewAbility.Remove(assigned);
                canHaveNeutralAbility.Remove(assigned);
                canHaveIntruderAbility.Remove(assigned);
                canHaveKillingAbility.Remove(assigned);
                canHaveTorch.Remove(assigned);
                canHaveEvilAbility.Remove(assigned);
                canHaveTaskedAbility.Remove(assigned);
                canHaveTunnelerAbility.Remove(assigned);
                canHaveBB.Remove(assigned);
                canHavePolitician.Remove(assigned);
                canHaveAbility.Remove(assigned);
                canHaveRuthless.Remove(assigned);
                canHaveSnitch.Shuffle();
                canHaveSyndicateAbility.Shuffle();
                canHaveCrewAbility.Shuffle();
                canHaveNeutralAbility.Shuffle();
                canHaveIntruderAbility.Shuffle();
                canHaveKillingAbility.Shuffle();
                canHaveTorch.Shuffle();
                canHaveEvilAbility.Shuffle();
                canHaveTaskedAbility.Shuffle();
                canHaveTunnelerAbility.Shuffle();
                canHaveBB.Shuffle();
                canHavePolitician.Shuffle();
                canHaveAbility.Shuffle();
                canHaveRuthless.Shuffle();
                AllAbilities.Shuffle();

                if (!assigned.GetAbility())
                    Gen(assigned, id, PlayerLayerEnum.Ability);
                else
                    AllAbilities.Add(GetSpawnItem(id));
            }
        }

        LogMessage("Abilities Done");
    }

    private static void GenObjectifiers()
    {
        var num = 0;

        for (var i = 107; i < 118; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                num = spawn.Count * (layer is LayerEnum.Rivals or LayerEnum.Lovers or LayerEnum.Linked ? 2 : 1);

                while (num > 0)
                {
                    AllObjectifiers.Add(spawn);
                    num--;
                }
            }

            LogInfo($"{layer} Done");
        }

        var maxObj = ObjectifiersSettings.MaxObjectifiers;
        var minObj = ObjectifiersSettings.MinObjectifiers;

        while (maxObj > CustomPlayer.AllPlayers.Count)
            maxObj--;

        while (minObj > CustomPlayer.AllPlayers.Count)
            minObj--;

        AllObjectifiers = Sort(AllObjectifiers, GameModeSettings.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minObj, maxObj + 1));

        var canHaveLoverorRival = CustomPlayer.AllPlayers;
        var canHaveNeutralObjectifier = CustomPlayer.AllPlayers;
        var canHaveCrewObjectifier = CustomPlayer.AllPlayers;
        var canHaveAllied = CustomPlayer.AllPlayers;
        var canHaveObjectifier = CustomPlayer.AllPlayers;
        var canHaveDefector = CustomPlayer.AllPlayers;

        canHaveLoverorRival.RemoveAll(x => x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll) || x.Is(LayerEnum.Actor) || x.Is(LayerEnum.Jester) || x.Is(LayerEnum.Shifter) || x == PureCrew);
        canHaveLoverorRival.Shuffle();

        canHaveNeutralObjectifier.RemoveAll(x => !x.Is(Faction.Neutral) || x == PureCrew);
        canHaveNeutralObjectifier.Shuffle();

        canHaveCrewObjectifier.RemoveAll(x => !x.Is(Faction.Crew) || x == PureCrew);
        canHaveCrewObjectifier.Shuffle();

        canHaveAllied.RemoveAll(x => !x.Is(Alignment.NeutralKill) || x == PureCrew);
        canHaveAllied.Shuffle();

        canHaveObjectifier.RemoveAll(x => x == PureCrew);
        canHaveObjectifier.Shuffle();

        canHaveDefector.RemoveAll(x => x == PureCrew || !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
        canHaveDefector.Shuffle();

        AllObjectifiers = [ .. AllObjectifiers.OrderBy(x => 100 - x.Chance) ];

        while (AllObjectifiers.Count > CustomPlayer.AllPlayers.Count && AllObjectifiers.Count > 1)
            AllObjectifiers.Remove(AllObjectifiers.Last());

        AllObjectifiers.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllObjectifiers)
                ids += $" {spawn.ID}";

            LogMessage("Objectifiers in the game: " + ids);
        }

        while (canHaveNeutralObjectifier.Any() || canHaveCrewObjectifier.Any() || canHaveLoverorRival.Count > 1 || canHaveObjectifier.Any() || canHaveDefector.Any())
        {
            if (AllObjectifiers.Count == 0)
                break;

            var id = AllObjectifiers.TakeFirst().ID;
            PlayerControl assigned = null;
            PlayerControl assignedOther = null;

            if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 1)
            {
                assigned = canHaveLoverorRival.TakeFirst();
                assignedOther = canHaveLoverorRival.TakeFirst();
            }
            else if (CrewObj.Contains(id) && canHaveCrewObjectifier.Any())
                assigned = canHaveCrewObjectifier.TakeFirst();
            else if (NeutralObj.Contains(id) && canHaveNeutralObjectifier.Any())
            {
                assigned = canHaveNeutralObjectifier.TakeFirst();

                if (id == LayerEnum.Linked)
                    assignedOther = canHaveNeutralObjectifier.TakeFirst();
            }
            else if (id == LayerEnum.Allied && canHaveAllied.Any())
                assigned = canHaveAllied.TakeFirst();
            else if (id == LayerEnum.Mafia && canHaveObjectifier.Count > 1)
                assigned = canHaveObjectifier.TakeFirst();
            else if (id == LayerEnum.Defector && canHaveDefector.Count > 1)
                assigned = canHaveDefector.TakeFirst();

            if (assigned)
            {
                canHaveLoverorRival.Remove(assigned);
                canHaveCrewObjectifier.Remove(assigned);
                canHaveNeutralObjectifier.Remove(assigned);
                canHaveAllied.Remove(assigned);
                canHaveObjectifier.Remove(assigned);
                canHaveDefector.Remove(assigned);
                canHaveLoverorRival.Shuffle();
                canHaveCrewObjectifier.Shuffle();
                canHaveNeutralObjectifier.Shuffle();
                canHaveAllied.Shuffle();
                canHaveObjectifier.Shuffle();
                canHaveDefector.Shuffle();
                AllObjectifiers.Shuffle();

                if (!assigned.GetObjectifier())
                    Gen(assigned, id, PlayerLayerEnum.Objectifier);
                else
                    AllObjectifiers.Add(GetSpawnItem(id));
            }

            if (assignedOther)
            {
                canHaveLoverorRival.Remove(assignedOther);
                canHaveCrewObjectifier.Remove(assignedOther);
                canHaveNeutralObjectifier.Remove(assignedOther);
                canHaveAllied.Remove(assignedOther);
                canHaveObjectifier.Remove(assignedOther);
                canHaveDefector.Remove(assignedOther);

                if (!assignedOther.GetObjectifier())
                    Gen(assignedOther, id, PlayerLayerEnum.Objectifier);
            }
        }

        LogMessage("Objectifiers Done");
    }

    private static void GenModifiers()
    {
        var num = 0;

        for (var i = 92; i < 106; i++)
        {
            var layer = (LayerEnum)i;
            var spawn = GetSpawnItem(layer);

            if (spawn.IsActive())
            {
                num = spawn.Count;

                while (num > 0)
                {
                    AllModifiers.Add(spawn);
                    num--;
                }
            }

            LogInfo($"{layer} Done");
        }

        var maxMod = ModifiersSettings.MaxModifiers;
        var minMod = ModifiersSettings.MinModifiers;

        while (maxMod > CustomPlayer.AllPlayers.Count)
            maxMod--;

        while (minMod > CustomPlayer.AllPlayers.Count)
            minMod--;

        AllModifiers = Sort(AllModifiers, GameModeSettings.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minMod, maxMod + 1));

        var canHaveBait = CustomPlayer.AllPlayers;
        var canHaveDiseased = CustomPlayer.AllPlayers;
        var canHaveProfessional = CustomPlayer.AllPlayers;
        var canHaveModifier = CustomPlayer.AllPlayers;
        var canHaveShy = CustomPlayer.AllPlayers;

        canHaveBait.RemoveAll(x => x.Is(LayerEnum.Vigilante) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Thief) || x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll));
        canHaveBait.Shuffle();

        canHaveDiseased.RemoveAll(x => x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll));
        canHaveDiseased.Shuffle();

        canHaveProfessional.RemoveAll(x => !(x.Is(LayerEnum.Bullseye) || x.Is(LayerEnum.Slayer) || x.Is(LayerEnum.Hitman) || x.Is(LayerEnum.Sniper)));
        canHaveProfessional.Shuffle();

        canHaveShy.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Swapper) &&
            !Swapper.SwapperButton) || (x.Is(LayerEnum.Actor) && !Actor.ActorButton) || (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) &&
            !Executioner.ExecutionerButton) || x.Is(LayerEnum.ButtonBarry) || (x.Is(LayerEnum.Politician) && !Politician.PoliticianButton) || (!Dictator.DictatorButton &&
            x.Is(LayerEnum.Dictator)) || (!Monarch.MonarchButton && x.Is(LayerEnum.Monarch)));
        canHaveShy.Shuffle();

        AllModifiers = [ .. AllModifiers.OrderBy(x => 100 - x.Chance) ];

        while (AllModifiers.Count > CustomPlayer.AllPlayers.Count && AllModifiers.Count > 1)
            AllModifiers.Remove(AllModifiers.Last());

        AllModifiers.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllModifiers)
                ids += $" {spawn.ID}";

            LogMessage("Modifiers in the game: " + ids);
        }

        while (canHaveBait.Any() || canHaveDiseased.Any() || canHaveProfessional.Any() || canHaveModifier.Any())
        {
            if (AllModifiers.Count == 0)
                break;

            var id = AllModifiers.TakeFirst().ID;
            PlayerControl assigned = null;

            if (canHaveBait.Any() && id == LayerEnum.Bait)
                assigned = canHaveBait.TakeFirst();
            else if (canHaveDiseased.Any() && id == LayerEnum.Diseased)
                assigned = canHaveDiseased.TakeFirst();
            else if (canHaveProfessional.Any() && id == LayerEnum.Professional)
                assigned = canHaveProfessional.TakeFirst();
            else if (canHaveModifier.Any() && GlobalMod.Contains(id))
                assigned = canHaveModifier.TakeFirst();
            else if (canHaveShy.Any() && id == LayerEnum.Shy)
                assigned = canHaveShy.TakeFirst();

            if (assigned)
            {
                canHaveBait.Remove(assigned);
                canHaveDiseased.Remove(assigned);
                canHaveProfessional.Remove(assigned);
                canHaveModifier.Remove(assigned);
                canHaveShy.Remove(assigned);
                canHaveBait.Shuffle();
                canHaveDiseased.Shuffle();
                canHaveProfessional.Shuffle();
                canHaveModifier.Shuffle();
                canHaveShy.Shuffle();
                AllModifiers.Shuffle();

                if (!assigned.GetModifier())
                    Gen(assigned, id, PlayerLayerEnum.Modifier);
                else
                    AllModifiers.Add(GetSpawnItem(id));
            }
        }

        LogMessage("Modifiers Done");
    }

    private static void SetTargets()
    {
        foreach (var ally in PlayerLayer.GetLayers<Allied>())
        {
            var alliedRole = ally.Player.GetRole();
            var crew = Allied.AlliedFaction == AlliedFaction.Crew;
            var intr = Allied.AlliedFaction == AlliedFaction.Intruder;
            var syn = Allied.AlliedFaction == AlliedFaction.Syndicate;
            var factions = new List<byte>() { 1, 3, 0 };
            byte faction;

            if (Allied.AlliedFaction == AlliedFaction.Random)
            {
                faction = factions.Random();
                intr = faction == 1;
                syn = faction == 3;
                crew = faction == 0;
            }
            else
                faction = factions[(int)Allied.AlliedFaction - 1];

            if (crew)
                alliedRole.FactionColor = CustomColorManager.Crew;
            else if (intr)
                alliedRole.FactionColor = CustomColorManager.Intruder;
            else if (syn)
                alliedRole.FactionColor = CustomColorManager.Syndicate;

            ally.Side = alliedRole.Faction = (Faction)faction;
            alliedRole.Alignment = alliedRole.Alignment.GetNewAlignment((Faction)faction);
            ally.Player.SetImpostor((Faction)faction is Faction.Intruder or Faction.Syndicate);
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ally, faction);
        }

        LogMessage("Allied Faction Set Done");

        var lovers = PlayerLayer.GetLayers<Lovers>();
        lovers.Shuffle();

        for (var i = 0; i < lovers.Count; i++)
        {
            var lover = lovers[i];

            if (lover.OtherLover)
                continue;

            var other = lovers[i + 1];
            lover.OtherLover = other.Player;
            other.OtherLover = lover.Player;
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, lover, other);

            if (TownOfUsReworked.IsTest)
                LogMessage($"Lovers = {lover.PlayerName} & {other.PlayerName}");
        }

        foreach (var lover in lovers)
        {
            if (!lover.OtherLover)
                NullLayer(lover.Player, PlayerLayerEnum.Objectifier);
        }

        LogMessage("Lovers Set");

        var rivals = PlayerLayer.GetLayers<Rivals>();
        rivals.Shuffle();

        for (var i = 0; i < rivals.Count; i++)
        {
            var rival = rivals[i];

            if (rival.OtherRival)
                continue;

            var other = rivals[i + 1];
            rival.OtherRival = other.Player;
            other.OtherRival = rival.Player;
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, rival, other);

            if (TownOfUsReworked.IsTest)
                LogMessage($"Rivals = {rival.PlayerName} & {other.PlayerName}");
        }

        foreach (var rival in rivals)
        {
            if (!rival.OtherRival)
                NullLayer(rival.Player, PlayerLayerEnum.Objectifier);
        }

        LogMessage("Rivals Set");

        var linked = PlayerLayer.GetLayers<Linked>();
        linked.Shuffle();

        for (var i = 0; i < linked.Count; i++)
        {
            var link = linked[i];

            if (link.OtherLink)
                continue;

            var other = linked[i + 1];
            link.OtherLink = other.Player;
            other.OtherLink = link.Player;
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, link, other);

            if (TownOfUsReworked.IsTest)
                LogMessage($"Linked = {link.PlayerName} & {other.PlayerName}");
        }

        foreach (var link in linked)
        {
            if (!link.OtherLink)
                NullLayer(link.Player, PlayerLayerEnum.Objectifier);
        }

        LogMessage("Linked Set");

        if (PlayerLayer.GetLayers<Mafia>().Count == 1)
        {
            foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(LayerEnum.Mafia)))
                NullLayer(player, PlayerLayerEnum.Objectifier);
        }

        LogMessage("Mafia Set");

        if (!Executioner.ExecutionerCanPickTargets)
        {
            foreach (var exe in PlayerLayer.GetLayers<Executioner>())
            {
                exe.TargetPlayer = null;
                var ratelimit = 0;

                while ((!exe.TargetPlayer || exe.TargetPlayer == exe.Player || exe.TargetPlayer.IsLinkedTo(exe.Player) || exe.TargetPlayer.Is(Alignment.CrewSov)) && ratelimit < 10000)
                {
                    exe.TargetPlayer = CustomPlayer.AllPlayers.Random();
                    ratelimit++;
                }

                if (exe.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, exe, exe.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"Exe Target = {exe.TargetPlayer?.name}");
                }
            }

            LogMessage("Exe Targets Set");
        }

        if (Guesser.GuesserCanPickTargets)
        {
            foreach (var guess in PlayerLayer.GetLayers<Guesser>())
            {
                guess.TargetPlayer = CustomPlayer.AllPlayers.Random(x => x == guess.Player && !x.IsLinkedTo(guess.Player) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.CrewInvest) &&
                    !x.Is(LayerEnum.Indomitable));

                if (guess.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, guess, guess.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"Guess Target = {guess.TargetPlayer?.name}");
                }
            }

            LogMessage("Guess Targets Set");
        }

        if (GuardianAngel.GuardianAngelCanPickTargets)
        {
            foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
            {
                ga.TargetPlayer = CustomPlayer.AllPlayers.Random(x => x == ga.Player && !x.IsLinkedTo(ga.Player) && !x.Is(Alignment.NeutralEvil));

                if (ga.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ga, ga.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"GA Target = {ga.TargetPlayer?.name}");
                }
            }

            LogMessage("GA Target Set");
        }

        if (BountyHunter.BountyHunterCanPickTargets)
        {
            foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
            {
                bh.TargetPlayer = CustomPlayer.AllPlayers.Random(x => x != bh.Player && !bh.Player.IsLinkedTo(x));

                if (bh.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, bh, bh.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"BH Target = {bh.TargetPlayer?.name}");
                }
            }

            LogMessage("BH Targets Set");
        }

        if (Actor.ActorCanPickRole)
        {
            foreach (var act in PlayerLayer.GetLayers<Actor>())
            {
                act.FillRoles(CustomPlayer.AllPlayers.Random(x => x != act.Player));
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, act, act.PretendRoles);

                if (TownOfUsReworked.IsTest)
                    LogMessage($"Act Targets = {act.PretendListString()}");
            }

            LogMessage("Act Variables Set");
        }

        foreach (var jackal in PlayerLayer.GetLayers<Jackal>())
        {
            jackal.Recruit1 = CustomPlayer.AllPlayers.Random(x => !x.Is(Alignment.NeutralNeo) && !x.Is(SubFaction.Cabal) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.NeutralBen));

            if (jackal.Recruit1)
            {
                jackal.Recruit2 = CustomPlayer.AllPlayers.Random(x => !x.Is(Alignment.NeutralNeo) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.NeutralBen) &&
                    !x.Is(SubFaction.Cabal) && jackal.Recruit1.GetFaction() != x.GetFaction());
            }

            if (jackal.Recruit1)
                RpcConvert(jackal.Recruit1.PlayerId, jackal.PlayerId, SubFaction.Cabal);

            if (jackal.Recruit2)
                RpcConvert(jackal.Recruit2.PlayerId, jackal.PlayerId, SubFaction.Cabal);

            if (TownOfUsReworked.IsTest)
                LogMessage($"Recruits = {jackal.Recruit1?.name} (Good) & {jackal.Recruit2?.name} (Evil)");
        }

        LogMessage("Jackal Recruits Set");

        LogMessage("Targets Set");
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

        PlayerLayer.DeleteAll();

        SetPostmortals.AssassinatedPlayers.Clear();
        SetPostmortals.MisfiredPlayers.Clear();
        SetPostmortals.MarkedPlayers.Clear();
        SetPostmortals.EscapedPlayers.Clear();

        AllRoles.Clear();
        AllModifiers.Clear();
        AllAbilities.Clear();
        AllObjectifiers.Clear();

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

        Assassin.RemainingKills = Assassin.AssassinKills;

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
        Objects.Range.DestroyAll();

        // ClientStuff.CloseMenus();

        BodyLocations.Clear();

        CachedMorphs.Clear();

        TransitioningSize.Clear();

        TransitioningSpeed.Clear();

        UninteractiblePlayers.Clear();

        BetterAirship.SpawnPoints.Clear();
    }

    public static void BeginRoleGen()
    {
        if (IsHnS || !AmongUsClient.Instance.AmHost)
            return;

        LogMessage("RPC SET ROLE");
        LogMessage($"Current Mode = {GameModeSettings.GameMode}");
        ResetEverything();
        CallRpc(CustomRPC.Misc, MiscRPC.Start);
        LogMessage("Cleared Variables");
        LogMessage("Role Gen Start");

        if (IsKilling)
            GenKilling();
        else if (IsVanilla)
            GenVanilla();
        else if (IsRoleList)
            GenRoleList();
        else if (IsTaskRace)
            GenTaskRace();
        else if (IsCustomHnS)
            GenHideAndSeek();
        else
            GenClassicCustomAA();

        var players = CustomPlayer.AllPlayers;

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

        AllRoles = [ .. AllRoles.OrderBy(x => x.Chance) ];

        while (AllRoles.Count > players.Count && AllRoles.Count > 1)
            AllRoles.Remove(AllRoles.First());

        AllRoles.Shuffle();
        players.Shuffle();
        LogMessage("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            LogMessage("Roles in the game: " + ids);
        }

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        LogMessage("Role Spawn Done");

        if (IsKilling)
        {
            Role.SyndicateHasChaosDrive = true;
            AssignChaosDrive();
            LogMessage("Assigned Drive");
        }

        PureCrew = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew)).Random();
        CallRpc(CustomRPC.Misc, MiscRPC.SyncPureCrew, PureCrew);
        LogMessage("Synced Pure Crew");

        if (!IsVanilla)
        {
            if (!IsRoleList && !IsTaskRace && !IsCustomHnS)
            {
                if (GameModifiers.EnableObjectifiers)
                    GenObjectifiers();

                if (GameModifiers.EnableAbilities)
                    GenAbilities();

                if (GameModifiers.EnableModifiers)
                    GenModifiers();
            }

            Convertible = CustomPlayer.AllPlayers.Count(x => x.Is(SubFaction.None) && x != PureCrew);
            CallRpc(CustomRPC.Misc, MiscRPC.SyncConvertible, Convertible);
            SetTargets();
        }

        if (MapPatches.CurrentMap == 4)
        {
            BetterAirship.SpawnPoints.AddRange(Spawns.GetRandomRange(3));
            CallRpc(CustomRPC.Misc, MiscRPC.SetSpawnAirship, BetterAirship.SpawnPoints);
        }

        if (TownOfUsReworked.IsTest)
            LogMessage("Name -> Role, Objectifier, Modifier, Ability");

        CallRpc(CustomRPC.Misc, MiscRPC.EndRoleGen);

        foreach (var player in CustomPlayer.AllPlayers)
        {
            var role = player.GetRole() ?? new Roleless().Start<Role>(player);
            var mod = player.GetModifier() ?? new Modifierless().Start<Modifier>(player);
            var ab = player.GetAbility() ?? new Abilityless().Start<Ability>(player);
            var obj = player.GetObjectifier() ?? new Objectifierless().Start<Objectifier>(player);

            /*PlayerLayer.LayerLookup[player.PlayerId] = [ role, mod, ab, obj ];
            Role.RoleLookup[player.PlayerId] = role;
            Modifier.ModifierLookup[player.PlayerId] = mod;
            Objectifier.ObjectifierLookup[player.PlayerId] = obj;
            Ability.AbilityLookup[player.PlayerId] = ab;*/

            if (TownOfUsReworked.IsTest)
                LogMessage($"{player.name} -> {role}, {obj}, {mod}, {ab}");
        }

        LogMessage("Gen Ended");
    }

    private static void Gen(PlayerControl player, LayerEnum id, PlayerLayerEnum rpc)
    {
        SetLayer(id, rpc).Start(player);
        CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, rpc, player);
    }

    private static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, LayerEnum.None, rpc);

    public static PlayerLayer SetLayer(LayerEnum id, PlayerLayerEnum rpc)
    {
        if (LayerDictionary.TryGetValue(id, out var dictEntry))
            return (PlayerLayer)Activator.CreateInstance(dictEntry.LayerType);
        else
            return rpc switch
            {
                PlayerLayerEnum.Role => new Roleless(),
                PlayerLayerEnum.Modifier => new Modifierless(),
                PlayerLayerEnum.Objectifier => new Objectifierless(),
                PlayerLayerEnum.Ability => new Abilityless(),
                _ => throw new NotImplementedException(id.ToString())
            };
    }

    public static void AssignChaosDrive()
    {
        var all = CustomPlayer.AllPlayers.Where(x => !x.HasDied() && x.Is(Faction.Syndicate) && x.IsBase(Faction.Syndicate)).ToList();

        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !all.Any())
            return;

        PlayerControl chosen = null;

        if (!Role.DriveHolder || Role.DriveHolder.HasDied())
        {
            chosen = all.Find(x => x.Is(LayerEnum.PromotedRebel));

            if (!chosen)
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
                role1.Alignment = role1.Alignment.GetNewAlignment(Faction.Neutral);
                Convertible--;

                if (CustomPlayer.Local == converted)
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