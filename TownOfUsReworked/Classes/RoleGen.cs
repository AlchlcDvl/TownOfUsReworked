using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.AlliedMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class RoleGen
    {
        private static readonly List<(Type, int, int, bool)> CrewAuditorRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewInvestigativeRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewProtectiveRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewSovereignRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewRoles = new();

        private static readonly List<(Type, int, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralNeophyteRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralRoles = new();

        private static readonly List<(Type, int, int, bool)> IntruderDeceptionRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderConcealingRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderRoles = new();

        private static readonly List<(Type, int, int, bool)> SyndicateDisruptionRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicatePowerRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateRoles = new();

        private static readonly List<(Type, int, int, bool)> AllModifiers = new();
        private static readonly List<(Type, int, int, bool)> AllAbilities = new();
        private static readonly List<(Type, int, int, bool)> AllObjectifiers = new();

        #pragma warning disable
        public static bool PhantomOn;
        public static bool RevealerOn;
        public static bool BansheeOn;
        public static bool GhoulOn;
        #pragma warning restore

        private static void Sort(List<(Type, int, int, bool)> items, int max, int min)
        {
            if (items.Count == 0)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > max)
                (max, min) = (min, max);

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min < 0)
                min = 0;

            var amount = Random.RandomRangeInt(min, max + 1);
            var tempList = new List<(Type, int, int, bool)>();

            foreach (var item in items)
            {
                var (_, chance, _, _) = (item.Item1, item.Item2, item.Item3, item.Item4);

                if (chance == 100)
                {
                    tempList.Add(item);
                }
                else
                {
                    var random = Random.RandomRangeInt(0, 100);

                    if (random < chance)
                        tempList.Add(item);
                }

                if (tempList.Count >= amount)
                    break;
            }

            tempList.Shuffle();
        }

        private static List<(Type, int, int, bool)> AASort(List<(Type, int, int, bool)> items, int amount)
        {
            var newList = new List<(Type, int, int, bool)>();

            while (newList.Count < amount && items.Count > 0)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Item4 && CustomGameOptions.EnableUniques)
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
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            CrewRoles.Clear();
            IntruderRoles.Clear();

            while (CrewRoles.Count < crewmates.Count)
                CrewRoles.Add((typeof(Crewmate), 100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    IntruderRoles.Add((typeof(Anarchist), 100, 57, false));
                else
                    IntruderRoles.Add((typeof(Impostor), 100, 52, false));
            }

            spawnList1 = CrewRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, _) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, _) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenKilling(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            #pragma warning disable
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            CrewRoles.Clear();
            IntruderRoles.Clear();
            SyndicateRoles.Clear();
            NeutralKillingRoles.Clear();

            Utils.LogSomething("Lists Cleared - Killing Only");

            if (!CustomGameOptions.AltImps)
            {
                IntruderRoles.Clear();
                IntruderRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, 41, CustomGameOptions.UniqueUndertaker));
                IntruderRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                IntruderRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                IntruderRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                IntruderRoles.Add((typeof(Wraith), CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                IntruderRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                IntruderRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                IntruderRoles.Add((typeof(TimeMaster), CustomGameOptions.TimeMasterOn, 55, CustomGameOptions.UniqueTimeMaster));
                IntruderRoles.Add((typeof(Disguiser), CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                IntruderRoles.Add((typeof(Consigliere), CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                IntruderRoles.Add((typeof(Consort), CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                IntruderRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                IntruderRoles.Add((typeof(Camouflager), CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                IntruderRoles.Add((typeof(Teleporter), CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                IntruderRoles.Add((typeof(Ambusher), CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
                IntruderRoles.Add((typeof(Impostor), 5, 52, false));

                if (CustomGameOptions.IntruderCount >= 3)
                    IntruderRoles.Add((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
            }

            SyndicateRoles.Add((typeof(Anarchist), 5, 57, false));
            SyndicateRoles.Add((typeof(Concealer), CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
            SyndicateRoles.Add((typeof(Shapeshifter), CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
            SyndicateRoles.Add((typeof(Warper), CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
            SyndicateRoles.Add((typeof(Gorgon), CustomGameOptions.GorgonOn, 59, CustomGameOptions.UniqueGorgon));
            SyndicateRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
            SyndicateRoles.Add((typeof(Framer), CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
            SyndicateRoles.Add((typeof(Crusader), CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));

            if (CustomGameOptions.SyndicateCount >= 3)
                SyndicateRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));

            NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
            NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
            NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
            NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
            NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
            NeutralKillingRoles.Add((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));

            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralKillingRoles.Add((typeof(Cryomaniac), CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
            {
                if (CustomGameOptions.PestSpawn)
                    NeutralKillingRoles.Add((typeof(Pestilence), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer));
                else
                    NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 34, CustomGameOptions.UniquePlaguebearer));
            }

            NeutralKillingRoles.Shuffle();

            int vigis = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;
            int vets = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;

            while (vigis > 0 || vets > 0)
            {
                if (vigis > 0)
                {
                    CrewRoles.Add((typeof(Vigilante), 100, 3, false));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add((typeof(Veteran), 100, 11, false));
                    vets--;
                }
            }

            Utils.LogSomething("Lists Set - Killing Only");

            var nonIntruderRoles = new List<(Type, int, int, bool)>();

            nonIntruderRoles.AddRange(CrewRoles);
            nonIntruderRoles.AddRange(NeutralKillingRoles);

            if (!CustomGameOptions.AltImps)
            {
                nonIntruderRoles.AddRange(SyndicateRoles);
            }
            else
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }

            Utils.LogSomething("Ranges Set");

            //Hoping it doesn't come to this
            while (nonIntruderRoles.Count < crewmates.Count)
                nonIntruderRoles.Add((typeof(Crewmate), 100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    IntruderRoles.Add((typeof(Anarchist), 100, 57, false));
                else
                    IntruderRoles.Add((typeof(Impostor), 100, 52, false));
            }

            Utils.LogSomething("Default Roles Entered");

            Sort(nonIntruderRoles, crewmates.Count, crewmates.Count);
            Sort(IntruderRoles, impostors.Count, impostors.Count);

            Utils.LogSomething("Killing Role List Sorted");

            spawnList1 = nonIntruderRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, _) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, _) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenClassicCustomAA(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            var allCount = crewmates.Count + impostors.Count;

            #pragma warning disable
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            crewmates.Shuffle();
            impostors.Shuffle();

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

            PhantomOn = Utils.Check(CustomGameOptions.PhantomOn);
            RevealerOn = Utils.Check(CustomGameOptions.RevealerOn);
            BansheeOn = Utils.Check(CustomGameOptions.BansheeOn);
            GhoulOn = Utils.Check(CustomGameOptions.GhoulOn);

            var num = 0;

            if (CustomGameOptions.MayorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MayorCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, 0, CustomGameOptions.UniqueMayor));
                    num--;
                }

                Utils.LogSomething("Mayor Done");
            }

            if (CustomGameOptions.SheriffOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SheriffCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, 1, CustomGameOptions.UniqueSheriff));
                    num--;
                }

                Utils.LogSomething("Sheriff Done");
            }

            if (CustomGameOptions.InspectorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.InspectorCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Inspector), CustomGameOptions.InspectorOn, 2, CustomGameOptions.UniqueInspector));
                    num--;
                }

                Utils.LogSomething("Inspector Done");
            }

            if (CustomGameOptions.VigilanteOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VigilanteCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, 3, CustomGameOptions.UniqueVigilante));
                    num--;
                }

                Utils.LogSomething("Vigilante Done");
            }

            if (CustomGameOptions.EngineerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.EngineerCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, 4, CustomGameOptions.UniqueEngineer));
                    num--;
                }

                Utils.LogSomething("Engineer Done");
            }

            if (CustomGameOptions.SwapperOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SwapperCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, 5, CustomGameOptions.UniqueSwapper));
                    num--;
                }

                Utils.LogSomething("Swapper Done");
            }

            if (CustomGameOptions.TimeLordOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TimeLordCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(TimeLord), CustomGameOptions.TimeLordOn, 7, CustomGameOptions.UniqueTimeLord));
                    num--;
                }

                Utils.LogSomething("Time Lord Done");
            }

            if (CustomGameOptions.MedicOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MedicCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, 8, CustomGameOptions.UniqueMedic));
                    num--;
                }

                Utils.LogSomething("Medic Done");
            }

            if (CustomGameOptions.AgentOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AgentCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Agent), CustomGameOptions.AgentOn, 9, CustomGameOptions.UniqueAgent));
                    num--;
                }

                Utils.LogSomething("Agent Done");
            }

            if (CustomGameOptions.AltruistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AltruistCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, 10, CustomGameOptions.UniqueAltruist));
                    num--;
                }

                Utils.LogSomething("Altruist Done");
            }

            if (CustomGameOptions.VeteranOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VeteranCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, 11, CustomGameOptions.UniqueVeteran));
                    num--;
                }

                Utils.LogSomething("Veteran Done");
            }

            if (CustomGameOptions.TrackerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TrackerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, 12, CustomGameOptions.UniqueTracker));
                    num--;
                }

                Utils.LogSomething("Tracker Done");
            }

            if (CustomGameOptions.TransporterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TransporterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, 13, CustomGameOptions.UniqueTransporter));
                    num--;
                }

                Utils.LogSomething("Transporter Done");
            }

            if (CustomGameOptions.MediumOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MediumCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, 14, CustomGameOptions.UniqueMedium));
                    num--;
                }

                Utils.LogSomething("Medium Done");
            }

            if (CustomGameOptions.CoronerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CoronerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Coroner), CustomGameOptions.CoronerOn, 15, CustomGameOptions.UniqueCoroner));
                    num--;
                }

                Utils.LogSomething("Coroner Done");
            }

            if (CustomGameOptions.OperativeOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.OperativeCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Operative), CustomGameOptions.OperativeOn, 16, CustomGameOptions.UniqueOperative));
                    num--;
                }

                Utils.LogSomething("Operative Done");
            }

            if (CustomGameOptions.DetectiveOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DetectiveCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, 17, CustomGameOptions.UniqueDetective));
                    num--;
                }

                Utils.LogSomething("Detective Done");
            }

            if (CustomGameOptions.EscortOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.EscortCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Escort), CustomGameOptions.EscortOn, 18, CustomGameOptions.UniqueEscort));
                    num--;
                }

                Utils.LogSomething("Escort Done");
            }

            if (CustomGameOptions.ShifterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShifterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Shifter), CustomGameOptions.ShifterOn, 19, CustomGameOptions.UniqueShifter));
                    num--;
                }

                Utils.LogSomething("Shifter Done");
            }

            if (CustomGameOptions.ChameleonOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ChameleonCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Chameleon), CustomGameOptions.ChameleonOn, 65, CustomGameOptions.UniqueChameleon));
                    num--;
                }

                Utils.LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.RetributionistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RetributionistCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Retributionist), CustomGameOptions.RetributionistOn, 68, CustomGameOptions.UniqueRetributionist));
                    num--;
                }

                Utils.LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.CrewmateOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.CrewCount;

                while (num > 0)
                {
                    CrewRoles.Add((typeof(Crewmate), CustomGameOptions.CrewmateOn, 20, false));
                    num--;
                }

                Utils.LogSomething("Crewmate Done");
            }

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VampireHunterCount : 1;

                while (num > 0)
                {
                    CrewAuditorRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter));
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
                    CrewAuditorRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, 71, CustomGameOptions.UniqueMystic));
                    num--;
                }

                Utils.LogSomething("Mystic Done");
            }

            if (CustomGameOptions.SeerOn > 0 && ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.BountyHunterOn > 0 ||
                CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||
                CustomGameOptions.TraitorOn > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 ||
                CustomGameOptions.GuardianAngelOn > 0 || CustomGameOptions.GuesserOn > 0))
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SeerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, 72, CustomGameOptions.UniqueSeer));
                    num--;
                }

                Utils.LogSomething("Seer Done");
            }

            if (CustomGameOptions.JesterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JesterCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, 22, CustomGameOptions.UniqueJester));
                    num--;
                }

                Utils.LogSomething("Jester Done");
            }

            if (CustomGameOptions.AmnesiacOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AmnesiacCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, 23, CustomGameOptions.UniqueAmnesiac));
                    num--;
                }

                Utils.LogSomething("Amnesiac Done");
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ExecutionerCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, 24, CustomGameOptions.UniqueExecutioner));
                    num--;
                }

                Utils.LogSomething("Executioner Done");
            }

            if (CustomGameOptions.SurvivorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SurvivorCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, 25, CustomGameOptions.UniqueSurvivor));
                    num--;
                }

                Utils.LogSomething("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GuardianAngelCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, 26, CustomGameOptions.UniqueGuardianAngel));
                    num--;
                }

                Utils.LogSomething("Guardian Angel Done");
            }

            if (CustomGameOptions.GlitchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GlitchCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
                    num--;
                }

                Utils.LogSomething("Glitch Done");
            }

            if (CustomGameOptions.MurdererOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MurdCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
                    num--;
                }

                Utils.LogSomething("Murderer Done");
            }

            if (CustomGameOptions.CryomaniacOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CryomaniacCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Cryomaniac), CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));
                    num--;
                }

                Utils.LogSomething("Cryomaniac Done");
            }

            if (CustomGameOptions.WerewolfOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WerewolfCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
                    num--;
                }

                Utils.LogSomething("Werewolf Done");
            }

            if (CustomGameOptions.ArsonistOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ArsonistCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));
                    num--;
                }

                Utils.LogSomething("Arsonist Done");
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JackalCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Jackal), CustomGameOptions.JackalOn, 32, CustomGameOptions.UniqueJackal));
                    num--;
                }

                Utils.LogSomething("Jackal Done");
            }

            if (CustomGameOptions.NecromancerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.NecromancerCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Necromancer), CustomGameOptions.NecromancerOn, 73, CustomGameOptions.UniqueNecromancer));
                    num--;
                }

                Utils.LogSomething("Necromancer Done");
            }

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.PlaguebearerCount : 1;

                while (num > 0)
                {
                    if (CustomGameOptions.PestSpawn)
                        NeutralKillingRoles.Add((typeof(Pestilence), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer));
                    else
                        NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 34, CustomGameOptions.UniquePlaguebearer));

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
                    NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
                    num--;
                }

                Utils.LogSomething("Serial Killer Done");
            }

            if (CustomGameOptions.JuggernautOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JuggernautCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
                    num--;
                }

                Utils.LogSomething("Juggeraut Done");
            }

            if (CustomGameOptions.CannibalOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CannibalCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Cannibal), CustomGameOptions.CannibalOn, 37, CustomGameOptions.UniqueCannibal));
                    num--;
                }

                Utils.LogSomething("Cannibal Done");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GuesserCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Guesser), CustomGameOptions.GuesserOn, 66, CustomGameOptions.UniqueGuesser));
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
                    NeutralEvilRoles.Add((typeof(Actor), CustomGameOptions.ActorOn, 69, CustomGameOptions.UniqueActor));
                    num--;
                }

                Utils.LogSomething("Actor Done");
            }

            if (CustomGameOptions.ThiefOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ThiefCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));
                    num--;
                }

                Utils.LogSomething("Thief Done");
            }

            if (CustomGameOptions.DraculaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DraculaCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Dracula), CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula));
                    num--;
                }

                Utils.LogSomething("Dracula Done");
            }

            if (CustomGameOptions.WhispererOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WhispererCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Whisperer), CustomGameOptions.WhispererOn, 67, CustomGameOptions.UniqueWhisperer));
                    num--;
                }

                Utils.LogSomething("Whisperer Done");
            }

            if (CustomGameOptions.TrollOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TrollCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Troll), CustomGameOptions.TrollOn, 40, CustomGameOptions.UniqueTroll));
                    num--;
                }

                Utils.LogSomething("Troll Done");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BHCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(BountyHunter), CustomGameOptions.BountyHunterOn, 70, CustomGameOptions.UniqueBountyHunter));
                    num--;
                }

                Utils.LogSomething("Bounty Hunter Done");
            }

            if (CustomGameOptions.UndertakerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.UndertakerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, 41, CustomGameOptions.UniqueUndertaker));
                    num--;
                }

                Utils.LogSomething("Undertaker Done");
            }

            if (CustomGameOptions.MorphlingOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MorphlingCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                    num--;
                }

                Utils.LogSomething("Morphling Done");
            }

            if (CustomGameOptions.BlackmailerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BlackmailerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                    num--;
                }

                Utils.LogSomething("Blackmailer Done");
            }

            if (CustomGameOptions.MinerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MinerCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                    num--;
                }

                Utils.LogSomething("Miner Done");
            }

            if (CustomGameOptions.TeleporterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TeleporterCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Teleporter), CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                    num--;
                }

                Utils.LogSomething("Teleporter Done");
            }

            if (CustomGameOptions.AmbusherOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AmbusherCount : 1;

                while (num > 0)
                {
                    IntruderKillingRoles.Add((typeof(Ambusher), CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
                    num--;
                }

                Utils.LogSomething("Ambusher Done");
            }

            if (CustomGameOptions.WraithOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WraithCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Wraith), CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                    num--;
                }

                Utils.LogSomething("Wraith Done");
            }

            if (CustomGameOptions.ConsortOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConsortCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Consort), CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                    num--;
                }

                Utils.LogSomething("Consort Done");
            }

            if (CustomGameOptions.JanitorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.JanitorCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                    num--;
                }

                Utils.LogSomething("Janitor Done");
            }

            if (CustomGameOptions.CamouflagerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CamouflagerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Camouflager), CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                    num--;
                }

                Utils.LogSomething("Camouflager Done");
            }

            if (CustomGameOptions.GrenadierOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GrenadierCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                    num--;
                }

                Utils.LogSomething("Grenadier Done");
            }

            if (CustomGameOptions.ImpostorOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.ImpCount;

                while (num > 0)
                {
                    IntruderRoles.Add((typeof(Impostor), CustomGameOptions.ImpostorOn, 52, false));
                    num--;
                }

                Utils.LogSomething("Impostor Done");
            }

            if (CustomGameOptions.ConsigliereOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConsigliereCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Consigliere), CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                    num--;
                }

                Utils.LogSomething("Consigliere Done");
            }

            if (CustomGameOptions.DisguiserOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DisguiserCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Disguiser), CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                    num--;
                }

                Utils.LogSomething("Disguiser Done");
            }

            if (CustomGameOptions.TimeMasterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TimeMasterCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(TimeMaster), CustomGameOptions.TimeMasterOn, 55, CustomGameOptions.UniqueTimeMaster));
                    num--;
                }

                Utils.LogSomething("Time Master Done");
            }

            if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GodfatherCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
                    num--;
                }

                Utils.LogSomething("Godfather Done");
            }

            if (CustomGameOptions.AnarchistOn > 0 && ConstantVariables.IsCustom)
            {
                num = CustomGameOptions.AnarchistCount;

                while (num > 0)
                {
                    SyndicateRoles.Add((typeof(Anarchist), CustomGameOptions.AnarchistOn, 57, false));
                    num--;
                }

                Utils.LogSomething("Anarchist Done");
            }

            if (CustomGameOptions.ShapeshifterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShapeshifterCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Shapeshifter), CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
                    num--;
                }

                Utils.LogSomething("Shapeshifter Done");
            }

            if (CustomGameOptions.GorgonOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GorgonCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Gorgon), CustomGameOptions.GorgonOn, 59, CustomGameOptions.UniqueGorgon));
                    num--;
                }

                Utils.LogSomething("Gorgon Done");
            }

            if (CustomGameOptions.FramerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FramerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Framer), CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
                    num--;
                }

                Utils.LogSomething("Framer Done");
            }

            if (CustomGameOptions.CrusaderOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CrusaderCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Crusader), CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));
                    num--;
                }

                Utils.LogSomething("Crusader Done");
            }

            if (CustomGameOptions.RebelOn > 0 && CustomGameOptions.SyndicateCount >= 3)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RebelCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));
                    num--;
                }

                Utils.LogSomething("Rebel Done");
            }

            if (CustomGameOptions.BeamerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BeamerCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Beamer), CustomGameOptions.BeamerOn, 74, CustomGameOptions.UniqueBeamer));
                    num--;
                }

                Utils.LogSomething("Beamer Done");
            }

            if (CustomGameOptions.PoisonerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.PoisonerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                    num--;
                }

                Utils.LogSomething("Poisoner Done");
            }

            if (CustomGameOptions.ConcealerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ConcealerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Concealer), CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
                    num--;
                }

                Utils.LogSomething("Concealer Done");
            }

            if (CustomGameOptions.WarperOn > 0 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 4 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.WarperCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Warper), CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
                    num--;
                }

                Utils.LogSomething("Warper Done");
            }

            if (CustomGameOptions.BomberOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BomberCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
                    num--;
                }

                Utils.LogSomething("Bomber Done");
            }

            if (CustomGameOptions.DrunkardOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DrunkardCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Drunkard), CustomGameOptions.DrunkardOn, 6, CustomGameOptions.UniqueDrunkard));
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
                        switch (Random.RandomRangeInt(0, 4))
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
                        switch (Random.RandomRangeInt(0, 4))
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

                    Sort(IntruderConcealingRoles, maxIC, minIC);
                    Sort(IntruderDeceptionRoles, maxID, minID);
                    Sort(IntruderKillingRoles, maxIK, minIK);
                    Sort(IntruderSupportRoles, maxIS, minIS);

                    IntruderRoles.AddRange(IntruderConcealingRoles);
                    IntruderRoles.AddRange(IntruderDeceptionRoles);
                    IntruderRoles.AddRange(IntruderKillingRoles);
                    IntruderRoles.AddRange(IntruderSupportRoles);

                    while (maxInt > CustomGameOptions.IntruderCount)
                        maxInt--;

                    while (minInt > CustomGameOptions.IntruderCount)
                        minInt--;

                    Sort(IntruderRoles, maxInt, minInt);

                    while (IntruderRoles.Count < CustomGameOptions.IntruderCount)
                        IntruderRoles.Add((typeof(Impostor), 100, 52, false));

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
                        switch (Random.RandomRangeInt(0, 4))
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
                        switch (Random.RandomRangeInt(0, 4))
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

                    Sort(NeutralBenignRoles, maxNB, minNB);
                    Sort(NeutralEvilRoles, maxNE, minNE);
                    Sort(NeutralKillingRoles, maxNK, minNK);
                    Sort(NeutralNeophyteRoles, maxNN, minNN);

                    NeutralRoles.AddRange(NeutralBenignRoles);
                    NeutralRoles.AddRange(NeutralEvilRoles);
                    NeutralRoles.AddRange(NeutralKillingRoles);
                    NeutralRoles.AddRange(NeutralNeophyteRoles);

                    while (maxNeut >= crewmates.Count)
                        maxNeut--;

                    while (minNeut >= crewmates.Count)
                        minNeut--;

                    Sort(NeutralRoles, maxNeut, minNeut);
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
                        switch (Random.RandomRangeInt(0, 4))
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
                        switch (Random.RandomRangeInt(0, 4))
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

                    Sort(SyndicateSupportRoles, maxSSu, minSSu);
                    Sort(SyndicateDisruptionRoles, maxSD, minSD);
                    Sort(SyndicateKillingRoles, maxSyK, minSyK);
                    Sort(SyndicatePowerRoles, maxSP, minSP);

                    SyndicateRoles.AddRange(SyndicateSupportRoles);
                    SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                    SyndicateRoles.AddRange(SyndicateKillingRoles);
                    SyndicateRoles.AddRange(SyndicatePowerRoles);

                    while (maxSyn > CustomGameOptions.SyndicateCount || maxSyn >= crewmates.Count - NeutralRoles.Count)
                        maxSyn--;

                    while (minSyn > CustomGameOptions.SyndicateCount || minSyn >= crewmates.Count - NeutralRoles.Count)
                        minSyn--;

                    Sort(SyndicateRoles, maxSyn, minSyn);

                    while (SyndicateRoles.Count < CustomGameOptions.SyndicateCount)
                        SyndicateRoles.Add((typeof(Anarchist), 100, 57, false));

                    SyndicateRoles.Shuffle();
                }

                if (CustomGameOptions.CrewMax > 0)
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
                        switch (Random.RandomRangeInt(0, 6))
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
                        switch (Random.RandomRangeInt(0, 6))
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

                    Sort(CrewAuditorRoles, maxCA, minCA);
                    Sort(CrewInvestigativeRoles, maxCI, minCI);
                    Sort(CrewKillingRoles, maxCK, minCK);
                    Sort(CrewProtectiveRoles, maxCP, minCP);
                    Sort(CrewSupportRoles, maxCS, minCS);
                    Sort(CrewSovereignRoles, maxCSv, minCSv);

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

                    Sort(CrewRoles, maxCrew, minCrew);

                    while (CrewRoles.Count < crewmates.Count - CustomGameOptions.SyndicateCount - NeutralRoles.Count)
                        CrewRoles.Add((typeof(Crewmate), 100, 20, false));

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

                if (!CustomGameOptions.AltImps)
                {
                    IntruderRoles.AddRange(IntruderConcealingRoles);
                    IntruderRoles.AddRange(IntruderDeceptionRoles);
                    IntruderRoles.AddRange(IntruderKillingRoles);
                    IntruderRoles.AddRange(IntruderSupportRoles);
                }
                else
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

            var NonIntruderRoles = new List<(Type, int, int, bool)>();

            NonIntruderRoles.AddRange(CrewRoles);
            NonIntruderRoles.AddRange(NeutralRoles);

            if (!CustomGameOptions.AltImps)
                NonIntruderRoles.AddRange(SyndicateRoles);
            else
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }

            spawnList1 = ConstantVariables.IsAA ? AASort(NonIntruderRoles, crewmates.Count) : NonIntruderRoles;
            spawnList2 = ConstantVariables.IsAA ? AASort(IntruderRoles, impostors.Count) : IntruderRoles;

            while (spawnList1.Count < crewmates.Count)
                spawnList1.Add((typeof(Crewmate), 100, 20, false));

            while (spawnList2.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    spawnList2.Add((typeof(Anarchist), 100, 57, false));
                else
                    spawnList2.Add((typeof(Impostor), 100, 52, false));
            }

            spawnList1.Shuffle();
            spawnList2.Shuffle();

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, unique) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, unique) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");

            var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && !x.Is(ObjectifierEnum.Allied)).ToList();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable);

            if (PhantomOn && toChooseFromNeut.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                var pc = toChooseFromNeut[rand];
                SetPhantom.WillBePhantom = pc;
                writer.Write(pc.PlayerId);
            }
            else
                writer.Write(byte.MaxValue);

            AmongUsClient.Instance.FinishRpcImmediately(writer);

            var toChooseFromSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate)).ToList();
            var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable);

            if (BansheeOn && toChooseFromSyn.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromSyn.Count);
                var pc = toChooseFromSyn[rand];
                SetBanshee.WillBeBanshee = pc;
                writer3.Write(pc.PlayerId);
            }
            else
                writer3.Write(byte.MaxValue);

            AmongUsClient.Instance.FinishRpcImmediately(writer3);

            var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Traitor) && !x.Is(ObjectifierEnum.Corrupted) &&
                !x.Is(ObjectifierEnum.Fanatic)).ToList();
            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable);

            if (RevealerOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];
                SetRevealer.WillBeRevealer = pc;
                writer2.Write(pc.PlayerId);
            }
            else
                writer2.Write(byte.MaxValue);

            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            var toChooseFromInt = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)).ToList();
            var writer4 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable);

            if (GhoulOn && toChooseFromInt.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromInt.Count);
                var pc = toChooseFromInt[rand];
                SetGhoul.WillBeGhoul = pc;
                writer4.Write(pc.PlayerId);
            }
            else
                writer4.Write(byte.MaxValue);

            AmongUsClient.Instance.FinishRpcImmediately(writer4);

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
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.CrewAssassinOn, 0, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Crew Assassin Done");
            }

            if (CustomGameOptions.SyndicateAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfSyndicateAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.SyndicateAssassinOn, 12, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Syndicate Assassin Done");
            }

            if (CustomGameOptions.IntruderAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfImpostorAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.IntruderAssassinOn, 11, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Intruder Assassin Done");
            }

            if (CustomGameOptions.NeutralAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfNeutralAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.NeutralAssassinOn, 14, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Neutral Assassin Done");
            }

            if (CustomGameOptions.RuthlessOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RuthlessCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Ruthless), CustomGameOptions.RuthlessOn, 10, CustomGameOptions.UniqueRuthless));
                    num--;
                }

                Utils.LogSomething("Ruthless Done");
            }

            if (CustomGameOptions.SnitchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.SnitchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Snitch), CustomGameOptions.SnitchOn, 1, CustomGameOptions.UniqueSnitch));
                    num--;
                }

                Utils.LogSomething("Snitch Done");
            }

            if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.InsiderCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Insider), CustomGameOptions.InsiderOn, 2, CustomGameOptions.UniqueInsider));
                    num--;
                }

                Utils.LogSomething("Insider Done");
            }

            if (CustomGameOptions.MultitaskerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.MultitaskerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn, 4, CustomGameOptions.UniqueMultitasker));
                    num--;
                }

                Utils.LogSomething("Multitasker Done");
            }

            if (CustomGameOptions.RadarOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RadarCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Radar), CustomGameOptions.RadarOn, 5, CustomGameOptions.UniqueRadar));
                    num--;
                }

                Utils.LogSomething("Radar Done");
            }

            if (CustomGameOptions.TiebreakerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TiebreakerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn, 6, CustomGameOptions.UniqueTiebreaker));
                    num--;
                }

                Utils.LogSomething("Tiebreaker Done");
            }

            if (CustomGameOptions.TorchOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TorchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Torch), CustomGameOptions.TorchOn, 7, CustomGameOptions.UniqueTorch));
                    num--;
                }

                Utils.LogSomething("Torch Done");
            }

            if (CustomGameOptions.UnderdogOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.UnderdogCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Underdog), CustomGameOptions.UnderdogOn, 8, CustomGameOptions.UniqueUnderdog));
                    num--;
                }

                Utils.LogSomething("Underdog Done");
            }

            if (CustomGameOptions.TunnelerOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TunnelerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Tunneler), CustomGameOptions.TunnelerOn, 9, CustomGameOptions.UniqueTunneler));
                    num--;
                }

                Utils.LogSomething("Tunneler Done");
            }

            if (CustomGameOptions.NinjaOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.NinjaCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Ninja), CustomGameOptions.NinjaOn, 14, CustomGameOptions.UniqueNinja));
                    num--;
                }

                Utils.LogSomething("Ninja Done");
            }

            if (CustomGameOptions.ButtonBarryOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ButtonBarryCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn, 15, CustomGameOptions.UniqueButtonBarry));
                    num--;
                }

                Utils.LogSomething("Button Barry Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllAbilities, CustomGameOptions.MaxAbilities, CustomGameOptions.MinAbilities);
            AllAbilities.Shuffle();

            var canHaveIntruderAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveNeutralAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveCrewAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveSyndicateAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveTunnelerAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveSnitch = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveTaskedAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveTorch = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveEvilAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveKillingAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveBB = PlayerControl.AllPlayerControls.ToArray().ToList();

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

            canHaveTorch.RemoveAll(player => !(player.Is(Faction.Crew) || player.Is(RoleAlignment.NeutralBen) || player.Is(RoleAlignment.NeutralEvil) || (player.Is(Faction.Neutral) &&
                CustomGameOptions.LightsAffectNeutrals) || (player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NKHasImpVision)));
            canHaveTorch.Shuffle();

            canHaveEvilAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)));
            canHaveEvilAbility.Shuffle();

            canHaveKillingAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(RoleAlignment.NeutralNeo) ||
                player.Is(RoleAlignment.NeutralKill)));
            canHaveKillingAbility.Shuffle();

            canHaveBB.RemoveAll(player => (player.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (player.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapperButton) || (player.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (player.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (player.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton));
            canHaveBB.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllAbilities, allCount) : AllAbilities;
            spawnList.Shuffle();

            while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
                canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
                canHaveTaskedAbility.Count > 0 || canHaveTorch.Count > 0 || canHaveKillingAbility.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, unique) = spawnList.TakeFirst();
                int[] Snitch = { 1 };
                int[] Syndicate = { 12 };
                int[] Crew = { 0 };
                int[] Neutral = { 13 };
                int[] Intruder = { 11 };
                int[] Killing = { 10, 14 };
                int[] Torch = { 7 };
                int[] Evil = { 8 };
                int[] Tasked = { 2, 4 };
                int[] Global = { 5, 6 };
                int[] Tunneler = { 9 };
                int[] BB = { 15 };

                if (canHaveSnitch.Count > 0 && Snitch.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveSnitch.TakeFirst(), id);
                else if (canHaveSyndicateAbility.Count > 0 && Syndicate.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveSyndicateAbility.TakeFirst(), id);
                else if (canHaveCrewAbility.Count > 0 && Crew.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveCrewAbility.TakeFirst(), id);
                else if (canHaveNeutralAbility.Count > 0 && Neutral.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveNeutralAbility.TakeFirst(), id);
                else if (canHaveIntruderAbility.Count > 0 && Intruder.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveIntruderAbility.TakeFirst(), id);
                else if (canHaveKillingAbility.Count > 0 && Killing.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveKillingAbility.TakeFirst(), id);
                else if (canHaveTorch.Count > 0 && Torch.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveTorch.TakeFirst(), id);
                else if (canHaveEvilAbility.Count > 0 && Evil.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveEvilAbility.TakeFirst(), id);
                else if (canHaveTaskedAbility.Count > 0 && Tasked.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveTaskedAbility.TakeFirst(), id);
                else if (canHaveAbility.Count > 0 && Global.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveAbility.TakeFirst(), id);
                else if (canHaveTunnelerAbility.Count > 0 && Tunneler.Contains(id) && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
                    Ability.GenAbility<Ability>(type, canHaveTunnelerAbility.TakeFirst(), id);
                else if (canHaveBB.Count > 0 && BB.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveBB.TakeFirst(), id);
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
                    AllObjectifiers.Add((typeof(Lovers), CustomGameOptions.LoversOn, 0, CustomGameOptions.UniqueLovers));
                    num--;
                }

                Utils.LogSomething("Lovers Done");
            }

            if (CustomGameOptions.RivalsOn > 0 && PlayerControl.AllPlayerControls.Count > 4)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.RivalsCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Rivals), CustomGameOptions.RivalsOn, 1, CustomGameOptions.UniqueRivals));
                    num--;
                }

                Utils.LogSomething("Rivals Done");
            }

            if (CustomGameOptions.FanaticOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FanaticCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Fanatic), CustomGameOptions.FanaticOn, 2, CustomGameOptions.UniqueFanatic));
                    num--;
                }

                Utils.LogSomething("Fanatic Done");
            }

            if (CustomGameOptions.CorruptedOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CorruptedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Corrupted), CustomGameOptions.CorruptedOn, 3, CustomGameOptions.UniqueCorrupted));
                    num--;
                }

                Utils.LogSomething("Corrupted Done");
            }

            if (CustomGameOptions.OverlordOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.OverlordCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Overlord), CustomGameOptions.OverlordOn, 4, CustomGameOptions.UniqueOverlord));
                    num--;
                }

                Utils.LogSomething("Overlord Done");
            }

            if (CustomGameOptions.AlliedOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.AlliedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Allied), CustomGameOptions.AlliedOn, 5, CustomGameOptions.UniqueAllied));
                    num--;
                }

                Utils.LogSomething("Allied Done");
            }

            if (CustomGameOptions.TraitorOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TraitorCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Traitor), CustomGameOptions.TraitorOn, 6, CustomGameOptions.UniqueTraitor));
                    num--;
                }

                Utils.LogSomething("Traitor Done");
            }

            if (CustomGameOptions.TaskmasterOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.TaskmasterCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Taskmaster), CustomGameOptions.TaskmasterOn, 7, CustomGameOptions.UniqueTaskmaster));
                    num--;
                }

                Utils.LogSomething("Taskmaster Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllObjectifiers, CustomGameOptions.MaxObjectifiers, CustomGameOptions.MinObjectifiers);
            AllObjectifiers.Shuffle();

            var canHaveLoverorRival = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveNeutralObjectifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveCrewObjectifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAllied = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveLoverorRival.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll) || player.Is(RoleEnum.Actor) || player.Is(RoleEnum.Jester) ||
                player.Is(RoleEnum.Shifter));
            canHaveLoverorRival.Shuffle();

            canHaveNeutralObjectifier.RemoveAll(player => !player.Is(Faction.Neutral));
            canHaveNeutralObjectifier.Shuffle();

            canHaveCrewObjectifier.RemoveAll(player => !player.Is(Faction.Crew));
            canHaveCrewObjectifier.Shuffle();

            canHaveAllied.RemoveAll(player => !player.Is(RoleAlignment.NeutralKill));
            canHaveAllied.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllObjectifiers, allCount) : AllObjectifiers;
            spawnList.Shuffle();

            while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 4)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, _) = spawnList.TakeFirst();
                int[] LoverRival = { 0, 1 };
                int[] Crew = { 2, 3, 6 };
                int[] Neutral = { 4, 7 };
                int[] Allied = { 5 };

                if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 4)
                {
                    if (id == 0)
                        Lovers.Gen(canHaveLoverorRival);
                    else if (id == 1)
                        Rivals.Gen(canHaveLoverorRival);
                }
                else if (Crew.Contains(id) && canHaveCrewObjectifier.Count > 0)
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveCrewObjectifier.TakeFirst(), id);
                else if (Neutral.Contains(id) && canHaveNeutralObjectifier.Count > 0)
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveNeutralObjectifier.TakeFirst(), id);
                else if (Allied.Contains(id) && canHaveAllied.Count > 0)
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveAllied.TakeFirst(), id);
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
                    AllModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn, 0, CustomGameOptions.UniqueDiseased));
                    num--;
                }

                Utils.LogSomething("Diseased Done");
            }

            if (CustomGameOptions.BaitOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.BaitCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn, 1, CustomGameOptions.UniqueBait));
                    num--;
                }

                Utils.LogSomething("Bait Done");
            }

            if (CustomGameOptions.DwarfOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DwarfCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Dwarf), CustomGameOptions.DwarfOn, 2, CustomGameOptions.UniqueDwarf));
                    num--;
                }

                Utils.LogSomething("Dwarf Done");
            }

            if (CustomGameOptions.VIPOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VIPCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(VIP), CustomGameOptions.VIPOn, 3, CustomGameOptions.UniqueVIP));
                    num--;
                }

                Utils.LogSomething("VIP Done");
            }

            if (CustomGameOptions.ShyOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ShyCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Shy), CustomGameOptions.ShyOn, 4, CustomGameOptions.UniqueShy));
                    num--;
                }

                Utils.LogSomething("Shy Done");
            }

            if (CustomGameOptions.GiantOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.GiantCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn, 5, CustomGameOptions.UniqueGiant));
                    num--;
                }

                Utils.LogSomething("Giant Done");
            }

            if (CustomGameOptions.DrunkOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.DrunkCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Drunk), CustomGameOptions.DrunkOn, 6, CustomGameOptions.UniqueDrunk));
                    num--;
                }

                Utils.LogSomething("Drunk Done");
            }

            if (CustomGameOptions.FlincherOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.FlincherCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Flincher), CustomGameOptions.FlincherOn, 7, CustomGameOptions.UniqueFlincher));
                    num--;
                }

                Utils.LogSomething("Flincher Done");
            }

            if (CustomGameOptions.CowardOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.CowardCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Coward), CustomGameOptions.CowardOn, 8, CustomGameOptions.UniqueCoward));
                    num--;
                }

                Utils.LogSomething("Coward Done");
            }

            if (CustomGameOptions.VolatileOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.VolatileCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Volatile), CustomGameOptions.VolatileOn, 9, CustomGameOptions.UniqueVolatile));
                    num--;
                }

                Utils.LogSomething("Volatile Done");
            }

            if (CustomGameOptions.IndomitableOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.IndomitableCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Indomitable), CustomGameOptions.IndomitableOn, 11, CustomGameOptions.UniqueIndomitable));
                    num--;
                }

                Utils.LogSomething("Indomitable Done");
            }

            if (CustomGameOptions.ProfessionalOn > 0)
            {
                num = ConstantVariables.IsCustom ? CustomGameOptions.ProfessionalCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Professional), CustomGameOptions.ProfessionalOn, 10, CustomGameOptions.UniqueProfessional));
                    num--;
                }

                Utils.LogSomething("Professional Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllModifiers, CustomGameOptions.MaxModifiers, CustomGameOptions.MinModifiers);
            AllModifiers.Shuffle();

            var canHaveBait = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveDiseased = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveProfessional = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveShy = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveIndomitable = PlayerControl.AllPlayerControls.ToArray().ToList();

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
                (player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapperButton) || (player.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (player.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (player.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) || player.Is(AbilityEnum.ButtonBarry));
            canHaveShy.Shuffle();

            var spawnList = ConstantVariables.IsAA ? AASort(AllModifiers, allCount) : AllModifiers;
            spawnList.Shuffle();

            while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, _) = spawnList.TakeFirst();
                int[] Bait = { 1 };
                int[] Diseased = { 0 };
                int[] Professional = { 10 };
                int[] Global = { 2, 3, 5, 6, 7, 8, 9 };
                int[] Shy = { 4 };
                int[] Indomitable = { 11 };

                if (canHaveBait.Count > 0 && Bait.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveBait.TakeFirst(), id);
                else if (canHaveDiseased.Count > 0 && Diseased.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveDiseased.TakeFirst(), id);
                else if (canHaveProfessional.Count > 0 && Professional.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveProfessional.TakeFirst(), id);
                else if (canHaveModifier.Count > 0 && Global.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveModifier.TakeFirst(), id);
                else if (canHaveShy.Count > 0 && Shy.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveShy.TakeFirst(), id);
                else if (canHaveIndomitable.Count > 0 && Indomitable.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveIndomitable.TakeFirst(), id);
            }

            Utils.LogSomething("Modifiers Done");
        }

        private static void SetTargets()
        {
            if (CustomGameOptions.AlliedOn > 0)
            {
                foreach (var ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied).Cast<Allied>())
                {
                    var alliedRole = Role.GetRole(ally.Player);
                    var crew = false;
                    var intr = false;
                    var syn = false;

                    if (CustomGameOptions.AlliedFaction == AlliedFaction.Intruder)
                        intr = true;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Syndicate)
                        syn = true;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Crew)
                        crew = true;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Random)
                    {
                        var random = Random.RandomRangeInt(0, 3);
                        intr = random == 0;
                        syn = random == 1;
                        crew = random == 2;
                    }

                    if (crew)
                    {
                        alliedRole.Faction = Faction.Crew;
                        alliedRole.IsCrewAlly = true;
                        alliedRole.FactionColor = Colors.Crew;
                        ally.Color = Colors.Crew;
                    }
                    else if (intr)
                    {
                        alliedRole.Faction = Faction.Intruder;
                        alliedRole.IsIntAlly = true;
                        alliedRole.FactionColor = Colors.Intruder;
                        ally.Color = Colors.Intruder;
                    }
                    else if (syn)
                    {
                        alliedRole.Faction = Faction.Syndicate;
                        alliedRole.IsSynAlly = true;
                        alliedRole.FactionColor = Colors.Syndicate;
                        ally.Color = Colors.Syndicate;
                    }

                    ally.Side = alliedRole.Faction;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAlliedFaction, SendOption.Reliable);
                    writer.Write(ally.Player.PlayerId);
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

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew))
                {
                    if (!player.Is(RoleEnum.Altruist))
                        gaTargets.Add(player);

                    if (Objectifier.GetObjectifier(player) != null)
                        goodRecruits.Add(player);

                    if (!player.Is(RoleAlignment.CrewSov) && !player.Is(ObjectifierEnum.Traitor))
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Neutral))
                {
                    if (!player.Is(RoleEnum.Executioner) && !player.Is(RoleEnum.Troll) && !player.Is(RoleEnum.GuardianAngel) && !player.Is(RoleEnum.Jester))
                    {
                        gaTargets.Add(player);

                        if (player.Is(RoleAlignment.NeutralKill) && Objectifier.GetObjectifier(player) != null)
                            evilRecruits.Add(player);
                    }

                    if (CustomGameOptions.ExeCanHaveNeutralTargets)
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                {
                    gaTargets.Add(player);

                    if (Objectifier.GetObjectifier(player) != null)
                        evilRecruits.Add(player);

                    if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                        exeTargets.Add(player);
                }

                guessTargets.Add(player);
                bhTargets.Add(player);
            }

            Utils.LogSomething("Targets Set");

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                foreach (var exe in Role.GetRoles(RoleEnum.Executioner).Cast<Executioner>())
                {
                    exe.TargetPlayer = null;

                    if (exeTargets.Count > 0)
                    {
                        while (exe.TargetPlayer == null || exe.TargetPlayer == exe.Player)
                        {
                            exeTargets.Shuffle();
                            var exeNum = Random.RandomRangeInt(0, exeTargets.Count);
                            exe.TargetPlayer = exeTargets[exeNum];
                        }

                        exeTargets.Remove(exe.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTarget, SendOption.Reliable);
                        writer.Write(exe.Player.PlayerId);
                        writer.Write(exe.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Exe Target Set");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                foreach (var guess in Role.GetRoles(RoleEnum.Guesser).Cast<Guesser>())
                {
                    guess.TargetPlayer = null;

                    if (guessTargets.Count > 0)
                    {
                        while (guess.TargetPlayer == null || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(ModifierEnum.Indomitable))
                        {
                            guessTargets.Shuffle();
                            var guessNum = Random.RandomRangeInt(0, guessTargets.Count);
                            guess.TargetPlayer = guessTargets[guessNum];
                        }

                        guessTargets.Remove(guess.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGuessTarget, SendOption.Reliable);
                        writer.Write(guess.Player.PlayerId);
                        writer.Write(guess.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Guess Target Set");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                foreach (var ga in Role.GetRoles(RoleEnum.GuardianAngel).Cast<GuardianAngel>())
                {
                    ga.TargetPlayer = null;

                    if (gaTargets.Count > 0)
                    {
                        while (ga.TargetPlayer == null || ga.TargetPlayer == ga.Player)
                        {
                            gaTargets.Shuffle();
                            var gaNum = Random.RandomRangeInt(0, gaTargets.Count);
                            ga.TargetPlayer = gaTargets[gaNum];
                        }

                        gaTargets.Remove(ga.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGATarget, SendOption.Reliable);
                        writer.Write(ga.Player.PlayerId);
                        writer.Write(ga.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"GA Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("GA Target Set");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                foreach (var bh in Role.GetRoles(RoleEnum.BountyHunter).Cast<BountyHunter>())
                {
                    bh.TargetPlayer = null;

                    if (bhTargets.Count > 0)
                    {
                        while (bh.TargetPlayer == null || bh.TargetPlayer == bh.Player)
                        {
                            bhTargets.Shuffle();
                            var bhNum = Random.RandomRangeInt(0, gaTargets.Count);
                            bh.TargetPlayer = bhTargets[bhNum];
                        }

                        bhTargets.Remove(bh.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBHTarget, SendOption.Reliable);
                        writer.Write(bh.Player.PlayerId);
                        writer.Write(bh.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"BH Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("BH Target Set");
            }

            if (CustomGameOptions.ActorOn > 0)
            {
                foreach (var act in Role.GetRoles(RoleEnum.Actor).Cast<Actor>())
                {
                    act.PretendRoles = InspectorResults.None;

                    while (act.PretendRoles == InspectorResults.None)
                    {
                        var actNum = Random.RandomRangeInt(0, 17);
                        act.PretendRoles = (InspectorResults)actNum;
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetActPretendList, SendOption.Reliable);
                    writer.Write(act.Player.PlayerId);
                    writer.Write((byte)act.PretendRoles);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                Utils.LogSomething("Act Variables Set");
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                foreach (var jackal in Role.GetRoles(RoleEnum.Jackal).Cast<Jackal>())
                {
                    jackal.GoodRecruit = null;
                    jackal.EvilRecruit = null;
                    jackal.BackupRecruit = null;

                    if (goodRecruits.Count > 0)
                    {
                        while (jackal.GoodRecruit == null || jackal.GoodRecruit == jackal.Player)
                        {
                            goodRecruits.Shuffle();
                            var goodNum = Random.RandomRangeInt(0, goodRecruits.Count - 1);
                            jackal.GoodRecruit = goodRecruits[goodNum];
                        }

                        goodRecruits.Remove(jackal.GoodRecruit);
                        (Role.GetRole(jackal.GoodRecruit)).SubFaction = SubFaction.Cabal;
                        Role.GetRole(jackal.GoodRecruit).IsRecruit = true;
                        jackal.Recruited.Add(jackal.GoodRecruit.PlayerId);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGoodRecruit, SendOption.Reliable);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.GoodRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Good Recruit = {jackal.GoodRecruit.name}");
                    }

                    if (evilRecruits.Count > 0)
                    {
                        while (jackal.EvilRecruit == null || jackal.EvilRecruit == jackal.Player)
                        {
                            evilRecruits.Shuffle();
                            var evilNum = Random.RandomRangeInt(0, evilRecruits.Count - 1);
                            jackal.EvilRecruit = evilRecruits[evilNum];
                        }

                        evilRecruits.Remove(jackal.EvilRecruit);
                        Role.GetRole(jackal.EvilRecruit).SubFaction = SubFaction.Cabal;
                        (Role.GetRole(jackal.EvilRecruit)).IsRecruit = true;
                        jackal.Recruited.Add(jackal.EvilRecruit.PlayerId);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetEvilRecruit, SendOption.Reliable);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.EvilRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Evil Recruit = {jackal.EvilRecruit.name}");
                    }
                }

                Utils.LogSomething("Jackal Recruits Set");
            }
        }

        public static void ResetEverything()
        {
            Role.NobodyWins = false;
            Objectifier.NobodyWins = false;

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

            Role.SyndicateHasChaosDrive = false;
            Role.ChaosDriveMeetingTimerCount = 0;

            MeetingPatches.MeetingCount = 0;

            RecordRewind.points.Clear();
            Murder.KilledPlayers.Clear();

            Role.Buttons.Clear();
            Role.SetColors();

            UpdateNames.PlayerNames.Clear();

            MiscPatches.ExileControllerPatch.lastExiled = null;

            Role.HiddenBodies.Clear();
            Role.AllVents.Clear();
            Role.AllVents.AddRange(Object.FindObjectsOfType<Vent>());

            Role.RoleDictionary.Clear();
            Objectifier.ObjectifierDictionary.Clear();
            Modifier.ModifierDictionary.Clear();
            Ability.AbilityDictionary.Clear();
        }

        public static void BeginRoleGen(List<GameData.PlayerInfo> infected)
        {
            if (!ConstantVariables.IsHnS)
            {
                if (ConstantVariables.IsKilling)
                    GenKilling(infected.ToList());
                else if (ConstantVariables.IsVanilla)
                    GenVanilla(infected.ToList());
                else
                    GenClassicCustomAA(infected.ToList());

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
            }
        }
    }
}