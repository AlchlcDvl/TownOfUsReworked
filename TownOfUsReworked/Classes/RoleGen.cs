namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class RoleGen
    {
        #pragma warning disable
        public static List<(int, int, bool)> CrewAuditorRoles = new();
        public static List<(int, int, bool)> CrewInvestigativeRoles = new();
        public static List<(int, int, bool)> CrewKillingRoles = new();
        public static List<(int, int, bool)> CrewProtectiveRoles = new();
        public static List<(int, int, bool)> CrewSovereignRoles = new();
        public static List<(int, int, bool)> CrewSupportRoles = new();
        public static List<(int, int, bool)> CrewRoles = new();

        public static List<(int, int, bool)> NeutralEvilRoles = new();
        public static List<(int, int, bool)> NeutralBenignRoles = new();
        public static List<(int, int, bool)> NeutralKillingRoles = new();
        public static List<(int, int, bool)> NeutralNeophyteRoles = new();
        public static List<(int, int, bool)> NeutralRoles = new();

        public static List<(int, int, bool)> IntruderDeceptionRoles = new();
        public static List<(int, int, bool)> IntruderConcealingRoles = new();
        public static List<(int, int, bool)> IntruderKillingRoles = new();
        public static List<(int, int, bool)> IntruderSupportRoles = new();
        public static List<(int, int, bool)> IntruderRoles = new();

        public static List<(int, int, bool)> SyndicateDisruptionRoles = new();
        public static List<(int, int, bool)> SyndicateKillingRoles = new();
        public static List<(int, int, bool)> SyndicatePowerRoles = new();
        public static List<(int, int, bool)> SyndicateSupportRoles = new();
        public static List<(int, int, bool)> SyndicateRoles = new();

        public static List<(int, int, bool)> AllModifiers = new();
        public static List<(int, int, bool)> AllAbilities = new();
        public static List<(int, int, bool)> AllObjectifiers = new();

        public static PlayerControl PureCrew;
        public static int Convertible;
        #pragma warning restore

        private static List<(int, int, bool)> Sort(this List<(int, int, bool)> items, int max, int min)
        {
            if (items.Count == 0)
                return items;

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
                var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                if (chance == 100)
                    tempList.Add(item);

                if (tempList.Count >= amount)
                    break;
            }

            foreach (var item in items)
            {
                var (chance, _, _) = (item.Item1, item.Item2, item.Item3);

                if (chance < 100)
                {
                    var random = URandom.RandomRangeInt(0, 100);

                    if (random < chance)
                        tempList.Add(item);
                }

                if (tempList.Count >= amount)
                    break;
            }

            tempList.Shuffle();
            return tempList;
        }

        private static List<(int, int, bool)> SpawnSort(this List<(int, int, bool)> items)
        {
            var tempList = new List<(int, int, bool)>();
            var amount = PlayerControl.AllPlayerControls.Count;

            if (items.Count < amount)
                amount = items.Count;

            while (tempList.Count < amount)
            {
                items.Shuffle();
                tempList.Add(items.TakeFirst());
            }

            tempList.Shuffle();
            return tempList;
        }

        private static List<(int, int, bool)> AASort(this List<(int, int, bool)> items, int amount)
        {
            var newList = new List<(int, int, bool)>();

            while (newList.Count < amount && items.Count > 0)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Item3 && CustomGameOptions.EnableUniques)
                    items.Remove(items[0]);

                newList.Shuffle();
            }

            return newList;
        }

        private static void GenVanilla(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            #pragma warning disable
            var spawnList1 = new List<(int, int, bool)>();
            var spawnList2 = new List<(int, int, bool)>();
            #pragma warning restore

            CrewRoles.Clear();
            IntruderRoles.Clear();

            while (CrewRoles.Count < crewmates.Count)
                CrewRoles.Add((100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    IntruderRoles.Add((100, 57, false));
                else
                    IntruderRoles.Add((100, 52, false));
            }

            spawnList1 = CrewRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (_, id, _) = spawnList2.TakeFirst();
                Gen(impostors.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (_, id, _) = spawnList1.TakeFirst();
                Gen(crewmates.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenKilling(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            var spawnList1 = new List<(int, int, bool)>();
            var spawnList2 = new List<(int, int, bool)>();

            CrewRoles.Clear();
            IntruderRoles.Clear();
            SyndicateRoles.Clear();
            NeutralKillingRoles.Clear();

            Utils.LogSomething("Lists Cleared - Killing Only");

            IntruderRoles.Clear();
            IntruderRoles.Add((CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
            IntruderRoles.Add((CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
            IntruderRoles.Add((CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
            IntruderRoles.Add((CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
            IntruderRoles.Add((CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
            IntruderRoles.Add((CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
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
            SyndicateRoles.Add((CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
            SyndicateRoles.Add((CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));

            if (CustomGameOptions.SyndicateCount >= 3)
                SyndicateRoles.Add((CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));

            NeutralKillingRoles.Add((CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
            NeutralKillingRoles.Add((CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
            NeutralKillingRoles.Add((CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
            NeutralKillingRoles.Add((CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
            NeutralKillingRoles.Add((CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
            NeutralKillingRoles.Add((CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));

            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralKillingRoles.Add((CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((CustomGameOptions.PlaguebearerOn, CustomGameOptions.PestSpawn ? 33 : 34, CustomGameOptions.UniquePlaguebearer));

            NeutralKillingRoles.Shuffle();

            int vigis = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;
            int vets = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;

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

            var nonIntruderRoles = new List<(int, int, bool)>();

            nonIntruderRoles.AddRange(CrewRoles);
            nonIntruderRoles.AddRange(NeutralKillingRoles);

            if (!CustomGameOptions.AltImps)
                nonIntruderRoles.AddRange(SyndicateRoles);
            else
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }

            Utils.LogSomething("Ranges Set");

            //Hoping it doesn't come to this
            while (nonIntruderRoles.Count < crewmates.Count)
                nonIntruderRoles.Add((100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
                IntruderRoles.Add((100, CustomGameOptions.AltImps ? 57 : 52, false));

            Utils.LogSomething("Default Roles Entered");

            nonIntruderRoles = nonIntruderRoles.Sort(crewmates.Count, crewmates.Count);
            IntruderRoles = IntruderRoles.Sort(impostors.Count, impostors.Count);

            Utils.LogSomething("Killing Role List Sorted");

            spawnList1 = nonIntruderRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (_, id, _) = spawnList2.TakeFirst();
                Gen(impostors.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (_, id, _) = spawnList1.TakeFirst();
                Gen(crewmates.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            Role.SyndicateHasChaosDrive = true;
            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenClassicCustomAA(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            var allCount = crewmates.Count + impostors.Count;
            var spawnList1 = new List<(int, int, bool)>();
            var spawnList2 = new List<(int, int, bool)>();

            crewmates.Shuffle();
            impostors.Shuffle();

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

            if (CustomGameOptions.SurvivorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SurvivorCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((CustomGameOptions.SurvivorOn, 25, CustomGameOptions.UniqueSurvivor));
                    num--;
                }

                Utils.LogSomething("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
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

            if (CustomGameOptions.JackalOn > 0 && PlayerControl.AllPlayerControls.Count > 5)
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
                    NeutralKillingRoles.Add((CustomGameOptions.PlaguebearerOn, CustomGameOptions.PestSpawn ? 33 : 34, CustomGameOptions.UniquePlaguebearer));
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

            if (ConstantVariables.IsClassic || ConstantVariables.IsCustom)
            {
                if (!CustomGameOptions.AltImps)
                {
                    var minIC = CustomGameOptions.ICMin;
                    var maxIC = CustomGameOptions.ICMax;
                    var minID = CustomGameOptions.IDMin;
                    var maxID = CustomGameOptions.IDMax;
                    var minIK = CustomGameOptions.IKMin;
                    var maxIK = CustomGameOptions.IKMax;
                    var minIS = CustomGameOptions.ISMin;
                    var maxIS = CustomGameOptions.ISMax;
                    var minInt = CustomGameOptions.IntruderMin;
                    var maxInt = CustomGameOptions.IntruderMax;
                    var maxIntSum = maxIC + maxID + maxIK + maxIS;
                    var minIntSum = minIC + minID + minIK + minIS;

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

                    while (minIntSum > minInt)
                    {
                        switch (URandom.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minIC > 0) minIC--;
                                break;

                            case 1:
                                if (minID > 0) minID--;
                                break;

                            case 2:
                                if (minIK > 0) minIK--;
                                break;

                            case 3:
                                if (minIS > 0) minIS--;
                                break;
                        }

                        minIntSum = minIC + minID + minIK + minIS;
                    }

                    IntruderConcealingRoles = IntruderConcealingRoles.Sort(maxIC, minIC);
                    IntruderDeceptionRoles = IntruderDeceptionRoles.Sort(maxID, minID);
                    IntruderKillingRoles = IntruderKillingRoles.Sort(maxIK, minIK);
                    IntruderSupportRoles = IntruderSupportRoles.Sort(maxIS, minIS);

                    IntruderRoles.AddRange(IntruderConcealingRoles);
                    IntruderRoles.AddRange(IntruderDeceptionRoles);
                    IntruderRoles.AddRange(IntruderKillingRoles);
                    IntruderRoles.AddRange(IntruderSupportRoles);

                    while (maxInt > CustomGameOptions.IntruderCount)
                        maxInt--;

                    while (minInt > CustomGameOptions.IntruderCount)
                        minInt--;

                    IntruderRoles = IntruderRoles.Sort(maxInt, minInt);

                    while (IntruderRoles.Count < CustomGameOptions.IntruderCount)
                        IntruderRoles.Add((100, 52, false));

                    IntruderRoles.Shuffle();
                }

                if (CustomGameOptions.NeutralMax > 0)
                {
                    var minNE = CustomGameOptions.NEMin;
                    var maxNE = CustomGameOptions.NEMax;
                    var minNB = CustomGameOptions.NBMin;
                    var maxNB = CustomGameOptions.NBMax;
                    var minNK = CustomGameOptions.NKMin;
                    var maxNK = CustomGameOptions.NKMax;
                    var minNN = CustomGameOptions.NNMin;
                    var maxNN = CustomGameOptions.NNMax;
                    var minNeut = CustomGameOptions.NeutralMin;
                    var maxNeut = CustomGameOptions.NeutralMax;
                    var maxNeutSum = maxNE + maxNB + maxNK + maxNN;
                    var minNeutSum = minNE + minNB + minNK + minNN;

                    while (maxNeutSum > maxNeut)
                    {
                        switch (URandom.RandomRangeInt(0, 4))
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
                        }

                        maxNeutSum = maxNE + maxNB + maxNK + maxNN;
                    }

                    while (minNeutSum > minNeut)
                    {
                        switch (URandom.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minNE > 0) minNE--;
                                break;

                            case 1:
                                if (minNB > 0) minNB--;
                                break;

                            case 2:
                                if (minNK > 0) minNK--;
                                break;

                            case 3:
                                if (minNN > 0) minNN--;
                                break;
                        }

                        minNeutSum = minNE + minNB + minNK + minNN;
                    }

                    NeutralBenignRoles = NeutralBenignRoles.Sort(maxNB, minNB);
                    NeutralEvilRoles = NeutralEvilRoles.Sort(maxNE, minNE);
                    NeutralKillingRoles = NeutralKillingRoles.Sort(maxNK, minNK);
                    NeutralNeophyteRoles = NeutralNeophyteRoles.Sort(maxNN, minNN);

                    NeutralRoles.AddRange(NeutralBenignRoles);
                    NeutralRoles.AddRange(NeutralEvilRoles);
                    NeutralRoles.AddRange(NeutralKillingRoles);
                    NeutralRoles.AddRange(NeutralNeophyteRoles);

                    while (maxNeut >= crewmates.Count)
                        maxNeut--;

                    while (minNeut >= crewmates.Count)
                        minNeut--;

                    NeutralRoles = NeutralRoles.Sort(maxNeut, minNeut);
                }

                if (CustomGameOptions.SyndicateCount > 0)
                {
                    var minSSu = CustomGameOptions.SSuMin;
                    var maxSSu = CustomGameOptions.SSuMax;
                    var minSD = CustomGameOptions.SDMin;
                    var maxSD = CustomGameOptions.SDMax;
                    var minSyK = CustomGameOptions.SyKMin;
                    var maxSyK = CustomGameOptions.SyKMax;
                    var minSP = CustomGameOptions.SPMin;
                    var maxSP = CustomGameOptions.SPMax;
                    var minSyn = CustomGameOptions.SyndicateMin;
                    var maxSyn = CustomGameOptions.SyndicateMax;
                    var maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
                    var minSynSum = minSSu + minSD + minSyK + minSP;

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

                    while (minSynSum > minSyn)
                    {
                        switch (URandom.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minSSu > 0) minSSu--;
                                break;

                            case 1:
                                if (minSD > 0) minSD--;
                                break;

                            case 2:
                                if (minSyK > 0) minSyK--;
                                break;

                            case 3:
                                if (minSP > 0) minSP--;
                                break;
                        }

                        minSynSum = minSSu + minSD + minSyK + minSP;
                    }

                    SyndicateSupportRoles = SyndicateSupportRoles.Sort(maxSSu, minSSu);
                    SyndicateDisruptionRoles = SyndicateDisruptionRoles.Sort(maxSD, minSD);
                    SyndicateKillingRoles = SyndicateKillingRoles.Sort(maxSyK, minSyK);
                    SyndicatePowerRoles = SyndicatePowerRoles.Sort(maxSP, minSP);

                    SyndicateRoles.AddRange(SyndicateSupportRoles);
                    SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                    SyndicateRoles.AddRange(SyndicateKillingRoles);
                    SyndicateRoles.AddRange(SyndicatePowerRoles);

                    while (maxSyn > CustomGameOptions.SyndicateCount || maxSyn >= crewmates.Count - NeutralRoles.Count)
                        maxSyn--;

                    while (minSyn > CustomGameOptions.SyndicateCount || minSyn >= crewmates.Count - NeutralRoles.Count)
                        minSyn--;

                    SyndicateRoles = SyndicateRoles.Sort(maxSyn, minSyn);

                    while (SyndicateRoles.Count < CustomGameOptions.SyndicateCount)
                        SyndicateRoles.Add((100, 57, false));

                    SyndicateRoles.Shuffle();
                }

                if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
                {
                    var minCI = CustomGameOptions.CIMin;
                    var maxCI = CustomGameOptions.CIMax;
                    var minCS = CustomGameOptions.CSMin;
                    var maxCS = CustomGameOptions.CSMax;
                    var minCA = CustomGameOptions.CAMin;
                    var maxCA = CustomGameOptions.CAMax;
                    var minCK = CustomGameOptions.CKMin;
                    var maxCK = CustomGameOptions.CKMax;
                    var minCP = CustomGameOptions.CPMin;
                    var maxCP = CustomGameOptions.CPMax;
                    var minCSv = CustomGameOptions.CSvMin;
                    var maxCSv = CustomGameOptions.CSvMax;
                    var minCrew = CustomGameOptions.CrewMin;
                    var maxCrew = CustomGameOptions.CrewMax;
                    var maxCrewSum = maxCA + maxCI + maxCK + maxCP + maxCS + maxCSv;
                    var minCrewSum = minCA + minCI + minCK + minCP + minCS + minCSv;

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

                    while (minCrewSum > minCrew)
                    {
                        switch (URandom.RandomRangeInt(0, 6))
                        {
                            case 0:
                                if (minCA > 0) minCA--;
                                break;

                            case 1:
                                if (minCI > 0) minCI--;
                                break;

                            case 2:
                                if (minCK > 0) minCK--;
                                break;

                            case 3:
                                if (minCP > 0) minCP--;
                                break;

                            case 4:
                                if (minCS > 0) minCS--;
                                break;

                            case 5:
                                if (minCSv > 0) minCSv--;
                                break;
                        }

                        minCrewSum = minCA + minCI + minCK + minCP + minCS + minCSv;
                    }

                    CrewAuditorRoles = CrewAuditorRoles.Sort(maxCA, minCA);
                    CrewInvestigativeRoles = CrewInvestigativeRoles.Sort(maxCI, minCI);
                    CrewKillingRoles = CrewKillingRoles.Sort(maxCK, minCK);
                    CrewProtectiveRoles = CrewProtectiveRoles.Sort(maxCP, minCP);
                    CrewSupportRoles = CrewSupportRoles.Sort(maxCS, minCS);
                    CrewSovereignRoles = CrewSovereignRoles.Sort(maxCSv, minCSv);

                    CrewRoles.AddRange(CrewAuditorRoles);
                    CrewRoles.AddRange(CrewInvestigativeRoles);
                    CrewRoles.AddRange(CrewKillingRoles);
                    CrewRoles.AddRange(CrewSupportRoles);
                    CrewRoles.AddRange(CrewProtectiveRoles);
                    CrewRoles.AddRange(CrewSovereignRoles);

                    while (maxCrew > crewmates.Count - SyndicateRoles.Count - NeutralRoles.Count)
                        maxCrew--;

                    while (minCrew > crewmates.Count - SyndicateRoles.Count - NeutralRoles.Count)
                        minCrew--;

                    CrewRoles = CrewRoles.Sort(maxCrew, minCrew);

                    while (CrewRoles.Count < crewmates.Count - CustomGameOptions.SyndicateCount - NeutralRoles.Count)
                        CrewRoles.Add((100, 20, false));

                    CrewRoles.Shuffle();
                }

                Utils.LogSomething("Classic/Custom Sorting Done");
            }
            else if (ConstantVariables.IsAA)
            {
                CrewRoles.AddRange(CrewAuditorRoles);
                CrewRoles.AddRange(CrewInvestigativeRoles);
                CrewRoles.AddRange(CrewKillingRoles);
                CrewRoles.AddRange(CrewSupportRoles);
                CrewRoles.AddRange(CrewProtectiveRoles);
                CrewRoles.AddRange(CrewSovereignRoles);

                SyndicateRoles.AddRange(SyndicateSupportRoles);
                SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                SyndicateRoles.AddRange(SyndicateKillingRoles);
                SyndicateRoles.AddRange(SyndicatePowerRoles);

                NeutralRoles.AddRange(NeutralBenignRoles);
                NeutralRoles.AddRange(NeutralEvilRoles);
                NeutralRoles.AddRange(NeutralKillingRoles);
                NeutralRoles.AddRange(NeutralNeophyteRoles);

                IntruderRoles.AddRange(IntruderConcealingRoles);
                IntruderRoles.AddRange(IntruderDeceptionRoles);
                IntruderRoles.AddRange(IntruderKillingRoles);
                IntruderRoles.AddRange(IntruderSupportRoles);

                if (CustomGameOptions.AltImps)
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }

                CrewRoles.Shuffle();
                SyndicateRoles.Shuffle();
                IntruderRoles.Shuffle();
                NeutralRoles.Shuffle();

                Utils.LogSomething("All Any Sorting Done");
            }

            var NonIntruderRoles = new List<(int, int, bool)>();
            NonIntruderRoles.AddRange(CrewRoles);
            NonIntruderRoles.AddRange(NeutralRoles);

            if (CustomGameOptions.AltImps)
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }
            else
                NonIntruderRoles.AddRange(SyndicateRoles);

            spawnList1 = ConstantVariables.IsAA ? NonIntruderRoles.AASort(crewmates.Count) : NonIntruderRoles;
            spawnList2 = ConstantVariables.IsAA ? IntruderRoles.AASort(impostors.Count) : IntruderRoles;

            while (spawnList1.Count < crewmates.Count)
                spawnList1.Add((100, 20, false));

            while (spawnList2.Count < impostors.Count)
                spawnList2.Add((100, CustomGameOptions.AltImps ? 57 : 52, false));

            if (!spawnList1.Any(CrewRoles.Contains) && !spawnList1.Contains((100, 20, false)))
            {
                spawnList1.Remove(spawnList1.Random());

                if (CrewRoles.Count > 0)
                    spawnList1.Add(CrewRoles.Random());
                else
                    spawnList1.Add((100, 20, false));

                Utils.LogSomething("Added Solo Crew");
            }

            spawnList1.Shuffle();
            spawnList2.Shuffle();

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (_, id, _) = spawnList2.TakeFirst();
                Gen(impostors.TakeFirst(), id, PlayerLayerEnum.Role);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (_, id, _) = spawnList1.TakeFirst();
                Gen(crewmates.TakeFirst(), id, PlayerLayerEnum.Role);
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

            var allCount = PlayerControl.AllPlayerControls.Count;
            AllAbilities = AllAbilities.Sort(CustomGameOptions.MaxAbilities, CustomGameOptions.MinAbilities);

            var canHaveIntruderAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveNeutralAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveCrewAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveSyndicateAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveTunnelerAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveSnitch = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveTaskedAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveTorch = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveEvilAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveKillingAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveAbility = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveBB = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHavePolitician = PlayerControl.AllPlayerControls.Il2CppToSystem();

            canHaveIntruderAbility.RemoveAll(player => !player.Is(Faction.Intruder) || (player.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
            canHaveIntruderAbility.Shuffle();

            canHaveNeutralAbility.RemoveAll(player => !(player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleAlignment.NeutralKill) || player.Is(RoleAlignment.NeutralPros)));
            canHaveNeutralAbility.Shuffle();

            canHaveCrewAbility.RemoveAll(player => !player.Is(Faction.Crew));
            canHaveCrewAbility.Shuffle();

            canHaveSyndicateAbility.RemoveAll(player => !player.Is(Faction.Syndicate));
            canHaveSyndicateAbility.Shuffle();

            canHaveTunnelerAbility.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(RoleEnum.Engineer));
            canHaveTunnelerAbility.Shuffle();

            canHaveSnitch.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(ObjectifierEnum.Traitor));
            canHaveSnitch.Shuffle();

            canHaveTaskedAbility.RemoveAll(player => !player.CanDoTasks());
            canHaveTaskedAbility.Shuffle();

            canHaveTorch.RemoveAll(player => (player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NKHasImpVision) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) &&
                !CustomGameOptions.LightsAffectNeutrals) || player.Is(Faction.Intruder));
            canHaveTorch.Shuffle();

            canHaveEvilAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)));
            canHaveEvilAbility.Shuffle();

            canHaveKillingAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(RoleAlignment.NeutralNeo) ||
                player.Is(RoleAlignment.NeutralKill)));
            canHaveKillingAbility.Shuffle();

            canHaveBB.RemoveAll(player => (player.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (player.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (player.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (player.Is(RoleEnum.Guesser) && !CustomGameOptions.GuesserButton) || (player.Is(RoleEnum.Executioner) &&
                !CustomGameOptions.ExecutionerButton) || (!CustomGameOptions.MonarchButton && player.Is(RoleEnum.Monarch)) || (!CustomGameOptions.DictatorButton &&
                player.Is(RoleEnum.Dictator)));
            canHaveBB.Shuffle();

            canHavePolitician.RemoveAll(player => player.Is(RoleAlignment.NeutralEvil) || player.Is(RoleAlignment.NeutralBen) || player.Is(RoleAlignment.NeutralNeo));
            canHavePolitician.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllAbilities, allCount) : AllAbilities;
            spawnList = spawnList.SpawnSort();

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

            if (CustomGameOptions.LoversOn > 0 && PlayerControl.AllPlayerControls.Count > 4)
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

            if (CustomGameOptions.RivalsOn > 0 && PlayerControl.AllPlayerControls.Count > 3)
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

            var allCount = PlayerControl.AllPlayerControls.Count;
            AllObjectifiers = AllObjectifiers.Sort(CustomGameOptions.MaxObjectifiers, CustomGameOptions.MinObjectifiers);

            var canHaveLoverorRival = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveNeutralObjectifier = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveCrewObjectifier = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveAllied = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveObjectifier = PlayerControl.AllPlayerControls.Il2CppToSystem();

            canHaveLoverorRival.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll) || player.Is(RoleEnum.Actor) || player.Is(RoleEnum.Jester) ||
                player.Is(RoleEnum.Shifter) || player == PureCrew);
            canHaveLoverorRival.Shuffle();

            canHaveNeutralObjectifier.RemoveAll(player => !player.Is(Faction.Neutral) || player == PureCrew);
            canHaveNeutralObjectifier.Shuffle();

            canHaveCrewObjectifier.RemoveAll(player => !player.Is(Faction.Crew) || player == PureCrew);
            canHaveCrewObjectifier.Shuffle();

            canHaveAllied.RemoveAll(player => !player.Is(RoleAlignment.NeutralKill) || player == PureCrew);
            canHaveAllied.Shuffle();

            canHaveObjectifier.RemoveAll(player => player == PureCrew);
            canHaveObjectifier.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllObjectifiers, allCount) : AllObjectifiers;
            spawnList = spawnList.SpawnSort();

            while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 4 || canHaveObjectifier.Count > 1)
            {
                if (spawnList.Count == 0)
                    break;

                var (chance, id, unique) = spawnList.TakeFirst();
                int[] LoverRival = { 0, 1 };
                int[] Crew = { 2, 3, 6 };
                int[] Neutral = { 4, 7 };
                int[] Allied = { 5 };
                int[] Global = { 8 };

                PlayerControl assigned = null;

                if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 4)
                    assigned = canHaveLoverorRival.TakeFirst();
                else if (Crew.Contains(id) && canHaveCrewObjectifier.Count > 0)
                    assigned = canHaveCrewObjectifier.TakeFirst();
                else if (Neutral.Contains(id) && canHaveNeutralObjectifier.Count > 0)
                    assigned = canHaveNeutralObjectifier.TakeFirst();
                else if (Allied.Contains(id) && canHaveAllied.Count > 0)
                    assigned = canHaveAllied.TakeFirst();
                else if (Global.Contains(id) && canHaveObjectifier.Count > 1)
                    assigned = canHaveObjectifier.TakeFirst();

                if (assigned != null)
                {
                    canHaveLoverorRival.Remove(assigned);
                    canHaveCrewObjectifier.Remove(assigned);
                    canHaveNeutralObjectifier.Remove(assigned);
                    canHaveAllied.Remove(assigned);
                    canHaveObjectifier.Remove(assigned);

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

            var allCount = PlayerControl.AllPlayerControls.Count;
            AllModifiers = AllModifiers.Sort(CustomGameOptions.MaxModifiers, CustomGameOptions.MinModifiers);

            var canHaveBait = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveDiseased = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveProfessional = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveModifier = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveShy = PlayerControl.AllPlayerControls.Il2CppToSystem();
            var canHaveIndomitable = PlayerControl.AllPlayerControls.Il2CppToSystem();

            canHaveBait.RemoveAll(player => player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Thief) || player.Is(RoleEnum.Altruist) ||
                player.Is(RoleEnum.Troll));
            canHaveBait.Shuffle();

            canHaveDiseased.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll));
            canHaveDiseased.Shuffle();

            canHaveProfessional.RemoveAll(player => !player.Is(AbilityEnum.Assassin));
            canHaveProfessional.Shuffle();

            canHaveIndomitable.RemoveAll(player => player.Is(RoleEnum.Actor));
            canHaveIndomitable.Shuffle();

            canHaveShy.RemoveAll(player => (player.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (player.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (player.Is(AbilityEnum.Swapper) && !CustomGameOptions.SwapperButton) || (player.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (player.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (player.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || player.Is(AbilityEnum.ButtonBarry) ||
                (player.Is(AbilityEnum.Politician) && !CustomGameOptions.PoliticianButton) || (!CustomGameOptions.DictatorButton && player.Is(RoleEnum.Dictator)) ||
                (!CustomGameOptions.MonarchButton && player.Is(RoleEnum.Monarch)));
            canHaveShy.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllModifiers, allCount) : AllModifiers;
            spawnList = spawnList.SpawnSort();

            while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (chance, id, unique) = spawnList.TakeFirst();
                int[] Bait = { 1 };
                int[] Diseased = { 0 };
                int[] Professional = { 10 };
                int[] Global = { 2, 3, 5, 6, 7, 8, 9 };
                int[] Shy = { 4 };
                int[] Indomitable = { 11 };

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
                else if (canHaveIndomitable.Count > 0 && Indomitable.Contains(id))
                    assigned = canHaveIndomitable.TakeFirst();

                if (assigned != null)
                {
                    canHaveBait.Remove(assigned);
                    canHaveDiseased.Remove(assigned);
                    canHaveProfessional.Remove(assigned);
                    canHaveModifier.Remove(assigned);
                    canHaveShy.Remove(assigned);
                    canHaveIndomitable.Remove(assigned);

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
                        ally.Color = Colors.Crew;
                    }
                    else if (intr)
                    {
                        alliedRole.Faction = Faction.Intruder;
                        alliedRole.FactionColor = Colors.Intruder;
                        ally.Color = Colors.Intruder;
                    }
                    else if (syn)
                    {
                        alliedRole.Faction = Faction.Syndicate;
                        alliedRole.FactionColor = Colors.Syndicate;
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

            var exeTargets = new List<PlayerControl>();
            var gaTargets = new List<PlayerControl>();
            var guessTargets = new List<PlayerControl>();
            var goodRecruits = new List<PlayerControl>();
            var evilRecruits = new List<PlayerControl>();
            var bhTargets = new List<PlayerControl>();
            var lovers = new List<PlayerControl>();
            var rivals = new List<PlayerControl>();
            var mafia = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(ObjectifierEnum.Lovers))
                    lovers.Add(player);
                else if (player.Is(ObjectifierEnum.Rivals))
                    rivals.Add(player);
                else if (player.Is(ObjectifierEnum.Mafia))
                    mafia.Add(player);
            }

            if (CustomGameOptions.LoversOn > 0)
            {
                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (lover.OtherLover != null || !lovers.Contains(lover.Player))
                        continue;

                    if (lovers.Count > 0)
                    {
                        while (lover.OtherLover == null || lover.OtherLover == lover.Player || (lover.Player.GetFaction() == lover.OtherLover.GetFaction() &&
                            !CustomGameOptions.LoversFaction))
                        {
                            lover.OtherLover = lovers.Random();
                        }

                        lovers.Remove(lover.OtherLover);
                        lovers.Remove(lover.Player);
                        Objectifier.GetObjectifier<Lovers>(lover.OtherLover).OtherLover = lover.Player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetCouple);
                        writer.Write(lover.PlayerId);
                        writer.Write(lover.OtherLover.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Lovers = {lover.Player.name} & {lover.OtherLover.name}");
                    }
                }

                foreach (var lover in Objectifier.GetObjectifiers<Lovers>(ObjectifierEnum.Lovers))
                {
                    if (lover.OtherLover == null || lovers.Contains(lover.Player))
                    {
                        _ = new Objectifierless(lover.Player);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                        writer.Write(lover.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        lover.Player = null;
                    }
                }

                Utils.LogSomething("Lovers Set");
            }

            if (CustomGameOptions.RivalsOn > 0)
            {
                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (rival.OtherRival != null || !rivals.Contains(rival.Player))
                        continue;

                    if (rivals.Count > 0)
                    {
                        while (rival.OtherRival == null || rival.OtherRival == rival.Player || (rival.Player.GetFaction() == rival.OtherRival.GetFaction() &&
                            !CustomGameOptions.RivalsFaction))
                        {
                            rival.OtherRival = rivals.Random();
                        }

                        rivals.Remove(rival.OtherRival);
                        rivals.Remove(rival.Player);
                        Objectifier.GetObjectifier<Rivals>(rival.OtherRival).OtherRival = rival.Player;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetDuo);
                        writer.Write(rival.PlayerId);
                        writer.Write(rival.OtherRival.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Rivals = {rival.Player.name} & {rival.OtherRival.name}");
                    }
                }

                foreach (var rival in Objectifier.GetObjectifiers<Rivals>(ObjectifierEnum.Rivals))
                {
                    if (rival.OtherRival == null || rivals.Contains(rival.Player))
                    {
                        _ = new Objectifierless(rival.Player);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                        writer.Write(rival.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        rival.Player = null;
                    }
                }

                Utils.LogSomething("Rivals Set");
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (Modifier.GetModifier(player) == null)
                {
                    _ = new Modifierless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullModifier, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (Ability.GetAbility(player) == null)
                {
                    _ = new Abilityless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullAbility, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (Objectifier.GetObjectifier(player) == null)
                {
                    _ = new Objectifierless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullObjectifier, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            Utils.LogSomething("Layers Nulled");

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew))
                {
                    if (!player.Is(RoleEnum.Altruist))
                        gaTargets.Add(player);

                    if (player != PureCrew)
                        goodRecruits.Add(player);

                    if (!player.Is(RoleAlignment.CrewSov))
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Neutral))
                {
                    if (!player.Is(RoleEnum.Executioner) && !player.Is(RoleEnum.Troll) && !player.Is(RoleEnum.GuardianAngel) && !player.Is(RoleEnum.Jester))
                    {
                        gaTargets.Add(player);

                        if (player.Is(RoleAlignment.NeutralKill))
                            evilRecruits.Add(player);
                    }

                    if (CustomGameOptions.ExeCanHaveNeutralTargets)
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                {
                    gaTargets.Add(player);

                    if (Objectifier.GetObjectifier(player).ObjectifierType == ObjectifierEnum.None)
                        evilRecruits.Add(player);

                    if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                        exeTargets.Add(player);
                }

                guessTargets.Add(player);
                bhTargets.Add(player);
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
                {
                    exe.TargetPlayer = null;

                    if (exeTargets.Count > 0)
                    {
                        while (exe.TargetPlayer == null || exe.TargetPlayer == exe.Player)
                        {
                            exeTargets.Shuffle();
                            var exeNum = URandom.RandomRangeInt(0, exeTargets.Count);
                            exe.TargetPlayer = exeTargets[exeNum];
                        }

                        exeTargets.Remove(exe.TargetPlayer);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetExeTarget);
                        writer.Write(exe.PlayerId);
                        writer.Write(exe.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Exe Target Set");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                foreach (var guess in Role.GetRoles<Guesser>(RoleEnum.Guesser))
                {
                    guess.TargetPlayer = null;

                    if (guessTargets.Count > 0)
                    {
                        while (guess.TargetPlayer == null || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(ModifierEnum.Indomitable))
                        {
                            guessTargets.Shuffle();
                            var guessNum = URandom.RandomRangeInt(0, guessTargets.Count);
                            guess.TargetPlayer = guessTargets[guessNum];
                        }

                        guessTargets.Remove(guess.TargetPlayer);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetGuessTarget);
                        writer.Write(guess.PlayerId);
                        writer.Write(guess.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Guess Target Set");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
                {
                    ga.TargetPlayer = null;

                    if (gaTargets.Count > 0)
                    {
                        while (ga.TargetPlayer == null || ga.TargetPlayer == ga.Player)
                        {
                            gaTargets.Shuffle();
                            var gaNum = URandom.RandomRangeInt(0, gaTargets.Count);
                            ga.TargetPlayer = gaTargets[gaNum];
                        }

                        gaTargets.Remove(ga.TargetPlayer);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetGATarget);
                        writer.Write(ga.PlayerId);
                        writer.Write(ga.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"GA Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("GA Target Set");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
                {
                    bh.TargetPlayer = null;

                    if (bhTargets.Count > 0)
                    {
                        while (bh.TargetPlayer == null || bh.TargetPlayer == bh.Player)
                        {
                            bhTargets.Shuffle();
                            var bhNum = URandom.RandomRangeInt(0, gaTargets.Count);
                            bh.TargetPlayer = bhTargets[bhNum];
                        }

                        bhTargets.Remove(bh.TargetPlayer);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetBHTarget);
                        writer.Write(bh.PlayerId);
                        writer.Write(bh.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"BH Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("BH Target Set");
            }

            if (CustomGameOptions.ActorOn > 0)
            {
                foreach (var act in Role.GetRoles<Actor>(RoleEnum.Actor))
                {
                    act.PretendRoles = InspectorResults.None;

                    while (act.PretendRoles == InspectorResults.None)
                    {
                        var actNum = URandom.RandomRangeInt(0, 17);
                        act.PretendRoles = (InspectorResults)actNum;
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                    writer.Write((byte)TargetRPC.SetActPretendList);
                    writer.Write(act.PlayerId);
                    writer.Write((byte)act.PretendRoles);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
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

                    if (goodRecruits.Count > 0)
                    {
                        while (jackal.GoodRecruit == null || jackal.GoodRecruit == jackal.Player)
                        {
                            goodRecruits.Shuffle();
                            var goodNum = URandom.RandomRangeInt(0, goodRecruits.Count - 1);
                            jackal.GoodRecruit = goodRecruits[goodNum];
                        }

                        goodRecruits.Remove(jackal.GoodRecruit);
                        Role.GetRole(jackal.GoodRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(jackal.GoodRecruit).SubFactionColor = Colors.Cabal;
                        Role.GetRole(jackal.GoodRecruit).IsRecruit = true;
                        jackal.Recruited.Add(jackal.GoodRecruit.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetGoodRecruit);
                        writer.Write(jackal.PlayerId);
                        writer.Write(jackal.GoodRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Good Recruit = {jackal.GoodRecruit.name}");
                    }

                    if (evilRecruits.Count > 0)
                    {
                        while (jackal.EvilRecruit == null || jackal.EvilRecruit == jackal.Player)
                        {
                            evilRecruits.Shuffle();
                            var evilNum = URandom.RandomRangeInt(0, evilRecruits.Count - 1);
                            jackal.EvilRecruit = evilRecruits[evilNum];
                        }

                        evilRecruits.Remove(jackal.EvilRecruit);
                        Role.GetRole(jackal.EvilRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(jackal.EvilRecruit).IsRecruit = true;
                        jackal.Recruited.Add(jackal.EvilRecruit.PlayerId);
                        Role.GetRole(jackal.EvilRecruit).SubFactionColor = Colors.Cabal;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
                        writer.Write((byte)TargetRPC.SetEvilRecruit);
                        writer.Write(jackal.PlayerId);
                        writer.Write(jackal.EvilRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Evil Recruit = {jackal.EvilRecruit.name}");
                    }
                }

                Utils.LogSomething("Jackal Recruits Set");
            }

            foreach (var player in PlayerControl.AllPlayerControls)
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
            }
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

            Murder.KilledPlayers.Clear();

            Role.Buttons.Clear();
            Role.SetColors();

            UpdateNames.PlayerNames.Clear();

            ConfirmEjects.LastExiled = null;

            Role.AllRoles.Clear();
            Objectifier.AllObjectifiers.Clear();
            Modifier.AllModifiers.Clear();
            Ability.AllAbilities.Clear();
            PlayerLayer.AllLayers.Clear();

            SetPostmortals.AssassinatedPlayers.Clear();

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

            Tasks.AllCustomPlateform.Clear();
            Tasks.NearestTask = null;

            Utils.RecentlyKilled.Clear();

            DisconnectHandler.Disconnected.Clear();

            CustomButton.AllButtons.Clear();

            Ash.AllPiles.Clear();
            Objects.Range.AllItems.Clear();
        }

        public static void BeginRoleGen(List<GameData.PlayerInfo> infected)
        {
            if (ConstantVariables.IsHnS)
                return;

            if (ConstantVariables.IsKilling)
                GenKilling(infected.ToList());
            else if (ConstantVariables.IsVanilla)
                GenVanilla(infected.ToList());
            else
                GenClassicCustomAA(infected.ToList());

            PureCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew)).ToList().Random();

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

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (Role.GetRole(player) == null)
                {
                    _ = new Roleless(player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NullRole, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            Convertible = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(SubFaction.None) && x != PureCrew);
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
                case 75:
                    _ = new Ambusher(player);
                    break;
                case 76:
                    _ = new Crusader(player);
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
            if (rpc == PlayerLayerEnum.Modifier)
                SetModifier(id, player);
            if (rpc == PlayerLayerEnum.Objectifier)
                SetObjectifier(id, player);
            if (rpc == PlayerLayerEnum.Ability)
                SetAbility(id, player);
        }

        public static void AssignChaosDrive()
        {
            if (CustomGameOptions.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
                return;

            var list = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.Syndicate));

            if (!list.Any())
                return;

            var all = PlayerControl.AllPlayerControls.Il2CppToSystem();

            #pragma warning disable
            if (Role.DriveHolder == null || Role.DriveHolder.Data.IsDead || Role.DriveHolder.Data.Disconnected)
            {
                var chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.PromotedRebel));

                if (chosen == null)
                {
                    chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleAlignment.SyndicateDisruption));

                    if (chosen == null)
                    {
                        chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleAlignment.SyndicateSupport));

                        if (chosen == null)
                        {
                            chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleAlignment.SyndicatePower));

                            if (chosen == null)
                            {
                                chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleAlignment.SyndicateKill));

                                if (chosen == null)
                                    chosen = all.Find(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Anarchist) || x.Is(RoleEnum.Rebel) || x.Is(RoleEnum.Sidekick)));
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
            #pragma warning restore
        }

        public static void Convert(byte target, byte convert, SubFaction sub, bool condition)
        {
            var converted = Utils.PlayerById(target);
            var converter = Utils.PlayerById(convert);

            if (condition || Convertible <= 0 || PureCrew == converted)
                Utils.MurderPlayer(converter, converted, DeathReasonEnum.Failed, true);
            else
            {
                var role1 = Role.GetRole(converted);
                var role2 = Role.GetRole(converter);
                var converts = converted.Is(SubFaction.None);

                if (!converts && !converted.Is(sub))
                    Utils.MurderPlayer(converter, converted, DeathReasonEnum.Failed, true);
                else
                {
                    var flash = sub switch
                    {
                        SubFaction.Undead => Colors.Undead,
                        SubFaction.Cabal => Colors.Cabal,
                        SubFaction.Reanimated => Colors.Reanimated,
                        SubFaction.Sect => Colors.Sect,
                        _ => Colors.Stalemate
                    };

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

                    role1.SubFaction = sub;
                    role1.SubFactionColor = flash;
                    Convertible--;

                    if (PlayerControl.LocalPlayer == converted)
                        Utils.Flash(flash);

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                        Utils.Flash(Colors.Mystic);
                }
            }
        }
    }
}