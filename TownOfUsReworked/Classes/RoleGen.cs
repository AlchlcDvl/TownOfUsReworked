namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class RoleGen
    {
        private static readonly List<(int Chance, RoleEnum Id, bool Unique)> CrewAuditorRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewKillingRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewSupportRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewSovereignRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewProtectiveRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewInvestigativeRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> CrewRoles = new();

        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralEvilRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralBenignRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralKillingRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralNeophyteRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralHarbingerRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> NeutralRoles = new();

        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> IntruderKillingRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> IntruderSupportRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> IntruderDeceptionRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> IntruderConcealingRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> IntruderRoles = new();

        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> SyndicatePowerRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> SyndicateSupportRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> SyndicateKillingRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> SyndicateDisruptionRoles = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> SyndicateRoles = new();

        private static readonly List<(int Chance, ModifierEnum ID, bool Unique)> AllModifiers = new();
        private static readonly List<(int Chance, AbilityEnum ID, bool Unique)> AllAbilities = new();
        private static readonly List<(int Chance, ObjectifierEnum ID, bool Unique)> AllObjectifiers = new();
        private static readonly List<(int Chance, RoleEnum ID, bool Unique)> AllRoles = new();

        public static PlayerControl PureCrew;
        public static int Convertible;

        private const RoleEnum Any = RoleEnum.Any;
        private static readonly List<RoleEnum> CA = new() { RoleEnum.Mystic, RoleEnum.VampireHunter };
        private static readonly List<RoleEnum> CI = new() { RoleEnum.Sheriff, RoleEnum.Inspector, RoleEnum.Tracker, RoleEnum.Medium, RoleEnum.Coroner, RoleEnum.Operative, RoleEnum.Seer,
            RoleEnum.Detective };
        private static readonly List<RoleEnum> CSv = new() { RoleEnum.Mayor, RoleEnum.Dictator, RoleEnum.Monarch };
        private static readonly List<RoleEnum> CP = new() { RoleEnum.Altruist, RoleEnum.Medic };
        private static readonly List<RoleEnum> CK = new() { RoleEnum.Vigilante, RoleEnum.Veteran};
        private static readonly List<RoleEnum> CS = new() { RoleEnum.Engineer, RoleEnum.Transporter, RoleEnum.Escort, RoleEnum.Shifter, RoleEnum.Chameleon, RoleEnum.Retributionist };
        private static readonly List<List<RoleEnum>> Crew = new() { CA, CI, CSv, CP, CK, CS };
        private static readonly List<RoleEnum> NB = new() { RoleEnum.Amnesiac, RoleEnum.GuardianAngel, RoleEnum.Survivor, RoleEnum.Thief };
        private static readonly List<RoleEnum> NE = new() { RoleEnum.Jester, RoleEnum.Actor, RoleEnum.BountyHunter, RoleEnum.Cannibal, RoleEnum.Executioner, RoleEnum.Guesser,
            RoleEnum.Troll };
        private static readonly List<RoleEnum> NN = new() { RoleEnum.Jackal, RoleEnum.Necromancer, RoleEnum.Dracula, RoleEnum.Whisperer };
        private static readonly List<RoleEnum> NH = new() { RoleEnum.Plaguebearer };
        private static readonly List<RoleEnum> NA = new() { RoleEnum.Pestilence };
        private static readonly List<RoleEnum> NK = new() { RoleEnum.Arsonist, RoleEnum.Cryomaniac, RoleEnum.Glitch, RoleEnum.Juggernaut, RoleEnum.Murderer, RoleEnum.SerialKiller,
            RoleEnum.Werewolf };
        private static readonly List<List<RoleEnum>> Neutral = new() { NB, NE, NN, NH, NK };
        private static readonly List<RoleEnum> IC = new() { RoleEnum.Blackmailer, RoleEnum.Camouflager, RoleEnum.Grenadier, RoleEnum.Janitor };
        private static readonly List<RoleEnum> ID = new() { RoleEnum.Morphling, RoleEnum.Disguiser, RoleEnum.Wraith };
        private static readonly List<RoleEnum> IK = new() { RoleEnum.Enforcer, RoleEnum.Ambusher };
        private static readonly List<RoleEnum> IS = new() { RoleEnum.Consigliere, RoleEnum.Godfather, RoleEnum.Miner, RoleEnum.Teleporter, RoleEnum.Consort };
        private static readonly List<List<RoleEnum>> Intruders = new() { IC, ID, IK, IS };
        private static readonly List<RoleEnum> SSu = new() { RoleEnum.Rebel, RoleEnum.Warper, RoleEnum.Stalker };
        private static readonly List<RoleEnum> SD = new() { RoleEnum.Concealer, RoleEnum.Drunkard, RoleEnum.Framer, RoleEnum.Shapeshifter, RoleEnum.Silencer };
        private static readonly List<RoleEnum> SP = new() { RoleEnum.TimeKeeper, RoleEnum.Spellslinger };
        private static readonly List<RoleEnum> SyK = new() { RoleEnum.Bomber, RoleEnum.Collider, RoleEnum.Crusader, RoleEnum.Poisoner };
        private static readonly List<List<RoleEnum>> Syndicate = new() { SSu, SyK, SD, SP };
        private static readonly List<RoleEnum> AlignmentEntries = new() { RoleEnum.CrewSupport, RoleEnum.CrewInvest, RoleEnum.CrewSov, RoleEnum.CrewProt, RoleEnum.CrewKill,
            RoleEnum.CrewAudit, RoleEnum.IntruderSupport, RoleEnum.IntruderConceal, RoleEnum.IntruderDecep, RoleEnum.IntruderKill, RoleEnum.NeutralApoc, RoleEnum.NeutralHarb,
            RoleEnum.NeutralBen, RoleEnum.NeutralEvil, RoleEnum.NeutralKill, RoleEnum.NeutralNeo, RoleEnum.SyndicateDisrup, RoleEnum.SyndicateKill, RoleEnum.SyndicatePower,
            RoleEnum.SyndicatePower };
        private static readonly List<RoleEnum> RandomEntries = new() { RoleEnum.RandomCrew, RoleEnum.RandomIntruder, RoleEnum.RandomSyndicate, RoleEnum.RandomNeutral };
        private static readonly List<List<RoleEnum>> Alignments = new() { CA, CI, CSv, CP, CK, CS, NB, NE, NN, NH, NK, IC, ID, IS, SSu, SD, SP, SyK, IK, NA };

        private static void Sort(this List<(int Chance, RoleEnum ID, bool Unique)> items, int max, int min)
        {
            if (items.Count is 0 or 1)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min > max)
                (max, min) = (min, max);

            var amount = URandom.RandomRangeInt(min, max + 1);
            var tempList = new List<(int Chance, RoleEnum ID, bool Unique)>();

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance == 100)
                    tempList.Add(item);
            }

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < item.Chance)
                        tempList.Add(item);
                }
            }

            tempList.Shuffle();
            items = tempList;
        }

        private static void Sort(this List<(int Chance, AbilityEnum ID, bool Unique)> items, int max, int min)
        {
            if (items.Count is 0 or 1)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min > max)
                (max, min) = (min, max);

            var amount = URandom.RandomRangeInt(min, max + 1);
            var tempList = new List<(int Chance, AbilityEnum ID, bool Unique)>();

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance == 100)
                    tempList.Add(item);
            }

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < item.Chance)
                        tempList.Add(item);
                }
            }

            tempList.Shuffle();
            items = tempList;
        }

        private static void Sort(this List<(int Chance, ModifierEnum ID, bool Unique)> items, int max, int min)
        {
            if (items.Count is 0 or 1)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min > max)
                (max, min) = (min, max);

            var amount = URandom.RandomRangeInt(min, max + 1);
            var tempList = new List<(int Chance, ModifierEnum ID, bool Unique)>();

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance == 100)
                    tempList.Add(item);
            }

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < item.Chance)
                        tempList.Add(item);
                }
            }

            tempList.Shuffle();
            items = tempList;
        }

        private static void Sort(this List<(int Chance, ObjectifierEnum ID, bool Unique)> items, int max, int min)
        {
            if (items.Count is 0 or 1)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min > max)
                (max, min) = (min, max);

            var amount = URandom.RandomRangeInt(min, max + 1);
            var tempList = new List<(int Chance, ObjectifierEnum ID, bool Unique)>();

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance == 100)
                    tempList.Add(item);
            }

            foreach (var item in items)
            {
                if (tempList.Count >= amount)
                    break;

                if (item.Chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < item.Chance)
                        tempList.Add(item);
                }
            }

            tempList.Shuffle();
            items = tempList;
        }

        private static void Sort(this List<(int Chance, RoleEnum ID, bool Unique)> items, int amount)
        {
            var newList = new List<(int Chance, RoleEnum ID, bool Unique)>();

            if (IsAA)
            {
                if (amount < CustomPlayer.AllPlayers.Count)
                    amount = CustomPlayer.AllPlayers.Count;

                while (newList.Count < amount && items.Count > 0)
                {
                    items.Shuffle();
                    newList.Add(items[0]);

                    if (items[0].Unique && CustomGameOptions.EnableUniques)
                        items.Remove(items[0]);

                    newList.Shuffle();
                }
            }
            else
            {
                items.Shuffle();

                if (amount < items.Count)
                    amount = items.Count;

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

                newList.Shuffle();
                items = newList;
            }

            items = newList;
        }

        private static void Sort(this List<(int Chance, AbilityEnum ID, bool Unique)> items, int amount)
        {
            var newList = new List<(int Chance, AbilityEnum ID, bool Unique)>();

            if (IsAA)
            {
                if (amount < CustomPlayer.AllPlayers.Count)
                    amount = CustomPlayer.AllPlayers.Count;

                while (newList.Count < amount && items.Count > 0)
                {
                    items.Shuffle();
                    newList.Add(items[0]);

                    if (items[0].Unique && CustomGameOptions.EnableUniques)
                        items.Remove(items[0]);

                    newList.Shuffle();
                }
            }
            else
            {
                items.Shuffle();

                if (amount < items.Count)
                    amount = items.Count;

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

                newList.Shuffle();
                items = newList;
            }

            items = newList;
        }

        private static void Sort(this List<(int Chance, ModifierEnum ID, bool Unique)> items, int amount)
        {
            var newList = new List<(int Chance, ModifierEnum ID, bool Unique)>();

            if (IsAA)
            {
                if (amount < CustomPlayer.AllPlayers.Count)
                    amount = CustomPlayer.AllPlayers.Count;

                while (newList.Count < amount && items.Count > 0)
                {
                    items.Shuffle();
                    newList.Add(items[0]);

                    if (items[0].Unique && CustomGameOptions.EnableUniques)
                        items.Remove(items[0]);

                    newList.Shuffle();
                }
            }
            else
            {
                items.Shuffle();

                if (amount < items.Count)
                    amount = items.Count;

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

                newList.Shuffle();
                items = newList;
            }

            items = newList;
        }

        private static void Sort(this List<(int Chance, ObjectifierEnum ID, bool Unique)> items, int amount)
        {
            var newList = new List<(int Chance, ObjectifierEnum ID, bool Unique)>();

            if (IsAA)
            {
                if (amount < CustomPlayer.AllPlayers.Count)
                    amount = CustomPlayer.AllPlayers.Count;

                while (newList.Count < amount && items.Count > 0)
                {
                    items.Shuffle();
                    newList.Add(items[0]);

                    if (items[0].Unique && CustomGameOptions.EnableUniques)
                        items.Remove(items[0]);

                    newList.Shuffle();
                }
            }
            else
            {
                items.Shuffle();

                if (amount < items.Count)
                    amount = items.Count;

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

                newList.Shuffle();
                items = newList;
            }

            items = newList;
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
                AllRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.AltImps ? RoleEnum.Anarchist : RoleEnum.Impostor));

            while (AllRoles.Count < players.Count)
                AllRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crewmate));

            var spawnList = AllRoles;
            spawnList.Shuffle();

            LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

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
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Enforcer));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Morphling));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Blackmailer));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Miner));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Teleporter));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Wraith));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Consort));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Janitor));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Camouflager));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Grenadier));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Impostor));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Consigliere));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Disguiser));
            IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Ambusher));

            if (imps >= 3)
                IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Godfather));

            SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Anarchist));
            SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Bomber));
            SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Poisoner));
            SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crusader));
            SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Collider));

            if (syn >= 3)
                SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Rebel));

            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Glitch));
            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Werewolf));
            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.SerialKiller));
            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Juggernaut));
            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Murderer));
            NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Thief));

            if (CustomGameOptions.AddArsonist)
                NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Arsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralRoles.Add(GenerateRoleSpawnItem(RoleEnum.Cryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
                NeutralRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.PestSpawn ? RoleEnum.Pestilence : RoleEnum.Plaguebearer));

            NeutralRoles.Sort(neutrals);

            var vigis = crew / 2;
            var vets = crew / 2;

            while (vigis > 0 || vets > 0)
            {
                if (vigis > 0)
                {
                    CrewRoles.Add(GenerateRoleSpawnItem(RoleEnum.Vigilante));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add(GenerateRoleSpawnItem(RoleEnum.Veteran));
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
                    ids += $"{id}, ";

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
                    CrewSovereignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Mayor));
                    num--;
                }

                LogSomething("Mayor Done");
            }

            if (CustomGameOptions.MonarchOn > 0)
            {
                num = CustomGameOptions.MonarchCount;

                while (num > 0)
                {
                    CrewSovereignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Monarch));
                    num--;
                }

                LogSomething("Monarch Done");
            }

            if (CustomGameOptions.DictatorOn > 0)
            {
                num = CustomGameOptions.DictatorCount;

                while (num > 0)
                {
                    CrewSovereignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Dictator));
                    num--;
                }

                LogSomething("Dictator Done");
            }

            if (CustomGameOptions.SheriffOn > 0)
            {
                num = CustomGameOptions.SheriffCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Sheriff));
                    num--;
                }

                LogSomething("Sheriff Done");
            }

            if (CustomGameOptions.InspectorOn > 0)
            {
                num = CustomGameOptions.InspectorCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Inspector));
                    num--;
                }

                LogSomething("Inspector Done");
            }

            if (CustomGameOptions.VigilanteOn > 0)
            {
                num = CustomGameOptions.VigilanteCount;

                while (num > 0)
                {
                    CrewKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Vigilante));
                    num--;
                }

                LogSomething("Vigilante Done");
            }

            if (CustomGameOptions.EngineerOn > 0)
            {
                num = CustomGameOptions.EngineerCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Engineer));
                    num--;
                }

                LogSomething("Engineer Done");
            }

            if (CustomGameOptions.MedicOn > 0)
            {
                num = CustomGameOptions.MedicCount;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add(GenerateRoleSpawnItem(RoleEnum.Medic));
                    num--;
                }

                LogSomething("Medic Done");
            }

            if (CustomGameOptions.AltruistOn > 0)
            {
                num = CustomGameOptions.AltruistCount;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add(GenerateRoleSpawnItem(RoleEnum.Altruist));
                    num--;
                }

                LogSomething("Altruist Done");
            }

            if (CustomGameOptions.VeteranOn > 0)
            {
                num = CustomGameOptions.VeteranCount;

                while (num > 0)
                {
                    CrewKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Veteran));
                    num--;
                }

                LogSomething("Veteran Done");
            }

            if (CustomGameOptions.TrackerOn > 0)
            {
                num = CustomGameOptions.TrackerCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Tracker));
                    num--;
                }

                LogSomething("Tracker Done");
            }

            if (CustomGameOptions.TransporterOn > 0)
            {
                num = CustomGameOptions.TransporterCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Transporter));
                    num--;
                }

                LogSomething("Transporter Done");
            }

            if (CustomGameOptions.MediumOn > 0)
            {
                num = CustomGameOptions.MediumCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Medium));
                    num--;
                }

                LogSomething("Medium Done");
            }

            if (CustomGameOptions.CoronerOn > 0)
            {
                num = CustomGameOptions.CoronerCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Coroner));
                    num--;
                }

                LogSomething("Coroner Done");
            }

            if (CustomGameOptions.OperativeOn > 0)
            {
                num = CustomGameOptions.OperativeCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Operative));
                    num--;
                }

                LogSomething("Operative Done");
            }

            if (CustomGameOptions.DetectiveOn > 0)
            {
                num = CustomGameOptions.DetectiveCount;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Detective));
                    num--;
                }

                LogSomething("Detective Done");
            }

            if (CustomGameOptions.EscortOn > 0)
            {
                num = CustomGameOptions.EscortCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Escort));
                    num--;
                }

                LogSomething("Escort Done");
            }

            if (CustomGameOptions.ShifterOn > 0)
            {
                num = CustomGameOptions.ShifterCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Shifter));
                    num--;
                }

                LogSomething("Shifter Done");
            }

            if (CustomGameOptions.ChameleonOn > 0)
            {
                num = CustomGameOptions.ChameleonCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Chameleon));
                    num--;
                }

                LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.RetributionistOn > 0)
            {
                num = CustomGameOptions.RetributionistCount;

                while (num > 0)
                {
                    CrewSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Retributionist));
                    num--;
                }

                LogSomething("Retributionist Done");
            }

            if (CustomGameOptions.CrewmateOn > 0 && IsCustom)
            {
                num = CustomGameOptions.CrewCount;

                while (num > 0)
                {
                    CrewRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crewmate));
                    num--;
                }

                LogSomething("Crewmate Done");
            }

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
            {
                num = CustomGameOptions.VampireHunterCount;

                while (num > 0)
                {
                    CrewAuditorRoles.Add(GenerateRoleSpawnItem(RoleEnum.VampireHunter));
                    num--;
                }

                LogSomething("Vampire Hunter Done");
            }

            if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 ||
                CustomGameOptions.JackalOn > 0))
            {
                num = CustomGameOptions.MysticCount;

                while (num > 0)
                {
                    CrewAuditorRoles.Add(GenerateRoleSpawnItem(RoleEnum.Mystic));
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
                    CrewInvestigativeRoles.Add(GenerateRoleSpawnItem(RoleEnum.Seer));
                    num--;
                }

                LogSomething("Seer Done");
            }

            if (CustomGameOptions.JesterOn > 0)
            {
                num = CustomGameOptions.JesterCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Jester));
                    num--;
                }

                LogSomething("Jester Done");
            }

            if (CustomGameOptions.AmnesiacOn > 0)
            {
                num = CustomGameOptions.AmnesiacCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Amnesiac));
                    num--;
                }

                LogSomething("Amnesiac Done");
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                num = CustomGameOptions.ExecutionerCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Executioner));
                    num--;
                }

                LogSomething("Executioner Done");
            }

            if (CustomGameOptions.SurvivorOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = CustomGameOptions.SurvivorCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Survivor));
                    num--;
                }

                LogSomething("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = CustomGameOptions.GuardianAngelCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateRoleSpawnItem(RoleEnum.GuardianAngel));
                    num--;
                }

                LogSomething("Guardian Angel Done");
            }

            if (CustomGameOptions.GlitchOn > 0)
            {
                num = CustomGameOptions.GlitchCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Glitch));
                    num--;
                }

                LogSomething("Glitch Done");
            }

            if (CustomGameOptions.MurdererOn > 0)
            {
                num = CustomGameOptions.MurdCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Murderer));
                    num--;
                }

                LogSomething("Murderer Done");
            }

            if (CustomGameOptions.CryomaniacOn > 0)
            {
                num = CustomGameOptions.CryomaniacCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Cryomaniac));
                    num--;
                }

                LogSomething("Cryomaniac Done");
            }

            if (CustomGameOptions.WerewolfOn > 0)
            {
                num = CustomGameOptions.WerewolfCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Werewolf));
                    num--;
                }

                LogSomething("Werewolf Done");
            }

            if (CustomGameOptions.ArsonistOn > 0)
            {
                num = CustomGameOptions.ArsonistCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Arsonist));
                    num--;
                }

                LogSomething("Arsonist Done");
            }

            if (CustomGameOptions.JackalOn > 0 && GameData.Instance.PlayerCount > 5)
            {
                num = CustomGameOptions.JackalCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(RoleEnum.Jackal));
                    num--;
                }

                LogSomething("Jackal Done");
            }

            if (CustomGameOptions.NecromancerOn > 0)
            {
                num = CustomGameOptions.NecromancerCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(RoleEnum.Necromancer));
                    num--;
                }

                LogSomething("Necromancer Done");
            }

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                num = CustomGameOptions.PlaguebearerCount;

                while (num > 0)
                {
                    NeutralHarbingerRoles.Add(GenerateRoleSpawnItem(CustomGameOptions.PestSpawn ? RoleEnum.Pestilence : RoleEnum.Plaguebearer));
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
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.SerialKiller));
                    num--;
                }

                LogSomething("Serial Killer Done");
            }

            if (CustomGameOptions.JuggernautOn > 0)
            {
                num = CustomGameOptions.JuggernautCount;

                while (num > 0)
                {
                    NeutralKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Juggernaut));
                    num--;
                }

                LogSomething("Juggeraut Done");
            }

            if (CustomGameOptions.CannibalOn > 0)
            {
                num = CustomGameOptions.CannibalCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Cannibal));
                    num--;
                }

                LogSomething("Cannibal Done");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                num = CustomGameOptions.GuesserCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Guesser));
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
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Actor));
                    num--;
                }

                LogSomething("Actor Done");
            }

            if (CustomGameOptions.ThiefOn > 0)
            {
                num = CustomGameOptions.ThiefCount;

                while (num > 0)
                {
                    NeutralBenignRoles.Add(GenerateRoleSpawnItem(RoleEnum.Thief));
                    num--;
                }

                LogSomething("Thief Done");
            }

            if (CustomGameOptions.DraculaOn > 0)
            {
                num = CustomGameOptions.DraculaCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(RoleEnum.Dracula));
                    num--;
                }

                LogSomething("Dracula Done");
            }

            if (CustomGameOptions.WhispererOn > 0)
            {
                num = CustomGameOptions.WhispererCount;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add(GenerateRoleSpawnItem(RoleEnum.Whisperer));
                    num--;
                }

                LogSomething("Whisperer Done");
            }

            if (CustomGameOptions.TrollOn > 0)
            {
                num = CustomGameOptions.TrollCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.Troll));
                    num--;
                }

                LogSomething("Troll Done");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                num = CustomGameOptions.BHCount;

                while (num > 0)
                {
                    NeutralEvilRoles.Add(GenerateRoleSpawnItem(RoleEnum.BountyHunter));
                    num--;
                }

                LogSomething("Bounty Hunter Done");
            }

            if (CustomGameOptions.MorphlingOn > 0)
            {
                num = CustomGameOptions.MorphlingCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Morphling));
                    num--;
                }

                LogSomething("Morphling Done");
            }

            if (CustomGameOptions.BlackmailerOn > 0)
            {
                num = CustomGameOptions.BlackmailerCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Blackmailer));
                    num--;
                }

                LogSomething("Blackmailer Done");
            }

            if (CustomGameOptions.MinerOn > 0)
            {
                num = CustomGameOptions.MinerCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Miner));
                    num--;
                }

                LogSomething("Miner Done");
            }

            if (CustomGameOptions.TeleporterOn > 0)
            {
                num = CustomGameOptions.TeleporterCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Teleporter));
                    num--;
                }

                LogSomething("Teleporter Done");
            }

            if (CustomGameOptions.AmbusherOn > 0)
            {
                num = CustomGameOptions.AmbusherCount;

                while (num > 0)
                {
                    IntruderKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Ambusher));
                    num--;
                }

                LogSomething("Ambusher Done");
            }

            if (CustomGameOptions.WraithOn > 0)
            {
                num = CustomGameOptions.WraithCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Wraith));
                    num--;
                }

                LogSomething("Wraith Done");
            }

            if (CustomGameOptions.ConsortOn > 0)
            {
                num = CustomGameOptions.ConsortCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Consort));
                    num--;
                }

                LogSomething("Consort Done");
            }

            if (CustomGameOptions.JanitorOn > 0)
            {
                num = CustomGameOptions.JanitorCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Janitor));
                    num--;
                }

                LogSomething("Janitor Done");
            }

            if (CustomGameOptions.CamouflagerOn > 0)
            {
                num = CustomGameOptions.CamouflagerCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Camouflager));
                    num--;
                }

                LogSomething("Camouflager Done");
            }

            if (CustomGameOptions.GrenadierOn > 0)
            {
                num = CustomGameOptions.GrenadierCount;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Grenadier));
                    num--;
                }

                LogSomething("Grenadier Done");
            }

            if (CustomGameOptions.ImpostorOn > 0 && IsCustom)
            {
                num = CustomGameOptions.ImpCount;

                while (num > 0)
                {
                    IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Impostor));
                    num--;
                }

                LogSomething("Impostor Done");
            }

            if (CustomGameOptions.ConsigliereOn > 0)
            {
                num = CustomGameOptions.ConsigliereCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Consigliere));
                    num--;
                }

                LogSomething("Consigliere Done");
            }

            if (CustomGameOptions.DisguiserOn > 0)
            {
                num = CustomGameOptions.DisguiserCount;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Disguiser));
                    num--;
                }

                LogSomething("Disguiser Done");
            }

            if (CustomGameOptions.EnforcerOn > 0)
            {
                num = CustomGameOptions.EnforcerCount;

                while (num > 0)
                {
                    IntruderKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Enforcer));
                    num--;
                }

                LogSomething("Enforcer Done");
            }

            if (CustomGameOptions.GodfatherOn > 0 && imps >= 3)
            {
                num = CustomGameOptions.GodfatherCount;

                while (num > 0)
                {
                    IntruderSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Godfather));
                    num--;
                }

                LogSomething("Godfather Done");
            }

            if (CustomGameOptions.AnarchistOn > 0 && IsCustom)
            {
                num = CustomGameOptions.AnarchistCount;

                while (num > 0)
                {
                    SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Anarchist));
                    num--;
                }

                LogSomething("Anarchist Done");
            }

            if (CustomGameOptions.ShapeshifterOn > 0)
            {
                num = CustomGameOptions.ShapeshifterCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Shapeshifter));
                    num--;
                }

                LogSomething("Shapeshifter Done");
            }

            if (CustomGameOptions.FramerOn > 0)
            {
                num = CustomGameOptions.FramerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Framer));
                    num--;
                }

                LogSomething("Framer Done");
            }

            if (CustomGameOptions.CrusaderOn > 0)
            {
                num = CustomGameOptions.CrusaderCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crusader));
                    num--;
                }

                LogSomething("Crusader Done");
            }

            if (CustomGameOptions.RebelOn > 0 && syn >= 3)
            {
                num = CustomGameOptions.RebelCount;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Rebel));
                    num--;
                }

                LogSomething("Rebel Done");
            }

            if (CustomGameOptions.PoisonerOn > 0)
            {
                num = CustomGameOptions.PoisonerCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Poisoner));
                    num--;
                }

                LogSomething("Poisoner Done");
            }

            if (CustomGameOptions.ColliderOn > 0)
            {
                num = CustomGameOptions.ColliderCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Collider));
                    num--;
                }

                LogSomething("Collider Done");
            }

            if (CustomGameOptions.ConcealerOn > 0)
            {
                num = CustomGameOptions.ConcealerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Concealer));
                    num--;
                }

                LogSomething("Concealer Done");
            }

            if (CustomGameOptions.WarperOn > 0 && (int)CustomGameOptions.Map is not 3 and not 4)
            {
                num = CustomGameOptions.WarperCount;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Warper));
                    num--;
                }

                LogSomething("Warper Done");
            }

            if (CustomGameOptions.BomberOn > 0)
            {
                num = CustomGameOptions.BomberCount;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add(GenerateRoleSpawnItem(RoleEnum.Bomber));
                    num--;
                }

                LogSomething("Bomber Done");
            }

            if (CustomGameOptions.SpellslingerOn > 0)
            {
                num = CustomGameOptions.SpellslingerCount;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add(GenerateRoleSpawnItem(RoleEnum.Spellslinger));
                    num--;
                }

                LogSomething("Spellslinger Done");
            }

            if (CustomGameOptions.StalkerOn > 0)
            {
                num = CustomGameOptions.StalkerCount;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add(GenerateRoleSpawnItem(RoleEnum.Stalker));
                    num--;
                }

                LogSomething("Stalker Done");
            }

            if (CustomGameOptions.DrunkardOn > 0)
            {
                num = CustomGameOptions.DrunkardCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Drunkard));
                    num--;
                }

                LogSomething("Drunkard Done");
            }

            if (CustomGameOptions.TimeKeeperOn > 0)
            {
                num = CustomGameOptions.TimeKeeperCount;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add(GenerateRoleSpawnItem(RoleEnum.TimeKeeper));
                    num--;
                }

                LogSomething("Time Keeper Done");
            }

            if (CustomGameOptions.SilencerOn > 0)
            {
                num = CustomGameOptions.SilencerCount;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add(GenerateRoleSpawnItem(RoleEnum.Silencer));
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

                    IntruderRoles.Sort(maxInt, minInt);

                    while (IntruderRoles.Count < imps)
                        IntruderRoles.Add(GenerateRoleSpawnItem(RoleEnum.Impostor));

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

                    SyndicateRoles.Sort(maxSyn, minSyn);

                    while (SyndicateRoles.Count < syn)
                        SyndicateRoles.Add(GenerateRoleSpawnItem(RoleEnum.Anarchist));

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

                    NeutralRoles.Sort(maxNeut, minNeut);
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

                    CrewRoles.Sort(maxCrew, minCrew);

                    while (CrewRoles.Count < crew)
                        CrewRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crewmate));

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
                spawnList.Add(GenerateRoleSpawnItem(RoleEnum.Crewmate));

            if (!spawnList.Any(x => CrewRoles.Contains(x) || x.ID == RoleEnum.Crewmate))
            {
                spawnList.Remove(spawnList.Random());
                spawnList.Add(CrewRoles.Count > 0 ? CrewRoles.Random() : GenerateRoleSpawnItem(RoleEnum.Crewmate));

                if (TownOfUsReworked.IsTest)
                    LogSomething("Added Solo Crew");
            }

            if (!spawnList.Any(x => x.ID == RoleEnum.Dracula) && spawnList.Any(x => x.ID == RoleEnum.VampireHunter))
            {
                var count = spawnList.RemoveAll(x => x.ID == RoleEnum.VampireHunter);

                while (count > 0)
                {
                    spawnList.Add(GenerateRoleSpawnItem(RoleEnum.Vigilante));
                    count--;
                }
            }

            spawnList.Shuffle();
            LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

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
                var random = RoleEnum.None;

                while (cachedCount == AllRoles.Count)
                {
                    ratelimit++;

                    if (id == RoleEnum.CrewAudit)
                        random = CA.Random();
                    else if (id == RoleEnum.CrewInvest)
                        random = CI.Random();
                    else if (id == RoleEnum.CrewSov)
                        random = CSv.Random();
                    else if (id == RoleEnum.CrewProt)
                        random = CP.Random();
                    else if (id == RoleEnum.CrewKill)
                        random = CK.Random();
                    else if (id == RoleEnum.CrewSupport)
                        random = CS.Random();
                    else if (id == RoleEnum.NeutralBen)
                        random = NB.Random();
                    else if (id == RoleEnum.NeutralEvil)
                        random = NE.Random();
                    else if (id == RoleEnum.NeutralNeo)
                        random = NN.Random();
                    else if (id == RoleEnum.NeutralHarb)
                        random = NH.Random();
                    else if (id == RoleEnum.NeutralApoc)
                        random = NA.Random();
                    else if (id == RoleEnum.NeutralKill)
                        random = NK.Random();
                    else if (id == RoleEnum.IntruderConceal)
                        random = IC.Random();
                    else if (id == RoleEnum.IntruderDecep)
                        random = ID.Random();
                    else if (id == RoleEnum.IntruderKill)
                        random = IK.Random();
                    else if (id == RoleEnum.IntruderSupport)
                        random = IS.Random();
                    else if (id == RoleEnum.SyndicateSupport)
                        random = SSu.Random();
                    else if (id == RoleEnum.SyndicatePower)
                        random = SP.Random();
                    else if (id == RoleEnum.SyndicateDisrup)
                        random = SD.Random();
                    else if (id == RoleEnum.SyndicateKill)
                        random = SyK.Random();

                    if (!AllRoles.Any(x => x.ID == random && x.Unique) && !bans.Any(x => x.Get() == random) && random != RoleEnum.None)
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
                var random = RoleEnum.None;

                while (cachedCount == AllRoles.Count)
                {
                    ratelimit++;

                    if (id == RoleEnum.RandomCrew)
                        random = Crew.Random().Random();
                    else if (id == RoleEnum.RandomNeutral)
                        random = Neutral.Random().Random();
                    else if (id == RoleEnum.RandomIntruder)
                        random = Intruders.Random().Random();
                    else if (id == RoleEnum.RandomSyndicate)
                        random = Syndicate.Random().Random();

                    if (!AllRoles.Any(x => x.ID == random && x.Unique) && !bans.Any(x => x.Get() == random) && random != RoleEnum.None)
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
                AllRoles.Add(GenerateRoleSpawnItem(RoleEnum.Crewmate));

            var spawnList = AllRoles;
            spawnList.Shuffle();

            LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

                LogSomething("Roles in the game: " + ids);
            }

            while (players.Count > 0 && spawnList.Count > 0)
                Gen(players.TakeFirst(), (int)spawnList.TakeFirst().ID, PlayerLayerEnum.Role);

            LogSomething("Role Spawn Done");
        }

        private static (int Chance, RoleEnum ID, bool Unique) GenerateRoleSpawnItem(RoleEnum id)
        {
            var things = id switch
            {
                RoleEnum.Mayor => (CustomGameOptions.MayorOn, CustomGameOptions.UniqueMayor),
                RoleEnum.Sheriff => (CustomGameOptions.SheriffOn, CustomGameOptions.UniqueSheriff),
                RoleEnum.Inspector => (CustomGameOptions.InspectorOn, CustomGameOptions.UniqueInspector),
                RoleEnum.Vigilante => (IsKilling ? 100 : CustomGameOptions.VigilanteOn, CustomGameOptions.UniqueVigilante),
                RoleEnum.Engineer => (CustomGameOptions.EngineerOn, CustomGameOptions.UniqueEngineer),
                RoleEnum.Monarch => (CustomGameOptions.MonarchOn, CustomGameOptions.UniqueMonarch),
                RoleEnum.Dictator => (CustomGameOptions.DictatorOn, CustomGameOptions.UniqueDictator),
                RoleEnum.Collider => (CustomGameOptions.ColliderOn, CustomGameOptions.UniqueCollider),
                RoleEnum.Medic => (CustomGameOptions.MedicOn, CustomGameOptions.UniqueMedic),
                RoleEnum.Stalker => (CustomGameOptions.StalkerOn, CustomGameOptions.UniqueStalker),
                RoleEnum.Altruist => (CustomGameOptions.AltruistOn, CustomGameOptions.UniqueAltruist),
                RoleEnum.Veteran => (IsKilling ? 100 : CustomGameOptions.VeteranOn, CustomGameOptions.UniqueVeteran),
                RoleEnum.Tracker => (CustomGameOptions.TrackerOn, CustomGameOptions.UniqueTracker),
                RoleEnum.Transporter => (CustomGameOptions.TransporterOn, CustomGameOptions.UniqueTransporter),
                RoleEnum.Medium => (CustomGameOptions.MediumOn, CustomGameOptions.UniqueMedium),
                RoleEnum.Coroner => (CustomGameOptions.CoronerOn, CustomGameOptions.UniqueCoroner),
                RoleEnum.Operative => (CustomGameOptions.OperativeOn, CustomGameOptions.UniqueOperative),
                RoleEnum.Detective => (CustomGameOptions.DetectiveOn, CustomGameOptions.UniqueDetective),
                RoleEnum.Escort => (CustomGameOptions.EscortOn, CustomGameOptions.UniqueDetective),
                RoleEnum.Shifter => (CustomGameOptions.ShifterOn, CustomGameOptions.UniqueShifter),
                RoleEnum.Crewmate => (CustomGameOptions.CrewmateOn, false),
                RoleEnum.VampireHunter => (CustomGameOptions.VampireHunterOn, CustomGameOptions.UniqueVampireHunter),
                RoleEnum.Jester => (CustomGameOptions.JesterOn, CustomGameOptions.UniqueJester),
                RoleEnum.Amnesiac => (CustomGameOptions.AmnesiacOn, CustomGameOptions.UniqueAmnesiac),
                RoleEnum.Executioner => (CustomGameOptions.ExecutionerOn, CustomGameOptions.UniqueExecutioner),
                RoleEnum.Survivor => (CustomGameOptions.SurvivorOn, CustomGameOptions.UniqueSurvivor),
                RoleEnum.GuardianAngel => (CustomGameOptions.GuardianAngelOn, CustomGameOptions.UniqueGuardianAngel),
                RoleEnum.Glitch => (CustomGameOptions.GlitchOn, CustomGameOptions.UniqueGlitch),
                RoleEnum.Murderer => (IsKilling ? 5 : CustomGameOptions.MurdererOn, CustomGameOptions.UniqueMurderer),
                RoleEnum.Cryomaniac => (CustomGameOptions.CryomaniacOn, CustomGameOptions.UniqueCryomaniac),
                RoleEnum.Werewolf => (CustomGameOptions.WerewolfOn, CustomGameOptions.UniqueWerewolf),
                RoleEnum.Arsonist => (CustomGameOptions.ArsonistOn, CustomGameOptions.UniqueArsonist),
                RoleEnum.Jackal => (CustomGameOptions.JackalOn, CustomGameOptions.UniqueJackal),
                RoleEnum.Plaguebearer => (CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer),
                RoleEnum.Pestilence => (CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer),
                RoleEnum.SerialKiller => (CustomGameOptions.SerialKillerOn, CustomGameOptions.UniqueSerialKiller),
                RoleEnum.Juggernaut => (CustomGameOptions.JuggernautOn, CustomGameOptions.UniqueJuggernaut),
                RoleEnum.Cannibal => (CustomGameOptions.CannibalOn, CustomGameOptions.UniqueCannibal),
                RoleEnum.Thief => (CustomGameOptions.ThiefOn, CustomGameOptions.UniqueThief),
                RoleEnum.Dracula => (CustomGameOptions.DraculaOn, CustomGameOptions.UniqueDracula),
                RoleEnum.Troll => (CustomGameOptions.TrollOn, CustomGameOptions.UniqueTroll),
                RoleEnum.Enforcer => (CustomGameOptions.EnforcerOn, CustomGameOptions.UniqueEnforcer),
                RoleEnum.Morphling => (CustomGameOptions.MorphlingOn, CustomGameOptions.UniqueMorphling),
                RoleEnum.Blackmailer => (CustomGameOptions.BlackmailerOn, CustomGameOptions.UniqueBlackmailer),
                RoleEnum.Miner => (CustomGameOptions.MinerOn, CustomGameOptions.UniqueMiner),
                RoleEnum.Teleporter => (CustomGameOptions.TeleporterOn, CustomGameOptions.UniqueTeleporter),
                RoleEnum.Wraith => (CustomGameOptions.WraithOn, CustomGameOptions.UniqueWraith),
                RoleEnum.Consort => (CustomGameOptions.ConsortOn, CustomGameOptions.UniqueConsort),
                RoleEnum.Janitor => (CustomGameOptions.JanitorOn, CustomGameOptions.UniqueJanitor),
                RoleEnum.Camouflager => (CustomGameOptions.CamouflagerOn, CustomGameOptions.UniqueCamouflager),
                RoleEnum.Grenadier => (CustomGameOptions.GrenadierOn, CustomGameOptions.UniqueGrenadier),
                RoleEnum.Poisoner => (CustomGameOptions.PoisonerOn, CustomGameOptions.UniquePoisoner),
                RoleEnum.Impostor => (IsKilling ? 5 : CustomGameOptions.ImpostorOn, false),
                RoleEnum.Consigliere => (CustomGameOptions.ConsigliereOn, CustomGameOptions.UniqueConsigliere),
                RoleEnum.Disguiser => (CustomGameOptions.DisguiserOn, CustomGameOptions.UniqueDisguiser),
                RoleEnum.Spellslinger => (CustomGameOptions.SpellslingerOn, CustomGameOptions.UniqueSpellslinger),
                RoleEnum.Godfather => (CustomGameOptions.GodfatherOn, CustomGameOptions.UniqueGodfather),
                RoleEnum.Anarchist => (IsKilling ? 5 : CustomGameOptions.AnarchistOn, false),
                RoleEnum.Shapeshifter => (CustomGameOptions.ShapeshifterOn, CustomGameOptions.UniqueShapeshifter),
                RoleEnum.Drunkard => (CustomGameOptions.DrunkardOn, CustomGameOptions.UniqueDrunk),
                RoleEnum.Framer => (CustomGameOptions.FramerOn, CustomGameOptions.UniqueFramer),
                RoleEnum.Rebel => (CustomGameOptions.RebelOn, CustomGameOptions.UniqueRebel),
                RoleEnum.Concealer => (CustomGameOptions.ConcealerOn, CustomGameOptions.UniqueConcealer),
                RoleEnum.Warper => (CustomGameOptions.WarperOn, CustomGameOptions.UniqueConcealer),
                RoleEnum.Bomber => (CustomGameOptions.BomberOn, CustomGameOptions.UniqueBomber),
                RoleEnum.Chameleon => (CustomGameOptions.ChameleonOn, CustomGameOptions.UniqueChameleon),
                RoleEnum.Guesser => (CustomGameOptions.GuesserOn, CustomGameOptions.UniqueGuesser),
                RoleEnum.Whisperer => (CustomGameOptions.WhispererOn, CustomGameOptions.UniqueWhisperer),
                RoleEnum.Retributionist => (CustomGameOptions.RetributionistOn, CustomGameOptions.UniqueRetributionist),
                RoleEnum.Actor => (CustomGameOptions.ActorOn, CustomGameOptions.UniqueActor),
                RoleEnum.BountyHunter => (CustomGameOptions.BountyHunterOn, CustomGameOptions.UniqueBountyHunter),
                RoleEnum.Mystic => (CustomGameOptions.MysticOn, CustomGameOptions.UniqueMystic),
                RoleEnum.Seer => (CustomGameOptions.SeerOn, CustomGameOptions.UniqueSeer),
                RoleEnum.Necromancer => (CustomGameOptions.NecromancerOn, CustomGameOptions.UniqueNecromancer),
                RoleEnum.TimeKeeper => (CustomGameOptions.TimeKeeperOn, CustomGameOptions.UniqueTimeKeeper),
                RoleEnum.Ambusher => (CustomGameOptions.AmbusherOn, CustomGameOptions.UniqueAmbusher),
                RoleEnum.Crusader => (CustomGameOptions.CrusaderOn, CustomGameOptions.UniqueCrusader),
                RoleEnum.Silencer => (CustomGameOptions.SilencerOn, CustomGameOptions.UniqueSilencer),
                _ => throw new NotImplementedException()
            };

            return (things.Item1, id, things.Item2);
        }

        private static (int Chance, AbilityEnum ID, bool Unique) GenerateAbilitySpawnItem(AbilityEnum id)
        {
            var things = id switch
            {
                AbilityEnum.CrewAssassin => (CustomGameOptions.CrewAssassinOn, CustomGameOptions.UniqueCrewAssassin),
                AbilityEnum.IntruderAssassin => (CustomGameOptions.IntruderAssassinOn, CustomGameOptions.UniqueIntruderAssassin),
                AbilityEnum.SyndicateAssassin => (CustomGameOptions.SyndicateAssassinOn, CustomGameOptions.UniqueSyndicateAssassin),
                AbilityEnum.NeutralAssassin => (CustomGameOptions.NeutralAssassinOn, CustomGameOptions.UniqueNeutralAssassin),
                AbilityEnum.ButtonBarry => (CustomGameOptions.ButtonBarryOn, CustomGameOptions.UniqueCrewAssassin),
                AbilityEnum.Insider => (CustomGameOptions.InsiderOn, CustomGameOptions.UniqueButtonBarry),
                AbilityEnum.Multitasker => (CustomGameOptions.MultitaskerOn, CustomGameOptions.UniqueMultitasker),
                AbilityEnum.Ninja => (CustomGameOptions.NinjaOn, CustomGameOptions.UniqueNinja),
                AbilityEnum.Politician => (CustomGameOptions.PoliticianOn, CustomGameOptions.UniquePolitician),
                AbilityEnum.Radar => (CustomGameOptions.RadarOn, CustomGameOptions.UniqueRadar),
                AbilityEnum.Ruthless => (CustomGameOptions.RuthlessOn, CustomGameOptions.UniqueRuthless),
                AbilityEnum.Snitch => (CustomGameOptions.SnitchOn, CustomGameOptions.UniqueSnitch),
                AbilityEnum.Swapper => (CustomGameOptions.SwapperOn, CustomGameOptions.UniqueSwapper),
                AbilityEnum.Tiebreaker => (CustomGameOptions.TiebreakerOn, CustomGameOptions.UniqueTiebreaker),
                AbilityEnum.Torch => (CustomGameOptions.TorchOn, CustomGameOptions.UniqueTorch),
                AbilityEnum.Tunneler => (CustomGameOptions.TunnelerOn, CustomGameOptions.UniqueTunneler),
                AbilityEnum.Underdog => (CustomGameOptions.UnderdogOn, CustomGameOptions.UniqueUnderdog),
                _ => throw new NotImplementedException()
            };

            return (things.Item1, id, things.Item2);
        }

        private static (int Chance, ModifierEnum ID, bool Unique) GenerateModifierSpawnItem(ModifierEnum id)
        {
            var things = id switch
            {
                ModifierEnum.Astral => (CustomGameOptions.AstralOn, CustomGameOptions.UniqueAstral),
                ModifierEnum.Bait => (CustomGameOptions.BaitOn, CustomGameOptions.UniqueBait),
                ModifierEnum.Coward => (CustomGameOptions.CowardOn, CustomGameOptions.UniqueCoward),
                ModifierEnum.Diseased => (CustomGameOptions.DiseasedOn, CustomGameOptions.UniqueDiseased),
                ModifierEnum.Drunk => (CustomGameOptions.DrunkOn, CustomGameOptions.UniqueDrunk),
                ModifierEnum.Dwarf => (CustomGameOptions.DwarfOn, CustomGameOptions.UniqueDwarf),
                ModifierEnum.Flincher => (CustomGameOptions.FlincherOn, CustomGameOptions.UniqueFlincher),
                ModifierEnum.Giant => (CustomGameOptions.GiantOn, CustomGameOptions.UniqueGiant),
                ModifierEnum.Indomitable => (CustomGameOptions.IndomitableOn, CustomGameOptions.UniqueIndomitable),
                ModifierEnum.Professional => (CustomGameOptions.ProfessionalOn, CustomGameOptions.UniqueProfessional),
                ModifierEnum.Shy => (CustomGameOptions.ShyOn, CustomGameOptions.UniqueShy),
                ModifierEnum.VIP => (CustomGameOptions.VIPOn, CustomGameOptions.UniqueVIP),
                ModifierEnum.Volatile => (CustomGameOptions.VolatileOn, CustomGameOptions.UniqueVolatile),
                ModifierEnum.Yeller => (CustomGameOptions.YellerOn, CustomGameOptions.UniqueYeller),
                _ => throw new NotImplementedException(),
            };

            return (things.Item1, id, things.Item2);
        }

        private static (int Chance, ObjectifierEnum ID, bool Unique) GenerateObjectifierSpawnItem(ObjectifierEnum id)
        {
            var things = id switch
            {
                ObjectifierEnum.Allied => (CustomGameOptions.AlliedOn, CustomGameOptions.UniqueAllied),
                ObjectifierEnum.Corrupted => (CustomGameOptions.CorruptedOn, CustomGameOptions.UniqueCorrupted),
                ObjectifierEnum.Defector => (CustomGameOptions.DefectorOn, CustomGameOptions.UniqueDefector),
                ObjectifierEnum.Fanatic => (CustomGameOptions.FanaticOn, CustomGameOptions.UniqueFanatic),
                ObjectifierEnum.Linked => (CustomGameOptions.LinkedOn, CustomGameOptions.UniqueLinked),
                ObjectifierEnum.Lovers => (CustomGameOptions.LoversOn, CustomGameOptions.UniqueLovers),
                ObjectifierEnum.Mafia => (CustomGameOptions.MafiaOn, CustomGameOptions.UniqueMafia),
                ObjectifierEnum.Overlord => (CustomGameOptions.OverlordOn, CustomGameOptions.UniqueOverlord),
                ObjectifierEnum.Rivals => (CustomGameOptions.RivalsOn, CustomGameOptions.UniqueRivals),
                ObjectifierEnum.Taskmaster => (CustomGameOptions.TaskmasterOn, CustomGameOptions.UniqueTaskmaster),
                ObjectifierEnum.Traitor => (CustomGameOptions.TraitorOn, CustomGameOptions.UniqueTraitor),
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
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.CrewAssassin));
                    num--;
                }

                LogSomething("Crew Assassin Done");
            }

            if (CustomGameOptions.SyndicateAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfSyndicateAssassins;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.SyndicateAssassin));
                    num--;
                }

                LogSomething("Syndicate Assassin Done");
            }

            if (CustomGameOptions.IntruderAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfIntruderAssassins;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.IntruderAssassin));
                    num--;
                }

                LogSomething("Intruder Assassin Done");
            }

            if (CustomGameOptions.NeutralAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfNeutralAssassins;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.NeutralAssassin));
                    num--;
                }

                LogSomething("Neutral Assassin Done");
            }

            if (CustomGameOptions.RuthlessOn > 0)
            {
                num = CustomGameOptions.RuthlessCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Ruthless));
                    num--;
                }

                LogSomething("Ruthless Done");
            }

            if (CustomGameOptions.SnitchOn > 0)
            {
                num = CustomGameOptions.SnitchCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Snitch));
                    num--;
                }

                LogSomething("Snitch Done");
            }

            if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
            {
                num = CustomGameOptions.InsiderCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Insider));
                    num--;
                }

                LogSomething("Insider Done");
            }

            if (CustomGameOptions.MultitaskerOn > 0)
            {
                num = CustomGameOptions.MultitaskerCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Multitasker));
                    num--;
                }

                LogSomething("Multitasker Done");
            }

            if (CustomGameOptions.RadarOn > 0)
            {
                num = CustomGameOptions.RadarCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Radar));
                    num--;
                }

                LogSomething("Radar Done");
            }

            if (CustomGameOptions.TiebreakerOn > 0)
            {
                num = CustomGameOptions.TiebreakerCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Tiebreaker));
                    num--;
                }

                LogSomething("Tiebreaker Done");
            }

            if (CustomGameOptions.TorchOn > 0)
            {
                num = CustomGameOptions.TorchCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Torch));
                    num--;
                }

                LogSomething("Torch Done");
            }

            if (CustomGameOptions.UnderdogOn > 0)
            {
                num = CustomGameOptions.UnderdogCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Underdog));
                    num--;
                }

                LogSomething("Underdog Done");
            }

            if (CustomGameOptions.TunnelerOn > 0 && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
            {
                num = CustomGameOptions.TunnelerCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Tunneler));
                    num--;
                }

                LogSomething("Tunneler Done");
            }

            if (CustomGameOptions.NinjaOn > 0)
            {
                num = CustomGameOptions.NinjaCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Ninja));
                    num--;
                }

                LogSomething("Ninja Done");
            }

            if (CustomGameOptions.ButtonBarryOn > 0)
            {
                num = CustomGameOptions.ButtonBarryCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.ButtonBarry));
                    num--;
                }

                LogSomething("Button Barry Done");
            }

            if (CustomGameOptions.PoliticianOn > 0)
            {
                num = CustomGameOptions.PoliticianCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Politician));
                    num--;
                }

                LogSomething("Politician Done");
            }

            if (CustomGameOptions.SwapperOn > 0)
            {
                num = CustomGameOptions.SwapperCount;

                while (num > 0)
                {
                    AllAbilities.Add(GenerateAbilitySpawnItem(AbilityEnum.Swapper));
                    num--;
                }

                LogSomething("Swapper Done");
            }

            var allCount = GameData.Instance.PlayerCount;
            AllAbilities.Sort(CustomGameOptions.MaxAbilities, CustomGameOptions.MinAbilities);

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

            canHaveIntruderAbility.RemoveAll(x => !x.Is(Faction.Intruder) || (x.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
            canHaveIntruderAbility.Shuffle();

            canHaveNeutralAbility.RemoveAll(x => !(x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralHarb)));
            canHaveNeutralAbility.Shuffle();

            canHaveCrewAbility.RemoveAll(x => !x.Is(Faction.Crew));
            canHaveCrewAbility.Shuffle();

            canHaveSyndicateAbility.RemoveAll(x => !x.Is(Faction.Syndicate));
            canHaveSyndicateAbility.Shuffle();

            canHaveTunnelerAbility.RemoveAll(x => !x.Is(Faction.Crew) || x.Is(RoleEnum.Engineer));
            canHaveTunnelerAbility.Shuffle();

            canHaveSnitch.RemoveAll(x => !x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Traitor));
            canHaveSnitch.Shuffle();

            canHaveTaskedAbility.RemoveAll(x => !x.CanDoTasks());
            canHaveTaskedAbility.Shuffle();

            canHaveTorch.RemoveAll(x => (x.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NKHasImpVision) || x.Is(Faction.Syndicate) || (x.Is(Faction.Neutral) &&
                !CustomGameOptions.LightsAffectNeutrals) || x.Is(Faction.Intruder));
            canHaveTorch.Shuffle();

            canHaveEvilAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)));
            canHaveEvilAbility.Shuffle();

            canHaveKillingAbility.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) ||
                x.Is(RoleAlignment.CrewKill) || x.Is(ObjectifierEnum.Corrupted)));
            canHaveKillingAbility.Shuffle();

            canHaveRuthless.RemoveAll(x => !(x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) ||
                x.Is(RoleAlignment.CrewKill) || x.Is(ObjectifierEnum.Corrupted)) || x.Is(RoleEnum.Juggernaut));
            canHaveRuthless.Shuffle();

            canHaveBB.RemoveAll(x => (x.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (x.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(RoleEnum.Guesser) && !CustomGameOptions.GuesserButton) || (x.Is(RoleEnum.Executioner) &&
                !CustomGameOptions.ExecutionerButton) || (!CustomGameOptions.MonarchButton && x.Is(RoleEnum.Monarch)) || (!CustomGameOptions.DictatorButton &&
                x.Is(RoleEnum.Dictator)));
            canHaveBB.Shuffle();

            canHavePolitician.RemoveAll(x => x.Is(RoleAlignment.NeutralEvil) || x.Is(RoleAlignment.NeutralBen) || x.Is(RoleAlignment.NeutralNeo));
            canHavePolitician.Shuffle();

            AllAbilities.Sort(allCount);
            var spawnList = AllAbilities;
            spawnList.Shuffle();

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

                LogSomething("Abilities in the game: " + ids);
            }

            while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
                canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
                canHaveTaskedAbility.Count > 0 || canHaveTorch.Count > 0 || canHaveKillingAbility.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var id = spawnList.TakeFirst().ID;
                AbilityEnum[] Snitch = { AbilityEnum.Snitch };
                AbilityEnum[] Syndicate = { AbilityEnum.SyndicateAssassin };
                AbilityEnum[] Crew = { AbilityEnum.CrewAssassin, AbilityEnum.Swapper };
                AbilityEnum[] Neutral = { AbilityEnum.NeutralAssassin };
                AbilityEnum[] Intruder = { AbilityEnum.IntruderAssassin };
                AbilityEnum[] Killing = { AbilityEnum.Ninja };
                AbilityEnum[] Torch = { AbilityEnum.Torch };
                AbilityEnum[] Evil = { AbilityEnum.Underdog };
                AbilityEnum[] Tasked = { AbilityEnum.Insider, AbilityEnum.Multitasker };
                AbilityEnum[] Global = { AbilityEnum.Radar, AbilityEnum.Tiebreaker };
                AbilityEnum[] Tunneler = { AbilityEnum.Tunneler };
                AbilityEnum[] BB = { AbilityEnum.ButtonBarry };
                AbilityEnum[] Pol = { AbilityEnum.Politician };
                AbilityEnum[] Ruth = { AbilityEnum.Ruthless };

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
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Lovers));
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Lovers));
                    num--;
                }

                LogSomething("Lovers Done");
            }

            if (CustomGameOptions.RivalsOn > 0 && GameData.Instance.PlayerCount > 3)
            {
                num = CustomGameOptions.RivalsCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Rivals));
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Rivals));
                    num--;
                }

                LogSomething("Rivals Done");
            }

            if (CustomGameOptions.FanaticOn > 0)
            {
                num = CustomGameOptions.FanaticCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Fanatic));
                    num--;
                }

                LogSomething("Fanatic Done");
            }

            if (CustomGameOptions.CorruptedOn > 0)
            {
                num = CustomGameOptions.CorruptedCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Corrupted));
                    num--;
                }

                LogSomething("Corrupted Done");
            }

            if (CustomGameOptions.OverlordOn > 0)
            {
                num = CustomGameOptions.OverlordCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Overlord));
                    num--;
                }

                LogSomething("Overlord Done");
            }

            if (CustomGameOptions.AlliedOn > 0)
            {
                num = CustomGameOptions.AlliedCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Allied));
                    num--;
                }

                LogSomething("Allied Done");
            }

            if (CustomGameOptions.TraitorOn > 0)
            {
                num = CustomGameOptions.TraitorCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Traitor));
                    num--;
                }

                LogSomething("Traitor Done");
            }

            if (CustomGameOptions.TaskmasterOn > 0)
            {
                num = CustomGameOptions.TaskmasterCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Taskmaster));
                    num--;
                }

                LogSomething("Taskmaster Done");
            }

            if (CustomGameOptions.MafiaOn > 0)
            {
                num = CustomGameOptions.MafiaCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Mafia));
                    num--;
                }

                LogSomething("Mafia Done");
            }

            if (CustomGameOptions.DefectorOn > 0)
            {
                num = CustomGameOptions.DefectorCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Defector));
                    num--;
                }

                LogSomething("Defector Done");
            }

            if (CustomGameOptions.LinkedOn > 0 && Role.GetRoles(Faction.Neutral).Count > 1 && GameData.Instance.PlayerCount > 3)
            {
                num = CustomGameOptions.LinkedCount;

                while (num > 0)
                {
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Linked));
                    AllObjectifiers.Add(GenerateObjectifierSpawnItem(ObjectifierEnum.Linked));
                    num--;
                }

                LogSomething("Linked Done");
            }

            var allCount = GameData.Instance.PlayerCount;
            AllObjectifiers.Sort(CustomGameOptions.MaxObjectifiers, CustomGameOptions.MinObjectifiers);

            var canHaveLoverorRival = CustomPlayer.AllPlayers;
            var canHaveNeutralObjectifier = CustomPlayer.AllPlayers;
            var canHaveCrewObjectifier = CustomPlayer.AllPlayers;
            var canHaveAllied = CustomPlayer.AllPlayers;
            var canHaveObjectifier = CustomPlayer.AllPlayers;
            var canHaveDefector = CustomPlayer.AllPlayers;

            canHaveLoverorRival.RemoveAll(x => x.Is(RoleEnum.Altruist) || x.Is(RoleEnum.Troll) || x.Is(RoleEnum.Actor) || x.Is(RoleEnum.Jester) || x.Is(RoleEnum.Shifter) || x == PureCrew);
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

            AllObjectifiers.Sort(allCount);
            var spawnList = AllObjectifiers;
            spawnList.Shuffle();

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

                LogSomething("Objectifiers in the game: " + ids);
            }

            while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 1 || canHaveObjectifier.Count > 0 || canHaveDefector.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (_, id, _) = spawnList.TakeFirst();
                ObjectifierEnum[] LoverRival = { ObjectifierEnum.Lovers, ObjectifierEnum.Rivals };
                ObjectifierEnum[] Crew = { ObjectifierEnum.Corrupted, ObjectifierEnum.Fanatic, ObjectifierEnum.Traitor };
                ObjectifierEnum[] Neutral = { ObjectifierEnum.Taskmaster, ObjectifierEnum.Overlord, ObjectifierEnum.Linked };
                ObjectifierEnum[] Allied = { ObjectifierEnum.Allied };
                ObjectifierEnum[] Global = { ObjectifierEnum.Mafia };
                ObjectifierEnum[] Defect = { ObjectifierEnum.Defector };

                PlayerControl assigned = null;

                if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 1)
                    assigned = canHaveLoverorRival.TakeFirst();
                else if (Crew.Contains(id) && canHaveCrewObjectifier.Count > 0)
                    assigned = canHaveCrewObjectifier.TakeFirst();
                else if (Neutral.Contains(id) && canHaveNeutralObjectifier.Count > 0)
                    assigned = canHaveNeutralObjectifier.TakeFirst();
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
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Diseased));
                    num--;
                }

                LogSomething("Diseased Done");
            }

            if (CustomGameOptions.BaitOn > 0)
            {
                num = CustomGameOptions.BaitCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Bait));
                    num--;
                }

                LogSomething("Bait Done");
            }

            if (CustomGameOptions.DwarfOn > 0)
            {
                num = CustomGameOptions.DwarfCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Dwarf));
                    num--;
                }

                LogSomething("Dwarf Done");
            }

            if (CustomGameOptions.VIPOn > 0)
            {
                num = CustomGameOptions.VIPCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.VIP));
                    num--;
                }

                LogSomething("VIP Done");
            }

            if (CustomGameOptions.ShyOn > 0)
            {
                num = CustomGameOptions.ShyCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Shy));
                    num--;
                }

                LogSomething("Shy Done");
            }

            if (CustomGameOptions.GiantOn > 0)
            {
                num = CustomGameOptions.GiantCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Giant));
                    num--;
                }

                LogSomething("Giant Done");
            }

            if (CustomGameOptions.DrunkOn > 0)
            {
                num = CustomGameOptions.DrunkCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Drunk));
                    num--;
                }

                LogSomething("Drunk Done");
            }

            if (CustomGameOptions.FlincherOn > 0)
            {
                num = CustomGameOptions.FlincherCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Flincher));
                    num--;
                }

                LogSomething("Flincher Done");
            }

            if (CustomGameOptions.CowardOn > 0)
            {
                num = CustomGameOptions.CowardCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Coward));
                    num--;
                }

                LogSomething("Coward Done");
            }

            if (CustomGameOptions.VolatileOn > 0)
            {
                num = CustomGameOptions.VolatileCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Volatile));
                    num--;
                }

                LogSomething("Volatile Done");
            }

            if (CustomGameOptions.IndomitableOn > 0)
            {
                num = CustomGameOptions.IndomitableCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Indomitable));
                    num--;
                }

                LogSomething("Indomitable Done");
            }

            if (CustomGameOptions.ProfessionalOn > 0)
            {
                num = CustomGameOptions.ProfessionalCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Professional));
                    num--;
                }

                LogSomething("Professional Done");
            }

            if (CustomGameOptions.AstralOn > 0)
            {
                num = CustomGameOptions.AstralCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Astral));
                    num--;
                }

                LogSomething("Astral Done");
            }

            if (CustomGameOptions.YellerOn > 0)
            {
                num = CustomGameOptions.YellerCount;

                while (num > 0)
                {
                    AllModifiers.Add(GenerateModifierSpawnItem(ModifierEnum.Yeller));
                    num--;
                }

                LogSomething("Yeller Done");
            }

            var allCount = GameData.Instance.PlayerCount;
            AllModifiers.Sort(CustomGameOptions.MaxModifiers, CustomGameOptions.MinModifiers);

            var canHaveBait = CustomPlayer.AllPlayers;
            var canHaveDiseased = CustomPlayer.AllPlayers;
            var canHaveProfessional = CustomPlayer.AllPlayers;
            var canHaveModifier = CustomPlayer.AllPlayers;
            var canHaveShy = CustomPlayer.AllPlayers;

            canHaveBait.RemoveAll(x => x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Shifter) || x.Is(RoleEnum.Thief) || x.Is(RoleEnum.Altruist) || x.Is(RoleEnum.Troll));
            canHaveBait.Shuffle();

            canHaveDiseased.RemoveAll(x => x.Is(RoleEnum.Altruist) || x.Is(RoleEnum.Troll));
            canHaveDiseased.Shuffle();

            canHaveProfessional.RemoveAll(x => !(x.Is(AbilityEnum.CrewAssassin) || x.Is(AbilityEnum.NeutralAssassin) || x.Is(AbilityEnum.IntruderAssassin) ||
                x.Is(AbilityEnum.SyndicateAssassin)));
            canHaveProfessional.Shuffle();

            canHaveShy.RemoveAll(x => (x.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (x.Is(AbilityEnum.Swapper) && !CustomGameOptions.SwapperButton) || (x.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (x.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || x.Is(AbilityEnum.ButtonBarry) ||
                (x.Is(AbilityEnum.Politician) && !CustomGameOptions.PoliticianButton) || (!CustomGameOptions.DictatorButton && x.Is(RoleEnum.Dictator)) ||
                (!CustomGameOptions.MonarchButton && x.Is(RoleEnum.Monarch)));
            canHaveShy.Shuffle();

            AllModifiers.Sort(allCount);
            var spawnList = AllModifiers;
            spawnList.Shuffle();

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $"{id}, ";

                LogSomething("Modifiers in the game: " + ids);
            }

            while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (_, id, _) = spawnList.TakeFirst();
                ModifierEnum[] Bait = { ModifierEnum.Bait };
                ModifierEnum[] Diseased = { ModifierEnum.Diseased };
                ModifierEnum[] Professional = { ModifierEnum.Professional };
                ModifierEnum[] Global = { ModifierEnum.Dwarf, ModifierEnum.VIP, ModifierEnum.Giant, ModifierEnum.Drunk, ModifierEnum.Flincher, ModifierEnum.Coward, ModifierEnum.Volatile,
                    ModifierEnum.Indomitable, ModifierEnum.Astral, ModifierEnum.Yeller };
                ModifierEnum[] Shy = { ModifierEnum.Shy };

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
                foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
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
                    CallRpc(CustomRPC.Target, TargetRPC.SetAlliedFaction, ally.Player, faction);
                }

                LogSomething("Allied Faction Set Done");
            }

            if (CustomGameOptions.LoversOn > 0)
            {
                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (lover.OtherLover)
                        continue;

                    var ratelimit = 0;

                    while (!lover.OtherLover || lover.OtherLover == lover.Player || (lover.Player.GetFaction() == lover.OtherLover.GetFaction() && !CustomGameOptions.LoversFaction) ||
                        !lover.OtherLover.Is(ObjectifierEnum.Lovers) || Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover)
                    {
                        lover.OtherLover = CustomPlayer.AllPlayers.Random();
                        ratelimit++;

                        if (ratelimit > 1000)
                            break;
                    }

                    if (!(!lover.OtherLover || lover.OtherLover == lover.Player || (lover.Player.GetFaction() == lover.OtherLover.GetFaction() && !CustomGameOptions.LoversFaction) ||
                        !lover.OtherLover.Is(ObjectifierEnum.Lovers) || Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover))
                    {
                        Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover = lover.Player;
                        CallRpc(CustomRPC.Target, TargetRPC.SetCouple, lover.Player, lover.OtherLover);

                        if (TownOfUsReworked.IsTest)
                            LogSomething($"Lovers = {lover.Player.name} & {lover.OtherLover?.name}");
                    }
                }

                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (!(!lover.OtherLover || lover.OtherLover == lover.Player || (lover.Player.GetFaction() == lover.OtherLover.GetFaction() && !CustomGameOptions.LoversFaction) ||
                        !lover.OtherLover.Is(ObjectifierEnum.Lovers)))
                    {
                        NullLayer(lover.Player, PlayerLayerEnum.Objectifier);
                    }
                }

                LogSomething("Lovers Set");
            }

            if (CustomGameOptions.RivalsOn > 0)
            {
                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (rival.OtherRival)
                        continue;

                    var ratelimit = 0;

                    while (!rival.OtherRival || rival.OtherRival == rival.Player || (rival.Player.GetFaction() == rival.OtherRival.GetFaction() && !CustomGameOptions.RivalsFaction) ||
                        !rival.OtherRival.Is(ObjectifierEnum.Rivals) || Objectifier.GetObjectifier<Rivals>(rival.OtherRival).OtherRival)
                    {
                        rival.OtherRival = CustomPlayer.AllPlayers.Random();
                        ratelimit++;

                        if (ratelimit > 1000)
                            break;
                    }

                    if (!(!rival.OtherRival || rival.OtherRival == rival.Player || (rival.Player.GetFaction() == rival.OtherRival.GetFaction() && !CustomGameOptions.RivalsFaction) ||
                        !rival.OtherRival.Is(ObjectifierEnum.Rivals)))
                    {
                        Objectifier.GetObjectifier<Rivals>(rival.OtherRival).OtherRival = rival.Player;
                        CallRpc(CustomRPC.Target, TargetRPC.SetDuo, rival.Player, rival.OtherRival);

                        if (TownOfUsReworked.IsTest)
                            LogSomething($"Rivals = {rival.Player.name} & {rival.OtherRival?.name}");
                    }
                }

                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (!(!rival.OtherRival || rival.OtherRival == rival.Player || (rival.Player.GetFaction() == rival.OtherRival.GetFaction() && !CustomGameOptions.RivalsFaction) ||
                        !rival.OtherRival.Is(ObjectifierEnum.Rivals)))
                    {
                        NullLayer(rival.Player, PlayerLayerEnum.Objectifier);
                    }
                }

                LogSomething("Rivals Set");
            }

            if (CustomGameOptions.LinkedOn > 0)
            {
                foreach (var link in Objectifier.GetObjectifiers<Linked>(ObjectifierEnum.Linked))
                {
                    if (link.OtherLink)
                        continue;

                    var ratelimit = 0;

                    while (!link.OtherLink || link.OtherLink == link.Player || !link.OtherLink.Is(Faction.Neutral) || !link.OtherLink.Is(ObjectifierEnum.Linked) ||
                        Objectifier.GetObjectifier<Linked>(link.OtherLink).OtherLink)
                    {
                        link.OtherLink = CustomPlayer.AllPlayers.Random();
                        ratelimit++;

                        if (ratelimit > 1000)
                            break;
                    }

                    if (!(!link.OtherLink || link.OtherLink == link.Player || !link.OtherLink.Is(Faction.Neutral) || !link.OtherLink.Is(ObjectifierEnum.Linked)))
                    {
                        Objectifier.GetObjectifier<Linked>(link.OtherLink).OtherLink = link.Player;
                        CallRpc(CustomRPC.Target, TargetRPC.SetLinked, link.Player, link.OtherLink);

                        if (TownOfUsReworked.IsTest)
                            LogSomething($"Linked = {link.PlayerName} & {link.OtherLink?.name}");
                    }
                }

                foreach (var link in Objectifier.GetObjectifiers<Linked>(ObjectifierEnum.Linked))
                {
                    if (!link.OtherLink || link.OtherLink == link.Player || !link.OtherLink.Is(Faction.Neutral) || !link.OtherLink.Is(ObjectifierEnum.Linked))
                        NullLayer(link.Player, PlayerLayerEnum.Objectifier);
                }

                LogSomething("Linked Set");
            }

            if (CustomGameOptions.MafiaOn > 0)
            {
                if (CustomPlayer.AllPlayers.Count(x => x.Is(ObjectifierEnum.Mafia)) == 1)
                {
                    foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(ObjectifierEnum.Mafia)))
                        NullLayer(player, PlayerLayerEnum.Objectifier);
                }

                LogSomething("Mafia Set");
            }

            LogSomething("Layers Nulled");

            if (CustomGameOptions.ExecutionerOn > 0 && !CustomGameOptions.ExecutionerCanPickTargets)
            {
                foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
                {
                    exe.TargetPlayer = null;
                    var ratelimit = 0;

                    while (!exe.TargetPlayer || exe.TargetPlayer == exe.Player || exe.TargetPlayer.IsLinkedTo(exe.Player) || exe.TargetPlayer.Is(RoleAlignment.CrewSov))
                    {
                        exe.TargetPlayer = PlayerControl.AllPlayerControls.Random();
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
                foreach (var guess in Role.GetRoles<Guesser>(RoleEnum.Guesser))
                {
                    guess.TargetPlayer = null;
                    var ratelimit = 0;

                    while (!guess.TargetPlayer || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(ModifierEnum.Indomitable) || guess.TargetPlayer.IsLinkedTo(guess.Player) ||
                        guess.TargetPlayer.Is(RoleAlignment.CrewInvest))
                    {
                        guess.TargetPlayer = PlayerControl.AllPlayerControls.Random();
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
                foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
                {
                    ga.TargetPlayer = null;
                    var ratelimit = 0;

                    while (!ga.TargetPlayer || ga.TargetPlayer == ga.Player || ga.TargetPlayer.IsLinkedTo(ga.Player) || ga.TargetPlayer.Is(RoleAlignment.NeutralEvil))
                    {
                        ga.TargetPlayer = PlayerControl.AllPlayerControls.Random();
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
                foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
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
                foreach (var act in Role.GetRoles<Actor>(RoleEnum.Actor))
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
                foreach (var jackal in Role.GetRoles<Jackal>(RoleEnum.Jackal))
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

                if ((!CustomGameOptions.MayorButton && player.Is(RoleEnum.Mayor)) || (!CustomGameOptions.SwapperButton && player.Is(AbilityEnum.Swapper)) || (!CustomGameOptions.ActorButton
                    && player.Is(RoleEnum.Actor)) || player.Is(ModifierEnum.Shy) || (!CustomGameOptions.ExecutionerButton && player.Is(RoleEnum.Executioner)) ||
                    (!CustomGameOptions.GuesserButton && player.Is(RoleEnum.Guesser)) || (!CustomGameOptions.JesterButton && player.Is(RoleEnum.Jester)) ||
                    (!CustomGameOptions.PoliticianButton && player.Is(AbilityEnum.Politician)) || (!CustomGameOptions.DictatorButton && player.Is(RoleEnum.Dictator)) ||
                    (!CustomGameOptions.MonarchButton && player.Is(RoleEnum.Monarch)))
                {
                    player.RemainingEmergencies = 0;
                    CallRpc(CustomRPC.Misc, MiscRPC.RemoveMeetings, player);
                }

                if (TownOfUsReworked.IsTest)
                    LogSomething($"{player.name} -> {Role.GetRole(player)}, {Objectifier.GetObjectifier(player)}, {Modifier.GetModifier(player)}, {Ability.GetAbility(player)}");

                player.MaxReportDistance = CustomGameOptions.ReportDistance;
                CallRpc(CustomRPC.Misc, MiscRPC.SetReports, player);
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

            InteractableBehaviour.AllCustomPlateform.Clear();
            InteractableBehaviour.NearestTask = null;

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
        }

        public static void BeginRoleGen()
        {
            if (IsHnS)
                return;

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
        }

        private static Role SetRole(int id, PlayerControl player) => (RoleEnum)id switch
        {
            RoleEnum.Altruist => new Altruist(player),
            RoleEnum.Chameleon => new Chameleon(player),
            RoleEnum.Coroner => new Coroner(player),
            RoleEnum.Crewmate => new Crewmate(player),
            RoleEnum.Detective => new Detective(player),
            RoleEnum.Dictator => new Dictator(player),
            RoleEnum.Engineer => new Engineer(player),
            RoleEnum.Escort => new Escort(player),
            RoleEnum.Inspector => new Inspector(player),
            RoleEnum.Mayor => new Mayor(player),
            RoleEnum.Medic => new Medic(player),
            RoleEnum.Medium => new Medium(player),
            RoleEnum.Monarch => new Monarch(player),
            RoleEnum.Mystic => new Mystic(player),
            RoleEnum.Operative => new Operative(player),
            RoleEnum.Retributionist => new Retributionist(player),
            RoleEnum.Revealer => new Revealer(player),
            RoleEnum.Seer => new Seer(player),
            RoleEnum.Sheriff => new Sheriff(player),
            RoleEnum.Shifter => new Shifter(player),
            RoleEnum.Tracker => new Tracker(player),
            RoleEnum.Transporter => new Transporter(player),
            RoleEnum.VampireHunter => new VampireHunter(player),
            RoleEnum.Veteran => new Veteran(player),
            RoleEnum.Vigilante => new Vigilante(player),
            RoleEnum.Actor => new Actor(player),
            RoleEnum.Amnesiac => new Amnesiac(player),
            RoleEnum.Arsonist => new Arsonist(player),
            RoleEnum.BountyHunter => new BountyHunter(player),
            RoleEnum.Cannibal => new Cannibal(player),
            RoleEnum.Cryomaniac => new Cryomaniac(player),
            RoleEnum.Dracula => new Dracula(player),
            RoleEnum.Executioner => new Executioner(player),
            RoleEnum.Glitch => new Glitch(player),
            RoleEnum.GuardianAngel => new GuardianAngel(player),
            RoleEnum.Guesser => new Guesser(player),
            RoleEnum.Jackal => new Jackal(player),
            RoleEnum.Jester => new Jester(player),
            RoleEnum.Juggernaut => new Juggernaut(player),
            RoleEnum.Murderer => new Murderer(player),
            RoleEnum.Necromancer => new Necromancer(player),
            RoleEnum.Pestilence => new Pestilence(player),
            RoleEnum.Phantom => new Phantom(player),
            RoleEnum.Plaguebearer => new Plaguebearer(player),
            RoleEnum.SerialKiller => new SerialKiller(player),
            RoleEnum.Survivor => new Survivor(player),
            RoleEnum.Thief => new Thief(player),
            RoleEnum.Troll => new Troll(player),
            RoleEnum.Werewolf => new Werewolf(player),
            RoleEnum.Whisperer => new Whisperer(player),
            RoleEnum.Betrayer => new Betrayer(player),
            RoleEnum.Ambusher => new Ambusher(player),
            RoleEnum.Blackmailer => new Blackmailer(player),
            RoleEnum.Camouflager => new Camouflager(player),
            RoleEnum.Consigliere => new Consigliere(player),
            RoleEnum.Consort => new Consort(player),
            RoleEnum.Disguiser => new Disguiser(player),
            RoleEnum.Enforcer => new Enforcer(player),
            RoleEnum.Ghoul => new Ghoul(player),
            RoleEnum.Godfather => new Godfather(player),
            RoleEnum.Grenadier => new Grenadier(player),
            RoleEnum.Impostor => new Impostor(player),
            RoleEnum.Janitor => new Janitor(player),
            RoleEnum.Mafioso => new Mafioso(player),
            RoleEnum.Miner => new Miner(player),
            RoleEnum.Morphling => new Morphling(player),
            RoleEnum.PromotedGodfather => new PromotedGodfather(player),
            RoleEnum.Teleporter => new Teleporter(player),
            RoleEnum.Wraith => new Wraith(player),
            RoleEnum.Anarchist => new Anarchist(player),
            RoleEnum.Banshee => new Banshee(player),
            RoleEnum.Bomber => new Bomber(player),
            RoleEnum.Concealer => new Concealer(player),
            RoleEnum.Collider => new PlayerLayers.Roles.Collider(player),
            RoleEnum.Crusader => new Crusader(player),
            RoleEnum.Drunkard => new Drunkard(player),
            RoleEnum.Framer => new Framer(player),
            RoleEnum.Poisoner => new Poisoner(player),
            RoleEnum.PromotedRebel => new PromotedRebel(player),
            RoleEnum.Rebel => new Rebel(player),
            RoleEnum.Shapeshifter => new Shapeshifter(player),
            RoleEnum.Sidekick => new Sidekick(player),
            RoleEnum.Silencer => new Silencer(player),
            RoleEnum.Spellslinger => new Spellslinger(player),
            RoleEnum.Stalker => new Stalker(player),
            RoleEnum.TimeKeeper => new TimeKeeper(player),
            RoleEnum.Warper => new Warper(player),
            _ => new Roleless(player)
        };

        private static Ability SetAbility(int id, PlayerControl player) => (AbilityEnum)id switch
        {
            AbilityEnum.CrewAssassin => new CrewAssassin(player),
            AbilityEnum.IntruderAssassin => new IntruderAssassin(player),
            AbilityEnum.NeutralAssassin => new NeutralAssassin(player),
            AbilityEnum.SyndicateAssassin => new SyndicateAssassin(player),
            AbilityEnum.ButtonBarry => new ButtonBarry(player),
            AbilityEnum.Insider => new Insider(player),
            AbilityEnum.Multitasker => new Multitasker(player),
            AbilityEnum.Ninja => new Ninja(player),
            AbilityEnum.Politician => new Politician(player),
            AbilityEnum.Radar => new Radar(player),
            AbilityEnum.Ruthless => new Ruthless(player),
            AbilityEnum.Snitch => new Snitch(player),
            AbilityEnum.Swapper => new Swapper(player),
            AbilityEnum.Tiebreaker => new Tiebreaker(player),
            AbilityEnum.Torch => new Torch(player),
            AbilityEnum.Tunneler => new Tunneler(player),
            AbilityEnum.Underdog => new Underdog(player),
            _ => new Abilityless(player),
        };

        private static Objectifier SetObjectifier(int id, PlayerControl player) => (ObjectifierEnum)id switch
        {
            ObjectifierEnum.Allied => new Allied(player),
            ObjectifierEnum.Corrupted => new Corrupted(player),
            ObjectifierEnum.Defector => new Defector(player),
            ObjectifierEnum.Fanatic => new Fanatic(player),
            ObjectifierEnum.Linked => new Linked(player),
            ObjectifierEnum.Lovers => new Lovers(player),
            ObjectifierEnum.Mafia => new Mafia(player),
            ObjectifierEnum.Overlord => new Overlord(player),
            ObjectifierEnum.Rivals => new Rivals(player),
            ObjectifierEnum.Taskmaster => new Taskmaster(player),
            ObjectifierEnum.Traitor => new Traitor(player),
            _ => new Objectifierless(player),
        };

        private static Modifier SetModifier(int id, PlayerControl player) => (ModifierEnum)id switch
        {
            ModifierEnum.Astral => new Astral(player),
            ModifierEnum.Bait => new Bait(player),
            ModifierEnum.Coward => new Coward(player),
            ModifierEnum.Diseased => new Diseased(player),
            ModifierEnum.Drunk => new Drunk(player),
            ModifierEnum.Dwarf => new Dwarf(player),
            ModifierEnum.Flincher => new Flincher(player),
            ModifierEnum.Giant => new Giant(player),
            ModifierEnum.Indomitable => new Indomitable(player),
            ModifierEnum.Professional => new Professional(player),
            ModifierEnum.Shy => new Shy(player),
            ModifierEnum.VIP => new VIP(player),
            ModifierEnum.Volatile => new Volatile(player),
            ModifierEnum.Yeller => new Yeller(player),
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
            if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
                return;

            if (!CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate)))
                return;

            var all = CustomPlayer.AllPlayers.Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate)).ToList();
            PlayerControl chosen = null;

            if (Role.DriveHolder == null || Role.DriveHolder.Data.IsDead || Role.DriveHolder.Data.Disconnected)
            {
                chosen = all.Find(x => x.Is(RoleEnum.PromotedRebel));

                if (chosen == null)
                    chosen = all.Find(x => x.Is(RoleAlignment.SyndicateDisrup));

                if (chosen == null)
                    chosen = all.Find(x => x.Is(RoleAlignment.SyndicateSupport));

                if (chosen == null)
                    chosen = all.Find(x => x.Is(RoleAlignment.SyndicatePower));

                if (chosen == null)
                    chosen = all.Find(x => x.Is(RoleAlignment.SyndicateKill));

                if (chosen == null)
                    chosen = all.Find(x => x.Is(RoleEnum.Anarchist) || x.Is(RoleEnum.Rebel) || x.Is(RoleEnum.Sidekick));
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
                    if (converter.Is(RoleEnum.Dracula))
                    {
                        if (converts)
                        {
                            ((Dracula)role2).Converted.Add(target);
                            role1.IsBitten = true;
                        }
                        else if (converted.IsBitten())
                            ((Dracula)role2).Converted.Add(target);
                        else if (converted.Is(RoleEnum.Dracula))
                        {
                            ((Dracula)role2).Converted.AddRange(((Dracula)role1).Converted);
                            ((Dracula)role1).Converted.AddRange(((Dracula)role2).Converted);
                        }
                    }
                    else if (converter.Is(RoleEnum.Whisperer))
                    {
                        if (converts)
                        {
                            ((Whisperer)role2).Persuaded.Add(target);
                            role1.IsPersuaded = true;
                        }
                        else if (converted.IsPersuaded())
                            ((Whisperer)role2).Persuaded.Add(target);
                        else if (converted.Is(RoleEnum.Whisperer))
                        {
                            ((Whisperer)role2).Persuaded.AddRange(((Whisperer)role1).Persuaded);
                            ((Whisperer)role1).Persuaded.AddRange(((Whisperer)role2).Persuaded);
                        }
                    }
                    else if (converter.Is(RoleEnum.Necromancer))
                    {
                        if (converts)
                        {
                            ((Necromancer)role2).Resurrected.Add(target);
                            role1.IsResurrected = true;
                        }
                        else if (converted.IsResurrected())
                            ((Necromancer)role2).Resurrected.Add(target);
                        else if (converted.Is(RoleEnum.Necromancer))
                        {
                            ((Necromancer)role2).Resurrected.AddRange(((Necromancer)role1).Resurrected);
                            ((Necromancer)role1).Resurrected.AddRange(((Necromancer)role2).Resurrected);
                        }
                    }
                    else if (converter.Is(RoleEnum.Jackal))
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
                        else if (converted.Is(RoleEnum.Jackal))
                        {
                            ((Jackal)role2).Recruited.AddRange(((Jackal)role1).Recruited);
                            ((Jackal)role1).Recruited.AddRange(((Jackal)role2).Recruited);
                        }
                    }

                    var flash = sub switch
                    {
                        SubFaction.Undead => Colors.Undead,
                        SubFaction.Cabal => Colors.Cabal,
                        SubFaction.Reanimated => Colors.Reanimated,
                        SubFaction.Sect => Colors.Sect,
                        _ => Colors.Stalemate
                    };

                    var symbol = sub switch
                    {
                        SubFaction.Undead => "γ",
                        SubFaction.Cabal => "$",
                        SubFaction.Reanimated => "Σ",
                        SubFaction.Sect => "Λ",
                        _ => "φ"
                    };

                    role1.SubFaction = sub;
                    role1.SubFactionColor = flash;
                    role1.RoleAlignment = role1.RoleAlignment.GetNewAlignment(Faction.Neutral);
                    role1.SubFactionSymbol = symbol;
                    Convertible--;

                    if (CustomPlayer.Local == converted)
                        Flash(flash);
                    else if (CustomPlayer.Local.Is(RoleEnum.Mystic))
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
}