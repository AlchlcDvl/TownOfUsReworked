namespace TownOfUsReworked.Classes;

[HarmonyPatch]
public static class RoleGen
{
    private static readonly List<GenerationData> CrewAuditorRoles = new();
    private static readonly List<GenerationData> CrewKillingRoles = new();
    private static readonly List<GenerationData> CrewSupportRoles = new();
    private static readonly List<GenerationData> CrewSovereignRoles = new();
    private static readonly List<GenerationData> CrewProtectiveRoles = new();
    private static readonly List<GenerationData> CrewInvestigativeRoles = new();
    private static readonly List<GenerationData> CrewRoles = new();

    private static readonly List<GenerationData> NeutralEvilRoles = new();
    private static readonly List<GenerationData> NeutralBenignRoles = new();
    private static readonly List<GenerationData> NeutralKillingRoles = new();
    private static readonly List<GenerationData> NeutralNeophyteRoles = new();
    private static readonly List<GenerationData> NeutralHarbingerRoles = new();
    private static readonly List<GenerationData> NeutralRoles = new();

    private static readonly List<GenerationData> IntruderKillingRoles = new();
    private static readonly List<GenerationData> IntruderSupportRoles = new();
    private static readonly List<GenerationData> IntruderDeceptionRoles = new();
    private static readonly List<GenerationData> IntruderConcealingRoles = new();
    private static readonly List<GenerationData> IntruderRoles = new();

    private static readonly List<GenerationData> SyndicatePowerRoles = new();
    private static readonly List<GenerationData> SyndicateSupportRoles = new();
    private static readonly List<GenerationData> SyndicateKillingRoles = new();
    private static readonly List<GenerationData> SyndicateDisruptionRoles = new();
    private static readonly List<GenerationData> SyndicateRoles = new();

    private static readonly List<GenerationData> AllModifiers = new();
    private static readonly List<GenerationData> AllAbilities = new();
    private static readonly List<GenerationData> AllObjectifiers = new();
    private static readonly List<GenerationData> AllRoles = new();

    public static PlayerControl PureCrew;
    public static int Convertible;

    private static readonly List<LayerEnum> CA = new() { LayerEnum.Mystic, LayerEnum.VampireHunter };
    private static readonly List<LayerEnum> CI = new() { LayerEnum.Sheriff, LayerEnum.Inspector, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer,
        LayerEnum.Detective };
    private static readonly List<LayerEnum> CSv = new() { LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch };
    private static readonly List<LayerEnum> CP = new() { LayerEnum.Altruist, LayerEnum.Medic };
    private static readonly List<LayerEnum> CU = new() { LayerEnum.Crewmate };
    private static readonly List<LayerEnum> CK = new() { LayerEnum.Vigilante, LayerEnum.Veteran};
    private static readonly List<LayerEnum> CS = new() { LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Shifter, LayerEnum.Chameleon, LayerEnum.Retributionist };
    private static readonly List<List<LayerEnum>> Crew = new() { CA, CI, CSv, CP, CK, CS, CU };
    private static readonly List<LayerEnum> NB = new() { LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief };
    private static readonly List<LayerEnum> NE = new() { LayerEnum.Jester, LayerEnum.Actor, LayerEnum.BountyHunter, LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser,
        LayerEnum.Troll };
    private static readonly List<LayerEnum> NN = new() { LayerEnum.Jackal, LayerEnum.Necromancer, LayerEnum.Dracula, LayerEnum.Whisperer };
    private static readonly List<LayerEnum> NH = new() { LayerEnum.Plaguebearer };
    private static readonly List<LayerEnum> NA = new() { LayerEnum.Pestilence };
    private static readonly List<LayerEnum> NK = new() { LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.SerialKiller,
        LayerEnum.Werewolf };
    private static readonly List<List<LayerEnum>> Neutral = new() { NB, NE, NN, NH, NK };
    private static readonly List<LayerEnum> IC = new() { LayerEnum.Blackmailer, LayerEnum.Camouflager, LayerEnum.Grenadier, LayerEnum.Janitor };
    private static readonly List<LayerEnum> ID = new() { LayerEnum.Morphling, LayerEnum.Disguiser, LayerEnum.Wraith };
    private static readonly List<LayerEnum> IK = new() { LayerEnum.Enforcer, LayerEnum.Ambusher };
    private static readonly List<LayerEnum> IS = new() { LayerEnum.Consigliere, LayerEnum.Godfather, LayerEnum.Miner, LayerEnum.Teleporter, LayerEnum.Consort };
    private static readonly List<LayerEnum> IU = new() { LayerEnum.Impostor };
    private static readonly List<List<LayerEnum>> Intruders = new() { IC, ID, IK, IS, IU };
    private static readonly List<LayerEnum> SSu = new() { LayerEnum.Rebel, LayerEnum.Warper, LayerEnum.Stalker };
    private static readonly List<LayerEnum> SD = new() { LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer };
    private static readonly List<LayerEnum> SP = new() { LayerEnum.TimeKeeper, LayerEnum.Spellslinger };
    private static readonly List<LayerEnum> SyK = new() { LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner };
    private static readonly List<LayerEnum> SU = new() { LayerEnum.Anarchist };
    private static readonly List<List<LayerEnum>> Syndicate = new() { SSu, SyK, SD, SP, SU };
    private static readonly List<LayerEnum> AlignmentEntries = new() { LayerEnum.CrewSupport, LayerEnum.CrewInvest, LayerEnum.CrewSov, LayerEnum.CrewProt, LayerEnum.CrewKill,
        LayerEnum.CrewAudit, LayerEnum.IntruderSupport, LayerEnum.IntruderConceal, LayerEnum.IntruderDecep, LayerEnum.IntruderKill, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb,
        LayerEnum.NeutralBen, LayerEnum.NeutralEvil, LayerEnum.NeutralKill, LayerEnum.NeutralNeo, LayerEnum.SyndicateDisrup, LayerEnum.SyndicateKill, LayerEnum.SyndicatePower,
        LayerEnum.SyndicatePower, LayerEnum.IntruderUtil, LayerEnum.CrewUtil, LayerEnum.SyndicateUtil };
    private static readonly List<LayerEnum> RandomEntries = new() { LayerEnum.RandomCrew, LayerEnum.RandomIntruder, LayerEnum.RandomSyndicate, LayerEnum.RandomNeutral };
    private static readonly List<List<LayerEnum>> Alignments = new() { CA, CI, CSv, CP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA };

    private static void Sort(this List<GenerationData> items, int amount)
    {
        var newList = new List<GenerationData>();
        items.Shuffle();

        if (IsAA)
        {
            if (amount != CustomPlayer.AllPlayers.Count)
                amount = CustomPlayer.AllPlayers.Count;

            var rate = 0;

            while (newList.Count < amount && items.Count > 0)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Unique && CustomGameOptions.EnableUniques)
                    items.Remove(items[0]);
                else
                    rate++;

                if (rate > 10000)
                    break;
            }
        }
        else
        {
            if (items.Count < amount)
                amount = items.Count;

            var guaranteed = items.Where(x => x.Chance == 100).ToList();
            guaranteed.Shuffle();
            var optionals = items.Where(x => Check(x.Chance)).ToList();
            optionals.Shuffle();
            newList.AddRange(guaranteed);
            newList.AddRange(optionals);

            while (newList.Count > amount)
                newList.Remove(newList[^1]);
        }

        items = newList;
        items.Shuffle();
    }

    private static void GetAdjustedFactions(out int impostors, out int syndicate, out int neutrals, out int crew)
    {
        var players = GameData.Instance.PlayerCount;
        impostors = CustomGameOptions.IntruderCount;
        syndicate = CustomGameOptions.SyndicateCount;
        neutrals = IsKilling ? CustomGameOptions.NeutralRoles : URandom.RandomRangeInt(CustomGameOptions.NeutralMin, CustomGameOptions.NeutralMax);

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
        LogInfo("Role Gen Start");
        var players = CustomPlayer.AllPlayers;

        AllRoles.Clear();

        while (AllRoles.Count < (CustomGameOptions.AltImps ? CustomGameOptions.SyndicateCount : CustomGameOptions.IntruderCount))
            AllRoles.Add(GenerateSpawnItem(CustomGameOptions.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < players.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));

        AllRoles.Shuffle();
        LogInfo("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            LogMessage("Roles in the game: " + ids);
        }

        while (players.Count > 0 && AllRoles.Count > 0)
            Gen(players.TakeFirst(), (int)AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        LogInfo("Role Spawn Done");
    }

    private static void GenKilling()
    {
        LogInfo("Role Gen Start");
        var players = CustomPlayer.AllPlayers;
        GetAdjustedFactions(out var imps, out var syn, out var neutrals, out var crew);

        CrewRoles.Clear();
        IntruderRoles.Clear();
        SyndicateRoles.Clear();
        NeutralRoles.Clear();

        LogInfo("Lists Cleared - Killing Only");

        IntruderRoles.Clear();
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

        SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Anarchist));
        SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Bomber));
        SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Poisoner));
        SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Crusader));
        SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Collider));

        if (syn >= 3)
            SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Rebel));

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

        NeutralRoles.Sort(neutrals);

        var vigis = crew / 2;
        var vets = crew / 2;

        while (vigis > 0 || vets > 0)
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
        }

        LogInfo("Lists Set - Killing Only");

        IntruderRoles.Sort(imps);
        CrewRoles.Sort(crew);

        LogInfo("Killing Role List Sorted");

        AllRoles.AddRange(NeutralRoles);
        AllRoles.AddRange(CrewRoles);
        AllRoles.AddRange(SyndicateRoles);

        if (!CustomGameOptions.AltImps)
            AllRoles.AddRange(IntruderRoles);

        LogInfo("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            LogMessage("Roles in the game: " + ids);
        }

        AllRoles.Shuffle();

        while (players.Count > 0 && AllRoles.Count > 0)
            Gen(players.TakeFirst(), (int)AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        Role.SyndicateHasChaosDrive = true;
        AssignChaosDrive();
        LogInfo("Role Spawn Done");
    }

    private static void GenClassicCustomAA()
    {
        LogInfo("Role Gen Start");
        var players = CustomPlayer.AllPlayers;

        SetPostmortals.PhantomOn = Check(CustomGameOptions.PhantomOn);
        SetPostmortals.RevealerOn = Check(CustomGameOptions.RevealerOn);
        SetPostmortals.BansheeOn = Check(CustomGameOptions.BansheeOn);
        SetPostmortals.GhoulOn = Check(CustomGameOptions.GhoulOn);

        GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);

        var num = 0;

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

        if (CustomGameOptions.InspectorOn > 0)
        {
            num = CustomGameOptions.InspectorCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Inspector));
                num--;
            }

            LogInfo("Inspector Done");
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

        if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.JackalOn >
            0))
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
            CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||
            CustomGameOptions.TraitorOn > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 ||
            CustomGameOptions.GuardianAngelOn > 0 || CustomGameOptions.GuesserOn > 0 || CustomGameOptions.ShifterOn > 0))
        {
            num = CustomGameOptions.SeerCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateSpawnItem(LayerEnum.Seer));
                num--;
            }

            LogInfo("Seer Done");
        }

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

        if (CustomGameOptions.MinerOn > 0)
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
                IntruderSupportRoles.Add(GenerateSpawnItem(LayerEnum.Godfather));
                num--;
            }

            LogInfo("Godfather Done");
        }

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
                SyndicateSupportRoles.Add(GenerateSpawnItem(LayerEnum.Rebel));
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

        if (CustomGameOptions.WarperOn > 0 && (int)CustomGameOptions.Map is not (4 or 5 or 6))
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

        if (CustomGameOptions.TimeKeeperOn > 0)
        {
            num = CustomGameOptions.TimeKeeperCount;

            while (num > 0)
            {
                SyndicatePowerRoles.Add(GenerateSpawnItem(LayerEnum.TimeKeeper));
                num--;
            }

            LogInfo("Time Keeper Done");
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

        if (IsClassic || IsCustom)
        {
            if (!CustomGameOptions.AltImps)
            {
                var maxIC = CustomGameOptions.ICMax;
                var maxID = CustomGameOptions.IDMax;
                var maxIK = CustomGameOptions.IKMax;
                var maxIS = CustomGameOptions.ISMax;
                var minInt = CustomGameOptions.IntruderMin;
                var maxInt = CustomGameOptions.IntruderMax;
                var maxIntSum = maxIC + maxID + maxIK + maxIS;

                while (maxIntSum > maxInt)
                {
                    switch (URandom.RandomRangeInt(0, 4))
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
                    }

                    maxIntSum = maxIC + maxID + maxIK + maxIS;
                }

                IntruderConcealingRoles.Sort(maxIC);
                IntruderDeceptionRoles.Sort(maxID);
                IntruderKillingRoles.Sort(maxIK);
                IntruderSupportRoles.Sort(maxIS);

                IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles);

                while (maxInt > imps)
                    maxInt--;

                while (minInt > imps)
                    minInt--;

                IntruderRoles.Sort(URandom.RandomRangeInt(minInt, maxInt + 1));

                while (IntruderRoles.Count < imps)
                    IntruderRoles.Add(GenerateSpawnItem(LayerEnum.Impostor));

                IntruderRoles.Shuffle();
            }

            if (CustomGameOptions.SyndicateCount > 0)
            {
                var maxSSu = CustomGameOptions.SSuMax;
                var maxSD = CustomGameOptions.SDMax;
                var maxSyK = CustomGameOptions.SyKMax;
                var maxSP = CustomGameOptions.SPMax;
                var minSyn = CustomGameOptions.SyndicateMin;
                var maxSyn = CustomGameOptions.SyndicateMax;
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

                SyndicateSupportRoles.Sort(maxSSu);
                SyndicateDisruptionRoles.Sort(maxSD);
                SyndicateKillingRoles.Sort(maxSyK);
                SyndicatePowerRoles.Sort(maxSP);

                SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);

                while (maxSyn > syn)
                    maxSyn--;

                while (minSyn > syn)
                    minSyn--;

                SyndicateRoles.Sort(URandom.RandomRangeInt(minSyn, maxSyn + 1));

                while (SyndicateRoles.Count < syn)
                    SyndicateRoles.Add(GenerateSpawnItem(LayerEnum.Anarchist));

                SyndicateRoles.Shuffle();
            }

            if (CustomGameOptions.NeutralMax > 0)
            {
                var maxNE = CustomGameOptions.NEMax;
                var maxNB = CustomGameOptions.NBMax;
                var maxNK = CustomGameOptions.NKMax;
                var maxNN = CustomGameOptions.NNMax;
                var maxNH = CustomGameOptions.NHMax;
                var minNeut = CustomGameOptions.NeutralMin;
                var maxNeut = CustomGameOptions.NeutralMax;
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

                NeutralBenignRoles.Sort(maxNB);
                NeutralEvilRoles.Sort(maxNE);
                NeutralKillingRoles.Sort(maxNK);
                NeutralNeophyteRoles.Sort(maxNN);
                NeutralHarbingerRoles.Sort(maxNH);

                NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

                while (maxNeut > neut)
                    maxNeut--;

                while (minNeut > neut)
                    minNeut--;

                NeutralRoles.Sort(URandom.RandomRangeInt(minNeut, maxNeut + 1));
                NeutralRoles.Shuffle();
            }

            if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
            {
                var maxCI = CustomGameOptions.CIMax;
                var maxCS = CustomGameOptions.CSMax;
                var maxCA = CustomGameOptions.CAMax;
                var maxCK = CustomGameOptions.CKMax;
                var maxCP = CustomGameOptions.CPMax;
                var maxCSv = CustomGameOptions.CSvMax;
                var minCrew = CustomGameOptions.CrewMin;
                var maxCrew = CustomGameOptions.CrewMax;
                var maxCrewSum = maxCA + maxCI + maxCK + maxCP + maxCS + maxCSv;

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
                            if (maxCP > 0) maxCP--;
                            break;
                    }

                    maxCrewSum = maxCA + maxCI + maxCK + maxCP + maxCS + maxCSv;
                }

                CrewAuditorRoles.Sort(maxCA);
                CrewInvestigativeRoles.Sort(maxCI);
                CrewKillingRoles.Sort(maxCK);
                CrewProtectiveRoles.Sort(maxCP);
                CrewSupportRoles.Sort(maxCS);
                CrewSovereignRoles.Sort(maxCSv);

                CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);

                while (maxCrew > crew)
                    maxCrew--;

                while (minCrew > crew)
                    minCrew--;

                CrewRoles.Sort(URandom.RandomRangeInt(minCrew, maxCrew + 1));

                while (CrewRoles.Count < crew)
                    CrewRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));

                CrewRoles.Shuffle();
            }

            LogInfo("Classic/Custom Sorting Done");
        }
        else if (IsAA)
        {
            CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);
            IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles);
            SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);
            NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

            CrewRoles.Shuffle();
            SyndicateRoles.Shuffle();
            IntruderRoles.Shuffle();
            NeutralRoles.Shuffle();

            LogInfo("All Any Sorting Done");
        }

        if (CustomGameOptions.AltImps)
        {
            IntruderRoles.Clear();
            IntruderRoles.AddRange(SyndicateRoles);
            SyndicateRoles.Clear();
        }
        else
            SyndicateRoles.Sort(syn);

        IntruderRoles.Sort(CustomGameOptions.AltImps ? syn : imps);
        CrewRoles.Sort(crew);
        NeutralRoles.Sort(neut);
        AllRoles.AddRanges(IntruderRoles, CrewRoles, NeutralRoles, SyndicateRoles);

        while (AllRoles.Count < players.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));

        if (!AllRoles.Any(x => CrewRoles.Contains(x) || x.ID == LayerEnum.Crewmate))
        {
            AllRoles.Remove(AllRoles.Random());
            AllRoles.Add(CrewRoles.Count > 0 ? CrewRoles.Random() : GenerateSpawnItem(LayerEnum.Crewmate));

            if (TownOfUsReworked.IsTest)
                LogInfo("Added Solo Crew");
        }

        if (!AllRoles.Any(x => x.ID == LayerEnum.Dracula) && AllRoles.Any(x => x.ID == LayerEnum.VampireHunter))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.VampireHunter);

            while (count > 0)
            {
                AllRoles.Add(GenerateSpawnItem(LayerEnum.Vigilante));
                count--;
            }
        }

        while (AllRoles.Count > players.Count)
        {
            AllRoles.Shuffle();
            AllRoles.Remove(AllRoles[^1]);
        }

        AllRoles.Shuffle();
        LogInfo("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            LogMessage("Roles in the game: " + ids);
        }

        while (players.Count > 0 && AllRoles.Count > 0)
            Gen(players.TakeFirst(), (int)AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        LogInfo("Role Gen End");
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) || CustomOption.GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Any(x =>
        x.Name.Contains("Ban") && x.Get() == id) || (id == LayerEnum.Crewmate && CustomGameOptions.BanCrewmate) || (id == LayerEnum.Impostor && CustomGameOptions.BanImpostor) || (id ==
        LayerEnum.Anarchist && CustomGameOptions.BanAnarchist);

    private static void GenRoleList()
    {
        LogInfo("Role Gen Start");
        var players = CustomPlayer.AllPlayers;
        var entries = CustomOption.GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Where(x => x.Name.Contains("Entry"));
        var bans = CustomOption.GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Where(x => x.Name.Contains("Ban"));
        var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get())).ToList();
        var randoms = entries.Where(x => RandomEntries.Contains(x.Get())).ToList();
        var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get()))).ToList();
        var anies = entries.Where(x => x.Get() == LayerEnum.Any).ToList();
        //I have no idea what plural for any is lmao

        AllRoles.Clear();

        SetPostmortals.PhantomOn = CustomGameOptions.EnablePhantom;
        SetPostmortals.RevealerOn = CustomGameOptions.EnableRevealer;
        SetPostmortals.BansheeOn = CustomGameOptions.EnableBanshee;
        SetPostmortals.GhoulOn = CustomGameOptions.EnableGhoul;

        foreach (var entry in roles)
        {
            var ratelimit = 0;
            var id = entry.Get();
            var cachedCount = AllRoles.Count;

            while (cachedCount == AllRoles.Count)
            {
                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(id));

                if (ratelimit > 10000)
                    break;
            }
        }

        foreach (var entry in alignments)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;
            var random = LayerEnum.None;

            while (cachedCount == AllRoles.Count)
            {
                if (id == LayerEnum.CrewAudit)
                    random = CA.Random();
                else if (id == LayerEnum.CrewInvest)
                    random = CI.Random();
                else if (id == LayerEnum.CrewSov)
                    random = CSv.Random();
                else if (id == LayerEnum.CrewProt)
                    random = CP.Random();
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

                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(random));

                if (ratelimit > 10000)
                    break;
            }
        }

        foreach (var entry in randoms)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;
            var random = LayerEnum.None;

            while (cachedCount == AllRoles.Count)
            {
                if (id == LayerEnum.RandomCrew)
                    random = Crew.Random().Random();
                else if (id == LayerEnum.RandomNeutral)
                    random = Neutral.Random().Random();
                else if (id == LayerEnum.RandomIntruder)
                    random = Intruders.Random().Random();
                else if (id == LayerEnum.RandomSyndicate)
                    random = Syndicate.Random().Random();

                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(random));

                if (ratelimit > 10000)
                    break;
            }
        }

        foreach (var entry in anies)
        {
            var cachedCount = AllRoles.Count;
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count)
            {
                var random = Alignments.Random().Random();

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GenerateSpawnItem(random));

                if (ratelimit > 10000)
                    break;
            }
        }

        //Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        //In case if the ratelimits disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < players.Count)
            AllRoles.Add(GenerateSpawnItem(LayerEnum.Crewmate));

        LogInfo("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            LogMessage("Roles in the game: " + ids);
        }

        while (players.Count > 0 && AllRoles.Count > 0)
            Gen(players.TakeFirst(), (int)AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        LogInfo("Role Spawn Done");
    }

    private static GenerationData GenerateSpawnItem(LayerEnum id)
    {
        var things = id switch
        {
            LayerEnum.Mayor => (CustomGameOptions.MayorOn, CustomGameOptions.UniqueMayor),
            LayerEnum.Sheriff => (CustomGameOptions.SheriffOn, CustomGameOptions.UniqueSheriff),
            LayerEnum.Inspector => (CustomGameOptions.InspectorOn, CustomGameOptions.UniqueInspector),
            LayerEnum.Vigilante => (IsKilling ? 100 : CustomGameOptions.VigilanteOn, CustomGameOptions.UniqueVigilante),
            LayerEnum.Engineer => (CustomGameOptions.EngineerOn, CustomGameOptions.UniqueEngineer),
            LayerEnum.Monarch => (CustomGameOptions.MonarchOn, CustomGameOptions.UniqueMonarch),
            LayerEnum.Dictator => (CustomGameOptions.DictatorOn, CustomGameOptions.UniqueDictator),
            LayerEnum.Collider => (CustomGameOptions.ColliderOn, CustomGameOptions.UniqueCollider),
            LayerEnum.Medic => (CustomGameOptions.MedicOn, CustomGameOptions.UniqueMedic),
            LayerEnum.Stalker => (CustomGameOptions.StalkerOn, CustomGameOptions.UniqueStalker),
            LayerEnum.Altruist => (CustomGameOptions.AltruistOn, CustomGameOptions.UniqueAltruist),
            LayerEnum.Veteran => (IsKilling ? 100 : CustomGameOptions.VeteranOn, CustomGameOptions.UniqueVeteran),
            LayerEnum.Tracker => (CustomGameOptions.TrackerOn, CustomGameOptions.UniqueTracker),
            LayerEnum.Transporter => (CustomGameOptions.TransporterOn, CustomGameOptions.UniqueTransporter),
            LayerEnum.Medium => (CustomGameOptions.MediumOn, CustomGameOptions.UniqueMedium),
            LayerEnum.Coroner => (CustomGameOptions.CoronerOn, CustomGameOptions.UniqueCoroner),
            LayerEnum.Operative => (CustomGameOptions.OperativeOn, CustomGameOptions.UniqueOperative),
            LayerEnum.Detective => (CustomGameOptions.DetectiveOn, CustomGameOptions.UniqueDetective),
            LayerEnum.Escort => (CustomGameOptions.EscortOn, CustomGameOptions.UniqueDetective),
            LayerEnum.Shifter => (CustomGameOptions.ShifterOn, CustomGameOptions.UniqueShifter),
            LayerEnum.Crewmate => (CustomGameOptions.CrewmateOn, false),
            LayerEnum.VampireHunter => (CustomGameOptions.VampireHunterOn, CustomGameOptions.UniqueVampireHunter),
            LayerEnum.Jester => (CustomGameOptions.JesterOn, CustomGameOptions.UniqueJester),
            LayerEnum.Amnesiac => (CustomGameOptions.AmnesiacOn, CustomGameOptions.UniqueAmnesiac),
            LayerEnum.Executioner => (CustomGameOptions.ExecutionerOn, CustomGameOptions.UniqueExecutioner),
            LayerEnum.Survivor => (CustomGameOptions.SurvivorOn, CustomGameOptions.UniqueSurvivor),
            LayerEnum.GuardianAngel => (CustomGameOptions.GuardianAngelOn, CustomGameOptions.UniqueGuardianAngel),
            LayerEnum.Glitch => (CustomGameOptions.GlitchOn, CustomGameOptions.UniqueGlitch),
            LayerEnum.Murderer => (IsKilling ? 5 : CustomGameOptions.MurdererOn, CustomGameOptions.UniqueMurderer),
            LayerEnum.Cryomaniac => (CustomGameOptions.CryomaniacOn, CustomGameOptions.UniqueCryomaniac),
            LayerEnum.Werewolf => (CustomGameOptions.WerewolfOn, CustomGameOptions.UniqueWerewolf),
            LayerEnum.Arsonist => (CustomGameOptions.ArsonistOn, CustomGameOptions.UniqueArsonist),
            LayerEnum.Jackal => (CustomGameOptions.JackalOn, CustomGameOptions.UniqueJackal),
            LayerEnum.Plaguebearer => (CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer),
            LayerEnum.Pestilence => (CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer),
            LayerEnum.SerialKiller => (CustomGameOptions.SerialKillerOn, CustomGameOptions.UniqueSerialKiller),
            LayerEnum.Juggernaut => (CustomGameOptions.JuggernautOn, CustomGameOptions.UniqueJuggernaut),
            LayerEnum.Cannibal => (CustomGameOptions.CannibalOn, CustomGameOptions.UniqueCannibal),
            LayerEnum.Thief => (CustomGameOptions.ThiefOn, CustomGameOptions.UniqueThief),
            LayerEnum.Dracula => (CustomGameOptions.DraculaOn, CustomGameOptions.UniqueDracula),
            LayerEnum.Troll => (CustomGameOptions.TrollOn, CustomGameOptions.UniqueTroll),
            LayerEnum.Enforcer => (CustomGameOptions.EnforcerOn, CustomGameOptions.UniqueEnforcer),
            LayerEnum.Morphling => (CustomGameOptions.MorphlingOn, CustomGameOptions.UniqueMorphling),
            LayerEnum.Blackmailer => (CustomGameOptions.BlackmailerOn, CustomGameOptions.UniqueBlackmailer),
            LayerEnum.Miner => (CustomGameOptions.MinerOn, CustomGameOptions.UniqueMiner),
            LayerEnum.Teleporter => (CustomGameOptions.TeleporterOn, CustomGameOptions.UniqueTeleporter),
            LayerEnum.Wraith => (CustomGameOptions.WraithOn, CustomGameOptions.UniqueWraith),
            LayerEnum.Consort => (CustomGameOptions.ConsortOn, CustomGameOptions.UniqueConsort),
            LayerEnum.Janitor => (CustomGameOptions.JanitorOn, CustomGameOptions.UniqueJanitor),
            LayerEnum.Camouflager => (CustomGameOptions.CamouflagerOn, CustomGameOptions.UniqueCamouflager),
            LayerEnum.Grenadier => (CustomGameOptions.GrenadierOn, CustomGameOptions.UniqueGrenadier),
            LayerEnum.Poisoner => (CustomGameOptions.PoisonerOn, CustomGameOptions.UniquePoisoner),
            LayerEnum.Impostor => (IsKilling ? 5 : CustomGameOptions.ImpostorOn, false),
            LayerEnum.Consigliere => (CustomGameOptions.ConsigliereOn, CustomGameOptions.UniqueConsigliere),
            LayerEnum.Disguiser => (CustomGameOptions.DisguiserOn, CustomGameOptions.UniqueDisguiser),
            LayerEnum.Spellslinger => (CustomGameOptions.SpellslingerOn, CustomGameOptions.UniqueSpellslinger),
            LayerEnum.Godfather => (CustomGameOptions.GodfatherOn, CustomGameOptions.UniqueGodfather),
            LayerEnum.Anarchist => (IsKilling ? 5 : CustomGameOptions.AnarchistOn, false),
            LayerEnum.Shapeshifter => (CustomGameOptions.ShapeshifterOn, CustomGameOptions.UniqueShapeshifter),
            LayerEnum.Drunkard => (CustomGameOptions.DrunkardOn, CustomGameOptions.UniqueDrunk),
            LayerEnum.Framer => (CustomGameOptions.FramerOn, CustomGameOptions.UniqueFramer),
            LayerEnum.Rebel => (CustomGameOptions.RebelOn, CustomGameOptions.UniqueRebel),
            LayerEnum.Concealer => (CustomGameOptions.ConcealerOn, CustomGameOptions.UniqueConcealer),
            LayerEnum.Warper => (CustomGameOptions.WarperOn, CustomGameOptions.UniqueConcealer),
            LayerEnum.Bomber => (CustomGameOptions.BomberOn, CustomGameOptions.UniqueBomber),
            LayerEnum.Chameleon => (CustomGameOptions.ChameleonOn, CustomGameOptions.UniqueChameleon),
            LayerEnum.Guesser => (CustomGameOptions.GuesserOn, CustomGameOptions.UniqueGuesser),
            LayerEnum.Whisperer => (CustomGameOptions.WhispererOn, CustomGameOptions.UniqueWhisperer),
            LayerEnum.Retributionist => (CustomGameOptions.RetributionistOn, CustomGameOptions.UniqueRetributionist),
            LayerEnum.Actor => (CustomGameOptions.ActorOn, CustomGameOptions.UniqueActor),
            LayerEnum.BountyHunter => (CustomGameOptions.BountyHunterOn, CustomGameOptions.UniqueBountyHunter),
            LayerEnum.Mystic => (CustomGameOptions.MysticOn, CustomGameOptions.UniqueMystic),
            LayerEnum.Seer => (CustomGameOptions.SeerOn, CustomGameOptions.UniqueSeer),
            LayerEnum.Necromancer => (CustomGameOptions.NecromancerOn, CustomGameOptions.UniqueNecromancer),
            LayerEnum.TimeKeeper => (CustomGameOptions.TimeKeeperOn, CustomGameOptions.UniqueTimeKeeper),
            LayerEnum.Ambusher => (CustomGameOptions.AmbusherOn, CustomGameOptions.UniqueAmbusher),
            LayerEnum.Crusader => (CustomGameOptions.CrusaderOn, CustomGameOptions.UniqueCrusader),
            LayerEnum.Silencer => (CustomGameOptions.SilencerOn, CustomGameOptions.UniqueSilencer),
            LayerEnum.CrewAssassin => (CustomGameOptions.CrewAssassinOn, CustomGameOptions.UniqueCrewAssassin),
            LayerEnum.IntruderAssassin => (CustomGameOptions.IntruderAssassinOn, CustomGameOptions.UniqueIntruderAssassin),
            LayerEnum.SyndicateAssassin => (CustomGameOptions.SyndicateAssassinOn, CustomGameOptions.UniqueSyndicateAssassin),
            LayerEnum.NeutralAssassin => (CustomGameOptions.NeutralAssassinOn, CustomGameOptions.UniqueNeutralAssassin),
            LayerEnum.ButtonBarry => (CustomGameOptions.ButtonBarryOn, CustomGameOptions.UniqueCrewAssassin),
            LayerEnum.Insider => (CustomGameOptions.InsiderOn, CustomGameOptions.UniqueButtonBarry),
            LayerEnum.Multitasker => (CustomGameOptions.MultitaskerOn, CustomGameOptions.UniqueMultitasker),
            LayerEnum.Ninja => (CustomGameOptions.NinjaOn, CustomGameOptions.UniqueNinja),
            LayerEnum.Politician => (CustomGameOptions.PoliticianOn, CustomGameOptions.UniquePolitician),
            LayerEnum.Radar => (CustomGameOptions.RadarOn, CustomGameOptions.UniqueRadar),
            LayerEnum.Ruthless => (CustomGameOptions.RuthlessOn, CustomGameOptions.UniqueRuthless),
            LayerEnum.Snitch => (CustomGameOptions.SnitchOn, CustomGameOptions.UniqueSnitch),
            LayerEnum.Swapper => (CustomGameOptions.SwapperOn, CustomGameOptions.UniqueSwapper),
            LayerEnum.Tiebreaker => (CustomGameOptions.TiebreakerOn, CustomGameOptions.UniqueTiebreaker),
            LayerEnum.Torch => (CustomGameOptions.TorchOn, CustomGameOptions.UniqueTorch),
            LayerEnum.Tunneler => (CustomGameOptions.TunnelerOn, CustomGameOptions.UniqueTunneler),
            LayerEnum.Underdog => (CustomGameOptions.UnderdogOn, CustomGameOptions.UniqueUnderdog),
            LayerEnum.Astral => (CustomGameOptions.AstralOn, CustomGameOptions.UniqueAstral),
            LayerEnum.Bait => (CustomGameOptions.BaitOn, CustomGameOptions.UniqueBait),
            LayerEnum.Coward => (CustomGameOptions.CowardOn, CustomGameOptions.UniqueCoward),
            LayerEnum.Diseased => (CustomGameOptions.DiseasedOn, CustomGameOptions.UniqueDiseased),
            LayerEnum.Drunk => (CustomGameOptions.DrunkOn, CustomGameOptions.UniqueDrunk),
            LayerEnum.Dwarf => (CustomGameOptions.DwarfOn, CustomGameOptions.UniqueDwarf),
            LayerEnum.Giant => (CustomGameOptions.GiantOn, CustomGameOptions.UniqueGiant),
            LayerEnum.Indomitable => (CustomGameOptions.IndomitableOn, CustomGameOptions.UniqueIndomitable),
            LayerEnum.Professional => (CustomGameOptions.ProfessionalOn, CustomGameOptions.UniqueProfessional),
            LayerEnum.Shy => (CustomGameOptions.ShyOn, CustomGameOptions.UniqueShy),
            LayerEnum.VIP => (CustomGameOptions.VIPOn, CustomGameOptions.UniqueVIP),
            LayerEnum.Volatile => (CustomGameOptions.VolatileOn, CustomGameOptions.UniqueVolatile),
            LayerEnum.Yeller => (CustomGameOptions.YellerOn, CustomGameOptions.UniqueYeller),
            LayerEnum.Allied => (CustomGameOptions.AlliedOn, CustomGameOptions.UniqueAllied),
            LayerEnum.Corrupted => (CustomGameOptions.CorruptedOn, CustomGameOptions.UniqueCorrupted),
            LayerEnum.Defector => (CustomGameOptions.DefectorOn, CustomGameOptions.UniqueDefector),
            LayerEnum.Fanatic => (CustomGameOptions.FanaticOn, CustomGameOptions.UniqueFanatic),
            LayerEnum.Linked => (CustomGameOptions.LinkedOn, CustomGameOptions.UniqueLinked),
            LayerEnum.Lovers => (CustomGameOptions.LoversOn, CustomGameOptions.UniqueLovers),
            LayerEnum.Mafia => (CustomGameOptions.MafiaOn, CustomGameOptions.UniqueMafia),
            LayerEnum.Overlord => (CustomGameOptions.OverlordOn, CustomGameOptions.UniqueOverlord),
            LayerEnum.Rivals => (CustomGameOptions.RivalsOn, CustomGameOptions.UniqueRivals),
            LayerEnum.Taskmaster => (CustomGameOptions.TaskmasterOn, CustomGameOptions.UniqueTaskmaster),
            LayerEnum.Traitor => (CustomGameOptions.TraitorOn, CustomGameOptions.UniqueTraitor),
            _ => throw new NotImplementedException(),
        };

        return new(things.Item1, id, things.Item2);
    }

    private static void GenAbilities()
    {
        AllAbilities.Clear();
        var num = 0;

        if (CustomGameOptions.CrewAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfCrewAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.CrewAssassin));
                num--;
            }

            LogInfo("Crew Assassin Done");
        }

        if (CustomGameOptions.SyndicateAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfSyndicateAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.SyndicateAssassin));
                num--;
            }

            LogInfo("Syndicate Assassin Done");
        }

        if (CustomGameOptions.IntruderAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfIntruderAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.IntruderAssassin));
                num--;
            }

            LogInfo("Intruder Assassin Done");
        }

        if (CustomGameOptions.NeutralAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfNeutralAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateSpawnItem(LayerEnum.NeutralAssassin));
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

        if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
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

        if (CustomGameOptions.TunnelerOn > 0 && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
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

        AllAbilities.Sort(URandom.RandomRangeInt(minAb, maxAb + 1));

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

        AllAbilities.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllAbilities)
                ids += $" {spawn.ID}";

            LogMessage("Abilities in the game: " + ids);
        }

        while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
            canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
            canHaveTaskedAbility.Count > 0 || canHaveTorch.Count > 0 || canHaveKillingAbility.Count > 0)
        {
            if (AllAbilities.Count == 0)
                break;

            var id = AllAbilities.TakeFirst().ID;
            LayerEnum[] Snitch = { LayerEnum.Snitch };
            LayerEnum[] Syndicate = { LayerEnum.SyndicateAssassin };
            LayerEnum[] Crew = { LayerEnum.CrewAssassin, LayerEnum.Swapper };
            LayerEnum[] Neutral = { LayerEnum.NeutralAssassin };
            LayerEnum[] Intruder = { LayerEnum.IntruderAssassin };
            LayerEnum[] Killing = { LayerEnum.Ninja };
            LayerEnum[] Torch = { LayerEnum.Torch };
            LayerEnum[] Evil = { LayerEnum.Underdog };
            LayerEnum[] Tasked = { LayerEnum.Insider, LayerEnum.Multitasker };
            LayerEnum[] Global = { LayerEnum.Radar, LayerEnum.Tiebreaker };
            LayerEnum[] Tunneler = { LayerEnum.Tunneler };
            LayerEnum[] BB = { LayerEnum.ButtonBarry };
            LayerEnum[] Pol = { LayerEnum.Politician };
            LayerEnum[] Ruth = { LayerEnum.Ruthless };

            PlayerControl assigned = null;

            if (canHaveSnitch.Count > 0 && Snitch.Contains(id))
                assigned = canHaveSnitch.TakeFirst();
            else if (canHaveSyndicateAbility.Count > 0 && Syndicate.Contains(id))
                assigned = canHaveSyndicateAbility.TakeFirst();
            else if (canHaveCrewAbility.Count > 0 && Crew.Contains(id))
                assigned = canHaveCrewAbility.TakeFirst();
            else if (canHaveNeutralAbility.Count > 0 && Neutral.Contains(id))
                assigned = canHaveNeutralAbility.TakeFirst();
            else if (canHaveIntruderAbility.Count > 0 && Intruder.Contains(id))
                assigned = canHaveIntruderAbility.TakeFirst();
            else if (canHaveKillingAbility.Count > 0 && Killing.Contains(id))
                assigned = canHaveKillingAbility.TakeFirst();
            else if (canHaveTorch.Count > 0 && Torch.Contains(id))
                assigned = canHaveTorch.TakeFirst();
            else if (canHaveEvilAbility.Count > 0 && Evil.Contains(id))
                assigned = canHaveEvilAbility.TakeFirst();
            else if (canHaveTaskedAbility.Count > 0 && Tasked.Contains(id))
                assigned = canHaveTaskedAbility.TakeFirst();
            else if (canHaveTunnelerAbility.Count > 0 && Tunneler.Contains(id) && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
                assigned = canHaveTunnelerAbility.TakeFirst();
            else if (canHaveBB.Count > 0 && BB.Contains(id))
                assigned = canHaveBB.TakeFirst();
            else if (canHavePolitician.Count > 0 && Pol.Contains(id))
                assigned = canHavePolitician.TakeFirst();
            else if (canHaveAbility.Count > 0 && Global.Contains(id))
                assigned = canHaveAbility.TakeFirst();
            else if (canHaveRuthless.Count > 0 && Ruth.Contains(id))
                assigned = canHaveRuthless.TakeFirst();

            if (assigned != null)
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

                if (Ability.GetAbility(assigned) == null)
                    Gen(assigned, (int)id, PlayerLayerEnum.Ability);
                else
                    AllAbilities.Add(GenerateSpawnItem(id));
            }
        }

        LogInfo("Abilities Done");
    }

    private static void GenObjectifiers()
    {
        AllObjectifiers.Clear();
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

        AllObjectifiers.Sort(URandom.RandomRangeInt(minObj, maxObj + 1));

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

        AllObjectifiers.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllObjectifiers)
                ids += $" {spawn.ID}";

            LogMessage("Objectifiers in the game: " + ids);
        }

        while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 1 || canHaveObjectifier.Count > 0 || canHaveDefector.Count > 0)
        {
            if (AllObjectifiers.Count == 0)
                break;

            var id = AllObjectifiers.TakeFirst().ID;
            LayerEnum[] LoverRival = { LayerEnum.Lovers, LayerEnum.Rivals };
            LayerEnum[] Crew = { LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor };
            LayerEnum[] Neutral = { LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked };
            LayerEnum[] Allied = { LayerEnum.Allied };
            LayerEnum[] Global = { LayerEnum.Mafia };
            LayerEnum[] Defect = { LayerEnum.Defector };

            PlayerControl assigned = null;
            PlayerControl assignedOther = null;

            if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 1)
            {
                assigned = canHaveLoverorRival.TakeFirst();
                assignedOther = canHaveLoverorRival.TakeFirst();
            }
            else if (Crew.Contains(id) && canHaveCrewObjectifier.Count > 0)
                assigned = canHaveCrewObjectifier.TakeFirst();
            else if (Neutral.Contains(id) && canHaveNeutralObjectifier.Count > 0)
            {
                assigned = canHaveNeutralObjectifier.TakeFirst();

                if (id == LayerEnum.Linked)
                    assignedOther = canHaveNeutralObjectifier.TakeFirst();
            }
            else if (Allied.Contains(id) && canHaveAllied.Count > 0)
                assigned = canHaveAllied.TakeFirst();
            else if (Global.Contains(id) && canHaveObjectifier.Count > 1)
                assigned = canHaveObjectifier.TakeFirst();
            else if (Defect.Contains(id) && canHaveDefector.Count > 1)
                assigned = canHaveDefector.TakeFirst();

            if (assigned != null)
            {
                canHaveLoverorRival.Remove(assigned);
                canHaveCrewObjectifier.Remove(assigned);
                canHaveNeutralObjectifier.Remove(assigned);
                canHaveAllied.Remove(assigned);
                canHaveObjectifier.Remove(assigned);
                canHaveDefector.Remove(assigned);

                if (Objectifier.GetObjectifier(assigned) == null)
                    Gen(assigned, (int)id, PlayerLayerEnum.Objectifier);
                else
                    AllObjectifiers.Add(GenerateSpawnItem(id));
            }

            if (assignedOther != null)
            {
                canHaveLoverorRival.Remove(assignedOther);
                canHaveCrewObjectifier.Remove(assignedOther);
                canHaveNeutralObjectifier.Remove(assignedOther);
                canHaveAllied.Remove(assignedOther);
                canHaveObjectifier.Remove(assignedOther);
                canHaveDefector.Remove(assignedOther);

                if (Objectifier.GetObjectifier(assignedOther) == null)
                    Gen(assignedOther, (int)id, PlayerLayerEnum.Objectifier);
            }
        }

        LogInfo("Objectifiers Done");
    }

    private static void GenModifiers()
    {
        AllModifiers.Clear();
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


        var maxMod = CustomGameOptions.MaxModifiers;
        var minMod = CustomGameOptions.MinModifiers;

        while (maxMod > CustomPlayer.AllPlayers.Count)
            maxMod--;

        while (minMod > CustomPlayer.AllPlayers.Count)
            minMod--;

        AllModifiers.Sort(URandom.RandomRangeInt(minMod, maxMod + 1));

        var canHaveBait = CustomPlayer.AllPlayers;
        var canHaveDiseased = CustomPlayer.AllPlayers;
        var canHaveProfessional = CustomPlayer.AllPlayers;
        var canHaveModifier = CustomPlayer.AllPlayers;
        var canHaveShy = CustomPlayer.AllPlayers;

        canHaveBait.RemoveAll(x => x.Is(LayerEnum.Vigilante) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Thief) || x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll));
        canHaveBait.Shuffle();

        canHaveDiseased.RemoveAll(x => x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll));
        canHaveDiseased.Shuffle();

        canHaveProfessional.RemoveAll(x => !(x.Is(LayerEnum.CrewAssassin) || x.Is(LayerEnum.NeutralAssassin) || x.Is(LayerEnum.IntruderAssassin) ||
            x.Is(LayerEnum.SyndicateAssassin)));
        canHaveProfessional.Shuffle();

        canHaveShy.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(LayerEnum.Jester) && !CustomGameOptions.JesterButton) || (x.Is(LayerEnum.Swapper) &&
            !CustomGameOptions.SwapperButton) || (x.Is(LayerEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(LayerEnum.Guesser) && !CustomGameOptions.GuesserButton) ||
            (x.Is(LayerEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || x.Is(LayerEnum.ButtonBarry) || (x.Is(LayerEnum.Politician) && !CustomGameOptions.PoliticianButton) ||
            (!CustomGameOptions.DictatorButton && x.Is(LayerEnum.Dictator)) || (!CustomGameOptions.MonarchButton && x.Is(LayerEnum.Monarch)));
        canHaveShy.Shuffle();

        AllModifiers.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllModifiers)
                ids += $" {spawn.ID}";

            LogMessage("Modifiers in the game: " + ids);
        }

        while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
        {
            if (AllModifiers.Count == 0)
                break;

            var id = AllModifiers.TakeFirst().ID;
            LayerEnum[] Bait = { LayerEnum.Bait };
            LayerEnum[] Diseased = { LayerEnum.Diseased };
            LayerEnum[] Professional = { LayerEnum.Professional };
            LayerEnum[] Global = { LayerEnum.Dwarf, LayerEnum.VIP, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral, LayerEnum.Indomitable,
                LayerEnum.Yeller };
            LayerEnum[] Shy = { LayerEnum.Shy };

            PlayerControl assigned = null;

            if (canHaveBait.Count > 0 && Bait.Contains(id))
                assigned = canHaveBait.TakeFirst();
            else if (canHaveDiseased.Count > 0 && Diseased.Contains(id))
                assigned = canHaveDiseased.TakeFirst();
            else if (canHaveProfessional.Count > 0 && Professional.Contains(id))
                assigned = canHaveProfessional.TakeFirst();
            else if (canHaveModifier.Count > 0 && Global.Contains(id))
                assigned = canHaveModifier.TakeFirst();
            else if (canHaveShy.Count > 0 && Shy.Contains(id))
                assigned = canHaveShy.TakeFirst();

            if (assigned != null)
            {
                canHaveBait.Remove(assigned);
                canHaveDiseased.Remove(assigned);
                canHaveProfessional.Remove(assigned);
                canHaveModifier.Remove(assigned);
                canHaveShy.Remove(assigned);

                if (Modifier.GetModifier(assigned) == null)
                    Gen(assigned, (int)id, PlayerLayerEnum.Modifier);
                else
                    AllModifiers.Add(GenerateSpawnItem(id));
            }
        }

        LogInfo("Modifiers Done");
    }

    private static void SetTargets()
    {
        if (CustomGameOptions.AlliedOn > 0)
        {
            foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
            {
                var alliedRole = Role.GetRole(ally.Player);
                var crew = CustomGameOptions.AlliedFaction == AlliedFaction.Crew;
                var intr = CustomGameOptions.AlliedFaction == AlliedFaction.Intruder;
                var syn = CustomGameOptions.AlliedFaction == AlliedFaction.Syndicate;
                var factions = new List<int>() { 1, 3, 0 };
                int faction;

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
                {
                    alliedRole.FactionColor = Colors.Crew;
                    alliedRole.IsCrewAlly = crew;
                }
                else if (intr)
                {
                    alliedRole.FactionColor = Colors.Intruder;
                    alliedRole.IsIntAlly = intr;
                }
                else if (syn)
                {
                    alliedRole.FactionColor = Colors.Syndicate;
                    alliedRole.IsSynAlly = syn;
                }

                ally.Side = alliedRole.Faction = (Faction)faction;
                alliedRole.Alignment = alliedRole.Alignment.GetNewAlignment((Faction)faction);
                ally.Data.SetImpostor((Faction)faction is Faction.Intruder or Faction.Syndicate);
                CallRpc(CustomRPC.Target, TargetRPC.SetAlliedFaction, ally.Player, faction);
            }

            LogInfo("Allied Faction Set Done");
        }

        if (CustomGameOptions.LoversOn > 0)
        {
            var lovers = Objectifier.GetObjectifiers<Lovers>(LayerEnum.Lovers);

            foreach (var lover in lovers)
            {
                if (lover.OtherLover != null)
                    continue;

                var index = lovers.IndexOf(lover);

                if (index == lovers.Count - 1)
                    break;

                var other = lovers[index + 1];
                lover.OtherLover = other.Player;
                other.OtherLover = lover.Player;
                CallRpc(CustomRPC.Target, TargetRPC.SetCouple, lover.Player, other.Player);

                if (TownOfUsReworked.IsTest)
                    LogMessage($"Lovers = {lover.PlayerName} & {other.PlayerName}");
            }

            foreach (var lover in lovers)
            {
                if (lover.OtherLover == null)
                    NullLayer(lover.Player, PlayerLayerEnum.Objectifier);
            }

            LogInfo("Lovers Set");
        }

        if (CustomGameOptions.RivalsOn > 0)
        {
            var rivals = Objectifier.GetObjectifiers<Rivals>(LayerEnum.Rivals);

            foreach (var rival in rivals)
            {
                if (rival.OtherRival != null)
                    continue;

                var index = rivals.IndexOf(rival);

                if (index == rivals.Count - 1)
                    break;

                var other = rivals[index + 1];
                rival.OtherRival = other.Player;
                other.OtherRival = rival.Player;
                CallRpc(CustomRPC.Target, TargetRPC.SetDuo, rival.Player, other.Player);

                if (TownOfUsReworked.IsTest)
                    LogMessage($"Rivals = {rival.PlayerName} & {other.PlayerName}");
            }

            foreach (var rival in rivals)
            {
                if (rival.OtherRival == null)
                    NullLayer(rival.Player, PlayerLayerEnum.Objectifier);
            }

            LogInfo("Rivals Set");
        }

        if (CustomGameOptions.LinkedOn > 0)
        {
            var linked = Objectifier.GetObjectifiers<Linked>(LayerEnum.Linked);

            foreach (var link in linked)
            {
                if (link.OtherLink != null)
                    continue;

                var index = linked.IndexOf(link);

                if (index == linked.Count - 1)
                    break;

                var other = linked[index + 1];
                link.OtherLink = other.Player;
                other.OtherLink = link.Player;
                CallRpc(CustomRPC.Target, TargetRPC.SetLinked, link.Player, other.Player);

                if (TownOfUsReworked.IsTest)
                    LogMessage($"Linked = {link.PlayerName} & {other.PlayerName}");
            }

            foreach (var link in linked)
            {
                if (link.OtherLink == null)
                    NullLayer(link.Player, PlayerLayerEnum.Objectifier);
            }

            LogInfo("Linked Set");
        }

        if (CustomGameOptions.MafiaOn > 0)
        {
            if (Objectifier.GetObjectifiers(LayerEnum.Mafia).Count == 1)
            {
                foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(LayerEnum.Mafia)))
                    NullLayer(player, PlayerLayerEnum.Objectifier);
            }

            LogInfo("Mafia Set");
        }

        if (CustomGameOptions.ExecutionerOn > 0 && !CustomGameOptions.ExecutionerCanPickTargets)
        {
            foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
            {
                exe.TargetPlayer = null;
                var ratelimit = 0;

                while (!exe.TargetPlayer || exe.TargetPlayer == exe.Player || exe.TargetPlayer.IsLinkedTo(exe.Player) || exe.TargetPlayer.Is(Alignment.CrewSov))
                {
                    exe.TargetPlayer = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (exe.TargetPlayer)
                {
                    CallRpc(CustomRPC.Target, TargetRPC.SetExeTarget, exe, exe.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"Exe Target = {exe.TargetPlayer?.name}");
                }
            }

            LogInfo("Exe Targets Set");
        }

        if (CustomGameOptions.GuesserOn > 0 && !CustomGameOptions.GuesserCanPickTargets)
        {
            foreach (var guess in Role.GetRoles<Guesser>(LayerEnum.Guesser))
            {
                guess.TargetPlayer = null;
                var ratelimit = 0;

                while (!guess.TargetPlayer || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(LayerEnum.Indomitable) || guess.TargetPlayer.IsLinkedTo(guess.Player) ||
                    guess.TargetPlayer.Is(Alignment.CrewInvest))
                {
                    guess.TargetPlayer = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (guess.TargetPlayer)
                {
                    CallRpc(CustomRPC.Target, TargetRPC.SetGuessTarget, guess.Player, guess.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"Guess Target = {guess.TargetPlayer?.name}");
                }
            }

            LogInfo("Guess Targets Set");
        }

        if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.GuardianAngelCanPickTargets)
        {
            foreach (var ga in Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel))
            {
                ga.TargetPlayer = null;
                var ratelimit = 0;

                while (!ga.TargetPlayer || ga.TargetPlayer == ga.Player || ga.TargetPlayer.IsLinkedTo(ga.Player) || ga.TargetPlayer.Is(Alignment.NeutralEvil))
                {
                    ga.TargetPlayer = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (ga.TargetPlayer)
                {
                    CallRpc(CustomRPC.Target, TargetRPC.SetGATarget, ga.Player, ga.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"GA Target = {ga.TargetPlayer?.name}");
                }
            }

            LogInfo("GA Target Set");
        }

        if (CustomGameOptions.BountyHunterOn > 0 && !CustomGameOptions.BountyHunterCanPickTargets)
        {
            foreach (var bh in Role.GetRoles<BountyHunter>(LayerEnum.BountyHunter))
            {
                bh.TargetPlayer = null;
                var ratelimit = 0;

                while (!bh.TargetPlayer || bh.TargetPlayer == bh.Player || bh.Player.IsLinkedTo(bh.TargetPlayer))
                {
                    bh.TargetPlayer = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (bh.TargetPlayer)
                {
                    CallRpc(CustomRPC.Target, TargetRPC.SetBHTarget, bh.Player, bh.TargetPlayer);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"BH Target = {bh.TargetPlayer?.name}");
                }
            }

            LogInfo("BH Targets Set");
        }

        if (CustomGameOptions.ActorOn > 0 && !CustomGameOptions.ActorCanPickRole)
        {
            foreach (var act in Role.GetRoles<Actor>(LayerEnum.Actor))
            {
                var ratelimit = 0;

                while (!act.TargetRole || act.PretendRoles == InspectorResults.None)
                {
                    act.TargetRole = Role.GetRole(CustomPlayer.AllPlayers.Random());
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (act.TargetRole)
                {
                    CallRpc(CustomRPC.Target, TargetRPC.SetActPretendList, act.Player, act.TargetRole);

                    if (TownOfUsReworked.IsTest)
                        LogMessage($"Act Target = {act.PretendRoles}");
                }
            }

            LogInfo("Act Variables Set");
        }

        if (CustomGameOptions.JackalOn > 0)
        {
            foreach (var jackal in Role.GetRoles<Jackal>(LayerEnum.Jackal))
            {
                jackal.GoodRecruit = null;
                jackal.EvilRecruit = null;
                jackal.BackupRecruit = null;
                PlayerControl goodRecruit = null;
                PlayerControl evilRecruit = null;
                var ratelimit = 0;

                while (goodRecruit == null || goodRecruit == jackal.Player || goodRecruit.Is(Alignment.NeutralKill) || goodRecruit.Is(Alignment.NeutralHarb) ||
                    goodRecruit.Is(Faction.Intruder) || goodRecruit.Is(Faction.Syndicate) || goodRecruit.Is(Alignment.NeutralNeo) || goodRecruit.Is(SubFaction.Cabal))
                {
                    goodRecruit = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                ratelimit = 0;

                while (evilRecruit == null || evilRecruit == jackal.Player || evilRecruit.Is(Faction.Crew) || evilRecruit.Is(Alignment.NeutralBen) ||
                    evilRecruit.Is(Alignment.NeutralNeo) || evilRecruit.Is(SubFaction.Cabal))
                {
                    evilRecruit = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (!(goodRecruit == null || goodRecruit == jackal.Player || goodRecruit.Is(Alignment.NeutralKill) || goodRecruit.Is(Alignment.NeutralHarb) ||
                    goodRecruit.Is(Faction.Intruder) || goodRecruit.Is(Faction.Syndicate) || goodRecruit.Is(Alignment.NeutralNeo) || goodRecruit.Is(SubFaction.Cabal)))
                {
                    RpcConvert(goodRecruit.PlayerId, jackal.PlayerId, SubFaction.Cabal);
                }

                if (!(evilRecruit == null || evilRecruit == jackal.Player || evilRecruit.Is(Faction.Crew) || evilRecruit.Is(Alignment.NeutralBen) ||
                    evilRecruit.Is(Alignment.NeutralNeo) || evilRecruit.Is(SubFaction.Cabal)))
                {
                    RpcConvert(evilRecruit.PlayerId, jackal.PlayerId, SubFaction.Cabal);
                }

                if (TownOfUsReworked.IsTest)
                    LogMessage($"Recruits = {jackal.GoodRecruit?.name} (Good) & {jackal.EvilRecruit?.name} (Evil)");
            }

            LogInfo("Jackal Recruits Set");
        }

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!Modifier.GetModifier(player))
                NullLayer(player, PlayerLayerEnum.Modifier);

            if (!Ability.GetAbility(player))
                NullLayer(player, PlayerLayerEnum.Ability);

            if (!Objectifier.GetObjectifier(player))
                NullLayer(player, PlayerLayerEnum.Objectifier);

            if (!Role.GetRole(player))
                NullLayer(player, PlayerLayerEnum.Role);

            if (TownOfUsReworked.IsTest)
                LogMessage($"{player.name} -> {Role.GetRole(player)}, {Objectifier.GetObjectifier(player)}, {Modifier.GetModifier(player)}, {Ability.GetAbility(player)}");
        }

        LogInfo("Players Set");
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

        Objectifier.LoveWins = false;
        Objectifier.RivalWins = false;
        Objectifier.TaskmasterWins = false;
        Objectifier.OverlordWins = false;
        Objectifier.CorruptedWins = false;
        Objectifier.MafiaWins = false;

        Role.SyndicateHasChaosDrive = false;
        Role.ChaosDriveMeetingTimerCount = 0;
        Role.DriveHolder = null;

        Role.Cleaned.Clear();

        MeetingPatches.MeetingCount = 0;

        KilledPlayers.Clear();

        Role.Buttons.Clear();

        UpdateNames.PlayerNames.Clear();
        UpdateNames.ColorNames.Clear();

        ConfirmEjects.LastExiled = null;

        Role.AllRoles.Clear();
        Objectifier.AllObjectifiers.Clear();
        Modifier.AllModifiers.Clear();
        Ability.AllAbilities.Clear();
        PlayerLayer.AllLayers.Clear();

        SetPostmortals.AssassinatedPlayers.Clear();

        AllRoles.Clear();

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

        IntruderDeceptionRoles.Clear();
        IntruderConcealingRoles.Clear();
        IntruderKillingRoles.Clear();
        IntruderSupportRoles.Clear();
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

        GameSettings.SettingsPage = 0;
        GameSettings.CurrentPage = 1;

        Assassin.RemainingKills = CustomGameOptions.AssassinKills;

        Summary.Disconnected.Clear();

        Footprint.OddEven.Clear();

        CachedFirstDead = FirstDead;
        FirstDead = null;

        //Role.IsLeft = false;

        PlayerLayer.DeleteAll();

        CustomMeeting.DestroyAll();
        CustomArrow.DestroyAll();
        CustomMenu.DestroyAll();
        CustomButton.DestroyAll();

        Ash.DestroyAll();
        Objects.Range.DestroyAll();

        OtherButtonsPatch.CloseMenus();

        BodyLocations.Clear();
    }

    public static void BeginRoleGen()
    {
        LogInfo("RPC SET ROLE");

        if (IsHnS || !AmongUsClient.Instance.AmHost)
            return;

        ResetEverything();
        CallRpc(CustomRPC.Misc, MiscRPC.Start);
        LogInfo("Cleared Variables");

        if (IsKilling)
            GenKilling();
        else if (IsVanilla)
            GenVanilla();
        else if (IsRoleList)
            GenRoleList();
        else
            GenClassicCustomAA();

        PureCrew = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew)).ToList().Random();
        CallRpc(CustomRPC.Misc, MiscRPC.SyncPureCrew, PureCrew);
        LogInfo("Synced Pure Crew");

        if (!IsVanilla)
        {
            if (!IsRoleList)
            {
                if (CustomGameOptions.EnableObjectifiers)
                    GenObjectifiers();

                if (CustomGameOptions.EnableAbilities)
                    GenAbilities();

                if (CustomGameOptions.EnableModifiers)
                    GenModifiers();
            }

            Convertible = CustomPlayer.AllPlayers.Count(x => x.Is(SubFaction.None) && x != PureCrew);
            SetTargets();
        }

        SpawnInMinigamePatch.SpawnPoints.Clear();

        if (AmongUsClient.Instance.AmHost)
        {
            var random = (byte)URandom.RandomRangeInt(0, 6);

            while (SpawnInMinigamePatch.SpawnPoints.Count < 3)
            {
                random = (byte)URandom.RandomRangeInt(0, 6);

                if (!SpawnInMinigamePatch.SpawnPoints.Contains(random))
                    SpawnInMinigamePatch.SpawnPoints.Add(random);
            }

            CallRpc(CustomRPC.Misc, MiscRPC.SetSpawnAirship, SpawnInMinigamePatch.SpawnPoints.ToArray());
        }
    }

    private static Role SetRole(int id, PlayerControl player) => (LayerEnum)id switch
    {
        LayerEnum.Altruist => new Altruist(player),
        LayerEnum.Chameleon => new Chameleon(player),
        LayerEnum.Coroner => new Coroner(player),
        LayerEnum.Crewmate => new Crewmate(player),
        LayerEnum.Detective => new Detective(player),
        LayerEnum.Dictator => new Dictator(player),
        LayerEnum.Engineer => new Engineer(player),
        LayerEnum.Escort => new Escort(player),
        LayerEnum.Inspector => new Inspector(player),
        LayerEnum.Mayor => new Mayor(player),
        LayerEnum.Medic => new Medic(player),
        LayerEnum.Medium => new Medium(player),
        LayerEnum.Monarch => new Monarch(player),
        LayerEnum.Mystic => new Mystic(player),
        LayerEnum.Operative => new Operative(player),
        LayerEnum.Retributionist => new Retributionist(player),
        LayerEnum.Revealer => new Revealer(player),
        LayerEnum.Seer => new Seer(player),
        LayerEnum.Sheriff => new Sheriff(player),
        LayerEnum.Shifter => new Shifter(player),
        LayerEnum.Tracker => new Tracker(player),
        LayerEnum.Transporter => new Transporter(player),
        LayerEnum.VampireHunter => new VampireHunter(player),
        LayerEnum.Veteran => new Veteran(player),
        LayerEnum.Vigilante => new Vigilante(player),
        LayerEnum.Actor => new Actor(player),
        LayerEnum.Amnesiac => new Amnesiac(player),
        LayerEnum.Arsonist => new Arsonist(player),
        LayerEnum.BountyHunter => new BountyHunter(player),
        LayerEnum.Cannibal => new Cannibal(player),
        LayerEnum.Cryomaniac => new Cryomaniac(player),
        LayerEnum.Dracula => new Dracula(player),
        LayerEnum.Executioner => new Executioner(player),
        LayerEnum.Glitch => new Glitch(player),
        LayerEnum.GuardianAngel => new GuardianAngel(player),
        LayerEnum.Guesser => new Guesser(player),
        LayerEnum.Jackal => new Jackal(player),
        LayerEnum.Jester => new Jester(player),
        LayerEnum.Juggernaut => new Juggernaut(player),
        LayerEnum.Murderer => new Murderer(player),
        LayerEnum.Necromancer => new Necromancer(player),
        LayerEnum.Pestilence => new Pestilence(player),
        LayerEnum.Phantom => new Phantom(player),
        LayerEnum.Plaguebearer => new Plaguebearer(player),
        LayerEnum.SerialKiller => new SerialKiller(player),
        LayerEnum.Survivor => new Survivor(player),
        LayerEnum.Thief => new Thief(player),
        LayerEnum.Troll => new Troll(player),
        LayerEnum.Werewolf => new Werewolf(player),
        LayerEnum.Whisperer => new Whisperer(player),
        LayerEnum.Betrayer => new Betrayer(player),
        LayerEnum.Ambusher => new Ambusher(player),
        LayerEnum.Blackmailer => new Blackmailer(player),
        LayerEnum.Camouflager => new Camouflager(player),
        LayerEnum.Consigliere => new Consigliere(player),
        LayerEnum.Consort => new Consort(player),
        LayerEnum.Disguiser => new Disguiser(player),
        LayerEnum.Enforcer => new Enforcer(player),
        LayerEnum.Ghoul => new Ghoul(player),
        LayerEnum.Godfather => new Godfather(player),
        LayerEnum.Grenadier => new Grenadier(player),
        LayerEnum.Impostor => new Impostor(player),
        LayerEnum.Janitor => new Janitor(player),
        LayerEnum.Mafioso => new Mafioso(player),
        LayerEnum.Miner => new Miner(player),
        LayerEnum.Morphling => new Morphling(player),
        LayerEnum.PromotedGodfather => new PromotedGodfather(player),
        LayerEnum.Teleporter => new Teleporter(player),
        LayerEnum.Wraith => new Wraith(player),
        LayerEnum.Anarchist => new Anarchist(player),
        LayerEnum.Banshee => new Banshee(player),
        LayerEnum.Bomber => new Bomber(player),
        LayerEnum.Concealer => new Concealer(player),
        LayerEnum.Collider => new PlayerLayers.Roles.Collider(player),
        LayerEnum.Crusader => new Crusader(player),
        LayerEnum.Drunkard => new Drunkard(player),
        LayerEnum.Framer => new Framer(player),
        LayerEnum.Poisoner => new Poisoner(player),
        LayerEnum.PromotedRebel => new PromotedRebel(player),
        LayerEnum.Rebel => new Rebel(player),
        LayerEnum.Shapeshifter => new Shapeshifter(player),
        LayerEnum.Sidekick => new Sidekick(player),
        LayerEnum.Silencer => new Silencer(player),
        LayerEnum.Spellslinger => new Spellslinger(player),
        LayerEnum.Stalker => new Stalker(player),
        LayerEnum.TimeKeeper => new TimeKeeper(player),
        LayerEnum.Warper => new Warper(player),
        _ => new Roleless(player)
    };

    private static Ability SetAbility(int id, PlayerControl player) => (LayerEnum)id switch
    {
        LayerEnum.CrewAssassin => new CrewAssassin(player),
        LayerEnum.IntruderAssassin => new IntruderAssassin(player),
        LayerEnum.NeutralAssassin => new NeutralAssassin(player),
        LayerEnum.SyndicateAssassin => new SyndicateAssassin(player),
        LayerEnum.ButtonBarry => new ButtonBarry(player),
        LayerEnum.Insider => new Insider(player),
        LayerEnum.Multitasker => new Multitasker(player),
        LayerEnum.Ninja => new Ninja(player),
        LayerEnum.Politician => new Politician(player),
        LayerEnum.Radar => new Radar(player),
        LayerEnum.Ruthless => new Ruthless(player),
        LayerEnum.Snitch => new Snitch(player),
        LayerEnum.Swapper => new Swapper(player),
        LayerEnum.Tiebreaker => new Tiebreaker(player),
        LayerEnum.Torch => new Torch(player),
        LayerEnum.Tunneler => new Tunneler(player),
        LayerEnum.Underdog => new Underdog(player),
        _ => new Abilityless(player),
    };

    private static Objectifier SetObjectifier(int id, PlayerControl player) => (LayerEnum)id switch
    {
        LayerEnum.Allied => new Allied(player),
        LayerEnum.Corrupted => new Corrupted(player),
        LayerEnum.Defector => new Defector(player),
        LayerEnum.Fanatic => new Fanatic(player),
        LayerEnum.Linked => new Linked(player),
        LayerEnum.Lovers => new Lovers(player),
        LayerEnum.Mafia => new Mafia(player),
        LayerEnum.Overlord => new Overlord(player),
        LayerEnum.Rivals => new Rivals(player),
        LayerEnum.Taskmaster => new Taskmaster(player),
        LayerEnum.Traitor => new Traitor(player),
        _ => new Objectifierless(player),
    };

    private static Modifier SetModifier(int id, PlayerControl player) => (LayerEnum)id switch
    {
        LayerEnum.Astral => new Astral(player),
        LayerEnum.Bait => new Bait(player),
        LayerEnum.Coward => new Coward(player),
        LayerEnum.Diseased => new Diseased(player),
        LayerEnum.Drunk => new Drunk(player),
        LayerEnum.Dwarf => new Dwarf(player),
        LayerEnum.Giant => new Giant(player),
        LayerEnum.Indomitable => new Indomitable(player),
        LayerEnum.Professional => new Professional(player),
        LayerEnum.Shy => new Shy(player),
        LayerEnum.VIP => new VIP(player),
        LayerEnum.Volatile => new Volatile(player),
        LayerEnum.Yeller => new Yeller(player),
        _ => new Modifierless(player),
    };

    private static void Gen(PlayerControl player, int id, PlayerLayerEnum rpc)
    {
        SetLayer(id, player, rpc);
        CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, id, player, rpc);
    }

    private static void NullLayer(PlayerControl player, PlayerLayerEnum rpc) => Gen(player, 10000, rpc);

    public static void SetLayer(int id, PlayerControl player, PlayerLayerEnum rpc)
    {
        if (rpc == PlayerLayerEnum.Role)
            _ = SetRole(id, player);
        else if (rpc == PlayerLayerEnum.Modifier)
            _ = SetModifier(id, player);
        else if (rpc == PlayerLayerEnum.Objectifier)
            _ = SetObjectifier(id, player);
        else if (rpc == PlayerLayerEnum.Ability)
            _ = SetAbility(id, player);
    }

    public static void AssignChaosDrive()
    {
        if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && x.Is(Faction.Syndicate)))
            return;

        var all = CustomPlayer.AllPlayers.Where(x => !x.HasDied() && x.Is(Faction.Syndicate)).ToList();
        PlayerControl chosen = null;

        if (Role.DriveHolder == null || Role.DriveHolder.HasDied())
        {
            chosen = all.Find(x => x.Is(LayerEnum.PromotedRebel));

            if (chosen == null)
                chosen = all.Find(x => x.Is(Alignment.SyndicateDisrup));

            if (chosen == null)
                chosen = all.Find(x => x.Is(Alignment.SyndicateSupport));

            if (chosen == null)
                chosen = all.Find(x => x.Is(Alignment.SyndicatePower));

            if (chosen == null)
                chosen = all.Find(x => x.Is(Alignment.SyndicateKill));

            if (chosen == null)
                chosen = all.Find(x => x.Is(LayerEnum.Anarchist) || x.Is(LayerEnum.Rebel) || x.Is(LayerEnum.Sidekick));
        }

        if (chosen != null)
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
            var role1 = Role.GetRole(converted);
            var role2 = Role.GetRole(converter);
            var converts = converted.Is(SubFaction.None);

            if (!converts && !converted.Is(sub))
                Interact(converter, converted, true, true);
            else
            {
                if (converter.Is(LayerEnum.Dracula))
                {
                    if (converts)
                    {
                        ((Dracula)role2).Converted.Add(target);
                        role1.IsBitten = true;
                    }
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
                    {
                        ((Whisperer)role2).Persuaded.Add(target);
                        role1.IsPersuaded = true;
                    }
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
                    {
                        ((Necromancer)role2).Resurrected.Add(target);
                        role1.IsResurrected = true;
                    }
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
                        role1.IsRecruit = true;

                        if (jackal.GoodRecruit == null)
                            jackal.GoodRecruit = converted;
                        else if (jackal.EvilRecruit == null)
                            jackal.EvilRecruit = converted;
                        else if (jackal.BackupRecruit == null)
                            jackal.BackupRecruit = converted;
                    }
                    else if (converted.IsRecruit())
                        ((Jackal)role2).Recruited.Add(target);
                    else if (converted.Is(LayerEnum.Jackal))
                    {
                        ((Jackal)role2).Recruited.AddRange(((Jackal)role1).Recruited);
                        ((Jackal)role1).Recruited.AddRange(((Jackal)role2).Recruited);
                    }
                }

                var (flash, symbol) = sub switch
                {
                    SubFaction.Undead => (Colors.Undead, "γ"),
                    SubFaction.Cabal => (Colors.Cabal, "$"),
                    SubFaction.Reanimated => (Colors.Reanimated, "Σ"),
                    SubFaction.Sect => (Colors.Sect, "Λ"),
                    _ => (Colors.SubFaction, "φ")
                };

                role1.SubFaction = sub;
                role1.SubFactionColor = flash;
                role1.Alignment = role1.Alignment.GetNewAlignment(Faction.Neutral);
                role1.SubFactionSymbol = symbol;
                Convertible--;

                if (CustomPlayer.Local == converted)
                    Flash(flash);
                else if (CustomPlayer.Local.Is(LayerEnum.Mystic))
                    Flash(Colors.Mystic);
            }
        }
    }

    public static void RpcConvert(byte target, byte convert, SubFaction sub, bool condition = false)
    {
        Convert(target, convert, sub, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, sub, condition);
    }
}