namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class RoleGen
    {
        private static readonly List<(int, int, bool)> CrewAuditorRoles = new();
        private static readonly List<(int, int, bool)> CrewKillingRoles = new();
        private static readonly List<(int, int, bool)> CrewSupportRoles = new();
        private static readonly List<(int, int, bool)> CrewSovereignRoles = new();
        private static readonly List<(int, int, bool)> CrewProtectiveRoles = new();
        private static readonly List<(int, int, bool)> CrewInvestigativeRoles = new();
        private static readonly List<(int, int, bool)> CrewRoles = new();

        private static readonly List<(int, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(int, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(int, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(int, int, bool)> NeutralNeophyteRoles = new();
        private static readonly List<(int, int, bool)> NeutralHarbingerRoles = new();
        private static readonly List<(int, int, bool)> NeutralRoles = new();

        private static readonly List<(int, int, bool)> IntruderKillingRoles = new();
        private static readonly List<(int, int, bool)> IntruderSupportRoles = new();
        private static readonly List<(int, int, bool)> IntruderDeceptionRoles = new();
        private static readonly List<(int, int, bool)> IntruderConcealingRoles = new();
        private static readonly List<(int, int, bool)> IntruderRoles = new();

        private static readonly List<(int, int, bool)> SyndicatePowerRoles = new();
        private static readonly List<(int, int, bool)> SyndicateSupportRoles = new();
        private static readonly List<(int, int, bool)> SyndicateKillingRoles = new();
        private static readonly List<(int, int, bool)> SyndicateDisruptionRoles = new();
        private static readonly List<(int, int, bool)> SyndicateRoles = new();

        private static readonly List<(int, int, bool)> AllModifiers = new();
        private static readonly List<(int, int, bool)> AllAbilities = new();
        private static readonly List<(int, int, bool)> AllObjectifiers = new();
        private static readonly List<(int, int, bool)> AllRoles = new();

        public static PlayerControl PureCrew;
        public static int Convertible;

        private static void Sort(this List<(int, int, bool)> items, int max, int min)
        {
            if (items.Count is 1 or 0)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min > max)
                (max, min) = (min, max);

            var amount = URandom.RandomRangeInt(min, max + 1);
            var tempList = new List<(int, int, bool)>();

            foreach (var item in items)
            {
                if (tempList.Count == amount)
                    break;

                var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                if (chance == 100)
                    tempList.Add(item);
            }

            foreach (var item in items)
            {
                if (tempList.Count == amount)
                    break;

                var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                if (chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < chance)
                        tempList.Add(item);
                }
            }

            tempList.Shuffle();
            items = tempList;
        }

        private static void Sort(this List<(int, int, bool)> items, int amount)
        {
            var newList = new List<(int, int, bool)>();

            if (ConstantVariables.IsAA)
            {
                if (amount < CustomPlayer.AllPlayers.Count)
                    amount = CustomPlayer.AllPlayers.Count;

                while (newList.Count < amount && items.Count > 0)
                {
                    items.Shuffle();
                    newList.Add(items[0]);

                    if (items[0].Item3 && CustomGameOptions.EnableUniques)
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
                    if (newList.Count == amount)
                        break;

                    var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                    if (chance == 100)
                        newList.Add(item);
                }

                foreach (var item in items)
                {
                    if (newList.Count == amount)
                        break;

                    var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                    if (chance < 100)
                    {
                        var random = URandom.RandomRangeInt(0, 100);

                        if (random < chance)
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
            neutrals = ConstantVariables.IsKilling ? CustomGameOptions.NeutralRoles : URandom.RandomRangeInt(CustomGameOptions.NeutralMin, CustomGameOptions.NeutralMax);

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

            if (!ConstantVariables.IsAA)
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
        }

        private static void GenVanilla()
        {
            Utils.LogSomething("Role Gen Start");
            var players = CustomPlayer.AllPlayers;
            var spawnList = new List<(int, int, bool)>();

            AllRoles.Clear();

            while (AllRoles.Count < (CustomGameOptions.AltImps ? CustomGameOptions.SyndicateCount : CustomGameOptions.IntruderCount))
                AllRoles.Add((100, CustomGameOptions.AltImps ? 57 : 52, false));

            while (AllRoles.Count < players.Count)
                AllRoles.Add((100, 20, false));

            spawnList = AllRoles;

            Utils.LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $" {id}";

                Utils.LogSomething("Roles in the game " + ids);
            }

            while (AllRoles.Count > 0 && spawnList.Count > 0)
            {
                var id = spawnList.TakeFirst().Item2;
                Gen(players.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenKilling()
        {
            Utils.LogSomething("Role Gen Start");
            var players = CustomPlayer.AllPlayers;
            var spawnList = new List<(int, int, bool)>();
            GetAdjustedFactions(out var imps, out var syn, out var neutrals, out var crew);

            CrewRoles.Clear();
            IntruderRoles.Clear();
            SyndicateRoles.Clear();
            NeutralRoles.Clear();

            Utils.LogSomething("Lists Cleared - Killing Only");

            IntruderRoles.Clear();
            IntruderRoles.Add((CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
            IntruderRoles.Add((CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
            IntruderRoles.Add((CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
            IntruderRoles.Add((CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
            IntruderRoles.Add((CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
            IntruderRoles.Add((CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
            IntruderRoles.Add((CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
            IntruderRoles.Add((CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
            IntruderRoles.Add((CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
            IntruderRoles.Add((CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
            IntruderRoles.Add((CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
            IntruderRoles.Add((CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
            IntruderRoles.Add((5, 52, false));

            if (CustomGameOptions.IntruderCount >= 3)
                IntruderRoles.Add((CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));

            SyndicateRoles.Add((5, 57, false));
            SyndicateRoles.Add((CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
            SyndicateRoles.Add((CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
            SyndicateRoles.Add((CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));
            SyndicateRoles.Add((CustomGameOptions.ColliderOn, 7, CustomGameOptions.UniqueCollider));

            if (CustomGameOptions.SyndicateCount >= 3)
                SyndicateRoles.Add((CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));

            NeutralRoles.Add((CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
            NeutralRoles.Add((CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
            NeutralRoles.Add((CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
            NeutralRoles.Add((CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
            NeutralRoles.Add((CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
            NeutralRoles.Add((CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));

            if (CustomGameOptions.AddArsonist)
                NeutralRoles.Add((CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralRoles.Add((CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
                NeutralRoles.Add((CustomGameOptions.PlaguebearerOn, CustomGameOptions.PestSpawn ? 33 : 34, CustomGameOptions.UniquePlaguebearer));

            NeutralRoles.Sort(neutrals);

            var vigis = crew / 2;
            var vets = crew / 2;

            while (vigis > 0 || vets > 0)
            {
                if (vigis > 0)
                {
                    CrewRoles.Add((100, 3, false));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add((100, 11, false));
                    vets--;
                }
            }

            Utils.LogSomething("Lists Set - Killing Only");

            IntruderRoles.Sort(imps);
            CrewRoles.Sort(crew);

            Utils.LogSomething("Killing Role List Sorted");

            AllRoles.AddRange(NeutralRoles);
            AllRoles.AddRange(CrewRoles);
            AllRoles.AddRange(SyndicateRoles);

            if (!CustomGameOptions.AltImps)
                AllRoles.AddRange(IntruderRoles);

            Utils.LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in AllRoles)
                    ids += $" {id}";

                Utils.LogSomething("Roles in the game " + ids);
            }

            spawnList = AllRoles;

            while (players.Count > 0 && spawnList.Count > 0)
            {
                var id = spawnList.TakeFirst().Item2;
                Gen(players.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            Role.SyndicateHasChaosDrive = true;
            AssignChaosDrive();
            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenClassicCustomAA()
        {
            Utils.LogSomething("Role Gen Start");
            var players = CustomPlayer.AllPlayers;
            var spawnList = new List<(int, int, bool)>();

            SetPostmortals.PhantomOn = Utils.Check(CustomGameOptions.PhantomOn);
            SetPostmortals.RevealerOn = Utils.Check(CustomGameOptions.RevealerOn);
            SetPostmortals.BansheeOn = Utils.Check(CustomGameOptions.BansheeOn);
            SetPostmortals.GhoulOn = Utils.Check(CustomGameOptions.GhoulOn);

            var num = 0;

            if (CustomGameOptions.MayorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MayorCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((CustomGameOptions.MayorOn, 0, CustomGameOptions.UniqueMayor));
                    num--;
                }

                Utils.LogSomething("Mayor Done");
            }

            if (CustomGameOptions.MonarchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MonarchCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((CustomGameOptions.MonarchOn, 5, CustomGameOptions.UniqueMonarch));
                    num--;
                }

                Utils.LogSomething("Monarch Done");
            }

            if (CustomGameOptions.DictatorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DictatorCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((CustomGameOptions.DictatorOn, 6, CustomGameOptions.UniqueDictator));
                    num--;
                }

                Utils.LogSomething("Dictator Done");
            }

            if (CustomGameOptions.SheriffOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SheriffCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.SheriffOn, 1, CustomGameOptions.UniqueSheriff));
                    num--;
                }

                Utils.LogSomething("Sheriff Done");
            }

            if (CustomGameOptions.InspectorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.InspectorCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.InspectorOn, 2, CustomGameOptions.UniqueInspector));
                    num--;
                }

                Utils.LogSomething("Inspector Done");
            }

            if (CustomGameOptions.VigilanteOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VigilanteCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((CustomGameOptions.VigilanteOn, 3, CustomGameOptions.UniqueVigilante));
                    num--;
                }

                Utils.LogSomething("Vigilante Done");
            }

            if (CustomGameOptions.EngineerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.EngineerCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.EngineerOn, 4, CustomGameOptions.UniqueEngineer));
                    num--;
                }

                Utils.LogSomething("Engineer Done");
            }

            if (CustomGameOptions.MedicOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MedicCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((CustomGameOptions.MedicOn, 8, CustomGameOptions.UniqueMedic));
                    num--;
                }

                Utils.LogSomething("Medic Done");
            }

            if (CustomGameOptions.AltruistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AltruistCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((CustomGameOptions.AltruistOn, 10, CustomGameOptions.UniqueAltruist));
                    num--;
                }

                Utils.LogSomething("Altruist Done");
            }

            if (CustomGameOptions.VeteranOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VeteranCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((CustomGameOptions.VeteranOn, 11, CustomGameOptions.UniqueVeteran));
                    num--;
                }

                Utils.LogSomething("Veteran Done");
            }

            if (CustomGameOptions.TrackerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TrackerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.TrackerOn, 12, CustomGameOptions.UniqueTracker));
                    num--;
                }

                Utils.LogSomething("Tracker Done");
            }

            if (CustomGameOptions.TransporterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TransporterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.TransporterOn, 13, CustomGameOptions.UniqueTransporter));
                    num--;
                }

                Utils.LogSomething("Transporter Done");
            }

            if (CustomGameOptions.MediumOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MediumCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.MediumOn, 14, CustomGameOptions.UniqueMedium));
                    num--;
                }

                Utils.LogSomething("Medium Done");
            }

            if (CustomGameOptions.CoronerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CoronerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.CoronerOn, 15, CustomGameOptions.UniqueCoroner));
                    num--;
                }

                Utils.LogSomething("Coroner Done");
            }

            if (CustomGameOptions.OperativeOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.OperativeCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.OperativeOn, 16, CustomGameOptions.UniqueOperative));
                    num--;
                }

                Utils.LogSomething("Operative Done");
            }

            if (CustomGameOptions.DetectiveOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DetectiveCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.DetectiveOn, 17, CustomGameOptions.UniqueDetective));
                    num--;
                }

                Utils.LogSomething("Detective Done");
            }

            if (CustomGameOptions.EscortOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.EscortCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.EscortOn, 18, CustomGameOptions.UniqueEscort));
                    num--;
                }

                Utils.LogSomething("Escort Done");
            }

            if (CustomGameOptions.ShifterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShifterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.ShifterOn, 19, CustomGameOptions.UniqueShifter));
                    num--;
                }

                Utils.LogSomething("Shifter Done");
            }

            if (CustomGameOptions.ChameleonOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ChameleonCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.ChameleonOn, 65, CustomGameOptions.UniqueChameleon));
                    num--;
                }

                Utils.LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.RetributionistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RetributionistCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((CustomGameOptions.RetributionistOn, 68, CustomGameOptions.UniqueRetributionist));
                    num--;
                }

                Utils.LogSomething("Retributionist Done");
            }

            if (CustomGameOptions.CrewmateOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.CrewCount;

                while (num > 0)
                {
                    CrewRoles.Add((CustomGameOptions.CrewmateOn, 20, false));
                    num--;
                }

                Utils.LogSomething("Crewmate Done");
            }

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VampireHunterCount : 1;

                while (num > 0)
                {
                    CrewAuditorRoles.Add((CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter));
                    num--;
                }

                Utils.LogSomething("Vampire Hunter Done");
            }

            if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 ||
                CustomGameOptions.JackalOn > 0))
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MysticCount : 1;

                while (num > 0)
                {
                    CrewAuditorRoles.Add((CustomGameOptions.MysticOn, 71, CustomGameOptions.UniqueMystic));
                    num--;
                }

                Utils.LogSomething("Mystic Done");
            }

            if (CustomGameOptions.SeerOn > 0 && ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.BountyHunterOn > 0 ||
                CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||
                CustomGameOptions.TraitorOn > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 ||
                CustomGameOptions.GuardianAngelOn > 0 || CustomGameOptions.GuesserOn > 0 || CustomGameOptions.ShifterOn > 0))
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SeerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((CustomGameOptions.SeerOn, 72, CustomGameOptions.UniqueSeer));
                    num--;
                }

                Utils.LogSomething("Seer Done");
            }

            if (CustomGameOptions.JesterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JesterCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.JesterOn, 22, CustomGameOptions.UniqueJester));
                    num--;
                }

                Utils.LogSomething("Jester Done");
            }

            if (CustomGameOptions.AmnesiacOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AmnesiacCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((CustomGameOptions.AmnesiacOn, 23, CustomGameOptions.UniqueAmnesiac));
                    num--;
                }

                Utils.LogSomething("Amnesiac Done");
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ExecutionerCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.ExecutionerOn, 24, CustomGameOptions.UniqueExecutioner));
                    num--;
                }

                Utils.LogSomething("Executioner Done");
            }

            if (CustomGameOptions.SurvivorOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SurvivorCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((CustomGameOptions.SurvivorOn, 25, CustomGameOptions.UniqueSurvivor));
                    num--;
                }

                Utils.LogSomething("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0 && !CustomGameOptions.AvoidNeutralKingmakers)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GuardianAngelCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((CustomGameOptions.GuardianAngelOn, 26, CustomGameOptions.UniqueGuardianAngel));
                    num--;
                }

                Utils.LogSomething("Guardian Angel Done");
            }

            if (CustomGameOptions.GlitchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GlitchCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
                    num--;
                }

                Utils.LogSomething("Glitch Done");
            }

            if (CustomGameOptions.MurdererOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MurdCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
                    num--;
                }

                Utils.LogSomething("Murderer Done");
            }

            if (CustomGameOptions.CryomaniacOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CryomaniacCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));
                    num--;
                }

                Utils.LogSomething("Cryomaniac Done");
            }

            if (CustomGameOptions.WerewolfOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WerewolfCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
                    num--;
                }

                Utils.LogSomething("Werewolf Done");
            }

            if (CustomGameOptions.ArsonistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ArsonistCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));
                    num--;
                }

                Utils.LogSomething("Arsonist Done");
            }

            if (CustomGameOptions.JackalOn > 0 && GameData.Instance.PlayerCount > 5)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JackalCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((CustomGameOptions.JackalOn, 32, CustomGameOptions.UniqueJackal));
                    num--;
                }

                Utils.LogSomething("Jackal Done");
            }

            if (CustomGameOptions.NecromancerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.NecromancerCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((CustomGameOptions.NecromancerOn, 73, CustomGameOptions.UniqueNecromancer));
                    num--;
                }

                Utils.LogSomething("Necromancer Done");
            }

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.PlaguebearerCount : 1;

                while (num > 0)
                {
                    NeutralHarbingerRoles.Add((CustomGameOptions.PlaguebearerOn, CustomGameOptions.PestSpawn ? 34 : 33, CustomGameOptions.UniquePlaguebearer));
                    num--;
                }

                var PBorPest = CustomGameOptions.PestSpawn ? "Pestilence" : "Plaguebearer";
                Utils.LogSomething($"{PBorPest} Done");
            }

            if (CustomGameOptions.SerialKillerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SKCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
                    num--;
                }

                Utils.LogSomething("Serial Killer Done");
            }

            if (CustomGameOptions.JuggernautOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JuggernautCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
                    num--;
                }

                Utils.LogSomething("Juggeraut Done");
            }

            if (CustomGameOptions.CannibalOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CannibalCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.CannibalOn, 37, CustomGameOptions.UniqueCannibal));
                    num--;
                }

                Utils.LogSomething("Cannibal Done");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GuesserCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.GuesserOn, 66, CustomGameOptions.UniqueGuesser));
                    num--;
                }

                Utils.LogSomething("Guesser Done");
            }

            if (CustomGameOptions.ActorOn > 0 && (CustomGameOptions.CrewAssassinOn > 0 || CustomGameOptions.NeutralAssassinOn > 0 || CustomGameOptions.SyndicateAssassinOn > 0 ||
                CustomGameOptions.IntruderAssassinOn > 0))
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ActorCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.ActorOn, 69, CustomGameOptions.UniqueActor));
                    num--;
                }

                Utils.LogSomething("Actor Done");
            }

            if (CustomGameOptions.ThiefOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ThiefCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));
                    num--;
                }

                Utils.LogSomething("Thief Done");
            }

            if (CustomGameOptions.DraculaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DraculaCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula));
                    num--;
                }

                Utils.LogSomething("Dracula Done");
            }

            if (CustomGameOptions.WhispererOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WhispererCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((CustomGameOptions.WhispererOn, 67, CustomGameOptions.UniqueWhisperer));
                    num--;
                }

                Utils.LogSomething("Whisperer Done");
            }

            if (CustomGameOptions.TrollOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TrollCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.TrollOn, 40, CustomGameOptions.UniqueTroll));
                    num--;
                }

                Utils.LogSomething("Troll Done");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BHCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((CustomGameOptions.BountyHunterOn, 70, CustomGameOptions.UniqueBountyHunter));
                    num--;
                }

                Utils.LogSomething("Bounty Hunter Done");
            }

            if (CustomGameOptions.MorphlingOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MorphlingCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                    num--;
                }

                Utils.LogSomething("Morphling Done");
            }

            if (CustomGameOptions.BlackmailerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BlackmailerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                    num--;
                }

                Utils.LogSomething("Blackmailer Done");
            }

            if (CustomGameOptions.MinerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MinerCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                    num--;
                }

                Utils.LogSomething("Miner Done");
            }

            if (CustomGameOptions.TeleporterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TeleporterCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                    num--;
                }

                Utils.LogSomething("Teleporter Done");
            }

            if (CustomGameOptions.AmbusherOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AmbusherCount : 1;

                while (num > 0)
                {
                    IntruderKillingRoles.Add((CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
                    num--;
                }

                Utils.LogSomething("Ambusher Done");
            }

            if (CustomGameOptions.WraithOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WraithCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                    num--;
                }

                Utils.LogSomething("Wraith Done");
            }

            if (CustomGameOptions.ConsortOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConsortCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                    num--;
                }

                Utils.LogSomething("Consort Done");
            }

            if (CustomGameOptions.JanitorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JanitorCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                    num--;
                }

                Utils.LogSomething("Janitor Done");
            }

            if (CustomGameOptions.CamouflagerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CamouflagerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                    num--;
                }

                Utils.LogSomething("Camouflager Done");
            }

            if (CustomGameOptions.GrenadierOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GrenadierCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                    num--;
                }

                Utils.LogSomething("Grenadier Done");
            }

            if (CustomGameOptions.ImpostorOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.ImpCount;

                while (num > 0)
                {
                    IntruderRoles.Add((CustomGameOptions.ImpostorOn, 52, false));
                    num--;
                }

                Utils.LogSomething("Impostor Done");
            }

            if (CustomGameOptions.ConsigliereOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConsigliereCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                    num--;
                }

                Utils.LogSomething("Consigliere Done");
            }

            if (CustomGameOptions.DisguiserOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DisguiserCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                    num--;
                }

                Utils.LogSomething("Disguiser Done");
            }

            if (CustomGameOptions.EnforcerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.EnforcerCount : 1;

                while (num > 0)
                {
                    IntruderKillingRoles.Add((CustomGameOptions.EnforcerOn, 41, CustomGameOptions.UniqueEnforcer));
                    num--;
                }

                Utils.LogSomething("Enforcer Done");
            }

            if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GodfatherCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
                    num--;
                }

                Utils.LogSomething("Godfather Done");
            }

            if (CustomGameOptions.AnarchistOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.AnarchistCount;

                while (num > 0)
                {
                    SyndicateRoles.Add((CustomGameOptions.AnarchistOn, 57, false));
                    num--;
                }

                Utils.LogSomething("Anarchist Done");
            }

            if (CustomGameOptions.ShapeshifterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShapeshifterCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
                    num--;
                }

                Utils.LogSomething("Shapeshifter Done");
            }

            if (CustomGameOptions.FramerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FramerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
                    num--;
                }

                Utils.LogSomething("Framer Done");
            }

            if (CustomGameOptions.CrusaderOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CrusaderCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));
                    num--;
                }

                Utils.LogSomething("Crusader Done");
            }

            if (CustomGameOptions.RebelOn > 0 && CustomGameOptions.SyndicateCount >= 3)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RebelCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));
                    num--;
                }

                Utils.LogSomething("Rebel Done");
            }

            if (CustomGameOptions.PoisonerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.PoisonerCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                    num--;
                }

                Utils.LogSomething("Poisoner Done");
            }

            if (CustomGameOptions.ColliderOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ColliderCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((CustomGameOptions.ColliderOn, 7, CustomGameOptions.UniqueCollider));
                    num--;
                }

                Utils.LogSomething("Collider Done");
            }

            if (CustomGameOptions.ConcealerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConcealerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
                    num--;
                }

                Utils.LogSomething("Concealer Done");
            }

            if (CustomGameOptions.WarperOn > 0 && TownOfUsReworked.VanillaOptions.MapId != 4 && TownOfUsReworked.VanillaOptions.MapId != 5)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WarperCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
                    num--;
                }

                Utils.LogSomething("Warper Done");
            }

            if (CustomGameOptions.BomberOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BomberCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
                    num--;
                }

                Utils.LogSomething("Bomber Done");
            }

            if (CustomGameOptions.SpellslingerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SpellslingerCount : 1;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add((CustomGameOptions.SpellslingerOn, 55, CustomGameOptions.UniqueSpellslinger));
                    num--;
                }

                Utils.LogSomething("Spellslinger Done");
            }

            if (CustomGameOptions.StalkerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.StalkerCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((CustomGameOptions.StalkerOn, 9, CustomGameOptions.UniqueStalker));
                    num--;
                }

                Utils.LogSomething("Stalker Done");
            }

            if (CustomGameOptions.DrunkardOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DrunkardCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((CustomGameOptions.DrunkardOn, 59, CustomGameOptions.UniqueDrunkard));
                    num--;
                }

                Utils.LogSomething("Drunkard Done");
            }

            if (CustomGameOptions.TimeKeeperOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TimeKeeperCount : 1;

                while (num > 0)
                {
                    SyndicatePowerRoles.Add((CustomGameOptions.TimeKeeperOn, 74, CustomGameOptions.UniqueTimeKeeper));
                    num--;
                }

                Utils.LogSomething("Time Keeper Done");
            }

            if (CustomGameOptions.SilencerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SilencerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((CustomGameOptions.SilencerOn, 77, CustomGameOptions.UniqueSilencer));
                    num--;
                }

                Utils.LogSomething("Silencer Done");
            }

            GetAdjustedFactions(out var imps, out var syn, out var neut, out var crew);

            if (ConstantVariables.IsClassic || ConstantVariables.IsCustom)
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
                        IntruderRoles.Add((100, 52, false));

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
                        SyndicateRoles.Add((100, 57, false));

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
                        CrewRoles.Add((100, 20, false));

                    CrewRoles.Shuffle();
                }

                Utils.LogSomething("Classic/Custom Sorting Done");
            }
            else if (ConstantVariables.IsAA)
            {
                CrewRoles.AddRanges(CrewAuditorRoles, CrewInvestigativeRoles, CrewKillingRoles, CrewSupportRoles, CrewProtectiveRoles, CrewSovereignRoles);
                IntruderRoles.AddRanges(IntruderConcealingRoles, IntruderDeceptionRoles, IntruderKillingRoles, IntruderSupportRoles);
                SyndicateRoles.AddRanges(SyndicateSupportRoles, SyndicateKillingRoles, SyndicatePowerRoles, SyndicateDisruptionRoles);
                NeutralRoles.AddRanges(NeutralBenignRoles, NeutralEvilRoles, NeutralKillingRoles, NeutralNeophyteRoles, NeutralHarbingerRoles);

                CrewRoles.Shuffle();
                SyndicateRoles.Shuffle();
                IntruderRoles.Shuffle();
                NeutralRoles.Shuffle();

                Utils.LogSomething("All Any Sorting Done");
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
            spawnList = AllRoles;

            while (spawnList.Count < players.Count)
                spawnList.Add((100, 20, false));

            if (!spawnList.Any(CrewRoles.Contains) && !spawnList.Contains((100, 20, false)))
            {
                spawnList.Remove(spawnList.Random());

                if (CrewRoles.Count > 0)
                    spawnList.Add(CrewRoles.Random());
                else
                    spawnList.Add((100, 20, false));

                Utils.LogSomething("Added Solo Crew");
            }

            spawnList.Shuffle();
            Utils.LogSomething("Layers Sorted");

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $" {id}";

                Utils.LogSomething("Roles in the game " + ids);
            }

            while (players.Count > 0 && spawnList.Count > 0)
            {
                var id = spawnList.TakeFirst().Item2;
                Gen(players.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            Utils.LogSomething("Role Gen End");
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
                    AllAbilities.Add((CustomGameOptions.CrewAssassinOn, 0, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Crew Assassin Done");
            }

            if (CustomGameOptions.SyndicateAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfSyndicateAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.SyndicateAssassinOn, 12, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Syndicate Assassin Done");
            }

            if (CustomGameOptions.IntruderAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfImpostorAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.IntruderAssassinOn, 11, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Intruder Assassin Done");
            }

            if (CustomGameOptions.NeutralAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfNeutralAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.NeutralAssassinOn, 14, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Neutral Assassin Done");
            }

            if (CustomGameOptions.RuthlessOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RuthlessCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.RuthlessOn, 10, CustomGameOptions.UniqueRuthless));
                    num--;
                }

                Utils.LogSomething("Ruthless Done");
            }

            if (CustomGameOptions.SnitchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SnitchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.SnitchOn, 1, CustomGameOptions.UniqueSnitch));
                    num--;
                }

                Utils.LogSomething("Snitch Done");
            }

            if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.InsiderCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.InsiderOn, 2, CustomGameOptions.UniqueInsider));
                    num--;
                }

                Utils.LogSomething("Insider Done");
            }

            if (CustomGameOptions.MultitaskerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MultitaskerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.MultitaskerOn, 4, CustomGameOptions.UniqueMultitasker));
                    num--;
                }

                Utils.LogSomething("Multitasker Done");
            }

            if (CustomGameOptions.RadarOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RadarCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.RadarOn, 5, CustomGameOptions.UniqueRadar));
                    num--;
                }

                Utils.LogSomething("Radar Done");
            }

            if (CustomGameOptions.TiebreakerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TiebreakerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.TiebreakerOn, 6, CustomGameOptions.UniqueTiebreaker));
                    num--;
                }

                Utils.LogSomething("Tiebreaker Done");
            }

            if (CustomGameOptions.TorchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TorchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.TorchOn, 7, CustomGameOptions.UniqueTorch));
                    num--;
                }

                Utils.LogSomething("Torch Done");
            }

            if (CustomGameOptions.UnderdogOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.UnderdogCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.UnderdogOn, 8, CustomGameOptions.UniqueUnderdog));
                    num--;
                }

                Utils.LogSomething("Underdog Done");
            }

            if (CustomGameOptions.TunnelerOn > 0 && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TunnelerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.TunnelerOn, 9, CustomGameOptions.UniqueTunneler));
                    num--;
                }

                Utils.LogSomething("Tunneler Done");
            }

            if (CustomGameOptions.NinjaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.NinjaCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.NinjaOn, 14, CustomGameOptions.UniqueNinja));
                    num--;
                }

                Utils.LogSomething("Ninja Done");
            }

            if (CustomGameOptions.ButtonBarryOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ButtonBarryCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.ButtonBarryOn, 15, CustomGameOptions.UniqueButtonBarry));
                    num--;
                }

                Utils.LogSomething("Button Barry Done");
            }

            if (CustomGameOptions.PoliticianOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.PoliticianCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.PoliticianOn, 16, CustomGameOptions.UniquePolitician));
                    num--;
                }

                Utils.LogSomething("Politician Done");
            }

            if (CustomGameOptions.SwapperOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SwapperCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((CustomGameOptions.SwapperOn, 3, CustomGameOptions.UniqueSwapper));
                    num--;
                }

                Utils.LogSomething("Swapper Done");
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
            var canHaveAbility = CustomPlayer.AllPlayers;
            var canHaveBB = CustomPlayer.AllPlayers;
            var canHavePolitician = CustomPlayer.AllPlayers;

            canHaveIntruderAbility.RemoveAll(x => !x.Is(Faction.Intruder) || (x.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
            canHaveIntruderAbility.Shuffle();

            canHaveNeutralAbility.RemoveAll(x => !(x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralPros)));
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

            canHaveBB.RemoveAll(x => (x.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (x.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(RoleEnum.Guesser) && !CustomGameOptions.GuesserButton) || (x.Is(RoleEnum.Executioner) &&
                !CustomGameOptions.ExecutionerButton) || (!CustomGameOptions.MonarchButton && x.Is(RoleEnum.Monarch)) || (!CustomGameOptions.DictatorButton &&
                x.Is(RoleEnum.Dictator)));
            canHaveBB.Shuffle();

            canHavePolitician.RemoveAll(x => x.Is(RoleAlignment.NeutralEvil) || x.Is(RoleAlignment.NeutralBen) || x.Is(RoleAlignment.NeutralNeo));
            canHavePolitician.Shuffle();

            AllAbilities.Sort(allCount);
            var spawnList = AllAbilities;

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $" {id}";

                Utils.LogSomething("Abilities in the game " + ids);
            }

            while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
                canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
                canHaveTaskedAbility.Count > 0 || canHaveTorch.Count > 0 || canHaveKillingAbility.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (chance, id, unique) = spawnList.TakeFirst();
                int[] Snitch = { 1 };
                int[] Syndicate = { 12 };
                int[] Crew = { 0, 3 };
                int[] Neutral = { 13 };
                int[] Intruder = { 11 };
                int[] Killing = { 10, 14 };
                int[] Torch = { 7 };
                int[] Evil = { 8 };
                int[] Tasked = { 2, 4 };
                int[] Global = { 5, 6 };
                int[] Tunneler = { 9 };
                int[] BB = { 15 };
                int[] Pol = { 16 };

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

                    if (Ability.GetAbility(assigned) == null)
                        Gen(assigned, id, PlayerLayerEnum.Ability);
                    else
                        spawnList.Add((chance, id, unique));
                }
            }

            Utils.LogSomething("Abilities Done");
        }

        private static void GenObjectifiers()
        {
            AllObjectifiers.Clear();
            var num = 0;

            if (CustomGameOptions.LoversOn > 0 && GameData.Instance.PlayerCount > 4)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.LoversCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.LoversOn, 0, CustomGameOptions.UniqueLovers));
                    AllObjectifiers.Add((CustomGameOptions.LoversOn, 0, CustomGameOptions.UniqueLovers));
                    num--;
                }

                Utils.LogSomething("Lovers Done");
            }

            if (CustomGameOptions.RivalsOn > 0 && GameData.Instance.PlayerCount > 3)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RivalsCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.RivalsOn, 1, CustomGameOptions.UniqueRivals));
                    AllObjectifiers.Add((CustomGameOptions.RivalsOn, 1, CustomGameOptions.UniqueRivals));
                    num--;
                }

                Utils.LogSomething("Rivals Done");
            }

            if (CustomGameOptions.FanaticOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FanaticCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.FanaticOn, 2, CustomGameOptions.UniqueFanatic));
                    num--;
                }

                Utils.LogSomething("Fanatic Done");
            }

            if (CustomGameOptions.CorruptedOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CorruptedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.CorruptedOn, 3, CustomGameOptions.UniqueCorrupted));
                    num--;
                }

                Utils.LogSomething("Corrupted Done");
            }

            if (CustomGameOptions.OverlordOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.OverlordCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.OverlordOn, 4, CustomGameOptions.UniqueOverlord));
                    num--;
                }

                Utils.LogSomething("Overlord Done");
            }

            if (CustomGameOptions.AlliedOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AlliedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.AlliedOn, 5, CustomGameOptions.UniqueAllied));
                    num--;
                }

                Utils.LogSomething("Allied Done");
            }

            if (CustomGameOptions.TraitorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TraitorCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.TraitorOn, 6, CustomGameOptions.UniqueTraitor));
                    num--;
                }

                Utils.LogSomething("Traitor Done");
            }

            if (CustomGameOptions.TaskmasterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TaskmasterCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.TaskmasterOn, 7, CustomGameOptions.UniqueTaskmaster));
                    num--;
                }

                Utils.LogSomething("Taskmaster Done");
            }

            if (CustomGameOptions.MafiaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MafiaCount : 2;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.MafiaOn, 8, CustomGameOptions.UniqueMafia));
                    num--;
                }

                Utils.LogSomething("Mafia Done");
            }

            if (CustomGameOptions.DefectorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DefectorCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((CustomGameOptions.DefectorOn, 9, CustomGameOptions.UniqueDefector));
                    num--;
                }

                Utils.LogSomething("Defector Done");
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

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $" {id}";

                Utils.LogSomething("Objectifiers in the game " + ids);
            }

            while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 1 || canHaveObjectifier.Count > 0 || canHaveDefector.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (chance, id, unique) = spawnList.TakeFirst();
                int[] LoverRival = { 0, 1 };
                int[] Crew = { 2, 3, 6 };
                int[] Neutral = { 4, 7 };
                int[] Allied = { 5 };
                int[] Global = { 8 };
                int[] Defect = { 9 };

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
                        Gen(assigned, id, PlayerLayerEnum.Objectifier);
                    else
                        spawnList.Add((chance, id, unique));
                }
            }

            Utils.LogSomething("Objectifiers Done");
        }

        private static void GenModifiers()
        {
            AllModifiers.Clear();
            var num = 0;

            if (CustomGameOptions.DiseasedOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DiseasedCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.DiseasedOn, 0, CustomGameOptions.UniqueDiseased));
                    num--;
                }

                Utils.LogSomething("Diseased Done");
            }

            if (CustomGameOptions.BaitOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BaitCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.BaitOn, 1, CustomGameOptions.UniqueBait));
                    num--;
                }

                Utils.LogSomething("Bait Done");
            }

            if (CustomGameOptions.DwarfOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DwarfCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.DwarfOn, 2, CustomGameOptions.UniqueDwarf));
                    num--;
                }

                Utils.LogSomething("Dwarf Done");
            }

            if (CustomGameOptions.VIPOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VIPCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.VIPOn, 3, CustomGameOptions.UniqueVIP));
                    num--;
                }

                Utils.LogSomething("VIP Done");
            }

            if (CustomGameOptions.ShyOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShyCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.ShyOn, 4, CustomGameOptions.UniqueShy));
                    num--;
                }

                Utils.LogSomething("Shy Done");
            }

            if (CustomGameOptions.GiantOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GiantCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.GiantOn, 5, CustomGameOptions.UniqueGiant));
                    num--;
                }

                Utils.LogSomething("Giant Done");
            }

            if (CustomGameOptions.DrunkOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DrunkCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.DrunkOn, 6, CustomGameOptions.UniqueDrunk));
                    num--;
                }

                Utils.LogSomething("Drunk Done");
            }

            if (CustomGameOptions.FlincherOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FlincherCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.FlincherOn, 7, CustomGameOptions.UniqueFlincher));
                    num--;
                }

                Utils.LogSomething("Flincher Done");
            }

            if (CustomGameOptions.CowardOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CowardCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.CowardOn, 8, CustomGameOptions.UniqueCoward));
                    num--;
                }

                Utils.LogSomething("Coward Done");
            }

            if (CustomGameOptions.VolatileOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VolatileCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.VolatileOn, 9, CustomGameOptions.UniqueVolatile));
                    num--;
                }

                Utils.LogSomething("Volatile Done");
            }

            if (CustomGameOptions.IndomitableOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.IndomitableCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.IndomitableOn, 11, CustomGameOptions.UniqueIndomitable));
                    num--;
                }

                Utils.LogSomething("Indomitable Done");
            }

            if (CustomGameOptions.ProfessionalOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ProfessionalCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.ProfessionalOn, 10, CustomGameOptions.UniqueProfessional));
                    num--;
                }

                Utils.LogSomething("Professional Done");
            }

            if (CustomGameOptions.AstralOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AstralCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.AstralOn, 12, CustomGameOptions.UniqueAstral));
                    num--;
                }

                Utils.LogSomething("Astral Done");
            }

            if (CustomGameOptions.YellerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.YellerCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((CustomGameOptions.YellerOn, 13, CustomGameOptions.UniqueYeller));
                    num--;
                }

                Utils.LogSomething("Yeller Done");
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

            canHaveProfessional.RemoveAll(x => !x.Is(AbilityEnum.Assassin));
            canHaveProfessional.Shuffle();

            canHaveShy.RemoveAll(x => (x.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (x.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (x.Is(AbilityEnum.Swapper) && !CustomGameOptions.SwapperButton) || (x.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (x.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (x.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || x.Is(AbilityEnum.ButtonBarry) ||
                (x.Is(AbilityEnum.Politician) && !CustomGameOptions.PoliticianButton) || (!CustomGameOptions.DictatorButton && x.Is(RoleEnum.Dictator)) ||
                (!CustomGameOptions.MonarchButton && x.Is(RoleEnum.Monarch)));
            canHaveShy.Shuffle();

            AllModifiers.Sort(allCount);
            var spawnList = AllModifiers;

            if (TownOfUsReworked.IsTest)
            {
                var ids = "";

                foreach (var (_, id, _) in spawnList)
                    ids += $" {id}";

                Utils.LogSomething("Modifiers in the game " + ids);
            }

            while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (chance, id, unique) = spawnList.TakeFirst();
                int[] Bait = { 1 };
                int[] Diseased = { 0 };
                int[] Professional = { 10 };
                int[] Global = { 2, 3, 5, 6, 7, 8, 9, 11, 12, 13 };
                int[] Shy = { 4 };

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
                        Gen(assigned, id, PlayerLayerEnum.Modifier);
                    else
                        spawnList.Add((chance, id, unique));
                }
            }

            Utils.LogSomething("Modifiers Done");
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

                    if (CustomGameOptions.AlliedFaction == AlliedFaction.Random)
                    {
                        var random = URandom.RandomRangeInt(0, 3);
                        intr = random == 0;
                        syn = random == 1;
                        crew = random == 2;
                    }

                    if (crew)
                    {
                        alliedRole.Faction = Faction.Crew;
                        alliedRole.FactionColor = Colors.Crew;
                        alliedRole.RoleAlignment = RoleAlignment.CrewKill;
                        ally.Color = Colors.Crew;
                    }
                    else if (intr)
                    {
                        alliedRole.Faction = Faction.Intruder;
                        alliedRole.FactionColor = Colors.Intruder;
                        alliedRole.RoleAlignment = RoleAlignment.IntruderKill;
                        ally.Color = Colors.Intruder;
                    }
                    else if (syn)
                    {
                        alliedRole.Faction = Faction.Syndicate;
                        alliedRole.FactionColor = Colors.Syndicate;
                        alliedRole.RoleAlignment = RoleAlignment.SyndicateKill;
                        ally.Color = Colors.Syndicate;
                    }

                    ally.Side = alliedRole.Faction;
                    alliedRole.IsSynAlly = syn;
                    alliedRole.IsCrewAlly = crew;
                    alliedRole.IsIntAlly = intr;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetAlliedFaction);
                    writer.Write(ally.PlayerId);
                    writer.Write((byte)alliedRole.Faction);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                Utils.LogSomething("Allied Faction Set Done");
            }

            if (CustomGameOptions.LoversOn > 0)
            {
                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (lover.OtherLover != null)
                        continue;

                    while (lover.OtherLover == null || lover.OtherLover == lover.Player || (lover.Player.GetFaction() == lover.OtherLover.GetFaction() &&
                        !CustomGameOptions.LoversFaction) || !lover.OtherLover.Is(ObjectifierEnum.Lovers) || Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover != null)
                    {
                        lover.OtherLover = CustomPlayer.AllPlayers.Random();
                    }

                    Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover = lover.Player;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetCouple);
                    writer.Write(lover.PlayerId);
                    writer.Write(lover.OtherLover.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (TownOfUsReworked.IsTest)
                        Utils.LogSomething($"Lovers = {lover.Player.name} & {lover.OtherLover.name}");
                }

                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (lover.OtherLover == null)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                        writer.Write(lover.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        _ = new Objectifierless(lover.Player);
                    }
                }

                Utils.LogSomething("Lovers Set");
            }

            if (CustomGameOptions.RivalsOn > 0)
            {
                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (rival.OtherRival != null)
                        continue;

                    while (rival.OtherRival == null || rival.OtherRival == rival.Player || (rival.Player.GetFaction() == rival.OtherRival.GetFaction() &&
                        !CustomGameOptions.RivalsFaction) || !rival.OtherRival.Is(ObjectifierEnum.Rivals) || Objectifier.GetObjectifier<Rivals>(rival.OtherRival).OtherRival != null)
                    {
                        rival.OtherRival = CustomPlayer.AllPlayers.Random();
                    }

                    Objectifier.GetObjectifier<Rivals>(rival.OtherRival).OtherRival = rival.Player;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetDuo);
                    writer.Write(rival.PlayerId);
                    writer.Write(rival.OtherRival.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (TownOfUsReworked.IsTest)
                        Utils.LogSomething($"Rivals = {rival.Player.name} & {rival.OtherRival.name}");
                }

                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (rival.OtherRival == null)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                        writer.Write(rival.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        _ = new Objectifierless(rival.Player);
                    }
                }

                Utils.LogSomething("Rivals Set");
            }

            if (CustomGameOptions.MafiaOn > 0)
            {
                if (CustomPlayer.AllPlayers.Count(x => x.Is(ObjectifierEnum.Mafia)) == 1)
                {
                    foreach (var player in CustomPlayer.AllPlayers.Where(x => x.Is(ObjectifierEnum.Mafia)))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                        writer.Write(player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        _ = new Objectifierless(player);
                    }
                }

                Utils.LogSomething("Mafia Set");
            }

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (!Modifier.GetModifier(player))
                {
                    _ = new Modifierless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullModifier, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (!Ability.GetAbility(player))
                {
                    _ = new Abilityless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullAbility, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (!Objectifier.GetObjectifier(player))
                {
                    _ = new Objectifierless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            Utils.LogSomething("Layers Nulled");

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
                {
                    bh.TargetPlayer = null;

                    while (bh.TargetPlayer == null || bh.TargetPlayer == bh.Player || bh.TargetPlayer == bh.Player.GetOtherLover() || bh.TargetPlayer == bh.Player.GetOtherRival())
                        bh.TargetPlayer = CustomPlayer.AllPlayers.Random();

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetBHTarget);
                    writer.Write(bh.PlayerId);
                    writer.Write(bh.TargetPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (TownOfUsReworked.IsTest)
                        Utils.LogSomething($"BH Target = {bh.TargetPlayer.name}");
                }

                Utils.LogSomething("BH Targets Set");
            }

            if (CustomGameOptions.ActorOn > 0)
            {
                foreach (var act in Role.GetRoles<Actor>(RoleEnum.Actor))
                {
                    act.PretendRoles = InspectorResults.None;

                    while (act.PretendRoles == InspectorResults.None || act.PretendRoles == act.InspectorResults)
                        act.PretendRoles = Role.GetRole(CustomPlayer.AllPlayers.Random()).InspectorResults;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetActPretendList);
                    writer.Write(act.PlayerId);
                    writer.Write((byte)act.PretendRoles);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (TownOfUsReworked.IsTest)
                        Utils.LogSomething(act.PretendRoles);
                }

                Utils.LogSomething("Act Variables Set");
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                foreach (var jackal in Role.GetRoles<Jackal>(RoleEnum.Jackal))
                {
                    jackal.GoodRecruit = null;
                    jackal.EvilRecruit = null;
                    jackal.BackupRecruit = null;

                    while (jackal.GoodRecruit == null || jackal.GoodRecruit == jackal.Player || jackal.GoodRecruit.Is(RoleAlignment.NeutralKill) ||
                        jackal.GoodRecruit.Is(Faction.Intruder) || jackal.GoodRecruit.Is(Faction.Syndicate))
                    {
                        jackal.GoodRecruit = CustomPlayer.AllPlayers.Random();
                    }

                    Role.GetRole(jackal.GoodRecruit).SubFaction = SubFaction.Cabal;
                    Role.GetRole(jackal.GoodRecruit).SubFactionColor = Colors.Cabal;
                    Role.GetRole(jackal.GoodRecruit).IsRecruit = true;
                    jackal.Recruited.Add(jackal.GoodRecruit.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetGoodRecruit);
                    writer.Write(jackal.PlayerId);
                    writer.Write(jackal.GoodRecruit.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    while (jackal.EvilRecruit == null || jackal.EvilRecruit == jackal.Player || jackal.EvilRecruit.Is(Faction.Crew) || jackal.EvilRecruit.Is(RoleAlignment.NeutralBen))
                        jackal.EvilRecruit = CustomPlayer.AllPlayers.Random();

                    Role.GetRole(jackal.EvilRecruit).SubFaction = SubFaction.Cabal;
                    Role.GetRole(jackal.EvilRecruit).IsRecruit = true;
                    jackal.Recruited.Add(jackal.EvilRecruit.PlayerId);
                    Role.GetRole(jackal.EvilRecruit).SubFactionColor = Colors.Cabal;
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer2.Write((byte)TargetRPC.SetEvilRecruit);
                    writer2.Write(jackal.PlayerId);
                    writer2.Write(jackal.EvilRecruit.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    if (TownOfUsReworked.IsTest)
                        Utils.LogSomething($"Recruits = {jackal.GoodRecruit.name} (Good) & {jackal.EvilRecruit.name} (Evil)");
                }

                Utils.LogSomething("Jackal Recruits Set");
            }

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if ((!CustomGameOptions.MayorButton && player.Is(RoleEnum.Mayor)) || (!CustomGameOptions.SwapperButton && player.Is(AbilityEnum.Swapper)) || (!CustomGameOptions.ActorButton
                    && player.Is(RoleEnum.Actor)) || player.Is(ModifierEnum.Shy) || (!CustomGameOptions.ExecutionerButton && player.Is(RoleEnum.Executioner)) ||
                    (!CustomGameOptions.GuesserButton && player.Is(RoleEnum.Guesser)) || (!CustomGameOptions.JesterButton && player.Is(RoleEnum.Jester)) ||
                    (!CustomGameOptions.PoliticianButton && player.Is(AbilityEnum.Politician)) || (!CustomGameOptions.DictatorButton && player.Is(RoleEnum.Dictator)) ||
                    (!CustomGameOptions.MonarchButton && player.Is(RoleEnum.Monarch)))
                {
                    player.RemainingEmergencies = 0;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RemoveMeetings, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (!Role.GetRole(player))
                {
                    _ = new Roleless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullRole, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (TownOfUsReworked.IsTest)
                {
                    Utils.LogSomething($"{player.name} -> {Role.GetRole(player).Name}, {Objectifier.GetObjectifier(player).Name}, {Modifier.GetModifier(player).Name}, " +
                        Ability.GetAbility(player).Name);
                }

                player.MaxReportDistance = CustomGameOptions.ReportDistance;
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetReports, SendOption.Reliable);
                writer2.Write(player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }

            Utils.LogSomething("Players Set");
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

            Utils.KilledPlayers.Clear();

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

            SetPostmortals.WillBeBanshee = null;
            SetPostmortals.WillBeGhoul = null;
            SetPostmortals.WillBeRevealer = null;
            SetPostmortals.WillBePhantom = null;

            PureCrew = null;
            Convertible = 0;

            InteractableBehaviour.AllCustomPlateform.Clear();
            InteractableBehaviour.NearestTask = null;

            Utils.RecentlyKilled.Clear();

            DisconnectHandler.Disconnected.Clear();

            CustomButton.AllButtons.Clear();

            Ash.AllPiles.Clear();
            Objects.Range.AllItems.Clear();

            GameSettings.SettingsPage = 0;

            Assassin.RemainingKills = CustomGameOptions.AssassinKills;

            Summary.Disconnected.Clear();
        }

        public static void BeginRoleGen()
        {
            if (ConstantVariables.IsHnS)
                return;

            if (ConstantVariables.IsKilling)
                GenKilling();
            else if (ConstantVariables.IsVanilla)
                GenVanilla();
            else
                GenClassicCustomAA();

            PureCrew = CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Crew)).ToList().Random();

            if (!ConstantVariables.IsVanilla)
            {
                if (CustomGameOptions.EnableObjectifiers)
                    GenObjectifiers();

                if (CustomGameOptions.EnableAbilities)
                    GenAbilities();

                if (CustomGameOptions.EnableModifiers)
                    GenModifiers();

                SetTargets();
            }

            Convertible = CustomPlayer.AllPlayers.Count(x => x.Is(SubFaction.None) && x != PureCrew);
        }

        private static void SetRole(int id, PlayerControl player)
        {
            switch (id)
            {
                case 0:
                    _ = new Mayor(player);
                    break;
                case 1:
                    _ = new Sheriff(player);
                    break;
                case 2:
                    _ = new Inspector(player);
                    break;
                case 3:
                    _ = new Vigilante(player);
                    break;
                case 4:
                    _ = new Engineer(player);
                    break;
                case 5:
                    _ = new Monarch(player);
                    break;
                case 6:
                    _ = new Dictator(player);
                    break;
                case 7:
                    _ = new PlayerLayers.Roles.Collider(player);
                    break;
                case 8:
                    _ = new Medic(player);
                    break;
                case 9:
                    _ = new Stalker(player);
                    break;
                case 10:
                    _ = new Altruist(player);
                    break;
                case 11:
                    _ = new Veteran(player);
                    break;
                case 12:
                    _ = new Tracker(player);
                    break;
                case 13:
                    _ = new Transporter(player);
                    break;
                case 14:
                    _ = new Medium(player);
                    break;
                case 15:
                    _ = new Coroner(player);
                    break;
                case 16:
                    _ = new Operative(player);
                    break;
                case 17:
                    _ = new Detective(player);
                    break;
                case 18:
                    _ = new Escort(player);
                    break;
                case 19:
                    _ = new Shifter(player);
                    break;
                case 20:
                    _ = new Crewmate(player);
                    break;
                case 21:
                    _ = new VampireHunter(player);
                    break;
                case 22:
                    _ = new Jester(player);
                    break;
                case 23:
                    _ = new Amnesiac(player);
                    break;
                case 24:
                    _ = new Executioner(player);
                    break;
                case 25:
                    _ = new Survivor(player);
                    break;
                case 26:
                    _ = new GuardianAngel(player);
                    break;
                case 27:
                    _ = new Glitch(player);
                    break;
                case 28:
                    _ = new Murderer(player);
                    break;
                case 29:
                    _ = new Cryomaniac(player);
                    break;
                case 30:
                    _ = new Werewolf(player);
                    break;
                case 31:
                    _ = new Arsonist(player);
                    break;
                case 32:
                    _ = new Jackal(player);
                    break;
                case 33:
                    _ = new Plaguebearer(player);
                    break;
                case 34:
                    _ = new Pestilence(player);
                    break;
                case 35:
                    _ = new SerialKiller(player);
                    break;
                case 36:
                    _ = new Juggernaut(player);
                    break;
                case 37:
                    _ = new Cannibal(player);
                    break;
                case 38:
                    _ = new Thief(player);
                    break;
                case 39:
                    _ = new Dracula(player);
                    break;
                case 40:
                    _ = new Troll(player);
                    break;
                case 41:
                    _ = new Enforcer(player);
                    break;
                case 42:
                    _ = new Morphling(player);
                    break;
                case 43:
                    _ = new Blackmailer(player);
                    break;
                case 44:
                    _ = new Miner(player);
                    break;
                case 45:
                    _ = new Teleporter(player);
                    break;
                case 46:
                    _ = new Wraith(player);
                    break;
                case 47:
                    _ = new Consort(player);
                    break;
                case 48:
                    _ = new Janitor(player);
                    break;
                case 49:
                    _ = new Camouflager(player);
                    break;
                case 50:
                    _ = new Grenadier(player);
                    break;
                case 51:
                    _ = new Poisoner(player);
                    break;
                case 52:
                    _ = new Impostor(player);
                    break;
                case 53:
                    _ = new Consigliere(player);
                    break;
                case 54:
                    _ = new Disguiser(player);
                    break;
                case 55:
                    _ = new Spellslinger(player);
                    break;
                case 56:
                    _ = new Godfather(player);
                    break;
                case 57:
                    _ = new Anarchist(player);
                    break;
                case 58:
                    _ = new Shapeshifter(player);
                    break;
                case 59:
                    _ = new Drunkard(player);
                    break;
                case 60:
                    _ = new Framer(player);
                    break;
                case 61:
                    _ = new Rebel(player);
                    break;
                case 62:
                    _ = new Concealer(player);
                    break;
                case 63:
                    _ = new Warper(player);
                    break;
                case 64:
                    _ = new Bomber(player);
                    break;
                case 65:
                    _ = new Chameleon(player);
                    break;
                case 66:
                    _ = new Guesser(player);
                    break;
                case 67:
                    _ = new Whisperer(player);
                    break;
                case 68:
                    _ = new Retributionist(player);
                    break;
                case 69:
                    _ = new Actor(player);
                    break;
                case 70:
                    _ = new BountyHunter(player);
                    break;
                case 71:
                    _ = new Mystic(player);
                    break;
                case 72:
                    _ = new Seer(player);
                    break;
                case 73:
                    _ = new Necromancer(player);
                    break;
                case 74:
                    _ = new TimeKeeper(player);
                    break;
                case 75:
                    _ = new Ambusher(player);
                    break;
                case 76:
                    _ = new Crusader(player);
                    break;
                case 77:
                    _ = new Silencer(player);
                    break;
            }
        }

        private static void SetAbility(int id, PlayerControl player)
        {
            switch (id)
            {
                case 0:
                case 11:
                case 12:
                case 13:
                    _ = new Assassin(player);
                    break;
                case 1:
                    _ = new Snitch(player);
                    break;
                case 2:
                    _ = new Insider(player);
                    break;
                case 3:
                    _ = new Swapper(player);
                    break;
                case 4:
                    _ = new Multitasker(player);
                    break;
                case 5:
                    _ = new Radar(player);
                    break;
                case 6:
                    _ = new Tiebreaker(player);
                    break;
                case 7:
                    _ = new Torch(player);
                    break;
                case 8:
                    _ = new Underdog(player);
                    break;
                case 9:
                    _ = new Tunneler(player);
                    break;
                case 10:
                    _ = new Ruthless(player);
                    break;
                case 14:
                    _ = new Ninja(player);
                    break;
                case 15:
                    _ = new ButtonBarry(player);
                    break;
                case 16:
                    _ = new Politician(player);
                    break;
            }
        }

        private static void SetObjectifier(int id, PlayerControl player)
        {
            switch (id)
            {
                case 0:
                    _ = new Lovers(player);
                    break;
                case 1:
                    _ = new Rivals(player);
                    break;
                case 2:
                    _ = new Fanatic(player);
                    break;
                case 3:
                    _ = new Corrupted(player);
                    break;
                case 4:
                    _ = new Overlord(player);
                    break;
                case 5:
                    _ = new Allied(player);
                    break;
                case 6:
                    _ = new Traitor(player);
                    break;
                case 7:
                    _ = new Taskmaster(player);
                    break;
                case 8:
                    _ = new Mafia(player);
                    break;
                case 9:
                    _ = new Defector(player);
                    break;
            }
        }

        private static void SetModifier(int id, PlayerControl player)
        {
            switch (id)
            {
                case 0:
                    _ = new Diseased(player);
                    break;
                case 1:
                    _ = new Bait(player);
                    break;
                case 2:
                    _ = new Dwarf(player);
                    break;
                case 3:
                    _ = new VIP(player);
                    break;
                case 4:
                    _ = new Shy(player);
                    break;
                case 5:
                    _ = new Giant(player);
                    break;
                case 6:
                    _ = new Drunk(player);
                    break;
                case 7:
                    _ = new Flincher(player);
                    break;
                case 8:
                    _ = new Coward(player);
                    break;
                case 9:
                    _ = new Volatile(player);
                    break;
                case 10:
                    _ = new Professional(player);
                    break;
                case 11:
                    _ = new Indomitable(player);
                    break;
                case 12:
                    _ = new Astral(player);
                    break;
                case 13:
                    _ = new Yeller(player);
                    break;
            }
        }

        private static void Gen(PlayerControl player, int id, PlayerLayerEnum rpc)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetLayer, SendOption.Reliable);
            writer.Write(id);
            writer.Write(player.PlayerId);
            writer.Write((byte)rpc);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetLayer(id, player, rpc);
        }

        public static void SetLayer(int id, PlayerControl player, PlayerLayerEnum rpc)
        {
            if (rpc == PlayerLayerEnum.Role)
                SetRole(id, player);
            else if (rpc == PlayerLayerEnum.Modifier)
                SetModifier(id, player);
            else if (rpc == PlayerLayerEnum.Objectifier)
                SetObjectifier(id, player);
            else if (rpc == PlayerLayerEnum.Ability)
                SetAbility(id, player);
        }

        public static void AssignChaosDrive()
        {
            if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
                return;

            if (!CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate)))
                return;

            var all = CustomPlayer.AllPlayers.Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (Role.DriveHolder == null || Role.DriveHolder.Data.IsDead || Role.DriveHolder.Data.Disconnected)
            {
                var chosen = all.Find(x => x.Is(RoleEnum.PromotedRebel));

                if (chosen == null)
                {
                    chosen = all.Find(x => x.Is(RoleAlignment.SyndicateDisrup));

                    if (chosen == null)
                    {
                        chosen = all.Find(x => x.Is(RoleAlignment.SyndicateSupport));

                        if (chosen == null)
                        {
                            chosen = all.Find(x => x.Is(RoleAlignment.SyndicatePower));

                            if (chosen == null)
                            {
                                chosen = all.Find(x => x.Is(RoleAlignment.SyndicateKill));

                                if (chosen == null)
                                    chosen = all.Find(x => x.Is(RoleEnum.Anarchist) || x.Is(RoleEnum.Rebel) || x.Is(RoleEnum.Sidekick));
                            }
                        }
                    }
                }

                if (chosen != null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ChaosDrive, SendOption.Reliable);
                    writer.Write(chosen.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Role.DriveHolder = chosen;
                }
            }
        }

        public static void Convert(byte target, byte convert, SubFaction sub, bool condition)
        {
            var converted = Utils.PlayerById(target);
            var converter = Utils.PlayerById(convert);

            if (condition || Convertible <= 0 || PureCrew == converted)
                Utils.Interact(converter, converted, true, true);
            else
            {
                var role1 = Role.GetRole(converted);
                var role2 = Role.GetRole(converter);
                var converts = converted.Is(SubFaction.None);

                if (!converts && !converted.Is(sub))
                    Utils.Interact(converter, converted, true, true);
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
                            ((Jackal)role2).Recruited.Add(target);
                            ((Jackal)role2).BackupRecruit = converted;
                            role1.IsRecruit = true;
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
                        Utils.Flash(flash);
                    else if (CustomPlayer.Local.Is(RoleEnum.Mystic))
                        Utils.Flash(Colors.Mystic);
                }
            }
        }

        public static void RpcConvert(byte target, byte convert, SubFaction sub, bool condition = false)
        {
            Convert(target, convert, sub, condition);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Convert);
            writer.Write(convert);
            writer.Write(target);
            writer.Write((byte)sub);
            writer.Write(condition);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}