namespace TownOfUsReworked.Classes;

[HarmonyPatch]
public static class RoleGen
{
    private static readonly List<(int Chance, LayerEnum Id, bool Unique)> CrewAuditorRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewKillingRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewSupportRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewSovereignRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewProtectiveRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewInvestigativeRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> CrewRoles = new();

    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralEvilRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralBenignRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralKillingRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralNeophyteRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralHarbingerRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> NeutralRoles = new();

    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> IntruderKillingRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> IntruderSupportRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> IntruderDeceptionRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> IntruderConcealingRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> IntruderRoles = new();

    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> SyndicatePowerRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> SyndicateSupportRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> SyndicateKillingRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> SyndicateDisruptionRoles = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> SyndicateRoles = new();

    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> AllModifiers = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> AllAbilities = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> AllObjectifiers = new();
    private static readonly List<(int Chance, LayerEnum ID, bool Unique)> AllRoles = new();

    public static PlayerControl PureCrew;
    public static int Convertible;

    private const LayerEnum Any = LayerEnum.Any;
    private static readonly List<LayerEnum> CA = new() { LayerEnum.Mystic, LayerEnum.VampireHunter };
    private static readonly List<LayerEnum> CI = new() { LayerEnum.Sheriff, LayerEnum.Inspector, LayerEnum.Tracker, LayerEnum.Medium, LayerEnum.Coroner, LayerEnum.Operative, LayerEnum.Seer,
        LayerEnum.Detective };
    private static readonly List<LayerEnum> CSv = new() { LayerEnum.Mayor, LayerEnum.Dictator, LayerEnum.Monarch };
    private static readonly List<LayerEnum> CP = new() { LayerEnum.Altruist, LayerEnum.Medic };
    private static readonly List<LayerEnum> CK = new() { LayerEnum.Vigilante, LayerEnum.Veteran};
    private static readonly List<LayerEnum> CS = new() { LayerEnum.Engineer, LayerEnum.Transporter, LayerEnum.Escort, LayerEnum.Shifter, LayerEnum.Chameleon, LayerEnum.Retributionist };
    private static readonly List<List<LayerEnum>> Crew = new() { CA, CI, CSv, CP, CK, CS };
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
    private static readonly List<List<LayerEnum>> Intruders = new() { IC, ID, IK, IS };
    private static readonly List<LayerEnum> SSu = new() { LayerEnum.Rebel, LayerEnum.Warper, LayerEnum.Stalker };
    private static readonly List<LayerEnum> SD = new() { LayerEnum.Concealer, LayerEnum.Drunkard, LayerEnum.Framer, LayerEnum.Shapeshifter, LayerEnum.Silencer };
    private static readonly List<LayerEnum> SP = new() { LayerEnum.TimeKeeper, LayerEnum.Spellslinger };
    private static readonly List<LayerEnum> SyK = new() { LayerEnum.Bomber, LayerEnum.Collider, LayerEnum.Crusader, LayerEnum.Poisoner };
    private static readonly List<List<LayerEnum>> Syndicate = new() { SSu, SyK, SD, SP };
    private static readonly List<LayerEnum> AlignmentEntries = new() { LayerEnum.CrewSupport, LayerEnum.CrewInvest, LayerEnum.CrewSov, LayerEnum.CrewProt, LayerEnum.CrewKill,
        LayerEnum.CrewAudit, LayerEnum.IntruderSupport, LayerEnum.IntruderConceal, LayerEnum.IntruderDecep, LayerEnum.IntruderKill, LayerEnum.NeutralApoc, LayerEnum.NeutralHarb,
        LayerEnum.NeutralBen, LayerEnum.NeutralEvil, LayerEnum.NeutralKill, LayerEnum.NeutralNeo, LayerEnum.SyndicateDisrup, LayerEnum.SyndicateKill, LayerEnum.SyndicatePower,
        LayerEnum.SyndicatePower };
    private static readonly List<LayerEnum> RandomEntries = new() { LayerEnum.RandomCrew, LayerEnum.RandomIntruder, LayerEnum.RandomSyndicate, LayerEnum.RandomNeutral };
    private static readonly List<List<LayerEnum>> Alignments = new() { CA, CI, CSv, CP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA };

    private static void Sort(this List<(int Chance, LayerEnum ID, bool Unique)> items, int amount)
    {
        var newList = new List<(int Chance, LayerEnum ID, bool Unique)>();

        if (amount < CustomPlayer.AllPlayers.Count)
            amount = CustomPlayer.AllPlayers.Count;

        if (amount < items.Count)
            amount = items.Count;

        items.Shuffle();

        if (IsAA)
        {
            while (newList.Count < amount && items.Count > 0)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Unique && CustomGameOptions.EnableUniques)
                    items.Remove(items[0]);
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (newList.Count >= amount)
                    break;

                if (item.Chance == 100)
                    newList.Add(item);
            }

            foreach (var item in items)
            {
                if (newList.Count >= amount)
                    break;

                if (item.Chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < item.Chance)
                        newList.Add(item);
                }
            }
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

        var random = URandom.RandomRangeInt(0, 100);

        if (players <= 6)
        {
            if (random <= 5)
                impostors = 0;
            else
                impostors = 1;
        }
        else if (players == 7)
        {
            if (random < 5)
                impostors = 0;
            else if (random < 20)
                impostors = 2;
            else
                impostors = 1;
        }
        else if (players == 8)
        {
            if (random < 5)
                impostors = 0;
            else if (random < 40)
                impostors = 2;
            else
                impostors = 1;
        }
        else if (players == 9)
        {
            if (random < 5)
                impostors = 0;
            else if (random < 50)
                impostors = 2;
            else
                impostors = 1;
        }
        else if (players == 10)
        {
            if (random < 5)
                impostors = 0;
            else if (random < 10)
                impostors = 3;
            else if (random < 60)
                impostors = 2;
            else
                impostors = 1;
        }
        else if (players == 11)
        {
            if (random < 10)
                impostors = 0;
            else if (random < 60)
                impostors = 2;
            else if (random < 70)
                impostors = 3;
            else
                impostors = 1;
        }
        else if (players == 12)
        {
            if (random < 10)
                impostors = 0;
            else if (random < 60)
                impostors = 2;
            else if (random < 80)
                impostors = 3;
            else
                impostors = 1;
        }
        else if (players == 13)
        {
            if (random < 10)
                impostors = 0;
            else if (random < 60)
                impostors = 2;
            else if (random < 90)
                impostors = 3;
            else
                impostors = 1;
        }
        else if (players == 14)
        {
            if (random < 5)
                impostors = 0;
            else if (random < 25)
                impostors = 1;
            else if (random < 50)
                impostors = 3;
            else
                impostors = 2;
        }
        else if (random < 5)
            impostors = 0;
        else if (random < 20)
            impostors = 1;
        else if (random < 60)
            impostors = 3;
        else if (random < 90)
            impostors = 2;
        else
            impostors = 4;

        random = URandom.RandomRangeInt(0, 100);

        if (players <= 6)
        {
            if (random <= 5)
                syndicate = 0;
            else
                syndicate = 1;
        }
        else if (players == 7)
        {
            if (random < 5)
                syndicate = 0;
            else if (random < 20)
                syndicate = 2;
            else
                syndicate = 1;
        }
        else if (players == 8)
        {
            if (random < 5)
                syndicate = 0;
            else if (random < 40)
                syndicate = 2;
            else
                syndicate = 1;
        }
        else if (players == 9)
        {
            if (random < 5)
                syndicate = 0;
            else if (random < 50)
                syndicate = 2;
            else
                syndicate = 1;
        }
        else if (players == 10)
        {
            if (random < 5)
                syndicate = 0;
            else if (random < 10)
                syndicate = 3;
            else if (random < 60)
                syndicate = 2;
            else
                syndicate = 1;
        }
        else if (players == 11)
        {
            if (random < 10)
                syndicate = 0;
            else if (random < 60)
                syndicate = 2;
            else if (random < 70)
                syndicate = 3;
            else
                syndicate = 1;
        }
        else if (players == 12)
        {
            if (random < 10)
                syndicate = 0;
            else if (random < 60)
                syndicate = 2;
            else if (random < 80)
                syndicate = 3;
            else
                syndicate = 1;
        }
        else if (players == 13)
        {
            if (random < 10)
                syndicate = 0;
            else if (random < 60)
                syndicate = 2;
            else if (random < 90)
                syndicate = 3;
            else
                syndicate = 1;
        }
        else if (players == 14)
        {
            if (random < 5)
                syndicate = 0;
            else if (random < 25)
                syndicate = 1;
            else if (random < 50)
                syndicate = 3;
            else
                syndicate = 2;
        }
        else if (random < 5)
            syndicate = 0;
        else if (random < 20)
            syndicate = 1;
        else if (random < 60)
            syndicate = 3;
        else if (random < 90)
            syndicate = 2;
        else
            syndicate = 4;

        random = URandom.RandomRangeInt(0, 100);

        if (players <= 6)
        {
            if (random <= 5)
                neutrals = 0;
            else
                neutrals = 1;
        }
        else if (players == 7)
        {
            if (random < 5)
                neutrals = 0;
            else if (random < 20)
                neutrals = 2;
            else
                neutrals = 1;
        }
        else if (players == 8)
        {
            if (random < 5)
                neutrals = 0;
            else if (random < 40)
                neutrals = 2;
            else
                neutrals = 1;
        }
        else if (players == 9)
        {
            if (random < 5)
                neutrals = 0;
            else if (random < 50)
                neutrals = 2;
            else
                neutrals = 1;
        }
        else if (players == 10)
        {
            if (random < 5)
                neutrals = 0;
            else if (random < 10)
                neutrals = 3;
            else if (random < 60)
                neutrals = 2;
            else
                neutrals = 1;
        }
        else if (players == 11)
        {
            if (random < 10)
                neutrals = 0;
            else if (random < 60)
                neutrals = 2;
            else if (random < 70)
                neutrals = 3;
            else
                neutrals = 1;
        }
        else if (players == 12)
        {
            if (random < 10)
                neutrals = 0;
            else if (random < 60)
                neutrals = 2;
            else if (random < 80)
                neutrals = 3;
            else
                neutrals = 1;
        }
        else if (players == 13)
        {
            if (random < 10)
                neutrals = 0;
            else if (random < 60)
                neutrals = 2;
            else if (random < 90)
                neutrals = 3;
            else
                neutrals = 1;
        }
        else if (players == 14)
        {
            if (random < 5)
                neutrals = 0;
            else if (random < 25)
                neutrals = 1;
            else if (random < 50)
                neutrals = 3;
            else
                neutrals = 2;
        }
        else if (random < 5)
            neutrals = 0;
        else if (random < 20)
            neutrals = 1;
        else if (random < 60)
            neutrals = 3;
        else if (random < 90)
            neutrals = 2;
        else
            neutrals = 4;

        if (impostors == 0 && syndicate == 0 && neutrals == 0)
        {
            random = URandom.RandomRangeInt(0, 3);

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

    private static void GenVanilla()
    {
        LogSomething("Role Gen Start");
        var players = CustomPlayer.AllPlayers;

        AllRoles.Clear();

        while (AllRoles.Count < (CustomGameOptions.AltImps ? CustomGameOptions.SyndicateCount : CustomGameOptions.IntruderCount))
            AllRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < players.Count)
            AllRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crewmate));

        var spawnList = AllRoles;
        spawnList.Shuffle();

        LogSomething("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Roles in the game: " + ids);
        }

        while (players.Count > 0 && spawnList.Count > 0)
            Gen(players.TakeFirst(), (int)spawnList.TakeFirst().ID, PlayerLayerEnum.Role);

        LogSomething("Role Spawn Done");
    }

    private static void GenKilling()
    {
        LogSomething("Role Gen Start");
        var players = CustomPlayer.AllPlayers;
        GetAdjustedFactions(out var imps, out var syn, out var neutrals, out var crew);

        CrewRoles.Clear();
        IntruderRoles.Clear();
        SyndicateRoles.Clear();
        NeutralRoles.Clear();

        LogSomething("Lists Cleared - Killing Only");

        IntruderRoles.Clear();
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Enforcer));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Morphling));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Blackmailer));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Miner));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Teleporter));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Wraith));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Consort));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Janitor));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Camouflager));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Grenadier));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Impostor));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Consigliere));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Disguiser));
        IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Ambusher));

        if (imps >= 3)
            IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Godfather));

        SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Anarchist));
        SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Bomber));
        SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Poisoner));
        SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crusader));
        SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Collider));

        if (syn >= 3)
            SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Rebel));

        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Glitch));
        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Werewolf));
        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.SerialKiller));
        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Juggernaut));
        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Murderer));
        NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Thief));

        if (CustomGameOptions.AddArsonist)
            NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Arsonist));

        if (CustomGameOptions.AddCryomaniac)
            NeutralRoles.Add(GenerateRoleSpawnItem(LayerEnum.Cryomaniac));

        if (CustomGameOptions.AddPlaguebearer)
            NeutralRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.PestSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));

        NeutralRoles.Sort(neutrals);

        var vigis = crew / 2;
        var vets = crew / 2;

        while (vigis > 0 || vets > 0)
        {
            if (vigis > 0)
            {
                CrewRoles.Add(GenerateRoleSpawnItem(LayerEnum.Vigilante));
                vigis--;
            }

            if (vets > 0)
            {
                CrewRoles.Add(GenerateRoleSpawnItem(LayerEnum.Veteran));
                vets--;
            }
        }

        LogSomething("Lists Set - Killing Only");

        IntruderRoles.Sort(imps);
        CrewRoles.Sort(crew);

        LogSomething("Killing Role List Sorted");

        AllRoles.AddRange(NeutralRoles);
        AllRoles.AddRange(CrewRoles);
        AllRoles.AddRange(SyndicateRoles);

        if (!CustomGameOptions.AltImps)
            AllRoles.AddRange(IntruderRoles);

        LogSomething("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in AllRoles)
                ids += $" {id}";

            LogSomething("Roles in the game: " + ids);
        }

        var spawnList = AllRoles;
        spawnList.Shuffle();

        while (players.Count > 0 && spawnList.Count > 0)
            Gen(players.TakeFirst(), (int)spawnList.TakeFirst().ID, PlayerLayerEnum.Role);

        Role.SyndicateHasChaosDrive = true;
        AssignChaosDrive();
        LogSomething("Role Spawn Done");
    }

    private static void GenClassicCustomAA()
    {
        LogSomething("Role Gen Start");
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
                CrewSovereignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Mayor));
                num--;
            }

            LogSomething("Mayor Done");
        }

        if (CustomGameOptions.MonarchOn > 0)
        {
            num = CustomGameOptions.MonarchCount;

            while (num > 0)
            {
                CrewSovereignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Monarch));
                num--;
            }

            LogSomething("Monarch Done");
        }

        if (CustomGameOptions.DictatorOn > 0)
        {
            num = CustomGameOptions.DictatorCount;

            while (num > 0)
            {
                CrewSovereignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Dictator));
                num--;
            }

            LogSomething("Dictator Done");
        }

        if (CustomGameOptions.SheriffOn > 0)
        {
            num = CustomGameOptions.SheriffCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Sheriff));
                num--;
            }

            LogSomething("Sheriff Done");
        }

        if (CustomGameOptions.InspectorOn > 0)
        {
            num = CustomGameOptions.InspectorCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Inspector));
                num--;
            }

            LogSomething("Inspector Done");
        }

        if (CustomGameOptions.VigilanteOn > 0)
        {
            num = CustomGameOptions.VigilanteCount;

            while (num > 0)
            {
                CrewKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Vigilante));
                num--;
            }

            LogSomething("Vigilante Done");
        }

        if (CustomGameOptions.EngineerOn > 0)
        {
            num = CustomGameOptions.EngineerCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Engineer));
                num--;
            }

            LogSomething("Engineer Done");
        }

        if (CustomGameOptions.MedicOn > 0)
        {
            num = CustomGameOptions.MedicCount;

            while (num > 0)
            {
                CrewProtectiveRoles.Add(GenerateRoleSpawnItem(LayerEnum.Medic));
                num--;
            }

            LogSomething("Medic Done");
        }

        if (CustomGameOptions.AltruistOn > 0)
        {
            num = CustomGameOptions.AltruistCount;

            while (num > 0)
            {
                CrewProtectiveRoles.Add(GenerateRoleSpawnItem(LayerEnum.Altruist));
                num--;
            }

            LogSomething("Altruist Done");
        }

        if (CustomGameOptions.VeteranOn > 0)
        {
            num = CustomGameOptions.VeteranCount;

            while (num > 0)
            {
                CrewKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Veteran));
                num--;
            }

            LogSomething("Veteran Done");
        }

        if (CustomGameOptions.TrackerOn > 0)
        {
            num = CustomGameOptions.TrackerCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Tracker));
                num--;
            }

            LogSomething("Tracker Done");
        }

        if (CustomGameOptions.TransporterOn > 0)
        {
            num = CustomGameOptions.TransporterCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Transporter));
                num--;
            }

            LogSomething("Transporter Done");
        }

        if (CustomGameOptions.MediumOn > 0)
        {
            num = CustomGameOptions.MediumCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Medium));
                num--;
            }

            LogSomething("Medium Done");
        }

        if (CustomGameOptions.CoronerOn > 0)
        {
            num = CustomGameOptions.CoronerCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Coroner));
                num--;
            }

            LogSomething("Coroner Done");
        }

        if (CustomGameOptions.OperativeOn > 0)
        {
            num = CustomGameOptions.OperativeCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Operative));
                num--;
            }

            LogSomething("Operative Done");
        }

        if (CustomGameOptions.DetectiveOn > 0)
        {
            num = CustomGameOptions.DetectiveCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Detective));
                num--;
            }

            LogSomething("Detective Done");
        }

        if (CustomGameOptions.EscortOn > 0)
        {
            num = CustomGameOptions.EscortCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Escort));
                num--;
            }

            LogSomething("Escort Done");
        }

        if (CustomGameOptions.ShifterOn > 0)
        {
            num = CustomGameOptions.ShifterCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Shifter));
                num--;
            }

            LogSomething("Shifter Done");
        }

        if (CustomGameOptions.ChameleonOn > 0)
        {
            num = CustomGameOptions.ChameleonCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Chameleon));
                num--;
            }

            LogSomething("Chameleon Done");
        }

        if (CustomGameOptions.RetributionistOn > 0)
        {
            num = CustomGameOptions.RetributionistCount;

            while (num > 0)
            {
                CrewSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Retributionist));
                num--;
            }

            LogSomething("Retributionist Done");
        }

        if (CustomGameOptions.CrewmateOn > 0 && IsCustom)
        {
            num = CustomGameOptions.CrewCount;

            while (num > 0)
            {
                CrewRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crewmate));
                num--;
            }

            LogSomething("Crewmate Done");
        }

        if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
        {
            num = CustomGameOptions.VampireHunterCount;

            while (num > 0)
            {
                CrewAuditorRoles.Add(GenerateRoleSpawnItem(LayerEnum.VampireHunter));
                num--;
            }

            LogSomething("Vampire Hunter Done");
        }

        if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 || CustomGameOptions.JackalOn >
            0))
        {
            num = CustomGameOptions.MysticCount;

            while (num > 0)
            {
                CrewAuditorRoles.Add(GenerateRoleSpawnItem(LayerEnum.Mystic));
                num--;
            }

            LogSomething("Mystic Done");
        }

        if (CustomGameOptions.SeerOn > 0 && ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.BountyHunterOn > 0 ||
            CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||
            CustomGameOptions.TraitorOn > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 ||
            CustomGameOptions.GuardianAngelOn > 0 || CustomGameOptions.GuesserOn > 0 || CustomGameOptions.ShifterOn > 0))
        {
            num = CustomGameOptions.SeerCount;

            while (num > 0)
            {
                CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(LayerEnum.Seer));
                num--;
            }

            LogSomething("Seer Done");
        }

        if (CustomGameOptions.JesterOn > 0)
        {
            num = CustomGameOptions.JesterCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Jester));
                num--;
            }

            LogSomething("Jester Done");
        }

        if (CustomGameOptions.AmnesiacOn > 0)
        {
            num = CustomGameOptions.AmnesiacCount;

            while (num > 0)
            {
                NeutralBenignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Amnesiac));
                num--;
            }

            LogSomething("Amnesiac Done");
        }

        if (CustomGameOptions.ExecutionerOn > 0)
        {
            num = CustomGameOptions.ExecutionerCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Executioner));
                num--;
            }

            LogSomething("Executioner Done");
        }

        if (CustomGameOptions.SurvivorOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
        {
            num = CustomGameOptions.SurvivorCount;

            while (num > 0)
            {
                NeutralBenignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Survivor));
                num--;
            }

            LogSomething("Survivor Done");
        }

        if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
        {
            num = CustomGameOptions.GuardianAngelCount;

            while (num > 0)
            {
                NeutralBenignRoles.Add(GenerateRoleSpawnItem(LayerEnum.GuardianAngel));
                num--;
            }

            LogSomething("Guardian Angel Done");
        }

        if (CustomGameOptions.GlitchOn > 0)
        {
            num = CustomGameOptions.GlitchCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Glitch));
                num--;
            }

            LogSomething("Glitch Done");
        }

        if (CustomGameOptions.MurdererOn > 0)
        {
            num = CustomGameOptions.MurdCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Murderer));
                num--;
            }

            LogSomething("Murderer Done");
        }

        if (CustomGameOptions.CryomaniacOn > 0)
        {
            num = CustomGameOptions.CryomaniacCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Cryomaniac));
                num--;
            }

            LogSomething("Cryomaniac Done");
        }

        if (CustomGameOptions.WerewolfOn > 0)
        {
            num = CustomGameOptions.WerewolfCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Werewolf));
                num--;
            }

            LogSomething("Werewolf Done");
        }

        if (CustomGameOptions.ArsonistOn > 0)
        {
            num = CustomGameOptions.ArsonistCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Arsonist));
                num--;
            }

            LogSomething("Arsonist Done");
        }

        if (CustomGameOptions.JackalOn > 0 && GameData.Instance.PlayerCount > 5)
        {
            num = CustomGameOptions.JackalCount;

            while (num > 0)
            {
                NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(LayerEnum.Jackal));
                num--;
            }

            LogSomething("Jackal Done");
        }

        if (CustomGameOptions.NecromancerOn > 0)
        {
            num = CustomGameOptions.NecromancerCount;

            while (num > 0)
            {
                NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(LayerEnum.Necromancer));
                num--;
            }

            LogSomething("Necromancer Done");
        }

        if (CustomGameOptions.PlaguebearerOn > 0)
        {
            num = CustomGameOptions.PlaguebearerCount;

            while (num > 0)
            {
                NeutralHarbingerRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.PestSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));
                num--;
            }

            var PBorPest = CustomGameOptions.PestSpawn ? "Pestilence" : "Plaguebearer";
            LogSomething($"{PBorPest} Done");
        }

        if (CustomGameOptions.SerialKillerOn > 0)
        {
            num = CustomGameOptions.SKCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.SerialKiller));
                num--;
            }

            LogSomething("Serial Killer Done");
        }

        if (CustomGameOptions.JuggernautOn > 0)
        {
            num = CustomGameOptions.JuggernautCount;

            while (num > 0)
            {
                NeutralKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Juggernaut));
                num--;
            }

            LogSomething("Juggeraut Done");
        }

        if (CustomGameOptions.CannibalOn > 0)
        {
            num = CustomGameOptions.CannibalCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Cannibal));
                num--;
            }

            LogSomething("Cannibal Done");
        }

        if (CustomGameOptions.GuesserOn > 0)
        {
            num = CustomGameOptions.GuesserCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Guesser));
                num--;
            }

            LogSomething("Guesser Done");
        }

        if (CustomGameOptions.ActorOn > 0 && (CustomGameOptions.CrewAssassinOn > 0 || CustomGameOptions.NeutralAssassinOn > 0 || CustomGameOptions.SyndicateAssassinOn > 0 ||
            CustomGameOptions.IntruderAssassinOn > 0))
        {
            num = CustomGameOptions.ActorCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Actor));
                num--;
            }

            LogSomething("Actor Done");
        }

        if (CustomGameOptions.ThiefOn > 0)
        {
            num = CustomGameOptions.ThiefCount;

            while (num > 0)
            {
                NeutralBenignRoles.Add(GenerateRoleSpawnItem(LayerEnum.Thief));
                num--;
            }

            LogSomething("Thief Done");
        }

        if (CustomGameOptions.DraculaOn > 0)
        {
            num = CustomGameOptions.DraculaCount;

            while (num > 0)
            {
                NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(LayerEnum.Dracula));
                num--;
            }

            LogSomething("Dracula Done");
        }

        if (CustomGameOptions.WhispererOn > 0)
        {
            num = CustomGameOptions.WhispererCount;

            while (num > 0)
            {
                NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(LayerEnum.Whisperer));
                num--;
            }

            LogSomething("Whisperer Done");
        }

        if (CustomGameOptions.TrollOn > 0)
        {
            num = CustomGameOptions.TrollCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.Troll));
                num--;
            }

            LogSomething("Troll Done");
        }

        if (CustomGameOptions.BountyHunterOn > 0)
        {
            num = CustomGameOptions.BHCount;

            while (num > 0)
            {
                NeutralEvilRoles.Add(GenerateRoleSpawnItem(LayerEnum.BountyHunter));
                num--;
            }

            LogSomething("Bounty Hunter Done");
        }

        if (CustomGameOptions.MorphlingOn > 0)
        {
            num = CustomGameOptions.MorphlingCount;

            while (num > 0)
            {
                IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Morphling));
                num--;
            }

            LogSomething("Morphling Done");
        }

        if (CustomGameOptions.BlackmailerOn > 0)
        {
            num = CustomGameOptions.BlackmailerCount;

            while (num > 0)
            {
                IntruderConcealingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Blackmailer));
                num--;
            }

            LogSomething("Blackmailer Done");
        }

        if (CustomGameOptions.MinerOn > 0)
        {
            num = CustomGameOptions.MinerCount;

            while (num > 0)
            {
                IntruderSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Miner));
                num--;
            }

            LogSomething("Miner Done");
        }

        if (CustomGameOptions.TeleporterOn > 0)
        {
            num = CustomGameOptions.TeleporterCount;

            while (num > 0)
            {
                IntruderSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Teleporter));
                num--;
            }

            LogSomething("Teleporter Done");
        }

        if (CustomGameOptions.AmbusherOn > 0)
        {
            num = CustomGameOptions.AmbusherCount;

            while (num > 0)
            {
                IntruderKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Ambusher));
                num--;
            }

            LogSomething("Ambusher Done");
        }

        if (CustomGameOptions.WraithOn > 0)
        {
            num = CustomGameOptions.WraithCount;

            while (num > 0)
            {
                IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Wraith));
                num--;
            }

            LogSomething("Wraith Done");
        }

        if (CustomGameOptions.ConsortOn > 0)
        {
            num = CustomGameOptions.ConsortCount;

            while (num > 0)
            {
                IntruderSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Consort));
                num--;
            }

            LogSomething("Consort Done");
        }

        if (CustomGameOptions.JanitorOn > 0)
        {
            num = CustomGameOptions.JanitorCount;

            while (num > 0)
            {
                IntruderConcealingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Janitor));
                num--;
            }

            LogSomething("Janitor Done");
        }

        if (CustomGameOptions.CamouflagerOn > 0)
        {
            num = CustomGameOptions.CamouflagerCount;

            while (num > 0)
            {
                IntruderConcealingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Camouflager));
                num--;
            }

            LogSomething("Camouflager Done");
        }

        if (CustomGameOptions.GrenadierOn > 0)
        {
            num = CustomGameOptions.GrenadierCount;

            while (num > 0)
            {
                IntruderConcealingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Grenadier));
                num--;
            }

            LogSomething("Grenadier Done");
        }

        if (CustomGameOptions.ImpostorOn > 0 && IsCustom)
        {
            num = CustomGameOptions.ImpCount;

            while (num > 0)
            {
                IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Impostor));
                num--;
            }

            LogSomething("Impostor Done");
        }

        if (CustomGameOptions.ConsigliereOn > 0)
        {
            num = CustomGameOptions.ConsigliereCount;

            while (num > 0)
            {
                IntruderSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Consigliere));
                num--;
            }

            LogSomething("Consigliere Done");
        }

        if (CustomGameOptions.DisguiserOn > 0)
        {
            num = CustomGameOptions.DisguiserCount;

            while (num > 0)
            {
                IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Disguiser));
                num--;
            }

            LogSomething("Disguiser Done");
        }

        if (CustomGameOptions.EnforcerOn > 0)
        {
            num = CustomGameOptions.EnforcerCount;

            while (num > 0)
            {
                IntruderKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Enforcer));
                num--;
            }

            LogSomething("Enforcer Done");
        }

        if (CustomGameOptions.GodfatherOn > 0 && imps >= 3)
        {
            num = CustomGameOptions.GodfatherCount;

            while (num > 0)
            {
                IntruderSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Godfather));
                num--;
            }

            LogSomething("Godfather Done");
        }

        if (CustomGameOptions.AnarchistOn > 0 && IsCustom)
        {
            num = CustomGameOptions.AnarchistCount;

            while (num > 0)
            {
                SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Anarchist));
                num--;
            }

            LogSomething("Anarchist Done");
        }

        if (CustomGameOptions.ShapeshifterOn > 0)
        {
            num = CustomGameOptions.ShapeshifterCount;

            while (num > 0)
            {
                SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Shapeshifter));
                num--;
            }

            LogSomething("Shapeshifter Done");
        }

        if (CustomGameOptions.FramerOn > 0)
        {
            num = CustomGameOptions.FramerCount;

            while (num > 0)
            {
                SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Framer));
                num--;
            }

            LogSomething("Framer Done");
        }

        if (CustomGameOptions.CrusaderOn > 0)
        {
            num = CustomGameOptions.CrusaderCount;

            while (num > 0)
            {
                SyndicateKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crusader));
                num--;
            }

            LogSomething("Crusader Done");
        }

        if (CustomGameOptions.RebelOn > 0 && syn >= 3)
        {
            num = CustomGameOptions.RebelCount;

            while (num > 0)
            {
                SyndicateSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Rebel));
                num--;
            }

            LogSomething("Rebel Done");
        }

        if (CustomGameOptions.PoisonerOn > 0)
        {
            num = CustomGameOptions.PoisonerCount;

            while (num > 0)
            {
                SyndicateKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Poisoner));
                num--;
            }

            LogSomething("Poisoner Done");
        }

        if (CustomGameOptions.ColliderOn > 0)
        {
            num = CustomGameOptions.ColliderCount;

            while (num > 0)
            {
                SyndicateKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Collider));
                num--;
            }

            LogSomething("Collider Done");
        }

        if (CustomGameOptions.ConcealerOn > 0)
        {
            num = CustomGameOptions.ConcealerCount;

            while (num > 0)
            {
                SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Concealer));
                num--;
            }

            LogSomething("Concealer Done");
        }

        if (CustomGameOptions.WarperOn > 0 && (int)CustomGameOptions.Map is not 3 and not 4)
        {
            num = CustomGameOptions.WarperCount;

            while (num > 0)
            {
                SyndicateSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Warper));
                num--;
            }

            LogSomething("Warper Done");
        }

        if (CustomGameOptions.BomberOn > 0)
        {
            num = CustomGameOptions.BomberCount;

            while (num > 0)
            {
                SyndicateKillingRoles.Add(GenerateRoleSpawnItem(LayerEnum.Bomber));
                num--;
            }

            LogSomething("Bomber Done");
        }

        if (CustomGameOptions.SpellslingerOn > 0)
        {
            num = CustomGameOptions.SpellslingerCount;

            while (num > 0)
            {
                SyndicatePowerRoles.Add(GenerateRoleSpawnItem(LayerEnum.Spellslinger));
                num--;
            }

            LogSomething("Spellslinger Done");
        }

        if (CustomGameOptions.StalkerOn > 0)
        {
            num = CustomGameOptions.StalkerCount;

            while (num > 0)
            {
                SyndicateSupportRoles.Add(GenerateRoleSpawnItem(LayerEnum.Stalker));
                num--;
            }

            LogSomething("Stalker Done");
        }

        if (CustomGameOptions.DrunkardOn > 0)
        {
            num = CustomGameOptions.DrunkardCount;

            while (num > 0)
            {
                SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Drunkard));
                num--;
            }

            LogSomething("Drunkard Done");
        }

        if (CustomGameOptions.TimeKeeperOn > 0)
        {
            num = CustomGameOptions.TimeKeeperCount;

            while (num > 0)
            {
                SyndicatePowerRoles.Add(GenerateRoleSpawnItem(LayerEnum.TimeKeeper));
                num--;
            }

            LogSomething("Time Keeper Done");
        }

        if (CustomGameOptions.SilencerOn > 0)
        {
            num = CustomGameOptions.SilencerCount;

            while (num > 0)
            {
                SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(LayerEnum.Silencer));
                num--;
            }

            LogSomething("Silencer Done");
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
                    IntruderRoles.Add(GenerateRoleSpawnItem(LayerEnum.Impostor));

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
                    SyndicateRoles.Add(GenerateRoleSpawnItem(LayerEnum.Anarchist));

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
                    CrewRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crewmate));

                CrewRoles.Shuffle();
            }

            LogSomething("Classic/Custom Sorting Done");
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

            LogSomething("All Any Sorting Done");
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
        var spawnList = AllRoles;

        while (spawnList.Count < players.Count)
            spawnList.Add(GenerateRoleSpawnItem(LayerEnum.Crewmate));

        if (!spawnList.Any(x => CrewRoles.Contains(x) || x.ID == LayerEnum.Crewmate))
        {
            spawnList.Remove(spawnList.Random());
            spawnList.Add(CrewRoles.Count > 0 ? CrewRoles.Random() : GenerateRoleSpawnItem(LayerEnum.Crewmate));

            if (TownOfUsReworked.IsTest)
                LogSomething("Added Solo Crew");
        }

        if (!spawnList.Any(x => x.ID == LayerEnum.Dracula) && spawnList.Any(x => x.ID == LayerEnum.VampireHunter))
        {
            var count = spawnList.RemoveAll(x => x.ID == LayerEnum.VampireHunter);

            while (count > 0)
            {
                spawnList.Add(GenerateRoleSpawnItem(LayerEnum.Vigilante));
                count--;
            }
        }

        while (spawnList.Count > players.Count)
        {
            spawnList.Shuffle();
            spawnList.Remove(spawnList[^1]);
        }

        spawnList.Shuffle();
        LogSomething("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Roles in the game: " + ids);
        }

        while (players.Count > 0 && spawnList.Count > 0)
            Gen(players.TakeFirst(), (int)spawnList.TakeFirst().ID, PlayerLayerEnum.Role);

        LogSomething("Role Gen End");
    }

    private static void GenRoleList()
    {
        LogSomething("Role Gen Start");
        var players = CustomPlayer.AllPlayers;
        var entries = CustomOption.GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Where(x => x.Name.Contains("Entry"));
        var bans = CustomOption.GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Where(x => x.Name.Contains("Ban"));
        var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get())).ToList();
        var randoms = entries.Where(x => RandomEntries.Contains(x.Get())).ToList();
        var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get()))).ToList();
        var anies = entries.Where(x => x.Get() == Any).ToList();
        //I have no idea what plural for any is lmao

        AllRoles.Clear();

        foreach (var entry in roles)
        {
            var ratelimit = 0;
            var id = entry.Get();
            var cachedCount = AllRoles.Count;

            while (cachedCount == AllRoles.Count)
            {
                ratelimit++;

                if (!AllRoles.Any(x => x.ID == id && x.Unique) && !bans.Any(x => x.Get() == id))
                    AllRoles.Add(GenerateRoleSpawnItem(id));

                if (ratelimit > 1000)
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
                ratelimit++;

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

                if (!AllRoles.Any(x => x.ID == random && x.Unique) && !bans.Any(x => x.Get() == random) && random != LayerEnum.None)
                    AllRoles.Add(GenerateRoleSpawnItem(random));

                if (ratelimit > 1000)
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
                ratelimit++;

                if (id == LayerEnum.RandomCrew)
                    random = Crew.Random().Random();
                else if (id == LayerEnum.RandomNeutral)
                    random = Neutral.Random().Random();
                else if (id == LayerEnum.RandomIntruder)
                    random = Intruders.Random().Random();
                else if (id == LayerEnum.RandomSyndicate)
                    random = Syndicate.Random().Random();

                if (!AllRoles.Any(x => x.ID == random && x.Unique) && !bans.Any(x => x.Get() == random) && random != LayerEnum.None)
                    AllRoles.Add(GenerateRoleSpawnItem(random));

                if (ratelimit > 1000)
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
                ratelimit++;

                if (!AllRoles.Any(x => x.ID == random && x.Unique) && !bans.Any(x => x.Get() == random))
                    AllRoles.Add(GenerateRoleSpawnItem(random));

                if (ratelimit > 1000)
                    break;
            }
        }

        //Added rate limits to ensure the loops do not go on forveer if roles have been set to unique

        //In case if the ratelimits disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < players.Count)
            AllRoles.Add(GenerateRoleSpawnItem(LayerEnum.Crewmate));

        var spawnList = AllRoles;
        spawnList.Shuffle();

        LogSomething("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Roles in the game: " + ids);
        }

        while (players.Count > 0 && spawnList.Count > 0)
            Gen(players.TakeFirst(), (int)spawnList.TakeFirst().ID, PlayerLayerEnum.Role);

        LogSomething("Role Spawn Done");
    }

    private static (int Chance, LayerEnum ID, bool Unique) GenerateRoleSpawnItem(LayerEnum id)
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
            _ => throw new NotImplementedException()
        };

        return (things.Item1, id, things.Item2);
    }

    private static (int Chance, LayerEnum ID, bool Unique) GenerateAbilitySpawnItem(LayerEnum id)
    {
        var things = id switch
        {
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
            _ => throw new NotImplementedException()
        };

        return (things.Item1, id, things.Item2);
    }

    private static (int Chance, LayerEnum ID, bool Unique) GenerateModifierSpawnItem(LayerEnum id)
    {
        var things = id switch
        {
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
            _ => throw new NotImplementedException(),
        };

        return (things.Item1, id, things.Item2);
    }

    private static (int Chance, LayerEnum ID, bool Unique) GenerateObjectifierSpawnItem(LayerEnum id)
    {
        var things = id switch
        {
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

        return (things.Item1, id, things.Item2);
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
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.CrewAssassin));
                num--;
            }

            LogSomething("Crew Assassin Done");
        }

        if (CustomGameOptions.SyndicateAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfSyndicateAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.SyndicateAssassin));
                num--;
            }

            LogSomething("Syndicate Assassin Done");
        }

        if (CustomGameOptions.IntruderAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfIntruderAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.IntruderAssassin));
                num--;
            }

            LogSomething("Intruder Assassin Done");
        }

        if (CustomGameOptions.NeutralAssassinOn > 0)
        {
            num = CustomGameOptions.NumberOfNeutralAssassins;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.NeutralAssassin));
                num--;
            }

            LogSomething("Neutral Assassin Done");
        }

        if (CustomGameOptions.RuthlessOn > 0)
        {
            num = CustomGameOptions.RuthlessCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Ruthless));
                num--;
            }

            LogSomething("Ruthless Done");
        }

        if (CustomGameOptions.SnitchOn > 0)
        {
            num = CustomGameOptions.SnitchCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Snitch));
                num--;
            }

            LogSomething("Snitch Done");
        }

        if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
        {
            num = CustomGameOptions.InsiderCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Insider));
                num--;
            }

            LogSomething("Insider Done");
        }

        if (CustomGameOptions.MultitaskerOn > 0)
        {
            num = CustomGameOptions.MultitaskerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Multitasker));
                num--;
            }

            LogSomething("Multitasker Done");
        }

        if (CustomGameOptions.RadarOn > 0)
        {
            num = CustomGameOptions.RadarCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Radar));
                num--;
            }

            LogSomething("Radar Done");
        }

        if (CustomGameOptions.TiebreakerOn > 0)
        {
            num = CustomGameOptions.TiebreakerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Tiebreaker));
                num--;
            }

            LogSomething("Tiebreaker Done");
        }

        if (CustomGameOptions.TorchOn > 0)
        {
            num = CustomGameOptions.TorchCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Torch));
                num--;
            }

            LogSomething("Torch Done");
        }

        if (CustomGameOptions.UnderdogOn > 0)
        {
            num = CustomGameOptions.UnderdogCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Underdog));
                num--;
            }

            LogSomething("Underdog Done");
        }

        if (CustomGameOptions.TunnelerOn > 0 && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
        {
            num = CustomGameOptions.TunnelerCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Tunneler));
                num--;
            }

            LogSomething("Tunneler Done");
        }

        if (CustomGameOptions.NinjaOn > 0)
        {
            num = CustomGameOptions.NinjaCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Ninja));
                num--;
            }

            LogSomething("Ninja Done");
        }

        if (CustomGameOptions.ButtonBarryOn > 0)
        {
            num = CustomGameOptions.ButtonBarryCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.ButtonBarry));
                num--;
            }

            LogSomething("Button Barry Done");
        }

        if (CustomGameOptions.PoliticianOn > 0)
        {
            num = CustomGameOptions.PoliticianCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Politician));
                num--;
            }

            LogSomething("Politician Done");
        }

        if (CustomGameOptions.SwapperOn > 0)
        {
            num = CustomGameOptions.SwapperCount;

            while (num > 0)
            {
                AllAbilities.Add(GenerateAbilitySpawnItem(LayerEnum.Swapper));
                num--;
            }

            LogSomething("Swapper Done");
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

        canHaveNeutralAbility.RemoveAll(x => !(x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralHarb)));
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

        canHaveTorch.RemoveAll(x => (x.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NKHasImpVision) || x.Is(Faction.Syndicate) || (x.Is(Faction.Neutral) &&
            !CustomGameOptions.LightsAffectNeutrals) || x.Is(Faction.Intruder));
        canHaveTorch.Shuffle();

        canHaveEvilAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
        canHaveEvilAbility.Shuffle();

        canHaveKillingAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(LayerEnum.Corrupted)));
        canHaveKillingAbility.Shuffle();

        canHaveRuthless.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(LayerEnum.Corrupted)) || x.Is(LayerEnum.Juggernaut));
        canHaveRuthless.Shuffle();

        canHaveBB.RemoveAll(x => (x.Is(LayerEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(LayerEnum.Jester) && !CustomGameOptions.JesterButton) || (x.Is(LayerEnum.Actor) &&
            !CustomGameOptions.ActorButton) || (x.Is(LayerEnum.Guesser) && !CustomGameOptions.GuesserButton) || (x.Is(LayerEnum.Executioner) && !CustomGameOptions.ExecutionerButton) ||
            (!CustomGameOptions.MonarchButton && x.Is(LayerEnum.Monarch)) || (!CustomGameOptions.DictatorButton && x.Is(LayerEnum.Dictator)));
        canHaveBB.Shuffle();

        canHavePolitician.RemoveAll(x => x.Is(RoleAlignment.NeutralEvil) || x.Is(RoleAlignment.NeutralBen) || x.Is(RoleAlignment.NeutralNeo));
        canHavePolitician.Shuffle();

        var spawnList = AllAbilities;
        spawnList.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Abilities in the game: " + ids);
        }

        while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
            canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
            canHaveTaskedAbility.Count > 0 || canHaveTorch.Count > 0 || canHaveKillingAbility.Count > 0)
        {
            if (spawnList.Count == 0)
                break;

            var id = spawnList.TakeFirst().ID;
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
                    spawnList.Add(GenerateAbilitySpawnItem(id));
            }
        }

        LogSomething("Abilities Done");
    }

    private static void GenObjectifiers()
    {
        AllObjectifiers.Clear();
        var num = 0;

        if (CustomGameOptions.LoversOn > 0 && GameData.Instance.PlayerCount > 4)
        {
            num = CustomGameOptions.LoversCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Lovers));
                num--;
            }

            LogSomething("Lovers Done");
        }

        if (CustomGameOptions.RivalsOn > 0 && GameData.Instance.PlayerCount > 3)
        {
            num = CustomGameOptions.RivalsCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Rivals));
                num--;
            }

            LogSomething("Rivals Done");
        }

        if (CustomGameOptions.FanaticOn > 0)
        {
            num = CustomGameOptions.FanaticCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Fanatic));
                num--;
            }

            LogSomething("Fanatic Done");
        }

        if (CustomGameOptions.CorruptedOn > 0)
        {
            num = CustomGameOptions.CorruptedCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Corrupted));
                num--;
            }

            LogSomething("Corrupted Done");
        }

        if (CustomGameOptions.OverlordOn > 0)
        {
            num = CustomGameOptions.OverlordCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Overlord));
                num--;
            }

            LogSomething("Overlord Done");
        }

        if (CustomGameOptions.AlliedOn > 0)
        {
            num = CustomGameOptions.AlliedCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Allied));
                num--;
            }

            LogSomething("Allied Done");
        }

        if (CustomGameOptions.TraitorOn > 0)
        {
            num = CustomGameOptions.TraitorCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Traitor));
                num--;
            }

            LogSomething("Traitor Done");
        }

        if (CustomGameOptions.TaskmasterOn > 0)
        {
            num = CustomGameOptions.TaskmasterCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Taskmaster));
                num--;
            }

            LogSomething("Taskmaster Done");
        }

        if (CustomGameOptions.MafiaOn > 0)
        {
            num = CustomGameOptions.MafiaCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Mafia));
                num--;
            }

            LogSomething("Mafia Done");
        }

        if (CustomGameOptions.DefectorOn > 0)
        {
            num = CustomGameOptions.DefectorCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Defector));
                num--;
            }

            LogSomething("Defector Done");
        }

        if (CustomGameOptions.LinkedOn > 0 && Role.GetRoles(Faction.Neutral).Count > 1 && GameData.Instance.PlayerCount > 3)
        {
            num = CustomGameOptions.LinkedCount;

            while (num > 0)
            {
                AllObjectifiers.Add(GenerateObjectifierSpawnItem(LayerEnum.Linked));
                num--;
            }

            LogSomething("Linked Done");
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

        canHaveAllied.RemoveAll(x => !x.Is(RoleAlignment.NeutralKill) || x == PureCrew);
        canHaveAllied.Shuffle();

        canHaveObjectifier.RemoveAll(x => x == PureCrew);
        canHaveObjectifier.Shuffle();

        canHaveDefector.RemoveAll(x => x == PureCrew || !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
        canHaveDefector.Shuffle();

        var spawnList = AllObjectifiers;
        spawnList.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Objectifiers in the game: " + ids);
        }

        while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 1 || canHaveObjectifier.Count > 0 || canHaveDefector.Count > 0)
        {
            if (spawnList.Count == 0)
                break;

            var (_, id, _) = spawnList.TakeFirst();
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
                    spawnList.Add(GenerateObjectifierSpawnItem(id));
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

        LogSomething("Objectifiers Done");
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
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Diseased));
                num--;
            }

            LogSomething("Diseased Done");
        }

        if (CustomGameOptions.BaitOn > 0)
        {
            num = CustomGameOptions.BaitCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Bait));
                num--;
            }

            LogSomething("Bait Done");
        }

        if (CustomGameOptions.DwarfOn > 0)
        {
            num = CustomGameOptions.DwarfCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Dwarf));
                num--;
            }

            LogSomething("Dwarf Done");
        }

        if (CustomGameOptions.VIPOn > 0)
        {
            num = CustomGameOptions.VIPCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.VIP));
                num--;
            }

            LogSomething("VIP Done");
        }

        if (CustomGameOptions.ShyOn > 0)
        {
            num = CustomGameOptions.ShyCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Shy));
                num--;
            }

            LogSomething("Shy Done");
        }

        if (CustomGameOptions.GiantOn > 0)
        {
            num = CustomGameOptions.GiantCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Giant));
                num--;
            }

            LogSomething("Giant Done");
        }

        if (CustomGameOptions.DrunkOn > 0)
        {
            num = CustomGameOptions.DrunkCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Drunk));
                num--;
            }

            LogSomething("Drunk Done");
        }

        if (CustomGameOptions.CowardOn > 0)
        {
            num = CustomGameOptions.CowardCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Coward));
                num--;
            }

            LogSomething("Coward Done");
        }

        if (CustomGameOptions.VolatileOn > 0)
        {
            num = CustomGameOptions.VolatileCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Volatile));
                num--;
            }

            LogSomething("Volatile Done");
        }

        if (CustomGameOptions.IndomitableOn > 0)
        {
            num = CustomGameOptions.IndomitableCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Indomitable));
                num--;
            }

            LogSomething("Indomitable Done");
        }

        if (CustomGameOptions.ProfessionalOn > 0)
        {
            num = CustomGameOptions.ProfessionalCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Professional));
                num--;
            }

            LogSomething("Professional Done");
        }

        if (CustomGameOptions.AstralOn > 0)
        {
            num = CustomGameOptions.AstralCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Astral));
                num--;
            }

            LogSomething("Astral Done");
        }

        if (CustomGameOptions.YellerOn > 0)
        {
            num = CustomGameOptions.YellerCount;

            while (num > 0)
            {
                AllModifiers.Add(GenerateModifierSpawnItem(LayerEnum.Yeller));
                num--;
            }

            LogSomething("Yeller Done");
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

        var spawnList = AllModifiers;
        spawnList.Shuffle();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var (_, id, _) in spawnList)
                ids += $" {id}";

            LogSomething("Modifiers in the game: " + ids);
        }

        while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
        {
            if (spawnList.Count == 0)
                break;

            var (_, id, _) = spawnList.TakeFirst();
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
                    spawnList.Add(GenerateModifierSpawnItem(id));
            }
        }

        LogSomething("Modifiers Done");
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
                alliedRole.RoleAlignment = alliedRole.RoleAlignment.GetNewAlignment((Faction)faction);
                ally.Data.SetImpostor((Faction)faction is Faction.Intruder or Faction.Syndicate);
                CallRpc(CustomRPC.Target, TargetRPC.SetAlliedFaction, ally.Player, faction);
            }

            LogSomething("Allied Faction Set Done");
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
                    LogSomething($"Lovers = {lover.PlayerName} & {other.PlayerName}");
            }

            foreach (var lover in lovers)
            {
                if (lover.OtherLover == null)
                    NullLayer(lover.Player, PlayerLayerEnum.Objectifier);
            }

            LogSomething("Lovers Set");
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
                    LogSomething($"Rivals = {rival.PlayerName} & {other.PlayerName}");
            }

            foreach (var rival in rivals)
            {
                if (rival.OtherRival == null)
                    NullLayer(rival.Player, PlayerLayerEnum.Objectifier);
            }

            LogSomething("Rivals Set");
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
                    LogSomething($"Linked = {link.PlayerName} & {other.PlayerName}");
            }

            foreach (var link in linked)
            {
                if (link.OtherLink == null)
                    NullLayer(link.Player, PlayerLayerEnum.Objectifier);
            }

            LogSomething("Linked Set");
        }

        if (CustomGameOptions.MafiaOn > 0)
        {
            if (Objectifier.GetObjectifiers(LayerEnum.Mafia).Count == 1)
            {
                foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(LayerEnum.Mafia)))
                    NullLayer(player, PlayerLayerEnum.Objectifier);
            }

            LogSomething("Mafia Set");
        }

        if (CustomGameOptions.ExecutionerOn > 0 && !CustomGameOptions.ExecutionerCanPickTargets)
        {
            foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
            {
                exe.TargetPlayer = null;
                var ratelimit = 0;

                while (!exe.TargetPlayer || exe.TargetPlayer == exe.Player || exe.TargetPlayer.IsLinkedTo(exe.Player) || exe.TargetPlayer.Is(RoleAlignment.CrewSov))
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
                        LogSomething($"Exe Target = {exe.TargetPlayer?.name}");
                }
            }

            LogSomething("Exe Targets Set");
        }

        if (CustomGameOptions.GuesserOn > 0 && !CustomGameOptions.GuesserCanPickTargets)
        {
            foreach (var guess in Role.GetRoles<Guesser>(LayerEnum.Guesser))
            {
                guess.TargetPlayer = null;
                var ratelimit = 0;

                while (!guess.TargetPlayer || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(LayerEnum.Indomitable) || guess.TargetPlayer.IsLinkedTo(guess.Player) ||
                    guess.TargetPlayer.Is(RoleAlignment.CrewInvest))
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
                        LogSomething($"Guess Target = {guess.TargetPlayer?.name}");
                }
            }

            LogSomething("Guess Targets Set");
        }

        if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.GuardianAngelCanPickTargets)
        {
            foreach (var ga in Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel))
            {
                ga.TargetPlayer = null;
                var ratelimit = 0;

                while (!ga.TargetPlayer || ga.TargetPlayer == ga.Player || ga.TargetPlayer.IsLinkedTo(ga.Player) || ga.TargetPlayer.Is(RoleAlignment.NeutralEvil))
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
                        LogSomething($"GA Target = {ga.TargetPlayer?.name}");
                }
            }

            LogSomething("GA Target Set");
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
                        LogSomething($"BH Target = {bh.TargetPlayer?.name}");
                }
            }

            LogSomething("BH Targets Set");
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
                        LogSomething(act.PretendRoles);
                }
            }

            LogSomething("Act Variables Set");
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

                while (goodRecruit == null || goodRecruit == jackal.Player || goodRecruit.Is(RoleAlignment.NeutralKill) || goodRecruit.Is(RoleAlignment.NeutralHarb) ||
                    goodRecruit.Is(Faction.Intruder) || goodRecruit.Is(Faction.Syndicate) || goodRecruit.Is(RoleAlignment.NeutralNeo) || goodRecruit.Is(SubFaction.Cabal))
                {
                    goodRecruit = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                ratelimit = 0;

                while (evilRecruit == null || evilRecruit == jackal.Player || evilRecruit.Is(Faction.Crew) || evilRecruit.Is(RoleAlignment.NeutralBen) ||
                    evilRecruit.Is(RoleAlignment.NeutralNeo) || evilRecruit.Is(SubFaction.Cabal))
                {
                    evilRecruit = CustomPlayer.AllPlayers.Random();
                    ratelimit++;

                    if (ratelimit > 1000)
                        break;
                }

                if (!(goodRecruit == null || goodRecruit == jackal.Player || goodRecruit.Is(RoleAlignment.NeutralKill) || goodRecruit.Is(RoleAlignment.NeutralHarb) ||
                    goodRecruit.Is(Faction.Intruder) || goodRecruit.Is(Faction.Syndicate) || goodRecruit.Is(RoleAlignment.NeutralNeo) || goodRecruit.Is(SubFaction.Cabal)))
                {
                    RpcConvert(goodRecruit.PlayerId, jackal.PlayerId, SubFaction.Cabal);
                }

                if (!(evilRecruit == null || evilRecruit == jackal.Player || evilRecruit.Is(Faction.Crew) || evilRecruit.Is(RoleAlignment.NeutralBen) ||
                    evilRecruit.Is(RoleAlignment.NeutralNeo) || evilRecruit.Is(SubFaction.Cabal)))
                {
                    RpcConvert(evilRecruit.PlayerId, jackal.PlayerId, SubFaction.Cabal);
                }

                if (TownOfUsReworked.IsTest)
                    LogSomething($"Recruits = {jackal.GoodRecruit?.name} (Good) & {jackal.EvilRecruit?.name} (Evil)");
            }

            LogSomething("Jackal Recruits Set");
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

            if ((!CustomGameOptions.MayorButton && player.Is(LayerEnum.Mayor)) || (!CustomGameOptions.SwapperButton && player.Is(LayerEnum.Swapper)) || (!CustomGameOptions.ActorButton
                && player.Is(LayerEnum.Actor)) || player.Is(LayerEnum.Shy) || (!CustomGameOptions.ExecutionerButton && player.Is(LayerEnum.Executioner)) ||
                (!CustomGameOptions.GuesserButton && player.Is(LayerEnum.Guesser)) || (!CustomGameOptions.JesterButton && player.Is(LayerEnum.Jester)) ||
                (!CustomGameOptions.PoliticianButton && player.Is(LayerEnum.Politician)) || (!CustomGameOptions.DictatorButton && player.Is(LayerEnum.Dictator)) ||
                (!CustomGameOptions.MonarchButton && player.Is(LayerEnum.Monarch)))
            {
                player.RemainingEmergencies = 0;
                CallRpc(CustomRPC.Misc, MiscRPC.RemoveMeetings, player);
            }

            if (TownOfUsReworked.IsTest)
                LogSomething($"{player.name} -> {Role.GetRole(player)}, {Objectifier.GetObjectifier(player)}, {Modifier.GetModifier(player)}, {Ability.GetAbility(player)}");
        }

        LogSomething("Players Set");
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
        Role.InfectorsWin = false;

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

        CustomButton.AllButtons.Clear();

        Ash.AllPiles.Clear();
        Objects.Range.AllItems.Clear();

        GameSettings.SettingsPage = 0;

        Assassin.RemainingKills = CustomGameOptions.AssassinKills;

        Summary.Disconnected.Clear();

        Footprint.OddEven.Clear();

        CachedFirstDead = FirstDead;
        FirstDead = null;

        //Role.IsLeft = false;
    }

    public static void BeginRoleGen()
    {
        if (IsHnS)
            return;

        ResetEverything();
        CallRpc(CustomRPC.Misc, MiscRPC.Start);
        LogSomething("Cleared Variables");

        if (IsKilling)
            GenKilling();
        else if (IsVanilla)
            GenVanilla();
        else if (IsRoleList)
            GenRoleList();
        else
            GenClassicCustomAA();

        PureCrew = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew)).ToList().Random();

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

    private static void NullLayer(PlayerControl player, PlayerLayerEnum rpc)
    {
        SetLayer(10000, player, rpc);
        CallRpc(CustomRPC.Misc, MiscRPC.SetLayer, 10000, player, rpc);
    }

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
        if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate)))
            return;

        var all = CustomPlayer.AllPlayers.Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate)).ToList();
        PlayerControl chosen = null;

        if (Role.DriveHolder == null || Role.DriveHolder.Data.IsDead || Role.DriveHolder.Data.Disconnected)
        {
            chosen = all.Find(x => x.Is(LayerEnum.PromotedRebel));

            if (chosen == null)
                chosen = all.Find(x => x.Is(RoleAlignment.SyndicateDisrup));

            if (chosen == null)
                chosen = all.Find(x => x.Is(RoleAlignment.SyndicateSupport));

            if (chosen == null)
                chosen = all.Find(x => x.Is(RoleAlignment.SyndicatePower));

            if (chosen == null)
                chosen = all.Find(x => x.Is(RoleAlignment.SyndicateKill));

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
            Interact(converter, converted, true, true);
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
                role1.RoleAlignment = role1.RoleAlignment.GetNewAlignment(Faction.Neutral);
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