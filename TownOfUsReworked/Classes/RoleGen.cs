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

    private static bool Check(int probability, bool sorting = false)
    {
        if (probability == 0)
            return false;

        if (probability == 100)
            return !sorting;

        return URandom.RandomRangeInt(1, 100) <= probability;
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

                if (items[0].Unique && CustomGameOptions.EnableUniques)
                    items.Remove(items[0]);
                else
                    rate++;
            }
        }
        else
        {
            var guaranteed = items.Where(x => x.Chance == 100).ToList();
            guaranteed.Shuffle();
            var optionals = items.Where(x => Check(x.Chance, true)).ToList();
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
        impostors = CustomGameOptions.IntruderCount;
        syndicate = CustomGameOptions.SyndicateCount;
        neutrals = IsKilling ? CustomGameOptions.NeutralRoles : URandom.RandomRangeInt(CustomGameOptions.NeutralMin, CustomGameOptions.NeutralMax + 1);

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
        while (AllRoles.Count < (CustomGameOptions.AltImps ? CustomGameOptions.SyndicateCount : CustomGameOptions.IntruderCount))
            AllRoles.Add(GenerateSpawnItem(CustomGameOptions.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));
    }

    private static void GenKilling()
    {
        GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);

        if (imps > 0)
        {
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Enforcer));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Morphling));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Blackmailer));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Miner));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Teleporter));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Wraith));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Consort));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Janitor));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Camouflager));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Grenadier));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Impostor));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Consigliere));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Disguiser));
            IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Ambusher));

            if (imps >= 3)
                IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Godfather));

            IntruderRoles = Sort(IntruderRoles, imps);
        }

        if (syn > 0)
        {
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Anarchist));
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Bomber));
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Poisoner));
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Crusader));
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Collider));

            if (syn >= 3)
                SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Rebel));

            SyndicateRoles = Sort(SyndicateRoles, syn);
        }

        if (neut > 0)
        {
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Glitch));
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Werewolf));
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.SerialKiller));
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Juggernaut));
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Murderer));
            NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Thief));

            if (CustomGameOptions.AddArsonist)
                NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Arsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralRoles.Add(GenerateSpawnItem(LayerEnum.Cryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
                NeutralRoles.Add(GenerateSpawnItem(CustomGameOptions.PestSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));

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
                    CrewRoles.Add(GenerateSpawnItem(LayerEnum.Vigilante));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add(GenerateSpawnItem(LayerEnum.Veteran));
                    vets--;
                }

                if (basts > 0)
                {
                    CrewRoles.Add(GenerateSpawnItem(LayerEnum.Bastion));
                    basts--;
                }
            }

            CrewRoles = Sort(CrewRoles, crew);
        }

        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!CustomGameOptions.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Vigilante));
    }

    private static void GenClassicCustomAA()
    {
        SetPostmortals.PhantomOn = Check(CustomGameOptions.PhantomOn);
        SetPostmortals.RevealerOn = Check(CustomGameOptions.RevealerOn);
        SetPostmortals.BansheeOn = Check(CustomGameOptions.BansheeOn);
        SetPostmortals.GhoulOn = Check(CustomGameOptions.GhoulOn);
        GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);
        var num = 0;

        if (crew > 0)
        {
            if (CustomGameOptions.MayorOn > 0)
            {
                num = CustomGameOptions.MayorCount;

                while (num > 0)
                {
                    CrewSovereignRoles.Add(GenerateSpawnItem(LayerEnum.Mayor));
                    num--;
                }

                LogInfo("Mayor Done");
            }

            if (CustomGameOptions.MonarchOn > 0)
            {
                num = CustomGameOptions.MonarchCount;

                while (num > 0)
                {
                    CrewSovereignRoles.Add(GenerateSpawnItem(LayerEnum.Monarch));
                    num--;
                }

                LogInfo("Monarch Done");
            }

            if (CustomGameOptions.DictatorOn > 0)
            {
                num = CustomGameOptions.DictatorCount;

                while (num > 0)
                {
                    CrewSovereignRoles.Add(GenerateSpawnItem(LayerEnum.Dictator));
                    num--;
                }

                LogInfo("Dictator Done");
            }

            if (CustomGameOptions.SheriffOn > 0)
            {
                num = CustomGameOptions.SheriffCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Sheriff));
                    num--;
                }

                LogInfo("Sheriff Done");
            }

            if (CustomGameOptions.VigilanteOn > 0)
            {
                num = CustomGameOptions.VigilanteCount;

                while (num > 0)
                {
                    CrewKillingRoles.Add(GenerateSpawnItem(LayerEnum.Vigilante));
                    num--;
                }

                LogInfo("Vigilante Done");
            }

            if (CustomGameOptions.EngineerOn > 0)
            {
                num = CustomGameOptions.EngineerCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Engineer));
                    num--;
                }

                LogInfo("Engineer Done");
            }

            if (CustomGameOptions.MedicOn > 0)
            {
                num = CustomGameOptions.MedicCount;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add(GenerateSpawnItem(LayerEnum.Medic));
                    num--;
                }

                LogInfo("Medic Done");
            }

            if (CustomGameOptions.AltruistOn > 0)
            {
                num = CustomGameOptions.AltruistCount;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add(GenerateSpawnItem(LayerEnum.Altruist));
                    num--;
                }

                LogInfo("Altruist Done");
            }

            if (CustomGameOptions.VeteranOn > 0)
            {
                num = CustomGameOptions.VeteranCount;

                while (num > 0)
                {
                    CrewKillingRoles.Add(GenerateSpawnItem(LayerEnum.Veteran));
                    num--;
                }

                LogInfo("Veteran Done");
            }

            if (CustomGameOptions.BastionOn > 0 && CustomGameOptions.WhoCanVent != WhoCanVentOptions.NoOne)
            {
                num = CustomGameOptions.BastionCount;

                while (num > 0)
                {
                    CrewKillingRoles.Add(GenerateSpawnItem(LayerEnum.Bastion));
                    num--;
                }

                LogInfo("Bastion Done");
            }

            if (CustomGameOptions.TrackerOn > 0)
            {
                num = CustomGameOptions.TrackerCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Tracker));
                    num--;
                }

                LogInfo("Tracker Done");
            }

            if (CustomGameOptions.TransporterOn > 0)
            {
                num = CustomGameOptions.TransporterCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Transporter));
                    num--;
                }

                LogInfo("Transporter Done");
            }

            if (CustomGameOptions.MediumOn > 0)
            {
                num = CustomGameOptions.MediumCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Medium));
                    num--;
                }

                LogInfo("Medium Done");
            }

            if (CustomGameOptions.CoronerOn > 0)
            {
                num = CustomGameOptions.CoronerCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Coroner));
                    num--;
                }

                LogInfo("Coroner Done");
            }

            if (CustomGameOptions.OperativeOn > 0)
            {
                num = CustomGameOptions.OperativeCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Operative));
                    num--;
                }

                LogInfo("Operative Done");
            }

            if (CustomGameOptions.DetectiveOn > 0)
            {
                num = CustomGameOptions.DetectiveCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Detective));
                    num--;
                }

                LogInfo("Detective Done");
            }

            if (CustomGameOptions.EscortOn > 0)
            {
                num = CustomGameOptions.EscortCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Escort));
                    num--;
                }

                LogInfo("Escort Done");
            }

            if (CustomGameOptions.ShifterOn > 0)
            {
                num = CustomGameOptions.ShifterCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Shifter));
                    num--;
                }

                LogInfo("Shifter Done");
            }

            if (CustomGameOptions.ChameleonOn > 0)
            {
                num = CustomGameOptions.ChameleonCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Chameleon));
                    num--;
                }

                LogInfo("Chameleon Done");
            }

            if (CustomGameOptions.RetributionistOn > 0)
            {
                num = CustomGameOptions.RetributionistCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateSpawnItem(LayerEnum.Retributionist));
                    num--;
                }

                LogInfo("Retributionist Done");
            }

            if (CustomGameOptions.TrapperOn > 0)
            {
                num = CustomGameOptions.TrapperCount;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add(GenerateSpawnItem(LayerEnum.Trapper));
                    num--;
                }

                LogInfo("Trapper Done");
            }

            if (CustomGameOptions.CrewmateOn > 0 && IsCustom)
            {
                num = CustomGameOptions.CrewCount;

                while (num > 0)
                {
                    CrewRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));
                    num--;
                }

                LogInfo("Crewmate Done");
            }

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
            {
                num = CustomGameOptions.VampireHunterCount;

                while (num > 0)
                {
                    CrewAuditorRoles.Add(GenerateSpawnItem(LayerEnum.VampireHunter));
                    num--;
                }

                LogInfo("Vampire Hunter Done");
            }

            if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.JackalOn
                > 0))
            {
                num = CustomGameOptions.MysticCount;

                while (num > 0)
                {
                    CrewAuditorRoles.Add(GenerateSpawnItem(LayerEnum.Mystic));
                    num--;
                }

                LogInfo("Mystic Done");
            }

            if (CustomGameOptions.SeerOn > 0 && ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.BountyHunterOn > 0 ||
                CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||  CustomGameOptions.TraitorOn
                > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 || CustomGameOptions.GuardianAngelOn > 0 ||
                CustomGameOptions.GuesserOn > 0 || CustomGameOptions.ShifterOn > 0))
            {
                num = CustomGameOptions.SeerCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Seer));
                    num--;
                }

                LogInfo("Seer Done");
            }
        }

        if (neut > 0)
        {
            if (CustomGameOptions.JesterOn > 0)
            {
                num = CustomGameOptions.JesterCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Jester));
                    num--;
                }

                LogInfo("Jester Done");
            }

            if (CustomGameOptions.AmnesiacOn > 0)
            {
                num = CustomGameOptions.AmnesiacCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateSpawnItem(LayerEnum.Amnesiac));
                    num--;
                }

                LogInfo("Amnesiac Done");
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                num = CustomGameOptions.ExecutionerCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Executioner));
                    num--;
                }

                LogInfo("Executioner Done");
            }

            if (CustomGameOptions.SurvivorOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = CustomGameOptions.SurvivorCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateSpawnItem(LayerEnum.Survivor));
                    num--;
                }

                LogInfo("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = CustomGameOptions.GuardianAngelCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateSpawnItem(LayerEnum.GuardianAngel));
                    num--;
                }

                LogInfo("Guardian Angel Done");
            }

            if (CustomGameOptions.GlitchOn > 0)
            {
                num = CustomGameOptions.GlitchCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Glitch));
                    num--;
                }

                LogInfo("Glitch Done");
            }

            if (CustomGameOptions.MurdererOn > 0)
            {
                num = CustomGameOptions.MurdCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Murderer));
                    num--;
                }

                LogInfo("Murderer Done");
            }

            if (CustomGameOptions.CryomaniacOn > 0)
            {
                num = CustomGameOptions.CryomaniacCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Cryomaniac));
                    num--;
                }

                LogInfo("Cryomaniac Done");
            }

            if (CustomGameOptions.WerewolfOn > 0)
            {
                num = CustomGameOptions.WerewolfCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Werewolf));
                    num--;
                }

                LogInfo("Werewolf Done");
            }

            if (CustomGameOptions.ArsonistOn > 0)
            {
                num = CustomGameOptions.ArsonistCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Arsonist));
                    num--;
                }

                LogInfo("Arsonist Done");
            }

            if (CustomGameOptions.JackalOn > 0 && GameData.Instance.PlayerCount > 5)
            {
                num = CustomGameOptions.JackalCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateSpawnItem(LayerEnum.Jackal));
                    num--;
                }

                LogInfo("Jackal Done");
            }

            if (CustomGameOptions.NecromancerOn > 0)
            {
                num = CustomGameOptions.NecromancerCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateSpawnItem(LayerEnum.Necromancer));
                    num--;
                }

                LogInfo("Necromancer Done");
            }

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                num = CustomGameOptions.PlaguebearerCount;

                while (num > 0)
                {
                    NeutralHarbingerRoles.Add(GenerateSpawnItem(CustomGameOptions.PestSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));
                    num--;
                }

                var PBorPest = CustomGameOptions.PestSpawn ? "Pestilence" : "Plaguebearer";
                LogInfo($"{PBorPest} Done");
            }

            if (CustomGameOptions.SerialKillerOn > 0)
            {
                num = CustomGameOptions.SKCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.SerialKiller));
                    num--;
                }

                LogInfo("Serial Killer Done");
            }

            if (CustomGameOptions.JuggernautOn > 0)
            {
                num = CustomGameOptions.JuggernautCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateSpawnItem(LayerEnum.Juggernaut));
                    num--;
                }

                LogInfo("Juggeraut Done");
            }

            if (CustomGameOptions.CannibalOn > 0)
            {
                num = CustomGameOptions.CannibalCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Cannibal));
                    num--;
                }

                LogInfo("Cannibal Done");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                num = CustomGameOptions.GuesserCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Guesser));
                    num--;
                }

                LogInfo("Guesser Done");
            }

            if (CustomGameOptions.ActorOn > 0 && (CustomGameOptions.CrewAssassinOn > 0 || CustomGameOptions.NeutralAssassinOn > 0 || CustomGameOptions.SyndicateAssassinOn > 0 ||
                CustomGameOptions.IntruderAssassinOn > 0))
            {
                num = CustomGameOptions.ActorCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Actor));
                    num--;
                }

                LogInfo("Actor Done");
            }

            if (CustomGameOptions.ThiefOn > 0)
            {
                num = CustomGameOptions.ThiefCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateSpawnItem(LayerEnum.Thief));
                    num--;
                }

                LogInfo("Thief Done");
            }

            if (CustomGameOptions.DraculaOn > 0)
            {
                num = CustomGameOptions.DraculaCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateSpawnItem(LayerEnum.Dracula));
                    num--;
                }

                LogInfo("Dracula Done");
            }

            if (CustomGameOptions.WhispererOn > 0)
            {
                num = CustomGameOptions.WhispererCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateSpawnItem(LayerEnum.Whisperer));
                    num--;
                }

                LogInfo("Whisperer Done");
            }

            if (CustomGameOptions.TrollOn > 0)
            {
                num = CustomGameOptions.TrollCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.Troll));
                    num--;
                }

                LogInfo("Troll Done");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                num = CustomGameOptions.BHCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateSpawnItem(LayerEnum.BountyHunter));
                    num--;
                }

                LogInfo("Bounty Hunter Done");
            }
        }

        if (imps > 0)
        {
            if (CustomGameOptions.MorphlingOn > 0)
            {
                num = CustomGameOptions.MorphlingCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateSpawnItem(LayerEnum.Morphling));
                    num--;
                }

                LogInfo("Morphling Done");
            }

            if (CustomGameOptions.BlackmailerOn > 0)
            {
                num = CustomGameOptions.BlackmailerCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateSpawnItem(LayerEnum.Blackmailer));
                    num--;
                }

                LogInfo("Blackmailer Done");
            }

            if (CustomGameOptions.MinerOn > 0 && CustomGameOptions.WhoCanVent != WhoCanVentOptions.NoOne && !(!CustomGameOptions.MinerSpawnOnMira && MapPatches.CurrentMap == 2))
            {
                num = CustomGameOptions.MinerCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateSpawnItem(LayerEnum.Miner));
                    num--;
                }

                LogInfo("Miner Done");
            }

            if (CustomGameOptions.TeleporterOn > 0)
            {
                num = CustomGameOptions.TeleporterCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateSpawnItem(LayerEnum.Teleporter));
                    num--;
                }

                LogInfo("Teleporter Done");
            }

            if (CustomGameOptions.AmbusherOn > 0)
            {
                num = CustomGameOptions.AmbusherCount;

                while (num > 0)
                {
                    IntruderKillingRoles.Add(GenerateSpawnItem(LayerEnum.Ambusher));
                    num--;
                }

                LogInfo("Ambusher Done");
            }

            if (CustomGameOptions.WraithOn > 0)
            {
                num = CustomGameOptions.WraithCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateSpawnItem(LayerEnum.Wraith));
                    num--;
                }

                LogInfo("Wraith Done");
            }

            if (CustomGameOptions.ConsortOn > 0)
            {
                num = CustomGameOptions.ConsortCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateSpawnItem(LayerEnum.Consort));
                    num--;
                }

                LogInfo("Consort Done");
            }

            if (CustomGameOptions.JanitorOn > 0)
            {
                num = CustomGameOptions.JanitorCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateSpawnItem(LayerEnum.Janitor));
                    num--;
                }

                LogInfo("Janitor Done");
            }

            if (CustomGameOptions.CamouflagerOn > 0)
            {
                num = CustomGameOptions.CamouflagerCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateSpawnItem(LayerEnum.Camouflager));
                    num--;
                }

                LogInfo("Camouflager Done");
            }

            if (CustomGameOptions.GrenadierOn > 0)
            {
                num = CustomGameOptions.GrenadierCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateSpawnItem(LayerEnum.Grenadier));
                    num--;
                }

                LogInfo("Grenadier Done");
            }

            if (CustomGameOptions.ImpostorOn > 0 && IsCustom)
            {
                num = CustomGameOptions.ImpCount;

                while (num > 0)
                {
                    IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Impostor));
                    num--;
                }

                LogInfo("Impostor Done");
            }

            if (CustomGameOptions.ConsigliereOn > 0)
            {
                num = CustomGameOptions.ConsigliereCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateSpawnItem(LayerEnum.Consigliere));
                    num--;
                }

                LogInfo("Consigliere Done");
            }

            if (CustomGameOptions.DisguiserOn > 0)
            {
                num = CustomGameOptions.DisguiserCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateSpawnItem(LayerEnum.Disguiser));
                    num--;
                }

                LogInfo("Disguiser Done");
            }

            if (CustomGameOptions.EnforcerOn > 0)
            {
                num = CustomGameOptions.EnforcerCount;

                while (num > 0)
                {
                    IntruderKillingRoles.Add(GenerateSpawnItem(LayerEnum.Enforcer));
                    num--;
                }

                LogInfo("Enforcer Done");
            }

            if (CustomGameOptions.GodfatherOn > 0 && imps >= 3)
            {
                num = CustomGameOptions.GodfatherCount;

                while (num > 0)
                {
                    IntruderHeadRoles.Add(GenerateSpawnItem(LayerEnum.Godfather));
                    num--;
                }

                LogInfo("Godfather Done");
            }
        }

        if (syn > 0)
        {
            if (CustomGameOptions.AnarchistOn > 0 && IsCustom)
            {
                num = CustomGameOptions.AnarchistCount;

                while (num > 0)
                {
                    SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Anarchist));
                    num--;
                }

                LogInfo("Anarchist Done");
            }

            if (CustomGameOptions.ShapeshifterOn > 0)
            {
                num = CustomGameOptions.ShapeshifterCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Shapeshifter));
                    num--;
                }

                LogInfo("Shapeshifter Done");
            }

            if (CustomGameOptions.FramerOn > 0)
            {
                num = CustomGameOptions.FramerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Framer));
                    num--;
                }

                LogInfo("Framer Done");
            }

            if (CustomGameOptions.CrusaderOn > 0)
            {
                num = CustomGameOptions.CrusaderCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateSpawnItem(LayerEnum.Crusader));
                    num--;
                }

                LogInfo("Crusader Done");
            }

            if (CustomGameOptions.RebelOn > 0 && syn >= 3)
            {
                num = CustomGameOptions.RebelCount;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add(GenerateSpawnItem(LayerEnum.Rebel));
                    num--;
                }

                LogInfo("Rebel Done");
            }

            if (CustomGameOptions.PoisonerOn > 0)
            {
                num = CustomGameOptions.PoisonerCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateSpawnItem(LayerEnum.Poisoner));
                    num--;
                }

                LogInfo("Poisoner Done");
            }

            if (CustomGameOptions.ColliderOn > 0)
            {
                num = CustomGameOptions.ColliderCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateSpawnItem(LayerEnum.Collider));
                    num--;
                }

                LogInfo("Collider Done");
            }

            if (CustomGameOptions.ConcealerOn > 0)
            {
                num = CustomGameOptions.ConcealerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Concealer));
                    num--;
                }

                LogInfo("Concealer Done");
            }

            if (CustomGameOptions.WarperOn > 0)
            {
                num = CustomGameOptions.WarperCount;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add(GenerateSpawnItem(LayerEnum.Warper));
                    num--;
                }

                LogInfo("Warper Done");
            }

            if (CustomGameOptions.BomberOn > 0)
            {
                num = CustomGameOptions.BomberCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateSpawnItem(LayerEnum.Bomber));
                    num--;
                }

                LogInfo("Bomber Done");
            }

            if (CustomGameOptions.SpellslingerOn > 0)
            {
                num = CustomGameOptions.SpellslingerCount;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add(GenerateSpawnItem(LayerEnum.Spellslinger));
                    num--;
                }

                LogInfo("Spellslinger Done");
            }

            if (CustomGameOptions.StalkerOn > 0)
            {
                num = CustomGameOptions.StalkerCount;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add(GenerateSpawnItem(LayerEnum.Stalker));
                    num--;
                }

                LogInfo("Stalker Done");
            }

            if (CustomGameOptions.DrunkardOn > 0)
            {
                num = CustomGameOptions.DrunkardCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Drunkard));
                    num--;
                }

                LogInfo("Drunkard Done");
            }

            if (CustomGameOptions.TimekeeperOn > 0)
            {
                num = CustomGameOptions.TimekeeperCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Timekeeper));
                    num--;
                }

                LogInfo("Timekeeper Done");
            }

            if (CustomGameOptions.SilencerOn > 0)
            {
                num = CustomGameOptions.SilencerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateSpawnItem(LayerEnum.Silencer));
                    num--;
                }

                LogInfo("Silencer Done");
            }
        }

        if (IsClassic || IsCustom)
        {
            if (!CustomGameOptions.AltImps && imps > 0)
            {
                var maxIC = CustomGameOptions.ICMax;
                var maxID = CustomGameOptions.IDMax;
                var maxIK = CustomGameOptions.IKMax;
                var maxIS = CustomGameOptions.ISMax;
                var maxIH = CustomGameOptions.IHMax;
                var minInt = CustomGameOptions.IntruderMin;
                var maxInt = CustomGameOptions.IntruderMax;

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

                if (!CustomGameOptions.IgnoreAlignmentCaps)
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

                IntruderRoles = Sort(IntruderRoles, CustomGameOptions.IgnoreFactionCaps ? imps : URandom.RandomRangeInt(minInt, maxInt + 1));
            }

            while (IntruderRoles.Count < imps)
                IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Impostor));

            if (syn > 0)
            {
                var maxSSu = CustomGameOptions.SSuMax;
                var maxSD = CustomGameOptions.SDMax;
                var maxSyK = CustomGameOptions.SyKMax;
                var maxSP = CustomGameOptions.SPMax;
                var minSyn = CustomGameOptions.SyndicateMin;
                var maxSyn = CustomGameOptions.SyndicateMax;

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

                if (!CustomGameOptions.IgnoreAlignmentCaps)
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

                SyndicateRoles = Sort(SyndicateRoles, CustomGameOptions.IgnoreFactionCaps ? syn : URandom.RandomRangeInt(minSyn, maxSyn + 1));
            }

            while (SyndicateRoles.Count < syn)
                SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Anarchist));

            if (neut > 0)
            {
                var maxNE = CustomGameOptions.NEMax;
                var maxNB = CustomGameOptions.NBMax;
                var maxNK = CustomGameOptions.NKMax;
                var maxNN = CustomGameOptions.NNMax;
                var maxNH = CustomGameOptions.NHMax;
                var minNeut = CustomGameOptions.NeutralMin;
                var maxNeut = CustomGameOptions.NeutralMax;

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

                if (!CustomGameOptions.IgnoreAlignmentCaps)
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

                NeutralRoles = Sort(NeutralRoles, CustomGameOptions.IgnoreFactionCaps ? neut : URandom.RandomRangeInt(minNeut, maxNeut + 1));
            }

            if (crew > 0)
            {
                var maxCI = CustomGameOptions.CIMax;
                var maxCS = CustomGameOptions.CSMax;
                var maxCA = CustomGameOptions.CAMax;
                var maxCK = CustomGameOptions.CKMax;
                var maxCrP = CustomGameOptions.CrPMax;
                var maxCSv = CustomGameOptions.CSvMax;
                var minCrew = CustomGameOptions.CrewMin;
                var maxCrew = CustomGameOptions.CrewMax;

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

                if (!CustomGameOptions.IgnoreAlignmentCaps)
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

                CrewRoles = Sort(CrewRoles, CustomGameOptions.IgnoreFactionCaps ? crew : URandom.RandomRangeInt(minCrew, maxCrew + 1));
            }

            while (CrewRoles.Count < crew)
                CrewRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));

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

        if (!CustomGameOptions.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) || RoleListEntryOption.IsBanned(id) || (id == LayerEnum.Crewmate &&
        CustomGameOptions.BanCrewmate) || (id == LayerEnum.Impostor && CustomGameOptions.BanImpostor) || (id == LayerEnum.Anarchist && CustomGameOptions.BanAnarchist) || (id ==
        LayerEnum.Murderer && CustomGameOptions.BanMurderer) || id == LayerEnum.Actor;

    private static void GenRoleList()
    {
        var entries = CustomOption.GetOptions<RoleListEntryOption>().Where(x => x.Name.Contains("Entry"));
        var bans = CustomOption.GetOptions<RoleListEntryOption>().Where(x => x.IsBan);
        var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get()));
        var randoms = entries.Where(x => RandomEntries.Contains(x.Get()));
        var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get())));
        var anies = entries.Where(x => x.Get() == LayerEnum.Any);
        // I have no idea what plural for any is lmao

        SetPostmortals.PhantomOn = CustomGameOptions.EnablePhantom;
        SetPostmortals.RevealerOn = CustomGameOptions.EnableRevealer;
        SetPostmortals.BansheeOn = CustomGameOptions.EnableBanshee;
        SetPostmortals.GhoulOn = CustomGameOptions.EnableGhoul;

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
                    AllRoles.Add(GenerateSpawnItem(id));
            }
        }

        foreach (var entry in alignments)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;
            var random = LayerEnum.None;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                if (id == LayerEnum.CrewAudit)
                    random = CA.Random();
                else if (id == LayerEnum.CrewInvest)
                    random = CI.Random();
                else if (id == LayerEnum.CrewSov)
                    random = CSv.Random();
                else if (id == LayerEnum.CrewProt)
                    random = CrP.Random();
                else if (id == LayerEnum.CrewKill)
                    random = CK.Random();
                else if (id == LayerEnum.CrewSupport)
                    random = CS.Random();
                else if (id == LayerEnum.NeutralBen)
                    random = NB.Random();
                else if (id == LayerEnum.NeutralEvil)
                    random = NE.Random();
                else if (id == LayerEnum.NeutralNeo)
                    random = NN.Random();
                else if (id == LayerEnum.NeutralHarb)
                    random = NH.Random();
                else if (id == LayerEnum.NeutralApoc)
                    random = NA.Random();
                else if (id == LayerEnum.NeutralKill)
                    random = NK.Random();
                else if (id == LayerEnum.IntruderConceal)
                    random = IC.Random();
                else if (id == LayerEnum.IntruderDecep)
                    random = ID.Random();
                else if (id == LayerEnum.IntruderKill)
                    random = IK.Random();
                else if (id == LayerEnum.IntruderSupport)
                    random = IS.Random();
                else if (id == LayerEnum.SyndicateSupport)
                    random = SSu.Random();
                else if (id == LayerEnum.SyndicatePower)
                    random = SP.Random();
                else if (id == LayerEnum.SyndicateDisrup)
                    random = SD.Random();
                else if (id == LayerEnum.SyndicateKill)
                    random = SyK.Random();
                else if (id == LayerEnum.CrewUtil)
                    random = CU.Random();
                else if (id == LayerEnum.IntruderUtil)
                    random = IU.Random();
                else if (id == LayerEnum.SyndicateUtil)
                    random = SU.Random();
                else if (id == LayerEnum.IntruderHead)
                    random = IH.Random();

                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(random));
            }
        }

        foreach (var entry in randoms)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;
            var random = LayerEnum.None;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                if (id == LayerEnum.RandomCrew)
                    random = Crew.Random().Random();
                else if (id == LayerEnum.RandomNeutral)
                    random = Neutral.Random().Random();
                else if (id == LayerEnum.RandomIntruder)
                    random = Intruders.Random().Random();
                else if (id == LayerEnum.RandomSyndicate)
                    random = Syndicate.Random().Random();
                else if (id == LayerEnum.RegularCrew)
                    random = RegCrew.Random().Random();
                else if (id == LayerEnum.RegularIntruder)
                    random = RegIntruders.Random().Random();
                else if (id == LayerEnum.RegularNeutral)
                    random = RegNeutral.Random().Random();
                else if (id == LayerEnum.HarmfulNeutral)
                    random = HarmNeutral.Random().Random();
                else if (id == LayerEnum.RegularSyndicate)
                    random = RegSyndicate.Random().Random();

                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(random));
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
                    AllRoles.Add(GenerateSpawnItem(random));
            }
        }

        // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        // In case if the ratelimits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));
    }

    private static void GenTaskRace()
    {
        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Runner));
    }

    private static void GenHideAndSeek()
    {
        while (AllRoles.Count < CustomGameOptions.HunterCount)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Hunter));

        while (AllRoles.Count < CustomPlayer.AllPlayers.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Hunted));
    }

    private static RoleOptionData GenerateSpawnItem(LayerEnum id) => id switch
    {
        LayerEnum.Anarchist => SyndicateUtilityRoles.Anarchist,
        LayerEnum.Impostor => IntruderUtilityRoles.Impostor,
        LayerEnum.Murderer => Options2.NeutralKillingRoles.Murderer,
        LayerEnum.Vigilante => Options2.CrewKillingRoles.Vigilante,
        LayerEnum.Veteran => Options2.CrewKillingRoles.Veteran,
        LayerEnum.Bastion => Options2.CrewKillingRoles.Bastion,
        LayerEnum.Crewmate => CrewUtilityRoles.Crewmate,
        LayerEnum.Runner or LayerEnum.Hunter or LayerEnum.Hunted => new(100, 100, false, false, id),
        _ => OptionAttribute.GetOptions<LayersOptionAttribute>().Find(x => x.Layer == id)?.Get() ?? throw new NotImplementedException(id.ToString())
    };

    private static void GenAbilities()
    {
        var num = 0;

        if (CustomGameOptions.CrewAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfCrewAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Bullseye));
                num--;
            }

            LogInfo("Crew Assassin Done");
        }

        if (CustomGameOptions.SyndicateAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfSyndicateAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Sniper));
                num--;
            }

            LogInfo("Syndicate Assassin Done");
        }

        if (CustomGameOptions.IntruderAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfIntruderAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Hitman));
                num--;
            }

            LogInfo("Intruder Assassin Done");
        }

        if (CustomGameOptions.NeutralAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfNeutralAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Slayer));
                num--;
            }

            LogInfo("Neutral Assassin Done");
        }

        if (CustomGameOptions.RuthlessOn > 0)
        {
            num = CustomGameOptions.RuthlessCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Ruthless));
                num--;
            }

            LogInfo("Ruthless Done");
        }

        if (CustomGameOptions.SnitchOn > 0)
        {
            num = CustomGameOptions.SnitchCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Snitch));
                num--;
            }

            LogInfo("Snitch Done");
        }

        if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting != AnonVotes.Disabled)
        {
            num = CustomGameOptions.InsiderCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Insider));
                num--;
            }

            LogInfo("Insider Done");
        }

        if (CustomGameOptions.MultitaskerOn > 0)
        {
            num = CustomGameOptions.MultitaskerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Multitasker));
                num--;
            }

            LogInfo("Multitasker Done");
        }

        if (CustomGameOptions.RadarOn > 0)
        {
            num = CustomGameOptions.RadarCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Radar));
                num--;
            }

            LogInfo("Radar Done");
        }

        if (CustomGameOptions.TiebreakerOn > 0)
        {
            num = CustomGameOptions.TiebreakerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Tiebreaker));
                num--;
            }

            LogInfo("Tiebreaker Done");
        }

        if (CustomGameOptions.TorchOn > 0)
        {
            num = CustomGameOptions.TorchCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Torch));
                num--;
            }

            LogInfo("Torch Done");
        }

        if (CustomGameOptions.UnderdogOn > 0)
        {
            num = CustomGameOptions.UnderdogCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Underdog));
                num--;
            }

            LogInfo("Underdog Done");
        }

        if (CustomGameOptions.TunnelerOn > 0 && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && !CustomGameOptions.CrewVent)
        {
            num = CustomGameOptions.TunnelerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Tunneler));
                num--;
            }

            LogInfo("Tunneler Done");
        }

        if (CustomGameOptions.NinjaOn > 0)
        {
            num = CustomGameOptions.NinjaCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Ninja));
                num--;
            }

            LogInfo("Ninja Done");
        }

        if (CustomGameOptions.ButtonBarryOn > 0)
        {
            num = CustomGameOptions.ButtonBarryCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.ButtonBarry));
                num--;
            }

            LogInfo("Button Barry Done");
        }

        if (CustomGameOptions.PoliticianOn > 0)
        {
            num = CustomGameOptions.PoliticianCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Politician));
                num--;
            }

            LogInfo("Politician Done");
        }

        if (CustomGameOptions.SwapperOn > 0)
        {
            num = CustomGameOptions.SwapperCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.Swapper));
                num--;
            }

            LogInfo("Swapper Done");
        }

        var maxAb = CustomGameOptions.MaxAbilities;
        var minAb = CustomGameOptions.MinAbilities;

        while (maxAb > CustomPlayer.AllPlayers.Count)
            maxAb--;

        while (minAb > CustomPlayer.AllPlayers.Count)
            minAb--;

        AllAbilities = Sort(AllAbilities, CustomGameOptions.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minAb, maxAb + 1));

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

        canHaveIntruderAbility.RemoveAll(x => !x.Is(Faction.Intruder) || (x.Is(LayerEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
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

        canHaveTorch.RemoveAll(x => (x.Is(Alignment.NeutralKill) && !CustomGameOptions.NKHasImpVision) || x.Is(Faction.Syndicate) || (x.Is(Faction.Neutral) &&
            !CustomGameOptions.LightsAffectNeutrals) || x.Is(Faction.Intruder));
        canHaveTorch.Shuffle();

        canHaveEvilAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
        canHaveEvilAbility.Shuffle();

        canHaveKillingAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) ||
            x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted)));
        canHaveKillingAbility.Shuffle();

        canHaveRuthless.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) ||
            x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted)) || x.Is(LayerEnum.Juggernaut));
        canHaveRuthless.Shuffle();

        canHaveBB.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(LayerEnum.Jester) && !CustomGameOptions.JesterButton) || (x.Is(LayerEnum.Actor) &&
            !CustomGameOptions.ActorButton) || (x.Is(LayerEnum.Guesser) && !CustomGameOptions.GuesserButton) || (x.Is(LayerEnum.Executioner) && !CustomGameOptions.ExecutionerButton) ||
            (!CustomGameOptions.MonarchButton && x.Is(LayerEnum.Monarch)) || (!CustomGameOptions.DictatorButton && x.Is(LayerEnum.Dictator)));
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

        while (canHaveSnitch.Any() || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Any()) || canHaveIntruderAbility.Any() || canHaveTorch.Any() ||
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
            else if (canHaveTunnelerAbility.Any() && id == LayerEnum.Tunneler && CustomGameOptions.WhoCanVent != WhoCanVentOptions.Everyone)
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
                    AllAbilities.Add(GenerateSpawnItem(id));
            }
        }

        LogMessage("Abilities Done");
    }

    private static void GenObjectifiers()
    {
        var num = 0;

        if (CustomGameOptions.LoversOn > 0 && GameData.Instance.PlayerCount > 4)
        {
            num = CustomGameOptions.LoversCount * 2;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Lovers));
                num--;
            }

            LogInfo("Lovers Done");
        }

        if (CustomGameOptions.RivalsOn > 0 && GameData.Instance.PlayerCount > 3)
        {
            num = CustomGameOptions.RivalsCount * 2;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Rivals));
                num--;
            }

            LogInfo("Rivals Done");
        }

        if (CustomGameOptions.FanaticOn > 0)
        {
            num = CustomGameOptions.FanaticCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Fanatic));
                num--;
            }

            LogInfo("Fanatic Done");
        }

        if (CustomGameOptions.CorruptedOn > 0)
        {
            num = CustomGameOptions.CorruptedCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Corrupted));
                num--;
            }

            LogInfo("Corrupted Done");
        }

        if (CustomGameOptions.OverlordOn > 0)
        {
            num = CustomGameOptions.OverlordCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Overlord));
                num--;
            }

            LogInfo("Overlord Done");
        }

        if (CustomGameOptions.AlliedOn > 0)
        {
            num = CustomGameOptions.AlliedCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Allied));
                num--;
            }

            LogInfo("Allied Done");
        }

        if (CustomGameOptions.TraitorOn > 0)
        {
            num = CustomGameOptions.TraitorCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Traitor));
                num--;
            }

            LogInfo("Traitor Done");
        }

        if (CustomGameOptions.TaskmasterOn > 0)
        {
            num = CustomGameOptions.TaskmasterCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Taskmaster));
                num--;
            }

            LogInfo("Taskmaster Done");
        }

        if (CustomGameOptions.MafiaOn > 0)
        {
            num = CustomGameOptions.MafiaCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Mafia));
                num--;
            }

            LogInfo("Mafia Done");
        }

        if (CustomGameOptions.DefectorOn > 0)
        {
            num = CustomGameOptions.DefectorCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Defector));
                num--;
            }

            LogInfo("Defector Done");
        }

        if (CustomGameOptions.LinkedOn > 0 && Role.GetRoles(Faction.Neutral).Count > 1 && GameData.Instance.PlayerCount > 3)
        {
            num = CustomGameOptions.LinkedCount * 2;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateSpawnItem(LayerEnum.Linked));
                num--;
            }

            LogInfo("Linked Done");
        }


        var maxObj = CustomGameOptions.MaxObjectifiers;
        var minObj = CustomGameOptions.MinObjectifiers;

        while (maxObj > CustomPlayer.AllPlayers.Count)
            maxObj--;

        while (minObj > CustomPlayer.AllPlayers.Count)
            minObj--;

        AllObjectifiers = Sort(AllObjectifiers, CustomGameOptions.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minObj, maxObj + 1));

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
                    AllObjectifiers.Add(GenerateSpawnItem(id));
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

        if (CustomGameOptions.DiseasedOn > 0)
        {
            num = CustomGameOptions.DiseasedCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Diseased));
                num--;
            }

            LogInfo("Diseased Done");
        }

        if (CustomGameOptions.BaitOn > 0)
        {
            num = CustomGameOptions.BaitCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Bait));
                num--;
            }

            LogInfo("Bait Done");
        }

        if (CustomGameOptions.DwarfOn > 0)
        {
            num = CustomGameOptions.DwarfCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Dwarf));
                num--;
            }

            LogInfo("Dwarf Done");
        }

        if (CustomGameOptions.VIPOn > 0)
        {
            num = CustomGameOptions.VIPCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.VIP));
                num--;
            }

            LogInfo("VIP Done");
        }

        if (CustomGameOptions.ShyOn > 0)
        {
            num = CustomGameOptions.ShyCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Shy));
                num--;
            }

            LogInfo("Shy Done");
        }

        if (CustomGameOptions.GiantOn > 0)
        {
            num = CustomGameOptions.GiantCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Giant));
                num--;
            }

            LogInfo("Giant Done");
        }

        if (CustomGameOptions.DrunkOn > 0)
        {
            num = CustomGameOptions.DrunkCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Drunk));
                num--;
            }

            LogInfo("Drunk Done");
        }

        if (CustomGameOptions.CowardOn > 0)
        {
            num = CustomGameOptions.CowardCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Coward));
                num--;
            }

            LogInfo("Coward Done");
        }

        if (CustomGameOptions.VolatileOn > 0)
        {
            num = CustomGameOptions.VolatileCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Volatile));
                num--;
            }

            LogInfo("Volatile Done");
        }

        if (CustomGameOptions.IndomitableOn > 0)
        {
            num = CustomGameOptions.IndomitableCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Indomitable));
                num--;
            }

            LogInfo("Indomitable Done");
        }

        if (CustomGameOptions.ProfessionalOn > 0)
        {
            num = CustomGameOptions.ProfessionalCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Professional));
                num--;
            }

            LogInfo("Professional Done");
        }

        if (CustomGameOptions.AstralOn > 0)
        {
            num = CustomGameOptions.AstralCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Astral));
                num--;
            }

            LogInfo("Astral Done");
        }

        if (CustomGameOptions.YellerOn > 0)
        {
            num = CustomGameOptions.YellerCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Yeller));
                num--;
            }

            LogInfo("Yeller Done");
        }

        if (CustomGameOptions.ColorblindOn > 0)
        {
            num = CustomGameOptions.ColorblindCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateSpawnItem(LayerEnum.Colorblind));
                num--;
            }

            LogInfo("Colorblind Done");
        }


        var maxMod = CustomGameOptions.MaxModifiers;
        var minMod = CustomGameOptions.MinModifiers;

        while (maxMod > CustomPlayer.AllPlayers.Count)
            maxMod--;

        while (minMod > CustomPlayer.AllPlayers.Count)
            minMod--;

        AllModifiers = Sort(AllModifiers, CustomGameOptions.IgnoreLayerCaps ? CustomPlayer.AllPlayers.Count : URandom.RandomRangeInt(minMod, maxMod + 1));

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

        canHaveShy.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(LayerEnum.Jester) && !CustomGameOptions.JesterButton) || (x.Is(LayerEnum.Swapper) &&
            !CustomGameOptions.SwapperButton) || (x.Is(LayerEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(LayerEnum.Guesser) && !CustomGameOptions.GuesserButton) ||
            (x.Is(LayerEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || x.Is(LayerEnum.ButtonBarry) || (x.Is(LayerEnum.Politician) && !CustomGameOptions.PoliticianButton) ||
            (!CustomGameOptions.DictatorButton && x.Is(LayerEnum.Dictator)) || (!CustomGameOptions.MonarchButton && x.Is(LayerEnum.Monarch)));
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
                    AllModifiers.Add(GenerateSpawnItem(id));
            }
        }

        LogMessage("Modifiers Done");
    }

    private static void SetTargets()
    {
        if (CustomGameOptions.AlliedOn > 0)
        {
            foreach (var ally in PlayerLayer.GetLayers<Allied>())
            {
                var alliedRole = ally.Player.GetRole();
                var crew = CustomGameOptions.AlliedFaction == AlliedFaction.Crew;
                var intr = CustomGameOptions.AlliedFaction == AlliedFaction.Intruder;
                var syn = CustomGameOptions.AlliedFaction == AlliedFaction.Syndicate;
                var factions = new List<byte>() { 1, 3, 0 };
                byte faction;

                if (CustomGameOptions.AlliedFaction == AlliedFaction.Random)
                {
                    faction = factions.Random();
                    intr = faction == 1;
                    syn = faction == 3;
                    crew = faction == 0;
                }
                else
                    faction = factions[(int)CustomGameOptions.AlliedFaction - 1];

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
        }

        if (CustomGameOptions.LoversOn > 0)
        {
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
        }

        if (CustomGameOptions.RivalsOn > 0)
        {
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
        }

        if (CustomGameOptions.LinkedOn > 0)
        {
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
        }

        if (CustomGameOptions.MafiaOn > 0)
        {
            if (PlayerLayer.GetLayers<Mafia>().Count == 1)
            {
                foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(LayerEnum.Mafia)))
                    NullLayer(player, PlayerLayerEnum.Objectifier);
            }

            LogMessage("Mafia Set");
        }

        if (CustomGameOptions.ExecutionerOn > 0 && !CustomGameOptions.ExecutionerCanPickTargets)
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

        if (CustomGameOptions.GuesserOn > 0 && !CustomGameOptions.GuesserCanPickTargets)
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

        if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.GuardianAngelCanPickTargets)
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

        if (CustomGameOptions.BountyHunterOn > 0 && !CustomGameOptions.BountyHunterCanPickTargets)
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

        if (CustomGameOptions.ActorOn > 0 && !CustomGameOptions.ActorCanPickRole)
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

        if (CustomGameOptions.JackalOn > 0)
        {
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
        }

        LogMessage("Targets Set");
    }

    public static void ResetEverything()
    {
        PlayerLayer.NobodyWins = false;

        Role.CrewWin = false;
        Role.SyndicateWin = false;
        Role.IntruderWin = false;
        Role.AllNeutralsWin = false;

        Role.UndeadWin = false;
        Role.CabalWin = false;
        Role.SectWin = false;
        Role.ReanimatedWin = false;
        Role.ApocalypseWins = false;

        Role.NKWins = false;

        Role.GlitchWins = false;
        Role.WerewolfWins = false;
        Role.JuggernautWins = false;
        Role.ArsonistWins = false;
        Role.MurdererWins = false;
        Role.SerialKillerWins = false;

        Role.PhantomWins = false;

        Role.JesterWins = false;
        Role.ActorWins = false;
        Role.ExecutionerWins = false;
        Role.GuesserWins = false;
        Role.BountyHunterWins = false;
        Role.CannibalWins = false;
        Role.TrollWins = false;

        Role.TaskRunnerWins = false;

        Role.HunterWins = false;
        Role.HuntedWins = false;

        Objectifier.LoveWins = false;
        Objectifier.RivalWins = false;
        Objectifier.TaskmasterWins = false;
        Objectifier.OverlordWins = false;
        Objectifier.CorruptedWins = false;
        Objectifier.MafiaWins = false;
        Objectifier.DefectorWins = false;

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

        Assassin.RemainingKills = CustomGameOptions.AssassinKills;

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
        LogMessage($"Current Mode = {CustomGameOptions.GameMode}");
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
                AllRoles.Add(GenerateSpawnItem(LayerEnum.Vigilante));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Mystic);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GenerateSpawnItem(LayerEnum.Seer));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.VampireHunter or LayerEnum.Amnesiac or LayerEnum.Thief or LayerEnum.Godfather or LayerEnum.Shifter or LayerEnum.Guesser or LayerEnum.Rebel
            or LayerEnum.Executioner or LayerEnum.GuardianAngel or LayerEnum.BountyHunter or LayerEnum.Mystic or LayerEnum.Actor))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Seer);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GenerateSpawnItem(LayerEnum.Sheriff));
        }

        if (AllRoles.Any(x => x.ID == LayerEnum.Cannibal) && AllRoles.Any(x => x.ID == LayerEnum.Janitor) && CustomGameOptions.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var count = AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor));

            for (var i = 0; i < count; i++)
            {
                if (chance == 0)
                    AllRoles.Add(NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GenerateSpawnItem(LayerEnum.Amnesiac)));
                else
                    AllRoles.Add(IntruderRoles.Random(x => x.ID != LayerEnum.Janitor, GenerateSpawnItem(LayerEnum.Impostor)));
            }
        }

        if (players.Count <= 4 && AllRoles.Any(x => x.ID == LayerEnum.Amnesiac))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Amnesiac);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GenerateSpawnItem(LayerEnum.Thief));
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
                if (CustomGameOptions.EnableObjectifiers)
                    GenObjectifiers();

                if (CustomGameOptions.EnableAbilities)
                    GenAbilities();

                if (CustomGameOptions.EnableModifiers)
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

    public static PlayerLayer SetLayer(LayerEnum id, PlayerLayerEnum rpc) => id switch
    {
        LayerEnum.Altruist => new Altruist(),
        LayerEnum.Chameleon => new Chameleon(),
        LayerEnum.Coroner => new Coroner(),
        LayerEnum.Crewmate => new Crewmate(),
        LayerEnum.Detective => new Detective(),
        LayerEnum.Dictator => new Dictator(),
        LayerEnum.Engineer => new Engineer(),
        LayerEnum.Escort => new Escort(),
        LayerEnum.Mayor => new Mayor(),
        LayerEnum.Medic => new Medic(),
        LayerEnum.Medium => new Medium(),
        LayerEnum.Monarch => new Monarch(),
        LayerEnum.Mystic => new Mystic(),
        LayerEnum.Operative => new Operative(),
        LayerEnum.Retributionist => new Retributionist(),
        LayerEnum.Revealer => new Revealer(),
        LayerEnum.Seer => new Seer(),
        LayerEnum.Sheriff => new Sheriff(),
        LayerEnum.Shifter => new Shifter(),
        LayerEnum.Tracker => new Tracker(),
        LayerEnum.Transporter => new Transporter(),
        LayerEnum.VampireHunter => new VampireHunter(),
        LayerEnum.Veteran => new Veteran(),
        LayerEnum.Vigilante => new Vigilante(),
        LayerEnum.Actor => new Actor(),
        LayerEnum.Amnesiac => new Amnesiac(),
        LayerEnum.Arsonist => new Arsonist(),
        LayerEnum.BountyHunter => new BountyHunter(),
        LayerEnum.Cannibal => new Cannibal(),
        LayerEnum.Cryomaniac => new Cryomaniac(),
        LayerEnum.Dracula => new Dracula(),
        LayerEnum.Executioner => new Executioner(),
        LayerEnum.Glitch => new Glitch(),
        LayerEnum.GuardianAngel => new GuardianAngel(),
        LayerEnum.Guesser => new Guesser(),
        LayerEnum.Jackal => new Jackal(),
        LayerEnum.Jester => new Jester(),
        LayerEnum.Juggernaut => new Juggernaut(),
        LayerEnum.Murderer => new Murderer(),
        LayerEnum.Necromancer => new Necromancer(),
        LayerEnum.Pestilence => new Pestilence(),
        LayerEnum.Phantom => new Phantom(),
        LayerEnum.Plaguebearer => new Plaguebearer(),
        LayerEnum.SerialKiller => new SerialKiller(),
        LayerEnum.Survivor => new Survivor(),
        LayerEnum.Thief => new Thief(),
        LayerEnum.Troll => new Troll(),
        LayerEnum.Werewolf => new Werewolf(),
        LayerEnum.Whisperer => new Whisperer(),
        LayerEnum.Betrayer => new Betrayer(),
        LayerEnum.Ambusher => new Ambusher(),
        LayerEnum.Blackmailer => new Blackmailer(),
        LayerEnum.Camouflager => new Camouflager(),
        LayerEnum.Consigliere => new Consigliere(),
        LayerEnum.Consort => new Consort(),
        LayerEnum.Disguiser => new Disguiser(),
        LayerEnum.Enforcer => new Enforcer(),
        LayerEnum.Ghoul => new Ghoul(),
        LayerEnum.Godfather => new Godfather(),
        LayerEnum.Grenadier => new Grenadier(),
        LayerEnum.Impostor => new Impostor(),
        LayerEnum.Janitor => new Janitor(),
        LayerEnum.Mafioso => new Mafioso(),
        LayerEnum.Miner => new Miner(),
        LayerEnum.Morphling => new Morphling(),
        LayerEnum.PromotedGodfather => new PromotedGodfather(),
        LayerEnum.Teleporter => new Teleporter(),
        LayerEnum.Wraith => new Wraith(),
        LayerEnum.Anarchist => new Anarchist(),
        LayerEnum.Banshee => new Banshee(),
        LayerEnum.Bomber => new Bomber(),
        LayerEnum.Concealer => new Concealer(),
        LayerEnum.Collider => new PlayerLayers.Roles.Collider(),
        LayerEnum.Crusader => new Crusader(),
        LayerEnum.Drunkard => new Drunkard(),
        LayerEnum.Framer => new Framer(),
        LayerEnum.Poisoner => new Poisoner(),
        LayerEnum.PromotedRebel => new PromotedRebel(),
        LayerEnum.Rebel => new Rebel(),
        LayerEnum.Shapeshifter => new Shapeshifter(),
        LayerEnum.Sidekick => new Sidekick(),
        LayerEnum.Silencer => new Silencer(),
        LayerEnum.Spellslinger => new Spellslinger(),
        LayerEnum.Stalker => new Stalker(),
        LayerEnum.Timekeeper => new Timekeeper(),
        LayerEnum.Warper => new Warper(),
        LayerEnum.Runner => new Runner(),
        LayerEnum.Hunter => new Hunter(),
        LayerEnum.Hunted => new Hunted(),
        LayerEnum.Bastion => new Bastion(),
        LayerEnum.Trapper => new Trapper(),
        LayerEnum.Bullseye => new CrewAssassin(),
        LayerEnum.Hitman => new IntruderAssassin(),
        LayerEnum.Slayer => new NeutralAssassin(),
        LayerEnum.Sniper => new SyndicateAssassin(),
        LayerEnum.ButtonBarry => new ButtonBarry(),
        LayerEnum.Insider => new Insider(),
        LayerEnum.Multitasker => new Multitasker(),
        LayerEnum.Ninja => new Ninja(),
        LayerEnum.Politician => new Politician(),
        LayerEnum.Radar => new Radar(),
        LayerEnum.Ruthless => new Ruthless(),
        LayerEnum.Snitch => new Snitch(),
        LayerEnum.Swapper => new Swapper(),
        LayerEnum.Tiebreaker => new Tiebreaker(),
        LayerEnum.Torch => new Torch(),
        LayerEnum.Tunneler => new Tunneler(),
        LayerEnum.Underdog => new Underdog(),
        LayerEnum.Allied => new Allied(),
        LayerEnum.Corrupted => new Corrupted(),
        LayerEnum.Defector => new Defector(),
        LayerEnum.Fanatic => new Fanatic(),
        LayerEnum.Linked => new Linked(),
        LayerEnum.Lovers => new Lovers(),
        LayerEnum.Mafia => new Mafia(),
        LayerEnum.Overlord => new Overlord(),
        LayerEnum.Rivals => new Rivals(),
        LayerEnum.Taskmaster => new Taskmaster(),
        LayerEnum.Traitor => new Traitor(),
        LayerEnum.Astral => new Astral(),
        LayerEnum.Bait => new Bait(),
        LayerEnum.Coward => new Coward(),
        LayerEnum.Diseased => new Diseased(),
        LayerEnum.Drunk => new Drunk(),
        LayerEnum.Dwarf => new Dwarf(),
        LayerEnum.Giant => new Giant(),
        LayerEnum.Indomitable => new Indomitable(),
        LayerEnum.Professional => new Professional(),
        LayerEnum.Shy => new Shy(),
        LayerEnum.VIP => new VIP(),
        LayerEnum.Volatile => new Volatile(),
        LayerEnum.Yeller => new Yeller(),
        LayerEnum.Colorblind => new Colorblind(),
        _ => rpc switch
        {
            PlayerLayerEnum.Role => new Roleless(),
            PlayerLayerEnum.Modifier => new Modifierless(),
            PlayerLayerEnum.Objectifier => new Objectifierless(),
            PlayerLayerEnum.Ability => new Abilityless(),
            _ => throw new NotImplementedException(id.ToString())
        }
    };

    public static void AssignChaosDrive()
    {
        var all = CustomPlayer.AllPlayers.Where(x => !x.HasDied() && x.Is(Faction.Syndicate) && x.IsBase(Faction.Syndicate)).ToList();

        if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !all.Any())
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